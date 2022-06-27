//using PMPal001;
using PMPal.Controls;
using PMPal.Views;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PMPal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BackgroundWorker workerThread = null;
        
        //lock object for synchronization;
        //private static object _syncLock = new object();

        bool _keepRunning = false;
        bool _localMachine = true;
        bool _switchingLocal = false;
        string logfolder = Util.GetLogFolder();
        public MainWindow()
        {

            File.AppendAllText($"{Util.GetLogFolder()}\\PMPal-params.txt", $"\nWindow before initialize - {DateTime.Now}");
            InitializeComponent();
            File.AppendAllText($"{Util.GetLogFolder()}\\PMPal-params.txt", $"\nWindow after initialize - {DateTime.Now}");
            this.Initialized += MainWindow_Initialized;
            this.Loaded += MainWindow_Loaded;

            Patient.UIContext = SynchronizationContext.Current;


        }

        private void MainWindow_Initialized(object sender, EventArgs e)
        {
            File.AppendAllText($"{Util.GetLogFolder()}\\PMPal-params.txt", $"\nWindow in initialize - {DateTime.Now}");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            File.AppendAllText($"{Util.GetLogFolder()}\\PMPal-params.txt", $"\nWindow loaded-getting title - {DateTime.Now}");
            this.Title +=  " Version: " + Assembly.GetEntryAssembly().GetName().Version.ToString();
            File.AppendAllText($"{Util.GetLogFolder()}\\PMPal-params.txt", $"\nWindow loaded-title: {this.Title} - {DateTime.Now}");
            InstantiateWorkerThread();

            //this.EventGrid.DataContext = Events.eventData.DefaultView;
            //BindingOperations.EnableCollectionSynchronization(EventGrid.Items, _syncLock);

            workerThread.RunWorkerAsync();

            File.AppendAllText($"{Util.GetLogFolder()}\\PMPal-params.txt", $"\nWindow loaded");
        }
        
        #region threading
        private void InstantiateWorkerThread()
        {
            workerThread = new BackgroundWorker();
            //workerThread.ProgressChanged += WorkerThread_ProgressChanged;
            workerThread.DoWork += WorkerThread_DoWork;
            //workerThread.RunWorkerCompleted += WorkerThread_RunWorkerCompleted;
            workerThread.WorkerReportsProgress = true;
            workerThread.WorkerSupportsCancellation = true;
        }

        //private void WorkerThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    this.EventGrid.Items.Refresh();
        //    this.lstPatientNames.Items.Refresh();
            
        //    if (SynchronizationContext.Current != Patient.UIContext)
        //        Patient.UIContext = SynchronizationContext.Current;
        //    //this.EventGrid.DataContext = PMPal.eventData.DefaultView;
        //}

        //private void WorkerThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    if (e.Cancelled)
        //    {
        //        ;
        //    }
        //    else
        //    {
        //        ;
        //    }

        //    if (_switchingLocal)
        //    {
        //        SwitchLocal();
        //    }
        //}

        //private void SwitchLocal()
        //{
        //    //TODO: make it a soft reset.
        //    ResetGrid();

        //    if (!_blStopped)
        //        workerThread.RunWorkerAsync();

        //    _switchingLocal = false;
        //    btnGetLocal.IsEnabled = true;
        //}

        private void WorkerThread_DoWork(object sender, DoWorkEventArgs e)
        {

            _keepRunning = true;

            while (_keepRunning)
            {

                ReceiveMessages();
                workerThread.ReportProgress(0, new object());

                //_keepRunning = false;
                if (workerThread.CancellationPending)
                {
                    // this is important as it set the cancelled property of RunWorkerCompletedEventArgs to true
                    e.Cancel = true;
                    break;
                }
                Thread.Sleep(5000);
            }
            
        }

        public void ReceiveMessages()
        {
            //messages = new Dictionary<Guid, string>();
            //while (true)
            //{
            using (var conn = new SqlConnection(Util.ConnectionString))
            {
                conn.Open();
                using (var txn = conn.BeginTransaction())
                {
                    TimeSpan _timeout = default;

                    // Get the latest message from the queue.
                    Events.GetMessagesFromRetrieveTable(conn, txn, _timeout, _localMachine, 50, Patient.UIContext);

                    #region old
                    //this.EventGrid.Items.DeferRefresh();
                    //foreach (var messagestring in Util.messages)
                    //{
                    //    /// var messagestring =mes.
                    //    if (messagestring.Key != Guid.Empty)
                    //    {
                    //        var handle = messagestring.Key;
                    //        var handleStr = handle.ToString().ToUpper();
                    //        var requestMessage = new RequestMessage(); //Util.Deserialize<RequestMessage>(message.Item2);
                    //        requestMessage.Text = messagestring.Value;

                    //        if (requestMessage.Text.Contains(Environment.MachineName))
                    //        {
                    //            Console.WriteLine($"{requestMessage.Text}");
                    //            //Console.WriteLine($"Received {handleStr} ({requestMessage.Text})");

                    //        }
                    //        //SendReply(conn, txn, _sendMessageType, handle, requestMessage);
                    //        // End the conversation on the target side.
                    //        Util.EndConversation(conn, txn, handle);
                    //        //Console.WriteLine($"Ended conversation {handleStr}.");
                    //    }
                    //}
                    // Commit the transaction.
                    //this.EventGrid.DataContext = Util.eventData.DefaultView;
                    #endregion
                    txn.Commit();
                }
                //}

                //int _sleepDuration = 500;

                ////Thread.Sleep(_sleepDuration);
                //break;
            }

        }
        private void ResetGrid()
        {
            //model.Clear();
            
            Events.eventData.DefaultView.RowFilter = "";
        }

        #endregion

        public bool ShowUpdateVersionMessage()
        {
            if (MessageBox.Show("There is a more recent version of this application. To download it open the Nextgen Application Launcher from the desktop, then reopen. Do you want to close the application now?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                return true;
            else
                return false;
          }

        public void SetPatientTab(string person_id = "", string mode = "", string user_id = "")
        {
            try
            {
                PatientLookup patientLookup = new PatientLookup();
                var patVwModel = patientLookup.DataContext as PatientsViewModel;
                File.AppendAllText($"{logfolder}\\PMPal-params.txt", $"\ngot PatientsViewModel");

                if(mode == "Message")
                {
                    patVwModel.SendMessage = true;
                }
                if (mode == "Encounters")
                {
                    patVwModel.ShowEncounters = true;
                }
                if (!string.IsNullOrEmpty(user_id))
                {
                    patVwModel.Curr_User_ID = user_id;

                }
                if (!string.IsNullOrEmpty(person_id))
                {
                    string logfolder = Util.GetLogFolder();
                    File.AppendAllText($"{logfolder}\\PMPal-params.txt", $"\ncalling findpatient with personid={person_id}");
                    patVwModel.FindPatient(person_id);
                }

                CloseableTab tabItem = new CloseableTab();
                Frame frame = new Frame();
                frame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Visible;
                tabItem.Content = frame;
                this.tabMain.Items.Add(tabItem);

                string header = patVwModel.ShowEncounters ? "Patient Encounters Lookup" : "Patient Lookup";
                //header += patVwModel.SelectedPatient?.Display_Name;
                
                tabItem.Title = header;
                frame.Navigate(patientLookup);
                tabMain.SelectedItem = tabItem;
                File.AppendAllText($"{logfolder}\\PMPal-params.txt", $"\nNavigated to frame");
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                throw ex;
            }
        }
        public void SetProviderTab()
        {
            ProviderSearch ps = new ProviderSearch();
            CloseableTab tabItem = new CloseableTab();
            Frame frame = new Frame();
            frame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
            tabItem.Content = frame;
            this.tabMain.Items.Add(tabItem);
            string header = "Provider Search";
            tabItem.Title = header;
            frame.Navigate(ps);
            tabMain.SelectedItem = tabItem;
        }
        public void SetCariesTab()
        {
            CariesForm cf = new CariesForm();
            CloseableTab tabItem = new CloseableTab();
            Frame frame = new Frame();
            frame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
            tabItem.Content = frame;
            this.tabMain.Items.Add(tabItem);
            string header = "Caries Form";
            tabItem.Title = header;
            frame.Navigate(cf);
            tabMain.SelectedItem = tabItem;
        }
        public void SetHolidayTab()
        {
            var hol = new Holidays();
            CloseableTab tabItem = new CloseableTab();
            Frame frame = new Frame();
            frame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
            tabItem.Content = frame;
            this.tabMain.Items.Add(tabItem);
            string header = "Holiday Schedule";
            tabItem.Title = header;
            frame.Navigate(hol);
            tabMain.SelectedItem = tabItem;
        }

        private void lblPlusHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetPatientTab();
            e.Handled = true;

        }
        
        private void mnuPatient_Click(object sender, RoutedEventArgs e)
        {
            SetPatientTab();
        }
        private void mnuProvider_Click(object sender, RoutedEventArgs e)
        {
            SetProviderTab();
        }

        private void mnuHoliday_Click(object sender, RoutedEventArgs e)
        {
            SetHolidayTab();
        }

        private void mnuCaries_Click(object sender, RoutedEventArgs e)
        {
            SetCariesTab();
        }

        private void mnuDental_Click(object sender, RoutedEventArgs e)
        {
            SetPatientTab("", "Encounters", "");
        }
    }
    public class StringNullOrEmptyToVisibilityConverter : System.Windows.Markup.MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string)
                ? Visibility.Collapsed : Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
    public class DebugDataBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            //Debugger.Break();
            return value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            //Debugger.Break();
            return value;
        }

    }

}
