using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace nur_tools_rfiddemo_xamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoPage : ContentPage
    {
        public InfoPage()
        {
            InitializeComponent();
        }

        public void SetUrl(string url)
        {
            Browser.Source = url;
        }

        void onNavigating(object sender, System.EventArgs e)
        {
            LoadingLabel.IsVisible = true;
        }

        void onNavigated(object sender, System.EventArgs e)
        {
            LoadingLabel.IsVisible = false;
        }


        void onBackClicked(object sender, System.EventArgs e)
        {
            if (Browser.CanGoBack)
                Browser.GoBack();
        }
        
    }
}