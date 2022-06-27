using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PMPal.Models
{
    class ProviderResource: INotifyPropertyChanged
    {
        public ProviderResource(string resourceID, string description)
        {
            ResourceID = resourceID;
            Description = description;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private string _resourceID;
        public string ResourceID
        {
            get { return _resourceID; }
            set { _resourceID = value; }
        }


        private string description;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set { _selected = value;
                NotifyPropertyChanged();
            }
        }

    }
}
