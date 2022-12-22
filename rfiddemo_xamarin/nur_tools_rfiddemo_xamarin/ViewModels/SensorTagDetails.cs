using NurApiDotNet.SensorTag;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using static NurApiDotNet.NurApi;

namespace nur_tools_rfiddemo_xamarin.Models
{
    public class SensorTagDetails : INotifyPropertyChanged
    {
        private string mCode;
        private string mType;
        private string mValue;
        private string mLastSeen;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SensorTagItem SensorTagItem { get; set; }

        public string Code
        {
            get { return mCode; }
            set
            {
                if (value != mCode)
                {
                    mCode = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SensorType
        {
            get { return mType; }
            set
            {
                if (value != mType)
                {
                    mType = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Value
        {
            get { return mValue; }
            set
            {
                if (value != mValue)
                {
                    mValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Lastseen
        {
            get { return mLastSeen; }
            set
            {
                if (value != mLastSeen)
                {
                    mLastSeen = value;
                    NotifyPropertyChanged();
                }
            }
        }


    }
}
