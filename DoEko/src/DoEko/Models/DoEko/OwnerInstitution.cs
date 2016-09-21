using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoEko.Models.DoEko
{
    public class OwnerInstitution : Owner
    {
        [Required]
        [StringLength(30)]
        [Display(Description ="",Name ="Nazwa",ShortName ="Nazwa")]
        public string Name { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Description = "", Name = "Nazwa cd.", ShortName = "Nazwa cd.")]
        public string Name2 { get; set; }

    }
}
