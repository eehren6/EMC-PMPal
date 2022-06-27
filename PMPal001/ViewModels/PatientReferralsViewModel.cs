using System;
using System.Collections.Generic;
using System.Text;
using PMPal.Models;
using System.Linq;
using System.Collections;

namespace PMPal.ViewModels
{
    public class PatientReferralsViewModel : ViewModelBase
    {
        public string PersonID { get; set; }
        public string EncID { get; set; }
        public ObservableDictionary<string, PatientReferral> referrals { get; set; }
        
        public PatientReferral SelectedReferral { get; set; }

        public Backend.DentalReferralNote dentalReferralNote;

        public PatientReferralsViewModel()
        {

        }

        public PatientReferralsViewModel(string personID, string encID)
        {
            PersonID = personID;
            EncID = encID;

            dentalReferralNote = new Backend.DentalReferralNote(PersonID, encID);
            referrals = new ObservableDictionary<string, PatientReferral>(dentalReferralNote.patientReferrals);
        }

        public void GenerateReferralPDF()
        {
            if (dentalReferralNote != null && SelectedReferral != null)
            {
                try
                {
                    dentalReferralNote.GenerateReferralLetter(SelectedReferral.ReferralID);

                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }

    }
}
