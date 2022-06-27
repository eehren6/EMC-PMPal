using System;
using System.Collections.Generic;
using System.Text;
using PMPal.Backend;
using PMPal.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace PMPal.ViewModels
{
    public class CariesFormViewModel : ViewModelBase
    {

        public PatientCariesForm cariesForm { get; set; }
        public DateTime RecordDate => cariesForm.RecordDate;

        public string EncNBR { get; set; }
        public Person Person { get; set; }
        public int FormTotal => cariesForm.Total;

        public string OverallRisk => cariesForm.Total > 5 ? "High" : cariesForm.Total > 2 ? "Moderate" : "Low";
        public CariesFormViewModel(string personID, string encID, string encNbr)
        {
            this.Person = Patient.GetPatientInfo(personID);
            cariesForm = new PatientCariesForm(Person.person_id,encID);
            this.EncNBR = encNbr;
            OnPropertyChanged("Person"); 
            OnPropertyChanged("RecordDate");
        }

        public CariesFormViewModel()
        {
            cariesForm = new PatientCariesForm();
        }

        public void UpdatePatientCariesForm(int questionID, int value)
        {
            cariesForm.UpdateForm(questionID, value);
        }

        public void ResetForm()
        {
            cariesForm.FormResults = new Dictionary<int, int?>();
        }

        public void SubmitForm()
        {
            try
            {
                cariesForm.SubmitCharge();
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
