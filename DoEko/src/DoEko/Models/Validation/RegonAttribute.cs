using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Models.DoEko;

namespace DoEko.Models.Validation
{
    public class RegonAttribute : ValidationAttribute//, IClientModelValidator
    {
        private string _regon;

        public RegonAttribute()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            _regon = value.ToString();
            if (!IsRegonValid())
            {
                var members = new List<string>() { validationContext.MemberName };

                return new ValidationResult(GetErrorMessage(), members);
            }
            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            if (string.IsNullOrEmpty(this.ErrorMessageString))
            {
                return "Regon jest nieprawidłowy";
            }
            else
            {
                return this.ErrorMessageString;
            }
        }

        private bool IsRegonValid()
        {
            byte[] weights;
            ulong regon = ulong.MinValue;
            byte[] digits;

            if (ulong.TryParse(_regon, out regon).Equals(false)) return false;

            switch (_regon.Length)
            {
                case 7:
                    weights = new byte[] { 2, 3, 4, 5, 6, 7 };
                    break;

                case 9:
                    weights = new byte[] { 8, 9, 2, 3, 4, 5, 6, 7 };
                    break;

                case 14:
                    weights = new byte[] { 2, 4, 8, 5, 0, 9, 7, 3, 6, 1, 2, 4, 8 };
                    break;

                default:
                    return false;
            }

            string sRegon = regon.ToString();
            digits = new byte[sRegon.Length];

            for (int i = 0; i < sRegon.Length; i++)
            {
                if (byte.TryParse(sRegon[i].ToString(), out digits[i]).Equals(false)) return false;
            }

            int checksum = 0;

            for (int i = 0; i < weights.Length; i++)
            {
                checksum += weights[i] * digits[i];
            }

            return (checksum % 11 % 10).Equals(digits[digits.Length - 1]);

        }
    }
}
