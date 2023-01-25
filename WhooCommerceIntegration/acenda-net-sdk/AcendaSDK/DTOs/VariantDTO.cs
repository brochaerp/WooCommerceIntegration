using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcendaSDK.DTOs
{
    public class VariantDTO
    {
        public string id { get; set; }
        public string product_id { get; set; }
        public string status { get; set; }
        public string name { get; set; }
        public string sku { get; set; }
        public string asin { get; set; }
        public string ean { get; set; }
        public string isbn { get; set; }
        public string has_stock { get; set; }
        public double cost { get; set; }
        public string barcode { get; set; }
        public double price { get; set; }
        public double compare_price { get; set; }
        public string popularity { get; set; }
        public string position { get; set; }
        public List<Image> images { get; set; }
        public int inventory_quantity { get; set; }
        public int inventory_minimum_quantity { get; set; }
        public bool inventory_tracking { get; set; }
        public string inventory_policy { get; set; }
        public string inventory_shipping_estimate { get; set; }
        public bool inventory_returnable { get; set; }
        public bool require_shipping { get; set; }
        public bool discountable { get; set; }
        public bool taxable { get; set; }
        public double weight { get; set; }
        public string date_publish { get; set; }
        public string date_expire { get; set; }
        public int inventory_shipping_leadtime_max { get; set; }


    }

    public class VariantInventoryDTO
    {
        public string id { get; set; }
        public string product_id { get; set; }
        public string sku { get; set; }
        public int inventory_quantity { get; set; }
        public int inventory_shipping_leadtime_max { get; set; }


    }

    public class VariantPriceDTO
    {
        public string id { get; set; }
        public string product_id { get; set; }
        public string sku { get; set; }
        public double price { get; set; }
        public bool taxable { get; set; }

    }
}
