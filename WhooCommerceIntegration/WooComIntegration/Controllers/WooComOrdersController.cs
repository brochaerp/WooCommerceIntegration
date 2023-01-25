using Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TkoController;
using TkoModel;
using TkoUtility;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v3;
using WooCommerceNET.WooCommerce.v3.Extension;

namespace WooComIntegration
{
    public class WooComOrdersController : WooComController
    {
        ICustomerManager CustomerMgr;
        public ISalesOrderManager SalesOrderMgr;

        IItemManager ItemMgr;

        public static Dictionary<ulong, bool> ReadyOrderIds = new Dictionary<ulong, bool>();
        private Dictionary<string, Guid> channelShipServices = new Dictionary<string, Guid>();


        public WooComOrdersController(string companyName, int proccessOrderTimerSeconds = 600)
            : base(companyName)
        {
            setupManagers();

            setupTimer(proccessOrderTimerSeconds, this.pollForReadyOrders);
        }

        private void setupManagers()
        {
            CustomerMgr = TkoContainer.GetObject<ICustomerManager>();
            SalesOrderMgr = TkoContainer.GetObject<ISalesOrderManager>();
            ItemMgr = TkoContainer.GetObject<IItemManager>();
        }


        private async void pollForReadyOrders(object sender, ElapsedEventArgs e)
        {
            List<Order> newOrders = new List<Order>();
            try
            {
                StopPolling();
                newOrders = getOrders("Created");

                DebugLogger.WriteLine(MessageSeverity.Debug, "GetOrders request returned {0} orders", newOrders.Count);

                channelShipServices = provider.GetChannelShipServices();

                foreach (var orderListResult in newOrders)
                {
                    WooComOrder savedWooComOrder;

                    ulong orderId = orderListResult.id ?? default(ulong);

                    if (isNewWooComOrder(orderId, out savedWooComOrder))
                    {
                        // Set the Id to false meaning that it wasn't saved to the database yet.
                        ReadyOrderIds[orderId] = false;

                        DebugLogger.WriteLine(MessageSeverity.Informational, "Processing order# {0}", orderId);

                        Order WooComOrder = GetWooComOrder((ulong)orderId);

                        bool acknowledged = processOrder(WooComOrder, savedWooComOrder);

                        if (acknowledged)
                            DebugLogger.WriteLine(MessageSeverity.Success, "Acknowledged WooCom Order:\t {0}", orderListResult);
                        else
                            DebugLogger.WriteLine(MessageSeverity.Failure, "Couldn't acknowledge order ({0})!", orderListResult);

                        ReadyOrderIds.Remove(orderId);
                    }
                }
            }
            catch (AggregateException ae)
            {
                foreach (var order in newOrders)
                {
                    ReadyOrderIds.Remove(order.id.Value);
                }

                foreach (Exception ex in ae.InnerExceptions)
                {
                    if (ex is TaskCanceledException && ex.InnerException != null)
                        OnErrorOccurred(ex.InnerException);
                    else
                        OnErrorOccurred(ex);
                }
            }
            catch (Exception ex)
            {
                foreach (var order in newOrders)
                {
                    ReadyOrderIds.Remove(order.id.Value);
                }

                OnErrorOccurred(ex);
            }
            finally
            {
                StartPolling();
            }
        }

        private Order GetWooComOrder(ulong id)
        {
            RestAPI rest = new RestAPI("http://www.yourstore.co.nz/wp-json/wc/v3/", "<WooCommerce Key>", "<WooCommerce Secret");
            WCObject wc = new WCObject(rest);

            var order = wc.Order.Get(id);
            return order.Result;
        }

