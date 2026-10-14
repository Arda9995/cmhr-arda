using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using AvvaMobile.Core.Utilities.FtpUpload;
using AvvaMobile.Core.Utilities.PushNotification;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.MemberRequests;
using Quark.CMS.Business.Models.Responses.MemberResponses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;
using AvvaMobile.Core.Caching;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class MemberService : BaseService, IMemberService
{
    private readonly FileUpload _fileUpload;

    private readonly IPushNotificationService _pushNotificationService;

    public MemberService(QuarkDataContext context, IHttpContextAccessor httpContext, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, FileUpload fileUpload, IPushNotificationService pushNotificationService, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _fileUpload = fileUpload;
        _pushNotificationService = pushNotificationService;
    }

    public ServiceResult<ListFiltersRequest> GetAllPagedFiltersAsync()
    {
        var result = new ServiceResult<ListFiltersRequest>();

        var filters = new ListFiltersRequest
        {
            StartDate = DateTime.Now.AddMonths(-12).ToFormattedDate(),
            EndDate = DateTime.Now.AddDays(1).ToFormattedDate(),
            IsEnabled = true,
            IsDeleted = false,
        };

        result.Data = filters;
        return result;
    }

    public async Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPagedAsync(BaseDataTableRequest tableRequest, ListFiltersRequest request)
    {
        var response = new BaseDataTableResponse<List<ListResponseItem>>();

        string enabledText = _localizer["Enabled"];
        string disabledText = _localizer["Disabled"];

        var startDate = request.StartDate.DateTimeParseExact();
        var endDate = request.EndDate.DateTimeParseExact();

        response.RecordsTotal = _context.Members.Count();

        var query = (from cus in _context.Members
                     where cus.CreatedDate > startDate
                           && cus.CreatedDate < endDate
                           && cus.IsEnabled == request.IsEnabled
                           && cus.IsDeleted == request.IsDeleted
                     orderby cus.ID
                     select new
                     {
                         ID = cus.ID,
                         Firstname = cus.Firstname,
                         Lastname = cus.Lastname,
                         MobilePhone = cus.MobilePhone,
                         Email = cus.Email,
                         CreatedDate = cus.CreatedDate,
                         IsEnabled = cus.IsEnabled,
                         IsEnabledText = cus.IsEnabled ? enabledText : disabledText
                     }).AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x =>
                x.Firstname.ToLower().Contains(tableRequest.SearchValue.ToLower()) ||
                x.Lastname.ToLower().Contains(tableRequest.SearchValue.ToLower()) ||
                x.Email.ToLower().Contains(tableRequest.SearchValue.ToLower()) ||
                x.MobilePhone.ToLower().Contains(tableRequest.SearchValue.ToLower()));
        }

        response.RecordsFiltered = query.Count();

        query = query.SortSkipTake(tableRequest);

        response.Data = await query.Select(x => new ListResponseItem
        {
            ID = x.ID,
            Firstname = x.Firstname,
            Lastname = x.Lastname,
            MobilePhone = x.MobilePhone.ToPhoneNumber(),
            Email = x.Email,
            CreatedDate = x.CreatedDate.ToFormattedDateTime(),
            IsEnabled = x.IsEnabled,
            IsEnabledText = x.IsEnabledText
        }).ToListAsync();

        return response;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> CreateAsync(CreateUpdateRequest request)
    {
        var result = new ServiceResult<CreateUpdateRequest> { Data = request };

        //request.MobilePhone = request.MobilePhone.ClearPhoneNumber();

        //var existedCustomer = await _context.Members.Where(x => x.Email == request.Email.Trim() || x.MobilePhone == request.MobilePhone).FirstOrDefaultAsync();

        //if (existedCustomer != null)
        //{
        //    result.SetError(_localizer["UserEmailOrMobilePhoneExist"]);
        //    return result;
        //}

        //if (request.ProfilePhoto != null)
        //{
        //    var uploadResult = _fileUpload.Upload(request.ProfilePhoto, _appSettingsKeys.CDN_CustomersImageFolder);
        //    if (uploadResult.IsSuccess)
        //    {
        //        request.ProfilePhotoUrl = uploadResult.FileName;
        //    }
        //}

        //var customer = new Member
        //{
        //    Firstname = request.Firstname.Trim(),
        //    Lastname = request.Lastname.Trim(),
        //    Email = request.Email.Trim(),
        //    MobilePhone = request.MobilePhone,
        //    IsEnabled = true,
        //    ProfilePhotoUrl = request.ProfilePhotoUrl,
        //    StaticSmsCode = request.StaticSmsCode,
        //};

        //if (request.StaticSmsCode)
        //{
        //    customer.StaticSmsCode = true;
        //    customer.SmsCode = request.SmsCode;
        //    customer.SmsCodeExpireDate = DateTime.Now.AddYears(1);
        //}

        //await _context.Members.AddAsync(customer);
        //await _context.SaveChangesAsync();

        //result.Data.ID = customer.ID;

        result.SetSuccess(_localizer["RecordAdded"]);
        return result;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> UpdateAsync(int id)
    {
        var result = new ServiceResult<CreateUpdateRequest>();

        var customer = await (from us in _context.Members
                              where us.ID == id
                              select new CreateUpdateRequest
                              {
                                  ID = us.ID,
                                  Firstname = us.Firstname,
                                  Lastname = us.Lastname,
                                  Email = us.Email,
                                  MobilePhone = us.MobilePhone.ToPhoneNumber(),
                                  ProfilePhotoUrl = us.ProfilePhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_CustomersImageFolder),
                                  IsEnabled = us.IsEnabled,
                              }).FirstOrDefaultAsync();
        if (customer is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        result.Data = customer;
        return result;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> UpdateAsync(CreateUpdateRequest request)
    {
        var result = new ServiceResult<CreateUpdateRequest> { Data = request };

        request.MobilePhone = request.MobilePhone.ClearPhoneNumber();

        var phoneNumberExist = await _context.Members.AnyAsync(x => x.Email == request.Email && x.ID != request.ID);
        if (phoneNumberExist)
        {
            result.SetError(_localizer["UserEmailOrMobilePhoneExist"]);
            return result;
        }

        var customer = await _context.Members.FirstOrDefaultAsync(x => x.ID == request.ID);
        if (customer is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        if (request.ProfilePhoto != null)
        {
            var uploadResult = _fileUpload.Upload(request.ProfilePhoto, _appSettingsKeys.CDN_CustomersImageFolder);
            if (uploadResult.IsSuccess)
            {
                customer.ProfilePhotoUrl = uploadResult.FileName;
            }
        }

        customer.Firstname = request.Firstname;
        customer.Lastname = request.Lastname;
        customer.Email = request.Email;
        customer.MobilePhone = request.MobilePhone;
        customer.IsEnabled = request.IsEnabled;

        _context.Members.Update(customer);
        await _context.SaveChangesAsync();

        result.SetSuccess(_localizer["RecordUpdated"]);
        return result;
    }

    public async Task<ServiceResult<DetailResponse>> DetailsAsync(int id)
    {
        var result = new ServiceResult<DetailResponse>();

        var customer = await (from cus in _context.Members
                              where cus.ID == id
                              select new DetailResponse
                              {
                                  ID = cus.ID,
                                  Firstname = cus.Firstname,
                                  Lastname = cus.Lastname,
                                  Email = cus.Email,
                                  MobilePhone = cus.MobilePhone.ToPhoneNumber(),
                                  ProfilePhotoUrl = cus.ProfilePhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_CustomersImageFolder),
                                  IsEnabled = cus.IsEnabled
                              }).FirstOrDefaultAsync();
        if (customer is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }
        result.Data = customer;
        return result;
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var result = new ServiceResult();

        var customer = await _context.Members.FirstOrDefaultAsync(x => x.ID == id);
        if (customer is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        _context.Members.Remove(customer);
        await _context.SaveChangesAsync();

        result.SetSuccess(_localizer["RecordDeleted"]);
        return result;
    }

    public async Task<ServiceResult> SendPushNotification(SendPushNotificationRequest request)
    {
        var result = new ServiceResult();

        var customers = await (from cus in _context.Members
                               where (request.CustomerID == null || cus.ID == request.CustomerID)
                                     && cus.IsEnabled
                                     && !cus.IsDeleted
                               select new { CustomerID = cus.ID, CustomerPushID = cus.MobileDevicePushID }).ToListAsync();
        var customerPushIDs = customers.Select(x => x.CustomerPushID).ToList();

        if (request.Image != null)
        {
            var uploadResult = _fileUpload.Upload(request.Image, _appSettingsKeys.CDN_NotificationImagesFolder);
            if (uploadResult.IsSuccess)
            {
                request.ImageUrl = uploadResult.FileName;
            }
        }

        var pushNotificationResult = await _pushNotificationService.SendAsync(customerPushIDs, request.Title, request.Message, request.ImageUrl);
        if (!pushNotificationResult.IsSuccess)
        {
            result.SetSuccess(_localizer["PushNotificationNotSended"]);
            return result;
        }
        else
        {
            //foreach (var customer in customers)
            //{
            //    var notification = new Notification
            //    {
            //        MemberID = customer.CustomerID,
            //        CreatedDate = DateTime.Now,
            //        NotificationSenderID = null,
            //        IsRead = false,
            //        Title = request.Title,
            //        Message = request.Message,
            //        ImageUrl = request.ImageUrl
            //    };
            //    _context.Notifications.Add(notification);
            //}
            //await _context.SaveChangesAsync();
        }
        result.SetSuccess(_localizer["PushNotificationSended"]);
        return result;
    }

    public async Task<BaseDataTableResponse<List<ListMemberNotificationsResponse>>> GetNotificationsPagedAsync(BaseDataTableRequest tableRequest, int id)
    {
        var response = new BaseDataTableResponse<List<ListMemberNotificationsResponse>>();

        var currentLanguageID = GetCurrentLanguageID(_localizer);
        string yesText = _localizer["Yes"];
        string noText = _localizer["No"];

        response.RecordsTotal = await _context.Notifications.CountAsync(x => x.MemberID == id);

        var query = (from nt in _context.Notifications
                     let sender = _context.Members.Where(x => x.ID == nt.NotificationSenderID).FirstOrDefault()
                     where nt.MemberID == id
                     select new
                     {
                         SenderID = sender == null ? 0 : sender.ID,
                         SenderName = sender == null ? _appSettingsKeys.Common_AppName : sender.FullName,
                         Title = nt.Title,
                         Message = nt.Message,
                         ImageUrl = nt.ImageUrl,
                         IsRead = nt.IsRead,
                         CreatedDate = nt.CreatedDate,
                         IsReadText = nt.IsRead ? yesText : noText,
                     }).AsQueryable();

        response.RecordsFiltered = query.Count();
        query = query.SortSkipTake(tableRequest);

        response.Data = await query.Select(x => new ListMemberNotificationsResponse
        {
            SenderID = x.SenderID == 0 ? null : x.SenderID,
            SenderName = x.SenderName,
            Title = x.Title,
            Message = x.Message,
            ImageUrl = x.ImageUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_NotificationImagesFolder),
            IsRead = x.IsRead,
            CreatedDate = x.CreatedDate.ToFormattedDateTime(),
            IsReadText = x.IsReadText,
        }).ToListAsync();

        return response;
    }

    public async Task<BaseDataTableResponse<List<ListMemberCreditCardsResponse>>> GetCreditCardsPagedAsync(BaseDataTableRequest tableRequest, int id)
    {
        var response = new BaseDataTableResponse<List<ListMemberCreditCardsResponse>>();

        response.RecordsTotal = await _context.CreditCards.CountAsync(x => x.MemberID == id);

        var query = (from cc in _context.CreditCards
                     where cc.MemberID == id
                     select new
                     {
                         ID = cc.ID,
                         CreatedDate = cc.CreatedDate,
                         Name = cc.Name,
                         CardHolderName = cc.CardHolderName,
                         CardNo = cc.CardNo,
                         ExpirationMonth = cc.ExpirationMonth,
                         ExpirationYear = cc.ExpirationYear
                     }).AsQueryable();

        query = query.SortSkipTake(tableRequest);

        response.RecordsFiltered = query.Count();

        response.Data = await query.Select(x => new ListMemberCreditCardsResponse
        {
            ID = x.ID,
            CreatedDate = x.CreatedDate.ToFormattedDateTime(),
            Name = x.Name,
            CardHolderName = x.CardHolderName,
            CardNo = x.CardNo.CreditCardMaskify(),
            ExpirationMonth = x.ExpirationMonth,
            ExpirationYear = x.ExpirationYear
        }).ToListAsync();

        return response;
    }
}