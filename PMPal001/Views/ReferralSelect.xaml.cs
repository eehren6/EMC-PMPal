using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for ReferralSelect.xaml
    /// </summary>
    public partial class ReferralSelect : Page
    {
        public PatientReferralsViewModel model { get; set; }

        public ReferralSelect(string personID, string encID)
        {
            InitializeComponent();
            model = new PatientReferralsViewModel(personID, encID);
            DataContext = model;

            if (refList.Items.Count > 0)
            {
                refList.SelectedItem = refList.Items[0];
                model.SelectedReferral = ((KeyValuePair<string, Models.PatientReferral>)refList.SelectedItem).Value;
            }
        }

        private void refList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            model.SelectedReferral = ((KeyValuePair<string, Models.PatientReferral>)refList.SelectedItem).Value;
        }

        private void CariesForm(string personID, string encID)
        {
            CariesForm frm = new CariesForm(personID, encID);
            NavigationService?.Navigate(frm);
        }

        private void refList_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            refList.Items.Refresh();
        }
        private void btnGenerateReferral_Click(object sender, RoutedEventArgs e)
        {
            if (model?.SelectedReferral != null)
            {
                var c = this.Cursor;
                try
                {
                    this.Cursor = Cursors.Wait;
                    model.GenerateReferralPDF();
                    this.Cursor = c;
                    MessageBox.Show("Referral PDF generated.");
                    //string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    //string fileName = "\\refdoc" + DateTime.Now.ToString("ddMM-hhmm") + ".pdf";
                    //OpenFolderWithFile(folderPath, fileName);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("There was a problem generating the document.\n\n" + ex.Message);
                }
                finally
                {
                    this.Cursor = c;
                }
            }
        }
        private void OpenFolderWithFile(string folderPath, string fileName)
        {
            if (Directory.Exists(folderPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = "/select, " + folderPath + fileName,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            }
            else
            {
                MessageBox.Show(string.Format("{0} Directory does not exist!", folderPath));
            }
        }
    }
}
