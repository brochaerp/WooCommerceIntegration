using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FogBugz;
using Extensions;
using WooComIntegration;
using System.Configuration;
using System.Diagnostics;
using System.ComponentModel;
using System.Net;

namespace WooComIntegrationConsole
{

    public class controller
    {
        List<WooComController> Controllers;
        private WooComOrdersController ordersController;
        private WooComShippingController shippingController;
        private WooComInventoryController inventoryController;
        private WooComPricingController pricingController;
        //private WooComFSOrdersController fsOrdersController;
        //private WooComReturnsController returnsController;
        //private WooComRefundsController refundsController;
        //private ProductsController productsController;
        private string connectionString, companyName, profileName;

        private bool StartAcclamare(AcclamareLoad acclamare, string user, string pass)
        {
            return acclamare.StartAcclamare(companyName, connectionString, user, pass);
        }

        public controller()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Controllers = new List<WooComController>();
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
                companyName = ConfigurationManager.AppSettings["CompanyName"];
                profileName = ConfigurationManager.AppSettings["ProfileName"];
                string user = ConfigurationManager.AppSettings["AccUser"];
                string pass = ConfigurationManager.AppSettings["AccPass"];
                string ProccessOrderSeconds = ConfigurationManager.AppSettings["ProccessOrderTimerSeconds"].ToString();
                string ShippingSeconds = ConfigurationManager.AppSettings["ShippingTimerSeconds"].ToString();
                string InventorySeconds = ConfigurationManager.AppSettings["InventoryTimerSeconds"].ToString();
                string PricingSeconds = ConfigurationManager.AppSettings["PricingTimerSeconds"].ToString();
                string ReturnsSeconds = ConfigurationManager.AppSettings["ReturnsTimerSeconds"].ToString();
                string RefundsSeconds = ConfigurationManager.AppSettings["RefundsTimerSeconds"].ToString();
                string ProductsSeconds = ConfigurationManager.AppSettings["ProductsTimerSeconds"].ToString();
                //string ProccessWFSOrderSeconds = ConfigurationManager.AppSettings["ProccessWFSOrderTimerSeconds"];
                string logSuccess = "";

                #region Start Acclamare

                AcclamareLoad acclamare = new AcclamareLoad();
                bool acclamareStarted = StartAcclamare(acclamare, user, pass);
                if (acclamareStarted)
                    logSuccess += string.Format("Successfully loaded Acclamare ({0}).\n", companyName);
                else
                    logSuccess += string.Format("Acclamare ({0}) didn't load correctly!\n", companyName);

                #endregion

                #region Setup DebugLogger

                DebugLogger.LogProvider = new WooComProvider(profileName);
                DebugLogger.DefaultGroupHeader = "WooComService";

                if (DebugLogger.LogToFile)
                    logSuccess += string.Format("Logging to file \"{0}\".\n", DebugLogger.LogFilePath);
                else
                    logSuccess += "Logging to database table WooComServiceLog.\n";

                DebugLogger.WriteLine(MessageSeverity.Debug, "Started Service...");

                #endregion

                #region Setup WooComOrdersController

                int timerSeconds = 20;
                if (int.TryParse(ProccessOrderSeconds, out timerSeconds))
                {
                    ordersController = new WooComOrdersController(profileName, timerSeconds);
                    ordersController.ErrorOccurred += this.LogError;

                    Controllers.Add(ordersController);
                    ordersController.StartPolling();

                    logSuccess += string.Format("Successfully started WooComOrdersController running every {0} seconds.\n", timerSeconds);
                }
                #endregion

                #region Setup WooComShippingController

                timerSeconds = 3000; // 6 hours
                if (int.TryParse(ShippingSeconds, out timerSeconds))
                {
                    shippingController = new WooComShippingController(profileName, timerSeconds);
                    shippingController.ErrorOccurred += this.LogError;

                    Controllers.Add(shippingController);
                    shippingController.StartPolling();

                    logSuccess += string.Format("Successfully started WooComShippingController running every {0} seconds.\n", timerSeconds);
                }
                #endregion

                #region Setup WooComInventoryController

