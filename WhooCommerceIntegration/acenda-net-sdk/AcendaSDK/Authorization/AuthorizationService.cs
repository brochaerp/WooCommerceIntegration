using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AcendaSDK
{
    public static class AuthorizationService
    {
        private static List<AuthInfo> authList = new List<AuthInfo>();


        private static string authFile = @".\acendaAuth.json";

        private static object lockObject =new object();
        static AuthorizationService()
        {


            if (File.Exists(authFile))
            {
                lock (lockObject)
                {
                    using (StreamReader r = new StreamReader(authFile))
                    {
                        string json = r.ReadToEnd();
                        authList = JsonConvert.DeserializeObject<List<AuthInfo>>(json);

                        r.Close();
                    }
                }

            }
            else
            {
                string json = JsonConvert.SerializeObject(authList.ToArray());

                lock (lockObject)
                {
                    using (StreamWriter wr = new StreamWriter(authFile))
                    {
                        wr.Write(json);
                        wr.Flush();
                        wr.Close();
                    }
                }

            }
        }

        private static List<AuthInfo> ReadFromFile()
        {

            lock (lockObject)
            {


                if (File.Exists(authFile))
                {
                    using (StreamReader r = new StreamReader(authFile))
                    {
                        string json = r.ReadToEnd();
                        authList = JsonConvert.DeserializeObject<List<AuthInfo>>(json);

                        r.Close();
                        return authList;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        private static void WriteToFile(List<AuthInfo> authorizationResults)
        {
            lock (lockObject)
            {
                string json = JsonConvert.SerializeObject(authList.ToArray());
                using (StreamWriter wr = new StreamWriter(authFile))
                {
                    wr.Write(json);
                    wr.Flush();
                    wr.Close();
                }
            }

        }

        private static void AddToAuthList(AuthInfo authorizationResult)
        {
            if (authList.Where(x => x.access_token == authorizationResult.access_token).FirstOrDefault() == null)
            {
                authList.Add(authorizationResult);
                WriteToFile(authList);
            }


        }


        public static AuthInfo Authorize(string clientId, string clientSecret, string storeName)
        {

            AuthInfo authInfo = GetAuthorized(new AuthParameters() { ClientId = clientId, ClientSecret = clientSecret, StoreName = storeName });
            if (authInfo == null)
            {
                var result = (AuthorizeWithResult(clientId, clientSecret));
                if (result.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    var authHeader = new AuthorizationHeader() { client_id = clientId, client_secret = clientSecret, scope = string.Empty, grant_type = "client_credentials" };
                    var newAuth = new AuthInfo()
                    {
                        access_token = result.access_token,
                        createdDate = DateTime.Now,
                        expires_in = result.expires_in,
                        authorizationHeader = authHeader
                    };
                    AddToAuthList(newAuth);

                    return newAuth;

                }
                else
                {
                    return null;
                }

            }
            else
            {
                return authInfo;
            }




        }
        private static AuthInfo GetAuthorized(AuthParameters authParameters)
        {
            var auth = authList.Where(x => x.authorizationHeader.client_id == authParameters.ClientId && x.authorizationHeader.client_secret == authParameters.ClientSecret && x.access_token != null).OrderByDescending(x => x.createdDate).FirstOrDefault();
            if (auth != null)
            {
                if ((DateTime.Now - auth.createdDate).TotalSeconds < auth.expires_in)
                {
                    return auth;
                }
            }

            return null;
        }


        private static AuthorizationResult AuthorizeWithResult(string clientId, string clientSecret)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = null;
                AuthorizationResult response = new AuthorizationResult();
                try
                {

                    client.BaseAddress = new Uri("https://www.acenda.com/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    Task.Run(async () =>
                   {
                       httpResponseMessage = await client.PostAsJsonAsync("/oauth/token",
                       new AuthorizationHeader()
                       {
                           client_id = clientId,
                           client_secret = clientSecret,
                           scope = string.Empty,
                           grant_type = "client_credentials"
                       });
                   }).Wait();
                    if (httpResponseMessage != null)
                    {
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            
                            Task.Run(async () =>
                            {
                                response = await httpResponseMessage.Content.ReadAsAsync<AuthorizationResult>();
                            }).Wait();

                            response.HttpStatusCode = httpResponseMessage.StatusCode;
                        }
                        else
                        {
                            response.HttpStatusCode = httpResponseMessage.StatusCode;

                        }
                        return response;
                    }
                    else
                    {
                        throw new Exception("Can not get any response");

                    }


                }
                catch (Exception ex)
                {
                    // Log 
                    throw ex;
                }
            }
        }
    }
}
