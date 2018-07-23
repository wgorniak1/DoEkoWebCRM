﻿using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DoEko.Controllers.ActionResults
{
    /// <summary>
    /// Represents an <see cref="ActionResult"/> that when executed will
    /// execute a callback to write the file content out as a stream.
    /// </summary>
    public class FileCallbackResult : FileResult
    {
        private Func<Stream, ActionContext, Task> _callback;
        private double _contentLength;

        /// <summary>
        /// Creates a new <see cref="FileCallbackResult"/> instance.
        /// </summary>
        /// <param name="contentType">The Content-Type header of the response.</param>
        /// <param name="callback">The stream with the file.</param>
        public FileCallbackResult(string contentType, double contentLength, Func<Stream, ActionContext, Task> callback)
            : this(MediaTypeHeaderValue.Parse(contentType), contentLength, callback)
        {
        }

        /// <summary>
        /// Creates a new <see cref="FileCallbackResult"/> instance.
        /// </summary>
        /// <param name="contentType">The Content-Type header of the response.</param>
        /// <param name="callback">The stream with the file.</param>
        public FileCallbackResult(MediaTypeHeaderValue contentType, double contentLength, Func<Stream, ActionContext, Task> callback)
            : base(contentType?.ToString())
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            Callback = callback;
            FileSize = contentLength;
            
        }

        public double FileSize
        {
            get
            {
                return _contentLength;
            }
            set
            {
                _contentLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the callback responsible for writing the file content to the output stream.
        /// </summary>
        public Func<Stream, ActionContext, Task> Callback
        {
            get
            {
                return _callback;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _callback = value;
            }
        }

        /// <inheritdoc />
        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var executor = new FileCallbackResultExecutor(context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>());
            return executor.ExecuteAsync(context, this);
        }

        private sealed class FileCallbackResultExecutor : FileResultExecutorBase
        {
            public FileCallbackResultExecutor(ILoggerFactory loggerFactory)
                : base(CreateLogger<FileCallbackResultExecutor>(loggerFactory))
            {
            }

            public Task ExecuteAsync(ActionContext context, FileCallbackResult result)
            {
                //after framework upgrade:
                //SetHeadersAndLog(context, result);
                SetHeadersAndLog(context, result, (long)(result.FileSize), true);
                return result.Callback(context.HttpContext.Response.Body, context);
            }
        }
    }
}