        private List<Order> getOrders(string orderStatus)
        {
            List<Order> orderList = new List<Order>();
            try
            {
                //create ordersrequestresponse object
                RestAPI rest = new RestAPI("http://www.yourstore.co.nz/wp-json/wc/v3/", "<WooCommerce Key>", "<WooCommerce Secret");
                WCObject wc = new WCObject(rest);

                orderList = wc.Order.GetAll(new Dictionary<string, string>() {
                    //{ "include", "10, 11, 12, 13, 14, 15" },
                    { "after", DateTime.Now.AddDays(-daysToDownload).ToString("yyyy-MM-dd") },
                    { "per_page", "15" } }).Result;

                //int page = 1;
                //while (true)
                //{
                //    var tempResult = orderService.GetAllPaginated(page, 100, testQuery);
                //    if (result == null && tempResult != null)
                //        result = tempResult;
                //    else if (tempResult?.result.Count > 0)
                //    {
                //        result.result.AddRange(tempResult.result);
                //    }
                //    else
                //        break;

                //    page++;
                //}


                return orderList;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting WooCom Orders!", ex);
            }
        }

        private bool isNewWooComOrder(ulong orderId, out WooComOrder WooComOrder)
        {
            bool isNew = !ReadyOrderIds.ContainsKey(orderId);
            WooComOrder = null;

            if (isNew)
            {
                isNew = false;

                WooComOrder = provider.GetWooComOrder((int)orderId);

                // If order is not saved yet OR is not yet Acknowledged.
                if (WooComOrder.IsEmpty || WooComOrder.IsNotYetAcknowledged)
                    isNew = true;
                else
                    DebugLogger.WriteLine(MessageSeverity.Warning, "Duplicate: Already saved WooCom Order:\t {0}", orderId);
                //        }
                //    }
            }
            else
                DebugLogger.WriteLine(MessageSeverity.Warning, "Duplicate: Already processing WooCom Order:\t {0}", orderId);

            return isNew;
        }

        public bool processOrder(Order WooComOrder, WooComOrder wmSystemOrder)
        {
            bool acknowledged = false;
            bool matchedItems = false;
            //if the order is not yet in the system convert it else use the one from the system
            if (wmSystemOrder.PurchaseOrderId.CurrentValue == 0)
                wmSystemOrder = (WooComOrder)WooComOrder;
            else
                wmSystemOrder = wmSystemOrder.CopyFromWooComOrder(WooComOrder);

            foreach (WooComOrderLine item in wmSystemOrder.MyLines)
            {
                verifyItem(item);
            }

            //matchedItems = wmSystemOrder.MyLines.TrueForAll(l => l.QuantityAvailable.CurrentValue >= l.Quantity.CurrentValue);
            matchedItems = wmSystemOrder.MyLines.TrueForAll(l => l.ItemId.CurrentValue != Guid.Empty);

            SalesOrder newSalesOrder = null;

            if (matchedItems)
            {
                newSalesOrder = uploadOrderToAcclamare(wmSystemOrder);

                if (newSalesOrder != null && newSalesOrder.SalesOrderId.CurrentValue != Guid.Empty)
                {

                    try
                    {
                        // Email the new Sales Order;
                        string subject = string.Format("New Sales Order #{0} (WooCom Order #{1})", newSalesOrder.SalesOrderNumber.CurrentValue, newSalesOrder.CustomerPO.UserValue);
                        //SendEmail(newSalesOrder, newSalesOrder.UserId.CurrentValue, provider.WooComProfileSetting.EmailTo.CurrentValue, subject);
                    }
                    catch (Exception ex)
                    {
                        DebugLogger.WriteLine(MessageSeverity.Error, ex.Message);
                        OnErrorOccurred(ex);
                    }

                    ReadyOrderIds[WooComOrder.id.Value] = true;
                }
                else
                {
                    // There is an error creating SalesOrder so don't acknowledge anything yet.
                    //wmSystemOrder.AcknowledgmentStatus.CurrentValue = (byte)orderLineStatusValueType.Created;
                }
            }
            else
            {
                // Reject the order because couldn't find a matching Item or there isn't enough Quantity Available.
                wmSystemOrder.AcknowledgmentStatus.CurrentValue = 255;

                try
                {
                    string subject = string.Format("Rejected WooCom Order # {0}", WooComOrder.id);

                    if (wmSystemOrder != null && !wmSystemOrder.IsEmpty)
                    {
                        // Email the rejected WooCom Order    
                        SendEmail(wmSystemOrder, getUploadingUserId(), provider.WooComProfileSetting.EmailTo.CurrentValue, subject);
                    }
                    else
                        DebugLogger.WriteLine(MessageSeverity.Failure, "No Rejected WooCom Order Email Sent! [Subject: {0}]", subject);
                }
                catch (Exception ex)
                {
                    DebugLogger.WriteLine(MessageSeverity.Error, "ERROR Rejecting WooCom Order! \t{0}", ex.Message);
                    OnErrorOccurred(ex);
                }
            }


            // Save the WooComOrder IF created new SalesOrder OR AcknowledgmentStatus is Created or Rejected.
            //if ((newSalesOrder != null && newSalesOrder.SalesOrderNumber.CurrentValue != 0) || wmSystemOrder.AcknowledgmentStatus.CurrentValue == (byte)orderLineStatusValueType.Created || wmSystemOrder.AcknowledgmentStatus.CurrentValue == 255)
            //{
            try
            {
                // Update the LineStatus of each line to match the WooComOrder AcknowledgmentStatus.
                //orderLineStatusValueType status = (orderLineStatusValueType)wmSystemOrder.AcknowledgmentStatus.CurrentValue;

                //foreach (var line in wmSystemOrder.MyLines)
                //{
                //    //line.LineStatus.CurrentValue = status.ToString();
                //    line.Status = TkoModel.DataStatus.Changed;
                //    line.CanSave();
                //}

                if (wmSystemOrder.CanSave() != CanSaveResult.NoChangesToSave)
                    SaveWooComOrder(wmSystemOrder);
            }
            catch (Exception ex)
            {
                DebugLogger.WriteLine(MessageSeverity.Error, ex.Message);
                OnErrorOccurred(ex);
                //OnErrorOccurred(new Exception(string.Format("Error saving WooCom Order # {0} [{1}]!\n\n{2}", w., orderDetails.WooComOrderId, ex)));                    
            }
            //}

            return acknowledged;
        }

