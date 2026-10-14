using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("Member.Types")]
public class MemberType
{
    public int ID { get; set; }
    public string Name { get; set; }
}

public enum MemberTypeEnum
{
    Individual = 1,
    Corporate = 2
}