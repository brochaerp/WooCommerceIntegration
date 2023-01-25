using System;
using System.Collections.Generic;

namespace AcendaSDK.DTOs
{
    public class OldPassword
    {
        public object password { get; set; }
        public int ts { get; set; }
    }

    public class AccessRules
    {
        public bool browse_catalog { get; set; }
        public bool show_prices { get; set; }
        public bool place_order { get; set; }
    }

    public class Group
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool require_email_verification { get; set; }
        public bool require_approval { get; set; }
        public List<object> approved_domains { get; set; }
        public string discount_field { get; set; }
        public double discount_percent { get; set; }
        public string spending_limit_type { get; set; }
        public double spending_limit_amount { get; set; }
        public double max_order_total { get; set; }
        public int max_order_item { get; set; }
        public int max_order_item_quantity { get; set; }
        public bool disable_tax { get; set; }
        public AccessRules access_rules { get; set; }
        public string date_modified { get; set; }
        public string date_created { get; set; }
    }

    public class CustomerResult
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
        public List<Group> group { get; set; }
    }

    public class CustomerDTO : BaseDTO
    {
        public CustomerResult result { get; set; }
    }
}
