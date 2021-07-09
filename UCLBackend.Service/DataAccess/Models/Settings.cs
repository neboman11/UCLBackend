using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCLBackend.DataAccess.Models
{
    [Table("Settings")]
    public class Setting
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
