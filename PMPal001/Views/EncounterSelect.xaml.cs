using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PMPal.Models;
using PMPal.ViewModels;

namespace PMPal
{
    /// <summary>
    /// Interaction logic for EncounterSelect.xaml
    /// </summary>
    public partial class EncounterSelect : Page
    {
        public EncountersViewModel EncountersVM = new EncountersViewModel();
        public Person Patient {get;set;}

        //TODO: find a better way to pass variables
        public EncounterSelect(Person patient, string providerID, string startDate, bool dentalOnly)
        {
            InitializeComponent();
            Patient = patient;
            EncountersVM = new EncountersViewModel(Patient, providerID, startDate, dentalOnly);
            DataContext = EncountersVM;
            encList.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();
            if (encList.Items.Count > 0)
            {
                encList.SelectedItem = encList.Items[0];
                EncountersVM.SelectedEncounter = ((KeyValuePair<string, PatientEncounter>)encList.SelectedItem).Value;
            }
        }

        private void encList_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            encList.Items.Refresh();
        }

        private void encList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(encList.SelectedItem!=null)
                EncountersVM.SelectedEncounter = ((KeyValuePair<string, PatientEncounter>)encList.SelectedItem).Value;
        }

        private void ReferralsSelect(string personID, string encID)
        {
            ReferralSelect frm = new ReferralSelect(personID, encID);
            NavigationService?.Navigate(frm);
        }


        private void CariesForm(string personID, string encID, string encNBR)
        {
            CariesForm frm = new CariesForm(personID, encID, encNBR);
            NavigationService?.Navigate(frm);
        }
        private void btnCaries_Click(object sender, RoutedEventArgs e)
        {
            CariesForm(this.Patient.person_id, EncountersVM.SelectedEncounter.EncounterID, EncountersVM.SelectedEncounter.EncounterNbr);

        }

        private void btnReferrals_Click(object sender, RoutedEventArgs e)
        {
            ReferralsSelect(this.Patient.person_id, EncountersVM.SelectedEncounter.EncounterID);

        }

        private void chkDentalOnly_Checked(object sender, RoutedEventArgs e)
        {
            EncountersVM.RefreshEncounters();
        }

        private void chkTodayOnly_Checked(object sender, RoutedEventArgs e)
        {
            EncountersVM.RefreshEncounters();
        }

        private void chkTodayOnly_Unchecked(object sender, RoutedEventArgs e)
        {
            EncountersVM.RefreshEncounters();
        }
    }
}