        private WooComOrderLine verifyItem(WooComOrderLine orderItem)
        {
            // Try to find Item
            orderItem.SetItemValues(provider.FindItemBySku(orderItem.Sku.CurrentValue));

            return orderItem;
        }

        #region SalesOrder methods

        private SalesOrder uploadOrderToAcclamare(WooComOrder wmSystemOrder)
        {
            SalesOrder salesOrder = findSalesOrderByCustomerPO(wmSystemOrder.PurchaseOrderId.CurrentValue.ToString());

            if (salesOrder == null)
            {
                salesOrder = createSalesOrder(wmSystemOrder);

                try
                {
                    foreach (var item in wmSystemOrder.MyLines.OrderBy(l => l.ItemId.CurrentValue))
                    {
                        addToSalesOrder(ref salesOrder, item);
                    }

                    salesOrder.LockId.CurrentValue = Guid.Empty;

                    CanSaveResult canSaveResult = salesOrder.CanSave();

                    //if (canSaveResult == CanSaveResult.HasError)
                    //{
                    //    string soErrors = "Acclamare Errors: ";

                    //    foreach (var error in salesOrder.CanSaveErrors())
                    //    {
                    //        soErrors += "\r\n\t- " + error;
                    //    }

                    //    OnErrorOccurred(new Exception("Error saving Sales Order! \r\n" + soErrors));
                    //    //Logging.Log("Error saving Sales Order! " + soErrors);
                    //}

                    salesOrder.Clone(SalesOrderMgr.SaveSalesOrder(salesOrder));

                    DebugLogger.WriteLine(MessageSeverity.Success, "Saved new Sales Order #{0} ( {1} )", salesOrder.SalesOrderNumber.CurrentValue, salesOrder.SalesOrderId.CurrentValue);

                    Guid betterFulillmentWarehouseId = SalesOrderMgr.FindBestWarehouseForSalesOrder(salesOrder.SalesOrderId.CurrentValue);
                    if (betterFulillmentWarehouseId != Guid.Empty && betterFulillmentWarehouseId != salesOrder.WarehouseId.CurrentValue)
                    {
                        salesOrder.WarehouseId.CurrentValue = betterFulillmentWarehouseId;
                        salesOrder.Clone(SalesOrderMgr.ChangeWarehouse(salesOrder));
                    }

                    //wmSystemOrder.SalesOrderNumber.CurrentValue = salesOrder.SalesOrderNumber.CurrentValue.ToString();
                    wmSystemOrder.SalesOrderId.CurrentValue = salesOrder.SalesOrderId.CurrentValue;
                    //wmSystemOrder.AcknowledgmentStatus.CurrentValue = (byte)orderLineStatusValueType.Acknowledged;

                    //Apply the payment to the new order
                    applyPaymentToOrder(salesOrder, CustomerPaymentSource.SalesOrderFullPrepayment, wmSystemOrder.order.number);
                }
                catch (SqlException ex)
                {
                    OnErrorOccurred(ex);
                }
                catch (Exception ex)
                {
                    OnErrorOccurred(ex);
                    DebugLogger.WriteLine(MessageSeverity.Error, "Error saving new Sales Order #{0} ( {1} )", salesOrder.SalesOrderNumber.CurrentValue, salesOrder.SalesOrderId.CurrentValue);
                }
            }

            return salesOrder;
        }

