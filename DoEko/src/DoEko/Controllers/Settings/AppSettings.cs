using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Controllers.Settings
{
    public class AppSettings
    {
        public AdminAccountOptions AdminAccountOptions { get; set; }
        public AzureStorageOptions AzureStorageOptions { get; set; }
        public SendGridOptions SendGridOptions { get; set; }
        public MailTemplatesOptions MailTemplatesOptions { get; set; }
        public TokenOptions TokenOptions { get; set; }
    }

    public class TokenOptions
    {
        public string SiteUrl { get; set; }
        public string Key { get; set; }
    }

    public class AzureStorageOptions
    {
        public string ConnectionString { get; set; }
    }

    public class AdminAccountOptions
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string PwdHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class SendGridOptions
    {
        public string APIKey { get; set; }
    }

    public class MailTemplatesOptions
    {
        public Company Company { get; set; }
        public Application Application { get; set; }
    }


    public class Company
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string URL { get; set; }
        public Image LogoSmall { get; set; }
        public Image LogoMedium { get; set; }
        public Image LogoLarge { get; set; }
    }

    public class Application
    {
        public string IconName { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public Image LogoSmall { get; set; }
        public Image LogoMedium { get; set; }
        public Image LogoLarge { get; set; }
        public string SupportMail { get; set; }
        public string AutoMailerMail { get; set; }
        public string AutoMailerName { get; set; }
    }
    public class Image
    {
        public string URL { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Name { get; set; }
    }
}
