using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewModels.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name ="Login")]
        public string UserName { get; set; }

        //[required]
        //[emailaddress]
        //public string email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="Hasło")]
        public string Password { get; set; }

        [Display(Name = "Zapamiętaj mnie?")]
        public bool RememberMe { get; set; }
    }
}
