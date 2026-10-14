using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using AvvaMobile.Core.Utilities.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.AnomalyRequests;
using Quark.CMS.Business.Models.Responses.AnomalyResponses;
using Quark.Data;
using Quark.Data.Models.News;
using Quark.Data.Models.Anomaly;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class AnomalyService : BaseService, IAnomalyService
{
    private readonly IMailService _mailService;
    private readonly IAnomalyCachingService _anomalyCachingService;
    private readonly IUserCachingService _userCachingService;

    public AnomalyService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, IMailService mailService, IAnomalyCachingService anomalyCachingService, IUserCachingService userCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _mailService = mailService;
        _anomalyCachingService = anomalyCachingService;
        _userCachingService = userCachingService;
    }

    public async Task<ServiceResult<ListFiltersRequest>> GetAllPagedFilters()
    {
        var result = new ServiceResult<ListFiltersRequest>();
        result.Data = new ListFiltersRequest();

        var dataTypesList = await _anomalyCachingService.AnomalyDataTypesAsSelectListItems();
        result.Data.DataTypes = dataTypesList.Data;
        result.Data.DataTypes.AddALLOption();

        var issueTypesList = await _anomalyCachingService.AnomalyIssueTypesAsSelectListItems();
        result.Data.IssueTypes = issueTypesList.Data;
        result.Data.IssueTypes.AddALLOption();

        var usersList = await _userCachingService.Users();
        result.Data.Users = usersList.Data;
        result.Data.Users.AddALLOption();

        result.Data.OnlyShowUnResolved = true;

        return result;
    }

    public async Task<BaseDataTableResponse<List<ListResponseAnomalyItem>>> GetAllPaged(BaseDataTableRequest tableRequest, ListFiltersRequest request)
    {
        var result = new BaseDataTableResponse<List<ListResponseAnomalyItem>>();

        result.RecordsTotal = await _context.Anomalies.CountAsync();

        string solvedText = _localizer["Solved"];
        string unSolvedText = _localizer["UnSolved"];

        var query = (from a in _context.Anomalies
                     join d in _context.AnomalyDataTypes on a.DataTypeID equals d.ID
                     join i in _context.AnomalyIssueTypes on a.IssueTypeID equals i.ID
                     join u in _context.Users on a.CreatedBy equals u.ID
                     where (request.SelectedDataTypeID.IsALLSelected() || request.SelectedDataTypeID == d.ID)
                           && (request.SelectedIssueTypeID.IsALLSelected() || request.SelectedIssueTypeID == i.ID)
                           && (request.SelectedUserID.IsALLSelected() || request.SelectedUserID == u.ID)
                     orderby a.ID descending
                     select new
                     {
                         ID = a.ID,
                         CreatedDate = a.CreatedDate,
                         DataURL = d.Endpoint + a.DataID,
                         Issue = i.Name,
                         IsSolved = a.IsSolved,
                         IsSolvedText = a.IsSolved ? solvedText : unSolvedText,
                         DataType = d.Name,
                         CreatedBy = u.FullName,
                         CreatedBySearch = u.Firstname
                     }).AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x =>
                x.DataURL.ToUpper().Contains(tableRequest.SearchValue.ToUpper()) ||
                x.Issue.ToUpper().Contains(tableRequest.SearchValue.ToUpper()) ||
                x.DataType.ToUpper().Contains(tableRequest.SearchValue.ToUpper()) ||
                x.CreatedBySearch.ToUpper().Contains(tableRequest.SearchValue.ToUpper()));
        }

        if (request.OnlyShowUnResolved)
        {
            query = query.Where(x => x.IsSolved == false);
        }

        result.RecordsFiltered = await query.CountAsync();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(x => new ListResponseAnomalyItem
        {
            ID = x.ID,
            CreatedDate = x.CreatedDate.ToFormattedDateTime(),
            DataURL = x.DataURL,
            Issue = x.Issue,
            IsSolved = x.IsSolved,
            IsSolvedText = x.IsSolvedText,
            DataType = x.DataType
        }).ToListAsync();

        return result;
    }

    public async Task<ServiceResult<DetailsResponse>> Details(int id)
    {
        var result = new ServiceResult<DetailsResponse>();

        var details = await (from a in _context.Anomalies
                             join d in _context.AnomalyDataTypes on a.DataTypeID equals d.ID
                             join i in _context.AnomalyIssueTypes on a.IssueTypeID equals i.ID
                             join u in _context.Users on a.CreatedBy equals u.ID
                             where a.ID == id
                             select new DetailsResponse
                             {
                                 ID = a.ID,
                                 CreatedDate = a.CreatedDate.ToFormattedDateTime(),
                                 DataURL = d.Endpoint + a.DataID,
                                 IsSolved = a.IsSolved,
                                 Issue = i.Name,
                                 Description = a.Description,
                                 CreatedBy = u.FullName,
                                 DataType = d.Name
                             }).FirstOrDefaultAsync();

        if (details is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        result.Data = details;

        return result;
    }

    public async Task<ServiceResult> MarkAsResolved(int id)
    {
        var result = new ServiceResult();

        var anomaly = await _context.Anomalies.FirstOrDefaultAsync(x => x.ID == id && x.IsSolved == false);

        if (anomaly is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        anomaly.IsSolved = true;

        _context.Anomalies.Update(anomaly);
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<ServiceResult> CheckNewsAnomaly(News news)
    {
        var result = new ServiceResult();
        
        await CheckDate(news);
        if (news.Content != null)
        {
            await CheckLink(news);
        }
        
        return result;
    }

    private async Task CheckDate(News news)
    {
        if (news.CreatedDate.AddDays(_appSettingsKeys.Anomaly_News_Date) < DateTime.Now)
        {
            var mails = await (from u in _context.Users
                join r in _context.RoleClaims on u.RoleID equals r.RoleID
                where r.Key == SecurityClaimKeys.ANOMALY && u.RoleID != 12 //yazılım geliştiriciler
                select u.Email).ToListAsync();
                

            var user = await _context.Users.FirstOrDefaultAsync(x => x.ID == CurrentUserID);
            var msg = "Haber oluşturulma tarihinden " + _appSettingsKeys.Anomaly_News_Date +
                      " gün sonra işlem yapıldı."; 

            var Anomaly = new Anomaly
            {
                CreatedDate = DateTime.Now,
                CreatedBy = CurrentUserID,
                DataTypeID = (int)AnomalyDataTypeEnum.News,
                DataID = news.ID,
                IssueTypeID = (int)AnomalyIssueTypeEnum.Date,
                IsSolved = false,
                Description = msg
            };

            var parameters = new Dictionary<string, string>
            {
                { "[FullName]", user.FullName },
                {"[CreatedDate]", Anomaly.CreatedDate.ToFormattedDateTime()},
                {"[Description]", Anomaly.Description},
                {"[Url]", _appSettingsKeys.Common_CMSAppUrl + "/News/Details/" + news.ID},
                {"[ClientName]", _appSettingsKeys.Common_ClientName},
                {"[ClientWebSite]", _appSettingsKeys.Common_ClientWebSite},
                {"[ClientLogo]", "Client Logo Eklenecek"}, 
            };

            await _mailService.Send(mails, "Bir Anomali Oluştu", "Anomaly", parameters); 

            await _context.Anomalies.AddAsync(Anomaly);
            await _context.SaveChangesAsync();
        }

    }

    private async Task CheckLink(News news)
    {
        if (news.Content.Contains("<a "))
        {
            var mails = await (from u in _context.Users
                join r in _context.RoleClaims on u.RoleID equals r.RoleID
                where r.Key == SecurityClaimKeys.ANOMALY && u.RoleID != 12 //yazılım geliştiriciler
                               select u.Email).ToListAsync();
            
            var user = await _context.Users.FirstOrDefaultAsync(x => x.ID == CurrentUserID);
            var msg = "Haber içeriğinde link bulunuyor."; //TODO: Localization

            var Anomaly = new Anomaly
            {
                CreatedDate = DateTime.Now,
                CreatedBy = CurrentUserID,
                DataTypeID = (int)AnomalyDataTypeEnum.News,
                DataID = news.ID,
                IssueTypeID = (int)AnomalyIssueTypeEnum.Link,
                IsSolved = false,
                Description = msg
            };

            var parameters = new Dictionary<string, string>
            {
                {"[FullName]", user.FullName },
                {"[CreatedDate]", Anomaly.CreatedDate.ToFormattedDateTime()},
                {"[Description]", Anomaly.Description},
                {"[Url]", _appSettingsKeys.Common_CMSAppUrl + "/News/Details/" + news.ID},
                {"[ClientName]", _appSettingsKeys.Common_ClientName},
                {"[ClientWebSite]", _appSettingsKeys.Common_ClientWebSite},
                {"[ClientLogo]", "Client Logo Eklenecek"}
            };

            await _mailService.Send(mails, "Bir Anomali Oluştu", "Anomaly", parameters);

            await _context.Anomalies.AddAsync(Anomaly);
            await _context.SaveChangesAsync();
        }
    }
}