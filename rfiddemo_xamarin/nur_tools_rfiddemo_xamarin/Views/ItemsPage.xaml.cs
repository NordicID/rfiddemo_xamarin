using System;
using System.ComponentModel;
using Xamarin.Forms;
using NurApiDotNet;

using nur_tools_rfiddemo_xamarin.Models;
using nur_tools_rfiddemo_xamarin.ViewModels;

namespace nur_tools_rfiddemo_xamarin.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        MainPageViewModel viewModel;
        
        public ItemsPage()
        {
            InitializeComponent();            
            BindingContext = viewModel = new MainPageViewModel();                     
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Item;
            
            if (item == null)
                return;

            if (item.itemMain == MainPageItem.Connection)
            {
                await Navigation.PushAsync(new DeviceSelectPage());
            }
            else if (item.itemMain == MainPageItem.Settings)
            {
                await Navigation.PushAsync(new SettingsPage());
            }
            else if (item.itemMain == MainPageItem.Inventory)
            {
                await Navigation.PushAsync(new InventoryPage());
                          
            }
            else if (item.itemMain == MainPageItem.SensorTag)
            {
                await Navigation.PushAsync(new SensorTagPage());

            }
            else if (item.itemMain == MainPageItem.Barcode)
            {
                //Try get accessory configuration from reader yo find out is there Barcode reader.
                //Exception occurs if device has not accessories or not connected.
                bool sorry = false;
                try
                {
                    
                    AccessoryConfig cfg = App.Nur.AccGetConfig();
                    //There is accessories but is there barcode scanner.
                    if (cfg.hasImagerScanner() == false)
                        sorry = true;                                           
                    else await Navigation.PushAsync(new BarcodePage());
                }
                catch (Exception)
                {
                    sorry = true;
                }

                if(sorry)
                    await DisplayAlert("Sorry","Barcode not available", "OK");

            }
            
            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        /*
        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }
        */

        protected override void OnAppearing()
        {            
            base.OnAppearing();

            //Need in order to show status messages bottom of the screen
            App.BindStatusMessage(MyStatusBar);

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);                        
        }

        protected override void OnDisappearing()
        {                    
            base.OnDisappearing();            
        }

       
        

    }
}