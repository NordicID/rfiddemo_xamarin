using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;
using nur_tools_rfiddemo_xamarin.Models;

namespace nur_tools_rfiddemo_xamarin.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {                
        public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public SettingsPageViewModel()
        {
            Title = "Settings";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(() => ExecuteLoadItemsCommand());
        }

        void ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                
                Items.Add(new Item { itemSettings = SettingsPageItem.RFID, Text = "RFID", Description = "RFID Link settings", ImageName = "ic_settings.png" });
                Items.Add(new Item { itemSettings = SettingsPageItem.Inventory, Text = "Inventory", Description = "Inventory settings", ImageName = "ic_settings.png" });
                Items.Add(new Item { itemSettings = SettingsPageItem.StoreSetup, Text = "Save RFID- and Inventory settings", Description = "Saved to non-volatile memory of module", ImageName = "saveToModule.png" });
                Items.Add(new Item { itemSettings = SettingsPageItem.InventoryRead, Text = "Inventory Read", Description = "Inventory Read settings", ImageName = "ic_settings.png" });
                Items.Add(new Item { itemSettings = SettingsPageItem.InventoryEx, Text = "InventoryEx", Description = "Inventory Ex settings", ImageName = "ic_settings.png" });                
                Items.Add(new Item { itemSettings = SettingsPageItem.Antenna, Text = "Antenna", Description = "Antenna setup", ImageName = "ic_settings.png" });                
                //Items.Add(new Item { itemSettings = SettingsPageItem.Barcode, Text = "Barcode", Description = "Imager test/configuration", ImageName = "ic_settings.png" });
                //Items.Add(new Item { itemSettings = SettingsPageItem.Reader, Text = "Reader", Description = "General reader specific settings", ImageName = "ic_settings.png" });
                //Items.Add(new Item { itemSettings = SettingsPageItem.Sensors, Text = "Sensors", Description = "Sensor settings", ImageName = "ic_settings.png" });

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

