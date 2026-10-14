using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quark.Data.Models.Common
{
    [Table("Common.EntityTypes")]
    public class EntityType
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public enum EntityTypeEnum
    {
        News = 1,
        Authors = 2,
        Categories = 3,
        Menus = 4,
        Pages = 5,
        Books = 6,
        Tags = 7,
        Sources = 8,
        Unknown = 99,
    }
}
