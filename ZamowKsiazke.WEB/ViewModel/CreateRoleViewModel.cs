﻿﻿using System.ComponentModel.DataAnnotations;

namespace ZamowKsiazke.ViewModel
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Nazwa roli jest wymagana")]
        [Display(Name = "Role")]
        public string RoleName { get; set; } = string.Empty;
    }
}
