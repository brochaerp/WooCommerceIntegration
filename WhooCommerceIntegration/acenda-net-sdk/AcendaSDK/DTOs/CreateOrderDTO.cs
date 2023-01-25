using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcendaSDK.DTOs
{
    
    public class BillingAddress
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone_number { get; set; }
        public string street_line1 { get; set; }
        public string street_line2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
    }

    public class ShippingAddress
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone_number { get; set; }
        public string street_line1 { get; set; }
        public string street_line2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
    }

    public class CreateOrderItem
    {
        public int quantity { get; set; }
        public int product_id { get; set; }
    }

    public class Transaction
    {
        public string transaction_id { get; set; }
        public string type { get; set; }
        public double amount { get; set; }
    }

    public class Payment
    {
        public double amount { get; set; }
        public string status { get; set; }
        public string platform { get; set; }
        public List<Transaction> transactions { get; set; }
    }

    public class CreateOrderDTO
    {
        public string email { get; set; }
        public int? customer_id { get; set; } 
        public BillingAddress billing_address { get; set; }
        public ShippingAddress shipping_address { get; set; }
        public string shipping_method { get; set; }
        public List<CreateOrderItem> items { get; set; }
        public List<Payment> payments { get; set; }
    }
}
