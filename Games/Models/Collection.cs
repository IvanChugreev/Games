using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Games.Models
{
    [Table("Collections")]
    public class Collection
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; }

        [HiddenInput(DisplayValue = false)]
        public long? OldId { get; set; }

        public IEnumerable<Game>? Games { get; set; }
    }
}
