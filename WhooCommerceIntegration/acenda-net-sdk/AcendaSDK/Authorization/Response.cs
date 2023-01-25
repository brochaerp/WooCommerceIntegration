using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Net;
namespace AcendaSDK
{
    public class Response
    {

        public HttpStatusCode HttpStatusCode { get; set; }
        public bool IsSuccessStatusCode { get; set; } = false;
        public bool IsBadRequestStatusCode { get; set; } = false;
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ExtraInfo { get; set; }
        public object Result { get; set; }
    }
}
