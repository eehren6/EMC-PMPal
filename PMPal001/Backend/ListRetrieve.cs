using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;

namespace PMPal.Backend
{
    static class ListRetrieve
    {

        private static Dictionary<string, string> _contactPrefOptions;
        public static Dictionary<string, string> ContactPrefOptions
        {
            get {
                if (_contactPrefOptions == null)
                    _contactPrefOptions = GetContactPrefOptions();
                return _contactPrefOptions; }

        }
        private static Dictionary<string, string> GetContactPrefOptions()
        {
            string sp_name = "ezra_get_contact_pref_options";
            var res = Util.Get(sp_name, System.Data.CommandType.StoredProcedure).Tables[0].AsEnumerable();
            return res.ToDictionary(x => x.Field<string>("pref_id"), x => x.Field<string>("pref_description"));
        }
    }
}
