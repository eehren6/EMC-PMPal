using PMPal.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMPal.ViewModels
{
    public class PatientMessageViewModel : ValidationBase, INotifyPropertyChanged
    {
        public Dictionary<string, string> DepartmentDict = new Dictionary<string, string>();
        public event PropertyChangedEventHandler PropertyChanged;

        public const string defaultMessage = "Thank you for contacting Ezra Medical Center. You may book a COVID vaccine appointment via our online scheduling system https://link.ezramedical.org/vaccineschedule";
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private Person _selectedPatient;
        public Person SelectedPatient
        {
            get { 
                return _selectedPatient; }
            set
            {
                _selectedPatient = value;
                NotifyPropertyChanged("SelectedPatient");

            }
        }

        private string _selectedDeptID;
        public string SelectedDeptID
        {
            get { return _selectedDeptID; }
            set { _selectedDeptID = value;
                NotifyPropertyChanged("SelectedDeptID");
            }
        }

        public string HeaderMessage
        {
            get { return SelectedPatient==null ? "" : "Send SMS Message To " + SelectedPatient.Display_Name; }
        }

        public string UserID;

        public PatientMessageViewModel()
        {

        }

        public PatientMessageViewModel(Person selectedPatient, string userID)
        {
            _selectedPatient = selectedPatient;
            UserID = userID;
            LoadDepartments();
        }
        public void LoadDepartments()
        {
            if(DepartmentDict.Count == 0)
            {
                DepartmentDict.Add("Ezra Medical", "Ezra Medical");
                DepartmentDict.Add("60th street", "60th street");
            }
        }
        public void SetSelectedDepartment(string deptID)
        {
            this.SelectedDeptID = deptID;
        }
        public string GetDefaultMessage(string deptID)
        {
            return defaultMessage;
            //return @"Dear Ezra patient, we are currently offering a limited amount of Johnson & Johnson vaccines to our patients. You may book an appointment via our online scheduling system https://link.ezramedical.org/vaccineschedule. Please note that the online scheduling system will only accept appointments from existing Ezra patients.";
        }
        public string SendMessage(string dept, string message)
        {
            try
            {
                var result = Patient.SendPatientMessage(_selectedPatient, dept, UserID, message);
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
