using DoEko.Controllers.Settings;
using Microsoft.Extensions.Options;
using SendGrid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//using RazorEngine;
//using RazorEngine.Templating;
using Microsoft.AspNetCore.Server;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using DoEko.Models.Identity;
using DoEko.Services.MailModels;
using SendGrid.Helpers.Mail;
using RazorLight;


namespace DoEko.Services
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public const string templateExtension = ".cshtml";
        /// <summary>
        /// Provides service connection details
        /// </summary>
        private readonly SendGridOptions _sendGridOptions;
        /// <summary>
        /// Holds path to e-mail html templates used in the application
        /// </summary>
        private readonly string _templateFolderPath;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="options"></param>
        private readonly MailModels.BaseMailModel _mailModel;

        public AuthMessageSender(IOptions<AppSettings> options, IHostingEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager)
        {
            _sendGridOptions = options.Value.SendGridOptions;
            _templateFolderPath = Path.Combine(hostingEnvironment.ContentRootPath, "Services", "MailTemplates");
            _mailModel = new MailModels.BaseMailModel
            {
                Subject = "",
                Company = options.Value.MailTemplatesOptions.Company,
                Application = options.Value.MailTemplatesOptions.Application
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var myMessage = new SendGridMessage();
            myMessage.AddTo(email);
            myMessage.From = new EmailAddress(_mailModel.Application.SupportMail, "Administrator aplikacji");
            myMessage.Subject = subject;
            myMessage.PlainTextContent = message;
            myMessage.HtmlContent = message;

            var client = new SendGridClient(_sendGridOptions.APIKey);

            return client.SendEmailAsync(myMessage);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task SendEmailAsync(string email, string subject, string templateName, object model)
        {
            //string MailTemplate = File.ReadAllText(Path.Combine(_templateFolderPath, templateName + ".cshtml"),System.Text.Encoding.GetEncoding("Windows-1252"));
            //string MailBody;

            BaseMailModel typedModel = (BaseMailModel)model;
            typedModel.Application = _mailModel.Application;
            typedModel.Company = _mailModel.Company;

            var engine = new  RazorLightEngineBuilder()
                .UseFilesystemProject(_templateFolderPath)
                .UseMemoryCachingProvider()
                .Build();

            string mailBody = engine.CompileRenderAsync(templateName + templateExtension, model).GetAwaiter().GetResult();
            
            return SendEmailAsync(email, subject, mailBody);

        }
    }
    public enum MailTemplates
    {
        [Display(Name = "AccessGrantedMailTemplate")]
        AccessGranted,
        [Display(Name = "ConfirmEmailMailTemplate")]
        ConfirmEmail,
        [Display(Name = "PasswordResetMailTemplate")]
        PasswordReset
    }
}
