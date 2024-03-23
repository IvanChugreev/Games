using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Games.Models
{
    [Table("Accounts")]
    public class Account
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Nickname { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ReviewsNumber { get; set; }
    }
}