        private SalesOrder findSalesOrderByCustomerPO(string customerPO)
        {
            SalesOrder soFound = null;

            Guid orderId = provider.FindWooComSalesOrderId(customerPO);

            if (orderId != Guid.Empty)
                soFound = SalesOrderMgr.GetSalesOrder(orderId, Guid.Empty);

            return soFound;
        }

        private SalesOrder createSalesOrder(WooComOrder wmSystemOrder)
        {
            SalesOrder newOrder = SalesOrderMgr.NewSalesOrder(getUploadingUserId());

            var customer = getCustomer(wmSystemOrder);

            populateSalesOrder(ref newOrder, customer, wmSystemOrder);

            return newOrder;
        }

        private SalesOrder populateSalesOrder(ref SalesOrder order, TkoModel.Customer customer, WooComOrder aSystemOrder)
        {
            order.SetCustomer(customer);

            order.SourceId.CurrentValue = provider.WooComProfileSetting.OrderSourceId.CurrentValue;
            order.OrderedOnDate.UserValue = aSystemOrder.OrderDate.CurrentValue;
            order.OrderedBy.UserValue = "NA";

            order.ShippingName.UserValue = aSystemOrder.Name.CurrentValue;
            order.ShippingEmailAddress.UserValue = aSystemOrder.CustomerEmailId.CurrentValue;
            order.ShippingAddress1.UserValue = aSystemOrder.Address1.CurrentValue;
            order.ShippingAddress2.UserValue = aSystemOrder.Address2.CurrentValue;
            order.ShippingCity.UserValue = aSystemOrder.City.CurrentValue;
            order.ShippingState.UserValue = aSystemOrder.State.CurrentValue;
            order.ShippingZip.UserValue = aSystemOrder.PostalCode.CurrentValue;
            //order.ShippingCountry.UserValue = "USA";
            order.ShippingPhoneNumber.UserValue = aSystemOrder.Phone.CurrentValue;

            order.CustomerPO.UserValue = aSystemOrder.PurchaseOrderId.CurrentValue.ToString();
            order.CustomField5.CurrentValue = aSystemOrder.MarketplaceId.CurrentValue.ToString(); //Marketplace Order Number

            order.FreightPaymentMethod.CurrentValue = (byte)FreightPayMethod.ManualCharge;
            order.Freight.CurrentValue = aSystemOrder.ShippingRate.CurrentValue;
            order.ShippingFreight.CurrentValue = order.Freight.CurrentValue;

            order.EstimatedTax.UserValue = aSystemOrder.MyLines.Sum(l => l.TaxAmount.CurrentValue);
            order.ShippingEstimatedTax.UserValue = order.EstimatedTax.CurrentValue;
            var taxableState = provider.WooComProfileSetting.TaxableState.CurrentValue;
            string orderShipState = order.ShippingState.CurrentValue.ToLower();
            bool isTaxableState = (!string.IsNullOrEmpty(taxableState) && !string.IsNullOrEmpty(orderShipState) && taxableState.Split(',').Any(s => s.ToLower() == orderShipState));

            if (order.EstimatedTax.CurrentValue > 0 && isTaxableState)
            {
                if (customer.MyLocations.Count > 0)
                {
                    order.LocationId.CurrentValue = customer.MyLocations[0].LocationId.CurrentValue;
                    order.IsTaxable.CurrentValue = customer.MyLocations[0].IsTaxable.CurrentValue;
                }
            }

            // Set carrier
            //var matchedCarrier = channelShipServices[aSystemOrder.order.shipping.FirstOrDefault()?.name.ToUpper()];
            //order.CarrierServiceId.UserValue = matchedCarrier;

            order.LateAfterDate.CurrentValue = aSystemOrder.EstimatedDeliveryDate.CurrentValue ?? order.LateAfterDate.CurrentValue;

            return order;
        }

