using AcendaSDK.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcendaSDK.Service
{
    public class ServiceFactory 
    {
        public AuthorizationHeader AuthorizationHeader = new AuthorizationHeader();
        private AuthInfo _authInfo = new AuthInfo();
        public string StoreName { get; set; }
        public ServiceFactory(string clientId, string clientSecret,string storeName)
        {
           
            AuthorizationHeader.client_id = clientId;
            AuthorizationHeader.client_secret = clientSecret;
            AuthorizationHeader.grant_type = "client_credentials";
            AuthorizationHeader.scope = string.Empty;
            StoreName = HelperFunctions.CreateMD5(storeName);
            var authInfo = AuthorizationService.Authorize( clientId , clientSecret , StoreName);
              if(authInfo != null )
            {
                _authInfo = authInfo;

            }
              
        }
        public  IService GetService( Enums.ServiceType serviceType, string serviceName="acenda")
        {
             
           switch(serviceType )
            {
                case Enums.ServiceType.Product:
                    return ProductService.Instance(new AuthInfo() {
                        access_token = _authInfo.access_token,
                        authorizationHeader = _authInfo.authorizationHeader,
                        createdDate = _authInfo.createdDate,
                        expires_in = _authInfo.expires_in}, AuthorizationHeader.client_id, AuthorizationHeader.client_secret, StoreName);
                case Enums.ServiceType.Order: 
                    return OrderService.Instance(new AuthInfo()
                    {
                        access_token = _authInfo.access_token,
                        authorizationHeader = _authInfo.authorizationHeader,
                        createdDate = _authInfo.createdDate,
                        expires_in = _authInfo.expires_in
                    }, AuthorizationHeader.client_id, AuthorizationHeader.client_secret, StoreName);

                case Enums.ServiceType.Inventory:
                    return InventoryService.Instance(new AuthInfo()
                    {
                        access_token = _authInfo.access_token,
                        authorizationHeader = _authInfo.authorizationHeader,
                        createdDate = _authInfo.createdDate,
                        expires_in = _authInfo.expires_in
                    }, AuthorizationHeader.client_id, AuthorizationHeader.client_secret, StoreName);
                case Enums.ServiceType.Customer:
                    return CustomerService.Instance(new AuthInfo()
                    {
                        access_token = _authInfo.access_token,
                        authorizationHeader = _authInfo.authorizationHeader,
                        createdDate = _authInfo.createdDate,
                        expires_in = _authInfo.expires_in
                    }, AuthorizationHeader.client_id, AuthorizationHeader.client_secret, StoreName);

                case Enums.ServiceType.Generic:
                    return GenericService.Instance(new AuthInfo()
                    {
                        access_token = _authInfo.access_token,
                        authorizationHeader = _authInfo.authorizationHeader,
                        createdDate = _authInfo.createdDate,
                        expires_in = _authInfo.expires_in,
                        
                    }, AuthorizationHeader.client_id, AuthorizationHeader.client_secret, StoreName,serviceName);
                default:
                    return null;
            } 
   
        }
    }

}
