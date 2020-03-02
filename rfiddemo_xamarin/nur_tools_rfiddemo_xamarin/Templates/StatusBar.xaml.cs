using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace nur_tools_rfiddemo_xamarin.Templates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusBar : Frame
    {
        StatusMsgItem item;

        public StatusBar()
        {
            InitializeComponent();
            item = new StatusMsgItem();
            BindingContext = item;
        }

        public void SetText(string txt)
        {
            item.Text = txt;
        }

        public void SetTextColor(Color clr)
        {
            item.TextColor = clr;
        }

        public void SetBkColor(Color clr)
        {
            item.BackgroundColor = clr;
        }
    }

    public class StatusMsgItem : INotifyPropertyChanged
    {
        string mStatusText;
        Color mStatusTextColor;
        Color mBackgroundColor;

        public string Text
        {
            get { return mStatusText; }
            set
            {
                if (value != mStatusText)
                {
                    mStatusText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Color TextColor
        {
            get { return mStatusTextColor; }
            set
            {
                if (value != mStatusTextColor)
                {
                    mStatusTextColor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Color BackgroundColor
        {
            get { return mBackgroundColor; }
            set
            {
                if (value != mBackgroundColor)
                {
                    mBackgroundColor = value;
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