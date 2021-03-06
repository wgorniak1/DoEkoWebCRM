﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewModels.UserViewModel
{
    public class UserDetailsViewModel
    {
        [Required]
        public string UserID { get; set; }

        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [StringLength(30)]
        [Display(Description = "", Name = "Imię", ShortName = "Imię")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [StringLength(30)]
        [Display(Description = "", Name = "Nazwisko", ShortName = "Nazwisko")]
        public string LastName { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "{0} musi mieć przynajmniej {2} i maksymalnie {1} znaków.", MinimumLength = 5)]
        [Display(Name = "Nazwa użytkownika")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [DisplayFormat(HtmlEncode = true, NullDisplayText = "Nie ustawiono!")]
        [Display(Name = "Adres e-mail")]
        public string Email { get; set; }

        //[Required]
        //[StringLength(16, ErrorMessage = "{0} musi mieć przynajmniej {2} i maksymalnie {1} znaków", MinimumLength = 8)]
        //[DataType(DataType.Password)]
        //[Display(Name = "Hasło")]
        //public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Powtórz hasło")]
        //[Compare(nameof(Password), ErrorMessage = "{0} nie jest identyczne z Hasło.")]
        //public string ConfirmPassword { get; set; }

        //[Required]
        //[Display(Name = "Przypisany do roli")]
        //public IList<string> RoleNames { get; set; }

        [Required]
        [Display(Name = "Przypisany do roli")]
        public string RoleId { get; set; }
    }
}
