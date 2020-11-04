using nur_tools_rfiddemo_xamarin.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using nur_tools_rfiddemo_xamarin.Models;
using NurApiDotNet;

namespace nur_tools_rfiddemo_xamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        SettingsPageViewModel viewModel;

        public SettingsPage()
        {
            InitializeComponent();            

            BindingContext = viewModel = new SettingsPageViewModel();                                   
        }

        async void OnSettingsItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Item;

            if (item == null)
                return;

            if (!App.Nur.IsConnected())
            {
                await DisplayAlert("No reader connection", "Connect to reader first. ", "OK");
                return;
            }

            if (item.itemSettings == SettingsPageItem.RFID)
            {
                await Navigation.PushAsync(new SettingsRFID());
            }
            else if (item.itemSettings == SettingsPageItem.Antenna)
            {
                await Navigation.PushAsync(new SettingsPages.SettingsAntenna());
            }
            else if (item.itemSettings == SettingsPageItem.Inventory)
            {
                await Navigation.PushAsync(new SettingsPages.SettingsInventory());
            }
            else if (item.itemSettings == SettingsPageItem.InventoryRead)
            {
                await Navigation.PushAsync(new SettingsPages.SettingsInventroyRead());
            }
            else if (item.itemSettings == SettingsPageItem.InventoryEx)
            {
                await Navigation.PushAsync(new SettingsPages.SettingsInvExt());
            }
            else if (item.itemSettings == SettingsPageItem.GPIO)
            {
                if (App.Nur.Capabilites.maxGPIO > 0)
                    await Navigation.PushAsync(new SettingsPages.SettingsGPIO());
                else await DisplayAlert("Sorry", "No GPIO available for this device", "OK");
            }
            else if(item.itemSettings == SettingsPageItem.Export)
            {                
                await Navigation.PushAsync(new SettingsPages.SettingsExport());
            }
            else if (item.itemSettings == SettingsPageItem.Reader)
            {

            }
            else if (item.itemSettings == SettingsPageItem.Sensors)
            {

            }
            else if (item.itemSettings == SettingsPageItem.Barcode)
            {

            }
            else if (item.itemSettings == SettingsPageItem.StoreSetup)
            {
                try
                {
                    App.Nur.StoreCurrentSetup(NurApi.STORE_ALL);
                    App.ShowShortStatusMessage("Settings stored successfully", 3, Color.White, Color.Green);

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Operation failed!", ex.Message, "OK");
                }
            }
            else if (item.itemSettings == SettingsPageItem.FactoryDefaults)
            {
                Device.BeginInvokeOnMainThread(async () => {
                    var result = await DisplayAlert("Restoring factory defaults", "Are you sure?", "Yes", "No");
                    if (result)
                    {
                        try
                        {
                            App.Nur.FactoryReset(0x994F6B32);
                            App.ShowShortStatusMessage("Restoring factory defaults..", 3, Color.White, Color.Green);
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Operation failed!", ex.Message, "OK");
                        }
                    }
                });                
            }

                // Manually deselect item.
                ItemsListView.SelectedItem = null;
        }
        
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
            Debug.WriteLine(" SettingsPage OnDisappearing");           
            base.OnDisappearing();
        }
       
    }
}