                timerSeconds = 3000; // 6 hours
                if (int.TryParse(InventorySeconds, out timerSeconds))
                {
                    inventoryController = new WooComInventoryController(profileName, timerSeconds);
                    inventoryController.ErrorOccurred += this.LogError;

                    Controllers.Add(inventoryController);
                    inventoryController.StartPolling();

                    logSuccess += string.Format("Successfully started WooComInventoryController running every {0} seconds.\n", timerSeconds);
                }
                #endregion

                #region Setup WooComPricingController

                timerSeconds = 21600; // 6 hours
                if (int.TryParse(PricingSeconds, out timerSeconds))
                {
                    pricingController = new WooComPricingController(profileName, timerSeconds);
                    pricingController.ErrorOccurred += this.LogError;

                    Controllers.Add(pricingController);
                    pricingController.StartPolling();

                    logSuccess += string.Format("Successfully started WooComPricingController running every {0} seconds.\n", timerSeconds);
                }
                #endregion

                #region Setup WooComReturnsController

                timerSeconds = 21600; // 6 hours
                if (int.TryParse(ReturnsSeconds, out timerSeconds))
                {
                    //returnsController = new WooComReturnsController(profileName, timerSeconds);
                    //returnsController.ErrorOccurred += this.LogError;

                    //Controllers.Add(returnsController);
                    //returnsController.StartPolling();

                    //logSuccess += string.Format("Successfully started WooComReturnsController running every {0} seconds.\n", timerSeconds);
                }
                #endregion

                #region Setup WooComRefundsController

                timerSeconds = 21600; // 6 hours
                if (int.TryParse(RefundsSeconds, out timerSeconds))
                {
                    //refundsController = new WooComRefundsController(profileName, timerSeconds);
                    //refundsController.ErrorOccurred += this.LogError;

                    //Controllers.Add(refundsController);
                    //refundsController.StartPolling();

                    //logSuccess += string.Format("Successfully started WooComRefundsController running every {0} seconds.\n", timerSeconds);
                }
                #endregion

                #region Setup ProductsController

                timerSeconds = 20000; // 6 hours
                if (int.TryParse(ProductsSeconds, out timerSeconds))
                {
                    //productsController = new ProductsController(profileName, timerSeconds);
                    //productsController.ErrorOccurred += this.LogError;

                    //Controllers.Add(productsController);
                    //productsController.StartPolling();

                    //logSuccess += string.Format("Successfully started ProductsSeconds running every {0} seconds.\n", timerSeconds);
                }
                #endregion

                LogMessage(logSuccess, EventLogEntryType.Information);

            }
            catch (Exception ex)
            {
                LogError(this, new ExceptionEventArgs(ex));
            }
        }

        private void LogMessage(string message, EventLogEntryType eventType)
        {
            //setupLog();

            //this.EventLog.WriteEntry(message, eventType);
        }

        private void LogError(object sender, ExceptionEventArgs ex)
        {
            LogMessage(String.Format("WooComService Error: {0}", ex.PassedException), EventLogEntryType.Error);

            try
            {
                string defaultMsg = string.Format("BugzScout Submission: {0}", ex.PassedException.Message);
                BugReport.Submit("https://erpinterface.fogbugz.com/scoutSubmit.asp", "Quality Assurance", "WooComService", "Code", "Shmily@ErpInterface.com", false, defaultMsg, ex.PassedException, true, "{0}.{1}.{2}.{3}", true);
            }
            catch (Exception e)
            {
                //this.EventLog.WriteEntry(String.Format("Couldn't report an Error to Fogbugz: {0}", e), EventLogEntryType.Error);
            }
        }

        //private void setupLog()
        //{
        //    // Add lock block so shouldn't initialize the same object twice.

        //    ((ISupportInitialize)EventLog.be).BeginInit();

        //    if (!EventLog.SourceExists(EventLog.siou.Source))
        //    {
        //        EventLog.CreateEventSource(EventLog.Source, this.EventLog.Log);
        //    }

        //    ((ISupportInitialize)(EventLog)).EndInit();
        //}

    }

}
