using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcendaSDK.DTOs
{
    public class ProductDTO : BaseDTO
    {
        public Product result { get; set; }
    }

    public class Variant
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public string status { get; set; }
        public string name { get; set; }
        public string sku { get; set; }
        public double price { get; set; }
        public double cost { get; set; }
        public double compare_price { get; set; }
        public double save_price { get; set; }
        public double save_percent { get; set; }
        public int position { get; set; }
        public List<object> images { get; set; }
        public int inventory_quantity { get; set; }
        public int inventory_minimum_quantity { get; set; }
        public bool inventory_tracking { get; set; }
        public string inventory_policy { get; set; }
        public string inventory_shipping_estimate { get; set; }
        public int inventory_shipping_leadtime_min { get; set; }
        public int inventory_shipping_leadtime_max { get; set; }
        public bool inventory_returnable { get; set; }
        public bool has_stock { get; set; }
        public int popularity { get; set; }
        public bool require_shipping { get; set; }
        public bool discountable { get; set; }
        public bool taxable { get; set; }
        public string weight { get; set; }
        public string date_modified { get; set; }
        public string date_created { get; set; }
        public bool enable_instockemail { get; set; }
        public string color { get; set; }
        public string size { get; set; }
    }

    public class Category
    {
        public int id { get; set; }
        public List<int> parent_id { get; set; }
        public string status { get; set; }
        public bool track_status { get; set; }
        public string selection { get; set; }
        public string query { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
        public int position { get; set; }
        public string image_type { get; set; }
        public List<object> image_sort { get; set; }
        public List<Image> images { get; set; }
        public string page_title { get; set; }
        public string meta_description { get; set; }
        public string meta_keywords { get; set; }
        public string date_created { get; set; }
        public string date_modified { get; set; }
    }

}
