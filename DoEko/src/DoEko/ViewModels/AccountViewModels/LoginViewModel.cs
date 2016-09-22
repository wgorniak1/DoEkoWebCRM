using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewModels.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [Display(Name ="Konto użytkownika")]
        public string UserName { get; set; }

        //[required]
        //[emailaddress]
        //public string email { get; set; }

        [Required(ErrorMessage = "{0} jest polem obowiązkowym")]
        [DataType(DataType.Password)]
        [Display(Name ="Hasło")]
        public string Password { get; set; }

        [Display(Name = "Zapamiętaj mnie?")]
        public bool RememberMe { get; set; }
    }
}
