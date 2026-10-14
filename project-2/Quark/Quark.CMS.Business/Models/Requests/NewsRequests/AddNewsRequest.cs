using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.CMS.Business.Models.Requests.NewsRequests;

public class AddNewsRequest
{
    public int NewsTypeID { get; set; }
    public List<SelectListItem> NewsTypeIDs { get; set; }
    
    public int StatusID { get; set; }
    public List<SelectListItem> StatusIDs { get; set; }
    public int CategoryID { get; set; }
    public List<SelectListItem> CategoryIDs { get; set; }

    public int? AuthorID { get; set; }

    public string Slug { get; set; }

    public string Title { get; set; }
    public string Subtitle { get; set; }
    
    public string Content { get; set; }

    public string Tags { get; set; }
    
    public string HeadlinePhotoURL { get; set; }
    public string UpHeadline { get; set; }
    public string SubHeading { get; set; }
    public string UpSubHeading { get; set; }
    public bool ShowUpHeadline { get; set; }
    public bool ShowSubHeading { get; set; }
    public bool ShowUpSubHeading { get; set; }
    public string RedirectLink { get; set; }


}

public class AddNewsValidator : AbstractValidator<AddNewsRequest>
{
    public AddNewsValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Haber başlığı boş geçilemez.");

        RuleFor(x => x.Subtitle)
            .NotEmpty().WithMessage("Haber alt başlığı boş geçilemez.");
        
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Haber içeriği boş geçilemez.");
        
        RuleFor(x => x.StatusID)
            .NotEmpty().WithMessage("Haber durumu boş geçilemez.");
        
        RuleFor(x => x.NewsTypeID)
            .NotEmpty().WithMessage("Haber tipi boş geçilemez.");

        RuleFor(x => x.UpHeadline)
          .NotEmpty().WithMessage("Upheadline boş geçilemez");

        RuleFor(x => x.SubHeading)
          .NotEmpty().WithMessage("SubHeading boş geçilemez");

        RuleFor(x => x.UpSubHeading)
          .NotEmpty().WithMessage("UpSubHeading boş geçilemez");
    }
}


