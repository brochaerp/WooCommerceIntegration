using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcendaSDK.DTOs
{
    public class ProductListDTO  :BaseDTO
    { 
        public List<Product> result { get; set; }
    }
    public class VariantOption
    {
        public string name { get; set; }
        public int position { get; set; }
        public List<string> values { get; set; }
    }

    public class Image
    {
        public string id { get; set; }
        public string date_created { get; set; }
        public string date_modified { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string mini { get; set; }
        public string thumbnail { get; set; }
        public string standard { get; set; }
        public string large { get; set; }
        public string retina { get; set; }
        public string original { get; set; }

       
    }

    public class Product
    {
        public int id { get; set; }
        public string group { get; set; }
        public string status { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
        public List<object> collection_id { get; set; }
        public List<int> category_id { get; set; }
        public List<object> customer_group_id { get; set; }
        public int popularity { get; set; }
        public string brand { get; set; }
        public string type { get; set; }
        public List<string> tags { get; set; }
        public string description { get; set; }
        public List<object> cross_sellers { get; set; }
        public int review_score { get; set; }
        public List<VariantOption> variant_options { get; set; }
        public List<string> options { get; set; }
        public List<Image> images { get; set; }
        public List<object> videos { get; set; }
        public List<object> dynamic_attributes { get; set; }
        public List<object> personalization_options { get; set; }
        public string page_title { get; set; }
        public string meta_keywords { get; set; }
        public string meta_description { get; set; }
        public string date_modified { get; set; }
        public string date_created { get; set; }
        public string title { get; set; }
        public string thumbnail { get; set; }
        public string url { get; set; }
        public List<string> category { get; set; }
        public List<object> collections { get; set; }
        public List<Variant> variants { get; set; }
        public List<Category> categories { get; set; }

    }

   

}
