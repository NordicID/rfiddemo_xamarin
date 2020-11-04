using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using nur_tools_rfiddemo_xamarin.Templates;
using NurApiDotNet;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace nur_tools_rfiddemo_xamarin.Views.SettingsPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsInventroyRead : ContentPage
    {
        ObservableCollection<ListItem> itemList = new ObservableCollection<ListItem>();
                
        public SettingsInventroyRead()
        {
            InitializeComponent();            
        }

        enum ItemID
        {
            Bank,
            Type,
            StartAddress,
            WordCount            
        }

        private void PrepareListItems()
        {           
            itemList.Clear();

            Device.BeginInvokeOnMainThread(async () =>
            {
                ListItemStyle style = new ListItemStyle("ic_settings_black", 20, Color.White, Color.Black, Color.Blue);

                Bank bank = (Bank)App.InvReadParams.bank;
                itemList.Add(new ListItem(style, "Bank", bank.ToString(), ItemID.Bank));

                InventoryReadType itype = (InventoryReadType)App.InvReadParams.type;
                itemList.Add(new ListItem(style, "Type", itype.ToString(), ItemID.Type));

                itemList.Add(new ListItem(style, "Start address", App.InvReadParams.wAddress.ToString(), ItemID.StartAddress));
                itemList.Add(new ListItem(style, "Word count", App.InvReadParams.wLength.ToString(), ItemID.WordCount));
            });
        }

        async void HandleBankSelection(ListItem item)
        {
            string[] arr = Enum.GetNames(typeof(Bank));
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set Bank", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {                
                Array bankValues = Enum.GetValues(typeof(Bank));
                Bank b = (Bank)bankValues.GetValue(levelIndex);                
                App.InvReadParams.bank = (uint)b;               
                item.ItemValueText = b.ToString();               
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void HandleTypeSelection(ListItem item)
        {
            string[] arr = Enum.GetNames(typeof(InventoryReadType));
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set Inventory Type", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                Array values = Enum.GetValues(typeof(InventoryReadType));
                InventoryReadType type = (InventoryReadType)values.GetValue(levelIndex);
                App.InvReadParams.type = (uint)type;               
                item.ItemValueText = type.ToString();                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void HandleAddrSelection(ListItem item)
        {            
            string result = await DisplayPromptAsync("Set start address", "(word)", "OK", "Cancel", App.InvReadParams.wAddress.ToString(), maxLength: 2, keyboard: Keyboard.Numeric, App.InvReadParams.wAddress.ToString());

            //TODO: Validating

            try
            {
                App.InvReadParams.wAddress = Convert.ToUInt32(result);               
                item.ItemValueText = App.InvReadParams.wAddress.ToString();                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void HandleWordCountSelection(ListItem item)
        {
            string result = await DisplayPromptAsync("Set word count", "Num of words to read", "OK", "Cancel", App.InvReadParams.wLength.ToString(), maxLength: 2, keyboard: Keyboard.Numeric, App.InvReadParams.wLength.ToString());

            //TODO: Validating

            try
            {
                App.InvReadParams.wLength = Convert.ToUInt32(result);                
                item.ItemValueText = App.InvReadParams.wLength.ToString();                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        private void MySettingsList_OnItemSelected(object sender, EventArgs e)
        {
            //User tapped item. Selection not shown. If wanted to show something, it must be done manually.
            ItemTappedEventArgs arg = (ItemTappedEventArgs)e;
            ListItem item = (ListItem)arg.Item;

            if (item.MyObject != null)
            {
                ItemID id = (ItemID)item.MyObject;

                switch (id)
                {
                    case ItemID.Bank:
                        HandleBankSelection(item);
                        break;
                    case ItemID.Type:
                        HandleTypeSelection(item);
                        break;
                    case ItemID.StartAddress:
                        HandleAddrSelection(item);
                        break;
                    case ItemID.WordCount:
                        HandleWordCountSelection(item);
                        break;
                }              
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

        protected override async void OnDisappearing()
        {            
            try
            {
                App.Nur.SetInventoryRead(App.InvReadParams); //Apply new setting
                //Save settings to device memory so we can use same settings when app started next time
                string output = JsonConvert.SerializeObject(App.InvReadParams);               
                Preferences.Set("InvReadParams", output);
            }
            catch(Exception e)
            {
                await DisplayAlert("Operation failed!", e.Message, "OK");
            }
                     

            MySettingsList.OnItemSelected -= MySettingsList_OnItemSelected;
            base.OnDisappearing();
        }

    }
}