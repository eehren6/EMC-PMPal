using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using PMPal.Models;

namespace PMPal
{
    public class Person
    {

        public string person_id { get; set; }

        public string site_id;

        private string _person_nbr;
        public string person_nbr
        {
            get { return string.IsNullOrEmpty(_person_nbr) ? _person_nbr : _person_nbr.Trim(); }
            set { _person_nbr = value; }
        }
        int other_id_number;

        [Required]
        public string Last_name { get; set; }

        [Required]
        public string First_name { get; set; }

        public string Middle_name { get; set; }

        public string Nick_name { get; set; }

        public string Display_Name
        {
            get { return $"{Last_name}, {First_name}"; }
        }
        public string suffix;
        public string prefix;
        public Address PrimaryAddress { get; set; }
        public Address SecondaryAddress { get; set; }

        private string _home_phone;
        public string Home_phone
        {
            get { return _home_phone; }
            set
            { _home_phone = value; } //                   VARCHAR(10)     ;
        }

        public string sec_home_phone { get; set; } //VARCHAR(10);
        public string Masked_Home_Phone
        {
            get
            {
                if (!string.IsNullOrEmpty(_home_phone))
                {
                    string tmp = _home_phone;
                    if (tmp.Length == 11)
                        tmp = tmp.Substring(1);
                    if (tmp.Length == 10)
                        return $"({tmp.Substring(0, 3)}) {tmp.Substring(3, 3)}-{tmp.Substring(6, 4)}";
                    else
                        return tmp;
                }
                else
                    return _home_phone;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string input = value;
                    _home_phone = new String(input.Where(c => c >= '0' && c <= '9').ToArray());
                }
                else
                    _home_phone = value;
            }
        }
        public string day_phone { get; set; }        // VARCHAR(10)     ;
        public string day_phone_ext { get; set; }   // VARCHAR(10)     ;
        public string alt_phone { get; set; }// VARCHAR(10)     ;
        public string alt_phone_desc { get; set; } // VARCHAR(25)     ;
        public string alt_phone_ext;                // VARCHAR(5)      ;

        [Required]
        public string Date_of_birth { get; set; } // VARCHAR(8)      ;
        public string DOB_display
        {
            get { return Date_of_birth != null && Date_of_birth.Length == 8 ? $"{Date_of_birth.Substring(4, 2)}/{Date_of_birth.Substring(6, 2)}/{Date_of_birth.Substring(0, 4)}" : Date_of_birth; }
            set
            {
                DateTime dt;
                if (DateTime.TryParse(value, out dt))
                {
                    Date_of_birth = dt.ToString("yyyyMMdd");
                }
                //Date_of_birth = value; 
            }
        }

        [Required]
        public string Sex { get => sex; set => sex = value; } // CHAR(1)         ;
        public string Ssn { get; set; } // CHAR(9)         ;

        public string Masked_Ssn
        {
            get { return (string.IsNullOrWhiteSpace(Ssn) ? "" : $"###-##-{Ssn.Substring(5)}"); }
        }
        public char marital_status { get; set; } // CHAR(1)         ;
        public char expired_ind; // CHAR(1)         CONSTRAINTDFperson_expired_ind DEFAULT('N')  ;

        public string expired_date; //                 VARCHAR(8)      ;
        public char smoker_ind { get; set; } // CHAR(1)         CONSTRAINTDFperson_smoker_ind DEFAULT('N')  ;

        public string veteran_ind { get; set; }//                  CHAR(1)         CONSTRAINTDFperson_veteran_ind DEFAULT('N')  ;

        public string Race_id { get; set; } //UNIQUEIDENTIFIER

        [Required]
        public string Race { get; set; } //VARCHAR(100)    ;


        public int PCP_Encounters { get; set; }

        public string Member_Status
        {
            get
            {
                if (PCP_Encounters > 3) return "VIP";
                else return "";
            }
        }

        public string language_id { get; set; }

        public string religion_id;                  // UNIQUEIDENTIFIER ;

        public string church_id;                    // UNIQUEIDENTIFIER ;

        public string language { get; set; }             // VARCHAR(100)    ;

        public string lang_barrier { get; set; }
        public string primarycare_prov_id { get; set; }  // UNIQUEIDENTIFIER ;
        public string primarycare_prov_name { get; set; } //        VARCHAR(30)     ;
        public string email_address { get; set; } // VARCHAR(80)     ;
                                                  


        public string uds_homeless_status_id { get; set; }                //        UNIQUEIDENTIFIER ;
        public string homeless { get; set; }
        public string uds_language_barrier_id { get; set; }
        public string uds_primary_med_coverage_id { get; set; }                 //  UNIQUEIDENTIFIER ;
        public string primary_med_coverage { get; set; }
        public string home_phone_comment { get; set; }
        public string day_phone_comment { get; set; }
        public string alt_phone_comment { get; set; }
        public string sec_home_phone_comment { get; set; }
        public string email_address_comment { get; set; }
        public string Contact_pref_id
        {
            get { return contact_pref_id; }
            set { 
                contact_pref_id = value;
                if (!string.IsNullOrEmpty(value))
                {
                    string desc = "";
                    PMPal.Backend.ListRetrieve.ContactPrefOptions.TryGetValue(contact_pref_id.ToUpper(), out desc);
                    _contact_pref_desc = desc;
                }
            }
        }

