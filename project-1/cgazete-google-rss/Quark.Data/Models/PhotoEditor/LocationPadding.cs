using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.PhotoEditor;

[Table("PhotoEditor.LocationPaddings")]
public class LocationPadding
{
    public int ID { get; set; }
    public string Name { get; set; }
    public bool IsUpper { get; set; }
    public bool IsCenter { get; set; }
    public bool IsStartLeft { get; set; }
    public int Padding { get; set; }
}