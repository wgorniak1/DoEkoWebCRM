using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models
{
    public class DoubleModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(double))
            {
                //    var propertyBinders = new Dictionary<ModelMetadata, IModelBinder>();
                //    foreach (var property in context.Metadata.Properties)
                //    {
                //        propertyBinders.Add(property, context.CreateBinder(property));
                //    }
                var loggerFactory = (ILoggerFactory)context.Services.GetService(typeof(ILoggerFactory));

                var simpleTypeModelBinder = new SimpleTypeModelBinder(typeof(double), loggerFactory);

                return new DoubleModelBinder(simpleTypeModelBinder);
            }

            return null;
        }
    }
}
