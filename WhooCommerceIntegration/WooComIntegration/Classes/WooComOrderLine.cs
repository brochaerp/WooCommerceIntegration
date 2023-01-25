using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TkoModel;
using WooCommerceNET.WooCommerce.v2;

namespace WooComIntegration
{
    public class WooComOrderLine : BaseModel
    {

        OrderLineItem orderLine;
        public GuidField LineId { get; private set; }
        public GuidField WooComOrderId { get; private set; }
        public GuidField SalesOrderLineId { get; private set; }
        public IntField OrderItemId { get; private set; }
        public StringField Sku { get; private set; }
        public StringField ProductName { get; private set; }
        public DecimalField ItemPrice { get; private set; }
        public DecimalField TaxAmount { get; private set; }
        public DecimalField Shipping { get; private set; }
        public DecimalField Quantity { get; private set; }
        public StringField UnitOfMeasure { get; private set; }
        public DateField StatusDate { get; private set; }
        public StringField LineStatus { get; private set; }

        public GuidField ItemId { get; private set; }
        public GuidField CustomerItemId { get; private set; }
        public StringField ItemNumber { get; private set; }
        public StringField QuantityUm { get; private set; }
        public DecimalField QuantityUmRatio { get; private set; }
        public ByteField ItemStatus { get; private set; }
        public DecimalField QuantityAvailable { get; private set; }
        public IntField QuantityShipped { get; private set; }

        public WooComOrderLine()
        {
            MyFields.Add(LineId = new GuidField() { ColumnName = "LineId", DisplayName = "LineId", IsRequired = true });
            MyFields.Add(WooComOrderId = new GuidField() { ColumnName = "WooComOrderId", DisplayName = "WooComOrderId", IsRequired = true });
            MyFields.Add(SalesOrderLineId = new GuidField() { ColumnName = "SalesOrderLineId", DisplayName = "SalesOrderLineId", IsRequired = true });
            MyFields.Add(OrderItemId = new IntField() { ColumnName = "LineNumber", DisplayName = "LineNumber", IsRequired = true });
            MyFields.Add(Sku = new StringField() { ColumnName = "Sku", DisplayName = "Sku", MaxLength = 50 });
            MyFields.Add(ProductName = new StringField() { ColumnName = "ProductName", DisplayName = "ProductName", MaxLength = 50 });
            MyFields.Add(ItemPrice = new DecimalField() { ColumnName = "ItemPrice", DisplayName = "ItemPrice", IsRequired = true, CanBeZero = true });
            MyFields.Add(TaxAmount = new DecimalField() { ColumnName = "TaxAmount", DisplayName = "TaxAmount", IsRequired = true, CanBeZero = true });
            MyFields.Add(Shipping = new DecimalField() { ColumnName = "Shipping", DisplayName = "Shipping", IsRequired = true, CanBeZero = true });
            MyFields.Add(Quantity = new DecimalField() { ColumnName = "Quantity", DisplayName = "Quantity", IsRequired = true, CanBeZero = true });
            MyFields.Add(UnitOfMeasure = new StringField() { ColumnName = "UnitOfMeasure", DisplayName = "UnitOfMeasure", MaxLength = 50 });
            MyFields.Add(StatusDate = new DateField() { ColumnName = "StatusDate", DisplayName = "StatusDate" });
            MyFields.Add(LineStatus = new StringField() { ColumnName = "LineStatus", DisplayName = "LineStatus", MaxLength = 50 });
            MyFields.Add(ItemId = new GuidField() { ColumnName = "ItemId", DisplayName = "ItemId", IsRequired = false, BelongsToTable = false });
            MyFields.Add(CustomerItemId = new GuidField() { ColumnName = "CustomerItemId", DisplayName = "CustomerItemId", IsRequired = true });
            MyFields.Add(ItemNumber = new StringField() { ColumnName = "ItemNumber", DisplayName = "ItemNumber", MaxLength = 50 });
            MyFields.Add(QuantityUm = new StringField() { ColumnName = "QuantityUm", DisplayName = "QuantityUm", MaxLength = 50 });
            MyFields.Add(QuantityUmRatio = new DecimalField() { ColumnName = "QuantityUmRatio", DisplayName = "QuantityUmRatio", CanBeZero = true });
            MyFields.Add(ItemStatus = new ByteField() { ColumnName = "ItemStatus", DisplayName = "ItemStatus" });
            MyFields.Add(QuantityAvailable = new DecimalField() { ColumnName = "QuantityAvailable", DisplayName = "QuantityAvailable", BelongsToTable = false });
            MyFields.Add(QuantityShipped = new IntField() { ColumnName = "QuantityShipped", DisplayName = "Quantity Shipped", IsRequired = true, CanBeZero = true, BelongsToTable = false });
        }

        public void SetItemValues(ItemSearchResult searchResult)
        {
            ItemId.CurrentValue = searchResult.ItemId;
            CustomerItemId.CurrentValue = searchResult.CustomerItemId;
            ItemNumber.CurrentValue = searchResult.ItemNumber;
            QuantityUm.CurrentValue = searchResult.QuantityUm;
            QuantityUmRatio.CurrentValue = searchResult.QuantityUmRatio;
            //ItemStatus.CurrentValue = (ItemsStatus).searchResult.ItemStatus;
            QuantityAvailable.CurrentValue = searchResult.QuantityAvailable;
            //
            //
        }

        public static explicit operator WooComOrderLine(OrderLineItem line)
        {
            WooComOrderLine WooComOrderLine = new WooComOrderLine();

            WooComOrderLine.orderLine = line;

            if (line != null)
            {
                if (WooComOrderLine.LineId.OriginalValue == Guid.Empty)
                    WooComOrderLine.LineId.OriginalValue = Guid.NewGuid();

                try
                {
                    WooComOrderLine.OrderItemId.OriginalValue = Convert.ToInt32(line.id);
                    WooComOrderLine.Sku.OriginalValue = line.sku;
                    WooComOrderLine.ProductName.OriginalValue = line.name;
                    WooComOrderLine.ItemPrice.OriginalValue = (decimal)line.price;
                    //WooComOrderLine.TaxAmount.OriginalValue = line.taxable;
                    //if (line.charges.Length > 1)
                    //{
                    //    WooComOrderLine.Shipping.OriginalValue = line.charges[1].chargeAmount.amount;
                    //    if (line.charges[1].tax != null)
                    //        WooComOrderLine.TaxAmount.OriginalValue += line.charges[1].tax.taxAmount.amount;
                    //}
                    WooComOrderLine.Quantity.OriginalValue = line.quantity ?? 0;
                    //WooComOrderLine.LineStatus.OriginalValue = line.fulfillment_status;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error converting orderLineType to WooComOrderLine!", ex);
                }

                WooComOrderLine.Status = DataStatus.Inserted;
            }

            return WooComOrderLine;
        }
    }
}
