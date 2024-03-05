using NurApiDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace nur_tools_rfiddemo_xamarin.Templates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReaderDiscoveryAndSelect : Frame
    {
        string _extReaderIPPort = "192.168.1.123:4333"; //Default

        readonly ObservableCollection<ListItem> itemList = new ObservableCollection<ListItem>();

        //Collection of device Uri's user can select to connect. Key to dictionary  will be "<device name>:<host address>"
        Dictionary<string, Uri> _uriList;

        ListItemStyle style;

        public event EventHandler<SelectedReaderUriEventArgs> OnSelectedReaderUri;

        public ReaderDiscoveryAndSelect()
        {
            InitializeComponent();

            _uriList = new Dictionary<string, Uri>();
            ReaderList.OnItemSelected += ReaderList_OnItemSelected;
            style = new ListItemStyle("connection.png", 40, Color.FromRgb(255, 255, 255), Color.DarkBlue, Color.Black);
            style.styleCellHeight = 60;
            style.styleSingleRow = false;

            Refresh();
        }

        private void ReaderList_OnItemSelected(object sender, EventArgs e)
        {
            ItemTappedEventArgs arg = (ItemTappedEventArgs)e;
            ListItem item = (ListItem)arg.Item;
            lock (_uriList)
            {
                if (arg.ItemIndex < _uriList.Count)
                {
                    SelectedReaderUriEventArgs args = new SelectedReaderUriEventArgs(_uriList.ElementAt(arg.ItemIndex).Value);
                    OnSelectedReaderUri?.Invoke(this, args);
                }
            }
        }

        /// <summary>
        /// App may set "<ip>;<port>" as socket address of external reader and user can select from list. Then popup open to edit ip and port.
        /// </summary>
        /// <param name="ipPort">example: "192.168.1.147:4333"</param>
        public void SetExternalReaderIPPort(string ipPort)
        {
            _extReaderIPPort = ipPort;
        }

        /// <summary>
        /// Add new Uri to List.
        /// </summary>
        /// <param name="uri">Uri of device</param>
        /// <param name="name">name of device</param>
        /// <returns>true if Uri added to list. false if already in list</returns>
        public bool AddUriToList(Uri uri, string name)
        {
            lock (_uriList)
            {
                string key = name + uri.Host;
                //Add Uri only if not already there..
                if (!_uriList.ContainsKey(key))
                {
                    _uriList.Add(key, uri);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Refresh devicelist.
        /// </summary>
        public void Refresh()
        {
            lock (_uriList)
                _uriList.Clear();

            lock (itemList)
                itemList.Clear();

            Device.BeginInvokeOnMainThread(() =>
            {
                string name = "";
                if (App.Nur.ConnectionStatus == NurTransportStatus.Connected)
                {
                    //Show device which is already connected
                    name = App.Nur.ConnectedDeviceUri.GetQueryParam("name");
                    if (string.IsNullOrEmpty(name)) name = App.Nur.ConnectedDeviceUri.GetAddress();
                    style.styleValueColor = Color.Green;
                    itemList.Add(new ListItem(style, name, App.Nur.ConnectedDeviceUri.Host + " (Connected)"));
                    style.styleValueColor = Color.Black;
                    AddUriToList(App.Nur.ConnectedDeviceUri, name);
                }

                try
                {
                    //Add possibility to create socket connection in to the External reader.
                    //User can edit IP and Port.
                    Uri myUri = new Uri("tcp://" + _extReaderIPPort + "/?name=External reader");
                    if (AddUriToList(myUri, "External reader")) // add only if not already connected and in UriList
                        itemList.Add(new ListItem(style, "Connect to external reader (socket)", _extReaderIPPort));
                }
                catch (Exception) { }


            });

            ReaderList.SetItemsSource(itemList);
        }

        public void StartDiscovery()
        {
            try
            {
                NurDeviceDiscovery.Start(OnDeviceDiscovered);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception:" + e.Message);
            }
        }

        public void StopDiscovery()
        {
            NurDeviceDiscovery.Stop(OnDeviceDiscovered);
        }

        private void OnDeviceDiscovered(object sender, NurDeviceDiscoveryEventArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Uri uri = args.Uri;

                System.Diagnostics.Debug.WriteLine("OnDeviceDiscovered " + uri.ToString() + " Visible " + args.Visible);

                string name = uri.GetQueryParam("name");
                if (string.IsNullOrEmpty(name))
                    name = uri.GetAddress();

                if (args.Visible)
                {
                    System.Diagnostics.Debug.WriteLine("OnDeviceDiscovered ADD " + name);
                    if (!_uriList.ContainsKey(name + uri.Host))
                    {
                        string txt = uri.GetAddress();
                        if (App.Nur.IsConnected() && App.Nur.ConnectedDeviceUri.Equals(uri))
                        {
                            style.styleValueColor = Color.Green;
                            txt += " (Connected)";
                        }
                        else
                        {
                            itemList.Add(new ListItem(style, name, txt));
                            AddUriToList(uri, name);
                            style.styleValueColor = Color.Black;
                        }
                    }
                }
                else
                {
                    //At this point if device is not visible any more, we should remove it from the list. But we dont't do that here. List will be refreshed when reopening.
                    System.Diagnostics.Debug.WriteLine("OnDeviceDiscovered Not visible " + name);
                }

                ReaderList.SetItemsSource(itemList);
            });
        }
    }

    public class SelectedReaderUriEventArgs
    {
        /// <summary>
        /// SelectedReadUri event arg
        /// </summary>
        /// <param name="uri">Selected reader Uri</param>      

        public SelectedReaderUriEventArgs(Uri uri)
        {
            this.selectedUri = uri;
        }

        /// <summary>
        /// Selected Uri of reader
        /// </summary>
        public Uri selectedUri;

    }
}