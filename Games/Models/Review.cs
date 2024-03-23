using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Games.Models
{
    [Table("Reviews")]
    public class Review
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int GameId { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string OwnerId { get; set; }
        
        [Required]
        [StringLength(2000)]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        [Range(0, 10)]
        public int RateValue { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public Game Game { get; set; }
        public Account Owner { get; set; }
    }
}
