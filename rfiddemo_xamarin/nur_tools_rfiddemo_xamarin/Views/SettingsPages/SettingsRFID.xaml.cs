using System;
using System.Collections.Generic;
using System.Diagnostics;
using NurApiDotNet;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static NurApiDotNet.NurApi;
using System.Collections.ObjectModel;
using nur_tools_rfiddemo_xamarin.Templates;

namespace nur_tools_rfiddemo_xamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsRFID : ContentPage
    {
        private readonly ObservableCollection<ListItem> itemList = new ObservableCollection<ListItem>();

        public SettingsRFID()
        {
            InitializeComponent();
        }

        enum ItemID
        {
            Region,
            RxSensitivity,
            LinkFreq,
            RXDec,
            TXMod,
            Autotune,
            RFProfile,
            TXLevel
        }

        private void PrepareListItems()
        {
            if (App.Nur.IsConnected())
            {
                itemList.Clear();

                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        ListItemStyle style = new ListItemStyle("ic_settings_black", 20, Color.White, Color.Black, Color.Blue);

                        RegionId region = (RegionId)App.Nur.Setup.regionId;
                        itemList.Add(new ListItem(style, "Region", region.ToString(), ItemID.Region));

                        RxSensitivity rxs = (RxSensitivity)App.Nur.Setup.rxSensitivity;
                        itemList.Add(new ListItem(style, "Rx Sensitivity", rxs.ToString(), ItemID.RxSensitivity));

                        int freq = (int)App.Nur.Setup.linkFreq;
                        itemList.Add(new ListItem(style, "Link Frequency", freq.ToString(), ItemID.LinkFreq));

                        RxDecoding rxd = (RxDecoding)App.Nur.Setup.rxDecoding;
                        itemList.Add(new ListItem(style, "Rx Decoding", rxd.ToString(), ItemID.RXDec));

                        TxModulation tmod = (TxModulation)App.Nur.Setup.txModulation;
                        itemList.Add(new ListItem(style, "Tx Modulation", tmod.ToString(), ItemID.TXMod));

                        AutotuneSetup.Mode autotuneSetupMode = (AutotuneSetup.Mode)App.Nur.Setup.autotune.mode;
                        itemList.Add(new ListItem(style, "Auto Tune", autotuneSetupMode.ToString(), ItemID.Autotune));

                        if (App.Nur.Capabilites.rfProfile)
                        {
                            RfProfile rfp = (RfProfile)App.Nur.Setup.rfProfile;
                            itemList.Add(new ListItem(style, "RF Profile", rfp.ToString(), ItemID.RFProfile));
                        }

                        List<double> levels = App.Nur.Capabilites.GetTxLevels();
                        string[] arr = new string[levels.Count];
                        itemList.Add(new ListItem(style, "Tx Level", levels[(int)App.Nur.Setup.txLevel].ToString("#.#") + " mW", ItemID.TXLevel));
                    }
                    catch (Exception e)
                    {
                        await DisplayAlert("Cannot read setup!", e.Message, "OK");
                    }
                });
            }
        }

        private async void HandleRegion(ListItem item)
        {
            string[] arr = Enum.GetNames(typeof(RegionId));
            string action = await DisplayActionSheet("Region", "Cancel", null, arr);
            IndexDictionary dict = new IndexDictionary(arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed  

            try
            {
                App.Nur.Region = levelIndex;
                item.ItemValueText = action;                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void HandleRxSensitivity(ListItem item)
        {
            string[] arr = Enum.GetNames(typeof(RxSensitivity));            
            string action = await DisplayActionSheet("RX Sensitivity", "Cancel", null, arr);
            IndexDictionary dict = new IndexDictionary(arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed   

            try
            {
                App.Nur.RxSensitivity = levelIndex;
                item.ItemValueText = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void HandleLinkFreq(ListItem item)
        {
            List<int> freqs = App.Nur.Capabilites.GetSupportedLinkFreqs();

            string[] arr = new string[freqs.Count];
            for (int x = 0; x < freqs.Count; x++)
                arr[x] = freqs[x].ToString() + " Hz";
                        
            string action = await DisplayActionSheet("Link Frequency", "Cancel", null, arr);
            IndexDictionary dict = new IndexDictionary(arr);
            int frqIndex = dict.GetItemIndex(action);
            if (frqIndex == -1) return; //Possible Cancel pressed  

            try
            {
                App.Nur.LinkFrequency = freqs[frqIndex];
                item.ItemValueText = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void HandleRXDec(ListItem item)
        {
            string[] arr = Enum.GetNames(typeof(RxDecoding));
            string action = await DisplayActionSheet("RX Decoding", "Cancel", null, arr);
            IndexDictionary dict = new IndexDictionary(arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed  

            try
            {
                App.Nur.RxDecoding = levelIndex;
                item.ItemValueText = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void HandleTXMod(ListItem item)
        {
            string[] arr = Enum.GetNames(typeof(TxModulation));
            string action = await DisplayActionSheet("TX Modulation", "Cancel", null, arr);
            IndexDictionary dict = new IndexDictionary(arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed  

            try
            {
                App.Nur.TxModulation = levelIndex;
                item.ItemValueText = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        private async void HandleRFProfile(ListItem item)
        {
            string[] arr = Enum.GetNames(typeof(RfProfile));
            string action = await DisplayActionSheet("RF Profile", "Cancel", null, arr);
            IndexDictionary dict = new IndexDictionary(arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed   

            try
            {
                App.Nur.RfProfile = levelIndex;
                item.ItemValueText = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        private async void HandleTxlevel(ListItem item)
        {
            //Dynamic txlevel set. Tx levels are different in readers. Dev caps can tell levels.
            List<double> levels = App.Nur.Capabilites.GetTxLevels(); 
            
            string[] arr = new string[levels.Count];
            for (int x=0;x<levels.Count;x++)
                 arr[x] = levels[x].ToString("#.#") + " mW";

            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set Tx level", "Cancel", null, arr);
           
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed                                  

            try
            {
                App.Nur.TxLevel = levelIndex;
                item.ItemValueText = action; //Put in UI
            }
            catch(Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        private async void HandleAutoTune(ListItem item)
        {
            string[] arr = Enum.GetNames(typeof(AutotuneSetup.Mode)); 
            string action = await DisplayActionSheet("AutoTune setup", "Cancel", null, arr);
            IndexDictionary dict = new IndexDictionary(arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed    

            try
            {
                AutotuneSetup setup = new AutotuneSetup();
                setup.mode = (byte)levelIndex;
                if (setup.mode == (byte)AutotuneSetup.Mode.ThresholdEnable)
                {
                    string result = await DisplayPromptAsync("Set threshold (dBm)","(0-40)", "OK", "Cancel", App.Nur.Setup.readRssiFilter.min.ToString(), maxLength: 3, keyboard: Keyboard.Numeric, App.Nur.Setup.readRssiFilter.min.ToString());

                    try
                    {
                        setup.threshold_dBm = (sbyte)Utils.ValidateAndConvertToInt(result, 0, 40);
                        action += " (threshold=" + setup.threshold_dBm.ToString()+")"; //TODO: notwork

                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.StartsWith("Cancel")) return;
                        await DisplayAlert("Invalid value", ex.Message, "OK");
                        return;
                    }
                }

                App.Nur.Autotune = setup;
                item.ItemValueText = action;
                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        

        private void MySettingsList_OnItemSelected(object sender, EventArgs e)
        {
            //User tapped item. Selection not shown. If wanted to show selection, it must be done manually like changing Bkcolor of item.
            ItemTappedEventArgs arg = (ItemTappedEventArgs)e;
            ListItem item = (ListItem)arg.Item;            
           
            if (item.MyObject != null)
            {
                ItemID id = (ItemID)item.MyObject;
                switch (id)
                {
                    case ItemID.Region:
                        HandleRegion(item);
                        break;
                    case ItemID.RxSensitivity:
                        HandleRxSensitivity(item);
                        break;
                    case ItemID.LinkFreq:
                        HandleLinkFreq(item);
                        break;
                    case ItemID.RXDec:
                        HandleRXDec(item);
                        break;
                    case ItemID.TXMod:
                        HandleTXMod(item);
                        break;
                    case ItemID.Autotune:
                        HandleAutoTune(item);
                        break;
                    case ItemID.RFProfile:
                        HandleRFProfile(item);
                        break;
                    case ItemID.TXLevel:
                        HandleTxlevel(item);
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

        protected override void OnDisappearing()
        {
            MySettingsList.OnItemSelected -= MySettingsList_OnItemSelected;
            base.OnDisappearing();
        }        

    }
}