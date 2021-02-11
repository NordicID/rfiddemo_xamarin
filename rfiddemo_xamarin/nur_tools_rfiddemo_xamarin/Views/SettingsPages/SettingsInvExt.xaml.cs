using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using nur_tools_rfiddemo_xamarin.Templates;
using NurApiDotNet;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace nur_tools_rfiddemo_xamarin.Views.SettingsPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsInvExt : ContentPage
    {
        ObservableCollection<ListItem> itemList = new ObservableCollection<ListItem>();
        
        public SettingsInvExt()
        {
            InitializeComponent();                   
        }

        enum ItemID
        {
            Q,
            Rounds,
            Session,
            Target,
            TransitTime,
            SelectState,
            Filter1,
            Filter2
        }

        private void PrepareListItems()
        {
            string txt;
            
            itemList.Clear();

            Device.BeginInvokeOnMainThread(() =>
            {
                ListItemStyle style = new ListItemStyle("ic_settings_black", 20, Color.White, Color.Black, Color.Blue);

                style.styleCellHeight = 40;

                if (App.InvExtParams.Q == 0) txt = "Auto";
                else txt = App.InvExtParams.Q.ToString();
                itemList.Add(new ListItem(style, "Q", txt, ItemID.Q));

                if (App.InvExtParams.rounds == 0) txt = "Auto";
                else txt = App.InvExtParams.rounds.ToString();
                itemList.Add(new ListItem(style, "Rounds", txt, ItemID.Rounds));
                itemList.Add(new ListItem(style, "Session", App.InvExtParams.session.ToString(), ItemID.Session));

                InventoryTarget target = (InventoryTarget)App.InvExtParams.inventoryTarget;
                itemList.Add(new ListItem(style, "Target", target.ToString(), ItemID.Target));
                itemList.Add(new ListItem(style, "Transit time", App.InvExtParams.transitTime.ToString(), ItemID.TransitTime));

                InventorySelectState sel = (InventorySelectState)App.InvExtParams.inventorySelState;
                itemList.Add(new ListItem(style, "Select state", sel.ToString(), ItemID.SelectState));

                ListItemStyle fStyle = new ListItemStyle("ic_filter.png", 20, Color.White, Color.Black, Color.Green);

                fStyle.styleCellHeight = 40;

                if (App.IsExtFilter1Enabled)
                {
                    Bank bnk = (Bank)App.InvExtFilter1.bank;
                    InventoryTarget tgt = (InventoryTarget)App.InvExtFilter1.target;
                    txt = "Act " + App.InvExtFilter1.action.ToString() + "," + bnk.ToString() + "," + App.InvExtFilter1.address.ToString() + "/" + App.InvExtFilter1.maskBitLength.ToString() + ", " + tgt.ToString() + " Data:" + NurApi.BinToHexString(App.InvExtFilter1.maskData);
                    fStyle.styleValueColor = Color.Green;
                    fStyle.styleSingleRow = false;
                    itemList.Add(new ListItem(fStyle, "Filter 1", txt, ItemID.Filter1));
                }
                else
                {
                    fStyle.styleValueColor = Color.Red;
                    itemList.Add(new ListItem(fStyle, "Filter 1", "Disabled", ItemID.Filter1));
                }

                if (App.IsExtFilter2Enabled)
                {
                    Bank bnk = (Bank)App.InvExtFilter2.bank;
                    InventoryTarget tgt = (InventoryTarget)App.InvExtFilter2.target;
                    txt = "Act " + App.InvExtFilter2.action.ToString() + "," + bnk.ToString() + "," + App.InvExtFilter2.address.ToString() + "/" + App.InvExtFilter2.maskBitLength.ToString() + ", " + tgt.ToString() + " Data:" + NurApi.BinToHexString(App.InvExtFilter2.maskData);
                    fStyle.styleValueColor = Color.Green;
                    fStyle.styleSingleRow = false;
                    itemList.Add(new ListItem(fStyle, "Filter 2", txt, ItemID.Filter2));
                }
                else
                {
                    fStyle.styleSingleRow = true;
                    fStyle.styleValueColor = Color.Red;
                    itemList.Add(new ListItem(fStyle, "Filter 2", "Disabled", ItemID.Filter2));
                }
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Fill items to list
            PrepareListItems();                                   
            
            //This event needed when user click item
            MySettingsList.OnItemSelected += MySettingsList_OnItemSelected;

            //Assing item list to ListTemplate
            MySettingsList.SetItemsSource(itemList);
        }

        protected override void OnDisappearing()
        {
            //Save settings to device memory so we can use same settings when app started next time
            string output = JsonConvert.SerializeObject(App.InvExtParams);
            Preferences.Set("InvExtParams", output);
            Preferences.Set("InvExtFilter1Enabled", App.IsExtFilter1Enabled);
            Preferences.Set("InvExtFilte2Enabled", App.IsExtFilter2Enabled);
            
            MySettingsList.OnItemSelected -= MySettingsList_OnItemSelected;
            base.OnDisappearing();
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
                    case ItemID.Q:
                        HandleInventoryQ(item);
                        break;
                    case ItemID.Rounds:
                        HandleInventoryRounds(item);
                        break;
                    case ItemID.Session:
                        HandleInventorySession(item);
                        break;
                    case ItemID.Target:
                        HandleInventoryTarget(item);
                        break;
                    case ItemID.TransitTime:
                        HandleTransitTime(item);
                        break;
                    case ItemID.SelectState:
                        HandleSelectState(item);
                        break;
                    case ItemID.Filter1:
                        HandleFilter1();
                        break;
                    case ItemID.Filter2:
                        HandleFilter2();
                        break;
                }
            }
        }

        async void HandleInventoryQ(ListItem item)
        {
            string[] arr = { "Auto", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set inventory Q", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed  

            try
            {
                App.InvExtParams.Q = levelIndex;
                item.ItemValueText = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void HandleInventoryRounds(ListItem item)
        {
            string[] arr = { "Auto", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set inventory rounds", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed  

            try
            {
                App.InvExtParams.rounds = levelIndex;
                item.ItemValueText = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void HandleInventorySession(ListItem item)
        {

            string[] arr = Enum.GetNames(typeof(TargetSession));
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set Session", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                Array bankValues = Enum.GetValues(typeof(TargetSession));
                TargetSession t = (TargetSession)bankValues.GetValue(levelIndex);
                App.InvExtParams.session = (int)t;
                item.ItemValueText = t.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void HandleTransitTime(ListItem item)
        {
            string result = await DisplayPromptAsync("Transit time", "0 = max 1000ms", "OK", "Cancel", App.InvExtParams.transitTime.ToString(), maxLength: 4, keyboard: Keyboard.Numeric, App.InvExtParams.transitTime.ToString());

            if (result == "Cancel") return;
            if (string.IsNullOrEmpty(result)) return;

            try
            {
                App.InvExtParams.transitTime = Convert.ToInt32(result);
                if (App.InvExtParams.transitTime > 1000)
                    App.InvExtParams.transitTime = 0;

                item.ItemValueText = App.InvExtParams.transitTime.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void HandleFilter1()
        {
            SettingsPages.SettingsFilter flt = new SettingsFilter();
            flt.SetFilter(App.InvExtFilter1, 0);
            await Navigation.PushAsync(flt);
           
        }

        async void HandleFilter2()
        {
            SettingsPages.SettingsFilter flt = new SettingsFilter();
            flt.SetFilter(App.InvExtFilter2, 1);
            await Navigation.PushAsync(flt);

            //await Navigation.PushAsync(new SettingsPages.SettingsFilter(App.InvExtFilter2, 1));            
        }

        async void HandleInventoryTarget(ListItem item)
        {
            string[] arr = Enum.GetNames(typeof(InventoryTarget));
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set Inventory Target", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                Array values = Enum.GetValues(typeof(InventoryTarget));
                InventoryTarget tg = (InventoryTarget)values.GetValue(levelIndex);
                App.InvExtParams.inventoryTarget = (int)tg;
                item.ItemValueText = tg.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void HandleSelectState(ListItem item)
        {
            string[] arr = Enum.GetNames(typeof(InventorySelectState));
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Select state", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                Array values = Enum.GetValues(typeof(InventorySelectState));
                InventorySelectState sel = (InventorySelectState)values.GetValue(levelIndex);
                App.InvExtParams.inventorySelState = (int)sel;
                item.ItemValueText = sel.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
    }
}