        private string _contact_pref_desc;          //VARCHAR(50)     ;
        public string Contact_pref_desc {
            get { return _contact_pref_desc; }
            set
            {
                _contact_pref_desc = value;
                if (!string.IsNullOrEmpty(value))
                {
                    string pref_id = PMPal.Backend.ListRetrieve.ContactPrefOptions.FirstOrDefault(x => x.Value == value).Key;
                    if (!string.IsNullOrEmpty(pref_id))
                        contact_pref_id = pref_id;
                }
            }
        }         
        public string contact_alert_ind { get; set; } // CHAR(1)          CONSTRAINTDFperson_contact_alert_ind DEFAULT('N')  ;


        public string self_pay_ind { get; set; }                //                 CHAR(1)         CONSTRAINTDFperson_self_pay_ind DEFAULT('N')  ;

        public string uds_race_id { get; set; }                 //                  UNIQUEIDENTIFIER ;

        public string uds_ethnicity_id { get; set; }            //             UNIQUEIDENTIFIER ;


        public string uds_veteran_status { get; set; }              //           CHAR(1)         CONSTRAINTDFperson_uds_veteran_status DEFAULT('U') ;
        
        public string ethnicity { get; set; }                   //                    VARCHAR(40)     ;
        public string ethnicity_id { get; set; }                //        UNIQUEIDENTIFIER ;
        
        public string cell_phone { get; set; }
       
        public string Cell_Phone_Display
        {
            get
            {
                if (!string.IsNullOrEmpty(cell_phone))
                {
                    string tmp = cell_phone;
                    if (tmp.Length == 11)
                        tmp = tmp.Substring(1);
                    if (tmp.Length == 10)
                        return $"({tmp.Substring(0, 3)}) {tmp.Substring(3, 3)}-{tmp.Substring(6, 4)}";
                    else
                        return tmp;
                }
                else
                    return cell_phone;
            }
        }
        public string cell_phone_comment { get; set; }              // VARCHAR(50)     ;

        public string Contact_Seq { get; set; }

        public string exempt_frm_person_merge { get; set; }

        #region unused fields


        //degree VARCHAR(12)     ;
        //prior_last_name VARCHAR(40)     ;
        //religion VARCHAR(100)    ;
        //church VARCHAR(100)    ;
        //student_status CHAR(1)         ;
        //image_id   UNIQUEIDENTIFIER ;
        //external_id                  VARCHAR(20)     ;
        //        int_home_phone VARCHAR(20)     ;
        //    int_work_phone VARCHAR(20)     ;
        //    int_zip VARCHAR(12)     ;
        //    created_by INT  ;
        //    create_timestamp DATETIME CONSTRAINTDFperson_create_timestamp DEFAULT(getdate())  ;

        //    uds_migrant_worker_status_id UNIQUEIDENTIFIER ;
        //uds_pub_hsng_pri_care_id     UNIQUEIDENTIFIER ;

        //uds_school_hlth_ctr_id       UNIQUEIDENTIFIER ;

        //uds_tribal_affiliation_id    UNIQUEIDENTIFIER ;

        //uds_blood_quantum_id         UNIQUEIDENTIFIER ;
        //    uds_consent_to_treat_ind CHAR(1)         CONSTRAINTDFperson_uds_consent_to_treat_ind DEFAULT('N')  ;

        //community_code_old           VARCHAR(200)    ;
        //    mothers_maiden_name VARCHAR(60)     ;
        //    create_timestamp_tz
        //        SMALLINT ;
        //    modify_timestamp_tz          SMALLINT ;
        //    uds_ihs_elig_status_id       UNIQUEIDENTIFIER ;
        //    uds_tribal_class_id          UNIQUEIDENTIFIER ;
        //    uds_decendancy_id            UNIQUEIDENTIFIER ;
        //    uds_consent_to_treat_date    CHAR(8)         ;
        //    community_code_id
        //        UNIQUEIDENTIFIER ;
        //    primary_dental_prov_id       UNIQUEIDENTIFIER ;
        //    primary_dental_prov_name     VARCHAR(30)     ;

        //    soundex_last_name            CHAR(4)          ;

        //   soundex_first_name           CHAR(4)          ;

        //  prefix_id                    UNIQUEIDENTIFIER ;

        //  suffix_id                    UNIQUEIDENTIFIER ;

        //  prefix_old                   VARCHAR(12)     ;
        //    suffix_old VARCHAR(12)     ;
        //    cause_of_death_code VARCHAR(7)      ;
        //    cause_of_death VARCHAR(255)    ;
        //    birth_mothers_lname VARCHAR(60)     ;
        //    birth_mothers_fname VARCHAR(60)     ;
        //    birth_mothers_mname VARCHAR(60)     ;
        //    expired_time
        //        DATETIME ;
        //    expired_time_tz              SMALLINT ;
        //portal_ind                   CHAR(1)         CONSTRAINTDFperson_portal_ind DEFAULT('N') 

