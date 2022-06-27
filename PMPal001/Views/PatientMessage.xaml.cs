using MaterialDesignThemes.Wpf;
using PMPal.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace PMPal
{
    /// <summary>
    /// Interaction logic for PatientDetails.xaml
    /// </summary>
    public partial class PatientMessage : Page
    {
        PatientMessageViewModel model;
        public PatientMessage()
        {
            InitializeComponent();
            //model = new PatientMessageViewModel();
            txtMessageText.Text = model.GetDefaultMessage("1");
        }

        public PatientMessage(Person selectedPatient, string user_id = "")
        {
            model = new PatientMessageViewModel(selectedPatient, user_id);
            this.DataContext = model;
            InitializeComponent();
            this.detailsGrid.DataContext = model;
            txtMessageText.Text = model.GetDefaultMessage("1");
            cmbDepartments.ItemsSource = model.DepartmentDict;
            cmbDepartments.DisplayMemberPath = "Value";
            cmbDepartments.SelectedValuePath = "Key";
        }
              

        private void BackLabel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void cmbDepartments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model.SetSelectedDepartment(cmbDepartments.SelectedValue.ToString());
        }

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserID))
                {
                    model.UserID = Util.GetActiveUser();
                }

                if (string.IsNullOrEmpty(model.UserID))
                {
                    model.UserID = "655";
                    //DialogHost.Show("User must be logged into Nextgen first.", "diagHost");
                    //return;
                }
                //else
                //{
                    string dept = "Ezra Medical";
                    string result = model.SendMessage(dept, txtMessageText.Text);
                lblMessageResult.Visibility = Visibility.Visible;
                txtMessageText.Text = "";
                //}
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }



        private void txtMessageText_TextChanged(object sender, TextChangedEventArgs e)
        {
            int maxLength = 500;
            if (txtMessageText.Text.Length >= maxLength)
            {
                DialogHost.Show($"You have reached the {maxLength} character maximum message length. ");
                txtMessageText.Text = txtMessageText.Text.Substring(0, maxLength);
                txtMessageText.CaretIndex = maxLength;
            }

            if(txtMessageText.Text.Length > 0)
            {
                lblMessageResult.Visibility = Visibility.Hidden;
            }
        }
    }
}
