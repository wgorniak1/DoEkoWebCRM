using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace DoEko.Models.Validation
{
    public class NIPAttribute : ValidationAttribute, IClientModelValidator
    {
        private string _nip;

        public NIPAttribute()
        {
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-nip", GetErrorMessage());
        }

        private bool MergeAttribute(
            IDictionary<string, string> attributes,
            string key,
            string value)
            {
                if (attributes.ContainsKey(key))
                {
                    return false;
                }
                attributes.Add(key, value);
                return true;
            }
    

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (!IsNIPValid(value))
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
                return "NIP jest nieprawidłowy";
            }
            else
            {
                return this.ErrorMessageString;
            }
        }

        private bool IsNIPValid(object value)
        {
            byte[] Factors = new byte[9] { 6, 5, 7, 2, 3, 4, 5, 6, 7 };
            byte[] ASCIINumbers = new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57 };
            int CalculatedValue = 0;
            int CheckValue = 0;

            _nip = value.ToString().Replace("-", "").Trim();

            if (_nip.Length != 10)
            {
                return false;
            }

            foreach (char l in _nip)
            {
                if (Array.IndexOf(ASCIINumbers, Convert.ToByte(l)) == -1) return false;
            }

            CheckValue = Convert.ToInt32(_nip[9].ToString());

            for (int i = 0; i < 9; i++)
            {
                CalculatedValue += Factors[i] * Convert.ToInt32(_nip[i].ToString());
            }

            return ((CalculatedValue % 11) == CheckValue);            
        }
    }
}
