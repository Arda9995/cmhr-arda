using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.MemberRequests
{
    public class SendPushNotificationRequest
    {
        public int? CustomerID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class SendPushNotificationRequestValidator : AbstractValidator<SendPushNotificationRequest>
    {
        public SendPushNotificationRequestValidator(IStringLocalizer<CommonResource> _localizer)
        {
            RuleFor(x => x.Title)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(150).WithMessage(_localizer["ValidationForMaxLength"]);;
            
            RuleFor(x => x.Message)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(500).WithMessage(_localizer["ValidationForMaxLength"]);;
        }
    }
}
