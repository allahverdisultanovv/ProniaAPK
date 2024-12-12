using System.ComponentModel.DataAnnotations;

namespace ProniaAPK.ViewModels
{
    public class LoginVM
    {
        [MaxLength(256)]
        [MinLength(4)]
        public string EmailOrUsername { get; set; }
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; }
        public bool IsPersistant { get; set; }
    }
}
