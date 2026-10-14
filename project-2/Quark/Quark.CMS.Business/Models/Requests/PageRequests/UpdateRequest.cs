using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quark.CMS.Business.Models.Requests.PageRequests
{
    public class UpdateRequest
    {
        public int ID { get; set; } 
        public string Key { get; set; }
        public string Keywords { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Slug { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string Breadcrumb { get; set; }
    }
    public class UpdateRequestValidator : AbstractValidator<UpdateRequest>
    {
        public UpdateRequestValidator(IStringLocalizer<CommonResource> _localizer)
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(200).WithMessage(x => _localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.Key)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(50).WithMessage(x => _localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.Slug)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(500).WithMessage(x => _localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage(x => _localizer["ValidationForMaxLength"]);

            RuleFor(x => x.SeoTitle)
                .MaximumLength(500).WithMessage(x => _localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.SeoDescription)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
            
            RuleFor(x => x.Breadcrumb)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(100).WithMessage(x => _localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.Content)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
        }
    }
}
