using AcendaSDK.DTOs;
using System.Net;
using System.Net.Http;

namespace AcendaSDK.Service
{
    public class InventoryService : IService
    {
        private static object lockObject = new object();
        private static InventoryService inventoryService;
        private AuthInfo _authInfo = new AuthInfo();
        private AuthParameters _authParameters = new AuthParameters();
        private InventoryService(AuthInfo authInfo, AuthParameters authParameters)
        {
            _authParameters = authParameters;
            _authInfo = authInfo;
        }

        /// <summary>
        /// Type must be ProductListDTO 
        /// </summary>
        /// <typeparam name="T">ProductListDTO </typeparam>
        /// <returns></returns>
        public T GetAll<T>() where T : new()
        {
            Response response = new Response();
            T productListDTO = new T();
            var token = AuthorizationService.Authorize(_authParameters.ClientId, _authParameters.ClientSecret, _authParameters.StoreName);
            var url = HelperFunctions.CreateUrlFromParts(_authParameters.StoreName, Constants.apiVariant, "", token.access_token);
            var result = HelperFunctions.HttpGet(url).GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
            {
                response.HttpStatusCode = result.StatusCode;
                response.Result = result.Content.ReadAsAsync<ProductVariantsDTO>().Result;
                if (response.Result != null)
                {
                    productListDTO = (T)response.Result;
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                response.HttpStatusCode = result.StatusCode;
                return productListDTO;
            }

            //var url = HelperFunctions.CreateUrlFromParts(storeName, Constants.apiProduct, string.Empty, _authInfo.access_token);
            //Uri(url,)
            return productListDTO;
        }

        public ProductVariantsDTO GetAllPaginated(int page = 0, int limit = 100, string query = "")
        {
            Response response = new Response();
            ProductVariantsDTO productListDTO = new ProductVariantsDTO();
            var token = AuthorizationService.Authorize(_authParameters.ClientId, _authParameters.ClientSecret, _authParameters.StoreName);
            var pagination = "page=" + page + "&limit=" + limit;
            var url = HelperFunctions.CreateUrlFromParts(_authParameters.StoreName, Constants.apiVariant, "", token.access_token, pagination, query);
            var result = HelperFunctions.HttpGet(url).GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
            {
                response.HttpStatusCode = result.StatusCode;
                response.Result = result.Content.ReadAsAsync<DTOs.ProductVariantsDTO>().Result;
                if (response.Result != null)
                {
                    productListDTO = (ProductVariantsDTO)response.Result;
                }
                else
                {
                    return default(ProductVariantsDTO);
                }
            }
            else
            {
                response.HttpStatusCode = result.StatusCode;
                return productListDTO;
            }


            return productListDTO;
        }

        public T GetById<T>(string id) where T : new()
        {
            throw new System.NotImplementedException();
        }

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data">VariantDTO</param>
        /// <returns></returns>
        public BaseDTO Update(string id, object data)
        {

            Response response = new Response();
            var variantDto = new VariantDTO();
            var variantPriceDto = new VariantPriceDTO();
            var variantInventoryDTO = new VariantInventoryDTO();

            if (data.GetType() == variantDto.GetType() || data.GetType() == variantPriceDto.GetType() || data.GetType() == variantInventoryDTO.GetType())
            {


                var token = AuthorizationService.Authorize(_authParameters.ClientId, _authParameters.ClientSecret, _authParameters.StoreName);
                var url = HelperFunctions.CreateUrlFromParts(_authParameters.StoreName, Constants.apiVariant, id, token.access_token);
                var result = HelperFunctions.HttpPut(url, data).GetAwaiter().GetResult();
                if (result != null)
                {

                    return result;

                }
                else
                {
                    return new BaseDTO()
                    {
                        code = (int)HttpStatusCode.BadRequest,


                    };
                }

            }
            else
            {
                throw new System.Exception("Not suppoerted type of parameter");
            }
        }
        public static InventoryService Instance(AuthInfo authInfo, string clientId, string clientSecret, string storeName)
        {
            if (inventoryService == null)
            {
                lock (lockObject)
                {
                    if (inventoryService == null)
                    {
                        inventoryService = new InventoryService(authInfo, new AuthParameters()
                        {
                            ClientId = clientId,
                            ClientSecret = clientSecret,
                            StoreName = storeName
                        });
                    }
                }
            }

            return inventoryService;
        }

        public BaseDTO Create(object data)
        {
            throw new System.NotImplementedException();
        }

        public BaseDTO Delete(string id)
        {
            throw new System.NotImplementedException();
        }
    }


}