        //public Guid GetCarrierService(shippingMethodCodeType shippingMethodRequested)
        //{
        //    Guid carrierId;

        //    switch (shippingMethodRequested)
        //    {
        //        case shippingMethodCodeType.Standard:
        //            carrierId = provider.WooComProfileSetting.DefaultCarrierServiceId.CurrentValue;
        //            break;

        //        default:
        //            carrierId = provider.WooComProfileSetting.DefaultCarrierServiceId.CurrentValue;
        //            break;
        //    }
        //    return carrierId;
        //}


        private TkoModel.Customer getCustomer(WooComOrder wmSystemOrder)
        {
            TkoModel.Customer customer = null;

            if (!provider.WooComProfileSetting.CreateNewCustomer.CurrentValue && provider.WooComProfileSetting.CustomerId.CurrentValue != Guid.Empty)
            {
                customer = CustomerMgr.GetCustomer(provider.WooComProfileSetting.CustomerId.CurrentValue);
            }
            else
            {
                customer = createNewCustomer(wmSystemOrder);
            }

            return customer;
        }

        private TkoModel.Customer createNewCustomer(WooComOrder wmSystemOrder)
        {
            TkoModel.Customer customer = CustomerMgr.NewCustomer(LoggedOnUser.MyUser.UserId.CurrentValue);

            populateCustomer(ref customer, wmSystemOrder);

            CustomerMgr.SaveCustomer(customer, LoggedOnUser.MyUser.UserId.CurrentValue);

            return customer;
        }

