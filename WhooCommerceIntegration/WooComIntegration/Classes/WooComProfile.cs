using System;
using System.Runtime.Serialization;
using TkoModel;

namespace WooComIntegration
{

    [Serializable]
    public class WooComProfile : BaseModel
    {

        public GuidField ProfileId { get; private set; }
        public StringField CompanyName { get; private set; }
        public StringField DefaultDirectory { get; private set; }
        public GuidField WarehouseId { get; private set; }
        public BooleanField CreateNewCustomer { get; private set; }
        public GuidField OrderSourceId { get; private set; }
        public GuidField TypeId { get; private set; }
        public GuidField TermId { get; private set; }
        public GuidField CustomerId { get; private set; }
        public StringField TaxableState { get; private set; }
        public GuidField DefaultCarrierServiceId { get; private set; }
        public GuidField FieldSalesForceId { get; private set; }
        public GuidField SalesForceId { get; private set; }
        public GuidField TaxingEntityId { get; private set; }
        public StringField ClientId { get; private set; }
        public StringField ClientSecret { get; private set; }
        public GuidField UploadUserId { get; private set; }
        public StringField EmailTo { get; private set; }
        public GuidField ChannelId { get; set; }

        /// <summary>
        /// Default constructor. Initializes fields and adds them to the collection.
        /// </summary>
        public WooComProfile()
        {
            MyFields.Add(ProfileId = new GuidField() { ColumnName = "SettingId", DisplayName = "Setting", IsRequired = true });
            MyFields.Add(CompanyName = new StringField() { ColumnName = "CompanyName", DisplayName = "Company Name", MaxLength = 50 });
            MyFields.Add(DefaultDirectory = new StringField() { ColumnName = "DefaultDirectory", DisplayName = "Default Directory", MaxLength = 1 });
            MyFields.Add(WarehouseId = new GuidField() { ColumnName = "Warehouse", DisplayName = "Warehouse" });
            MyFields.Add(CreateNewCustomer = new BooleanField() { ColumnName = "CreateNewCustomer", DisplayName = "Create New Customer" });
            MyFields.Add(OrderSourceId = new GuidField() { ColumnName = "OrderSource", DisplayName = "Order Source" });
            MyFields.Add(TypeId = new GuidField() { ColumnName = "Type", DisplayName = "Type" });
            MyFields.Add(TermId = new GuidField() { ColumnName = "Term", DisplayName = "Term" });
            MyFields.Add(CustomerId = new GuidField() { ColumnName = "Customer", DisplayName = "Customer" });
            MyFields.Add(TaxableState = new StringField() { ColumnName = "TaxableState", DisplayName = "Taxable State", MaxLength = 50 });
            MyFields.Add(DefaultCarrierServiceId = new GuidField() { ColumnName = "DefaultCarrierServiceId", DisplayName = "DefaultCarrierServiceId" });
            MyFields.Add(FieldSalesForceId = new GuidField() { ColumnName = "FieldSalesForce", DisplayName = "Field Sales Force" });
            MyFields.Add(SalesForceId = new GuidField() { ColumnName = "SalesForce", DisplayName = "Sales Force" });
            MyFields.Add(TaxingEntityId = new GuidField() { ColumnName = "TaxingEntity", DisplayName = "Taxing Entity" });
            MyFields.Add(ClientId = new StringField() { ColumnName = "ApiUser", DisplayName = "Api User", MaxLength = 50 });
            MyFields.Add(ClientSecret = new StringField() { ColumnName = "ApiSecret", DisplayName = "Api Secret", MaxLength = 50 });
            MyFields.Add(UploadUserId = new GuidField() { ColumnName = "UploadUserId", DisplayName = "Upload User" });
            MyFields.Add(EmailTo = new StringField() { ColumnName = "EmailTo", DisplayName = "Email To", MaxLength = 100 });
            MyFields.Add(ChannelId = new GuidField() { ColumnName = "ChannelId", DisplayName = "Channel Id" });
        }


        /// <summary>
        /// Converts the value of the underlying JetProfile object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current JetProfile object.</returns>
        public override string ToString()
        {
            return CompanyName.CurrentValue.ToString();
        }


    }
}
