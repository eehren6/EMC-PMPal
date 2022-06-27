using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using PMPal.Models;

namespace PMPal.ViewModels
{
    public class EncountersViewModel : ViewModelBase
    {
        private ObservableDictionary<string, PatientEncounter> encountersList;
        public ObservableDictionary<string, PatientEncounter> EncountersList
        {
            get => encountersList;
            set
            {
                encountersList = value;
                OnPropertyChanged(nameof(EncountersList));
            }
        }

        public PatientEncounter SelectedEncounter { get; set; }
        public Person Patient { get; set; }

        public string PersonName { get; set; }
        public string ProviderID { get; set; }
        DateTime StartDate { get; set; }

        public bool DentalOnly { get; set; }

        public bool TodayOnly { get; set; }
        public ICommand OpenCRACommand { get; }
        public ICommand OpenReferralsCommand { get; }
        public EncountersViewModel()
        {
            EncountersList = new ObservableDictionary<string, PatientEncounter>();
        }

        public EncountersViewModel(Person patient, string providerID, string startDate, bool dentalOnly)
        {
            this.Patient = patient;
            this.ProviderID = providerID;
            this.DentalOnly = dentalOnly;
            this.TodayOnly = true;
            if (startDate != "") this.StartDate = Convert.ToDateTime(startDate);
            //else
            //    this.StartDate = new DateTime(1900, 1, 1);

            GetEncounters();

            if(TodayOnly && encountersList.Count == 0)
            {
                TodayOnly = false;
                GetEncounters();
            }
        }

        private void GetEncounters()
        {
            StartDate = TodayOnly ? DateTime.Today : new DateTime(1900, 1, 1); ;

            if (DentalOnly)
                EncountersList = Backend.Repository.PatientEncounters.GetPatientEncountersDental(Patient.person_id, ProviderID, StartDate.ToString());
            else
                EncountersList = Backend.Repository.PatientEncounters.GetPatientEncounters(Patient.person_id, ProviderID, StartDate.ToString());
                        
        }

        public void RefreshEncounters()
        {
            GetEncounters();
        }

    }
}
