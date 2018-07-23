using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Internal;

namespace DoEko.Models
{

    public class DoubleModelBinder : IModelBinder
    {
        private readonly IModelBinder _fallbackBinder;

        public DoubleModelBinder(IModelBinder fallbackBinder)
        {
            if (fallbackBinder == null)
                throw new ArgumentNullException(nameof(fallbackBinder));

            _fallbackBinder = fallbackBinder;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult != null && !string.IsNullOrEmpty(valueProviderResult.FirstValue))
            {
                if (bindingContext.ModelType == typeof(double))
                {
                    double temp;
                    var attempted = valueProviderResult.FirstValue.Replace(".", ",");
                    if (double.TryParse(attempted,out temp)
                        
                        //double.TryParse(
                        //attempted,
                        //NumberStyles.Number,
                        //CultureInfo.InvariantCulture,
                        //out temp)
                    )
                    {
                        bindingContext.Result = ModelBindingResult.Success(temp);

                    }
                }
            }

            return Task.CompletedTask;
        }
    }

}


