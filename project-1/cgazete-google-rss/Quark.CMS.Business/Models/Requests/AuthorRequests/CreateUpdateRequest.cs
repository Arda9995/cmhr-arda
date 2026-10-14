using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.AuthorRequests
{
    public class CreateUpdateRequest
    {
        public int ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public IFormFile ProfilePhoto { get; set; }
        public string ClichePhotoUrl { get; set; }
        public IFormFile ClichePhoto { get; set; }
        public bool IsEnabled { get; set; }
        public bool ShowDailyColumnArticles { get; set; }
        public string Email { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public List<int> SelectedCategoryIDs { get; set; }
        public int Order { get; set; }
        public bool ShowInHome { get; set; }
        public string Url { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string About { get; set; }
        public string ColumnName { get; set; }
        public string Breadcrumb { get; set; }
        public string FacebookUrl { get; set; }
        public string XUrl { get; set; }
        public string YoutubeUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string LinkedinUrl { get; set; }
        public string BaseURL { get; set; }
    }
    public class CreateUpdateRequestValidator : AbstractValidator<CreateUpdateRequest>
    {
        public CreateUpdateRequestValidator(IStringLocalizer<CommonResource> _localizer)
        {
            RuleFor(x => x.Firstname)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(50).WithMessage(_localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.Lastname)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(50).WithMessage(_localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.Email)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(100).WithMessage(_localizer["ValidationForMaxLength"]);

            RuleFor(x => x.SelectedCategoryIDs)
                .NotNull().WithMessage(_localizer["ValidationForRequired"]);
            
            RuleFor(x => x.Url)
                .NotNull().WithMessage(_localizer["ValidationForRequired"])
                .MaximumLength(500).WithMessage(_localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.SeoTitle)
                .MaximumLength(500).WithMessage(_localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.ColumnName)
                .MaximumLength(100).WithMessage(_localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.Breadcrumb)
                .MaximumLength(100).WithMessage(_localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.FacebookUrl)
                .MaximumLength(500).WithMessage(_localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.YoutubeUrl)
                .MaximumLength(500).WithMessage(_localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.XUrl)
                .MaximumLength(500).WithMessage(_localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.LinkedinUrl)
                .MaximumLength(500).WithMessage(_localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.InstagramUrl)
                .MaximumLength(500).WithMessage(_localizer["ValidationForMaxLength"]);
        }
    }
}
