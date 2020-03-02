using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using NurApiDotNet;

using Xamarin.Forms;

using nur_tools_rfiddemo_xamarin.Models;
using nur_tools_rfiddemo_xamarin.Views;

namespace nur_tools_rfiddemo_xamarin.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public MainPageViewModel()
        {
            Title = "Nordic ID RFID Demo";
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
                Items.Add(new Item { itemMain = MainPageItem.Connection, Text = "Connection", Description = "Reader connection", ImageName= "connection2.png" });               
                Items.Add(new Item { itemMain = MainPageItem.Settings, Text = "Settings", Description = "Reader settings", ImageName = "ic_settings.png" });
                Items.Add(new Item { itemMain = MainPageItem.Inventory, Text = "Inventory", Description = "Read RFID tags", ImageName = "ic_inventory.png" });
                Items.Add(new Item { itemMain = MainPageItem.SensorTag, Text = "SensorTag", Description = "Read sensor tag (Temperature, Moisture)", ImageName = "ic_inventory.png" });
                //Items.Add(new Item { itemMain = MainPageItem.Write, Text = "Write", Description = "Write to tag", ImageName = "ic_write.png" });
                Items.Add(new Item { itemMain = MainPageItem.Barcode, Text = "Barcode", Description = "Read Barcode", ImageName = "ic_barcode.png" });                

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