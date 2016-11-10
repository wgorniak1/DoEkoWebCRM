using DoEko.Controllers.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Services.MailModels
{
    public class BaseMailModel
    {

        public string Subject { get; set; }
        public Company Company { get; set; }
        public Application Application { get; set; }
    }
    
    public class PasswordResetMailModel : BaseMailModel
    {
        public string PwdResetLink { get; set; }
        public string AccountLogin { get; set; }
    }

}