        private void populateCustomer(ref TkoModel.Customer customer, WooComOrder wmSystemOrder)
        {
            customer.AccountNumber.UserValue = wmSystemOrder.Name.CurrentValue;
            customer.CustomerName.UserValue = wmSystemOrder.Name.CurrentValue;

            customer.TermId.UserValue = provider.WooComProfileSetting.TermId.CurrentValue;
            customer.TypeId.UserValue = provider.WooComProfileSetting.TypeId.CurrentValue;
            customer.WarehouseId.UserValue = provider.WooComProfileSetting.WarehouseId.CurrentValue;

            customer.BillingEmailAddress.UserValue = wmSystemOrder.CustomerEmailId.CurrentValue;
            customer.BillingAddress1.UserValue = wmSystemOrder.Address1.CurrentValue;
            customer.BillingAddress2.UserValue = wmSystemOrder.Address2.CurrentValue;
            customer.BillingCity.UserValue = wmSystemOrder.City.CurrentValue;
            customer.BillingState.UserValue = wmSystemOrder.State.CurrentValue;
            customer.BillingZip.UserValue = wmSystemOrder.PostalCode.CurrentValue;
            //customer.BillingCountry.UserValue = "USA";
            customer.BillingPhoneNumber.UserValue = wmSystemOrder.Phone.CurrentValue;

            customer.ShippingEmailAddress.UserValue = wmSystemOrder.CustomerEmailId.CurrentValue;
            customer.ShippingAddress1.UserValue = wmSystemOrder.Address1.CurrentValue;
            customer.ShippingAddress2.UserValue = wmSystemOrder.Address2.CurrentValue;
            customer.ShippingCity.UserValue = wmSystemOrder.City.CurrentValue;
            customer.ShippingState.UserValue = wmSystemOrder.State.CurrentValue;
            customer.ShippingZip.UserValue = wmSystemOrder.PostalCode.CurrentValue;
            //customer.ShippingCountry.UserValue = "USA";
            customer.ShippingPhoneNumber.UserValue = wmSystemOrder.Phone.CurrentValue;

            customer.IsTaxable.CurrentValue = false;
            customer.TaxExemptNumber.UserValue = "Reseller - WooCom.com";

            customer.SalesForceId.UserValue = provider.WooComProfileSetting.SalesForceId.CurrentValue;
            customer.FieldSalesForceId.UserValue = provider.WooComProfileSetting.FieldSalesForceId.CurrentValue;

            DocumentControlCustomer docControl = CustomerMgr.NewDocumentControlCustomer(customer);
            docControl.DocumentType.UserValue = (byte)DocumentControlType.CustomerInvoice;
            docControl.DeliveryMethod.UserValue = 255;
            customer.MyDocuments.Add(docControl);
        }

        private SalesOrderLine addToSalesOrder(ref SalesOrder salesOrder, WooComOrderLine item)
        {
            SalesOrderLine line = SalesOrderMgr.NewSalesOrderLine(salesOrder);

            //Item itemFound = ItemMgr.GetItem(item.ItemId.CurrentValue);

            if (item.ItemId.CurrentValue != Guid.Empty)
            {
                // Setup line with item info. 
                line.Quantity.CurrentValue = item.Quantity.CurrentValue * item.QuantityUmRatio.CurrentValue;

                var sellItemResponse = SalesOrderMgr.SellItem(item.ItemId.CurrentValue, salesOrder.WarehouseId.CurrentValue, salesOrder.CustomerId.CurrentValue, salesOrder.LocationId.CurrentValue, line.Quantity.CurrentValue);

                ItemSearchResult itemResult = new ItemSearchResult();
                itemResult.ItemId = item.ItemId.CurrentValue;
                itemResult.ItemNumber = item.Sku.CurrentValue;
                itemResult.ItemDescription = item.ProductName.CurrentValue;
                itemResult.QuantityUm = item.QuantityUm.CurrentValue;
                itemResult.QuantityUmRatio = item.QuantityUmRatio.CurrentValue;
                itemResult.PriceCostUm = item.QuantityUm.CurrentValue;
                itemResult.PriceCostUmRatio = item.QuantityUmRatio.CurrentValue;
                itemResult.IsTaxable = item.TaxAmount.CurrentValue > 0;
                itemResult.CustomerItemId = item.CustomerItemId.CurrentValue;

                line.SetItemValues(itemResult, sellItemResponse, null);

                line.Price.CurrentValue = item.ItemPrice.CurrentValue / item.QuantityUmRatio.CurrentValue;

                item.SalesOrderLineId.CurrentValue = line.LineId.CurrentValue;

                salesOrder.MyLines.Add(line);

                CreateKitOrderLineDetail(salesOrder, line);
            }
            else
                throw new Exception(string.Format("Error: Couldn't find and add Item {0} ({1}) to Sales Order ({2})!", item.Sku.CurrentValue, item.ItemId, salesOrder.SalesOrderId.CurrentValue));

            return line;
        }

