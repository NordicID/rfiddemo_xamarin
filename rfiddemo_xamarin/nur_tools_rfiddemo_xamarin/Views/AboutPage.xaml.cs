using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NurApiDotNet;
using NurApiDotNet.Support;
using System.Collections.ObjectModel;
using nur_tools_rfiddemo_xamarin.Templates;
using Xamarin.Essentials;

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

        private void PrepareListItems()
        {            
            itemList.Clear();

            Device.BeginInvokeOnMainThread(() =>
            {
                ListItemStyle style = new ListItemStyle("", 5, Color.FromRgb(225, 225, 220), Color.Black, Color.Blue);
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

                }
                else
                {
                    style.styleBkColor = Color.Red;
                    itemList.Add(new ListItem(style, "Reader not connected", ""));
                }


            });
                                    
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Fill items to list
            PrepareListItems();

            //Assing item list to ListTemplate
            AboutList.SetItemsSource(itemList);

        }

        protected override void OnDisappearing()
        {            
            base.OnDisappearing();

        }
    }
}