using System;
using System.Collections.Generic;
using System.Text;

namespace PMPal.Models
{
    public class PatientReferral
    {
        public string ReferralID { get; set; }
        public string PersonID { get; set; }
        public Person RefPatient { get; set; }

        public string EncID { get; set; }

        public Provider ReferringProvider { get; set; }

        public string ReferralProvider { get; set; }

        public string TeethCondition { get; set; }

        public string Specialty { get; set; }

        public string Reason { get; set; }

        public string OfficeInstruction { get; set; }

        public string Procedure { get; set; }

        public string Status { get; set; }

        public string Note { get; set; }

        public DateTime NoteDate { get; set; }

        public PatientReferral() { }
            
    }
}
