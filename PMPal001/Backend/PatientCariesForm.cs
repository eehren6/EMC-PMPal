using System;
using System.Collections.Generic;
using System.Text;

namespace PMPal.Backend
{
    public class PatientCariesForm
    {
        public string FormID { get; set; }
        public string PersonID { get; set; }

        public string EncounterID { get; set; }

        public DateTime RecordDate { get; set; }

        public Dictionary<int, int?> FormResults { get; set; }

        private bool FormCreated;

        public int Total
        {
            get
            {
                int total = 0;
                if (FormResults != null)
                {
                    foreach (int? val in FormResults.Values)
                    {
                        if (val.HasValue)
                            total += val.Value;
                    }
                }
                return total;
            }
        }

        public PatientCariesForm()
        {
            FormResults = new Dictionary<int, int?>();
        }

        public PatientCariesForm(string personID, string encID)
        {
            this.PersonID = personID;
            this.EncounterID = encID;
            RecordDate = DateTime.Now;
            GetPatientForm();
        }
        public void UpdateForm(int questionID, int value)
        {
            //singular case.
            if (questionID == 10 && value == 2) value = 4;   

            FormResults[questionID] = value;
            SavePatientForm();
        }

        public void SavePatientForm()
        {
            try
            {
                    string user_id = Util.GetActiveUser();
                string sql = "";

                if (!FormCreated)
                {
                   sql = @$"
                    INSERT INTO [dbo].[ezra_dental_caries_frm_]
                       ([enterprise_id]
                       ,[practice_id]
                       ,[person_id]
                       ,[created_by]
                       ,[create_timestamp]
                       ,[modified_by]
                       ,[modify_timestamp]
                       ,[enc_id])
                    SELECT '00001', '0001','{PersonID}','{user_id}','{DateTime.Now:yyyy-MM-dd hh:mm}','{user_id}','{DateTime.Now:yyyy-MM-dd hh:mm}',
                            '{this.EncounterID}'
                    WHERE NOT EXISTS 
                        (SELECT 1 FROM ezra_dental_caries_frm_ WHERE enc_id='{this.EncounterID}');";
                }
                        
                sql += "UPDATE [dbo].[ezra_dental_caries_frm_]"
                + $" SET [modify_timestamp] = '{DateTime.Now:yyyy-MM-dd hh:mm}'"
              + (FormResults[1].HasValue ? $", [flouride_exp] = {FormResults[1]}" : "")
              + (FormResults[2].HasValue ? $", [sugary_drinks] = {FormResults[2]}" : "")
              + (FormResults[3].HasValue ? $", [caries_fam_exp] = {FormResults[3]}" : "")
              + (FormResults[4].HasValue ? $", [dental_hom] =  {FormResults[4]}" : "")
              + (FormResults[5].HasValue ? $", [spec_hc_needs] =  {FormResults[5]}" : "")
              + (FormResults[6].HasValue ? $", [chemo_therapy] =  {FormResults[6]}" : "")
              + (FormResults[7].HasValue ? $", [eating_disorder] =  {FormResults[7]}" : "")
              + (FormResults[8].HasValue ? $", [salv_flow_meds] = {FormResults[8]}" : "")
              + (FormResults[9].HasValue ? $", [drugs_alcohol] =  {FormResults[9]}" : "")
              + (FormResults[10].HasValue ? $", [visual_lesions] =  {FormResults[10]}" : "")
              + (FormResults[11].HasValue ? $", [missing_teeth] =  {FormResults[11]}" : "")
              + (FormResults[12].HasValue ? $", [visible_plaque] = {FormResults[12]}" : "")
              + (FormResults[13].HasValue ? $", [dental_appliances] = {FormResults[13]}" : "")
              + (FormResults[14].HasValue ? $", [salivary_flow] = {FormResults[14]} " : "")
              + (FormResults[15].HasValue ? $", [exposed_root] =  {FormResults[15]}" : "")
              + (FormResults[16].HasValue ? $", [restorations] =  {FormResults[16]}" : "")
              + (FormResults[17].HasValue ? $", [unusual_morphology] = {FormResults[17]}" : "")
              + @$",[total] = {Total}
               WHERE 
                enc_id = '{EncounterID}'";

                Util.Update(sql, System.Data.CommandType.Text, null, null);

                FormCreated = true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void SubmitCharge()
        {
            try
            {
                ApplyCRACharge();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private bool cptApplied = false;
        private void ApplyCRACharge()
        {
            try
            {
                string serviceItemID = Total > 5 ? "D0603" : Total > 2 ? "D0602" : "D0601";
                if (!cptApplied)
                {
                    InsertCPT(serviceItemID);
                    cptApplied = true;
                }

                UpdateCharge(serviceItemID);

            }
            catch(Exception)
            {
                throw;
            }
        }
                    
        private void InsertCPT(string cptCode)
        {

            string spName = "ezra_add_simple_procedure";

            var sp_params = new Dictionary<string, string>()
                {
                    { "@person_id", this.PersonID },
                    { "@enc_id", this.EncounterID },
                    { "@cpt_code", cptCode },
                    { "@user_id", Util.GetActiveUser() },
                    { "@dental_ind","Y" }
                };

            try
            {
                Util.Update(spName, System.Data.CommandType.StoredProcedure, sp_params);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateCharge(string cptCode) 
        {

            string spName = "ezra_edr_update_cra_cpt";

            var sp_params = new Dictionary<string, string>()
                {
                    { "@enc_id", this.EncounterID },
                    { "@new_cpt_code", cptCode },
                    { "@user_id", Util.GetActiveUser() }  
                };
            //NEED TO REPLACE DIFF CPT CODES
            try
            {
                Util.Update(spName, System.Data.CommandType.StoredProcedure, sp_params);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public void GetPatientForm()
        {
            try
            {

                FormResults = new Dictionary<int, int?>();
                for (int i = 0; i <= 17; i++)
                    FormResults[i] = null;

                string sql = $@"Select * from ezra_dental_caries_frm_
                        where enc_id = '{EncounterID}'";

                var record = Util.GetRecord(sql, System.Data.CommandType.Text);
                if(record != null)
                {
                    int res;
                    if (int.TryParse(record["flouride_exp"].ToString(), out res))   FormResults[1] = res;
                    if (int.TryParse(record["sugary_drinks"].ToString(), out res))  FormResults[2] = res;
                    if (int.TryParse(record["caries_fam_exp"].ToString(), out res)) FormResults[3] = res;
                    if (int.TryParse(record["dental_hom"].ToString(), out res))     FormResults[4] = res;
                    if (int.TryParse(record["spec_hc_needs"].ToString(), out res))  FormResults[5] = res;
                    if (int.TryParse(record["chemo_therapy"].ToString(), out res))  FormResults[6] = res;
                    if (int.TryParse(record["eating_disorder"].ToString(), out res)) FormResults[7] = res;
                    if (int.TryParse(record["salv_flow_meds"].ToString(), out res))     FormResults[8] = res;
                    if (int.TryParse(record["drugs_alcohol"].ToString(), out res))      FormResults[9] = res;
                    if (int.TryParse(record["visual_lesions"].ToString(), out res))     FormResults[10] = res;
                    if (int.TryParse(record["missing_teeth"].ToString(), out res)) FormResults[11] = res;
                    if (int.TryParse(record["visible_plaque"].ToString(), out res)) FormResults[12] = res;
                    if (int.TryParse(record["dental_appliances"].ToString(), out res))  FormResults[13] = res;
                    if (int.TryParse(record["salivary_flow"].ToString(), out res))      FormResults[14] = res;
                    if (int.TryParse(record["exposed_root"].ToString(), out res))      FormResults[15] = res;
                    if (int.TryParse(record["restorations"].ToString(), out res))      FormResults[16] = res;
                    if (int.TryParse(record["unusual_morphology"].ToString(), out res)) FormResults[17] = res;

                    FormCreated = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
