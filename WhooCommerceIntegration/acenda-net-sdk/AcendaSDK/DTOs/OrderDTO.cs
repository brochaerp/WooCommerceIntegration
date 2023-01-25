using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcendaSDK.DTOs
{


    public class Item
    {
        public int id { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }
        public int variant_id { get; set; }
        public string status { get; set; }
        public string name { get; set; }
        public string sku { get; set; }
        public string vendor { get; set; }
        public int quantity { get; set; }
        public int fulfilled_quantity { get; set; }
        public string fulfillment_status { get; set; }
        public int ordered_quantity { get; set; }
        public bool backorder { get; set; }
        public double price { get; set; }
        public double ordered_price { get; set; }
        public string marketplace_name { get; set; }
        public string date_created { get; set; }
        public string date_modified { get; set; }
        public bool taxable { get; set; }
        public bool returnable { get; set; }
    }

    public class Fulfillment
    {
        public int id { get; set; }
        public string date_modified { get; set; }
        public string date_created { get; set; }
        public string order_id { get; set; }
        public string status { get; set; }
        public string tracking_company { get; set; }
        public List<object> tracking_urls { get; set; }
        public List<object> tracking_numbers { get; set; }
        public string shipping_method { get; set; }
        public List<Item> items { get; set; }
        public List<object> packages { get; set; }
    }

    public class Shipping
    {
        public int id { get; set; }
        public string date_modified { get; set; }
        public string date_created { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string rule_by { get; set; }
        public string carrier_name { get; set; }
        public string carrier_method { get; set; }
        public int position { get; set; }
        public int zone_id { get; set; }
    }

    public class OrderResult
    {
        public int id { get; set; }
        public string date_modified { get; set; }
        public string date_created { get; set; }
        public string order_number { get; set; }
        public string status { get; set; }
        public string email { get; set; }
        public string ip { get; set; }
        public string shipping_first_name { get; set; }
        public string shipping_last_name { get; set; }
        public string shipping_phone_number { get; set; }
        public string shipping_street_line1 { get; set; }
        public string shipping_street_line2 { get; set; }
        public string shipping_city { get; set; }
        public string shipping_state { get; set; }
        public string shipping_zip { get; set; }
        public string shipping_country { get; set; }
        public int shipping_method { get; set; }
        public double shipping_rate { get; set; }
        public double shipping_rate_original { get; set; }
        public double tax_percent { get; set; }
        public bool tax_shipping { get; set; }
        public bool tax_included { get; set; }
        public int returns_pending { get; set; }
        public string returns_rma_numbers { get; set; }
        public int returnable_items { get; set; }
        public bool giftlist_present { get; set; }
        public double subtotal { get; set; }
        public double tax { get; set; }
        public double tax_original { get; set; }
        public double total { get; set; }
        public double charge_amount { get; set; }
        public double unsettled { get; set; }
        public string transaction_status { get; set; }
        public string fulfillment_status { get; set; }
        public bool fraud_check { get; set; }
        public FraudResults fraud_results { get; set; }
        public string marketplace_name { get; set; }
        public string marketplace_id { get; set; }
        public string marketplace_order_number { get; set; }
        public bool review_request_sent { get; set; }
        public string shipping_address { get; set; }
        public string name { get; set; }
        public double discount_price { get; set; }
        public double discount_shipping_price { get; set; }
        public double item_subtotal { get; set; }
        public double adjusted_subtotal { get; set; }
        public bool calculate_tax { get; set; }
        public string cancellation_window { get; set; }
        public double fraud_score { get; set; }
        public bool iscancellable { get; set; }
        public List<Item> items { get; set; }
        public List<object> returns { get; set; }
        public List<Fulfillment> fulfillments { get; set; }
        public List<object> payments { get; set; }
        public List<Shipping> shipping { get; set; }
    }

    public class OrderDTO : BaseDTO
    {

        public OrderResult result { get; set; }
    }
}
