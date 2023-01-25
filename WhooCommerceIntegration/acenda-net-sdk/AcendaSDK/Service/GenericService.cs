using System;
using System.Net;
using System.Net.Http;
using AcendaSDK.DTOs;

namespace AcendaSDK.Service
{
    public class GenericService : IService
    {
        private static object lockObject = new object();
        private static GenericService genericService;
        private AuthInfo _authInfo = new AuthInfo();
        private AuthParameters _authParameters = new AuthParameters();
        private string _serviceName;

        public GenericService(AuthInfo authInfo, AuthParameters authParameters, string serviceName)
        {
            _authParameters = authParameters;
            _authInfo = authInfo;
            _serviceName = serviceName;
        }

        public BaseDTO Create(object data)
        {
            var token = AuthorizationService.Authorize(_authParameters.ClientId, _authParameters.ClientSecret, _authParameters.StoreName);
            var url = HelperFunctions.CreateUrlFromParts(_authParameters.StoreName, "api/" + _serviceName, string.Empty, token.access_token);
            var result = HelperFunctions.HttpPost(url, data).GetAwaiter().GetResult();
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


        public BaseDTO Delete(string id)
        {
            Response response = new Response();
            BaseDTO baseDTO = new BaseDTO();
            var token = AuthorizationService.Authorize(_authParameters.ClientId, _authParameters.ClientSecret, _authParameters.StoreName);
            var url = HelperFunctions.CreateUrlFromParts(_authParameters.StoreName, "api/" + _serviceName, id, token.access_token);
            var result = HelperFunctions.HttpDelete(url).GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
            {
                response.HttpStatusCode = result.StatusCode;
                response.Result = result.Content.ReadAsAsync<BaseDTO>().Result;
                if (response.Result != null)
                {
                    baseDTO = (BaseDTO)response.Result;
                }
                else
                {
                    return default(BaseDTO);
                }
            }
            else
            {

                baseDTO.status = result.StatusCode.ToString();
                baseDTO.code = (int)result.StatusCode;
                return baseDTO;
            }
            return baseDTO;
        }

        public T GetAll<T>() where T : new()
        {
            Response response = new Response();
            T genericResponseDto = new T();
            var token = AuthorizationService.Authorize(_authParameters.ClientId, _authParameters.ClientSecret, _authParameters.StoreName);
            var url = HelperFunctions.CreateUrlFromParts(_authParameters.StoreName, "api/" + _serviceName, "", token.access_token);
            var result = HelperFunctions.HttpGet(url).GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
            {
                response.HttpStatusCode = result.StatusCode;
                response.Result = result.Content.ReadAsAsync<T>().Result;
                if (response.Result != null)
                {
                    genericResponseDto = (T)response.Result;
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                response.HttpStatusCode = result.StatusCode;
                return genericResponseDto;
            }


            return genericResponseDto;
        }

        public T GetAllPaginated<T>(int page = 0, int limit = 100, string query = "") where T : new()
        {
            Response response = new Response();
            T genericResponseDto = new T();
            var token = AuthorizationService.Authorize(_authParameters.ClientId, _authParameters.ClientSecret, _authParameters.StoreName);
            var pagination = "page=" + page + "&limit=" + limit;
            var url = HelperFunctions.CreateUrlFromParts(_authParameters.StoreName, "api/" + _serviceName, "", token.access_token, pagination, query);
            var result = HelperFunctions.HttpGet(url).GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
            {
                response.HttpStatusCode = result.StatusCode;
                response.Result = result.Content.ReadAsAsync<T>().Result;
                if (response.Result != null)
                {
                    genericResponseDto = (T)response.Result;
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                response.HttpStatusCode = result.StatusCode;
                return genericResponseDto;
            }


            return genericResponseDto;
        }

        public T GetById<T>(string id) where T : new()
        {
            Response response = new Response();
            T genericDTO = new T();
            var token = AuthorizationService.Authorize(_authParameters.ClientId, _authParameters.ClientSecret, _authParameters.StoreName);
            var url = HelperFunctions.CreateUrlFromParts(_authParameters.StoreName, "api/" + _serviceName, id, token.access_token);
            var result = HelperFunctions.HttpGet(url).GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
            {
                response.HttpStatusCode = result.StatusCode;
                response.Result = result.Content.ReadAsAsync<T>().Result;
                if (response.Result != null)
                {
                    genericDTO = (T)response.Result;
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                response.HttpStatusCode = result.StatusCode;
                return genericDTO;
            }


            return genericDTO;
        }

        public BaseDTO Update(string id, object data)
        {
            Response response = new Response();


            var token = AuthorizationService.Authorize(_authParameters.ClientId, _authParameters.ClientSecret, _authParameters.StoreName);
            var url = HelperFunctions.CreateUrlFromParts(_authParameters.StoreName, "api/" + _serviceName, id, token.access_token);
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
        public static GenericService Instance(AuthInfo authInfo, string clientId, string clientSecret, string storeName, string serviceName)
        {
            if (genericService == null)
            {
                lock (lockObject)
                {
                    if (genericService == null)
                    {
                        genericService = new GenericService(authInfo, new AuthParameters()
                        {
                            ClientId = clientId,
                            ClientSecret = clientSecret,
                            StoreName = storeName
                        }, serviceName);
                    }
                }
            }

            return genericService;
        }
    }
}
