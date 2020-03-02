using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NurApiDotNet;


using nur_tools_rfiddemo_xamarin.Models;

namespace nur_tools_rfiddemo_xamarin.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {               
        public MainPage()
        {
            InitializeComponent();
                        
            MasterBehavior = MasterBehavior.Popover;                       
        }

        public async void NavigateFromMenu(int id)
        {
            Console.WriteLine("NavigateFromMenu=" + id.ToString());

            switch (id)
            {                
                case (int)MenuItemType.About:
                    PushPage(new AboutPage());
                    break;
                case (int)MenuItemType.Updates:
                    await DisplayAlert("Sorry","Not implemented!", "OK");
                    break;
            }

        }

        public async void PushPage(ContentPage page)
        {
            await Detail.Navigation.PushAsync(page);

            if (Device.RuntimePlatform == Device.Android)
                await Task.Delay(100);

            IsPresented = false;
        }
    }
}