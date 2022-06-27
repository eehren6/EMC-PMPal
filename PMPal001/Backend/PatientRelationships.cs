using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using PMPal.Models;
using System.Security.Policy;
using System.Windows.Input;

namespace PMPal
{
    static class PatientRelationships
    {
        private static ObservableDictionary<string, string> relationships_list;
        public static ObservableDictionary<string, string> GetRelationships()
        {
            if (relationships_list == null)
                GetRelationsList();

            return relationships_list;
        }

        private static void GetRelationsList()
        {
            string sql = "select top 1000 * from code_tables where code_type like '%relation%' and delete_ind='N'";
            var ds = Util.Get(sql, System.Data.CommandType.Text);
            var dict = ds.Tables[0].AsEnumerable().ToDictionary(row => row.Field<string>("code"), row => row.Field<string>("description"));
            relationships_list = new ObservableDictionary<string, string>(dict);
        }

        public static string GetReciprocalRelation(string relation, string person_id)
        {
            string procedure_name = "ezra_get_recip_relationship";
            try
            {
                var parms = new Dictionary<string, string>();
                parms.Add("@person_id", person_id);
                parms.Add("@relationship", relation);
                string recip_relationship = Util.GetString(procedure_name, CommandType.StoredProcedure, parms);
                return recip_relationship;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            //switch (relation)
            //{
            //    case "Sibling":
            //        return "Sibling";
            //    case "Spouse":
            //        return "Spouse";
            //    case "Significant Other":
            //        return "Significant Other";
            //    case "Adopted Child":
            //        if (recip_gender == "Male")
            //            return "Father";
            //        else
            //            return "Mother";
            //    case "Child, Father is the Patient":
            //        return "Father";
            //    case "Child, Mother is the Patient":
            //        return "Mother";
            //    case "Father":
            //        return "Child, Father is the Patient";
            //    case "Mother":
            //        return "Child, Mother is the Patient";
            //    case "Friend":
            //        return "Friend";
            //    case "Foster Child":
            //        return "Guardian";
            //    default:
            //        return "";
            //}
        }

        public static void AddPersonRelationship(string person_id, string relation_id, string code, string user_id)
        {
            string procedure_name = "ng_add_person_relationship";
            try
            {
                var parms = new Dictionary<string, string>();
                parms.Add("@pi_person_id", person_id);
                parms.Add("@pi_related_person_id", relation_id);
                parms.Add("@pi_relation_code", code);
                parms.Add("@pi_user_id", user_id); // Events.GetActiveUser(Util.Update_Envmnt));
                Util.Update(procedure_name, CommandType.StoredProcedure, parms, null);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ObservableCollection<Relation> GetPatientRelationships(string person_id)
        {

            string procedure_name = "ezra_ng_get_person_relationship";
            try
            {
                var parms = new Dictionary<string, string>();
                parms.Add("@pi_person_id", person_id);
                var res = Util.Get(procedure_name, CommandType.StoredProcedure, parms, null);

                var Relations = new ObservableCollection<Relation>();
                if (res.Tables.Count == 1)
                {
                    foreach (var record in res.Tables[0].Rows)
                    {
                        DataRow dr = record as DataRow;
                        var rel = new Relation();
                        rel.First_Name = dr["first_name"].ToString();
                        rel.Last_Name = dr["last_name"].ToString();
                        rel.Relationship = dr["description"].ToString();
                        Relations.Add(rel);
                    }
                }
                return Relations;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

}