        //ific_pref_ind             CHAR(1)         CONSTRAINTDFperson_ific_pref_ind DEFAULT('N')  ;

        //person_merge_station_id      VARCHAR(50)     ;
        //    person_merge_flag CHAR(1)         ;
        //    risk_level CHAR(10)        ;
        //    exempt_frm_person_merge CHAR(1)         CONSTRAINTDFperson_exempt_frm_person_merge DEFAULT('N')  ;

        //image_id_4frontimage         UNIQUEIDENTIFIER ;

        //image_id_4barcodeImage       UNIQUEIDENTIFIER ;

        //sexual_orientation           CHAR(50)        ;
        //    preferred_pronoun CHAR(50)        ;
        //    enable_home_phone_ind CHAR(1)         CONSTRAINTDFperson_enable_home_phone_ind DEFAULT('N')  ;

        //enable_cell_phone_ind        CHAR(1)         CONSTRAINTDFperson_enable_cell_phone_ind DEFAULT('N')  ;

        //enable_email_address_ind     CHAR(1)         CONSTRAINTDFperson_enable_email_address_ind DEFAULT('N')  ;

        //enable_alt_phone_ind         CHAR(1)         CONSTRAINTDFperson_enable_alt_phone_ind DEFAULT('N')  ;

        //enable_day_phone_ind         CHAR(1)         CONSTRAINTDFperson_enable_day_phone_ind DEFAULT('N')  ;

        //enable_sec_hm_phone_ind      CHAR(1)         CONSTRAINTDFperson_enable_sec_hm_phone_ind DEFAULT('N')  ;

        //enable_int_phone_ind         CHAR(1)         CONSTRAINTDFperson_enable_int_phone_ind DEFAULT('N')  ;

        //current_gender               VARCHAR(25)     ;
        //    previous_first_name VARCHAR(60)     ;
        //    other_reason_so VARCHAR(50)     ;
        #endregion
        public string email_ind { get; set; }                 //                    CHAR(1)         CONSTRAINTDFperson_email_ind DEFAULT('N')  ;

        public string phone_ind { get; set; }                    //CHAR(1)         CONSTRAINTDFperson_phone_ind DEFAULT('N')  ;


        public string sms_ind { get; set; } //                      CHAR(1)         CONSTRAINTDFperson_sms_ind DEFAULT('N')  ;

        public string voice_ind { get; set; } // CHAR(1)         CONSTRAINTDFperson_voice_ind DEFAULT('N')  ;
        public string optout_ind { get; set; } //CHAR(1)         CONSTRAINTDFperson_optout_ind DEFAULT('N')  ;

        public bool Has_sec_address
        {
            get
            {
                return !string.IsNullOrEmpty(SecondaryAddress?.StreetAddress1);
            }
            set
            {
                ;
            }
        }

        private ObservableCollection<Relation> pat_relationships;

        public ObservableCollection<Relation> Patient_Relationships
        {
            get
            {
                if (pat_relationships == null && !string.IsNullOrWhiteSpace(this.person_id))
                    pat_relationships = PatientRelationships.GetPatientRelationships(this.person_id);
                return pat_relationships;
            }

        }
        #region IDataErrorInfo Members
        public string Error
        {
            get { return null; }
        }

        private string this[string columnName]
        {
            get
            {
                if (columnName == "Last_name" && string.IsNullOrEmpty(Last_name))
                    return "Last Name is required";

                if (columnName == "First_name" && string.IsNullOrEmpty(First_name))
                    return "First Name is required";

                if (columnName == "sex" && string.IsNullOrEmpty(sex))
                    return "Gender is required";

                if (columnName == "DOB_display" && string.IsNullOrEmpty(DOB_display))
                    return "Date of Birth is required";

                if (columnName == "Race" && string.IsNullOrEmpty(Race))
                    return "Race is required";

                return null;
            }
        }

        #endregion
        
        private Dictionary<String, List<String>> errors =
            new Dictionary<string, List<string>>();
        private string sex;
        private string contact_pref_id;
        private const string ID_ERROR = "Value cannot be less than 5.";
        private const string ID_WARNING = "Value should not be less than 10.";
        private const string NAME_ERROR = "Value must not contain any spaces.";
        private const string NAME_WARNING = "Value should be 5 characters or less.";

        // Adds the specified error to the errors collection if it is not already 
        // present, inserting it in the first position if isWarning is false. 
        public void AddError(string propertyName, string error, bool isWarning)
        {
            if (!errors.ContainsKey(propertyName))
                errors[propertyName] = new List<string>();

            if (!errors[propertyName].Contains(error))
            {
                if (isWarning) errors[propertyName].Add(error);
                else errors[propertyName].Insert(0, error);
            }
        }

        // Removes the specified error from the errors collection if it is present. 
        public void RemoveError(string propertyName, string error)
        {
            if (errors.ContainsKey(propertyName) &&
                errors[propertyName].Contains(error))
            {
                errors[propertyName].Remove(error);
                if (errors[propertyName].Count == 0) errors.Remove(propertyName);
            }
        }
    }
}
