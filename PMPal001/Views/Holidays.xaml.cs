using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using PMPal.ViewModels;
using PMPal.Models;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PMPal.Views
{
    /// <summary>
    /// Interaction logic for Holidays.xaml
    /// </summary>
    public partial class Holidays : Page
    {
        HolidayScheduleViewModel model;
        
        public Holidays()
        {
            InitializeComponent();

            model = this.DataContext as HolidayScheduleViewModel;

            //var binding = new Binding("SchedulesList");
            //binding.NotifyOnSourceUpdated=tr;
            //grdSchedules.ItemsSource = model.SchedulesList;
            grdSchedules.SourceUpdated += GrdSchedules_SourceUpdated;
            
        }

        private void GrdSchedules_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            grdSchedules.Items.Refresh();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            grdEditFields.Visibility = Visibility.Visible;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            grdEditFields.Visibility = Visibility.Visible;
            txtDescription.Text = model.SelectedSchedule.Description;
            dtStartDate.SelectedDate = model.SelectedSchedule.FromDate;
            dtEndDate.SelectedDate = model.SelectedSchedule.ToDate;
            tpFromTime.SelectedTime = model.SelectedSchedule.FromDate;
            tpToTime.SelectedTime = model.SelectedSchedule.ToDate;
        }


        private void ClearFields()
        {
            
            txtDescription.Text = "";
            dtEndDate.SelectedDate = null;
            dtStartDate.SelectedDate = null;
            grdEditFields.Visibility = Visibility.Hidden;
        }
        private void grdSchedules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (grdSchedules.SelectedIndex >= 0 && grdSchedules.SelectedIndex < model.SchedulesList.Count)
                model.SelectedSchedule = model.SchedulesList[grdSchedules.SelectedIndex];
            else
                model.SelectedSchedule = null;
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            string user = Util.GetActiveUser();
            if (user == "" || user == null )
            {
                MessageBox.Show("No active user identified. Try reloading PM.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var schedule = new Models.HolidaySchedule();
            schedule.Description = txtDescription.Text;
            if (dtStartDate.SelectedDate.HasValue)
                schedule.FromDate = dtStartDate.SelectedDate.Value;
            if (dtEndDate.SelectedDate.HasValue)
                schedule.ToDate = dtEndDate.SelectedDate.Value;
            if (tpFromTime.SelectedTime.HasValue)
                schedule.FromDate = schedule.FromDate.Date + tpFromTime.SelectedTime.Value.TimeOfDay;
            if (tpToTime.SelectedTime.HasValue)
                schedule.ToDate = schedule.ToDate.Date + tpToTime.SelectedTime.Value.TimeOfDay;

            if (!model.ValidateFields(schedule))
            {
                MessageBox.Show("Some fields are missing or selected dates are invalid.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
            else
            {
                bool isUpdate = (model.SelectedSchedule != null);
                bool blDone = false;
                string message = "";
                if (schedule.ToDate >= DateTime.Now)
                    message = $"This will affect all resource templates from {schedule.FromDate.ToString()} to {schedule.ToDate.ToString()}.\n\nAre you sure you wish to continue?";
                else
                    message = $"Be aware that the dates you have selected ({schedule.FromDate.ToString()} to {schedule.ToDate.ToString()}) are in the past and will affect previously applied resource templates.\n\nAre you sure you wish to continue?";

                if (MessageBox.Show(message, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (isUpdate)
                    {
                        schedule.ID = model.SelectedSchedule.ID;
                        model.SelectedSchedule = schedule;
                        blDone = model.UpdateSchedule();
                    }
                    else
                    {
                        model.SelectedSchedule = schedule;
                        blDone = model.InsertSchedule();
                    }
                }

                if(blDone)
                    ClearFields();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (model.SelectedSchedule != null && MessageBox.Show("Are you sure you want to delete this schedule?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                model.Removeschedule();

                //MessageBox.Show("Schedule Deleted");
            }
        }
        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to cancel?","Warning",MessageBoxButton.YesNo, MessageBoxImage.Warning)==MessageBoxResult.Yes)
                ClearFields();
        }

        //private void tpFromTime_SelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        //{
        //    if(tpFromTime.SelectedTime.HasValue)
        //        model.SetFromTime(tpFromTime.SelectedTime.Value);
        //}

        //private void tpToTime_SelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        //{
        //    if(tpToTime.SelectedTime.HasValue)
        //        model.SetToTime(tpToTime.SelectedTime.Value);
        //}
    }
}
