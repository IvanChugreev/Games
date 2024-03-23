using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Games.Models
{
    [Table("Companies")]
    public class Company
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string LogoUrl { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [HiddenInput(DisplayValue = false)]
        public long? OldId { get; set; }

        public ICollection<Game> Games { get; set; }
    }
}
