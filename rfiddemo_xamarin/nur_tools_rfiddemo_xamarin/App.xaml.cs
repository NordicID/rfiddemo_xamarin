using System;
using Xamarin.Forms;

using nur_tools_rfiddemo_xamarin.Views;
using nur_tools_rfiddemo_xamarin.Templates;
using NurApiDotNet;
using System.Collections.Generic;
using static NurApiDotNet.NurApi;
using System.Diagnostics;
using Xamarin.Essentials;
using Newtonsoft.Json;

namespace nur_tools_rfiddemo_xamarin
{
    public partial class App : Application
    {           
        static StatusBar curStatus;
        static int showMsgTime;

        public App()
        {
            InitializeComponent();
                       
            Nur = new NurApi();           
                       
            Nur.ConnectionStatusEvent += MNurApi_TransportStatusEvent;

            //======= Uncomment these two lines if need to get log information from the NurApi ==============
            //Nur.SetLogLevel(NurApi.LOG_VERBOSE);
            //Nur.SetLogToStdout(true);

            Nur.LogEvent += MNurApi_LogEvent;                      

            MainPage = new MainPage();
        }

        private void MNurApi_LogEvent(object sender, LogEventArgs e)
        {
            Debug.WriteLine(e.message);
        }

        private void MNurApi_TransportStatusEvent(object sender, NurTransportStatus e)
        {
            if (e == NurTransportStatus.Connected)
            {
                //Get fresh AntennaList from reader
                try
                {
                    AntennaList = Nur.GetAntennaList();
                    Nur.AccBeep(5); //Give short beep to reader
                } 
                catch (Exception ex)
                {
                    Debug.WriteLine("Cannot read AntennaList: " + ex.Message);
                }                                
            }

            ShowShortStatusMessage("", 0, Color.Black, Color.Black);
        }
               
        public static void UpdateTransportStatusBarText(StatusBar sBar)
        {
            if (sBar == null) return;

            sBar.SetBkColor(Color.Black);
            
            if (App.Nur.ConnectionStatus == NurTransportStatus.Connected)
            {
                string name = Nur.ConnectedDeviceUri?.GetQueryParam("name");
                if (string.IsNullOrEmpty(name))
                    name = Nur.Info.name;

                sBar.SetText("CONNECTED TO: " + name);
                sBar.SetTextColor(Color.LightGreen);

            }
            else if (App.Nur.ConnectionStatus == NurTransportStatus.Connecting)
            {                       
                sBar.SetText("CONNECTING...");
                sBar.SetTextColor(Color.Yellow);
            }
            else if (App.Nur.ConnectionStatus == NurTransportStatus.Disconnected)
            {
                sBar.SetText("DISCONNECTED");
                sBar.SetTextColor(Color.Red);
            }
        }

        public static List<AntennaMapping> AntennaList { get; private set; }

        public static NurApi Nur { get; set; }
                
        /// <summary>
        /// Holding InventoryEx params<br/>
        /// These cannot read from module
        /// </summary>
        public static InventoryExParams InvExtParams { get; set; } = new InventoryExParams();

        /// <summary>
        /// Holding InventoryEx filter
        /// </summary>
        public static InventoryExFilter InvExtFilter1 { get; set; } = new InventoryExFilter();

        /// <summary>
        /// True if InventoryEx filter is enabled
        /// </summary>
        public static bool IsExtFilter1Enabled { get; set; } = new bool();

        /// <summary>
        /// Holding InventoryEx filter
        /// </summary>
        public static InventoryExFilter InvExtFilter2 { get; set; } = new InventoryExFilter();

        /// <summary>
        /// True if InventoryEx filter is enabled
        /// </summary>
        public static bool IsExtFilter2Enabled { get; set; } = new bool();

        /// <summary>
        /// True if InventoryEx is enabled
        /// </summary>
        public static bool IsInventoryExEnabled { get; set; } = new bool();

        /// <summary>
        /// Holding InventoryRead params. These will be put in module when activated in InvOptions
        /// </summary>
        public static IrInformation InvReadParams { get; set; } = new IrInformation();
                
        /// <summary>
        /// True if inventory results shows 'Pure Uri' instead of EPC if tag is GS1 coded.
        /// </summary>
        public static bool IsShowGS1CodedTags { get; set; } = new bool();

        /// <summary>
        /// True if inventory results shows only GS1 coded tags in inventory result.
        /// </summary>
        public static bool IsShowOnlyGS1CodedTags { get; set; } = new bool();
        public static void BindStatusMessage(StatusBar o)
        {
            curStatus = o;
            ShowShortStatusMessage("", 0, Color.Black, Color.Black);
        }

        public static void ShowShortStatusMessage(string message, int showTimeSec, Color textColor, Color bkColor)
        {
            if (curStatus == null) return;
            
            // Update tag information in to the UI                          
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(message))
                    {
                        UpdateTransportStatusBarText(curStatus);
                        showTimeSec = 0;
                    }
                    else
                    {
                        curStatus.SetText(message);
                        curStatus.SetTextColor(textColor);
                        curStatus.SetBkColor(bkColor);

                        showMsgTime = showTimeSec;
                    }
                }
                catch (Exception)
                {
                    // Handle error here
                }
            });

        }

        protected override void OnStart()
        {
            //Showing Short messages only specified time, then original back.
            Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
            {
                if (showMsgTime == 0)
                {   
                    //It's time to show original message
                    UpdateTransportStatusBarText(curStatus);                 
                }

                showMsgTime--;
                
                return true; // True = Repeat again, False = Stop the timer
            });

            /*
             Reloading Preferences from Android/iOS/UWP device
            - InventoryExtended parameters and filter
            - InventoryRead parameters
            
            */

            try
            {
                string json = Preferences.Get("InvExtParams", "");
                if (!string.IsNullOrEmpty(json))
                {
                   InvExtParams = JsonConvert.DeserializeObject<InventoryExParams>(json);
                }

                json = Preferences.Get("InvExtFilter1", "");
                if (!string.IsNullOrEmpty(json))
                {
                    InvExtFilter1 = JsonConvert.DeserializeObject<InventoryExFilter>(json);
                }

                json = Preferences.Get("InvExtFilter2", "");
                if (!string.IsNullOrEmpty(json))
                {
                    InvExtFilter2 = JsonConvert.DeserializeObject<InventoryExFilter>(json);
                }

                IsExtFilter1Enabled = Preferences.Get("InvExtFilter1Enabled", false);
                IsExtFilter2Enabled = Preferences.Get("InvExtFilter2Enabled", false);

                json = Preferences.Get("InvReadParams", "");
                if (!string.IsNullOrEmpty(json))
                {
                    InvReadParams = JsonConvert.DeserializeObject<IrInformation>(json);
                }


            }
            catch(Exception e)
            {
                Debug.WriteLine("Error loading InvExt params:" + e.Message);
            }
            
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public static int ValidateAndConvertToInt(string result, int min, int max)
        {
            int val;

            if (string.IsNullOrEmpty(result))
                throw new Exception("Cancel or no value");

            try
            {
                val = Convert.ToInt32(result);
                if (val < min || val > max)
                    throw new Exception("Value not in range. Must be " + min.ToString() + "-" + max.ToString());

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return val;
        }
    }
}
