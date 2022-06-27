using System;
using System.Collections.Generic;
using System.Text;

namespace PMPal.Models
{

    public class Relation
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Relationship { get; set; }
        public Relation() { }

        public Relation(string first_name, string last_name, string relationship)
        {
            First_Name = first_name;
            Last_Name = last_name;
            Relationship = relationship;
        }

    }
}
