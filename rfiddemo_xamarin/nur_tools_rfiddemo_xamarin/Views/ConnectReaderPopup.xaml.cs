using System;
using nur_tools_rfiddemo_xamarin.Templates;
using NurApiDotNet;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace nur_tools_rfiddemo_xamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectReaderPopup : PopupPage
    {      
        public ConnectReaderPopup()
        {
            InitializeComponent();
            string extReader = Preferences.Get("ExtReader", "");
            if(!string.IsNullOrEmpty(extReader))                            
                readerSelect.SetExternalReaderIPPort(extReader);                            

            readerSelect.OnSelectedReaderUri += ReaderSelect_OnSelectedReaderUri;
        }

        private async void ConnectToUriAsync(Uri uri)
        {
            string name = uri.GetQueryParam("name");
            if (name.StartsWith("External reader"))
            {
                string result = await DisplayPromptAsync("Socket connection", "tcp://", "OK", "Cancel", "", 40, Xamarin.Forms.Keyboard.Default, uri.Host+":"+uri.Port.ToString());

                try
                {
                    if (result.StartsWith("Cancel")) return;
                    Uri newUri = new Uri("tcp://" + result + "/?name=External reader");                    
                    App.Nur.Connect(newUri);
                    Preferences.Set("ExtReader", newUri.Host + ":" + newUri.Port.ToString());                   
                    return;

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Operation failed!", ex.Message, "OK");
                    return;
                }
            }
            else
                App.Nur.Connect(uri);
        }
        private async void ReaderSelect_OnSelectedReaderUri(object sender, SelectedReaderUriEventArgs e)
        {            
            try
            {
                //User select the reader. Try connect.
                if (App.Nur.IsConnected())
                {
                    if (App.Nur.ConnectedDeviceUri.Equals(e.selectedUri))
                    {
                        //Device clicked already connected. Ask for just disconnect.
                        string name = App.Nur.ConnectedDeviceUri.GetQueryParam("name");
                        bool answer = await DisplayAlert(name, "Would you like to disconnect ?", "Yes", "No");
                        if (answer)
                        {
                            App.Nur.Disconnect();
                            readerSelect.Refresh();
                            readerSelect.StartDiscovery();
                            return;
                        }
                    }
                    else
                    {
                        //Disconnect existing connection and try connect to selected device
                        App.Nur.Disconnect();                        
                    }
                }
                
                ConnectToUriAsync(e.selectedUri);
                CloseAllPopup();                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }

        }

        private void OnStop(object sender, EventArgs e)
        {
            CloseAllPopup();
        }

        private void OnCloseButtonTapped(object sender, EventArgs e)
        {
            CloseAllPopup();
        }

        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();

            return false;
        }

        private async void CloseAllPopup()
        {
            await PopupNavigation.Instance.PopAllAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            readerSelect.StartDiscovery();
        }

        protected override void OnDisappearing()
        {
            readerSelect.StopDiscovery();
            base.OnDisappearing();
        }

    }
}