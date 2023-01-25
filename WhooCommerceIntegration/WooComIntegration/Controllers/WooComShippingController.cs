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
    public class WooComShippingController : WooComController
    {
        public WooComShippingController(string companyName, int timerSeconds = 21600)
            : base(companyName)
        {
            setupTimer(timerSeconds, this.pollForShippedOrders);
        }

        private void pollForShippedOrders(object sender, ElapsedEventArgs e)
        {
            try
            {
                StopPolling();
                var shippedList = provider.GetShippedWooComOrders();
                DebugLogger.WriteLine(MessageSeverity.Informational, "{0} shipped WooCom orders found.", shippedList.Count);

                RestAPI rest = new RestAPI("http://www.yourstore.co.nz/wp-json/wc/v3/", provider.WooComProfileSetting.ClientId.CurrentValue, provider.WooComProfileSetting.ClientSecret.CurrentValue);
                WCObject wc = new WCObject(rest);


                foreach (var order in shippedList)
                {
                    processShippedOrder(order);

                    //if (VerifyOrderMarkedAsShipped(order))
                    //    updateWooComOrderStatus(order);
                    updateWooComOrderStatus(order);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
            finally
            {
                StartPolling();
            }
        }

        private bool VerifyOrderMarkedAsShipped(WooComOrder order)
        {
            //ServiceFactory serviceFactory = new ServiceFactory(provider.WooComProfileSetting.ClientId.CurrentValue, provider.WooComProfileSetting.ClientSecret.CurrentValue, provider.WooComProfileSetting.CompanyName.CurrentValue);
            //OrderService orderService = (OrderService)serviceFactory.GetService(WooComSDK.Enums.ServiceType.Order);
            //var orderResult = orderService.GetById<OrderDTO>(order.PurchaseOrderId.CurrentValue.ToString());
            //if (orderResult != null && orderResult.result.fulfillments.Count > 0)
            //{
            //    DebugLogger.WriteLine(MessageSeverity.Informational, $"Order# {order.PurchaseOrderId.CurrentValue} has been marked as shipped");
            //    return true;
            //}

            return false;
        }

        private void processShippedOrder(WooComOrder order)
        {
            try
            {

                var fulfillment = new Order();
                
                //fulfillment.items = new List<FulfillmentItems>();
                //fulfillment.tr = new List<string>();
                //fulfillment.tracking_urls = new List<string>();
                //fulfillment.packages = new List<FulfillmentPackages>();
                //fulfillment.tracking_company = "UPS";
                //fulfillment.shipping_method = "Ground";
                //fulfillment.status = "success";
                //foreach (var item in order.MyLines)
                //{
                //    fulfillment.items.Add(new FulfillmentItems() { id = item.OrderItemId.CurrentValue, quantity = (int)(item.Quantity.CurrentValue) });
                //}
                ////fulfillment.tracking_company = order.ServiceName.CurrentValue;
                ////fulfillment.tracking_number = order.TrackingNumber.CurrentValue;
                //fulfillment.tracking_numbers.Add(order.TrackingNumber.CurrentValue);
                //fulfillment.tracking_urls.Add($"https://www.ups.com/track?loc=en_US&tracknum={order.TrackingNumber.CurrentValue}/trackdetails");

                try
                {
                    //orderService.CreateFulfillments(order.PurchaseOrderId.CurrentValue.ToString(), fulfillment);
                }
                catch (Exception ex)
                {
                    DebugLogger.WriteLine(MessageSeverity.Error, "Can't get new orderStatus from shippingUpdateRequestResponse.response! \r\n" + ex);
                }
            }
            catch (Exception ex)
            {

                DebugLogger.WriteLine(MessageSeverity.Error, "Can't ship WooCom Order # {0} ! \t{1}", order.PurchaseOrderId.CurrentValue, ex.Message);
            }
        }

        private void updateWooComOrderStatus(WooComOrder order)
        {
            order.AcknowledgmentStatus.CurrentValue = 255;

            // Update the LineStatus of each line to match the WooComOrder AcknowledgmentStatus. 
            foreach (var line in order.MyLines)
            {
                line.LineStatus.CurrentValue = "Shipped";

                line.Status = TkoModel.DataStatus.Changed;
                //line.CanSave();
            }

            order.Status = TkoModel.DataStatus.Changed;
            //order.CanSave();

            provider.SaveWooComOrder(order);
        }
    }
}
