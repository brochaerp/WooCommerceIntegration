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
using AcendaSDK.DTOs;

namespace AcendaService
{
    public class AcendaProvider : ILogProvider
    {
        public AcendaProfile AcendaProfileSetting;

        public Guid? ProfileSettingId { get { return AcendaProfileSetting.ProfileId.CurrentValue; } }

        public AcendaProvider()
        {
            AcendaProfileSetting = new AcendaProfile();
        }

        public AcendaProvider(string companyName)
        {
            AcendaProfileSetting = GetAcendaProfile(companyName);
        }

        public AcendaProvider(AcendaProfile profile)
        {
            AcendaProfileSetting = profile;
        }

        public AcendaProfile GetAcendaProfile(string companyName)
        {
            AcendaProfile profile = new AcendaProfile();

            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM dbo.AcendaProfile WHERE CompanyName = @CompanyName";

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

        public Guid FindAcendaOrder(int purchaseOrderId)
        {
            Guid orderId = Guid.Empty;

            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT AcendaOrderId FROM dbo.AcendaOrder WHERE PurchaseOrderId = @purchaseOrderId";

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

        internal Guid FindAcendaSalesOrderId(string customerPO)
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
                command.Parameters.AddWithValue("@CustomerId", AcendaProfileSetting.CustomerId.CurrentValue);
                command.Parameters.AddWithValue("@WarehouseId", AcendaProfileSetting.WarehouseId.CurrentValue);

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

        public AcendaOrder GetAcendaOrder(int AcendaOrderId)
        {
            Guid orderId = FindAcendaOrder(AcendaOrderId);
            if (orderId != Guid.Empty)
                return GetAcendaOrder(orderId);
            else
                return new AcendaOrder();
        }

        public AcendaOrder GetAcendaOrder(Guid AcendaOrderId)
        {
            AcendaOrder AcendaOrder = new AcendaOrder();

            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "pGetAcendaOrder";

                command.Parameters.AddWithValue("@AcendaOrderId", AcendaOrderId);

                SqlDataReader reader = command.ExecuteReader();
                if (!reader.HasRows)
                    return AcendaOrder;

                reader.Read();
                AcendaOrder.AcendaOrderId.OriginalValue = reader.GetSafeGuid(0);
                AcendaOrder.CustomerEmailId.OriginalValue = reader.GetSafeString(1);
                AcendaOrder.CustomerOrderId.OriginalValue = reader.GetSafeString(2);
                AcendaOrder.OrderDate.OriginalValue = reader.GetSafeDateTime(3);
                AcendaOrder.PurchaseOrderId.OriginalValue = reader.GetInt32(4);
                AcendaOrder.EstimatedDeliveryDate.OriginalValue = reader.GetSafeDateTime(5);
                AcendaOrder.EstimatedShipDate.OriginalValue = reader.GetSafeDateTime(6);
                AcendaOrder.MethodCode.OriginalValue = reader.GetByte(7);
                AcendaOrder.Phone.OriginalValue = reader.GetSafeString(8);
                AcendaOrder.Address1.OriginalValue = reader.GetSafeString(9);
                AcendaOrder.Address2.OriginalValue = reader.GetSafeString(10);
                AcendaOrder.AddressType.OriginalValue = reader.GetSafeString(11);
                AcendaOrder.City.OriginalValue = reader.GetSafeString(12);
                AcendaOrder.Country.OriginalValue = reader.GetSafeString(13);
                AcendaOrder.Name.OriginalValue = reader.GetSafeString(14);
                AcendaOrder.PostalCode.OriginalValue = reader.GetSafeString(15);
                AcendaOrder.State.OriginalValue = reader.GetSafeString(16);
                AcendaOrder.AcknowledgmentStatus.OriginalValue = reader.GetByte(17);
                AcendaOrder.DateAcknowledged.OriginalValue = reader.GetSafeDateTime(18);
                AcendaOrder.SalesOrderId.OriginalValue = reader.GetSafeGuid(19);;

                reader.NextResult();
                while (reader.Read())
                {
                    AcendaOrderLine orderLine = new AcendaOrderLine();
                    orderLine.LineId.OriginalValue = reader.GetSafeGuid(0);
                    orderLine.AcendaOrderId.CurrentValue = reader.GetSafeGuid(1);
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

                    AcendaOrder.MyLines.Add(orderLine);
                }

                reader.Close();
            }

            return AcendaOrder;
        }

        #region Save AcendaOrder Methods

        public AcendaOrder SaveAcendaOrder(AcendaOrder AcendaOrder)
        {
            if (AcendaOrder.Status == DataStatus.Inserted)
                InsertAcendaOrder(AcendaOrder);
            else if (AcendaOrder.Status == DataStatus.Changed)
                UpdateAcendaOrder(AcendaOrder);

            foreach (var item in AcendaOrder.MyLines)
            {
                SaveAcendaOrderLine(item);
            }

            AcendaOrder.Update();


            return AcendaOrder;
        }

        private void InsertAcendaOrder(AcendaOrder AcendaOrder)
        {
            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "pInsertAcendaOrder";

                command.Parameters.AddWithValue("@AcendaOrderId", AcendaOrder.AcendaOrderId.CurrentValue);
                command.Parameters.AddWithSafeValue("@CustomerEmailId", AcendaOrder.CustomerEmailId.CurrentValue);
                command.Parameters.AddWithSafeValue("@CustomerOrderId", AcendaOrder.CustomerOrderId.CurrentValue);
                command.Parameters.AddWithSafeValue("@OrderDate", AcendaOrder.OrderDate.CurrentValue);
                command.Parameters.AddWithSafeValue("@PurchaseOrderId", AcendaOrder.PurchaseOrderId.CurrentValue);
                command.Parameters.AddWithSafeValue("@EstimatedDeliveryDate", AcendaOrder.EstimatedDeliveryDate.CurrentValue);
                command.Parameters.AddWithSafeValue("@EstimatedShipDate", AcendaOrder.EstimatedShipDate.CurrentValue);
                command.Parameters.AddWithValue("@MethodCode", AcendaOrder.MethodCode.CurrentValue);
                command.Parameters.AddWithSafeValue("@Phone", AcendaOrder.Phone.CurrentValue);
                command.Parameters.AddWithSafeValue("@Address1", AcendaOrder.Address1.CurrentValue);
                command.Parameters.AddWithSafeValue("@Address2", AcendaOrder.Address2.CurrentValue);
                command.Parameters.AddWithSafeValue("@AddressType", AcendaOrder.AddressType.CurrentValue);
                command.Parameters.AddWithSafeValue("@City", AcendaOrder.City.CurrentValue);
                command.Parameters.AddWithSafeValue("@Country", AcendaOrder.Country.CurrentValue);
                command.Parameters.AddWithSafeValue("@Name", AcendaOrder.Name.CurrentValue);
                command.Parameters.AddWithSafeValue("@PostalCode", AcendaOrder.PostalCode.CurrentValue);
                command.Parameters.AddWithSafeValue("@State", AcendaOrder.State.CurrentValue);
                command.Parameters.AddWithValue("@AcknowledgmentStatus", AcendaOrder.AcknowledgmentStatus.CurrentValue);
                command.Parameters.AddWithSafeValue("@DateAcknowledged", AcendaOrder.DateAcknowledged.CurrentValue);
                command.Parameters.AddWithSafeValue("@SalesOrderId", AcendaOrder.SalesOrderId.CurrentValue);

                command.ExecuteNonQuery();
            }
        }

