using System;
using System.Collections.Generic;

namespace AcendaSDK.DTOs
{

    public class CustomerListResult
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string group_status { get; set; }
        public int group_id { get; set; }
        public double group_spending_limit_amount { get; set; }
        public string date_modified { get; set; }
        public string date_created { get; set; }
        public List<object> state { get; set; }
        public bool imported { get; set; }
        public int loginAttempts { get; set; }
        public List<OldPassword> old_passwords { get; set; }
        public string full_name { get; set; }
        public int loginattempts { get; set; }
    }

    public class CustomerListDTO : BaseDTO
    {
        public List<CustomerListResult> result { get; set; }
    }
}

