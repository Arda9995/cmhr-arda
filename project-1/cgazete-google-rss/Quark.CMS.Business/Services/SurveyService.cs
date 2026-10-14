using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using AvvaMobile.Core.Utilities.FtpUpload;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.SurveyRequests;
using Quark.CMS.Business.Models.Responses.SurveyResponses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Data.Models.Survey;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class SurveyService : BaseService, ISurveyService
{
    private readonly FileUpload _fileUpload;
    public SurveyService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, FileUpload fileUpload) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _fileUpload = fileUpload;
    }

    public ServiceResult<GetSurveyFiltersRequest> GetAllPagedFilters()
    {
        var result = new ServiceResult<GetSurveyFiltersRequest>();
        var filters = new GetSurveyFiltersRequest();
        result.Data = filters;
        return result;
    }

    public async Task<BaseDataTableResponse<List<ListResponseSurveyItem>>> GetAllPaged(BaseDataTableRequest tableRequest, GetSurveyFiltersRequest request)
    {
        var result = new BaseDataTableResponse<List<ListResponseSurveyItem>>();

        //result.RecordsTotal = await _context.Surveys.CountAsync();
        //string enabledText = _localizer["Enabled"];
        //string disabledText = _localizer["Disabled"];

        //var query = (from s in _context.Surveys
        //             orderby s.ID descending
        //             select new
        //             {
        //                 ID = s.ID,
        //                 IsEnabled = s.IsEnabled,
        //                 IsEnabledText = s.IsEnabled ? enabledText : disabledText,
        //                 Question = s.Question,
        //                 PhotoUrl = s.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_SurveyPhotos),
        //                 TotalAnswers = s.TotalAnswers,
        //                 CreatedDate = s.CreatedDate
        //             }).AsNoTracking().AsQueryable();

        //if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        //{
        //    query = query.Where(x =>
        //        x.Question.Contains(tableRequest.SearchValue));
        //}

        //result.RecordsFiltered = query.Count();

        //query = query.SortSkipTake(tableRequest);

        //result.Data = await query.Select(x => new ListResponseSurveyItem
        //{
        //    ID = x.ID,
        //    IsEnabled = x.IsEnabled,
        //    IsEnabledText = x.IsEnabledText,
        //    Question = x.Question,
        //    PhotoUrl = x.PhotoUrl,
        //    TotalAnswers = x.TotalAnswers,
        //    CreatedDate = x.CreatedDate.ToFormattedDate()
        //}).ToListAsync();

        return result;
    }

    public ServiceResult<CreateSurveyRequest> Create()
    {
        var result = new ServiceResult<CreateSurveyRequest> { Data = new CreateSurveyRequest() };

        return result;
    }

    public async Task<ServiceResult<CreateSurveyRequest>> Create(CreateSurveyRequest request)
    {
        var result = new ServiceResult<CreateSurveyRequest>();

        //var isExist = await _context.Surveys.AnyAsync(x => x.Question == request.Question);
        //if (isExist)
        //{
        //    result.SetError(_localizer["RecordAlreadyExist"]);
        //    return result;
        //}

        //if (request.Photo != null)
        //{
        //    var uploadResult = _fileUpload.Upload(request.Photo, _appSettingsKeys.CDN_SurveyPhotos);
        //    if (uploadResult.IsSuccess)
        //    {
        //        request.PhotoUrl = uploadResult.FileName;
        //    }
        //}

        //var survey = new Survey
        //{   
        //    Question = request.Question,
        //    Option1 = request.Option1,
        //    Option2 = request.Option2,
        //    Option3 = request.Option3,
        //    Option4 = request.Option4,
        //    CreatedDate = DateTime.Now,
        //    PhotoUrl = request.PhotoUrl,
        //    IsEnabled = false,
        //    StartDate = request.StartDate,
        //    EndDate = request.EndDate,
        //    CreatedBy = request.CreatedBy,
        //    Slug = request.Slug,
        //    LastUpdated = null
        //};

        //await _context.Surveys.AddAsync(survey);
        //await _context.SaveChangesAsync();
        //request.ID = survey.ID;
        //if (request.Slug.Contains("{ID}"))
        //{
        //    survey.Slug = request.Slug.Replace("{ID}", survey.ID.ToString());

        //    _context.Surveys.Update(survey);
        //    await _context.SaveChangesAsync();
        //}

        //result.SetSuccess(_localizer["RecordAdded"]);
        return result;
    }

    public async Task<ServiceResult<UpdateSurveyRequest>> Update(int id)
    {
        var result = new ServiceResult<UpdateSurveyRequest>();

        //var survey = await _context.Surveys.FirstOrDefaultAsync(x => x.ID == id);
        //if (survey is null)
        //{
        //    result.SetError(_localizer["RecordNotFound"]);
        //    return result;
        //}

        //var request = new UpdateSurveyRequest
        //{
        //    ID = survey.ID,
        //    Question = survey.Question,
        //    Option1 = survey.Option1,
        //    Option2 = survey.Option2,
        //    Option3 = survey.Option3,
        //    Option4 = survey.Option4,
        //    StartDate = survey.StartDate,
        //    EndDate = survey.EndDate,
        //    CreatedBy = survey.CreatedBy,
        //    Slug = survey.Slug,
        //    PhotoUrl = survey.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_SurveyPhotos)
        //};

        //result.Data = request;
        return result;
    }

    public async Task<ServiceResult<UpdateSurveyRequest>> Update(UpdateSurveyRequest request)
    {
        var result = new ServiceResult<UpdateSurveyRequest> { Data = request };

        //if (request.ID <= 0)
        //{
        //    result.SetError(_localizer["InvalidId"]);
        //    return result;
        //}

        //var survey = await _context.Surveys.FirstOrDefaultAsync(x => x.ID == request.ID);
        //if (survey is null)
        //{
        //    result.SetError(_localizer["RecordNotFound"]);
        //    return result;
        //}

        //if (!string.IsNullOrWhiteSpace(request.Question))
        //{
        //    var isExist = await _context.Surveys.AnyAsync(x => x.ID != request.ID && x.Question == request.Question);
        //    if (isExist)
        //    {
        //        result.SetError(_localizer["RecordAlreadyExist"]);
        //        return result;
        //    }
        //}

        //if (request.Photo != null)
        //{
        //    var uploadResult = _fileUpload.Upload(request.Photo, _appSettingsKeys.CDN_SurveyPhotos);
        //    if (!uploadResult.IsSuccess)
        //    {
        //        result.SetError(_localizer["ImageUploadFailed"]);
        //        return result;
        //    }
        //    survey.PhotoUrl = uploadResult.FileName;
        //}
        //else if (string.IsNullOrEmpty(survey.PhotoUrl))
        //{
        //    result.SetError(_localizer["ImageUploadRequired"]);
        //    return result;
        //}

        //survey.Question = request.Question;
        //survey.Option1 = request.Option1;
        //survey.Option2 = request.Option2;
        //survey.Option3 = request.Option3;
        //survey.Option4 = request.Option4;
        //survey.LastUpdated = DateTime.Now;
        //survey.StartDate = request.StartDate;
        //survey.EndDate = request.EndDate;
        //survey.CreatedBy = request.CreatedBy;
        //survey.Slug = request.Slug;

        //_context.Surveys.Update(survey);
        //var changes = await _context.SaveChangesAsync();
        //if (changes == 0)
        //{
        //    result.SetError(_localizer["UpdateFailed"]);
        //    return result;
        //}

        return result;
    }


    public async Task<ServiceResult<SurveyDetailsResponse>> Details(int id)
    {
        var result = new ServiceResult<SurveyDetailsResponse>();

        //var survey = await _context.Surveys.Where(x => x.ID == id).Select(x => new SurveyDetailsResponse
        //{
        //    ID = x.ID,
        //    Question = x.Question,
        //    Option1 = x.Option1,
        //    Option2 = x.Option2,
        //    Option3 = x.Option3,
        //    Option4 = x.Option4,
        //    CreatedDate = x.CreatedDate.ToFormattedDateTime(),
        //    LastUpdated = x.LastUpdated.ToFormattedDateTime(),
        //    IsEnabled = x.IsEnabled,
        //    TotalAnswers = x.TotalAnswers,
        //    Option1Answers = x.Option1Answers,
        //    Option2Answers = x.Option2Answers,
        //    Option3Answers = x.Option3Answers,
        //    Option4Answers = x.Option4Answers,
        //    CreatedBy = x.CreatedBy,
        //    Slug = x.Slug,
        //    PhotoUrl = x.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_SurveyPhotos),
        //    StartDate = x.StartDate,
        //    EndDate = x.EndDate
        //}).FirstOrDefaultAsync();

        //result.Data = survey;
        return result;
    }

    public async Task<ServiceResult> Delete(int id)
    {
        var result = new ServiceResult();

        var survey = await _context.Surveys.FirstOrDefaultAsync(x => x.ID == id);
        if (survey is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var answers = await _context.Answers.Where(x => x.SurveyID == survey.ID).ToListAsync();
        if (answers.Count > 0 && answers != null)
        {
            _context.Answers.RemoveRange(answers);
        }

        _context.Surveys.Remove(survey);
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<ServiceResult> EnableSurvey(int id)
    {
        var result = new ServiceResult();

        var survey = await _context.Surveys.FirstOrDefaultAsync(x => x.ID == id);
        //var oldSurvey = await _context.Surveys.FirstOrDefaultAsync(x => x.IsEnabled);

        survey.IsEnabled = true;
        //survey.EndDate = null;
        //oldSurvey.IsEnabled = false;
        //oldSurvey.EndDate = DateTime.Now;

        //var oldSurveyAnswers = await _context.Answers.Where(x => x.SurveyID == oldSurvey.ID).ToListAsync();
        //if (oldSurveyAnswers.Count > 0)
        //{
        //    oldSurvey.Option1Answers = oldSurveyAnswers.Count(x => x.Option == 1);
        //    oldSurvey.Option2Answers = oldSurveyAnswers.Count(x => x.Option == 2);
        //    oldSurvey.Option3Answers = oldSurveyAnswers.Count(x => x.Option == 3);
        //    oldSurvey.Option4Answers = oldSurveyAnswers.Count(x => x.Option == 4);
        //    oldSurvey.TotalAnswers = oldSurveyAnswers.Count;
        //    oldSurvey.LastUpdated = DateTime.Now;
        //}

        _context.Surveys.Update(survey);
        //_context.Surveys.Update(oldSurvey);
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<ServiceResult> RefreshAnswers(int id)
    {
        var result = new ServiceResult();

        //var survey = await _context.Surveys.FirstOrDefaultAsync(x => x.ID == id);
        //if (survey != null)
        //{
        //    var answers = await _context.Answers.Where(x => x.SurveyID == id).ToListAsync();
        //    if (answers.Count > 0 && answers != null)
        //    {
        //        survey.Option1Answers = answers.Count(x => x.Option == 1);
        //        survey.Option2Answers = answers.Count(x => x.Option == 2);
        //        survey.Option3Answers = answers.Count(x => x.Option == 3);
        //        survey.Option4Answers = answers.Count(x => x.Option == 4);
        //        survey.TotalAnswers = answers.Count;
        //        survey.LastUpdated = DateTime.Now;

        //        _context.Surveys.Update(survey);
        //        await _context.SaveChangesAsync();
        //    }
        //}

        return result;
    }
}