using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoEko.Models.Payroll
{
    public enum WageTypeUnit
    {
        [Display(Name ="szt.")]
        Item = 1,
        [Display(Name = "godz.")]
        Hour,
        [Display(Name = "dn.")]
        Day,
    }

    [Table(name: "WageTypeCatalog")]
    [WageTypeConfiguration()]
    public class WageTypeDefinition : IWageType
    {
        [Key]
        public int WageTypeDefinitionId { get; set; }
        [Display(Name ="Kod")]
        [MaxLength(4, ErrorMessage = "Maksymalna długość 4")]
        [Required(ErrorMessage ="Pole jest obowiązkowe")]
        public string Code { get; set; }
        [Display(Name = "Znaczenie")]
        [MaxLength(50,ErrorMessage = "Maksymalna długość 50")]
        [Required(ErrorMessage ="Pole jest obowiązkowe")]
        public string ShortDescription { get; set; }
        [Display(Name ="Liczba")]
        public double Number { get; set; }
        [Display(Name ="Jednostka")]
        public WageTypeUnit Unit { get; set; }
        [Display(Name ="Stawka")]
        public double Rate { get; set; }
        public bool RateMandatory { get; set; }
        [Display(Name ="Kwota")]
        public double Amount { get; set; }
        public bool AmountMandatory { get; set; }
        [Display(Name ="Waluta")]
        public string Currency { get; set; }
    }
}
