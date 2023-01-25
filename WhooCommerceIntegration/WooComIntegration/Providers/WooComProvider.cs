using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TkoUtility;
using TkoModel;
using TkoProvider;
using System.Data;
using System.ComponentModel;
using Extensions;
using WooCommerceNET.WooCommerce.v2;

namespace WooComIntegration
{
    public class WooComProvider : ILogProvider
    {
        public WooComProfile WooComProfileSetting;

        public Guid? ProfileSettingId { get { return WooComProfileSetting.ProfileId.CurrentValue; } }

        public WooComProvider()
        {
            WooComProfileSetting = new WooComProfile();
        }

        public WooComProvider(string companyName)
        {
            WooComProfileSetting = GetWooComProfile(companyName);
        }

        public WooComProvider(WooComProfile profile)
        {
            WooComProfileSetting = profile;
        }

        public WooComProfile GetWooComProfile(string companyName)
        {
            WooComProfile profile = new WooComProfile();

            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM dbo.WooComProfile WHERE CompanyName = @CompanyName";

                command.Parameters.AddWithValue("@CompanyName", companyName);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    profile.ProfileId.OriginalValue = reader.GetSafeGuid(0);
                    profile.CompanyName.OriginalValue = reader.GetSafeString(1);
                    profile.WarehouseId.OriginalValue = reader.GetSafeGuid(2);
                    profile.CreateNewCustomer.OriginalValue = reader.GetSafeBoolean(3);
                    profile.OrderSourceId.OriginalValue = reader.GetSafeGuid(4);
                    profile.TypeId.OriginalValue = reader.GetSafeGuid(5);
                    profile.TermId.OriginalValue = reader.GetSafeGuid(6);
                    profile.CustomerId.OriginalValue = reader.GetSafeGuid(7);
                    profile.TaxableState.OriginalValue = reader.GetSafeString(8);
                    profile.DefaultCarrierServiceId.OriginalValue = reader.GetSafeGuid(9);
                    profile.FieldSalesForceId.OriginalValue = reader.GetSafeGuid(10);
                    profile.SalesForceId.OriginalValue = reader.GetSafeGuid(11);
                    profile.TaxingEntityId.OriginalValue = reader.GetSafeGuid(12);
                    profile.ClientId.OriginalValue = reader.GetSafeString(13);
                    profile.ClientSecret.OriginalValue = reader.GetSafeString(14);
                    profile.UploadUserId.OriginalValue = reader.GetSafeGuid(15);
                    profile.EmailTo.OriginalValue = reader.GetSafeString(16);
                    profile.ChannelId.OriginalValue = reader.GetSafeGuid(17);

                    profile.Status = DataStatus.Unchanged;

                    break;
                }

                reader.Close();
            }

