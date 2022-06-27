using PMPal.Backend;
using PMPal.Models;
using PMPal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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

namespace PMPal.Views
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class ProviderSearch : Page
    {
        private ProvidersViewModel _model;

        private const string DialogIdentifier = "diagHost";
        public ProviderSearch()
        {
            InitializeComponent();

            _model = new ProvidersViewModel();
            this.DataContext = _model;
            cmbProviders.ItemsSource = _model.Providers.Values;
            //lstProviders.SelectedValuePath = "Key";
            //lstProviders.DisplayMemberPath = "Value.Description";
            cmbProviders.IsTextSearchEnabled = true;

            lstSelectedProviders.ItemsSource = _model.SelectedProvidersList;
            //((System.Collections.ObjectModel.ObservableCollection<ProviderResource>)lstSelectedProviders.ItemsSource).CollectionChanged += ProviderSearch_CollectionChanged;
        }

        //private void ProviderSearch_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //}

        private void tpFromTime_SelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            if (e != null && e.NewValue.HasValue)
                _model.SetFromTime(e.NewValue.Value);
        }

        private void tpToTime_SelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            if (e != null && e.NewValue.HasValue)
                _model.SetToTime(e.NewValue.Value);

        }

        private void dtFromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtFromDate.SelectedDate != null)
            {
                _model.StartDate = dtFromDate.SelectedDate.Value;
                 
                if(tpFromTime.SelectedTime.HasValue)
                    _model.SetFromTime(tpFromTime.SelectedTime.Value);
                
            }
        }

        private void dtToDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtToDate.SelectedDate != null)
            {
                _model.EndDate = dtToDate.SelectedDate.Value;

                if (tpToTime.SelectedTime.HasValue)
                    _model.SetToTime(tpToTime.SelectedTime.Value);
            }
        }

        private void btnBlockDates_Click(object sender, RoutedEventArgs e)
        {
                TextBlock txt1 = new TextBlock();
                txt1.HorizontalAlignment = HorizontalAlignment.Center;
                txt1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF53B3B"));
                txt1.Margin = new Thickness(4);
                txt1.TextWrapping = TextWrapping.WrapWithOverflow;
                txt1.FontSize = 18;

            if (!DateTime.TryParse(dtFromDate.Text, out _)
                || !DateTime.TryParse(dtToDate.Text, out _)
                )
            {
                txt1.Text = "Please enter Date and Times to Block.";
            }
            else if(_model.SelectedProvidersList.Count == 0)
            {
                txt1.Text = "Please select Provider(s).";
            }
             else if (_model.StartDate >= _model.EndDate || 
                (_model.StartDate == _model.EndDate && _model.StartDate.TimeOfDay >= _model.EndDate.TimeOfDay)
                )
                txt1.Text = "Begin and end times selected are invalid.";
            else
            {
                if(MessageBoxResult.Yes == MessageBox.Show($"Blocking all time slots on {_model.StartDate.ToShortDateString()} to {_model.EndDate.ToShortDateString()} from {_model.StartDate.ToLongTimeString()} until {_model.EndDate.ToLongTimeString()}. Are you sure you wish to continue?", "Confirm", MessageBoxButton.YesNo))
                {
                    _model.BlockDates();
                    txt1.Text = $"Dates Blocked for provider(s): {_model.SelectedProvidersNamesString}";
                    txt1.Foreground = Brushes.Black;
                }
            }

            MaterialDesignThemes.Wpf.DialogHost.Show(txt1, DialogIdentifier);
        }


        private void cmbProviders_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            cmbProviders.Items.Filter = i => ((PMPal.Models.ProviderResource)i).Description.ToLower().Contains(cmbProviders.Text.ToLower());
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

            try
            {
                var selected = ((PMPal.Models.ProviderResource)((CheckBox)sender).DataContext);
                            
                _model.AddProvider(selected);
                //lstSelectedProviders.Items.Refresh();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = ((CheckBox)sender).Content.ToString();

                _model.RemoveSelectedProviderName(selected);
            }
            catch (Exception)
            {
                throw;
            }
        }



        private void cmbProviders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbProviders.SelectedIndex = -1;
        }


        private void Name_Button_Click(object sender, RoutedEventArgs e)
        {
            cmbProviders.IsDropDownOpen = true;

            //_model.RemoveSelectedProviderName(((Button)sender).Content.ToString());
        }

        private void cmbProviders_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cmbProviders.IsDropDownOpen = true;
        }
    }


}
