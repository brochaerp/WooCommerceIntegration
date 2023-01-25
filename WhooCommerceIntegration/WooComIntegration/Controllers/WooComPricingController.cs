using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v2;

namespace WooComIntegration
{
    public class WooComPricingController : WooComController
    {
        public WooComPricingController(string companyName, int timerSeconds = 21600)
            : base(companyName)
        {
            setupTimer(timerSeconds, this.updateItemPrice);
        }

        private void updateItemPrice(object sender, ElapsedEventArgs e)
        {
            try
            {
                StopPolling();
                DebugLogger.WriteLine(MessageSeverity.Debug, "Starting pricing feed upload.");
                var itemPriceList = provider.GetWooComItemPricing();
                DebugLogger.WriteLine(MessageSeverity.Informational, "Got {0} pricing records.", itemPriceList.Count);

                RestAPI rest = new RestAPI("http://www.yourstore.co.nz/wp-json/wc/v3/", provider.WooComProfileSetting.ClientId.CurrentValue, provider.WooComProfileSetting.ClientSecret.CurrentValue);
                WCObject wc = new WCObject(rest);

                ProductBatch productBatch = new ProductBatch();
                productBatch.update = itemPriceList;

                var result = wc.Product.UpdateRange(productBatch).Result;
                

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
