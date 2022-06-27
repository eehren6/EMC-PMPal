using PMPal.ViewModels;
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

namespace PMPal
{
    /// <summary>
    /// Interaction logic for PatientDetails.xaml
    /// </summary>
    public partial class PatientDetails : Page
    {
        public PatientDetails()
        {
            InitializeComponent();
            this.ShowsNavigationUI = false;
        }

        private void btnAddMember_Click(object sender, RoutedEventArgs e)
        {
            var model = detailsGrid.DataContext as DetailsViewModel;
            PatientEditDetails editDetails = new PatientEditDetails(model.SelectedPatient);
            //editDetails.detailsGrid.DataContext = this.detailsGrid.DataContext;
            NavigationService.Navigate(editDetails);
        }


      



        private void detailsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //grdPatRelationships.ItemsSource = ((PatientViewModel)detailsGrid.DataContext).SelectedPatient.Patient_Relationships.def;
            //grdPatRelationships.Items.Refresh();
        }

        private void BackLabel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void btnOpenMessage_Click(object sender, RoutedEventArgs e)
        {
            var model = detailsGrid.DataContext as DetailsViewModel;
            PatientMessage patMessage = new PatientMessage(model.SelectedPatient);
            //editDetails.detailsGrid.DataContext = this.detailsGrid.DataContext;
            NavigationService.Navigate(patMessage);
        }
    }
}
