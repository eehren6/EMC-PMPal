using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace PMPal
{
    class PatientsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        //{
        //    add
        //    {
                
        //    }

        //    remove
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        public void NotifyPropertyChanged(string propertyName)
        {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private Person _selectedPatient;
        public Person SelectedPatient
        {
            get { return _selectedPatient; }
            set { 
                _selectedPatient = value;
                NotifyPropertyChanged("SelectedPatient");

            }
        }
        public string Curr_User_ID { get; set; }

        private string _filter_text;
        public string Filter_Text
        {
            get { return _filter_text; }
            set { 
                _filter_text = value;
            }
        }
        public ObservableDictionary<string, Person> Patients
        {
            get { 
                var res = Patient.GetPatients(FormatFilter(Filter_Text));
                return res; 
            }
        }

        private static string FormatFilter(string filter_text)
        {
            if (string.IsNullOrEmpty(filter_text))
                return "";

            char delimiter = ',';
            if (!filter_text.Contains(','))
                delimiter = ' ';

            string updated = "";
            var split = filter_text.Split(delimiter);
            for (int i = 0; i < split.Length; i++)
            {
                var f = split[i];

                DateTime dateF;
                if (DateTime.TryParse(f, out dateF))
                {
                    f = dateF.ToString("yyyyMMdd");
                }
                else if (f.Length == 8)  //try parseExact
                {
                    if (DateTime.TryParseExact(f, "MMddyyyy",
                           System.Globalization.CultureInfo.InvariantCulture,
                           System.Globalization.DateTimeStyles.None, out dateF))
                    {
                        f = dateF.ToString("yyyyMMdd");
                    }
                }
                updated += f + ",";
            }

            filter_text = updated.Remove(updated.Length - 1, 1);

            return filter_text;
        }


        public bool PatientFound = false;
        public bool SendMessage = false;
        public bool ShowEncounters = false;
        public void FindPatient(string patient_id)
        {
            /*don't need this, we already check within getpatientinfo*/
            //var res = Patients.Where(x => x.Key == patient_id).Select(x => x.Value).FirstOrDefault();
            //if(res != null)
            //{
            //    Filter_Text = res.person_nbr;

            //}
            //else
            //{
               var p = Patient.GetPatientInfo(patient_id);
               if(p != null && !string.IsNullOrEmpty(p.person_id))
                {
                if (!Patients.ContainsKey(p.person_id))
                    Patient.AddUpdatePatient(p.person_id, p);

                this.SelectedPatient = p;
                Filter_Text = p.person_nbr;
                PatientFound = true;
                }
            //}
        }

        public void Clear()
        {
            Events.ClearLists();

            SelectedPatient = new Person();

            NotifyPropertyChanged("PatientDisplayList");
        }

        
    }

}
