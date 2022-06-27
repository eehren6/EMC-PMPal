using System;
using System.Collections.Generic;
using System.Text;

namespace PMPal.Models
{
    public class PatientEncounter
    {
        public string PersonID { get; set; }

        private Person _person;
        public Person Person { 
            get {
                if (PersonID != "" && (_person == null || _person.person_id != PersonID))
                    _person = Patient.GetPatientInfo(PersonID);
                return _person;
            }
            set {
                _person = value;
            } 
        }
        public string EncounterID { get; set; }
        public string EncounterNbr { get; set; }
        public string EncounterDate { get; set; }

        public string ProviderID { get; set; }

        public string ProviderName { get; set; }

        //private Provider _provider;
        //Provider Provider { 
        //    get  { 
        //        if(ProviderID != "" && (_provider == null || _provider.person_id != Provider))
        //            _provider = Backend.Repository.ProvidersRepo.
        //    } 
            
        //    set {

        //    }
        //}


        public PatientEncounter()
        {
        }

        
    }
}