        private void CreateKitOrderLineDetail(SalesOrder salesOrder, SalesOrderLine line)
        {
            if (line.QuantityBackordered.CurrentValue == 0)
                return;

            TkoModel.Item item = ItemMgr.GetItem(line.ItemId.CurrentValue);

            if (line.QuantityBackordered.CurrentValue > 0 && item.MyKit.Count() > 0 && item.MyKit[0].IsBundle.CurrentValue)
            {
                DebugLogger.WriteLine(MessageSeverity.Informational, "Creating Kit order for SalesOrder (SalesOrderId: {0}, Item: {1})...", salesOrder.SalesOrderId.CurrentValue, item.ItemNumber.CurrentValue);
                decimal originalBoQuantity = line.QuantityBackordered.CurrentValue;
                //Save the sales order first
                line.QuantityCommitted.CurrentValue = 0;
                line.QuantityBackordered.CurrentValue = 0;
                line.QuantityCanceled.CurrentValue = 0;

                salesOrder.Clone(SalesOrderMgr.SaveSalesOrder(salesOrder));

                try
                {
                    //Create the detail to Kit order
                    SalesOrderLineDetail lineDetail = SalesOrderMgr.NewDetailToKitOrder(salesOrder, line, line.Quantity.CurrentValue, LoggedOnUser.MyUser.UserId.CurrentValue, true);
                    line.MyDetails.Add(lineDetail);
                    line.UpdateQuantityFromOther();

                    //Attempt to post the kit order
                    var kitOrderManager = TkoContainer.GetObject<IKitOrderManager>();
                    var kitOrder = kitOrderManager.GetKitOrder(lineDetail.KitOrderId.CurrentValue);
                    bool canPost = item.ControlType.CurrentValue == 0 && kitOrder.MyLines.All(l => l.ControlType.CurrentValue == 0);
                    if (canPost && kitOrder.QuantityToBuildNow.CurrentValue > 0 && PostingHeaderHelper.SetupPostingHeader(kitOrder, false))
                        kitOrderManager.PostOrder(kitOrder);
                    else
                    {
                        SalesOrderMgr.DeleteSalesOrderLineDetail(salesOrder, line, lineDetail);
                        line.UpdateQuantityFromOther();
                        line.QuantityBackordered.CurrentValue = originalBoQuantity;
                    }
                }
                catch (Exception ex)
                {
                    string msg = $"Error creating Kit order for SO: {salesOrder.SalesOrderNumber.CurrentValue} Line: {line.LineNumber.CurrentValue}!";

                    DebugLogger.WriteLine(MessageSeverity.Error, msg + "\t" + ex.Message);
                }
            }
        }

        private void applyPaymentToOrder(SalesOrder salesOrder, CustomerPaymentSource paymentSource, string channerOrderId)
        {
            ICustomerPaymentManager paymentMgr = TkoContainer.GetObject<ICustomerPaymentManager>();

            SalesOrderTotals orderTotal = SalesOrderMgr.GetSalesOrderTotals(salesOrder);

            CustomerPayment payment = paymentMgr.NewCustomerPayment(salesOrder, orderTotal, getUploadingUserId(), paymentSource);
            payment.PaymentType.CurrentValue = (byte)CustomerPaymentType.EFT;
            payment.PaymentAmount.CurrentValue = orderTotal.OrderTotal.Total.CurrentValue;
            payment.PaymentReference.CurrentValue = salesOrder.CustomerPO.CurrentValue;
            payment.PaymentNote.CurrentValue = channerOrderId;
            payment.CalculateAmounts();

            ///paymentMgr.BuildPaymentDetails(payment);
            PostingHeaderHelper.SetupPostingHeader(payment, false);

            paymentMgr.PostPayment(payment);
        }

        #endregion

        #region Save WooCom Order

