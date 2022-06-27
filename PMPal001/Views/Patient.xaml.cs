using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PatientScreen : Window
    {
        public PatientScreen()
        {

        }

        //public Patient LoadPatient(int person_id)
        //{
        //    Patient pat = new Patient();
        //    try
        //    {
        //        var pat_record = Util.eventData.Select("person_id = " + person_id).FirstOrDefault();

        //        pat.person_id = pat_record["person_id"].ToString();
        //        pat.person_nbr = pat_record["person_nbr"].ToString();
        //        pat.First_name = pat_record["first_name"].ToString();
        //        pat.Last_name = pat_record["last_name"].ToString();
        //        pat.Middle_name = pat_record["middle_name"].ToString();
        //        pat.Address_line_1 = pat_record["address_line_1"].ToString();
        //        pat.Address_line_2 = pat_record["address_line_2"].ToString();
        //        pat.City = pat_record["city"].ToString();
        //        pat.State = pat_record["state"].ToString();
        //        pat.Zip = pat_record["zip"].ToString();
        //        pat.Home_phone = pat_record["home_phone"].ToString();

        //        return pat;
        //    }
        //    catch(Exception ex)
        //    {
        //        var error = "patient not found";

        //        return pat;
        //    }

        //}

    }
}
