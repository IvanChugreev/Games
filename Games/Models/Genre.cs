using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Games.Models
{
    [Table("Genres")]
    public class Genre
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }

        [HiddenInput(DisplayValue = false)]
        public long? OldId { get; set; }

        public ICollection<Game> Games { get; set; }
    }
}
