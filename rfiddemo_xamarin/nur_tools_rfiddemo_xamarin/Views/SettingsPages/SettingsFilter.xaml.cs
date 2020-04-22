using System;
using Xamarin.Essentials;
using Newtonsoft.Json;
using NurApiDotNet;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using nur_tools_rfiddemo_xamarin.Templates;
using static NurApiDotNet.NurApi;

namespace nur_tools_rfiddemo_xamarin.Views.SettingsPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsFilter : ContentPage
    {
        ObservableCollection<ListItem> itemList = new ObservableCollection<ListItem>();
        InventoryExFilter mFilter;
        int mIndex;

        public SettingsFilter()
        {            
            InitializeComponent();              
        }

        public void SetFilter(InventoryExFilter filter, int index)
        {
            mFilter = filter;
            mIndex = index;
        }

        private void PrepareListItems()
        {
            string txt;

            itemList.Clear();

            Device.BeginInvokeOnMainThread(async () =>
            {
                ListItemStyle style = new ListItemStyle("ic_settings_black", 20, Color.White, Color.Black, Color.Blue);

                ListItem enabledItem = new ListItem();
                enabledItem.Selected = (mIndex == 0) ? App.IsExtFilter1Enabled : App.IsExtFilter2Enabled;
                enabledItem.ItemHeaderText = "Enable Filter " + (mIndex + 1).ToString();
                FilterEnable(enabledItem);
                itemList.Add(enabledItem);

                FilterAction action = (FilterAction)mFilter.action;
                itemList.Add(new ListItem(style, "Action", action.ToString()));

                Bank bank = (Bank)mFilter.bank;
                itemList.Add(new ListItem(style, "Bank", bank.ToString()));

                TargetSession target = (TargetSession)mFilter.target;
                itemList.Add(new ListItem(style, "Target session", target.ToString()));
                itemList.Add(new ListItem(style, "Address", mFilter.address.ToString()));

                if (mFilter.maskData != null)
                {
                    txt = NurApi.BinToHexString(mFilter.maskData);
                }
                else txt = "";

                itemList.Add(new ListItem(style, "Mask (HEX string)", txt));
                itemList.Add(new ListItem(style, "Mask length (bits)", mFilter.maskBitLength.ToString()));
            });
        }               

        void FilterEnable(ListItem enabledItem)
        {
            if (enabledItem.Selected)
            {
                enabledItem.SetImage("ic_enabled.png", 20);
                enabledItem.ItemValueText = "Enabled";
                enabledItem.ItemValueColor = Color.Green;
            }
            else
            {
                enabledItem.SetImage("ic_disabled.png", 20);
                enabledItem.ItemValueText = "Disabled";
                enabledItem.ItemValueColor = Color.Red;
            }

            if(mIndex==0) App.IsExtFilter1Enabled = enabledItem.Selected;
            else App.IsExtFilter2Enabled = enabledItem.Selected;           
        }

        async void ActionSelection(ListItem item)
        {
            /*
            string[] arr = new string[8];
            
            arr[0] = "assert SL or inventoried session flag -> A.Non - matching: deassert SL or inventoried session flag -> B. (action = 0)";
            arr[1] = "assert SL or inventoried session flag -> A.Non - matching: do nothing. (action = 1)";
            arr[2] = "do nothing.Non - matching: deassert SL or inventoried session->B. (action = 2)";
            arr[3] = "negate SL or invert inventoried session flag(A->B, B->A).Non - matching: do nothing. (action = 3)";
            arr[4] = "deassert SL or inventoried session flag->B.Non - matching: assert SL or inventoried session flag -> A. (action = 4)";
            arr[5] = "deassert SL or inventoried session flag -> B.Non - matching: do nothing. (action = 5)";
            arr[6] = "do nothing.Non - matching: assert SL or inventoried session flag -> A. (action = 6)";
            arr[7] = "do nothing.Non - matching: negate SL or invert inventoried session flag(A->B, B->A). (action = 7)";
            */
                        
            string[] arr = Enum.GetNames(typeof(FilterAction)); //Just name selection as Action0, Action1...
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set Filter action", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                Array actionValues = Enum.GetValues(typeof(FilterAction));
                FilterAction b = (FilterAction)actionValues.GetValue(levelIndex);
                mFilter.action = (byte)b;
                item.ItemValueText = b.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void BankSelection(ListItem item)
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
                mFilter.bank = (byte)b;
                item.ItemValueText = b.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void TargetSelection(ListItem item)
        {
            string[] arr = Enum.GetNames(typeof(TargetSession));
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set Target", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                Array targetValues = Enum.GetValues(typeof(TargetSession));
                TargetSession b = (TargetSession)targetValues.GetValue(levelIndex);
                mFilter.target = (byte)b;
                item.ItemValueText = b.ToString();                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void AddressSelection(ListItem item)
        {
            string result = await DisplayPromptAsync("Set address", "(bits)", "OK", "Cancel", mFilter.address.ToString(), maxLength: 3, keyboard: Keyboard.Numeric, mFilter.address.ToString());

            if (result == "Cancel") return;
            if (string.IsNullOrEmpty(result)) return;

            try
            {
                mFilter.address = Convert.ToUInt32(result);
                item.ItemValueText = mFilter.address.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void MaskDataSelection(ListItem item)
        {
            string placeHolder = "";
            if(mFilter.maskData != null)
            {
                placeHolder = NurApi.BinToHexString(mFilter.maskData);
            }
            string result = await DisplayPromptAsync("Set Mask", "(hex)", "OK", "Cancel", placeHolder, maxLength: 20, keyboard: Keyboard.Text, placeHolder);

            if (result == "Cancel") return;
            if (string.IsNullOrEmpty(result)) return;

            try
            {
                mFilter.maskData = NurApi.HexStringToBin(result);
                item.ItemValueText = result;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void MaskLengthSelection(ListItem item)
        {
            string result = await DisplayPromptAsync("Set Length", "(bits)", "OK", "Cancel", mFilter.maskBitLength.ToString(), maxLength: 3, keyboard: Keyboard.Numeric, mFilter.maskBitLength.ToString());

            if (result == "Cancel") return;
            if (string.IsNullOrEmpty(result)) return;

            try
            {
                mFilter.maskBitLength = Convert.ToInt32(result);
                item.ItemValueText = mFilter.maskBitLength.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
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

        private void MySettingsList_OnItemSelected(object sender, EventArgs e)
        {
            //User tapped item. Selection not shown. If wanted to show selection, it must be done manually like changing Bkcolor of item.
            ItemTappedEventArgs arg = (ItemTappedEventArgs)e;
            ListItem item = (ListItem)arg.Item;

            switch (arg.ItemIndex)
            {
                case 0:
                    FilterEnable(item);
                    break;
                case 1:
                    ActionSelection(item);
                    break;
                case 2:
                    BankSelection(item);
                    break;
                case 3:
                    TargetSelection(item);
                    break;
                case 4:
                    AddressSelection(item);
                    break;
                case 5:
                    MaskDataSelection(item);
                    break;
                case 6:
                    MaskLengthSelection(item);
                    break;
            }
        }

        protected override void OnDisappearing()
        {
            //Save settings to device memory so we can use same settings when app started next time
            string output = JsonConvert.SerializeObject(mFilter);
            if (mIndex == 0)
                Preferences.Set("InvExtFilter1", output);
            else Preferences.Set("InvExtFilter2", output);

            MySettingsList.OnItemSelected -= MySettingsList_OnItemSelected;
            base.OnDisappearing();

        }   
        
    }
}