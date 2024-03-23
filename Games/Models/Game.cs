using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Games.Models
{
    [Table("Games")]
    public class Game
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; }

        [Display(Name = "User Score")]
        public double UserScore { get; set; }

        [Display(Name = "Number Of User Scores")]
        public int NumberOfUserScores { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string? CoverUrl { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ReleaseDate { get; set; }

        [StringLength(2000)]
        public string Summary { get; set; }

        [Display(Name = "Story Line")]
        [StringLength(2000)]
        public string StoryLine { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int? CollectionId { get; set; }

        [HiddenInput(DisplayValue = false)]
        public long? OldId { get; set; }

        public Collection? Collection { get; set; }
        public ICollection<Company>? Companies { get; set; }
        public ICollection<GameMode>? GameModes { get; set; }
        public ICollection<Genre>? Genres { get; set; }
        public ICollection<Keyword>? Keywords { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
