
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace PMPal
{
    static class Util
    {
        public static string Envmnt = ConfigurationManager.AppSettings["env"].ToString();
        public static string Update_Envmnt = ConfigurationManager.AppSettings["update_env"].ToString();

        public static string ConnectionString = ConfigurationManager.ConnectionStrings[$"audit_{Envmnt}"].ConnectionString;
        //public static ObservableDictionary<Guid, string> messages = new ObservableDictionary<Guid, string>();


        public static string GetActiveUser(string env = "")
        {
            /*first try hard-coded backend setting*/
            string user_id = ConfigurationManager.AppSettings["user_id"].ToString();
            if (!string.IsNullOrEmpty(user_id))
                return user_id;

            /*else, if not returned*/

            if (env == "")
                env = Util.Envmnt;

            var sp_params = new Dictionary<string, string>()
            {
                {"@station_id",Environment.MachineName },
                {"@username",Environment.UserName }
            };

            try
            {
                var ds = Get("ezra_get_active_user", CommandType.StoredProcedure, sp_params, null, env);
                if (ds.Tables[0]?.Rows.Count == 0)
                    return null;
                else
                {
                    user_id = ds.Tables[0]?.Rows[0][0].ToString();
                    return user_id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetLatestVersion()
        {

            //var sp_params = new Dictionary<string, string>()
            //{
            //    {"@curr_version_num", Environment.MachineName },
            //    {"@environment", Envmnt}
            //};

            try
            {
                var latest_version = GetString("get_pmpal_version", CommandType.StoredProcedure, null, null, "ezra");

                return latest_version;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void UpdateLatestVersion()
        {

            string curr_version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

            var sp_params = new Dictionary<string, string>()
            {
                {"@curr_version_num", curr_version },
                {"@environment", Envmnt}
            };

            try
            {
                Update("update_pmpal_versions", CommandType.StoredProcedure, sp_params, null, "NGEzra");

                //return latest_version;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet Get(String commandText, CommandType commandType)
        {
            try
            {
                return Get(commandText, commandType, null, null);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static DataSet Get(String commandText, CommandType commandType, Dictionary<string, string> parameterCollection = null, Dictionary<string, string> output_params = null, string Env = "")
        {
            try
            {
                if (Env == "")
                    Env = Envmnt;
                //string connectionString = ConfigurationManager.ConnectionStrings["devl"].ConnectionString;
                DataSet ds = new DataSet();
                var results = new List<string>();
                string connString = ConfigurationManager.ConnectionStrings[Env].ConnectionString;
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    using (var sqlTransaction = conn.BeginTransaction())
                    {
                        //TimeSpan _timeout = default; //todo

                        using (var command = conn.CreateCommand())
                        {
                            // Create the command text for receiving a message.
                            command.CommandType = commandType;

                            command.CommandText = commandText;
                            command.Transaction = sqlTransaction;

                            //foreach (var param in parameterCollection)
                            //{
                            //    command.Parameters.AddWithValue(param.Key, param.Value);
                            //}

                            if (commandType == CommandType.StoredProcedure && parameterCollection != null)
                            {
                                SqlCommandBuilder.DeriveParameters(command);
                                foreach (SqlParameter param in command.Parameters)
                                {
                                    //param.ParameterName = param.ParameterName.Replace("@", "");
                                    if (param.Direction == ParameterDirection.InputOutput)
                                        param.Direction = ParameterDirection.Output;

                                    /*if we have a value for that paramater, pass it into the derivedParameters. */
                                    foreach (var p in parameterCollection)
                                        if (p.Key == param.ParameterName)
                                            param.Value = p.Value;

                                    //if(output_params != null)
                                    //    AddOutputParameters(output_params, command);
                                    //param.ParameterName = "@" + param.ParameterName;

                                }
                            }
                            try
                            {
                                using (var adapter = new SqlDataAdapter(command))
                                {
                                    adapter.Fill(ds);

                                    //command.ex
                                }
                            }
                            catch (Exception ex)
                            {
                                 throw;

                            }
                            finally
                            {
                                conn.Close();
                            }

                        }
                    }

                }

                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string GetString(String commandText, CommandType commandType = CommandType.Text, Dictionary<string, string> parameterCollection = null, Dictionary<string, string> output_params = null, string Env = "")
        {
            try
            {
                var ds = Get(commandText, commandType, parameterCollection, output_params, Env);
                if (ds.Tables != null && ds.Tables.Count == 1)
                    if(ds.Tables[0].Rows.Count > 0)
                        return ds.Tables[0].Rows[0][0].ToString();
                    else
                        return "";
                return null;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static DataRow GetRecord(String commandText, CommandType commandType, Dictionary<string, string> parameterCollection = null, Dictionary<string, string> output_params = null, string Env = "")
        {
            try {

                var ds = Get(commandText, commandType, parameterCollection, output_params, Env);
                if (ds.Tables != null && ds.Tables.Count == 1)
                    if (ds.Tables[0].Rows.Count > 0)
                        return ds.Tables[0].Rows[0];

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /*TODO: handle errors*/
        public static List<string> Update(String commandText, CommandType commandType, Dictionary<string, string> parameterCollection, Dictionary<string, string> output_params = null, string db_name = "")
        {
            //string connectionString = ConfigurationManager.ConnectionStrings["devl"].ConnectionString;
            //string envmt = string.IsNullOrWhiteSpace(envmnt_name) ? Util.Update_Envmnt : envmnt_name;
            var results = new List<string>();
            string connString = ConfigurationManager.ConnectionStrings[Util.Update_Envmnt].ConnectionString;
            
            //TODO: fix this up
            if (db_name == "NGEzra")
            {
                connString = connString.Replace("NGProd", db_name);
            }
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();

                using (var sqlTransaction = conn.BeginTransaction())
                {
                    //TimeSpan _timeout = default; //todo

                    using (var command = conn.CreateCommand())
                    {
                        // Create the command text for receiving a message.
                        command.CommandType = commandType;

                        command.CommandText = commandText;
                        command.Transaction = sqlTransaction;

                        //foreach (var param in parameterCollection)
                        //{
                        //    command.Parameters.AddWithValue(param.Key, param.Value);
                        //}

                        //AddOutputParameters(output_params, command);

                      
                        try
                        {
                            if (commandType == CommandType.StoredProcedure)
                            {
                                SqlCommandBuilder.DeriveParameters(command);
                                foreach (SqlParameter derivedParam in command.Parameters)
                                {
                                    //param.ParameterName = param.ParameterName.Replace("@", "");
                                    if (derivedParam.Direction == ParameterDirection.InputOutput)
                                        derivedParam.Direction = ParameterDirection.Output;


                                    /*if we have a value for that paramater, pass it into the derivedParameters. */
                                    foreach (var p in parameterCollection)
                                        if (p.Key == derivedParam.ParameterName && !string.IsNullOrWhiteSpace(p.Value))
                                            derivedParam.Value = p.Value;

                                    /*if the parameter is guid, convert the value to a guid as well and put it back in as such.*/
                                    if (derivedParam.DbType == DbType.Guid && derivedParam.Value != null)
                                    {
                                        var guidValue = new Guid(derivedParam.Value.ToString());
                                        derivedParam.Value = guidValue;
                                    }
                                    
                                    /*same with bit*/
                                    if (derivedParam.DbType == DbType.Boolean && derivedParam.Value != null)
                                    {
                                        var bitValue = bool.Parse(derivedParam.Value.ToString());
                                        derivedParam.Value = bitValue;
                                    }

                                    if (derivedParam.Value == "null")
                                        derivedParam.Value = DBNull.Value;
                                    //param.ParameterName = "@" + param.ParameterName;

                                }
                            }
                            string script = CommandAsSql(command);
                            command.ExecuteNonQuery();
                            command.Transaction.Commit();

                            if (output_params != null)
                            {
                                foreach (var out_param in output_params)
                                    results.Add(command.Parameters[out_param.Key].Value.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;

                            //return results;
                        }

                    }
                }
            }

            return results;
        }

        /*not needed, we retrieve the params already*/
        private static void AddOutputParameters(Dictionary<string, string> output_params, SqlCommand command)
        {
            foreach (var out_param in output_params)
            {
                if (out_param.Value == "int")
                {
                    command.Parameters.Add(out_param.Key, SqlDbType.Int).Direction = ParameterDirection.Output;
                }
                else
                {
                    int size = 0;
                    if (int.TryParse(out_param.Value, out size))
                    {
                        command.Parameters.Add(out_param.Key, SqlDbType.VarChar, size).Direction = ParameterDirection.Output;
                    }
                }

            }
        }
        public static String ParameterValueForSQL(this SqlParameter sp)
        {
            String retval = "";

            switch (sp.SqlDbType)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.Time:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                //case SqlDbType.DateTimeOffset:
                //    retval = "'" +  sp.Value?.ToString().Replace("'", "''") + "'";
                //    break;

                //case SqlDbType.Bit:
                //    retval = (sp.Value.ToBooleanOrDefault(false)) ? "1" : "0";
                //    break;

                default:
                    retval = sp.Value == null ? "''" : sp.Value.ToString().Replace("'", "''");
                    break;
            }

            return retval;
        }

        public static String CommandAsSql(this SqlCommand sc)
        {
            StringBuilder sql = new StringBuilder();
            Boolean FirstParam = true;

            sql.AppendLine("use " + sc.Connection.Database + ";");
            switch (sc.CommandType)
            {
                case CommandType.StoredProcedure:
                    sql.AppendLine("declare @return_value int;");

                    foreach (SqlParameter sp in sc.Parameters)
                    {
                        if ((sp.Direction == ParameterDirection.InputOutput) || (sp.Direction == ParameterDirection.Output))
                        {
                            sql.Append("declare " + sp.ParameterName + "\t" + sp.SqlDbType.ToString() + "\t= ");

                            sql.AppendLine(((sp.Direction == ParameterDirection.Output) ? "null" : sp.ParameterValueForSQL()) + ";");

                        }
                    }

                    sql.AppendLine("exec [" + sc.CommandText + "]");

                    foreach (SqlParameter sp in sc.Parameters)
                    {
                        if (sp.Direction != ParameterDirection.ReturnValue)
                        {
                            sql.Append((FirstParam) ? "\t" : "\t, ");

                            if (FirstParam) FirstParam = false;

                            if (sp.Direction == ParameterDirection.Input)
                                sql.AppendLine(sp.ParameterName + " = " + sp.ParameterValueForSQL());
                            else

                                sql.AppendLine(sp.ParameterName + " = " + sp.ParameterName + " output");
                        }
                    }
                    sql.AppendLine(";");

                    sql.AppendLine("select 'Return Value' = convert(varchar, @return_value);");

                    foreach (SqlParameter sp in sc.Parameters)
                    {
                        if ((sp.Direction == ParameterDirection.InputOutput) || (sp.Direction == ParameterDirection.Output))
                        {
                            sql.AppendLine("select '" + sp.ParameterName + "' = convert(varchar, " + sp.ParameterName + ");");
                        }
                    }
                    break;
                case CommandType.Text:
                    sql.AppendLine(sc.CommandText);
                    break;
            }

            return sql.ToString();
        }



        /* old
        public static List<Patient> GetPatientData(SqlConnection connection, SqlTransaction transaction, string queueName, TimeSpan timeout)
        {
            var patientList = new List<Patient>();
            var tupleList = new List<Tuple<Guid, string>>();

            using (var command = connection.CreateCommand())
            {
                // Create the command text for receiving a message.
                command.CommandType = CommandType.StoredProcedure;
                //command.CommandText = 
                command.Transaction = transaction;

                // Create a data reader.
                using (var reader = command.ExecuteReader())
                {
                    if (reader != null && reader.HasRows)
                    {
                        // Get the values from the DataReader.
                        reader.Read();
                        string messageBodyString = "";
                        var conversationHandle = reader.GetSqlGuid(reader.GetOrdinal("conversation_handle")).Value;
                        var messageVal = reader.GetValue(reader.GetOrdinal("message_body")).ToString();
                        if (messageVal != "")
                        {
                            var messageBody = reader.GetSqlBinary(reader.GetOrdinal("message_body")).Value;
                            messageBodyString = Encoding.Unicode.GetString(messageBody);
                        }

                        var messageTypeName = reader.GetSqlString(reader.GetOrdinal("message_type_name")).Value;

                        reader.Close();
                        //command.Transaction.Commit();
                        switch (messageTypeName)
                        {
                            case "http://schemas.microsoft.com/SQL/ServiceBroker/EndDialog":
                                EndConversation(connection, transaction, conversationHandle);
                                tupleList.Add(new Tuple<Guid, string>(Guid.Empty, null));
                                break;
                            case "http://schemas.microsoft.com/SQL/ServiceBroker/Error":
                                var messageAsString = messageBodyString;// Deserialize<string>(messageBody);
                                EventLog.WriteEntry("Application", messageAsString, EventLogEntryType.Error);
                                EndConversation(connection, transaction, conversationHandle);
                                tupleList.Add(new Tuple<Guid, string>(Guid.Empty, null));
                                break;
                            default: 
                                tupleList.Add(new Tuple<Guid, string>(Guid.Empty, null));
                                break;
                        }
                    }
                    //else
                    //{
                    //    // No rows returned.
                    //    return new Tuple<Guid, string>(Guid.Empty, null);
                    //}
                    return patientList;
                }
            }
        }

    */
        public static string GetLogFolder()
        {
            try
            {
                string logfolder = ConfigurationManager.AppSettings["log_folder"].ToString();
                if (!Directory.Exists(logfolder))
                    Directory.CreateDirectory(logfolder);

                return logfolder;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string GetSignatureFolder()
        {
            try
            {
                string sql = @"SELECT 
		                    preference_value + '\'
	                    FROM macro_master WITH(NOLOCK), preferences p WITH(NOLOCK)
	                    JOIN preference_list pl ON p.preference_id = pl.preference_id 
	                    WHERE 1=1
		                    AND macro_name = 'provider_signature'
		                    AND preference_name = 'Document Image Path'";

                string folder =  GetString(sql);
                string fileServer = "\\\\" + ConfigurationManager.AppSettings["file_server"].ToString();
                folder = folder.Replace("SERVERFILE-A", "").Replace("\\\\", fileServer);
                
                return folder;
            }
            catch (Exception)
            {
                throw;
            }


        }
        
        public static byte[] Serialize(object obj)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Flush();
                return stream.ToArray();
            }
        }
        public static object Deserialize(byte[] bytes)
        {
            if (bytes != null && bytes.LongLength > 0)
            {               

                using (var stream = new MemoryStream(bytes))
                {
                    var formatter = new BinaryFormatter();
                    formatter.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
                    return formatter.Deserialize(stream);
                }
            }
            else
            {
                return null;
            }
        }


        public static T Deserialize<T>(byte[] bytes)
        {
            var obj = Deserialize(bytes);
            if (obj != null)
            {
                return (T)obj;
            }
            return default(T);
        }
    }


    sealed class AllowAllAssemblyVersionsDeserializationBinder : System.Runtime.Serialization.SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;

            String currentAssembly = System.Reflection.Assembly.GetExecutingAssembly().FullName;

            // In this case we are always using the current assembly
            assemblyName = currentAssembly;

            // Get the type using the typeName and assemblyName
            typeToDeserialize = Type.GetType(String.Format("{0}, {1}",
                typeName, assemblyName));

            return typeToDeserialize;
        }
    }
}