        private void UpdateAcendaOrder(AcendaOrder AcendaOrder)
        {
            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                UpdateBuilder builder = new UpdateBuilder(command);

                builder.TableName = "AcendaOrder";
                builder.KeyFieldName = "AcendaOrderId";
                builder.KeyFieldValue = AcendaOrder.AcendaOrderId.CurrentValue;
                builder.AddField(AcendaOrder.AcendaOrderId);
                builder.AddField(AcendaOrder.CustomerEmailId);
                builder.AddField(AcendaOrder.CustomerOrderId);
                builder.AddField(AcendaOrder.OrderDate);
                builder.AddField(AcendaOrder.PurchaseOrderId);
                builder.AddField(AcendaOrder.EstimatedDeliveryDate);
                builder.AddField(AcendaOrder.EstimatedShipDate);
                builder.AddField(AcendaOrder.MethodCode);
                builder.AddField(AcendaOrder.Phone);
                builder.AddField(AcendaOrder.Address1);
                builder.AddField(AcendaOrder.Address2);
                builder.AddField(AcendaOrder.AddressType);
                builder.AddField(AcendaOrder.City);
                builder.AddField(AcendaOrder.Country);
                builder.AddField(AcendaOrder.Name);
                builder.AddField(AcendaOrder.PostalCode);
                builder.AddField(AcendaOrder.State);
                builder.AddField(AcendaOrder.AcknowledgmentStatus);
                builder.AddField(AcendaOrder.DateAcknowledged);
                builder.AddField(AcendaOrder.SalesOrderId);

                if (builder.FinalizeUpdateQuery())
                    command.ExecuteNonQuery();
            }
        }

