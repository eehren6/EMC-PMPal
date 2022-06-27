using PMPal.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace PMPal.Backend
{
    static class Providers
    {
        public static Dictionary<string,Provider> LoadProviders(DataSet dataSet)
        {
            if (dataSet?.Tables.Count == 0)
                return null;

            var prov_dict = new Dictionary<string, Provider>();
            foreach (var row in dataSet.Tables[0].Rows)
            {
                var dr = (DataRow)row;
                var provider = new Provider();

                provider.person_id = dr["provider_id"].ToString();
                provider.First_name = dr["first_name"].ToString();
                provider.Last_name = dr["last_name"].ToString();
                provider.Description = dr["Description"].ToString();

                prov_dict.Add(provider.person_id,provider);
            }

            return prov_dict;
        }

        public static Dictionary<string, ProviderResource> LoadProviderResources(DataSet dataSet)
        {
            if (dataSet?.Tables.Count == 0)
                return null;

            var prov_dict = new Dictionary<string, ProviderResource>();
            foreach (var row in dataSet.Tables[0].Rows)
            {
                var dr = (DataRow)row;
                var providerResource = new ProviderResource(dr["resource_id"].ToString(), dr["description"].ToString());

                prov_dict.Add(providerResource.ResourceID, providerResource);
            }

            return prov_dict;
        }

        public static void BlockDates(string providerResourceList, DateTime startDate, DateTime endDate, string userID, string category)
        {
            try
            {
                //return; //temp for testing.
                //string out_param_id = "CE8A2A95-02EC-414C-BB1E-1BFBA42F2552";   //TODO: replace code
                string sp_name = "prov_out_block_schedule_v2";
                var paramsList = new Dictionary<string, string>();
                paramsList.Add("@resource_ids", providerResourceList);
                paramsList.Add("@date_start", startDate.ToString("yyyyMMdd"));
                paramsList.Add("@date_end", endDate.ToString("yyyyMMdd"));
                paramsList.Add("@begintime", startDate.TimeOfDay.ToString("hhmm"));
                paramsList.Add("@endtime", endDate.TimeOfDay.ToString("hhmm"));
                paramsList.Add("@user_id", userID == null ? "622" : userID);
                paramsList.Add("@category_id", category);

                //TODO: just hardcoding here for now.
                string db_name = Util.Envmnt == "prod" ? "NGEzra" : "";

                var results = Util.Update(sp_name, CommandType.StoredProcedure, paramsList, null, db_name);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
