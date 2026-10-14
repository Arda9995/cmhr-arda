namespace Quark.CMS.Business.Models.Requests.PhotoEditor;

public class PrepareEditRequest
{
    public string BackgroundColor { get; set; }
    public int Opacity { get; set; }
    public int TextFontSize { get; set; }
    public string TextColor { get; set; }
}