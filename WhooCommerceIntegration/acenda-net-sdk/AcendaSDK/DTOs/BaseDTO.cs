using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
namespace AcendaSDK.DTOs
{
    public class BaseDTO
    {
        public int code { get; set; }
        public string status { get; set; }
        public double execution_time { get; set; }
        public int num_total { get; set; }

    }
}
