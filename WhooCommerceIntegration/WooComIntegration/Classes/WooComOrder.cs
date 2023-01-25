using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TkoModel;
using WooCommerceNET.WooCommerce.v3;

namespace WooComIntegration
{
    public class WooComOrder : BaseModel
    {
        public Order order { get; private set; }

        public GuidField WooComOrderId { get; private set; }
        public StringField CustomerEmailId { get; private set; }
        public StringField CustomerOrderId { get; private set; }
        public StringField OrderNumber { get; private set; }
        public DateTimeField OrderDate { get; private set; }
        public IntField PurchaseOrderId { get; private set; }
        public DateTimeField EstimatedDeliveryDate { get; private set; }
        public DateTimeField EstimatedShipDate { get; private set; }
        public IntField MethodCode { get; private set; }
        public StringField ShippingMethodName { get; private set; }
        public StringField Phone { get; private set; }
        public StringField Address1 { get; private set; }
        public StringField Address2 { get; private set; }
        public StringField AddressType { get; private set; }
        public StringField City { get; private set; }
        public StringField Country { get; private set; }
        public StringField Name { get; private set; }
        public StringField PostalCode { get; private set; }
        public StringField State { get; private set; }

        public ByteField AcknowledgmentStatus { get; private set; }
        public DateTimeField DateAcknowledged { get; private set; }
        public GuidField SalesOrderId { get; private set; }
        public IntField SalesOrderNumber { get; private set; }
        public StringField TrackingNumber { get; private set; }
        public DateTimeField TrackingDate { get; private set; }
        public StringField CarrierName { get; private set; }
        public StringField ServiceName { get; private set; }
        public MoneyField ShippingRate { get; private set; }
        public MoneyField ShippingRateOriginal { get; private set; }
        public DecimalField TaxPercent { get; private set; }
        public BooleanField TaxShipping { get; private set; }
        public BooleanField TaxIncluded { get; private set; }
        public MoneyField TaxAmount { get; private set; }
        public MoneyField Subtotal { get; private set; }
        public MoneyField Total { get; private set; }
        public StringField MarketplaceName { get; private set; }
        public StringField MarketplaceId { get; private set; }

        public List<WooComOrderLine> MyLines;

