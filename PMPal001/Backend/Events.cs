using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

using System.Threading;

namespace PMPal
{
    static class Events
    {
        public static DataTable eventData = new DataTable();


        //lock object for synchronization;
        private static object _syncLock = new object();
        
        /*TODO: add record amount and wait.*/
        public static void GetMessagesFromRetrieveTable(SqlConnection connection, SqlTransaction transaction, TimeSpan timeout, bool blLocalMachine, int recordAmount, SynchronizationContext context)
        {
            using (var command = connection.CreateCommand())
            {
                // Create the command text for receiving a message.
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = $"retrieve_audit_data";// $"WAITFOR (EXECUTE retrieve_audit_data ), TIMEOUT {timeout.TotalMilliseconds}";
                command.Transaction = transaction;
                command.Parameters.AddWithValue("@station", (blLocalMachine ? Environment.MachineName.ToString() : ""));
                command.Parameters.AddWithValue("@num_records", 50);

                //var tmp_eventData = new DataSet();
                ////tmp_eventData.EnforceConstraints = false;
                //tmp_eventData.Tables.Add(new DataTable());
                
                // Create a data reader.
                using (var reader = command.ExecuteReader())
                {
                    if (reader != null && reader.HasRows)
                    {
                        try
                        {
                            //TODO: clear test code
                            //lock (_syncLock)
                            //{
                            //tmp_eventData.Tables[0].Load(reader, LoadOption.Upsert);

                            //var res = from b in tmp_eventData.Tables[0].AsEnumerable() group b by b.Field<byte[]>("row_timestamp") into g                                      
                            //          let list = g.ToList()
                            //          where list.Count>1
                            //          select new
                            //          {
                            //              timestamp = g.Key,
                            //              Count = list.Count
                            //          };
                                      //.Select("row_timestamp").GroupBy(s => s)
    //.SelectMany(grp => grp.Skip(1));
                            //eventData.Merge(tmp_eventData.Tables[0], true);
                            eventData.Load(reader, LoadOption.Upsert);
                            //context.Send(x => patientList.Add(person_id, patient_text), null);
                            //}
                        }
                        catch (Exception ex)
                        {
                        //    DataRow[] rowErrors = tmp_eventData.Tables[0].GetErrors();

                        DataRow[] rowErrors = eventData.GetErrors();
                        System.Diagnostics.Debug.WriteLine("YourDataTable Errors:"
                                + rowErrors.Length);

                            for (int i = 0; i < rowErrors.Length; i++)
                            {
                                System.Diagnostics.Debug.WriteLine(rowErrors[i].RowError);

                                foreach (DataColumn col in rowErrors[i].GetColumnsInError())
                                {
                                    System.Diagnostics.Debug.WriteLine(col.ColumnName
                                        + ":" + rowErrors[i].GetColumnError(col));
                                }
                            }
                        }
                        finally
                        {
                            try
                            {
                                foreach (var row in eventData.Rows)
                                {
                                    var dr = row as DataRow;
                                    string person_id = dr["person_id"].ToString();

                                    DateTime dob;
                                    DateTime.TryParse(dr["date_of_birth"].ToString(), out dob);
                                    string dob_display = dob.ToString("MMddyyyy");
                                    string patName = $"{dr["last_name"]}, {dr["first_name"]}";
                                    DateTime event_time = DateTime.Parse(dr["event_timestamp"].ToString());
                                    string init_time = event_time.ToString("HH:mm:ss");

                                    int pcp_enc_count;
                                    int.TryParse(dr["pcp_enc_count"].ToString(),out pcp_enc_count);
                                    
                                        //string current_user_id = dr["user_id"].ToString();

                                    if (!Patient.patientList.ContainsKey(person_id))
                                    {

                                        try
                                        {
                                            var patient = Patient.GetPatientInfo(person_id); //Patient.LoadInitPatient(person_id, current_user_id, pcp_enc_count);

                                            Patient.AddUpdatePatient(person_id, patient, init_time);
                                        }
                                        catch (Exception ex)
                                        {
                                            throw ex;
                                        }
                                        //lock (_syncLock)
                                        //{
                                        //    context.Send(x => Patient.patientList.Add(person_id, patient_text), null);

                                        //}
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }



                    }

                }

                return; //eventData;
            }
        }

        public static void ClearLists()
        {
            eventData?.Clear();
            Patient.patientList?.Clear();
            Patient.patients?.Clear();
            //RefreshList();
        }

    }
}
