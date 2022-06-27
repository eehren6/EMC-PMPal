using PMPal.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;

using System.Windows.Threading;

namespace PMPal
{
    /// <summary>
    /// Interaction logic for PatientLookup.xaml
    /// </summary>
    public partial class PatientLookup : Page
    {
        PatientsViewModel model;
        PatientDetails details;


        DispatcherTimer lookupTimer = new DispatcherTimer();
        public PatientLookup()
        {
            InitializeComponent();            
            model = (PatientsViewModel)this.DataContext;
            //model.Patients.CollectionChanged += Patients_CollectionChanged;
            lookupTimer.Interval = TimeSpan.FromSeconds(1);
            lookupTimer.Tick += lookupTimer_Tick;
            this.Loaded += PatientLookup_Loaded;
        }

        private void PatientLookup_Loaded(object sender, RoutedEventArgs e)
        {
            details = new PatientDetails();

            if (model.PatientFound)
            {
                if (model.SendMessage)
                {
                    ShowPatientMessage();
                    model.SendMessage = false;
                }
                if (model.ShowEncounters)
                {
                    ShowEncounterDetails();
                    //model.ShowEncounters = false; don't revert
                }
                else
                    ShowPatientEditDetails();
                
                model.PatientFound = false;
            }
        }

        private void lookupTimer_Tick(object sender, EventArgs e)
        {
            lookupTimer.Stop();
            CallLookup();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainWindow());
        }

        private void txtPatientSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            lookupTimer.Stop();
            lookupTimer.Start();
        }

                
        private void CallLookup()
        {
                if (txtPatientSearch.Text.Length == 0 || txtPatientSearch.Text.Length >= 2)
                {
                    txtPatientSearch.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    patientList.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
                }
        }

      

        private void patientList_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            patientList.Items.Refresh();
        }


        private void patientList_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (patientList.SelectedItem != null)
            {
                model.SelectedPatient = ((Person)((KeyValuePair<string, Person>)patientList.SelectedItem).Value);

                if (model.ShowEncounters)
                    ShowEncounterDetails();
                else
                    ShowPatientEditDetails();
            }
        }

        private void ShowEncounterDetails()
        {
            EncounterSelect encounterSelect = new EncounterSelect(model.SelectedPatient, "", "", true);
            NavigationService?.Navigate(encounterSelect);
        }
        private void ShowPatientDetails()
        {
            DetailsViewModel vwModel = new DetailsViewModel();
            vwModel.SelectedPatient = model.SelectedPatient;
            details.detailsGrid.DataContext = vwModel;
            NavigationService?.Navigate(details);
        }

        private void ShowPatientEditDetails()
        {
            PatientEditDetails patientEditDetails = new PatientEditDetails(model.SelectedPatient, model.Curr_User_ID);
            NavigationService?.Navigate(patientEditDetails);
        }
        private void ShowPatientMessage()
        {
            PatientMessage patientMessage = new PatientMessage(model.SelectedPatient, model.Curr_User_ID);
            NavigationService?.Navigate(patientMessage);
        }
    }
}
