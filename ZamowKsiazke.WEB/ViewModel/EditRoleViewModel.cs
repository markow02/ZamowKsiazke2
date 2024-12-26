using System.ComponentModel.DataAnnotations;

namespace ZamowKsiazke.ViewModel
{
    public class EditRoleViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Role")]
        [Required(ErrorMessage = "Nazwa roli jest wymagana")]
        public string RoleName { get; set; }
        public List<string> Users { get; set; } = new List<string>();

    }
}
