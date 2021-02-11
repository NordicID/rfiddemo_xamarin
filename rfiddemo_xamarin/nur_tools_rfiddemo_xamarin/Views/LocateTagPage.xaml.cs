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
            
            progressFound.GaugeValueTextColor = Color.Black;
            progressFound.GaugeRadialWidth = 50;
            progressFound.RangeMax = 100;
            progressFound.RangeMin = 0;
            progressFound.GaugeUnitTextColor = Color.DarkRed;
            progressFound.GaugeUnitText = "%";

            try
            {
                locateTag.OnLocateTag += LocateTag_OnLocateTag;
                locateTag.Start(epcBuf);                
            }
            catch(Exception ex)
            {
                DisplayAlert("Operation Failed!", ex.Message, "OK");
            }

            // After tapping EPC text label, user can edit EPC to locate
            var editEPCTap = new TapGestureRecognizer();
            editEPCTap.Tapped += async (s, e) =>
            {
                try
                {
                    locateTag.Stop();
                    string result = await DisplayPromptAsync("EPC to locate", "(Hex string as word boundary)", initialValue: epcText.Text, maxLength: 64, keyboard: Keyboard.Text);
                    if(result != null)
                    {
                        //set new EPC to locate
                        epcText.Text = result;
                    }

                    locateTag.Start(NurApi.HexStringToBin(epcText.Text)); 
                }
                catch (Exception ex) 
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            };
            epcText.GestureRecognizers.Add(editEPCTap);
        }

        private void LocateTag_OnLocateTag(object sender, LocateTagEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (e.pros < 30) progressFound.GaugeColor = Color.Red;
                else if (e.pros >= 30 && e.pros < 70) progressFound.GaugeColor = Color.Orange;
                else progressFound.GaugeColor = Color.Green;

                progressFound.SetProgress(e.pros);
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