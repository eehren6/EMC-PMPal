using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using PMPal.Models;

namespace PMPal.Backend.Repository
{
    public static class PatientEncounters
    {
        public static ObservableDictionary<string, PatientEncounter> GetPatientEncountersDental(string personID, string providerID, string startDate)
        {
            try
            {
                //string locationID = "2C098DAF-8039-46F8-B59E-A3A25D223BD0"; //1312 Dental

                return GetPatientEncounters(personID, providerID, startDate, true);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public static ObservableDictionary<string, PatientEncounter> GetPatientEncounters(string personID, string providerID, string startDate, bool dentalOnly = false)
        {
            try
            {
                var encounters = new ObservableDictionary<string, PatientEncounter>();
                string sql = @$"select * from patient_encounter pe
                    JOIN provider_mstr pm on pm.provider_id = pe.rendering_provider_id
                    JOIN location_mstr lm on lm.location_id = pe.location_id
                    WHERE 1=1
                    AND person_id = '{personID}' 
                    AND ('{providerID}' = '' OR convert(varchar(36), rendering_provider_id) = '{providerID}') 
                    AND pe.create_timestamp >= '{startDate}'
                    AND ('{dentalOnly}' = 'false' OR location_name like '%dental%' OR location_name like 'v -%')
                    order by pe.create_timestamp desc";

                var dataSet = Util.Get(sql, System.Data.CommandType.Text);
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        var enc = new PatientEncounter();
                        enc.PersonID = personID;
                        enc.EncounterID = row["enc_ID"].ToString();
                        enc.ProviderID = row["rendering_provider_id"].ToString();
                        enc.EncounterNbr = row["enc_nbr"].ToString();
                        enc.EncounterDate = row["create_timestamp"].ToString();
                        enc.ProviderName = row["first_name"].ToString() + " " + row["last_name"].ToString();

                        encounters.Add(enc.EncounterID, enc);
                    }
                }

                return encounters;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
