using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace DoEko.Models.Validation
{
    public class RangeFromToAttribute : ValidationAttribute//, IClientModelValidator
    {
        public string FieldNameFrom { get; set; }
        public string FieldNameTo { get; set; }

        public RangeFromToAttribute(string fieldNameFrom, string fieldNameTo)
        {
            this.FieldNameFrom = fieldNameFrom;
            this.FieldNameTo = fieldNameTo;
        }

        public RangeFromToAttribute(string fieldNameFrom, string fieldNameTo, string errorMessage) : base(errorMessage)
        {
            this.FieldNameFrom = fieldNameFrom;
            this.FieldNameTo = fieldNameTo;
        }

        //public void AddValidation(ClientModelValidationContext context)
        //{
        //    if (context == null)
        //    {
        //        throw new ArgumentNullException(nameof(context));
        //    }

        //    MergeAttribute(context.Attributes, "data-val", "true");
        //    MergeAttribute(context.Attributes, "data-val-nip", GetErrorMessage());
        //}

        //private bool MergeAttribute(
        //    IDictionary<string, string> attributes,
        //    string key,
        //    string value)
        //    {
        //        if (attributes.ContainsKey(key))
        //        {
        //            return false;
        //        }
        //        attributes.Add(key, value);
        //        return true;
        //    }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var from = value.GetType().GetProperty(FieldNameFrom).GetValue(value);
                var to = value.GetType().GetProperty(FieldNameTo).GetValue(value);

                if (from.GetType() != to.GetType())
                {
                    return new ValidationResult(GetErrorMessage());
                }

                if (from.GetType() == typeof(DateTime))
                {
                    if ((DateTime)from > (DateTime)to)
                    {
                        return new ValidationResult(GetErrorMessage());
                    }
                }
                else
                {
                    if ((double)from > (double)to)
                    {
                        return new ValidationResult(GetErrorMessage());
                    }
                }
            }
            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            if (string.IsNullOrEmpty(this.ErrorMessageString))
            {
                return "Nieprawidłowy przedział od - do";
            }
            else
            {
                return this.ErrorMessageString;
            }
        }

    }
}
