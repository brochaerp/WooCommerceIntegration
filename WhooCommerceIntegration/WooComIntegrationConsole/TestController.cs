using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v3;

namespace AcendaIntegrationConsole
{
    public class testController
    {
        public testController()
        {
            GetApi();
        }

        public async void GetApi()
        {
            RestAPI restAPI = new RestAPI("https://kadrakitchenware.com/wp-json/wc/v3/", "ck_f4e3dc447e55a93c4c8b5334e6575e09211b0071", "cs_bd6c856a8b9458df3a9ad0561fd874f5b3ead40c");
            WCObject wco = new WCObject(restAPI);

            List<Product> products = await wco.Product.GetAll();

            foreach (Product product in products)
            {
                List<ProductAttributeLine> lstattributes = product.attributes;
                Console.WriteLine(product.name);
                foreach (ProductAttributeLine stattribute in lstattributes)
                {
                    ProductAttributeLine attr = stattribute;
                    Console.WriteLine(attr.name.ToString());
                    Console.WriteLine(attr.id.Value.ToString());
                }
            }

        }
            
    }
}