            return profile;
        }

        public Guid FindWooComOrder(int purchaseOrderId)
        {
            Guid orderId = Guid.Empty;

            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT WooComOrderId FROM dbo.WooComOrder WHERE PurchaseOrderId = @purchaseOrderId";

                command.Parameters.AddWithValue("@purchaseOrderId", purchaseOrderId);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orderId = reader.GetSafeGuid(0);

                    break;
                }

                reader.Close();
            }

            return orderId;
        }

        internal Guid FindWooComSalesOrderId(string customerPO)
        {
            Guid orderId = Guid.Empty;

            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT SalesOrderId FROM dbo.SalesOrder WHERE CustomerPO = @customerPO";

                command.Parameters.AddWithValue("@customerPO", customerPO);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orderId = reader.GetSafeGuid(0);

                    break;
                }

                reader.Close();
            }

            return orderId;
        }

        public ItemSearchResult FindItemBySku(string sku)
        {
            ItemSearchResult searchResult = new ItemSearchResult();

            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "pFindItemByMarketplaceSKU";

                command.Parameters.AddWithValue("@SKU", sku);
                command.Parameters.AddWithValue("@CustomerId", WooComProfileSetting.CustomerId.CurrentValue);
                command.Parameters.AddWithValue("@WarehouseId", WooComProfileSetting.WarehouseId.CurrentValue);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    searchResult.ItemId = reader.GetSafeGuid(0);
                    searchResult.QuantityUm = reader.GetSafeString(1);
                    searchResult.QuantityUmRatio = reader.GetSafeDecimal(2);
                    searchResult.PriceCostUm = reader.GetSafeString(1);
                    searchResult.PriceCostUmRatio = reader.GetSafeDecimal(2);
                    searchResult.CustomerItemId = reader.GetSafeGuid(3);
                    searchResult.QuantityAvailable = reader.GetSafeDecimal(4);

                    break;
                }

                reader.Close();
            }

            return searchResult;
        }

        public WooComOrder GetWooComOrder(int WooComOrderId)
        {
            Guid orderId = FindWooComOrder(WooComOrderId);
            if (orderId != Guid.Empty)
                return GetWooComOrder(orderId);
            else
                return new WooComOrder();
        }

        public WooComOrder GetWooComOrder(Guid WooComOrderId)
        {
            WooComOrder WooComOrder = new WooComOrder();

            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "pGetWooComOrder";

                command.Parameters.AddWithValue("@WooComOrderId", WooComOrderId);

                SqlDataReader reader = command.ExecuteReader();
                if (!reader.HasRows)
                    return WooComOrder;

                reader.Read();
                WooComOrder.WooComOrderId.OriginalValue = reader.GetSafeGuid(0);
                WooComOrder.CustomerEmailId.OriginalValue = reader.GetSafeString(1);
                WooComOrder.CustomerOrderId.OriginalValue = reader.GetSafeString(2);
                WooComOrder.OrderDate.OriginalValue = reader.GetSafeDateTime(3);
                WooComOrder.PurchaseOrderId.OriginalValue = reader.GetInt32(4);
                WooComOrder.EstimatedDeliveryDate.OriginalValue = reader.GetSafeDateTime(5);
                WooComOrder.EstimatedShipDate.OriginalValue = reader.GetSafeDateTime(6);
                WooComOrder.MethodCode.OriginalValue = reader.GetInt32(7);
                WooComOrder.Phone.OriginalValue = reader.GetSafeString(8);
                WooComOrder.Address1.OriginalValue = reader.GetSafeString(9);
                WooComOrder.Address2.OriginalValue = reader.GetSafeString(10);
                WooComOrder.AddressType.OriginalValue = reader.GetSafeString(11);
                WooComOrder.City.OriginalValue = reader.GetSafeString(12);
                WooComOrder.Country.OriginalValue = reader.GetSafeString(13);
                WooComOrder.Name.OriginalValue = reader.GetSafeString(14);
                WooComOrder.PostalCode.OriginalValue = reader.GetSafeString(15);
                WooComOrder.State.OriginalValue = reader.GetSafeString(16);
                WooComOrder.AcknowledgmentStatus.OriginalValue = reader.GetByte(17);
                WooComOrder.DateAcknowledged.OriginalValue = reader.GetSafeDateTime(18);
                WooComOrder.SalesOrderId.OriginalValue = reader.GetSafeGuid(19);
                WooComOrder.Status = DataStatus.Unchanged;

                reader.NextResult();
                while (reader.Read())
                {
                    WooComOrderLine orderLine = new WooComOrderLine();
                    orderLine.LineId.OriginalValue = reader.GetSafeGuid(0);
                    orderLine.WooComOrderId.CurrentValue = reader.GetSafeGuid(1);
                    orderLine.SalesOrderLineId.OriginalValue = reader.GetSafeGuid(2);
                    orderLine.OrderItemId.OriginalValue = reader.GetSafeInt32(3);
                    orderLine.Sku.OriginalValue = reader.GetSafeString(4);
                    orderLine.ProductName.OriginalValue = reader.GetSafeString(5);
                    orderLine.ItemPrice.OriginalValue = reader.GetSafeDecimal(6);
                    orderLine.TaxAmount.OriginalValue = reader.GetSafeDecimal(7);
                    orderLine.Quantity.OriginalValue = reader.GetSafeDecimal(8);
                    orderLine.UnitOfMeasure.OriginalValue = reader.GetSafeString(9);
                    orderLine.StatusDate.OriginalValue = reader.GetSafeDateTime(10);
                    orderLine.LineStatus.OriginalValue = reader.GetSafeString(11);
                    orderLine.Status = DataStatus.Unchanged;

                    WooComOrder.MyLines.Add(orderLine);
                }

                reader.Close();
            }

            return WooComOrder;
        }

        #region Save WooComOrder Methods

        public WooComOrder SaveWooComOrder(WooComOrder WooComOrder)
        {
            if (WooComOrder.Status == DataStatus.Inserted)
                InsertWooComOrder(WooComOrder);
            else if (WooComOrder.Status == DataStatus.Changed)
                UpdateWooComOrder(WooComOrder);

            foreach (var item in WooComOrder.MyLines)
            {
                SaveWooComOrderLine(item);
            }

            WooComOrder.Update();


            return WooComOrder;
        }

        private void InsertWooComOrder(WooComOrder WooComOrder)
        {
            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "pInsertWooComOrder";

                command.Parameters.AddWithValue("@WooComOrderId", WooComOrder.WooComOrderId.CurrentValue);
                command.Parameters.AddWithSafeValue("@CustomerEmailId", WooComOrder.CustomerEmailId.CurrentValue);
                command.Parameters.AddWithSafeValue("@CustomerOrderId", WooComOrder.CustomerOrderId.CurrentValue);
                command.Parameters.AddWithSafeValue("@OrderNumber", WooComOrder.order.number);
                command.Parameters.AddWithSafeValue("@OrderDate", WooComOrder.OrderDate.CurrentValue);
                command.Parameters.AddWithValue("@PurchaseOrderId", WooComOrder.PurchaseOrderId.CurrentValue);
                command.Parameters.AddWithSafeValue("@EstimatedDeliveryDate", WooComOrder.EstimatedDeliveryDate.CurrentValue);
                command.Parameters.AddWithSafeValue("@EstimatedShipDate", WooComOrder.EstimatedShipDate.CurrentValue);
                command.Parameters.AddWithValue("@MethodCode", WooComOrder.MethodCode.CurrentValue);
                command.Parameters.AddWithSafeValue("@Phone", WooComOrder.Phone.CurrentValue);
                command.Parameters.AddWithSafeValue("@Address1", WooComOrder.Address1.CurrentValue);
                command.Parameters.AddWithSafeValue("@Address2", WooComOrder.Address2.CurrentValue);
                command.Parameters.AddWithSafeValue("@AddressType", WooComOrder.AddressType.CurrentValue);
                command.Parameters.AddWithSafeValue("@City", WooComOrder.City.CurrentValue);
                command.Parameters.AddWithSafeValue("@Country", WooComOrder.Country.CurrentValue);
                command.Parameters.AddWithSafeValue("@Name", WooComOrder.Name.CurrentValue);
                command.Parameters.AddWithSafeValue("@PostalCode", WooComOrder.PostalCode.CurrentValue);
                command.Parameters.AddWithSafeValue("@State", WooComOrder.State.CurrentValue);
                command.Parameters.AddWithValue("@AcknowledgmentStatus", WooComOrder.AcknowledgmentStatus.CurrentValue);
                command.Parameters.AddWithSafeValue("@DateAcknowledged", WooComOrder.DateAcknowledged.CurrentValue);
                command.Parameters.AddWithSafeValue("@SalesOrderId", WooComOrder.SalesOrderId.CurrentValue);

                command.Parameters.AddWithValue("@ShippingRate", WooComOrder.order.shipping_total);
                command.Parameters.AddWithValue("@ShippingRateOriginal", 0);
                command.Parameters.AddWithValue("@TaxPercent", 0);
                command.Parameters.AddWithValue("@TaxShipping", WooComOrder.order.shipping_tax ?? 0);
                command.Parameters.AddWithValue("@TaxIncluded", WooComOrder.order.total_tax > 0);
                command.Parameters.AddWithValue("@TaxAmount", WooComOrder.order.total_tax);
                command.Parameters.AddWithValue("@Subtotal", 0);
                command.Parameters.AddWithValue("@Total", WooComOrder.order.total);
                command.Parameters.AddWithSafeValue("@MarketplaceName", "WooCom");
                command.Parameters.AddWithSafeValue("@MarketplaceId", string.Empty);
                command.Parameters.AddWithValue("@HoldDate", DBNull.Value);
                command.Parameters.AddWithSafeValue("@ChannelOrderId", WooComOrder.order.number);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    DebugLogger.Log(ex.Message);
                    throw;
                }
            }
        }

        private void UpdateWooComOrder(WooComOrder WooComOrder)
        {
            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                UpdateBuilder builder = new UpdateBuilder(command);

                builder.TableName = "WooComOrder";
                builder.KeyFieldName = "WooComOrderId";
                builder.KeyFieldValue = WooComOrder.WooComOrderId.CurrentValue;
                builder.AddField(WooComOrder.WooComOrderId);
                builder.AddField(WooComOrder.CustomerEmailId);
                builder.AddField(WooComOrder.CustomerOrderId);
                builder.AddField(WooComOrder.OrderDate);
                builder.AddField(WooComOrder.PurchaseOrderId);
                builder.AddField(WooComOrder.EstimatedDeliveryDate);
                builder.AddField(WooComOrder.EstimatedShipDate);
                builder.AddField(WooComOrder.MethodCode);
                builder.AddField(WooComOrder.Phone);
                builder.AddField(WooComOrder.Address1);
                builder.AddField(WooComOrder.Address2);
                builder.AddField(WooComOrder.AddressType);
                builder.AddField(WooComOrder.City);
                builder.AddField(WooComOrder.Country);
                builder.AddField(WooComOrder.Name);
                builder.AddField(WooComOrder.PostalCode);
                builder.AddField(WooComOrder.State);
                builder.AddField(WooComOrder.AcknowledgmentStatus);
                builder.AddField(WooComOrder.DateAcknowledged);
                builder.AddField(WooComOrder.SalesOrderId);

                if (builder.FinalizeUpdateQuery())
                    command.ExecuteNonQuery();
            }
        }

        #endregion

        #region Save WooComOrderLine Methods

        public void SaveWooComOrderLine(WooComOrderLine WooComOrderLine)
        {
            if (WooComOrderLine.Status == DataStatus.Inserted)
                InsertWooComOrderLine(WooComOrderLine);
            else if (WooComOrderLine.Status == DataStatus.Changed)
                UpdateWooComOrderLine(WooComOrderLine);

            WooComOrderLine.Update();
        }

        private void InsertWooComOrderLine(WooComOrderLine WooComOrderLine)
        {
            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "pInsertWooComOrderLine";

                command.Parameters.AddWithValue("@LineId", WooComOrderLine.LineId.CurrentValue);
                command.Parameters.AddWithValue("@WooComOrderId", WooComOrderLine.WooComOrderId.CurrentValue);
                command.Parameters.AddWithSafeValue("@SalesOrderLineId", WooComOrderLine.SalesOrderLineId.CurrentValue);
                command.Parameters.AddWithValue("@LineNumber", WooComOrderLine.OrderItemId.CurrentValue);
                command.Parameters.AddWithSafeValue("@Sku", WooComOrderLine.Sku.CurrentValue);
                command.Parameters.AddWithValue("@ProductName", WooComOrderLine.ProductName.CurrentValue);
                command.Parameters.AddWithValue("@ItemPrice", WooComOrderLine.ItemPrice.CurrentValue);
                command.Parameters.AddWithValue("@TaxAmount", WooComOrderLine.TaxAmount.CurrentValue);
                command.Parameters.AddWithValue("@Quantity", WooComOrderLine.Quantity.CurrentValue);
                command.Parameters.AddWithSafeValue("@StatusDate", WooComOrderLine.StatusDate.CurrentValue);
                command.Parameters.AddWithValue("@LineStatus", WooComOrderLine.LineStatus.CurrentValue);
                //command.Parameters.AddWithSafeValue("@ItemId", WooComOrderLine.ItemId.CurrentValue);
                command.Parameters.AddWithSafeValue("@CustomerItemId", WooComOrderLine.CustomerItemId.CurrentValue);
                command.Parameters.AddWithValue("@CommissionAmount", 0m);
                command.Parameters.AddWithValue("@ProductId", 0);
                command.Parameters.AddWithValue("@VariantId", 0);

                command.ExecuteNonQuery();
            }
        }

        private void UpdateWooComOrderLine(WooComOrderLine WooComOrderLine)
        {
            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                UpdateBuilder builder = new UpdateBuilder(command);

                builder.TableName = "WooComOrderLine";
                builder.KeyFieldName = "LineId";
                builder.KeyFieldValue = WooComOrderLine.LineId.CurrentValue;
                builder.AddField(WooComOrderLine.LineId);
                builder.AddField(WooComOrderLine.WooComOrderId);
                builder.AddField(WooComOrderLine.SalesOrderLineId);
                builder.AddField(WooComOrderLine.OrderItemId);
                builder.AddField(WooComOrderLine.Sku);
                builder.AddField(WooComOrderLine.ProductName);
                builder.AddField(WooComOrderLine.ItemPrice);
                builder.AddField(WooComOrderLine.TaxAmount);
                builder.AddField(WooComOrderLine.Quantity);
                builder.AddField(WooComOrderLine.StatusDate);
                builder.AddField(WooComOrderLine.LineStatus);
                //builder.AddField(WooComOrderLine.ItemId);
                builder.AddField(WooComOrderLine.CustomerItemId);

                if (builder.FinalizeUpdateQuery())
                    command.ExecuteNonQuery();
            }
        }

        #endregion

        public List<Product> GetWooComItemInventory()
        {
            List<Product> inventoryUpdates = new List<Product>();

            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandTimeout = 120;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "pGetInventoryForWooCom";

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Product itemInventory = new Product();

                    itemInventory.sku = reader.GetSafeString(0);
                    itemInventory.stock_quantity = reader.GetSafeInt32(1);
                    //itemInventory.inventory_shipping_leadtime_max = reader.GetInt32(2);
                    itemInventory.id = (ulong)reader.GetInt32(3);

                    inventoryUpdates.Add(itemInventory);
                }
            }

            return inventoryUpdates;
        }

        public void UpdateWooComProductVariant(Product item)
        {
            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandTimeout = 120;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "pUpdateWooComProductVariant";

                command.Parameters.AddWithValue("@VariantId", item.id);
                command.Parameters.AddWithSafeValue("@SKU", item.sku);
                command.Parameters.AddWithSafeValue("@Status", item.status);

                command.ExecuteNonQuery();
            }
        }

        public List<Product> GetWooComItemPricing()
        {
            List<Product> pricingUpdates = new List<Product>();

            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandTimeout = 120;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "pGetPricingForWooCom";

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Product itemPrice = new Product();

                    itemPrice.sku = reader.GetString(0);
                    itemPrice.price = reader.GetDecimal(1);
                    itemPrice.id = (ulong)reader.GetInt32(2);
                    pricingUpdates.Add(itemPrice);
                }
            }

            return pricingUpdates;
        }

        public List<WooComOrder> GetShippedWooComOrders()
        {
            List<WooComOrder> orders = new List<WooComOrder>();

            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "pGetShippedWooComOrders";

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    WooComOrder WooComOrderShipped = new WooComOrder();

                    WooComOrderShipped.WooComOrderId.OriginalValue = reader.GetSafeGuid(0);
                    WooComOrderShipped.PurchaseOrderId.OriginalValue = reader.GetInt32(1);
                    WooComOrderShipped.SalesOrderNumber.OriginalValue = reader.GetSafeInt32(2);
                    WooComOrderShipped.TrackingNumber.OriginalValue = reader.GetSafeString(3);
                    WooComOrderShipped.TrackingDate.OriginalValue = reader.GetSafeDateTime(4);
                    WooComOrderShipped.ServiceName.OriginalValue = reader.GetSafeString(5);
                    WooComOrderShipped.CarrierName.OriginalValue = reader.GetSafeString(6);

                    //WooComOrderShipped.TrackingDateIso.OriginalValue = reader.GetSafeString(7);

                    WooComOrderShipped.Status = DataStatus.Unchanged;

                    orders.Add(WooComOrderShipped);
                }

                GetWooComOrderLines(orders, reader);

                reader.Close();
            }

            return orders;
        }

        private void GetWooComOrderLines(List<WooComOrder> orders, SqlDataReader reader)
        {
            reader.NextResult();

            while (reader.Read())
            {
                WooComOrderLine orderLine = new WooComOrderLine();

                orderLine.LineId.OriginalValue = reader.GetSafeGuid(0);
                orderLine.WooComOrderId.OriginalValue = reader.GetSafeGuid(1);
                orderLine.SalesOrderLineId.OriginalValue = reader.GetSafeGuid(2);
                orderLine.OrderItemId.OriginalValue = reader.GetSafeInt32(3);
                orderLine.Sku.OriginalValue = reader.GetSafeString(4);
                orderLine.ProductName.OriginalValue = reader.GetSafeString(5);
                orderLine.ItemPrice.OriginalValue = reader.GetSafeDecimal(6);
                orderLine.TaxAmount.OriginalValue = reader.GetSafeDecimal(7);
                orderLine.Quantity.OriginalValue = reader.GetSafeInt32(8);
                orderLine.UnitOfMeasure.OriginalValue = reader.GetSafeString(9);
                orderLine.StatusDate.OriginalValue = reader.GetSafeDateTime(10);
                orderLine.LineStatus.OriginalValue = reader.GetSafeString(11);
                orderLine.QuantityShipped.OriginalValue = reader.GetSafeInt32(12);

                orderLine.Status = DataStatus.Unchanged;

                var orderHeader = orders.First(o => o.WooComOrderId.OriginalValue == orderLine.WooComOrderId.OriginalValue);
                orderHeader.MyLines.Add(orderLine);
            }
        }

        public Dictionary<string, Guid> GetChannelShipServices()
        {
            Dictionary<string, Guid> channelShipServices = new Dictionary<string, Guid>();

            string serviceName;
            Guid carrierService;

            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "pGetChannelShippingServices";

                command.Parameters.AddWithValue("@ChannelId", WooComProfileSetting.ChannelId.CurrentValue);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    serviceName = reader.GetString(0);
                    carrierService = reader.GetGuid(1);

                    channelShipServices[serviceName.ToUpper()] = carrierService;
                }

                reader.Close();
            }

            return channelShipServices;
        }


        #region Logging

        public void InsertServiceLog(IServiceLog serviceLog)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
                {
                    connection.Open();

                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "pInsertWooComServiceLog";

                    command.Parameters.AddWithValue("@LogId", serviceLog.LogId);
                    command.Parameters.AddWithValue("@LogDate", serviceLog.LogDate);
                    command.Parameters.AddWithValue("@Category", serviceLog.Category);
                    command.Parameters.AddWithSafeValue("@LogGroup", serviceLog.LogGroup);
                    command.Parameters.AddWithValue("@MessageText", serviceLog.MessageText);

                    serviceLog.ProfileSettingId = serviceLog.ProfileSettingId ?? ProfileSettingId;

                    if (serviceLog.ProfileSettingId.HasValue)
                        command.Parameters.AddWithValue("@ProfileSettingId", serviceLog.ProfileSettingId);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                DebugLogger.WriteLineToFile("Error Inserting ServiceLog: [{0}]!", serviceLog.ToString());
                DebugLogger.WriteToFile(ex);
            }
        }

        #endregion

    }
}
