using PMPal.Models;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PMPal.Backend.Repository
{
    static class ProvidersRepo
    {
        private static Dictionary<string, Provider> _providersDict;
        private static Dictionary<string, ProviderResource> _providerRecourcesDict;


        public static Dictionary<string, Provider> GetProviders()
        {
            if (_providersDict == null)
            {
                loadProviders();
            }
            return _providersDict;
        }

        public static Dictionary<string, ProviderResource> GetProviderResources(string filter = "")
        {
            if (_providerRecourcesDict == null)
            {
                loadProviderResources();
            }
            //if(!string.IsNullOrEmpty(filter))
            //{
            //    var results = _providerRecourcesDict
            //            .Where(x => x.Value.Description.Contains(filter));
                
            //    return results.ToDictionary()
            //}
            return _providerRecourcesDict;
        }

        private static void loadProviders()
        {
            string sp_name = "ezra_get_providers_pm";

            var res = Util.Get(sp_name, System.Data.CommandType.StoredProcedure);
            _providersDict = Providers.LoadProviders(res);
        }

        private static void loadProviderResources()
        {
            string sql = "SELECT * FROM RESOURCES WHERE DELETE_IND='N'";

            var res = Util.Get(sql, System.Data.CommandType.Text);
            _providerRecourcesDict = Providers.LoadProviderResources(res);
        }

    }
}
