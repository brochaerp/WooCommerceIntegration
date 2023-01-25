using WooComSDK.DTOs;
using WooComSDK.Service;
using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WooComIntegration
{
    public class ProductsController : WooComController
    {
        public ProductsController(string companyName, int timerSeconds = 21600)
            : base(companyName)
        {
            setupTimer(timerSeconds, this.downloadItems);
        }

        private void downloadItems(object sender, ElapsedEventArgs e)
        {
            try
            {
                StopPolling();
                DebugLogger.WriteLine(MessageSeverity.Debug, "Starting to download product list.");
                ServiceFactory serviceFactory = new ServiceFactory(provider.WooComProfileSetting.ClientId.CurrentValue, provider.WooComProfileSetting.ClientSecret.CurrentValue, provider.WooComProfileSetting.CompanyName.CurrentValue);
                InventoryService productService = (InventoryService)serviceFactory.GetService(WooComSDK.Enums.ServiceType.Inventory);

                ProductVariantsDTO items = new ProductVariantsDTO();
                int pageNumber = 0;
                do
                {
                    pageNumber++;
                    var temp = productService.GetAllPaginated(pageNumber, 100, "");
                    if (pageNumber == 1)
                        items = temp;
                    else
                    {
                        items.num_total = temp.num_total;
                        items.result.AddRange(temp.result);
                    }
                } while (items.num_total > items.result.Count);

                foreach (var item in items.result)
                {
                    provider.UpdateWooComProductVariant(item);
                }
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
