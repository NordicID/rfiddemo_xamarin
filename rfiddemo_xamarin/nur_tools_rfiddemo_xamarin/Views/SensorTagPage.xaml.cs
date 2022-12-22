using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using nur_tools_rfiddemo_xamarin.Models;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NurApiDotNet;
using System.Threading;
using System.Threading.Tasks;
using NurApiDotNet.SensorTag;

namespace nur_tools_rfiddemo_xamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorTagPage : ContentPage
    {
        ObservableCollection<SensorTagDetails> sensorTagDetails = new ObservableCollection<SensorTagDetails>();
        readonly SensorTag sensorTag;
        
        public SensorTagPage()
        {
            InitializeComponent();
            
            sensorTag = new SensorTag(App.Nur);           
        }

        async void OnItemTapped(object sender, ItemTappedEventArgs args)
        {
            SensorTagDetails item = sensorTagDetails[args.ItemIndex];

            if (item == null)
                return;

            //You can do something with selected sensor tag
            await DisplayAlert("Clicked!", item.SensorTagItem.epc, "OK");

        }

        async void OnStartInventoryClicked(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    App.ShowShortStatusMessage("Reading...", 4, Color.White, Color.Black);
                    List<SensorTagItem> sTags = sensorTag.ReadSensorTagsMagnus();

                    //Update tags to list
                    foreach (SensorTagItem tag in sTags)
                    {
                        AddOrUpdateObservable(tag);
                        try
                        {
                            App.Nur.AccBeep(10); //Give short beep to reader
                        }
                        catch (Exception) { } //It ok if accessory not supported by reader..
                    }

                    if (sTags.Count > 0)
                        App.ShowShortStatusMessage("Found " + sTags.Count.ToString() + " sensor tags", 2, Color.White, Color.Green);
                    else App.ShowShortStatusMessage("No tags found!", 2, Color.Red, Color.White);
                }
                catch (Exception ex)
                {
                    App.ShowShortStatusMessage(ex.Message, 2, Color.Red, Color.White);
                }
            });
        }

        async void OnClearInventoryClicked(object sender, EventArgs e)
        {
            try
            {                
                sensorTagDetails.Clear();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        private void UpdateSensorTagDetails(SensorTagDetails item, SensorTagItem sensorTagitem)
        {
            item.Code = sensorTagitem.epc;
            item.SensorType = sensorTagitem.model.ToString();
            item.Value = sensorTagitem.temperature.ToString("#.##");
            item.Lastseen = sensorTagitem.TimeStamp.ToLongTimeString();
            item.SensorTagItem = sensorTagitem;
        }
        
        private void AddOrUpdateObservable(SensorTagItem sensorTagitem)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var item = sensorTagDetails.FirstOrDefault(i => i.Code == sensorTagitem.epc);
                if (item != null)
                {
                    UpdateSensorTagDetails(item, sensorTagitem);
                }
                else
                {
                    //Add
                    SensorTagDetails detail = new SensorTagDetails();
                    UpdateSensorTagDetails(detail, sensorTagitem);
                    sensorTagDetails.Add(detail);
                }
            });            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Need in order to show status messages bottom of the screen
            App.BindStatusMessage(MyStatusBar);

            SensorTagsView.ItemsSource = sensorTagDetails;            
        }

        protected override void OnDisappearing()
        {            
            base.OnDisappearing();
        }

    }
}