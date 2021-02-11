using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NurApiDotNet;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static NurApiDotNet.NurApi;

namespace nur_tools_rfiddemo_xamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarcodePage : ContentPage
    {       
        private static bool isReading;
        private static bool isTriggerDown;
        private static bool justRelease;

        public BarcodePage()
        {
            InitializeComponent();           

            //Flags to indicate state
            isReading = false;           
            isTriggerDown = false;
            justRelease = false;            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Need in order to show status messages bottom of the screen
            App.BindStatusMessage(MyStatusBar);
            
            //IOChange event needed to catch button presses from the reader
            App.Nur.IOChangeEvent += Nur_IOChangeEvent;

            //Event fires when barcode read copmpleted. Success or not.
            App.Nur.OnAccBarcodeResult += Nur_OnAccBarcodeResult;            
        }

        private void Nur_OnAccBarcodeResult(object sender, AccBarcodeResult e)
        {
            Debug.WriteLine("OnAccBarcodeResult status = " + e.status.ToString() + " Barcode=" + e.Barcode);

            isReading = false;

            if (e.status == BarcodeReadStatus.Success)
            {
                try
                {
                    UpdateStatusText(e.Barcode);
                    App.ShowShortStatusMessage("Success!", 1, Color.Green, Color.LightGray);                   
                    App.Nur.AccBeep(100);
                }
                catch (Exception) { }
            }
            else
            {
                App.ShowShortStatusMessage(e.status.ToString(), 1, Color.Red, Color.LightGray);
            }
        }

        private void Nur_IOChangeEvent(object sender, IOChangeEventArgs e)
        {
            AccessorySensorSource source = (AccessorySensorSource)e.data.source;
            Debug.WriteLine("IOChangeEvent source=" + source.ToString() + " Dir=" + e.data.dir + " sensor=" + e.data.sensor.ToString());

            try
            {
                if (source == AccessorySensorSource.ButtonTrigger)
                {
                    if (e.data.dir == 0)
                    {
                        //Trigger released.
                        isTriggerDown = false;
                        if (justRelease)
                        {
                            justRelease = false;
                            return; //Just trigger release after cancel
                        }

                        //Start reading barcode. Possible aimer will be shutdown automatically                      
                        App.Nur.AccBarcodeStart(4000);
                        App.ShowShortStatusMessage("Reading...", 4, Color.Blue, Color.LightGray);
                        isReading = true;

                    }
                    else
                    {
                        //Trigger down
                        isTriggerDown = true;
                        if (isReading)
                        {
                            //Reading pending. User want to cancel..                            
                            App.Nur.AccBarcodeCancel();
                            App.ShowShortStatusMessage("Cancelled...", 1, Color.Red, Color.LightGray);
                            isReading = false;
                            justRelease = true;
                            return;
                        }

                        if (isReading == false)
                        {
                            //if user keep trigger button down at least 500ms, then activate aiming beam
                            CheckIfNeedToAim(500);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("IOChangeEvent exception=" + ex.Message);
            }
        }

        private async void CheckIfNeedToAim(int timeoutInSeconds)
        {
                await Task.Run(() =>
                {
                    try
                    {
                        Thread.Sleep(timeoutInSeconds);

                        if (isTriggerDown == true && isReading == false) //If trigger still down after timeout
                        {
                            App.Nur.AccImagerAim(true);
                            App.ShowShortStatusMessage("Aiming...", 4, Color.Blue, Color.LightGray);
                        }
                    }
                    catch (Exception)
                    {
                        // Handle error here
                    }
                });
        }

        private void UpdateStatusText(string status)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                statusTxt.Text = status;
            });
        }

        private void OnStartClicked(object sender, EventArgs e)
        {
            if (isReading)
            {
                //Cancel reading
                App.Nur.AccBarcodeCancel();
                App.ShowShortStatusMessage("Cancelled...", 1, Color.Red, Color.LightGray);
                isReading = false;
            }
            else
            {
                //Start reading barcode. Possible aimer will be shutdown automatically                      
                App.Nur.AccBarcodeStart(4000);
                App.ShowShortStatusMessage("Reading...", 4, Color.Blue, Color.LightGray);
                isReading = true;
            }
        }

        protected override void OnDisappearing()
        {
            
            base.OnDisappearing();
            App.Nur.IOChangeEvent -= Nur_IOChangeEvent;
            App.Nur.OnAccBarcodeResult -= Nur_OnAccBarcodeResult;
        }
    }
}