using System.ComponentModel.DataAnnotations;

namespace Games.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string UserId { get; set; }

        public string UserEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
    }
}
