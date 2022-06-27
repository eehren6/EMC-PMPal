using System;
using System.Collections.Generic;
using System.Globalization;
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
using MaterialDesignThemes.Wpf;
using PMPal.ViewModels;

namespace PMPal
{
    /// <summary>
    /// Interaction logic for PatientDetails.xaml
    /// </summary>
    public partial class PatientEditDetails : Page
    {
        DetailsViewModel model;
        private Person newFamilyMember;
        public PatientEditDetails()
        {
            InitializeComponent();
        }
        public PatientEditDetails(Person selectedPatient, string user_id = "")
        {
            InitializeComponent();

            this.DataContext = new DetailsViewModel(selectedPatient);
            model = this.DataContext as DetailsViewModel;
            model.Curr_User_ID = user_id;

            CopyPatient();

            txt_l_name.GetBindingExpression(TextBox.TextProperty).UpdateTarget();

            cmbRelationships.ItemsSource = PatientRelationships.GetRelationships();
            cmbRelationships.DisplayMemberPath = "Value";
            cmbRelationships.SelectedValuePath = "Key";

            cmbGender.ItemsSource = model.GenderValues;
            cmbGender.DisplayMemberPath = "Key";
            cmbGender.SelectedValuePath = "Value";

            //cmbContactPrefs.ItemsSource = model.ContactPrefOptions;
            //cmbContactPrefs.DisplayMemberPath = "Value";
            //cmbContactPrefs.SelectedValuePath = "Key";

            //cmbContactPrefs.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateTarget();
        }

        private void CmbContactPrefs_SourceUpdated(object sender, DataTransferEventArgs e)
        { 
        }

        string original_person_id = "";
        private void CopyPatient()
        {
            original_person_id = model.SelectedPatient.person_id;
            newFamilyMember = model.CreateNewFamilyMember();
            model.SelectedPatient = newFamilyMember;
        }

        private bool SaveCopyPatient()
        {
            try
            {
                txt_f_name.GetBindingExpression(TextBox.TextProperty).UpdateSource();

                string errorString;
                if (!model.ValidatePatient(out errorString))
                {
                    DialogHost.Show("The new patient is missing some required information:\n" + errorString, "diagHost");

                    return false;
                }
                //else if(!stkContactPrefs.BindingGroup.CommitEdit()) //separate check for this since it's a group validation. -EE
                //{
                //    DialogHost.Show("The new patient is missing some required information:\nNotification Preference is required." + errorString, "diagHost");

                //    return false;
                //}
                else
                {
                    string user_id = !string.IsNullOrEmpty(model.Curr_User_ID) ? model.Curr_User_ID : Util.GetActiveUser();

                    if (string.IsNullOrEmpty(user_id))
                    {
                        DialogHost.Show("User must be logged into Nextgen first.", "diagHost");
                        return false;
                    }
                    else
                    {
                        newFamilyMember = Patient.SavePatient(newFamilyMember, user_id);

                        if (cmbRelationships.SelectedValue != null)
                        {
                            PatientRelationships.AddPersonRelationship(newFamilyMember.person_id, original_person_id, cmbRelationships.SelectedValue.ToString(), user_id);
                            //apply reciprocal relationship
                            string recip_relation = PatientRelationships.GetReciprocalRelation(cmbRelationships.Text, original_person_id);
                            if(recip_relation != "")
                                PatientRelationships.AddPersonRelationship(original_person_id, newFamilyMember.person_id, recip_relation, user_id);
                        }
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
            //model.AddPatient(newSibling);
        }

        private void address_expander_Expanded(object sender, RoutedEventArgs e)
        {
            if (address_expander.IsExpanded)
                sec_addr_expander.GetBindingExpression(Expander.IsExpandedProperty)?.UpdateTarget();
        }

        private void sec_addr_expander_Expanded(object sender, RoutedEventArgs e)
        {
            /*need to reset binding in case it was lost by manually expanding.*/

            //var newBinding = new Binding("Has_sec_address");
            //newBinding.Mode = BindingMode.OneWay;
            //sec_addr_expander.SetBinding(Expander.IsExpandedProperty, newBinding);
            e.Handled = true;
        }

        private void expander_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Expander expander = (Expander)sender;
            if (expander.Name == "sec_addr_expander") //BAD
            {

            }
            if (!expander.IsVisible)
            {
                expander.IsExpanded = false;
                if (expander.Name == "sec_addr_expander") //BAD
                {
                    var binding = BindingOperations.GetBindingExpression(sec_addr_expander, Expander.IsExpandedProperty);
                    if (binding == null)
                    {
                        var newBinding = new Binding("Has_sec_address");
                        newBinding.Mode = BindingMode.OneWay;
                        sec_addr_expander.SetBinding(Expander.IsExpandedProperty, newBinding);

                    }
                }
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (SaveCopyPatient())
            {
                var page = new PatientDetails();
                page.detailsGrid.DataContext = this.DataContext;
                NavigationService.Navigate(page);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void BackLabel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
                NavigationService.GoBack();
        }

        private void cmbContactPrefs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            var binding = BindingOperations.GetBindingExpression(cmb, ComboBox.SelectedValueProperty);
            if (binding.HasError)
            {
                binding.UpdateTarget();
            }
        }

        private void chkOptout_Checked(object sender, RoutedEventArgs e)
        {
            chkPhoneInd.IsChecked = false;
            chkPhoneInd.IsEnabled = false;
        }

        private void chkOptout_Unchecked(object sender, RoutedEventArgs e)
        {
            chkPhoneInd.IsEnabled = true;
        }

        private void StackPanel_LostFocus(object sender, RoutedEventArgs e)
        {
            stkContactPrefs.BindingGroup.CommitEdit();
            //((FrameworkElement)stkContactPrefs.ContentTemplate.FindName("RootElement1", (FrameworkElement)VisualTreeHelper.GetChild(this.contentControl1, 0))).BindingGroup.CommitEdit();
            //var prefsRule = new ContactPrefsNullRule();
            //ValidationError error = new ValidationError(prefsRule, ContactPrefsGroup.ValidationRules);
        }
    }


    public class AgeRangeRule : ValidationRule
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public AgeRangeRule()
        {
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int age = 0;

            try
            {
                if (value != null)
                {
                    var dob = (DateTime)value;
                    age = DateTime.Today.Year - dob.Year;
                }
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Illegal characters or {e.Message}");
            }

            if ((age < Min) || (age > Max))
            {
                return new ValidationResult(false,
                  $"Please enter an age in the range: {Min}-{Max}.");
            }
            return ValidationResult.ValidResult;
        }
    }

    public class ContactPrefsNullRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            BindingGroup bindingGroup = (BindingGroup)value;
            var model = (DetailsViewModel)bindingGroup.Items[0];
            
            // validate contactPrefs
            object objPhoneValue = null;
            object objOptOutValue = null;
            bool phoneSet = bindingGroup.TryGetValue(model, "Phone_Ind", out objPhoneValue);
            bool optOutSet = bindingGroup.TryGetValue(model, "Optout_Ind", out objOptOutValue);

            if((objPhoneValue == null || (bool)objPhoneValue == false) 
                &&
                (objOptOutValue == null || (bool)objOptOutValue == false)
                )
            {
                return new ValidationResult(false, "Must choose a Contact Pref");
            }

            return ValidationResult.ValidResult;
        }
    }
}
