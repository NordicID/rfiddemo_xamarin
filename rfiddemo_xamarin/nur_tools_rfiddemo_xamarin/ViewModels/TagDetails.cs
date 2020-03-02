using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace nur_tools_rfiddemo_xamarin.Models
{
    public class TagDetails: INotifyPropertyChanged
    {
        private string mEPCKey;
        private string mEPC;
        private Color mEPCColor;
        private string mAntenna;
        private string mRSSI;
        private double mRssiScaled;
        private string mData;

        
        public string RSSI
        {
            get { return mRSSI; }
            set
            {
                if (value != mRSSI)
                {
                    mRSSI = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double RSSIScaled
        {
            get { return mRssiScaled; }
            set
            {
                if (value != mRssiScaled)
                {
                    mRssiScaled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Antenna
        {
            get { return mAntenna; }
            set
            {
                if (value != mAntenna)
                {
                    mAntenna = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string EPCKey
        {
            get { return mEPCKey; }
            set
            {
                if (value != mEPCKey)
                {
                    mEPCKey = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string EPC
        {
            get { return mEPC; }
            set
            {
                if (value != mEPC)
                {
                    mEPC = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Color EPCColor
        {
            get { return mEPCColor; }
            set
            {
                if (value != mEPCColor)
                {
                    mEPCColor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string DATA
        {
            get { return mData; }
            set
            {
                if (value != mData)
                {
                    mData = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
