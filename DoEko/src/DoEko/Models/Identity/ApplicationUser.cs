using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoEko.Models.Identity
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [StringLength(30)]
        [Display(Description = "", Name = "Imię", ShortName = "Imię")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [StringLength(30)]
        [Display(Description = "", Name = "Nazwisko", ShortName = "Nazwisko")]
        public string LastName { get; set; }

        public DateTime PasswordChangedOn { get; set; }

        public static ApplicationUser DummyUser() => new ApplicationUser
        {
            Email = "brak",
            UserName = "brak",
            FirstName = "Nieustawiono",
            LastName = ""
        };

        [NotMapped]
        public bool PasswordExpired {
            get {
                var period = DateTime.UtcNow - this.PasswordChangedOn;
                return ( period.Days > 30 );
            }
            private set { }
        }

        [NotMapped]
        public string FullName
        {
            get
            {
                return this.FirstName + ' ' + this.LastName;
            }
            private set { }
        }

        
    }
}
