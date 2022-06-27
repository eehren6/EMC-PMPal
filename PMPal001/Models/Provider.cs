using System;
using System.Collections.Generic;
using System.Text;

namespace PMPal.Models
{
    public class Provider: Person
    {
        public string UserID { get; set; }
        public string Description { get; set; }

        public string Salutary { get; set; }

        public string Degree { get; set; }

        public string Subgrouping { get; set; }

    }
}
