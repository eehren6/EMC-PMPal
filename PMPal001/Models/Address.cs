using System;
using System.Collections.Generic;
using System.Text;

namespace PMPal.Models
{
    public class Address
    {
        public string AddressType { get; set; }
        public string AddressTypeID { get; set; }
        public string StreetAddress1 { get; set; }
        public string StreetAddress2 { get; set; } 
        public string City { get; set; } 
        public string State { get; set; }
        public string Zip { get; set; } 
        public string Country { get; set; } 
        public string County { get; set; } 
        public string Country_id; 
        public string County_id; 
 
    }
}
