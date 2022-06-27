using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PMPal
{
    static class Patient
    {
        public static ObservableDictionary<string, string> patientList = new ObservableDictionary<string, string>();
        public static ObservableDictionary<string, Person> patients = new ObservableDictionary<string, Person>();

        public static SynchronizationContext UIContext
        {
            get; set;
        }

        public static Person LoadInitPatient(string person_id, string user_id, int pcp_enc_count)
        {
            var pat = LoadInitPatient(person_id, user_id);
            pat.PCP_Encounters = pcp_enc_count;
            return pat;
        }
        public static Person LoadInitPatient(string person_id, string user_id)
        {
            Person pat = new Person();
            try
            {
                
                var pat_record = Events.eventData.Select("person_id = '" + person_id + "'").FirstOrDefault();

                pat.person_id = pat_record["person_id"].ToString();
                pat.First_name = pat_record["first_name"].ToString();
                pat.Last_name = pat_record["last_name"].ToString();
                pat.Date_of_birth = pat_record["date_of_birth"].ToString();
                return pat;
            }
            catch (Exception ex)
            {
                var error = "patient not found";

                return pat;
            }

        }

        #region old LoadPatientFromEvents
        //public static Person LoadPatientFromEvents(string person_id, int user_id)
        //{
        //    Person pat = new Person();
        //    try
        //    {
        //        var pat_record = Events.eventData.Select("person_id = '" + person_id + "'").FirstOrDefault();

        //        pat.person_id = pat_record["person_id"].ToString();
        //        pat.person_nbr = pat_record["person_nbr"].ToString();
        //        pat.First_name = pat_record["first_name"].ToString();
        //        pat.Last_name = pat_record["last_name"].ToString();
        //        pat.Middle_name = pat_record["middle_name"].ToString();
        //        pat.Date_of_birth = pat_record["date_of_birth"].ToString();
        //        pat = pat_record["address_line_1"].ToString();
        //        pat.Address_line_2 = pat_record["address_line_2"].ToString();
        //        pat.City = pat_record["city"].ToString();
        //        pat.State = pat_record["state"].ToString();
        //        pat.Zip = pat_record["zip"].ToString();
        //        pat.Home_phone = pat_record["home_phone"].ToString();
        //        pat.Race = pat_record["Race"].ToString();
        //        pat.Race_id = pat_record["Race_ID"].ToString();
        //        pat.Sex = pat_record["Sex"].ToString();

        //        //SelectedPatient = pat;


        //        return pat;
        //    }
        //    catch (Exception ex)
        //    {
        //        var error = "patient not found";

        //        return pat;
        //    }

        //}
        #endregion
        public static Person GetPatientInfo(string person_id)
        {
            try
            {
                //if already gathered, just return record.
                if (patients.ContainsKey(person_id) && !string.IsNullOrEmpty(patients[person_id].person_nbr))
                {
                    return patients[person_id];
                }
                else
                {
                    List<Person> results = DoPatientLookup(person_id);

                    var p = new Person();
                    if (results.Count == 1)
                        p = results[0];

                    return p;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static List<Person> DoPatientLookup(string person_id, string filter_text = "")
        {
            try
            {
                string commandText = "ezra_get_person_record_v2";
                var pms = new Dictionary<string, string>();
                pms.Add("@pi_person_id", person_id);
                pms.Add("@pi_search_text", filter_text);
                var results = new List<Person>();

                var ds = Util.Get(commandText, CommandType.StoredProcedure, pms, null);

                results = LoadPatientsData(ds.Tables[0].Select());
                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static List<Person> LoadPatientsData(DataRow[] records)
        {
            var new_patients = new List<Person>();

            foreach(var row in records) 
            {
                DataRow dr = row as DataRow;
                Person pat = new Person();

                try
                {
                    pat.person_id = dr["person_id"].ToString();
                    pat.person_nbr = dr["person_nbr"].ToString();
                    pat.First_name = dr["first_name"].ToString();
                    pat.Last_name = dr["last_name"].ToString();
                    pat.Middle_name = dr["middle_name"].ToString();
                    pat.Nick_name = dr["Nickname"].ToString();
                    pat.Date_of_birth = dr["date_of_birth"].ToString();
                    pat.Sex = dr["Sex"].ToString();
                    pat.Ssn = dr["Ssn"].ToString();

                    /*Address Info*/

                    var address = new Models.Address();
                    address.StreetAddress1 = dr["address_line_1"].ToString();
                    address.StreetAddress2 = dr["address_line_2"].ToString();
                    address.City = dr["city"].ToString();
                    address.State = dr["state"].ToString();
                    address.Zip = dr["zip"].ToString();
                    address.Country = dr["country"].ToString();
                    address.Country_id = dr["country_id"].ToString();
                    pat.PrimaryAddress = address;

                    address = new Models.Address();
                    address.StreetAddress1 = dr["sec_address_line_1"].ToString();
                    address.StreetAddress2 = dr["sec_address_line_2"].ToString();
                    address.City = dr["sec_city"].ToString();
                    address.State = dr["sec_state"].ToString();
                    address.Zip = dr["sec_zip"].ToString();
                    address.Country = dr["sec_country"].ToString();
                    pat.SecondaryAddress = address;
                                        
                    /*contact info*/
                    pat.Home_phone = dr["home_phone"].ToString();
                    pat.home_phone_comment = dr["home_phone_comment"].ToString();
                    pat.day_phone = dr["day_phone"].ToString();
                    pat.day_phone_ext = dr["day_phone_ext"].ToString();
                    pat.day_phone_comment = dr["day_phone_comment"].ToString();
                    pat.alt_phone = dr["alt_phone"].ToString();
                    pat.alt_phone_ext = dr["alt_phone_ext"].ToString();
                    pat.alt_phone_desc = dr["alt_phone_desc"].ToString();
                    pat.alt_phone_comment = dr["alt_phone_comment"].ToString();
                    pat.cell_phone = dr["cell_phone"].ToString();
                    pat.cell_phone_comment = dr["cell_phone_comment"].ToString();
                    pat.sec_home_phone = dr["sec_home_phone"].ToString();
                    pat.sec_home_phone_comment = dr["sec_home_phone_comment"].ToString();
                    pat.email_address = dr["email_address"].ToString();
                    pat.email_address_comment = dr["email_address_comment"].ToString();

                    /*contact pref info*/
                    pat.email_ind = dr["email_ind"].ToString();
                    pat.phone_ind = dr["phone_ind"].ToString();
                    pat.sms_ind = dr["sms_ind"].ToString();
                    pat.voice_ind = dr["voice_ind"].ToString();
                    pat.optout_ind = dr["optout_ind"].ToString();
                    pat.Contact_pref_id = dr["Contact_pref_id"].ToString();
                    pat.Contact_pref_desc = dr["contact_pref_desc"].ToString();
                    pat.Contact_Seq = dr["contact_seq"].ToString();

                    /*ethnic info*/
                    pat.Race = dr["Race"].ToString();
                    pat.Race_id = dr["Race_Id"].ToString();
                    pat.ethnicity = dr["ethnicity"].ToString();
                    pat.ethnicity_id = dr["ethnicity_id"].ToString();
                    pat.uds_ethnicity_id = dr["uds_ethnicity_id"].ToString();
                    pat.language = dr["language"].ToString();
                    pat.language_id = dr["language_id"].ToString();
                    pat.lang_barrier = dr["lang_barrier"].ToString();
                    pat.uds_language_barrier_id = dr["uds_language_barrier_id"].ToString();
                    pat.uds_veteran_status = dr["uds_veteran_status"].ToString();
                    pat.uds_homeless_status_id = dr["uds_homeless_status_id"].ToString();
                    pat.homeless = dr["homeless"].ToString();

                    /*dr and coverage info*/
                    pat.uds_primary_med_coverage_id = dr["uds_primary_med_coverage_id"].ToString();
                    pat.primary_med_coverage = dr["payer_name"].ToString();
                    pat.primarycare_prov_id = dr["primarycare_prov_id"].ToString();
                    pat.primarycare_prov_name = dr["primarycare_prov_name"].ToString();
                    pat.self_pay_ind = dr["self_pay_ind"].ToString();
                    pat.PCP_Encounters = (int)dr["pcp_enc_count"];


                    new_patients.Add(pat);
                }
                catch (Exception ex)
                {
                    throw new Exception("Patient Not Found", ex);

                }
            }

            return new_patients;
        }

        public static string GetPatientEthnicity(string person_id, ref string ethnicity_string)
        {
            try
            {
                string commandText = "ezra_ngkbm_get_ethnicity";
                var pms = new Dictionary<string, string>();
                var outputpms = new Dictionary<string, string>();
                pms.Add("@person_id", person_id);
                string ethnicity_ids = "";

                var ds = Util.Get(commandText, CommandType.StoredProcedure, pms, outputpms);
                if (ds.Tables.Count > 0)
                {
                    ethnicity_string = ds.Tables[0].Rows[0][0].ToString();
                    ethnicity_ids = ds.Tables[0].Rows[0][1].ToString();

                }
                return ethnicity_ids;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static ObservableDictionary<string, Person> GetPatients(string filter_text = "")
        {
            if (string.IsNullOrEmpty(filter_text))
            {
                return patients;
            }
            else
            {
                return PatientLookup(filter_text);
            }
        }

        public static ObservableDictionary<string, Person> PatientLookup(string filter_text)
        {
            
            var results = DoPatientLookup("", filter_text);

            var tmpPatients = results.ToDictionary(x => x.person_id);

            return new ObservableDictionary<string, Person>(tmpPatients);

            
        }

        public static ObservableDictionary<string, string> GetPatientDisplayList()
        {
            return patientList;
        }

              

        //lock object for synchronization;
        private static object _syncLock = new object();
        public static void AddUpdatePatient(string person_id, Person p, string timeAdded = "")
        {
            if(string.IsNullOrEmpty(timeAdded))
                timeAdded = DateTime.Now.ToString("HH:mm:ss");

            if (!patients.ContainsKey(person_id))
            {
                if (UIContext == SynchronizationContext.Current)
                {
                    patients.Add(person_id, p);
                }
                else
                {
                    lock (_syncLock)
                    {
                        UIContext.Send(x =>
                        patients.Add(person_id, p), null);
                    }
                }
            }
            else if (string.IsNullOrEmpty(patients[person_id].person_nbr))
            {
                p.PCP_Encounters = patients[person_id].PCP_Encounters;  //pass to new record as it's not retrieved here.
                patients[person_id] = p;

            }
                

            if (!patientList.ContainsKey(person_id)) //probably not necessary just extra precaution
            {
                var text = $"{timeAdded} {p.First_name}, {p.Last_name} ({p.DOB_display})";
                if (p.PCP_Encounters >= 3)
                    text += " VIP";

                if (UIContext == SynchronizationContext.Current)
                    Patient.patientList.Add(person_id, text);
                else
                {
                    lock (_syncLock)
                    {
                        UIContext.Send(x => Patient.patientList.Add(person_id, text), null);
                    }
                }
            }
        }

        #region deprecated
        //public static string CopyPatient(Person copyFromPat, Person newPatient)
        //{
        //    //Dictionary<string, string> 

        //    var pat_params = new Dictionary<string, string>();
        //    pat_params.Add("pi_enterprise_id", "00001");
        //    pat_params.Add("pi_practice_id", "0001");
        //    pat_params.Add("pi_first_name", newPatient.First_name);
        //    pat_params.Add("pi_middle_name", newPatient.Middle_name);
        //    pat_params.Add("pi_last_name", newPatient.Last_name);
        //    pat_params.Add("pi_date_of_birth", newPatient.Date_of_birth);
        //    pat_params.Add("pi_user_id", Events.GetActiveUser());
        //    pat_params.Add("pi_site_id", "000");
        //    pat_params.Add("pi_address_line_1", copyFromPat.Address_line_1);
        //    pat_params.Add("pi_address_line_2", copyFromPat.Address_line_2);
        //    pat_params.Add("pi_city", copyFromPat.City);
        //    pat_params.Add("pi_state", copyFromPat.State);
        //    pat_params.Add("pi_zip", copyFromPat.Zip);
        //    pat_params.Add("pi_race_id", copyFromPat.Race_id);
        //    pat_params.Add("pi_race", copyFromPat.Race);

        //    //pat_params.Add("pi_country_id", "");
        //    //pat_params.Add("pi_county_id", "");
        //    //pat_params.Add("pi_country", "");
        //    //pat_params.Add("pi_county", "");
        //    //pat_params.Add("pi_ssn","");


        //    pat_params.Add("pi_home_phone", copyFromPat.Home_phone);
        //    pat_params.Add("pi_sex", newPatient.Sex.ToString());
        //    pat_params.Add("pi_expired_ind", "N");
        //    pat_params.Add("pi_smoker_ind", "N");
        //    pat_params.Add("pi_email_ind", "N");
        //    pat_params.Add("pi_phone_ind", "Y");
        //    pat_params.Add("pi_portal_ind", "N");
        //    pat_params.Add("pi_sms_ind", "N");
        //    pat_params.Add("pi_voice_ind", "N");
        //    pat_params.Add("pi_optout_ind", "N");
        //    pat_params.Add("pi_notific_pref_ind", "N");
        //    pat_params.Add("pi_veteran_ind", "N");
        //    pat_params.Add("pi_contact_alert_ind", "N");
        //    pat_params.Add("pi_self_pay", "N");
        //    pat_params.Add("pi_enterprise_chart_ind", "N");
        //    pat_params.Add("pi_uds_consent_to_treat", "N");
        //    pat_params.Add("pi_exempt_frm_person_merge", "N");
        //    pat_params.Add("pi_enable_home_phone_ind", "N");
        //    pat_params.Add("pi_enable_day_phone_ind", "N");
        //    pat_params.Add("pi_enable_alt_phone_ind", "N");
        //    pat_params.Add("pi_enable_sec_hm_phone_ind", "N");
        //    pat_params.Add("pi_enable_email_address_ind", "Y");
        //    pat_params.Add("pi_enable_cell_phone_ind", "Y");
        //    pat_params.Add("pi_enable_int_phone_ind", "N");
        //    pat_params.Add("pi_current_gender", newPatient.Sex.ToString());
        //    SavePatient(newPatient, pat_params);

        //    return newPatient.person_id;
        //}

        //private static void SavePatient(Person newPatient, Dictionary<string, string> pat_params)
        //{
        //    try
        //    {
        //        if (ValidatePatient(newPatient))
        //        {
        //            var out_params = new Dictionary<string, string>();
        //            out_params.Add("@pio_person_id", "36");
        //            out_params.Add("@po_result_code", "int");
        //            out_params.Add("@po_result_message", "255");
        //            var results = Util.Update("ng_add_person", CommandType.StoredProcedure, pat_params, out_params);

        //            if (results.Count == 3)
        //            {
        //                newPatient.person_id = results[0];
        //                SaveNewPersonAuditRecord(newPatient.person_id);
        //                newPatient = GetPatientInfo(newPatient.person_id);

        //                AddUpdatePatient(newPatient.person_id, newPatient);
        //                //SelectedPatient = newPatient;
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //    }
        //}
        #endregion
        public static void SaveNewPersonAuditRecord(string person_id)
        {

            var parms = new Dictionary<string, string>();
            parms.Add("@person_id", person_id);
            var sp_name = "ezra_add_pmpal_person_audit";
            try 
            { 
                Util.Update(sp_name, CommandType.StoredProcedure, parms, null);            
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static Person SavePatient(Person newPatient, string user_id)
        {
            var pat_params = new Dictionary<string, string>();

            var pat_props = newPatient.GetType()
     .GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var pat_dict = pat_props
          .ToDictionary(prop => prop.Name, prop => prop.GetValue(newPatient, null));

            foreach(var p in pat_dict)
            {
                if (p.Value == null || !p.Value.GetType().Namespace.Contains("PMPal"))                
                {
                    pat_params.Add("@pi_" + p.Key.ToLower(), (p.Value == null ? "" : p.Value.ToString()));
                }
            }

            //addresses
            pat_params.Add("@pi_address_line_1", newPatient.PrimaryAddress.StreetAddress1);
            pat_params.Add("@pi_address_line_2", newPatient.PrimaryAddress.StreetAddress2);
            pat_params.Add("@pi_city", newPatient.PrimaryAddress.City);
            pat_params.Add("@pi_state", newPatient.PrimaryAddress.State);
            pat_params.Add("@pi_zip", newPatient.PrimaryAddress.Zip);
            pat_params.Add("@pi_country", newPatient.PrimaryAddress.Country);
            pat_params.Add("@pi_country_id", newPatient.PrimaryAddress.Country_id);

            pat_params.Add("@sec_address_line_1", newPatient.SecondaryAddress?.StreetAddress1);
            pat_params.Add("@sec_address_line_2", newPatient.SecondaryAddress?.StreetAddress2);
            pat_params.Add("@sec_city", newPatient.SecondaryAddress?.City);
            pat_params.Add("@sec_state", newPatient.SecondaryAddress?.State);
            pat_params.Add("@sec_zip", newPatient.SecondaryAddress?.Zip);
            pat_params.Add("@sec_country", newPatient.SecondaryAddress?.Country);
            pat_params.Add("@sec_country_id", newPatient.SecondaryAddress?.Country_id);

            pat_params.Add("@pi_user_id", user_id);
            pat_params.Add("@pi_site_id", "000");
            pat_params.Add("@pi_enterprise_id", "00001");
            pat_params.Add("@pi_practice_id", "0001");
            pat_params.Add("@pi_current_gender", newPatient.Sex?.ToString());

            if (string.IsNullOrWhiteSpace(pat_params["@pi_race_id"]))
                pat_params["@pi_race_id"] = "null";

            var out_params = new Dictionary<string, string>();
            out_params.Add("@pio_person_id", "36");
            out_params.Add("@po_result_code", "int");
            out_params.Add("@po_result_message", "255");

            try
            {
                if (ValidatePatient(newPatient))
                {
                    var results = Util.Update("ng_add_patient", CommandType.StoredProcedure, pat_params, out_params);

                    if (results.Count == 3)
                    {
                        newPatient.person_id = results[0];

                        var sp_params = new Dictionary<string, string>()
                        {
                            { "@station", Environment.MachineName },
                            { "@user_id", user_id },
                            { "@person_id", newPatient.person_id }
                        };

                        Util.Update("ezra_set_active_station_patient", CommandType.StoredProcedure, sp_params, null);

                        Util.Envmnt = Util.Update_Envmnt;  //need to temporarily point to the env we just updated.

                        newPatient = GetPatientInfo(newPatient.person_id);

                        if(Util.Update_Envmnt != "devl")
                            SaveNewPersonAuditRecord(newPatient.person_id);


                        //and make sure to set it back.
                        Util.Envmnt = System.Configuration.ConfigurationManager.AppSettings["env"].ToString();

                        AddUpdatePatient(newPatient.person_id, newPatient);
                    }
                    return newPatient;
                }
                else
                    return null;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        public static bool ValidatePatient(Person p)
        {
            if (p.Date_of_birth == null || p.Date_of_birth.Length != 8 || !int.TryParse(p.Date_of_birth, out _))
                return false;
            
            if (string.IsNullOrEmpty(p.Last_name) 
                || string.IsNullOrEmpty(p.First_name) 
                || string.IsNullOrWhiteSpace(p.Sex))
                //|| string.IsNullOrEmpty(p.Race_id))
                return false;
            
            //if (!(p.phone_ind == "Y" || p.email_ind == "Y" || p.optout_ind == "Y"))
            //    return false;

            //if (p.Patient_Relationships == null || p.Patient_Relationships.Count == 0)
            //    return false;

            return true;
        }

        public static string SendPatientMessage(Person p, string dept, string userID, string message)
        {
            var pars = new Dictionary<string, string>()
            {
                { "@department",dept  },
                { "@user_id",userID },
                { "@message", message },
                { "@person_id", p.person_id }
            };

            string sp_name = "sms_PMPal";

            try
            {
                var results = Util.Update(sp_name, CommandType.StoredProcedure, pars, null, "NGEzra");

                return results.ToString();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }

}
