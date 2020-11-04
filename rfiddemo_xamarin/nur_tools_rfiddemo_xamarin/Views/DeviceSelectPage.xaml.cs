using System;
using System.Collections.ObjectModel;
using NurApiDotNet;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Diagnostics;
using Android.App.Admin;

namespace nur_tools_rfiddemo_xamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceSelectPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }
        Collection<Uri> UriList { get; set; }
              
        Uri ConnectedDevice;

        public DeviceSelectPage()
        {
            InitializeComponent();                 
            
            Items = new ObservableCollection<string>();
            UriList = new Collection<Uri>();

            MyListView.ItemsSource = Items;

            NurDeviceDiscovery.ErrorEvent += NurDeviceDiscovery_ErrorEvent;
        }

        private void NurDeviceDiscovery_ErrorEvent(object sender, string e)
        {
            Debug.WriteLine("NurDeviceDiscovery_ErrorEvent = " + e);
        }

        protected override void OnAppearing()
        {
            App.BindStatusMessage(MyStatusBar);

            Debug.WriteLine(" DeviceSelectPage OnAppearing");

            Items.Clear();
            UriList.Clear();
           
            base.OnAppearing();
            
            try
            {
                NurDeviceDiscovery.Start(OnDeviceDiscovered);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception:" + e.Message);                
            }
                        
            string tcpValue = Preferences.Get("lastTCP", "default");
            if (tcpValue != "default")
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Items.Add(tcpValue);
                });

                UriList.Add(new Uri(tcpValue));
            }

            App.ShowShortStatusMessage("Discovering devices..",100,Color.Yellow,Color.Black);
        }

        private void OnDeviceDiscovered(object sender, NurDeviceDiscoveryEventArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Uri uri = args.Uri;

                Debug.WriteLine("OnDeviceDiscovered " + uri.ToString() + " Visible " + args.Visible);

                string name = uri.GetQueryParam("name");
                if (string.IsNullOrEmpty(name))
                    name = uri.ToString();

                if (args.Visible)
                {
                    Debug.WriteLine("OnDeviceDiscovered ADD " + name);

                    Items.Add(name);
                    UriList.Add(uri);
                }
                else
                {
                    Debug.WriteLine("OnDeviceDiscovered REMOVE " + name);

                    Items.Remove(name);
                    UriList.Remove(uri);
                }
            });
        }

        protected override void OnDisappearing()
        {
            Debug.WriteLine(" DeviceSelectPage OnDisappearing");
            NurDeviceDiscovery.Stop(OnDeviceDiscovered);
            App.UpdateTransportStatusBarText(MyStatusBar);
            base.OnDisappearing();
        }
               
        void OnDisconnectClicked(object sender, EventArgs e)
        {            
            App.Nur.Disconnect();
            App.ShowShortStatusMessage("Discovering devices..", 100, Color.Yellow, Color.Black);
        }
                         
        async void OnConnectIntegratedReaderClicked(object sender, EventArgs e)
        {                       
            try
            {
                //Trying connect to integrated reader..
                string addr = "tcp://127.0.0.1:6734/?name=" + DeviceInfo.Model;
                Uri uri = new Uri(addr);
                App.Nur.Connect(uri);
                Preferences.Set("lastTCP", uri.ToString());
                App.ShowShortStatusMessage("Connecting to " + uri.ToString(), 100, Color.Yellow, Color.Black);

            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
                return;
            }
        }

        async void OnConnectTCPClicked(object sender, EventArgs e)
        {
            
            string result = await DisplayPromptAsync("TCP/IP connection", "tcp://", "OK", "Cancel","192.168.1.123:4333", maxLength: 40, keyboard: Keyboard.Default,"");

            try
            {
                if (result.StartsWith("Cancel")) return;
                Uri uri = new Uri("tcp://" + result);
                App.Nur.Connect(uri);
                Preferences.Set("lastTCP", uri.ToString());
                App.ShowShortStatusMessage("Connecting to " + uri.ToString(), 100, Color.Yellow, Color.Black);

            }
            catch (Exception ex)
            {               
                await DisplayAlert("Operation failed!", ex.Message, "OK");
                return;
            }
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            lock (this)
            {
                try
                {
                    //Just make sure existing connection disconnected
                    App.Nur.Disconnect();

                    ConnectedDevice = UriList[e.ItemIndex];

                    App.Nur.Connect(ConnectedDevice);
                    App.ShowShortStatusMessage("Connecting to " + ConnectedDevice.ToString(), 100, Color.Yellow, Color.Black);

                    //Deselect Item
                    ((Xamarin.Forms.ListView)sender).SelectedItem = null;
                } catch
                {

                }
            }
        }
    }
}