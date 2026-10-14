using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.BookRequests
{
    public class UpdateBookRequest
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public bool IsEnabled { get; set; }
        public string BaseURL { get; set; }
        public string ImageURL { get; set; }
        public IFormFile Image { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
    }
    public class UpdateRequestValidator : AbstractValidator<UpdateBookRequest>
    {
        public UpdateRequestValidator(IStringLocalizer<CommonResource> _localizer)
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(50).WithMessage(x => _localizer["ValidationForMaxLength"]);

            RuleFor(x => x.Author)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(50).WithMessage(x => _localizer["ValidationForMaxLength"]);
        }
    }
}


