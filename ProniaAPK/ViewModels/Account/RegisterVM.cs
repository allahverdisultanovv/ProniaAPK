using System.ComponentModel.DataAnnotations;

namespace ProniaAPK.ViewModels
{
    public class RegisterVM
    {
        [MinLength(3)]
        [MaxLength(25)]
        public string Name { get; set; }
        [MinLength(3)]
        [MaxLength(25)]
        public string SurName { get; set; }
        [MinLength(4)]
        [MaxLength(100)]
        public string Username { get; set; }
        [MaxLength(256)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ComfirmPassword { get; set; }
    }
}
