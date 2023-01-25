using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcendaSDK.DTOs
{
    public class OrderGetFulfillmentsDTO : BaseDTO
    {
        public List<OrderGetFulfillmentsResult> result { get; set; }
    }

    public class OrderGetFulfillmentsResult 
    {
        public string id { get; set; }
        public string date_created { get; set; }
        public string date_modified { get; set; }
        public string order_id { get; set; }
        public string status { get; set; }
        public string tracking_company { get; set; }
        public List<object> tracking_urls { get; set; }
        public List<string> tracking_numbers { get; set; }
        public string shipping_method { get; set; }
        public List<OrderGetFulfillmentsItem> items { get; set; }
        public List<OrderGetFulfillmentsPackage> packages { get; set; }
    }
    public class OrderGetFulfillmentsPackage
    {
        public double width { get; set; }
        public double height { get; set; }
        public double depth { get; set; }
        public double weight { get; set; }
    }
    public class OrderGetFulfillmentsItem
    {
        public string id { get; set; }
        public string date_created { get; set; }
        public string date_modified { get; set; }
        public string product_id { get; set; }
        public string status { get; set; }
        public string vendor { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public int fulfilled_quantity { get; set; }
        public string fulfillment_status { get; set; }
        public string order_id { get; set; }
        public string returnable { get; set; }
        public string returnable_quantity { get; set; }
        public string tracking_number { get; set; }
    }

}
