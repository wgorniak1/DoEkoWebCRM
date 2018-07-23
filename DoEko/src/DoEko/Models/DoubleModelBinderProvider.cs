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
        private readonly ILoggerFactory _loggerFactory;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggerFactory"></param>
        public DoubleModelBinderProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

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

                return new DoubleModelBinder(new SimpleTypeModelBinder(typeof(double),_loggerFactory));
            }

            return null;
        }
    }
}
