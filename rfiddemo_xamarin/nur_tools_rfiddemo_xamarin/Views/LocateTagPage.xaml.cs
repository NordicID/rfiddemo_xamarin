using System;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;
using NurApiDotNet;
using NordicID.NurApi.Support;
using Xamarin.Forms.Xaml;
using System.Diagnostics;

namespace nur_tools_rfiddemo_xamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocateTagPage : PopupPage
    {
        LocateTag locateTag;

        public LocateTagPage(byte [] epcBuf)
        {
            InitializeComponent();

            locateTag = new LocateTag(App.Nur);
            epcText.Text = NurApi.BinToHexString(epcBuf);
            try
            {
                locateTag.OnLocateTag += LocateTag_OnLocateTag;
                locateTag.Start(epcBuf);                
            }
            catch(Exception ex)
            {
                DisplayAlert("Operation Failed!", ex.Message, "OK");
            }
        }

        private void LocateTag_OnLocateTag(object sender, LocateTagEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                foundPros.Text = e.pros.ToString() + " %";
            });
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

        protected override void OnDisappearing()
        {
            try
            {
                locateTag.Stop();
            }
            catch (Exception) { }

            base.OnDisappearing();

            locateTag.OnLocateTag -= LocateTag_OnLocateTag;
           
            Debug.WriteLine("Stop LocateTag");


        }
    }
}