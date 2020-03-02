using System;
using System.Collections.Generic;
using System.Diagnostics;
using NurApiDotNet;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static NurApiDotNet.NurApi;

namespace nur_tools_rfiddemo_xamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsRFID : ContentPage
    {
        Dictionary<string,int> indexDict = new Dictionary<string, int>();
        
        public SettingsRFID()
        {
            InitializeComponent();
        }
                
        private async void PrepareControls()
        {
            if (App.Nur.IsConnected())
            {
                controlGrid.IsEnabled = true;

                try
                {
                    RegionId region = (RegionId)App.Nur.Setup.regionId;
                    LblRegion.Text = region.ToString();

                    RxSensitivity rxs = (RxSensitivity)App.Nur.Setup.rxSensitivity;
                    LblRxSensitivity.Text = rxs.ToString();

                    int freq = (int)App.Nur.Setup.linkFreq;
                    LblLinkFreq.Text = freq.ToString() + " Hz";

                    RxDecoding rxd = (RxDecoding)App.Nur.Setup.rxDecoding;
                    LblRXDec.Text = rxd.ToString();

                    TxModulation tmod = (TxModulation)App.Nur.Setup.txModulation;
                    LblTXMod.Text = tmod.ToString();
                    LblAutoTune.Text = App.Nur.Setup.autotune.mode.ToString();
                    
                    if(App.Nur.Capabilites.HasRfProfile())
                    {
                        RfProfile rfp = (RfProfile)App.Nur.Setup.rfProfile;
                        LblRFProfile.Text = rfp.ToString();
                        ButtonRFProfile.IsEnabled = true;
                    }
                    else
                    {
                        LblRFProfile.Text = "Not supported";
                        ButtonRFProfile.IsEnabled = false;
                    }
                                        
                    List<double> levels = App.Nur.Capabilites.GetTxLevels();
                    string[] arr = new string[levels.Count];
                    LblTxLevel.Text = levels[(int)App.Nur.Setup.txLevel].ToString("#.#") + " mW";
                }
                catch (Exception e)
                {
                    await DisplayAlert("Cannot read setup!", e.Message, "OK");
                }
            }
            else controlGrid.IsEnabled = false;


        }
        
        private async void OnButtonRegionClicked(object sender, EventArgs e)
        {
            string[] arr = Enum.GetNames(typeof(RegionId));
            string action = await DisplayActionSheet("Region", "Cancel", null, arr);
            IndexDictionary dict = new IndexDictionary(arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed  

            try
            {
                App.Nur.Region = levelIndex;               
                LblRegion.Text = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void OnButtonRxSensitivityClicked(object sender, EventArgs e)
        {
            string[] arr = Enum.GetNames(typeof(RxSensitivity));            
            string action = await DisplayActionSheet("RX Sensitivity", "Cancel", null, arr);
            IndexDictionary dict = new IndexDictionary(arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed   

            try
            {
                App.Nur.RxSensitivity = levelIndex;              
                LblRxSensitivity.Text = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void OnButtonLinkFreqClicked(object sender, EventArgs e)
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
                LblLinkFreq.Text = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void OnButtonRXDecClicked(object sender, EventArgs e)
        {
            string[] arr = Enum.GetNames(typeof(RxDecoding));
            string action = await DisplayActionSheet("RX Decoding", "Cancel", null, arr);
            IndexDictionary dict = new IndexDictionary(arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed  

            try
            {
                App.Nur.RxDecoding = levelIndex;               
                LblRXDec.Text = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void OnButtonTXModClicked(object sender, EventArgs e)
        {
            string[] arr = Enum.GetNames(typeof(TxModulation));
            string action = await DisplayActionSheet("TX Modulation", "Cancel", null, arr);
            IndexDictionary dict = new IndexDictionary(arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed  

            try
            {
                App.Nur.TxModulation = levelIndex;                
                LblTXMod.Text = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        private async void OnButtonRFProfileClicked(object sender, EventArgs e)
        {
            string[] arr = Enum.GetNames(typeof(RfProfile));
            string action = await DisplayActionSheet("RF Profile", "Cancel", null, arr);
            IndexDictionary dict = new IndexDictionary(arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed   

            try
            {
                App.Nur.RfProfile = levelIndex;               
                LblRFProfile.Text = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        private async void OnButtonTxlevelClicked(object sender, EventArgs e)
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
                LblTxLevel.Text = action; //Put in UI
            }
            catch(Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        private async void OnButtonAutoTuneClicked(object sender, EventArgs e)
        {
            string[] arr = Enum.GetNames(typeof(AutotuneSetup.Mode)); 
            string action = await DisplayActionSheet("AutoTune setup", "Cancel", null, arr);
            IndexDictionary dict = new IndexDictionary(arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed    

            try
            {
                AutotuneSetup setup = new AutotuneSetup();
                setup.mode = (AutotuneSetup.Mode)levelIndex;
                if (setup.mode == AutotuneSetup.Mode.ThresholdEnable)
                {
                    string result = await DisplayPromptAsync("Set threshold (dBm)","(0-40)", "OK", "Cancel", App.Nur.Setup.readRssiFilter.min.ToString(), maxLength: 3, keyboard: Keyboard.Numeric);

                    try
                    {
                        setup.threshold_dBm = NurApi.ValidateAndConvertToInt(result, 0, 40);
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
                LblAutoTune.Text = action;
                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        protected override void OnAppearing()
        {           
            base.OnAppearing();

            //Need in order to show status messages bottom of the screen
            App.BindStatusMessage(MyStatusBar);

            PrepareControls();            
        }

        protected override void OnDisappearing()
        {   
            base.OnDisappearing();
        }        

    }
}