        public WooComOrder()
        {
            MyFields.Add(WooComOrderId = new GuidField() { ColumnName = "WooComOrderId", DisplayName = "WooCom Order", IsRequired = true });
            MyFields.Add(PurchaseOrderId = new IntField() { ColumnName = "PurchaseOrderId", DisplayName = "PurchaseOrder" });
            MyFields.Add(OrderNumber = new StringField() { ColumnName = "OrderNumber", DisplayName = "OrderNumber" });
            MyFields.Add(CustomerEmailId = new StringField() { ColumnName = "customerEmailId", DisplayName = "customerEmailId", MaxLength = 50 });
            MyFields.Add(CustomerOrderId = new StringField() { ColumnName = "customerOrderId", DisplayName = "customerOrderId", MaxLength = 50 });
            MyFields.Add(OrderDate = new DateTimeField() { ColumnName = "orderDate", DisplayName = "orderDate" });
            MyFields.Add(EstimatedDeliveryDate = new DateTimeField() { ColumnName = "estimatedDeliveryDate", DisplayName = "estimatedDeliveryDate" });
            MyFields.Add(EstimatedShipDate = new DateTimeField() { ColumnName = "estimatedShipDate", DisplayName = "estimatedShipDate" });
            MyFields.Add(MethodCode = new IntField() { ColumnName = "methodCode", DisplayName = "methodCode" });
            MyFields.Add(ShippingMethodName = new StringField() { ColumnName = "ShippingMethodName", DisplayName = "Shipping Method Name", MaxLength = 50 });
            MyFields.Add(Phone = new StringField() { ColumnName = "phone", DisplayName = "phone", MaxLength = 50 });
            MyFields.Add(Address1 = new StringField() { ColumnName = "address1", DisplayName = "address1", MaxLength = 50 });
            MyFields.Add(Address2 = new StringField() { ColumnName = "address2", DisplayName = "address2", MaxLength = 50 });
            MyFields.Add(AddressType = new StringField() { ColumnName = "addressType", DisplayName = "addressType", MaxLength = 50 });
            MyFields.Add(City = new StringField() { ColumnName = "city", DisplayName = "city", MaxLength = 50 });
            MyFields.Add(Country = new StringField() { ColumnName = "country", DisplayName = "country", MaxLength = 50 });
            MyFields.Add(Name = new StringField() { ColumnName = "name", DisplayName = "name", MaxLength = 50 });
            MyFields.Add(PostalCode = new StringField() { ColumnName = "postalCode", DisplayName = "postalCode", MaxLength = 50 });
            MyFields.Add(State = new StringField() { ColumnName = "state", DisplayName = "state", MaxLength = 50 });
            MyFields.Add(AcknowledgmentStatus = new ByteField() { ColumnName = "AcknowledgmentStatus", DisplayName = "Acknowledged Status" });
            MyFields.Add(DateAcknowledged = new DateTimeField() { ColumnName = "DateAcknowledged", DisplayName = "Date Acknowledged" });
            MyFields.Add(SalesOrderId = new GuidField() { ColumnName = "SalesOrderId", DisplayName = "Sales Order" });
            MyFields.Add(SalesOrderNumber = new IntField() { ColumnName = "SalesOrderNumber", DisplayName = "Sales Order Number", CanBeZero = true, BelongsToTable = false });
            MyFields.Add(TrackingNumber = new StringField() { ColumnName = "TrackingNumber", DisplayName = "TrackingNumber", MaxLength = 50, BelongsToTable = false });
            MyFields.Add(TrackingDate = new DateTimeField() { ColumnName = "TrackingDate", DisplayName = "TrackingDate", BelongsToTable = false });
            MyFields.Add(CarrierName = new StringField() { ColumnName = "CarrierName", DisplayName = "CarrierName", MaxLength = 50, BelongsToTable = false });
            MyFields.Add(ServiceName = new StringField() { ColumnName = "ServiceName", DisplayName = "ServiceName", MaxLength = 50, BelongsToTable = false });
            MyFields.Add(ShippingRate = new MoneyField() { ColumnName = "ShippingRate", DisplayName = "ShippingRate", CanBeZero = true });
            MyFields.Add(ShippingRateOriginal = new MoneyField() { ColumnName = "ShippingRateOriginal", DisplayName = "ShippingRateOriginal", CanBeZero = true });
            MyFields.Add(TaxPercent = new DecimalField() { ColumnName = "TaxPercent", DisplayName = "TaxPercent", CanBeZero = true });
            MyFields.Add(TaxShipping = new BooleanField() { ColumnName = "TaxShipping", DisplayName = "TaxShipping" });
            MyFields.Add(TaxIncluded = new BooleanField() { ColumnName = "TaxIncluded", DisplayName = "TaxIncluded" });
            MyFields.Add(TaxAmount = new MoneyField() { ColumnName = "TaxAmount", DisplayName = "TaxAmount", CanBeZero = true });
            MyFields.Add(Subtotal = new MoneyField() { ColumnName = "Subtotal", DisplayName = "Subtotal", CanBeZero = true });
            MyFields.Add(Subtotal = new MoneyField() { ColumnName = "Subtotal", DisplayName = "Subtotal", CanBeZero = true });
            MyFields.Add(Total = new MoneyField() { ColumnName = "Total", DisplayName = "Total", CanBeZero = true });
            MyFields.Add(MarketplaceName = new StringField() { ColumnName = "MarketplaceName", DisplayName = "MarketplaceName", MaxLength = 50 });
            MyFields.Add(MarketplaceId = new StringField() { ColumnName = "MarketplaceId", DisplayName = "MarketplaceId", MaxLength = 50 });


            MyLines = new List<WooComOrderLine>();
        }

