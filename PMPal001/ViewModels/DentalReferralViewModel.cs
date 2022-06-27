using System;
using System.Collections.Generic;
using System.Text;

namespace PMPal.ViewModels
{
    class DentalReferralViewModel
    {
        public string PersonID { get; set; }
        public string EncID { get; set; }

        Backend.DentalReferralNote dentalReferralNote { get; set; }
        public DentalReferralViewModel(string personID,string encID)
        {
            PersonID = "DB29A045-8550-4637-8F2B-782A3C81FD3C"; //personID
            EncID = "3E6A67ED-A100-4DAB-9428-22C8B6BAFC38"; //encID;

            dentalReferralNote = new Backend.DentalReferralNote(PersonID, EncID);

        }
        public void GenerateReferral()
        {
            //dentalReferralNote.GeneratePDF(dentalReferralNote.GenerateHTMLDocument());
        }
    }

}
