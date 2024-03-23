using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Games.Models
{
    [Table("Keywords")]
    public class Keyword
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Name { get; set; }

        [HiddenInput(DisplayValue = false)]
        public long? OldId { get; set; }

        public ICollection<Game> Games { get; set; }
    }
}
