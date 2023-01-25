using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WooComIntegration
{
    public class WooComInventoryController : WooComController
    {
        public WooComInventoryController(string companyName, int timerSeconds = 21600)
            : base(companyName)
        {
            setupTimer(timerSeconds, this.updateItemInventory);
        }

        private void updateItemInventory(object sender, ElapsedEventArgs e)
        {
            try
            {
                StopPolling();
                DebugLogger.WriteLine(MessageSeverity.Debug, "Starting inventory feed upload.");
                var itemInventoryList = provider.GetWooComItemInventory();
                DebugLogger.WriteLine(MessageSeverity.Informational, "Got {0} inventory records.", itemInventoryList.Count);
                //ServiceFactory serviceFactory = new ServiceFactory(provider.WooComProfileSetting.ClientId.CurrentValue, provider.WooComProfileSetting.ClientSecret.CurrentValue, provider.WooComProfileSetting.CompanyName.CurrentValue);
                //InventoryService productService = (InventoryService)serviceFactory.GetService(WooComSDK.Enums.ServiceType.Inventory);
                //foreach (var item in itemInventoryList)
                //{
                //    productService.Update(item.id, item);
                //}
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                DebugLogger.WriteLine(MessageSeverity.Error, ex.Message);
            }
            finally
            {
                StartPolling();
            }
        }


    }
}
