using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcendaSDK.DTOs
{
    public class OrderListDTO : BaseDTO
    {
        public List<OrderListResult> result { get; set; }


    }
    public class FraudResults
    {
        public string maxmind { get; set; }
    }
    public class OrderListResult
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
        public string subtotal { get; set; }
        public string tax { get; set; }
        public string tax_original { get; set; }
        public string total { get; set; }
        public string charge_amount { get; set; }
        public string unsettled { get; set; }
        public string transaction_status { get; set; }
        public string fulfillment_status { get; set; }
        public bool fraud_check { get; set; }
        public FraudResults fraud_results { get; set; }
        public string marketplace_name { get; set; }
        public string marketplace_id { get; set; }
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
    }

}
