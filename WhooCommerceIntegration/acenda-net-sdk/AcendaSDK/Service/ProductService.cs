using AcendaSDK.DTOs;
using System.Net;
using System.Net.Http;

namespace AcendaSDK.Service
{
    public class ProductService : IService
    {

        private static object lockObject = new object();
        private static ProductService productService;
        private AuthInfo _authInfo = new AuthInfo();
        private AuthParameters _authParameters = new AuthParameters();
        private ProductService(AuthInfo authInfo, AuthParameters authParameters)
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
            var url = HelperFunctions.CreateUrlFromParts(_authParameters.StoreName, Constants.apiProduct, "", token.access_token);
            var result = HelperFunctions.HttpGet(url).GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
            {
                response.HttpStatusCode = result.StatusCode;
                response.Result = result.Content.ReadAsAsync<ProductListDTO>().Result;
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

        public ProductListDTO GetAllPaginated(int page = 0, int limit = 100, string query = "")
        {
            Response response = new Response();
            ProductListDTO productListDTO = new ProductListDTO();
            var token = AuthorizationService.Authorize(_authParameters.ClientId, _authParameters.ClientSecret, _authParameters.StoreName);
            var pagination = "page=" + page + "&limit=" + limit;
            var url = HelperFunctions.CreateUrlFromParts(_authParameters.StoreName, Constants.apiProduct, "", token.access_token, pagination, query);
            var result = HelperFunctions.HttpGet(url).GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
            {
                response.HttpStatusCode = result.StatusCode;
                response.Result = result.Content.ReadAsAsync<DTOs.ProductListDTO>().Result;
                if (response.Result != null)
                {
                    productListDTO = (ProductListDTO)response.Result;
                }
                else
                {
                    return default(ProductListDTO);
                }
            }
            else
            {
                response.HttpStatusCode = result.StatusCode;
                return productListDTO;
            }


            return productListDTO;
        }

        /// <summary>
        /// Type must be ProductDTO 
        /// </summary>
        /// <typeparam name="T">ProductDTO</typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById<T>(string id) where T : new()
        {
            Response response = new Response();
            T productDTO = new T();
            var token = AuthorizationService.Authorize(_authParameters.ClientId, _authParameters.ClientSecret, _authParameters.StoreName);
            var url = HelperFunctions.CreateUrlFromParts(_authParameters.StoreName, Constants.apiProduct, id, token.access_token);
            var result = HelperFunctions.HttpGet(url).GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
            {
                response.HttpStatusCode = result.StatusCode;
                response.Result = result.Content.ReadAsAsync<T>().Result;
                if (response.Result != null)
                {
                    productDTO = (T)response.Result;
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                response.HttpStatusCode = result.StatusCode;
                return productDTO;
            }

            //var url = HelperFunctions.CreateUrlFromParts(storeName, Constants.apiProduct, string.Empty, _authInfo.access_token);
            //Uri(url,)
            return productDTO;

        }

        public static ProductService Instance(AuthInfo authInfo, string clientId, string clientSecret, string storeName)
        {
            if (productService == null)
            {
                lock (lockObject)
                {
                    if (productService == null)
                    {
                        productService = new ProductService(authInfo, new AuthParameters()
                        {
                            ClientId = clientId,
                            ClientSecret = clientSecret,
                            StoreName = storeName
                        });
                    }
                }
            }

            return productService;
        }


        public ProductVariantsDTO GetVariants(string productId)
        {
            Response response = new Response();
            ProductVariantsDTO productVariantsDTO = new ProductVariantsDTO();
            var token = AuthorizationService.Authorize(_authParameters.ClientId, _authParameters.ClientSecret, _authParameters.StoreName);
            var url = HelperFunctions.CreateUrlFromParts(_authParameters.StoreName, Constants.apiProduct, productId + "/variants/", token.access_token);
            var result = HelperFunctions.HttpGet(url).GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
            {
                response.HttpStatusCode = result.StatusCode;
                response.Result = result.Content.ReadAsAsync<ProductVariantsDTO>().Result;
                if (response.Result != null)
                {
                    productVariantsDTO = (ProductVariantsDTO)response.Result;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                response.HttpStatusCode = result.StatusCode;
                return productVariantsDTO;
            }

            return productVariantsDTO;

        }


        public BaseDTO Update(string id, object data)
        {
            Response response = new Response();
            var type = new VariantDTO();

            if (data.GetType() == type.GetType())
            {
                var token = AuthorizationService.Authorize(_authParameters.ClientId, _authParameters.ClientSecret, _authParameters.StoreName);
                var url = HelperFunctions.CreateUrlFromParts(_authParameters.StoreName, Constants.apiProduct, id, token.access_token);
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
