using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NurApiDotNet;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static NurApiDotNet.NurApi;

namespace nur_tools_rfiddemo_xamarin.Views.SettingsPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsAntenna : ContentPage
    {
        public SettingsAntenna()
        {
            InitializeComponent();                                      
        }

        private async void AddAntennaSwitces()
        {
            try
            {
                AntTable.Clear();
                                
                for (int x = 0; x < App.AntennaList.Count; x++)
                {
                    SwitchCell antCell = new SwitchCell();
                    antCell.BindingContext = App.AntennaList[x];
                    antCell.Text = App.AntennaList[x].Name;
                    antCell.On = App.Nur.IsPhysicalAntennaEnabled(App.AntennaList[x].Name);
                    antCell.OnChanged += AntCell_OnChanged;

                    AntTable.Add(antCell);
                }
            }catch(Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
            
        }

        private void AntCell_OnChanged(object sender, ToggledEventArgs e)
        {
            SwitchCell at = (SwitchCell)sender;
            AntennaMapping ant = (AntennaMapping)at.BindingContext;
            if (e.Value == true) App.Nur.EnablePhysicalAntenna(ant.Name);
            else App.Nur.DisablePhysicalAntenna(ant.Name);
        }

        async void OnButtonAutoTuneSetupClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Sorry", "Not implemented", "OK");         

            return;
            /*string[] arr = Enum.GetNames(typeof(AutotuneSetup.Mode));

            string action = await DisplayActionSheet("AutoTune setup", "Cancel", null, arr);
            IndexDictionary dict = new IndexDictionary(arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed    

            try
            {
                AutotuneSetup setup = new AutotuneSetup();
                setup.mode = (byte)levelIndex;
                
                if (setup.mode == AUTOTUNE_MODE_THRESHOLD_ENABLE)
                {
                    string result = await DisplayPromptAsync("Set threshold (dBm)", "(0-40)", "OK", "Cancel", App.Nur.Setup.autotune.threshold_dBm.ToString(), maxLength: 2, keyboard: Keyboard.Numeric);
                  
                    try
                    {
                        setup.threshold_dBm = (sbyte)Utils.ValidateAndConvertToInt(result, 0, 40);
                        action = "Threshold " + setup.threshold_dBm.ToString(); //TODO: notwork
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.StartsWith("Cancel")) return;
                        await DisplayAlert("Invalid value", ex.Message, "OK");
                        return;
                    }
                }
                //Write to module
                App.Nur.Autotune = setup;                
                LblAutoTuneSetup.Text = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }*/
        }

        async void OnTuneClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Sorry", "Not implemented", "OK");
        }

        async void OnMeasRefPwrClicked(object sender, EventArgs e)
        {
            double refPower = App.Nur.GetReflectedPowerValue();
           await DisplayAlert("Reflected power", refPower.ToString() + " dBm", "OK");
        }

        private void PrepareControls()
        {
            AddAntennaSwitces();
            if (App.Nur.Autotune.mode == (byte)AutotuneSetup.Mode.ThresholdEnable)
                LblAutoTuneSetup.Text = "Threshold " + App.Nur.Setup.autotune.threshold_dBm.ToString();
            else LblAutoTuneSetup.Text = App.Nur.Setup.autotune.mode.ToString();
        }

        protected override void OnAppearing()
        {       
            base.OnAppearing();

            //Need in order to show status messages bottom of the screen
            App.BindStatusMessage(MyStatusBar);

            App.Nur.TuneEvent += Nur_TuneEvent;
            PrepareControls();           
        }

        private void Nur_TuneEvent(object sender, NurApi.TuneEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            App.Nur.TuneEvent -= Nur_TuneEvent;
        }

       
    }
}