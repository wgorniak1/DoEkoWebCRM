using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using DoEko.Models.DoEko;

namespace DoEko.Models.Validation
{


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AddressAttribute : ValidationAttribute
    {
        private readonly DoEkoContext _context;


        public AddressAttribute(DoEkoContext context)
        {
            _context = context;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            

            return base.IsValid(value, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            return base.FormatErrorMessage(name);
        }
    }
}
