using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DoEko.Models
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class WageTypeConfigurationAttribute : ValidationAttribute
    {
        public WageTypeConfigurationAttribute()
        {
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
