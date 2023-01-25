using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AcendaSDK
{
    public class AuthorizationResult 
    {


        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
        
        public HttpStatusCode HttpStatusCode { get; set; }

        
    }
}
