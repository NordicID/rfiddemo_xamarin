using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using nur_tools_rfiddemo_xamarin.Templates;
using NurApiDotNet;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;
using Xamarin.Essentials;

namespace nur_tools_rfiddemo_xamarin.Views.SettingsPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsInventory : ContentPage
    {
        ObservableCollection<ListItem> itemList = new ObservableCollection<ListItem>();
        
        public SettingsInventory()
        {
            InitializeComponent();
        }
        
        enum ItemID
        {
            Q,
            Rounds,
            Session,
            Target,
            EPCLength,
            FilterHdr,
            FltReadMin,
            FltReadMax,
            FltWriteMin,
            FltWriteMax,
            FltInvMin,
            FltInvMax
        }

        private async void PrepareListItems()
        {
            string txt;

            if (App.Nur.IsConnected())
            {
                itemList.Clear();
                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        ListItemStyle style = new ListItemStyle("ic_settings_black", 10, Color.White, Color.Black, Color.Blue);                      

                        if (App.Nur.Setup.inventoryQ == 0) txt = "Auto";
                        else txt = App.Nur.Setup.inventoryQ.ToString();
                        itemList.Add(new ListItem(style, "Q", txt, ItemID.Q));

                        if (App.Nur.Setup.inventoryRounds == 0) txt = "Auto";
                        else txt = App.Nur.Setup.inventoryRounds.ToString();
                        itemList.Add(new ListItem(style, "Rounds", txt, ItemID.Rounds));
                        itemList.Add(new ListItem(style, "Session", App.Nur.Setup.inventorySession.ToString(), ItemID.Session));

                        InventoryTarget target = (InventoryTarget)App.Nur.Setup.inventoryTarget;
                        itemList.Add(new ListItem(style, "Target", target.ToString(), ItemID.Target));

                        itemList.Add(new ListItem(style, "EPC length", App.Nur.Setup.inventoryEpcLength.ToString(), ItemID.EPCLength));

                        //Filters

                        ListItemStyle filterStyle = new ListItemStyle("ic_info_blue", 25, Color.FromRgb(230,255,255), Color.Black, Color.Blue);
                        filterStyle.styleHdrFontAttribute = FontAttributes.Bold;
                        filterStyle.styleHdrFontSize = 18;
                        itemList.Add(new ListItem(filterStyle, "Rssi Filters", "",ItemID.FilterHdr));

                        filterStyle.styleCellHeight = 35;
                        filterStyle.styleBkColor = Color.White;
                        filterStyle.styleHdrFontAttribute = FontAttributes.None;
                        filterStyle.styleHdrFontSize = 16;
                        filterStyle.styleImageSource = "ic_filter.png";

                        if (App.Nur.Setup.readRssiFilter.min == 0)
                        {
                            txt = "Disabled";
                            filterStyle.styleValueColor = Color.Red;
                        }
                        else
                        {
                            txt = App.Nur.Setup.readRssiFilter.min.ToString() + " dBm";
                        }

                        itemList.Add(new ListItem(filterStyle, "Read Min", txt, ItemID.FltReadMin));

                        if (App.Nur.Setup.readRssiFilter.max == 0)
                        {
                            txt = "Disabled";
                            filterStyle.styleValueColor = Color.Red;
                        }
                        else
                        {
                            txt = App.Nur.Setup.readRssiFilter.max.ToString() + " dBm";
                            filterStyle.styleValueColor = Color.Blue;
                        }
                        itemList.Add(new ListItem(filterStyle, "Read Max", txt, ItemID.FltReadMax));

                        if (App.Nur.Setup.writeRssiFilter.min == 0)
                        {
                            txt = "Disabled";
                            filterStyle.styleValueColor = Color.Red;
                        }
                        else
                        {
                            txt = App.Nur.Setup.writeRssiFilter.min.ToString() + " dBm";
                            filterStyle.styleValueColor = Color.Blue;
                        }
                        itemList.Add(new ListItem(filterStyle, "Write Min", txt, ItemID.FltWriteMin));

                        if (App.Nur.Setup.writeRssiFilter.max == 0)
                        {
                            txt = "Disabled";
                            filterStyle.styleValueColor = Color.Red;
                        }
                        else
                        {
                            txt = App.Nur.Setup.writeRssiFilter.max.ToString() + " dBm";
                            filterStyle.styleValueColor = Color.Blue;
                        }
                        itemList.Add(new ListItem(filterStyle, "Write Max", txt, ItemID.FltWriteMax));

                        if (App.Nur.Setup.inventoryRssiFilter.min == 0)
                        {
                            txt = "Disabled";
                            filterStyle.styleValueColor = Color.Red;
                        }
                        else
                        {
                            txt = App.Nur.Setup.inventoryRssiFilter.min.ToString() + " dBm";
                            filterStyle.styleValueColor = Color.Blue;
                        }
                        itemList.Add(new ListItem(filterStyle, "Inventory Min", txt, ItemID.FltInvMin));

                        if (App.Nur.Setup.inventoryRssiFilter.max == 0)
                        {
                            txt = "Disabled";
                            filterStyle.styleValueColor = Color.Red;
                        }
                        else
                        {
                            txt = App.Nur.Setup.inventoryRssiFilter.max.ToString() + " dBm";
                            filterStyle.styleValueColor = Color.Blue;
                        }
                        itemList.Add(new ListItem(filterStyle, "Inventory Max", txt, ItemID.FltInvMax));
                    }
                    catch (Exception e)
                    {
                        await DisplayAlert("Cannot read setup!", e.Message, "OK");
                    }
                });
            }            
            else
                await DisplayAlert("No reader connection", "Connect to reader first. ", "OK");

        }

        private async void HandleQ(ListItem item)
        {
            string[] arr = { "Auto", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set inventory Q", "Cancel", null, arr);
            
            int levelIndex = dict.GetItemIndex(action);           
            if (levelIndex == -1) return; //Possible Cancel pressed  

            try
            {
                App.Nur.InventoryQ = levelIndex;
                item.ItemValueText = action;                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }

           
        }
        private async void HandleRounds(ListItem item)
        {
            string[] arr = { "Auto", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set inventory rounds", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed  

            try
            {
                App.Nur.InventoryRounds = levelIndex;
                item.ItemValueText = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void HandleSession(ListItem item)
        {
            string[] arr = { "S0", "S1", "S2", "S3"};
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set inventory session", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                App.Nur.InventorySession = levelIndex;
                item.ItemValueText = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void HandleTarget(ListItem item)
        {
            string[] arr = { "A", "B", "AB" };
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set inventory target", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                App.Nur.InventoryTarget = levelIndex;
                item.ItemValueText = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void HandleEPCLength(ListItem item)
        {           
            int val;
            
            string result = await DisplayPromptAsync("EPC length filter in bytes (2-255)", "255 = disabled","OK","Cancel", App.Nur.Setup.inventoryEpcLength.ToString(), maxLength: 3, keyboard: Keyboard.Numeric, App.Nur.Setup.inventoryEpcLength.ToString());

            try
            {
                val= Utils.ConvertAndValidate(result, 2, 255);
            }
            catch(Exception ex)
            {
                if(ex.Message.StartsWith("Cancel")) return;
                await DisplayAlert("Invalid value", ex.Message, "OK");
                return;
            }                        
                        
            try
            {
                App.Nur.InventoryEpcLength = val;
                item.ItemValueText = val.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        private void ShowFilterItem(ListItem item, int val)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (val == 0)
                {
                    item.ItemValueColor = Color.Red;
                    item.ItemValueText = "Disabled";
                }
                else
                {
                    item.ItemValueColor = Color.Blue;
                    item.ItemValueText = val.ToString() + " dBm";
                }
            });
        }

        private async void HandleRssiFilterReadMin(ListItem item) 
        {
            int val;

            string result = await DisplayPromptAsync("RSSI filter min (Read)", "(-90 dBm - 0 dBm) 0 = disabled", "OK", "Cancel", App.Nur.Setup.readRssiFilter.min.ToString(), maxLength: 3, keyboard: Keyboard.Numeric, App.Nur.Setup.readRssiFilter.min.ToString());

            try
            {
                val = Utils.ConvertAndValidate(result, -90, 0);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Cancel")) return;
                await DisplayAlert("Invalid value", ex.Message, "OK");
                return;
            }

            try
            {
                App.Nur.Setup.readRssiFilter.min = val; //Set min to current setup
                App.Nur.ReadRssiFilter = App.Nur.Setup.readRssiFilter; //and Write it to module
                ShowFilterItem(item, val);                           
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void HandleRssiFilterReadMax(ListItem item) 
        {
            int val;

            string result = await DisplayPromptAsync("RSSI filter max (Read)", "(-90 dBm - 0 dBm) 0 = disabled", "OK", "Cancel", App.Nur.Setup.readRssiFilter.max.ToString(), maxLength: 3, keyboard: Keyboard.Numeric, App.Nur.Setup.readRssiFilter.max.ToString());

            try
            {
                val = Utils.ConvertAndValidate(result, -90, 0);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Cancel")) return;
                await DisplayAlert("Invalid value", ex.Message, "OK");
                return;
            }

            try
            {
                App.Nur.Setup.readRssiFilter.max = val;
                App.Nur.ReadRssiFilter = App.Nur.Setup.readRssiFilter; //and Write it to module
                ShowFilterItem(item, val);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void HandleRssiFilterWriteMin(ListItem item) 
        {
            int val;

            string result = await DisplayPromptAsync("RSSI filter min (Write)", "(-90 dBm - 0 dBm) 0 = disabled", "OK", "Cancel", App.Nur.Setup.writeRssiFilter.min.ToString(), maxLength: 3, keyboard: Keyboard.Numeric, App.Nur.Setup.writeRssiFilter.min.ToString());

            try
            {
                val = Utils.ConvertAndValidate(result, -90, 0);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Cancel")) return;
                await DisplayAlert("Invalid value", ex.Message, "OK");
                return;
            }

            try
            {
                App.Nur.Setup.writeRssiFilter.min = val;
                App.Nur.WriteRssiFilter = App.Nur.Setup.writeRssiFilter; //and Write it to module
                ShowFilterItem(item, val);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void HandleRssiFilterWriteMax(ListItem item) 
        {
            int val;

            string result = await DisplayPromptAsync("RSSI filter max (Write)", "(-90 dBm - 0 dBm) 0 = disabled", "OK", "Cancel", App.Nur.Setup.writeRssiFilter.max.ToString(), maxLength: 3, keyboard: Keyboard.Numeric, App.Nur.Setup.writeRssiFilter.max.ToString());

            try
            {
                val = Utils.ConvertAndValidate(result, -90, 0);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Cancel")) return;
                await DisplayAlert("Invalid value", ex.Message, "OK");
                return;
            }

            try
            {
                App.Nur.Setup.writeRssiFilter.max = val;
                App.Nur.WriteRssiFilter = App.Nur.Setup.writeRssiFilter; //and Write it to module
                ShowFilterItem(item, val);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void HandleRssiFilterInventoryMin(ListItem item)
        {
            int val;

            string result = await DisplayPromptAsync("RSSI filter min (Inventory)", "(-90 dBm - 0 dBm) 0 = disabled", "OK", "Cancel", App.Nur.Setup.inventoryRssiFilter.min.ToString(), maxLength: 3, keyboard: Keyboard.Numeric, App.Nur.Setup.inventoryRssiFilter.min.ToString());

            try
            {
                val = Utils.ConvertAndValidate(result, -90, 0);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Cancel")) return;
                await DisplayAlert("Invalid value", ex.Message, "OK");
                return;
            }

            try
            {
                App.Nur.Setup.inventoryRssiFilter.min = val;
                App.Nur.InventoryRssiFilter = App.Nur.Setup.inventoryRssiFilter; //and Write it to module
                ShowFilterItem(item, val);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void HandleRssiFilterInventoryMax(ListItem item)
        {
            int val;

            string result = await DisplayPromptAsync("RSSI filter max (Inventory)", "(-90 dBm - 0 dBm) 0 = disabled", "OK", "Cancel", App.Nur.Setup.inventoryRssiFilter.max.ToString(), maxLength: 3, keyboard: Keyboard.Numeric, App.Nur.Setup.inventoryRssiFilter.max.ToString());

            try
            {
                val = Utils.ConvertAndValidate(result, -90, 0);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Cancel")) return;
                await DisplayAlert("Invalid value", ex.Message, "OK");
                return;
            }

            try
            {
                App.Nur.Setup.inventoryRssiFilter.max = val;
                App.Nur.InventoryRssiFilter = App.Nur.Setup.inventoryRssiFilter; //and Write it to module
                ShowFilterItem(item, val);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        

        protected override void OnAppearing()
        {          
            base.OnAppearing();
                       
            PrepareListItems();
            //This event needed when user click item
            MySettingsList.OnItemSelected += MySettingsList_OnItemSelected;
           
            //Assing item list to ListTemplate
            MySettingsList.SetItemsSource(itemList);            
        }

        private async void MySettingsList_OnItemSelected(object sender, EventArgs e)
        {
            //User tapped item. Selection not shown. If wanted to show selection, it must be done manually like changing Bkcolor of item.
            ItemTappedEventArgs arg = (ItemTappedEventArgs)e;
            ListItem item = (ListItem)arg.Item;

            if (item.MyObject != null)
            {
                ItemID id = (ItemID)item.MyObject;

                switch (id)
                {
                    case ItemID.Q:
                        HandleQ(item);
                        break;
                    case ItemID.Rounds:
                        HandleRounds(item);
                        break;
                    case ItemID.Session:
                        HandleSession(item);
                        break;
                    case ItemID.Target:
                        HandleTarget(item);
                        break;
                    case ItemID.EPCLength:
                        HandleEPCLength(item);
                        break;
                    case ItemID.FilterHdr:
                        InfoPage info = new InfoPage();
                        info.SetUrl("https://www.nordicid.com/resources/expert-article/applying-rssi-filters-for-optimal-rfid-performance/");
                        await Navigation.PushAsync(info);
                        break;
                    case ItemID.FltReadMin:
                        HandleRssiFilterReadMin(item);
                        break;
                    case ItemID.FltReadMax:
                        HandleRssiFilterReadMax(item);
                        break;
                    case ItemID.FltWriteMin:
                        HandleRssiFilterWriteMin(item);
                        break;
                    case ItemID.FltWriteMax:
                        HandleRssiFilterWriteMax(item);
                        break;
                    case ItemID.FltInvMin:
                        HandleRssiFilterInventoryMin(item);
                        break;
                    case ItemID.FltInvMax:
                        HandleRssiFilterInventoryMax(item);
                        break;
                }
            }
        }
               
        protected override void OnDisappearing()
        {
            MySettingsList.OnItemSelected -= MySettingsList_OnItemSelected;           
            base.OnDisappearing();
        }
    }
}