        #endregion

        #region Save AcendaOrderLine Methods

        public void SaveAcendaOrderLine(AcendaOrderLine AcendaOrderLine)
        {
            if (AcendaOrderLine.Status == DataStatus.Inserted)
                InsertAcendaOrderLine(AcendaOrderLine);
            else if (AcendaOrderLine.Status == DataStatus.Changed)
                UpdateAcendaOrderLine(AcendaOrderLine);

            AcendaOrderLine.Update();
        }

        private void InsertAcendaOrderLine(AcendaOrderLine AcendaOrderLine)
        {
            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "pInsertAcendaOrderLine";

                command.Parameters.AddWithValue("@LineId", AcendaOrderLine.LineId.CurrentValue);
                command.Parameters.AddWithValue("@AcendaOrderId", AcendaOrderLine.AcendaOrderId.CurrentValue);
                command.Parameters.AddWithSafeValue("@SalesOrderLineId", AcendaOrderLine.SalesOrderLineId.CurrentValue);
                command.Parameters.AddWithValue("@LineNumber", AcendaOrderLine.OrderItemId.CurrentValue);
                command.Parameters.AddWithSafeValue("@Sku", AcendaOrderLine.Sku.CurrentValue);
                command.Parameters.AddWithValue("@ProductName", AcendaOrderLine.ProductName.CurrentValue);
                command.Parameters.AddWithValue("@ItemPrice", AcendaOrderLine.ItemPrice.CurrentValue);
                command.Parameters.AddWithValue("@TaxAmount", AcendaOrderLine.TaxAmount.CurrentValue);
                command.Parameters.AddWithValue("@Quantity", AcendaOrderLine.Quantity.CurrentValue);
                command.Parameters.AddWithValue("@UnitOfMeasure", AcendaOrderLine.UnitOfMeasure.CurrentValue);
                command.Parameters.AddWithSafeValue("@StatusDate", AcendaOrderLine.StatusDate.CurrentValue);
                command.Parameters.AddWithValue("@LineStatus", AcendaOrderLine.LineStatus.CurrentValue);
                //command.Parameters.AddWithSafeValue("@ItemId", AcendaOrderLine.ItemId.CurrentValue);
                command.Parameters.AddWithSafeValue("@CustomerItemId", AcendaOrderLine.CustomerItemId.CurrentValue);

                command.ExecuteNonQuery();
            }
        }

        private void UpdateAcendaOrderLine(AcendaOrderLine AcendaOrderLine)
        {
            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                UpdateBuilder builder = new UpdateBuilder(command);

                builder.TableName = "AcendaOrderLine";
                builder.KeyFieldName = "LineId";
                builder.KeyFieldValue = AcendaOrderLine.LineId.CurrentValue;
                builder.AddField(AcendaOrderLine.LineId);
                builder.AddField(AcendaOrderLine.AcendaOrderId);
                builder.AddField(AcendaOrderLine.SalesOrderLineId);
                builder.AddField(AcendaOrderLine.OrderItemId);
                builder.AddField(AcendaOrderLine.Sku);
                builder.AddField(AcendaOrderLine.ProductName);
                builder.AddField(AcendaOrderLine.ItemPrice);
                builder.AddField(AcendaOrderLine.TaxAmount);
                builder.AddField(AcendaOrderLine.Quantity);
                builder.AddField(AcendaOrderLine.UnitOfMeasure);
                builder.AddField(AcendaOrderLine.StatusDate);
                builder.AddField(AcendaOrderLine.LineStatus);
                //builder.AddField(AcendaOrderLine.ItemId);
                builder.AddField(AcendaOrderLine.CustomerItemId);

                if (builder.FinalizeUpdateQuery())
                    command.ExecuteNonQuery();
            }
        }

        #endregion

        public List<Variant> GetAcendaItemInventory()
        {
            List<Variant> inventoryUpdates = new List<Variant>();

            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandTimeout = 120;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "pGetInventoryForAcenda";

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Variant itemInventory = new Variant();

                    itemInventory.sku = reader.GetSafeString(0);
                    itemInventory.inventory_quantity = reader.GetSafeInt32(1);
                    itemInventory.inventory_shipping_leadtime_max = reader.GetInt32(2);

                    inventoryUpdates.Add(itemInventory);
                }
            }

            return inventoryUpdates;
        }

        public List<Variant> GetAcendaItemPricing()
        {
            List<Variant> pricingUpdates = new List<Variant>();

            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandTimeout = 120;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "pGetPricingForAcenda";

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Variant itemPrice = new Variant();

                    itemPrice.sku = reader.GetString(0);
                    itemPrice.price = (double)reader.GetDecimal(1);
                    pricingUpdates.Add(itemPrice);
                }
            }

            return pricingUpdates;
        }

        public List<AcendaOrder> GetShippedAcendaOrders()
        {
            List<AcendaOrder> orders = new List<AcendaOrder>();

            using (SqlConnection connection = new SqlConnection(TkoContainer.DefaultConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "pGetShippedAcendaOrders";

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    AcendaOrder AcendaOrderShipped = new AcendaOrder();

                    AcendaOrderShipped.AcendaOrderId.OriginalValue = reader.GetSafeGuid(0);
                    AcendaOrderShipped.PurchaseOrderId.OriginalValue = reader.GetSafeString(1);
                    AcendaOrderShipped.SalesOrderNumber.OriginalValue = reader.GetSafeInt32(2);
                    AcendaOrderShipped.TrackingNumber.OriginalValue = reader.GetSafeString(3);
                    AcendaOrderShipped.TrackingDate.OriginalValue = reader.GetSafeDateTime(4);
                    AcendaOrderShipped.ServiceName.OriginalValue = reader.GetSafeString(5);
                    AcendaOrderShipped.CarrierName.OriginalValue = reader.GetSafeString(6);
                    //AcendaOrderShipped.TrackingDateIso.OriginalValue = reader.GetSafeString(7);

                    AcendaOrderShipped.Status = DataStatus.Unchanged;

                    orders.Add(AcendaOrderShipped);
                }

                GetAcendaOrderLines(orders, reader);

                reader.Close();
            }

            return orders;
        }

        private void GetAcendaOrderLines(List<AcendaOrder> orders, SqlDataReader reader)
        {
            reader.NextResult();

            while (reader.Read())
            {
                AcendaOrderLine orderLine = new AcendaOrderLine();

                orderLine.LineId.OriginalValue = reader.GetSafeGuid(0);
                orderLine.AcendaOrderId.OriginalValue = reader.GetSafeGuid(1);
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

                var orderHeader = orders.First(o => o.AcendaOrderId.OriginalValue == orderLine.AcendaOrderId.OriginalValue);
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

                command.Parameters.AddWithValue("@ChannelId", AcendaProfileSetting.ChannelId.CurrentValue);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    serviceName = reader.GetString(0);
                    carrierService = reader.GetGuid(1);

                    channelShipServices[serviceName] = carrierService;
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
                    command.CommandText = "pInsertAcendaServiceLog";

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
