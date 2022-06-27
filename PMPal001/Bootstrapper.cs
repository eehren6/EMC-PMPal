using Microsoft.Extensions.DependencyInjection;
using PMPal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace PMPal
{
    class Bootstrapper
    {
        static string appName = Assembly.GetEntryAssembly().GetName().Name;
        static Mutex mutex = new Mutex(true, appName);
        static bool isMutexOwner = false;
        MainWindow wnd;
        DispatcherTimer scanTimer = new DispatcherTimer();

        private static string tmpPMPalArgsFilePath = System.IO.Path.GetTempPath() + "pmpal.tmp";
        public void StartupApp(StartupEventArgs e)
        {
            try
            {
                var logFolder = Util.GetLogFolder();
                File.AppendAllText($"{logFolder}\\PMPal-params.txt", "\napplication started - " + DateTime.Now.ToString());

                if (mutex.WaitOne(0, true)) //this is the mutex owner
                {
                    isMutexOwner = true;
                    scanTimer.Interval = TimeSpan.FromSeconds(2);
                    scanTimer.Tick += ScanTimer_Tick;

                    var serviceProvider = new ServiceCollection()
                        .AddLogging()
                        .AddTransient<PatientsViewModel>()
                        .AddTransient<PatientEditDetails>();


                    File.AppendAllText($"{logFolder}\\PMPal-params.txt", $"\nis owner, args length is: {e.Args.Length} - {DateTime.Now.ToString()}");
                    wnd = new MainWindow();

                    
                    if (e.Args.Length > 0)
                    {
                        //if (e.Args.Length > 1)
                        //    MessageBox.Show(e.Args[0].ToString() + " " + e.Args[1].ToString());
                        //else
                        //    MessageBox.Show(e.Args[0].ToString());

                        HandleParams(e.Args);
                    }
                    else
                        wnd.SetPatientTab();

                    /*for debugging purposes*/
                    PresentationTraceSources.Refresh();
                    PresentationTraceSources.DataBindingSource.Listeners.Add(new ConsoleTraceListener());
                    PresentationTraceSources.DataBindingSource.Listeners.Add(new DebugTraceListener());
                    PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning | SourceLevels.Error;
                    
                    //File.AppendAllText($"{logFolder}\\PMPal-params.txt", $"\nBefore show window - {DateTime.Now}");
                    wnd.Show();

                    /*check for latest version and possibly shut down.*/
                    if (!CheckLatestVersion())
                        if (wnd.ShowUpdateVersionMessage())
                        {
                            Application.Current.Shutdown();
                            return;
                        }

                    //File.AppendAllText($"{logFolder}\\PMPal-params.txt", $"\nAfter show window - {DateTime.Now}");
                    scanTimer.Start();

                }
                else if (e.Args.Length > 0)
                {
                    File.WriteAllText(System.IO.Path.GetTempPath() + "pmpal.tmp", e.Args[0].ToString());
                    Application.Current.Shutdown();
                }
            }
            catch (AbandonedMutexException ex)
            {

                var logFolder = Util.GetLogFolder();
                File.AppendAllText($"{logFolder}\\PMPal-params.txt", ex.ToString());
                mutex.ReleaseMutex();
                mutex.Dispose();
            }
            catch (Exception ex)
            {
                var logFolder = Util.GetLogFolder();
                File.AppendAllText($"{logFolder}\\PMPal-params.txt", ex.ToString());
            }
        }

        

        public static void ReleaseMutex()
        {
            if (isMutexOwner)
            {
                mutex.ReleaseMutex();
                mutex.Dispose();
            }
        }

        private void HandleParams(string[] args)
        {
            try
            {
                string logfolder = Util.GetLogFolder();
                string logFile = $"{logfolder}\\PMPal-params.txt";
                File.WriteAllText(logFile, string.Join(',', args));

                string target = "";

                for (int i = 0; i < args.Length; i++)
                {
                    string targetText = "target=";
                    if (args[i].Contains(targetText))
                    {
                        target = getValueFromParam(targetText, args[i], 1);
                    }
                    string user_id = "";
                    string userText = "user_id=";

                    if (args[i].Contains(userText))
                    {
                        //File.AppendAllText(logFile, $"\ngetting user text...");
                        user_id = getValueFromParam(userText, args[i]); //since don't know the length, it'll need to be last param for now

                        //File.AppendAllText(logFile, "user text " + user_id);
                        if (target == "2") { wnd.SetProviderTab(); }
                    }

                    if (target == "1")
                        SetPatientTab(args[i],"Edit", user_id);

                    if (target == "3")
                        SetPatientTab(args[i],"Message", user_id);

                    if (target == "4")
                        SetPatientTab(args[i], "Encounters", user_id);
                    if (target == "0")
                        UpdateLatestVersion();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetPatientTab(string args, string mode, string user_id)
        {
            string patText = "person_id=";
            if (args.Contains(patText))
            {
                //File.AppendAllText(logFile, $"\ngetting patient text... ");
                var person_id = getValueFromParam(patText, args, 36);
                //File.AppendAllText(logFile, "\npatient text: " + person_id);

                if (!string.IsNullOrEmpty(person_id))
                    wnd.SetPatientTab(person_id,mode, user_id);
            }
        }

        private string getValueFromParam(string searchText, string args, int length = 0)
        {
            try
            {
                var logFile = $"{Util.GetLogFolder()}\\PMPal-params.txt";

                if (length > 0)
                {
                    int startIndex = args.IndexOf(searchText) + searchText.Length;
                    int currentArgLength = startIndex + length;
                    if (args.Length < currentArgLength) {

                        length = args.IndexOf(' ', startIndex) > 0 ? args.IndexOf(' ', startIndex) - startIndex : args.Length - startIndex;
                        currentArgLength = startIndex + length;
                    }
                    File.AppendAllText(logFile, $"\ncurrentArgLength: {currentArgLength}, args.length: {args.Length}");
                    if (args.Length >= currentArgLength)
                    {
                        var value = args.Substring(startIndex, length);
                        return string.IsNullOrEmpty(value) ? "empty" : value;
                    }
                    else return "boo";
                }
                else
                    return args.Substring(args.IndexOf(searchText) + searchText.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void ScanTimer_Tick(object sender, EventArgs e)
        {
            ScanTextParam();
        }

        private void ScanTextParam()
        {
            if (File.Exists(tmpPMPalArgsFilePath))
            {
                string parms = File.ReadAllText(tmpPMPalArgsFilePath);
                var args = parms.Split(',');
                HandleParams(args);
                //string user_id = "";
                //string userText = "user_id=";
                //if (parms.Contains(userText))
                //    user_id = getValueFromParam(userText, parms); //since don't know the length, it'll need to be last param for now

                //string patText = "person_id=";
                //if (parms.Contains(patText))
                //{
                //    var person_id = getValueFromParam(patText, parms, 36);
                //    if (!string.IsNullOrEmpty(person_id))
                //        SetPatientTab(person_id, user_id);
                //}
                File.Delete(tmpPMPalArgsFilePath);

            }
        }

        private bool CheckLatestVersion()
        {

            string curr_version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            var latest_version = Util.GetLatestVersion();

            if (curr_version == latest_version)
                return true;
            else
                return false;

        }

        private void UpdateLatestVersion()
        {
            Util.UpdateLatestVersion();
        }
    }

}
