using PMPal.Backend;
using PMPal.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PMPal.ViewModels
{
    class HolidayScheduleViewModel : ValidationBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<HolidaySchedule> SchedulesList
        {
            get { return HolidaySchedules.HolidaySchedulesDict; }
        }

        public HolidaySchedule SelectedSchedule { get; set; }

        public bool InsertSchedule()
        {
            if (ValidateFields())
            {
                HolidaySchedules.InsertSchedule(SelectedSchedule.Description, SelectedSchedule.FromDate, SelectedSchedule.ToDate);
                NotifyPropertyChanged("SchedulesList");
                return true;
            }
            return false;
        }

        public bool UpdateSchedule()
        {
            if (ValidateFields())
            {
                HolidaySchedules.UpdateSchedule(SelectedSchedule.ID, SelectedSchedule.Description, SelectedSchedule.FromDate, SelectedSchedule.ToDate);
                NotifyPropertyChanged("SchedulesList");
                return true;
            }
            return false;
        }

        public bool Removeschedule()
        {
            if (SelectedSchedule.ID != "")
            {
                HolidaySchedules.RemoveSchedule(SelectedSchedule.ID);
                NotifyPropertyChanged("SchedulesList");
                return true;
            }
            return false;
        }

        //public void SetFromTime(DateTime newTime)
        //{
        //    SelectedSchedule.FromDate = SelectedSchedule.FromDate.Date + newTime.TimeOfDay;
        //}

        //public void SetToTime(DateTime newTime)
        //{
        //    SelectedSchedule.ToDate = SelectedSchedule.ToDate.Date + newTime.TimeOfDay;
        //}
        public bool ValidateFields(HolidaySchedule schedule = null)
        {
            if (schedule == null) schedule = SelectedSchedule;

            if (schedule.FromDate == DateTime.MinValue
                || schedule.ToDate == DateTime.MinValue
                || schedule.FromDate > schedule.ToDate)
                return false;

            if (schedule.Description == "")
                return false;

            return true;

        }
    }
}