        public override CanSaveResult CanSave()
        {
            CanSaveResult result = base.CanSave();
            result = MyLines.CanSave(result);

            return result;
        }

        public override List<string> CanSaveErrors()
        {
            List<string> errors = base.CanSaveErrors();
            errors.AddRange(MyLines.CanSaveErrors());

            return errors;
        }

        public override void Clone(BaseModel model)
        {
            base.Clone(model);
            MyLines.Clone((model as WooComOrder).MyLines);
        }

        public WooComOrder CopyFromWooComOrder(Order orderDetails)
        {
            order = orderDetails;

            if (orderDetails != null)
            {
                if (WooComOrderId.OriginalValue == Guid.Empty)
                    WooComOrderId.OriginalValue = Guid.NewGuid();

                PurchaseOrderId.OriginalValue = (int)orderDetails.id;
                OrderNumber.OriginalValue = orderDetails.number;
                CustomerEmailId.OriginalValue = orderDetails.billing?.email;

                //CustomerOrderId.OriginalValue = orderDetails.customerOrderId;
                OrderDate.OriginalValue = orderDetails.date_created;
                //EstimatedDeliveryDate.OriginalValue = orderDetails.shippingInfo.estimatedDeliveryDate;
                //EstimatedShipDate.OriginalValue = orderDetails.shippingInfo.estimatedShipDate;
                Name.OriginalValue = $"{orderDetails.shipping.first_name}  {orderDetails.shipping.last_name}";
                //ShippingMethodName.OriginalValue = orderDetails.shipping?.FirstOrDefault()?.name;
                //MethodCode.OriginalValue = orderDetails.shipping_method;
                Phone.OriginalValue = orderDetails.billing.phone;
                Address1.OriginalValue = orderDetails.shipping.address_1;
                Address2.OriginalValue = orderDetails.shipping.address_2;
                //AddressType.OriginalValue = orderDetails.shippingInfo.postalAddress.addressType;
                City.OriginalValue = orderDetails.shipping.city;
                State.OriginalValue = orderDetails.shipping.state;
                PostalCode.OriginalValue = orderDetails.shipping.postcode;
                Country.OriginalValue = orderDetails.shipping.address_1;
                ShippingRate.OriginalValue = (decimal)orderDetails.shipping_total;
                //ShippingRateOriginal.OriginalValue = (decimal)orderDetails.shipping_rate_original;
                //TaxPercent.OriginalValue = (decimal)orderDetails.tax_percent;
                TaxShipping.OriginalValue = orderDetails.shipping_tax > 0;
                TaxIncluded.OriginalValue = orderDetails.total_tax > 0;
                TaxAmount.OriginalValue = (decimal)orderDetails.total_tax;
                //Subtotal.OriginalValue = (decimal)orderDetails.subtotal;
                Total.OriginalValue = (decimal)orderDetails.total;
                //MarketplaceName.OriginalValue = orderDetails.marketplace_name;
                //MarketplaceId.OriginalValue = orderDetails.marketplace_id;

                foreach (var item in orderDetails.line_items)
                {
                    WooComOrderLine line = MyLines.FirstOrDefault(l => l.OrderItemId.CurrentValue == Convert.ToInt32(item.id));
                    if (line == null)
                    {
                        line = (WooComOrderLine)item;
                        line.WooComOrderId.OriginalValue = WooComOrderId.CurrentValue;
                        MyLines.Add(line);
                    }
                }
            }

            return this;
        }

