using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DoEko.Models.DoEko;

namespace DoEko.Models.Identity
{

    public enum AccessType
    {
        Create = 1,
        Read,
        Update,
        Delete
    }

    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base()
        {
            
            Projects = Projects ?? new List<UserProject>();
            
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [StringLength(30)]
        [Display(Description = "", Name = "Imię", ShortName = "Imię")]
        public string FirstName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "Pole {0} jest obowiązkowe")]
        [StringLength(30)]
        [Display(Description = "", Name = "Nazwisko", ShortName = "Nazwisko")]
        public string LastName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime PasswordChangedOn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<UserProject> Projects { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ApplicationUser DummyUser() => new ApplicationUser
        {
            Email = "brak",
            UserName = "brak",
            FirstName = "Nieustawiono",
            LastName = ""
        };

        [NotMapped]
        public virtual ICollection<int> ProjectIds {
            get {
                return Projects.Count > 0 ? Projects.Select(i => i.ProjectId).ToList() : new List<int>();
            }
            private set { }

        }

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

    public class UserProject
    {
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }

        //[ForeignKey("Project")]
        public int ProjectId { get; set; }

        public virtual ApplicationUser User { get; set; }

        //public virtual Project Project { get; set; }

        public AccessType AccessType { get; set; }
    }


}
