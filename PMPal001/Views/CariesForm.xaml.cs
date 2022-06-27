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
using PMPal.ViewModels;

namespace PMPal
{
    /// <summary>
    /// Interaction logic for CariesForm.xaml
    /// </summary>
    public partial class CariesForm : Page
    {
        CariesFormViewModel model;
        //int FormTotal { get; set; }
        bool loading;
        public CariesForm(string personID = "", string encID = "", string encNBR = "")
        {
            if (personID == "")
                personID = "D7D26392-3CE5-4085-9B72-DC78A28C9A96";
            if (encID == "")
                encID = "227CB003-7057-40CA-89E5-568F3F37919C";

            InitializeComponent();
            model = new CariesFormViewModel(personID, encID, encNBR);
            this.DataContext = model;
            loading = true;
            if (model.cariesForm.FormResults != null)
                UpdateRadioButtons();
            loading = false;
            //DentalReferralViewModel drm = new DentalReferralViewModel(model.Person.person_id, "");
            //drm.GenerateReferral();
        }

        private void UpdateRadioButtons()
        {
            foreach (var keyval in this.model.cariesForm.FormResults)
            {
                if (keyval.Value.HasValue)
                {
                    foreach (var element in grdQuestionForm.Children)
                    {
                        if (element is RadioButton)
                        {
                            var btn = element as RadioButton;
                            if (btn.GroupName.Contains(keyval.Key.ToString()))
                            {
                                int btnValue = Grid.GetColumn(btn) - 2;
                                if(keyval.Value.Value == btnValue)
                                    btn.IsChecked = true;
                            }
                        }
                    }
                }
            }
        }

        private void UpdateQuestionForm()
        {
            model.ResetForm();
            foreach(var element in grdQuestionForm.Children)
            {
                if(element is RadioButton)
                {
                    var btn = element as RadioButton;
                    if (btn.IsChecked.Value == true)
                    {
                        int rowIndex = Grid.GetRow(btn);
                        int colIndex = Grid.GetColumn(btn);
                        int value = colIndex - 2;
                        model.UpdatePatientCariesForm(rowIndex, value);
                    }
                }
            }
            txtTotal.GetBindingExpression(TextBlock.TextProperty)
                      .UpdateTarget();
            //FormTotal = model.FormTotal;

        }
        private void UpdateQuestionForm(string name)
        {
            foreach (var element in grdQuestionForm.Children)
            {
                if (element is RadioButton)
                {
                    var btn = element as RadioButton;
                    if (btn.Name == name)
                    {
                        int QuestionID = int.Parse(btn.GroupName.Substring(7));
                        int colIndex = Grid.GetColumn(btn);
                        int btnValue = colIndex - 2;
                        model.UpdatePatientCariesForm(QuestionID, btnValue);
                        break;
                    }
                }
            }
            txtTotal.GetBindingExpression(TextBlock.TextProperty)
                      .UpdateTarget();

            txtOverallRisk.GetBindingExpression(TextBlock.TextProperty)
                      .UpdateTarget();
            //FormTotal = model.FormTotal;

        }

        private void rb_checkedChanged(object sender, EventArgs e)
        {
            if (loading) return;

            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb.IsChecked == true)
                {
                    UpdateQuestionForm(rb.Name);
                }
            }

        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                model.SubmitForm();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