        public static explicit operator WooComOrder(Order orderDetails)
        {
            WooComOrder WooComOrder = new WooComOrder();

            WooComOrder.order = orderDetails;

            if (orderDetails != null)
            {
                if (WooComOrder.WooComOrderId.OriginalValue == Guid.Empty)
                    WooComOrder.WooComOrderId.OriginalValue = Guid.NewGuid();

                WooComOrder.PurchaseOrderId.OriginalValue = (int)orderDetails.id;
                WooComOrder.OrderNumber.OriginalValue = orderDetails.number;
                WooComOrder.CustomerEmailId.OriginalValue = orderDetails.billing.email;
                //WooComOrder.CustomerOrderId.OriginalValue = orderDetails.customerOrderId;
                WooComOrder.OrderDate.OriginalValue = orderDetails.date_created;
                //WooComOrder.EstimatedDeliveryDate.OriginalValue = orderDetails.shippingInfo.estimatedDeliveryDate;
                //WooComOrder.EstimatedShipDate.OriginalValue = orderDetails.shippingInfo.estimatedShipDate;
                //WooComOrder.ShippingMethodName.OriginalValue = orderDetails.shipping?.FirstOrDefault()?.name;
                //WooComOrder.MethodCode.OriginalValue = orderDetails.shipping_method;
                WooComOrder.Phone.OriginalValue = orderDetails.billing.phone;
                WooComOrder.Address1.OriginalValue = orderDetails.shipping.address_1;
                WooComOrder.Address2.OriginalValue = orderDetails.shipping.address_2;
                //WooComOrder.AddressType.OriginalValue = orderDetails.shippingInfo.postalAddress.addressType;
                WooComOrder.City.OriginalValue = orderDetails.shipping.city;
                WooComOrder.State.OriginalValue = orderDetails.shipping.state;
                WooComOrder.Country.OriginalValue = orderDetails.shipping.country;
                WooComOrder.Name.OriginalValue = $"{orderDetails.shipping.first_name}  {orderDetails.shipping.last_name}";
                WooComOrder.PostalCode.OriginalValue = orderDetails.shipping.postcode;
                //WooComOrder.AcknowledgedStatus.OriginalValue = orderDetails.shippingInfo.postalAddress.addressType;
                WooComOrder.ShippingRate.OriginalValue = (decimal)orderDetails.shipping_total;
                //WooComOrder.ShippingRateOriginal.OriginalValue = (decimal)orderDetails.shipping_rate_original;
                //WooComOrder.TaxPercent.OriginalValue = (decimal)orderDetails.tax_percent;
                WooComOrder.TaxShipping.OriginalValue = orderDetails.shipping_tax > 0;
                WooComOrder.TaxIncluded.OriginalValue = orderDetails.total_tax > 0;
                WooComOrder.TaxAmount.OriginalValue = (decimal)orderDetails.total_tax;
                //WooComOrder.Subtotal.OriginalValue = (decimal)orderDetails.subtotal;
                WooComOrder.Total.OriginalValue = (decimal)orderDetails.total;
                //WooComOrder.MarketplaceName.OriginalValue = orderDetails.marketplace_name;
                //WooComOrder.MarketplaceId.OriginalValue = orderDetails.marketplace_id;

                foreach (var item in orderDetails.line_items)
                {
                    WooComOrderLine line = (WooComOrderLine)item;
                    line.WooComOrderId.OriginalValue = WooComOrder.WooComOrderId.CurrentValue;

                    WooComOrder.MyLines.Add(line);
                }

                WooComOrder.Status = DataStatus.Inserted;
            }

            return WooComOrder;
        }

        /// <summary>
        /// Converts the value of the underlying WooComOrder object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current WooComOrder object.</returns>
        public override string ToString()
        {
            return string.Format("WooComOrder ( {0} )", PurchaseOrderId.CurrentValue);
        }

        public bool IsEmpty
        {
            get
            {
                return (WooComOrderId.CurrentValue == Guid.Empty && PurchaseOrderId.CurrentValue == 0);
            }
        }

        public bool IsNotYetAcknowledged
        {
            get
            {
                // Check if the order is not yet Acknowledged (Created OR 255 = Rejected).
                //return (AcknowledgmentStatus.CurrentValue == (byte)orderLineStatusValueType.Created || AcknowledgmentStatus.CurrentValue == 255);
                return false;
            }
        }
    }
}
