using System;
using Xamarin.Forms;

using nur_tools_rfiddemo_xamarin.Views;
using nur_tools_rfiddemo_xamarin.Templates;
using NurApiDotNet;
using System.Collections.Generic;
using static NurApiDotNet.NurApi;

namespace nur_tools_rfiddemo_xamarin
{
    public partial class App : Application
    {        
        static NurApi mNurApi;
        static NurDeviceDiscovery mDeviceDiscovery;
        static StatusBar curStatus;
        static int showMsgTime;

        public App()
        {
            InitializeComponent();
                       
            mNurApi = new NurApi();
            mDeviceDiscovery = new NurDeviceDiscovery();           
            mNurApi.OnTransportStatusChanged += MNurApi_OnTransportStatusChanged;           

            MainPage = new MainPage();
        }
               
        private void MNurApi_OnTransportStatusChanged(object sender, NurTransportStatus e)
        {
            if (e == NurTransportStatus.Connected)
            {                
                //Get fresh AntennaList from reader
                AntennaList = mNurApi.GetAntennaList();
            }

            ShowShortStatusMessage("", 0, Color.Black, Color.Black);
        }

        public static void UpdateTransportStatusBarText(StatusBar sBar)
        {
            if (sBar == null) return;

            sBar.SetBkColor(Color.Black);
            
            if (App.Nur.TransportStatus == NurTransportStatus.Connected)
            {
                string name = Nur.ConnectedDeviceUri?.GetQueryParam("name");
                if (string.IsNullOrEmpty(name))
                    name = Nur.Info.name;

                sBar.SetText("CONNECTED TO: " + name); //Nur.ConnectedDeviceUri?.GetQueryParam("name")); ;
                sBar.SetTextColor(Color.LightGreen);

            }
            else if (App.Nur.TransportStatus == NurTransportStatus.Connecting)
            {
                string name = Nur.ConnectedDeviceUri?.GetQueryParam("name");
                if (string.IsNullOrEmpty(name))
                    name = App.Nur.ConnectedDeviceUri.ToString();

                sBar.SetText("CONNECTING TO: " +  name);
                sBar.SetTextColor(Color.Yellow);
            }
            else if (App.Nur.TransportStatus == NurTransportStatus.Disconnected)
            {
                sBar.SetText("DISCONNECTED");
                sBar.SetTextColor(Color.Red);
            }
        }

        public static List<Antenna> AntennaList { get; private set; }

        public static NurApi Nur  
        {
            get { return mNurApi; }          
        }

        /// <summary>
        /// Holding InventoryEx params<br/>
        /// These cannot read from module
        /// </summary>
        public static InventoryExParams InvExtParams { get; } = new InventoryExParams();

        /// <summary>
        /// Holding InventoryEx filter
        /// </summary>
        public static InventoryExFilter InvExtFilter { get; } = new InventoryExFilter();

        /// <summary>
        /// True if InventoryEx filter is enabled
        /// </summary>
        public static bool IsExtFilterEnabled { get; set; } = new bool();

        /// <summary>
        /// True if InventoryEx is enabled
        /// </summary>
        public static bool IsInventoryExEnabled { get; set; } = new bool();

        /// <summary>
        /// True if Inventroy Read settings is taking accouct when doing inventory
        /// </summary>
        public static bool IsInventoryReadEnabled { get; set; } = new bool();

        /// <summary>
        /// True if inventory results shows 'Pure Uri' instead of EPC if tag is GS1 coded.
        /// </summary>
        public static bool IsShowGS1CodedTags { get; set; } = new bool();

        /// <summary>
        /// True if inventory results shows only GS1 coded tags in inventory result.
        /// </summary>
        public static bool IsShowOnlyGS1CodedTags { get; set; } = new bool();

        public static NurDeviceDiscovery NurDeviceSearch
        {
            get { return mDeviceDiscovery; }
        }
                
        public static void BindStatusMessage(StatusBar o)
        {
            curStatus = o;
            ShowShortStatusMessage("", 0, Color.Black, Color.Black);
        }

        public static void ShowShortStatusMessage(string message, int showTimeSec, Color textColor, Color bkColor)
        {
            if (curStatus == null) return;
            
            // Update tag information in to the UI                          
            Device.BeginInvokeOnMainThread(async () =>
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
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
        
    }
}
