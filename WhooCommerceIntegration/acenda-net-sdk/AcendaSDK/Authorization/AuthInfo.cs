using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcendaSDK
{
    public class AuthInfo
    {
        public AuthorizationHeader authorizationHeader { get; set; }
        public int expires_in { get; set; }
        public DateTime createdDate { get; set; }
        public string access_token { get; set; }

    }
}
