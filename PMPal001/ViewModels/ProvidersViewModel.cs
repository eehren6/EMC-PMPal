using PMPal;
using PMPal.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Windows;

namespace PMPal.ViewModels
{
    class ProvidersViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Dictionary<string, ProviderResource> _providers;
        public Dictionary<string, ProviderResource> Providers
        {
            get
            {
                if (_providers == null)
                    _providers = PMPal.Backend.Repository.ProvidersRepo.GetProviderResources(FilterText).ToDictionary(k => k.Key, k => new ProviderResource(k.Value.ResourceID, k.Value.Description));
                return _providers; 
            }
        }

        private ProviderResource _selectedProvider;

        public ProviderResource SelectedProvider
        {
            get { return _selectedProvider; }
            set { _selectedProvider = value; }
        }
        private ObservableCollection<ProviderResource> _selectedProvidersList;

        public ObservableCollection<ProviderResource> SelectedProvidersList
        {
            get
            {
                if (_selectedProvidersList == null)
                    _selectedProvidersList = new ObservableCollection<ProviderResource>();
                return _selectedProvidersList;
            }
            set
            {
                _selectedProvidersList = value;
            }

        }

        public Dictionary<string,string> ScheduleCategories
        {
            get
            {
                var categories = new Dictionary<string, string>();
                categories.Add(" Out", "CE8A2A95-02EC-414C-BB1E-1BFBA42F2552");
                categories.Add("Unscheduled", "A03BE608-1D25-45F6-8B35-24BCCE8BEA57");

                return categories;
            }
        }

        private string _selectedCategory;
        public string SelectedCategory
        {
            get
            {
                if (_selectedCategory == null)
                    _selectedCategory = ScheduleCategories.FirstOrDefault().Value;

                return _selectedCategory;
            }
            set
            {
                _selectedCategory = value;
            }
        }
        public void AddProvider(ProviderResource p)
        {
            SelectedProvidersList.Add(p);
            NotifyPropertyChanged("SelectedProvidersList");
        }

        public void RemoveSelectedProviderName(string name)
        {
            var prov = SelectedProvidersList.Where(x => x.Description == name).FirstOrDefault();
            SelectedProvidersList.Remove(prov);
            Providers[prov.ResourceID].Selected = false;
            NotifyPropertyChanged("Selected");
            NotifyPropertyChanged("SelectedProvidersList");
        }
        public string SelectedProvidersString
        {
            get
            {
                return SelectedProvidersList == null ? "" : string.Join(',', values: SelectedProvidersList.Select(x => x.ResourceID));
            }
        }

        public string SelectedProvidersNamesString
        {
            get
            {
                return SelectedProvidersList == null ? "" : string.Join(", ", values: SelectedProvidersList.Select(x => x.Description));
            }
        }
        public string FilterText { get; set; }

        private DateTime _startDate;
        public DateTime StartDate { 
            get
            {
                if (_startDate == DateTime.MinValue)
                    _startDate = DateTime.Today;
                return _startDate;
            }
            set
            {
                _startDate = value;
                NotifyPropertyChanged("StartDate");
            }
        }

        private DateTime _enddate;
        public DateTime EndDate {
            get {

                if (_enddate == DateTime.MinValue)
                    _enddate = DateTime.MaxValue;
                return _enddate;
            }
            set
            {
                _enddate = value;
                NotifyPropertyChanged("EndDate");
            }
        }

        public void SetFromTime(DateTime newTime)
        {
            StartDate = StartDate.Date + newTime.TimeOfDay;
        }

        public void SetToTime(DateTime newTime)
        {
            EndDate = EndDate.Date + newTime.TimeOfDay;
        }
        public string UserID { get; set; }

        public void BlockDates()
        {
            try
            {
                PMPal.Backend.Providers.BlockDates(SelectedProvidersString, StartDate, EndDate, UserID, SelectedCategory);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }


}
