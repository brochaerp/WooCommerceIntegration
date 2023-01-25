using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Reflection;
using TkoController;
using TkoUtility;
using TkoModel;
using System.Net.Mail;
using System.Net;

namespace WooComIntegration
{
    public class WooComController : ATimerController
    {
        //    public JetApiClient client { get; protected set; }
        public WooComProvider provider { get; protected set; }

        public string consumerId { get; set; }
        public string channelId { get; set; }
        public string privateKey { get; set; }
        public Timer timer { get; set; }
        public Timer inventoryTimer { get; set; }
        public int daysToDownload { get; set; }
        public int timerInterval { get; set; }
        //public int proccessWFSOrderSearchBackSeconds { get; set; }
        //public int proccessWFSOrderInitialSearchBackSeconds { get; set; }

        public WooComController(string companyName)
        {
            provider = new WooComProvider(companyName);

            //client = new JetApiClient(provider.JetSetting.ApiUser.CurrentValue, provider.JetSetting.ApiSecret.CurrentValue);
            //client.LoginAsync();

            //getConfigSettings();
        }

        public WooComController(string companyName, ElapsedEventHandler handlerMethod, int timerSeconds = 600)
            : this(companyName)
        {
            setupTimer(timerSeconds, handlerMethod);
        }

        private void getConfigSettings()
        {
            try
            {
                var assembly = Assembly.GetEntryAssembly();
                var currentPath = Path.GetDirectoryName(assembly.Location);

                consumerId = ConfigurationManager.AppSettings["ConsumerId"];
                channelId = ConfigurationManager.AppSettings["channelType"];

                var privateKeyFile = string.Format(@"{0}\{1}", currentPath, ConfigurationManager.AppSettings["privateKey"]);

                using (var stream = new StreamReader(privateKeyFile))
                {
                    privateKey = stream.ReadToEnd();
                }

                daysToDownload = int.Parse(ConfigurationManager.AppSettings["DaysToSearch"]);
                timerInterval = int.Parse(ConfigurationManager.AppSettings["TimerInterval"]);
                //proccessWFSOrderSearchBackSeconds = int.Parse(ConfigurationManager.AppSettings["proccessWFSOrderSearchBackSeconds"]);
                //proccessWFSOrderInitialSearchBackSeconds = int.Parse(ConfigurationManager.AppSettings["proccessWFSOrderInitialSearchBackSeconds"]);
            }
            catch (Exception ex)
            {
                //ex.LogWithSerilog();
                throw new Exception("Error loading Settings from Config file!", ex);
            }
        }

        #region Polling methods

        public void StartPolling()
        {
            repeatingTimer.Start();
            OnStarted(EventArgs.Empty);
        }

        public void StopPolling()
        {
            repeatingTimer.Stop();
            OnStopped(EventArgs.Empty);
        }

        #endregion

        protected Guid getUploadingUserId()
        {
            Guid uploadingUserId = provider.WooComProfileSetting.UploadUserId.CurrentValue;

            if (uploadingUserId == Guid.Empty)
                uploadingUserId = LoggedOnUser.MyUser.UserId.CurrentValue;

            return uploadingUserId;
        }

        public void SendEmail(Guid emailFromUserId, string emailTo, string subject, string body)
        {
            TKOUser emailFromUser = TkoContainer.GetObject<ITKOUserManager>().GetTKOUser(emailFromUserId);

            MailMessage mail = new MailMessage(emailFromUser.EmailAddress.CurrentValue, emailTo);
            mail.IsBodyHtml = true;
            mail.Subject = subject;
            mail.Body = body;

            using (SmtpClient client = new SmtpClient(emailFromUser.EmailServer.CurrentValue))
            {
                client.EnableSsl = emailFromUser.EmailSsl.CurrentValue;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(emailFromUser.EmailAccount.CurrentValue, emailFromUser.EmailPassword.CurrentValue);

                if (emailFromUser.EmailPort.CurrentValue > 0)
                    client.Port = emailFromUser.EmailPort.CurrentValue;

                client.Send(mail);
            }
        }

    }
}
