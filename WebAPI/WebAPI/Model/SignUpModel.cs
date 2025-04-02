using System.ComponentModel.DataAnnotations;

namespace WebAPI.Model
{
    public class SignUpModel
    {
        [Required]
        public string Firtname { get; set; } = string.Empty;
        [Required]
        public string Lastname { get; set; } = string.Empty;
        [Required,EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password {  get; set; } = string.Empty;
        [Required]
        public string ConfirmPass { get; set; } = string.Empty;

    }
}
