using System;
using System.Collections.ObjectModel;
using NurApiDotNet;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Diagnostics;

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
        }
        
        protected override void OnAppearing()
        {
            App.BindStatusMessage(MyStatusBar);

            Debug.WriteLine(" DeviceSelectPage OnAppearing");
            App.NurDeviceSearch.DeviceDiscoveredEvent += OnDeviceDiscovered;
           
            base.OnAppearing();
            
            try
            {
                App.NurDeviceSearch.StartDeviceDiscovery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception:" + e.Message);                
            }
                        
            string tcpValue = Preferences.Get("lastTCP", "default");
            if (tcpValue != "default")
            {
                Items.Add(tcpValue);
                UriList.Add(new Uri(tcpValue));
            }
        }

        protected override void OnDisappearing()
        {
            Debug.WriteLine(" DeviceSelectPage OnDisappearing");
            App.NurDeviceSearch.StopDeviceDiscovery();          
            App.NurDeviceSearch.DeviceDiscoveredEvent -= OnDeviceDiscovered;
            
            base.OnDisappearing();
        }

        public void OnDeviceDiscovered(IDeviceDiscover instance, Uri uri, bool visible)
        {
            Debug.WriteLine("OnDeviceDiscovered " + uri.ToString());
            string name = uri.GetQueryParam("name");
            if (string.IsNullOrEmpty(name))
                name = uri.ToString();

            Items.Add(name);
            UriList.Add(uri);                    
        }

        void OnDisconnectClicked(object sender, EventArgs e)
        {            
            App.Nur.Disconnect();
        }

        async void OnConnectTCPClicked(object sender, EventArgs e)
        {
            
            string result = await DisplayPromptAsync("TCP/IP connection", "tcp://", "OK", "Cancel","192.168.1.123:4333", maxLength: 40, keyboard: Keyboard.Default);

            try
            {
                if (result.StartsWith("Cancel")) return;
                Uri uri = new Uri("tcp://" + result);
                App.Nur.Connect(uri);
                Preferences.Set("lastTCP", uri.ToString());

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
            
            //Just make sure existing connection disconnected
            App.Nur.Disconnect();           

            ConnectedDevice = UriList[e.ItemIndex];
                       
            App.Nur.Connect(ConnectedDevice);            

            //Deselect Item
            ((Xamarin.Forms.ListView)sender).SelectedItem = null;
        }
    }
}