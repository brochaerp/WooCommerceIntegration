# Acenda .net SDK for .net 4.6+

# Authentication
Obtain your ClientId and Client Secret by creating a developer account: https://developer.acenda.com
Create an application and you will have your credentials.

## IMPORTANT: Make sure you select the desired access scope for your application. 
Now your app status is pending. Contact acenda development team for the approval of your app.
Once your app is approved, you can use the SDK as below:

There are predefined Services for easy access to most used acenda entities: Customer, Product,Order, Inventory
For rest of the services you can use the "Generic Service"

# Fill in below fields with client id, client secret and your store name
      ServiceFactory serviceFactory = new ServiceFactory("<CLIENT_ID>", "<CLIENT_SECRET>", "<STORE_NAME>");
      
# Create a Service Instance      
      ProductService product = (ProductService)serviceFactory.GetService(AcendaSDK.Enums.ServiceType.Product);
      
      OrderService order = (OrderService)serviceFactory.GetService(AcendaSDK.Enums.ServiceType.Order);
      
      InventoryService inventory = (InventoryService)serviceFactory.GetService(AcendaSDK.Enums.ServiceType.Inventory);
      
      CustomerService customer = (CustomerService)serviceFactory.GetService(AcendaSDK.Enums.ServiceType.Customer);
      
 # Define the api endpoint of the generic service (ex "customer")
      GenericService generic = (GenericService)serviceFactory.GetService(AcendaSDK.Enums.ServiceType.Generic, "customer");
      
# Get Customer By Id       
      CustomerDTO customerDTO = customer.GetById<CustomerDTO>("3191680");

# Get All Customers
      CustomerListDTO customerListDTO = customer.GetAll<CustomerListDTO>();

# Update Customer

      BaseDTO customerUpdateResult = customer.Update("3191680", new CreateUpdateCustomerDTO()
      {
          //first_name = "Alex",
          //last_name = "Vivian",
          //email = "alex@acenda.com",
          phone_number = "555-555-5555"
      });

      //Delete a customer by Id
      //BaseDTO customerDeleteResult = customer.Delete("3191677");


# Create a customer using generic service
To use generic service, create a DTO derived from the expected return value or input data.

      BaseDTO genericCreateResult = generic.Create(new CreateUpdateCustomerDTO()
      {
          first_name = "Bob",
          last_name = "Smith",
          email = "bob33@acenda.com",
          phone_number = "123-123-1234"
      });
# Get List of Customers using Generic Service
      //Get List of customers by using Generic Service

      CustomerListDTO customerListFromGenericDTO = generic.GetAll<CustomerListDTO>();

# Get List of all Products
      ProductListDTO productDTOs = product.GetAll<ProductListDTO>();

      //Get Product By Id
      ProductDTO productDTO = product.GetById<ProductDTO>("2");

      //Get Product Variant By Id
      ProductVariantsDTO productVariants = product.GetVariants("2");
 # Get a specific order and list of Orders
      //Get List of Orders
      OrderListDTO orderListDTO = order.GetAll<OrderListDTO>();

      //Get Order By Id
      OrderDTO orderDTO = order.GetById<OrderDTO>("2562634");

 # Create Customer using Customer Service
      //Create a customer using customer service
      BaseDTO customerCreateResult = customer.Create(new CreateUpdateCustomerDTO()
      {
          first_name = "Bob",
          last_name = "Smith",
          email = "bob19@acenda.com",
          phone_number = "123-123-1234"
      });

 # Update Inventory
      //Update inventory of a variant by Id (5 is variantId)
      BaseDTO inventoryUpdateResult = inventory.Update("5", new VariantDTO() { inventory_quantity = "200" });
 # Create Order
      BillingAddress billingAddress = new BillingAddress()
      {
          first_name = "bob",
          last_name = "smith",
          phone_number = "123-123-1234",
          street_line1 = "123 Test ln. ",

          city = "San Diego",
          state = "CA",
          zip = "92101",
          country = "US"
      };
      ShippingAddress shippingAddress = new ShippingAddress()
      {
          first_name = "bob",
          last_name = "smith",
          phone_number = "123-123-1234",
          street_line1 = "123 Test ln. ",

          city = "San Diego",
          state = "CA",
          zip = "92101",
          country = "US"
      };
      List<CreateOrderItem> creatOrderItem = new List<CreateOrderItem>();
      creatOrderItem.Add(new CreateOrderItem()
      {
          quantity = 1,
          product_id = 2


      });

      CreateOrderDTO createOrderDTO = new CreateOrderDTO()
      {
          email = "cob@acenda.com",

          billing_address = billingAddress,


          shipping_address = shippingAddress,


          items = creatOrderItem,



      };
      //Create order
      
      BaseDTO orderCreateResult = order.Create(createOrderDTO);
# Update Order
    order.Update("2562542", new UpdateOrderDTO()
    {
        status = "closed",
        email = "bob@acenda.com",
    });
# Create fulfillment for order         
      BaseDTO result = order.CreateFulfillments("2562599", new FulfillmentsDTO()
      {
          tracking_numbers = new List<string>() { "1Z999AA10123456784" },
          tracking_urls = new List<string>() { "https://www.ups.com/track?loc=en_US&tracknum=1Z999AA10123456784/trackdetails" },
          tracking_company = "UPS",
          shipping_method = "Ground",
          status = "success",
          items = new List<FulfillmentItems>()
      {
          new FulfillmentItems()
          {
              id = 74,//item id inside order object
              quantity  =1,
          }
      }

      });
      
# Get Fulfillment by Id of Order
      
          var orderGetFulfillmentsResult = order.GetFulfillments("2562611", "86");
          
# Example project

Here is an example project using this SDK. 
https://github.com/torreycommerce/acenda-net-sdk-client
    
