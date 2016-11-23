using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.Validation
{
    public class PESELAttribute : ValidationAttribute//, IClientModelValidator
    {
        private string _pesel;

        public PESELAttribute()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (!IsPESELValid(value))
                {
                    var members = new List<string>() { validationContext.MemberName };

                    return new ValidationResult(GetErrorMessage(), members);
                }

            }
            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            if (string.IsNullOrEmpty(this.ErrorMessageString))
            {
                return "PESEL jest nieprawidłowy";
            }
            else
            {
                return this.ErrorMessageString;
            }
        }

        private bool IsPESELValid(object value)
        {
            int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };

            _pesel = value.ToString();

            if (_pesel.Length != 11)
            {
                return false;
            }

            int controlSum = 0;
            for (int i = 0; i < 10; i++)
            {
                controlSum += weights[i] * int.Parse(_pesel[i].ToString());
            }

            int controlNum = 10 - controlSum % 10;
            if (controlNum == 10)
            {
                controlNum = 0;
            }

            return (controlNum == int.Parse(_pesel[10].ToString()));
            
        }
    }
}
