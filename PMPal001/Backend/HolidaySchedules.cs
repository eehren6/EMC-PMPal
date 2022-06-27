using PMPal.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace PMPal.Backend
{
    static class HolidaySchedules
    {

        private static ObservableCollection<HolidaySchedule> _holidaySchedulesDict;
        public static ObservableCollection<HolidaySchedule> HolidaySchedulesDict
        {
            get
            {
                if (_holidaySchedulesDict == null)
                    _holidaySchedulesDict = RetrieveHolidaySchedules();
                return _holidaySchedulesDict;
            }

        }
        private static ObservableCollection<HolidaySchedule> RetrieveHolidaySchedules()
        {
            try
            {
                var res = Util.Get("select * from holiday_schedule", 
                    CommandType.Text,null,null,"ezra").Tables[0].AsEnumerable();

                var dict = new ObservableCollection<HolidaySchedule>();
                foreach (var row in res)
                {
                    HolidaySchedule sched = new HolidaySchedule();
                    sched.ID = row["seq_no"].ToString();
                    sched.Description = row["description"].ToString();
                    sched.FromDate = DateTime.Parse(row["start_datetime"].ToString());
                    sched.ToDate = DateTime.Parse(row["end_datetime"].ToString());
                    dict.Add(sched);
                }

                return dict;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private static void RefreshSchedules()
        {
            _holidaySchedulesDict = null;
        }
        public static void InsertSchedule(string description, DateTime startDate, DateTime endDate)
        {
            try 
            { 
                var sp_params = new Dictionary<string, string>()
                {
                    { "@description", description },
                    { "@start_datetime", startDate.ToString() },
                    { "@end_datetime", endDate.ToString() },
                    { "@mode", "1" },
                    { "@user_ID", Util.GetActiveUser() }
                };
                var sp_name = "holiday_schedule_modify";
                Util.Update(sp_name, CommandType.StoredProcedure, sp_params, null, "NGEzra");
                RefreshSchedules();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void UpdateSchedule(string scheduleID, string description, DateTime startDate, DateTime endDate)
        {
            try { 
                var sp_params = new Dictionary<string, string>()
                {
                    { "@scheduleID", scheduleID },
                    { "@description", description },
                    { "@start_datetime", startDate.ToString() },
                    { "@end_datetime", endDate.ToString() },
                    { "@mode", "2" },
                    { "@user_ID", Util.GetActiveUser() }
                };
                var sp_name = "holiday_schedule_modify";
                Util.Update(sp_name, CommandType.StoredProcedure, sp_params, null, "NGEzra"); 
                RefreshSchedules();
            }
            catch(Exception ex)
            {
                throw ex;
            }
}

        public static void RemoveSchedule(string scheduleID)
        {
            try { 
                var sp_params = new Dictionary<string, string>()
                {
                    { "@scheduleID", scheduleID },
                    //{ "@description", description },
                    //{ "@start_datetime", startDate.ToString() },
                    //{ "@end_datetime", endDate.ToString() },
                    { "@mode", "3" },
                    { "@user_ID", Util.GetActiveUser() }
                };
                var sp_name = "holiday_schedule_modify";
                Util.Update(sp_name, CommandType.StoredProcedure, sp_params, null, "NGEzra");
                RefreshSchedules();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
