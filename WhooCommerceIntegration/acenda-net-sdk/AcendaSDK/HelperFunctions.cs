using AcendaSDK.DTOs;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AcendaSDK
{
    public static class HelperFunctions
    {
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }
        public static string CreateUrlFromParts(string storeHashed, string apiPath, string id, string accessToken, string pagination = "", string query = "")
        {
            string url = string.Empty;
            if (!string.IsNullOrEmpty(id))

            { url = Constants.BaseUrl + storeHashed + "/" + apiPath + "/" + id + "?access_token=" + accessToken; }
            else
            {
                url = Constants.BaseUrl + storeHashed + "/" + apiPath + "?access_token=" + accessToken;
            }
            if (!string.IsNullOrEmpty(query))
            {
                //char[] charsToTrim = { '{', ' ', '}' };
                //url = url + "&query={" + Uri.EscapeUriString(query.Trim(charsToTrim)) + "}";
                url = url + "&query=" + query;
            }
            if (!string.IsNullOrEmpty(pagination))
            {
                url = url + "&" + pagination;
            }
            return url;

        }
        public static async Task<HttpResponseMessage> HttpGet(string url)
        {
            HttpResponseMessage httpResponseMessage = null;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constants.BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                await Task.Run(async () =>
                 {

                     httpResponseMessage = await client.GetAsync(url);
                 });
            }
            return httpResponseMessage;

        }
        public static async Task<HttpResponseMessage> HttpDelete(string url)
        {
            HttpResponseMessage httpResponseMessage = null;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constants.BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                await Task.Run(async () =>
                {

                    httpResponseMessage = await client.DeleteAsync(url);
                });
            }
            return httpResponseMessage;

        }
        public static async Task<BaseDTO> HttpPut(string path, object obj)
        {
            HttpClient client = new HttpClient();
           
            BaseDTO response = new BaseDTO();
            try
            {
                string serializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                HttpWebRequest request = WebRequest.CreateHttp( path);
                request.Method = "PUT";
                request.AllowWriteStreamBuffering = false;
                request.ContentType = "application/json";
                request.Accept = "Accept=application/json";
                request.SendChunked = false;
                request.ContentLength = serializedObject.Length;
                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(serializedObject);
                }
                var response2 = request.GetResponse() as HttpWebResponse;
                using (Stream receiveStream = response2.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        var result = streamReader.ReadToEnd();
                        response = JsonConvert.DeserializeObject<BaseDTO>(result);
                        receiveStream.Close();
                        streamReader.Close();
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<BaseDTO> HttpPost(string path, object obj)
        {

           
            HttpClient client = new HttpClient();

            BaseDTO response = new BaseDTO();
            try
            {
                string serializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                HttpWebRequest request = WebRequest.CreateHttp(path);
                request.Method = "POST";
                request.AllowWriteStreamBuffering = true;
                request.ContentType = "application/json";
                request.Accept = "*/*";
                request.SendChunked = false;
                
                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(serializedObject);
                }
                var response2 = request.GetResponse() as HttpWebResponse;
                using (Stream receiveStream = response2.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        var result = streamReader.ReadToEnd();
                        response = JsonConvert.DeserializeObject<BaseDTO>(result);
                        receiveStream.Close();
                        streamReader.Close();
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