        public bool SaveWooComOrder(WooComOrder WooComOrder)
        {
            bool saved = false;

            WooComOrder savedOrder = provider.SaveWooComOrder(WooComOrder);

            if (savedOrder != null && savedOrder.WooComOrderId.CurrentValue != Guid.Empty && savedOrder.Status == DataStatus.Unchanged)
                saved = true;

            return saved;
        }

        //public bool SaveWooComOrder(WooComOrder orderDetails)
        //{
        //    if (orderDetails == null || orderDetails.WooComOrderId.CurrentValue == Guid.Empty)
        //        throw new ArgumentNullException("orderDetails");

        //    bool saved = SaveWooComOrder(orderDetails, true);

        //    if (saved)
        //    {
        //        try
        //        {
        //            // Email the saved Sales Order                
        //            string subject = string.Format("Saved WooCom Order # {0}", orderDetails.PurchaseOrderId.CurrentValue);
        //            SendEmail(orderDetails, getUploadingUserId(), "sruli@erpinterface.com", subject, "manually saved");
        //        }
        //        catch (Exception ex)
        //        {
        //            OnErrorOccurred(ex);
        //        }
        //    }

        //    return saved;
        //}

        #endregion

        #region Send Email methods

        public void SendEmail(SalesOrder order, Guid emailFromUserId, string emailTo, string subject)
        {
            string orderDetails = string.Format("\n<h3>Sales Order # {0} was created with the following items:</h3>\n", order.SalesOrderNumber.CurrentValue);

            orderDetails += "<table border = 1 style=\"border: 1px solid black; border-collapse: collapse; padding: 3px;\"> "
                            + "<tr>"
                            + "<th> # </th> "
                            + "<th> Item Number </th> "
                            + "<th> Description </th> "
                            + "<th> Quantity </th> "
                            + "<th> UM </th>"
                            + "<th> Price </th>"
                            + "</tr>";

            string itemRows = "";

            foreach (var item in order.MyLines)
            {
                itemRows += string.Format("<tr>\n<td>{0}</td> \n<td>{1}</td> \n<td>{2}</td> \n<td>{3:0}</td> \n<td>{4} ({5:0})</td> \n<td>{6:C}</td> \n</tr> \n",
                    item.LineNumber.CurrentValue,
                    item.ItemNumber.CurrentValue,
                    item.ItemDescription.CurrentValue,
                    item.Quantity.CurrentValue / item.QuantityUMRatio.CurrentValue,
                    item.QuantityUM.CurrentValue,
                    item.QuantityUMRatio.CurrentValue,
                    (item.Price.CurrentValue * item.QuantityUMRatio.CurrentValue));
            }

            orderDetails += itemRows + "\n</table>\n";

            var body = string.Format("<html><body>{0}</body></html>", orderDetails);

            SendEmail(emailFromUserId, emailTo, subject, body);
        }

        public void SendEmail(WooComOrder orderDetails, Guid emailFromUserId, string emailTo, string subject, string orderStatus = "rejected")
        {
            string orderInfo = string.Format("\n<h3>WooCom Order # {0} ({1}) was {2} with the following items:</h3>\n", orderDetails.PurchaseOrderId.CurrentValue, orderDetails.WooComOrderId, orderStatus);

            orderInfo += "<table border = 1 style=\"border: 1px solid black; border-collapse: collapse; padding: 3px;\"> "
                            + "<tr>"
                            + "<th> Sku </th> "
                            + "<th> Product name </th> "
                            + "<th> Qty Requested </th> "
                            + "</tr>";

            string itemRows = "";

            foreach (var item in orderDetails.MyLines)
            {
                itemRows += string.Format("<tr>\n<td>{0}</td> \n<td>{1}</td> \n<td>{2}</td> \n</tr>\n",
                    item.Sku.CurrentValue,
                    item.ProductName.CurrentValue,
                    item.Quantity.CurrentValue);
            }

            orderInfo += itemRows + "\n</table>\n";

            var body = string.Format("<html><body>{0}</body></html>", orderInfo);

            SendEmail(emailFromUserId, emailTo, subject, body);
        }

        #endregion

    }
}
