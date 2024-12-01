using System.ComponentModel.DataAnnotations.Schema;

namespace ProniaAPK.Areas.Admin.ViewModels
{
    public class CreateSlideVM
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }


        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
