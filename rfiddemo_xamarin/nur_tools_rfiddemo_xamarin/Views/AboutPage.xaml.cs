using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NurApiDotNet;
using NurApiDotNet.Support;
using System.Collections.ObjectModel;
using nur_tools_rfiddemo_xamarin.Templates;
using Xamarin.Essentials;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nur_tools_rfiddemo_xamarin.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AboutPage : ContentPage
    {
        readonly ObservableCollection<ListItem> itemList = new ObservableCollection<ListItem>();

        public AboutPage()
        {
            InitializeComponent();           
        }

        private async Task PrepareListItems()
        {
            //await Task.Run(async () => {


                Device.BeginInvokeOnMainThread(() =>
                {
                    itemList.Clear();

                    ListItemStyle style = new ListItemStyle("ic_box.png", 5, Color.FromRgb(225, 225, 220), Color.Black, Color.Blue);
                    style.styleCellHeight = 22;
                    itemList.Add(new ListItem(style, "App Versions", ""));
                    style.styleBkColor = Color.White;
                    itemList.Add(new ListItem(style, "Application", typeof(App).Assembly.GetName().Version.ToString()));
                    itemList.Add(new ListItem(style, "NurApi", typeof(NurApi).Assembly.GetName().Version.ToString()));
                    itemList.Add(new ListItem(style, "NurApi.Support", typeof(TagCodecService).Assembly.GetName().Version.ToString()));

                    style.styleBkColor = Color.FromRgb(225, 225, 220);
                    itemList.Add(new ListItem(style, "Platform & Device", ""));
                    style.styleBkColor = Color.White;
                    itemList.Add(new ListItem(style, "Device Model", DeviceInfo.Model));
                    itemList.Add(new ListItem(style, "OS version", DeviceInfo.VersionString));
                    itemList.Add(new ListItem(style, "Platform", DeviceInfo.Platform.ToString()));
                    itemList.Add(new ListItem(style, "Idiom", DeviceInfo.Idiom.ToString()));
                    if (App.Nur.IsConnected())
                    {
                        style.styleBkColor = Color.FromRgb(225, 225, 220);
                        itemList.Add(new ListItem(style, "Reader", ""));
                        style.styleBkColor = Color.White;

                        itemList.Add(new ListItem(style, "Name", App.Nur.Info.name));
                        itemList.Add(new ListItem(style, "Serial", App.Nur.Info.serial));
                        itemList.Add(new ListItem(style, "Alt serial", App.Nur.Info.altSerial));

                        NurApi.ModuleVersions vers = App.Nur.GetVersions();

                        itemList.Add(new ListItem(style, "Firmware version", vers.primaryVersion));
                        itemList.Add(new ListItem(style, "Bootloader version", vers.secondaryVersion));

                        if (App.Nur.Capabilites.IsOneWattReader())
                        {
                            string ver = App.Nur.Capabilites.secChipMajorVersion.ToString() + "." + App.Nur.Capabilites.secChipMinorVersion.ToString() + "." + App.Nur.Capabilites.secChipMaintenanceVersion.ToString() + "." + App.Nur.Capabilites.secChipReleaseVersion.ToString();
                            itemList.Add(new ListItem(style, "Secondary chip", ver));
                        }
                        itemList.Add(new ListItem(style, "Hardware version", App.Nur.Info.hwVersion));
                        itemList.Add(new ListItem(style, "Antennas", App.Nur.Info.maxAntennas.ToString()));
                        itemList.Add(new ListItem(style, "GPIO's", App.Nur.Info.numGpio.ToString()));
                        itemList.Add(new ListItem(style, "Sensors", App.Nur.Info.numSensors.ToString()));
                        itemList.Add(new ListItem(style, "Max Tx level", App.Nur.Capabilites.maxTxmW.ToString() + "mW"));

                        try
                        {
                            NurApi.AccessoryBatteryInfo batt = App.Nur.AccGetBatteryInfo();

                            style.styleBkColor = Color.FromRgb(225, 225, 220);
                            itemList.Add(new ListItem(style, "Battery", ""));
                            style.styleBkColor = Color.White;
                            //Battery information. If reader has not battery, exception thrown

                            itemList.Add(new ListItem(style, "Level %", batt.Percentage.ToString() + "%"));
                            itemList.Add(new ListItem(style, "mV/mA", batt.Voltage + "mV / " + batt.Current.ToString() + "mA"));
                            itemList.Add(new ListItem(style, "Capasity", batt.Capacity + "mAh"));
                            itemList.Add(new ListItem(style, "Charging", batt.Charging.ToString()));

                        }
                        catch (Exception)
                        {
                            itemList.Add(new ListItem(style, "No Battery", ""));
                        }

                        try
                        {
                            AccessoryConfig cfg = App.Nur.AccGetConfig();

                            style.styleBkColor = Color.FromRgb(225, 225, 220);
                            itemList.Add(new ListItem(style, "Accessories", ""));
                            style.styleBkColor = Color.White;

                            itemList.Add(new ListItem(style, "Has imager", cfg.hasImagerScanner().ToString()));
                            itemList.Add(new ListItem(style, "Has vibrator", cfg.hasVibrator().ToString()));
                            itemList.Add(new ListItem(style, "Has wireless charging", cfg.hasWirelessCharging().ToString()));
                            if (cfg.hasWirelessCharging())
                                itemList.Add(new ListItem(style, "Wireless charge status", App.Nur.AccGetWirelessChargeStatus().ToString()));
                            itemList.Add(new ListItem(style, "Allow pairing", cfg.GetAllowPairingState().ToString()));
                            itemList.Add(new ListItem(style, "HID mode", cfg.GetHidMode().ToString()));




                        }
                        catch (Exception e)
                        {
                            itemList.Add(new ListItem(style, "No accessories", ""));
                        }

                        try
                        {
                            List<NurApi.AccessorySensorConfig> sensorList = App.Nur.AccSensorEnumerate();

                            style.styleBkColor = Color.FromRgb(225, 225, 220);
                            if (sensorList.Count > 0)
                                itemList.Add(new ListItem(style, "Sensors", ""));
                            else itemList.Add(new ListItem(style, "No sensors", ""));
                            style.styleBkColor = Color.White;
                            style.styleSingleRow = false;
                            style.styleCellHeight = 60;
                            foreach (NurApi.AccessorySensorConfig asg in sensorList)
                            {
                                itemList.Add(new ListItem(style, asg.type.ToString(), "Source=" + asg.source.ToString() + " Feature=" + asg.feature));
                            }
                        }
                        catch (Exception)
                        {
                            itemList.Add(new ListItem(style, "No senors", ""));
                        }

                    }
                    else
                    {
                        style.styleBkColor = Color.Red;
                        itemList.Add(new ListItem(style, "Reader not connected", ""));
                    }


                //});

            });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Fill items to list           
            actIndicator.IsRunning = true;
            await PrepareListItems();
            actIndicator.IsRunning = false;
            //Assing item list to ListTemplate
            AboutList.SetItemsSource(itemList);

        }

        protected override void OnDisappearing()
        {            
            base.OnDisappearing();

        }
    }
}