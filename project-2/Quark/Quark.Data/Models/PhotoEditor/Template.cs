using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.PhotoEditor;

[Table("PhotoEditor.Templates")]
public class Template
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string BackgroundColor { get; set; }
    public string TextColor { get; set; }
    public int BackgroundOpacity { get; set; }
    public int TextFontSize { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedDatetime { get; set; }
    public bool IsEnabled { get; set; }
}