using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace PMPal.ViewModels
{
    class DetailsViewModel : ValidationBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableDictionary<string, string> _genderValues;
        public ObservableDictionary<string, string> GenderValues
        {
            get { return _genderValues; }
        }

        public ObservableDictionary<string, string> ContactPrefOptions
        {
            get { return new ObservableDictionary<string,string>(PMPal.Backend.ListRetrieve.ContactPrefOptions); }
        }

        //public KeyValuePair<string, string> SelectedContactPref
        public DetailsViewModel()
        {
            
        }

        public DetailsViewModel(Person person)
        {
            LoadGenderValues();

            string ethnicity = "";
            string ethnicity_ids = Patient.GetPatientEthnicity(person.person_id, ref ethnicity);
            person.ethnicity_id = ethnicity_ids;
            person.ethnicity = ethnicity;
            this.SelectedPatient = person;


            LoadGenderValues();
        }

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
        [Required]
        [DisplayName("First Name")]
        public string FirstName
        {
            get { return _selectedPatient?.First_name; }
            set { _selectedPatient.First_name = value;
                NotifyPropertyChanged("FirstName");
                ValidateProperty(value);
            }
        }
        [Required]
        [DisplayName("Last Name")]
        public string LastName
        {
            get { return _selectedPatient?.Last_name; }
            set
            {
                _selectedPatient.Last_name = value;
                NotifyPropertyChanged("LastName");
                ValidateProperty(value);
            }
        }
        [Required]
        [DisplayName("Gender")]
        public string Sex
        {
            get { return _selectedPatient?.Sex; }
            set
            {
                _selectedPatient.Sex = value;
                NotifyPropertyChanged("Sex"); 
                ValidateProperty(value);
            }
        }
        [Required]
        [DisplayName("Date Of Birth")]
        public string DOB
        {
            get { return _selectedPatient?.DOB_display; }
            set
            {
                _selectedPatient.DOB_display = value;
                NotifyPropertyChanged("DOB");
                ValidateProperty(value);
            }
        }

        private string _relationship = "";
        [Required]
        public string Relationship
        {
            get { return _relationship; }
            set { _relationship = value;
                NotifyPropertyChanged("Relationship");
                ValidateProperty(value);
            }
        }

        [Required]
        public string Cell_phone
        {
            get { return _selectedPatient?.cell_phone; }
            set {
                _selectedPatient.cell_phone = value;
                NotifyPropertyChanged("Cell_phone");
                ValidateProperty(value);
            }
        }

        //public bool NoCellPhone //required for expanding on validation
        //{
        //    get { return string.IsNullOrEmpty(Cell_phone); }
        //}

        public bool Phone_Ind
        {
            get { return (SelectedPatient?.phone_ind == "Y"); }

            set { 
                SelectedPatient.phone_ind = (value ? "Y" : "N");
                NotifyPropertyChanged("Phone_Ind");
            }
        }
        public bool Email_Ind
        {
            get { return (SelectedPatient?.email_ind == "Y"); }
            set
            {
                SelectedPatient.email_ind = (value ? "Y" : "N");
                NotifyPropertyChanged("Email_Ind");
            }
        }
        public bool Optout_Ind
        {
            get { return (SelectedPatient?.optout_ind == "Y"); }
            set
            {
                SelectedPatient.optout_ind = (value ? "Y" : "N");
                NotifyPropertyChanged("Optout_Ind");
            }
        }

        [Required]
        public bool Notifications_Missing
        {
            get { 
                if (string.IsNullOrEmpty(Cell_phone))
                    return true;
                else if (!Optout_Ind && !Phone_Ind)
                    return true;
                else return false;
                    }
        }
        public string Curr_User_ID { get; set; }
        private void LoadGenderValues()
        {
            _genderValues = new ObservableDictionary<string, string>();
            GenderValues.Add("Male", "M");
            GenderValues.Add("Female", "F");
            GenderValues.Add("Undifferentiated", "O");
        }

        public Person CreateNewFamilyMember()
        {
            if (String.IsNullOrEmpty(SelectedPatient?.Last_name))
                return new Person();

            Person newSibling = new Person();
            newSibling.Last_name = SelectedPatient.Last_name;
            newSibling.Home_phone = SelectedPatient.Home_phone;
            newSibling.PrimaryAddress = SelectedPatient.PrimaryAddress;
            newSibling.SecondaryAddress = SelectedPatient.SecondaryAddress;

            newSibling.Race = SelectedPatient.Race;
            newSibling.Race_id = SelectedPatient.Race_id;
            newSibling.language = SelectedPatient.language;

            /*contact into*/
            newSibling.cell_phone = SelectedPatient.cell_phone;
            newSibling.cell_phone_comment = SelectedPatient.cell_phone_comment;
            newSibling.Home_phone = SelectedPatient.Home_phone;
            newSibling.home_phone_comment = SelectedPatient.home_phone_comment;
            newSibling.day_phone = SelectedPatient.day_phone;
            newSibling.day_phone_ext = SelectedPatient.day_phone_comment;
            newSibling.day_phone_comment = SelectedPatient.day_phone_comment;
            newSibling.alt_phone = SelectedPatient.alt_phone;
            newSibling.alt_phone_ext = SelectedPatient.alt_phone_ext;
            newSibling.alt_phone_desc = SelectedPatient.alt_phone_desc;
            newSibling.alt_phone_comment = SelectedPatient.alt_phone_comment;
            newSibling.email_address = SelectedPatient.email_address;
            newSibling.email_address_comment = SelectedPatient.email_address_comment;
            newSibling.email_ind = SelectedPatient.email_ind;
            newSibling.phone_ind = SelectedPatient.phone_ind;
            newSibling.Contact_Seq = SelectedPatient.Contact_Seq;
            newSibling.Contact_pref_desc = SelectedPatient.Contact_pref_desc;
            newSibling.Contact_pref_id = SelectedPatient.Contact_pref_id;

            /*ethnic info*/
            newSibling.Race = SelectedPatient.Race;
            newSibling.Race_id = SelectedPatient.Race_id;
            newSibling.ethnicity = SelectedPatient.ethnicity;
            newSibling.ethnicity_id = SelectedPatient.ethnicity_id;
            newSibling.uds_ethnicity_id = SelectedPatient.uds_ethnicity_id;
            newSibling.language = SelectedPatient.language;
            newSibling.language_id = SelectedPatient.language_id;
            newSibling.uds_language_barrier_id = SelectedPatient.uds_language_barrier_id;
            newSibling.uds_veteran_status = SelectedPatient.uds_veteran_status;
            newSibling.uds_homeless_status_id = SelectedPatient.uds_homeless_status_id;

            /*dr and coverage info*/
            newSibling.uds_primary_med_coverage_id = SelectedPatient.uds_primary_med_coverage_id;
            newSibling.primarycare_prov_id = SelectedPatient.primarycare_prov_id;
            newSibling.primarycare_prov_name = SelectedPatient.primarycare_prov_name;
            newSibling.self_pay_ind = SelectedPatient.self_pay_ind;

            return newSibling;
        }

       

        public bool ValidatePatient(out string errorString)
        {
            errorString = "";

            ValidateModel();
            IEnumerable<string> errors = base.GetErrors("") as IEnumerable<string>;
            errorString = string.Join<string>('\n', errors);
            if (errors.Count() > 0 || !Patient.ValidatePatient(SelectedPatient))
            {
                return false;
                //ValidateProperty(FirstName, "FirstName");
                //ValidateProperty(LastName, "LastName");
                //ValidateProperty(Sex, "Sex");
                //ValidateProperty(DOB, "DOB");                
                //ValidateProperty(Relationship, "Relationship");
            }
            else
            {
                return true;
            }
            return false;
        }


    }



}
