using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NurApiDotNet;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace nur_tools_rfiddemo_xamarin.Views.SettingsPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsInventory : ContentPage
    {
        public SettingsInventory()
        {
            InitializeComponent();
        }
        
        private async void PrepareControls()
        {
            if (App.Nur.IsConnected())
            {
                controlGrid.IsEnabled = true;

                try
                {
                    if (App.Nur.Setup.inventoryQ == 0) LblQ.Text = "Auto";
                    else LblQ.Text = App.Nur.Setup.inventoryQ.ToString();

                    if (App.Nur.Setup.inventoryRounds == 0) LblRounds.Text = "Auto";
                    else LblRounds.Text = App.Nur.Setup.inventoryRounds.ToString();

                    InventoryTarget target = (InventoryTarget)App.Nur.Setup.inventoryTarget;
                    LblSession.Text = App.Nur.Setup.inventorySession.ToString();
                    LblTarget.Text = target.ToString();
                    LblEpcLen.Text = App.Nur.Setup.inventoryEpcLength.ToString();

                    if (App.Nur.Setup.readRssiFilter.min == 0) LblRssiReadMin.Text = "Disabled";
                    else LblRssiReadMin.Text = App.Nur.Setup.readRssiFilter.min.ToString() + " dBm";

                    if (App.Nur.Setup.readRssiFilter.max == 0) LblRssiReadMax.Text = "Disabled";
                    else LblRssiReadMax.Text = App.Nur.Setup.readRssiFilter.max.ToString() + " dBm";

                    if (App.Nur.Setup.writeRssiFilter.min == 0) LblRssiWriteMin.Text = "Disabled";
                    else LblRssiWriteMin.Text = App.Nur.Setup.writeRssiFilter.min.ToString() + " dBm";

                    if (App.Nur.Setup.writeRssiFilter.max == 0) LblRssiWriteMax.Text = "Disabled";
                    else LblRssiWriteMax.Text = App.Nur.Setup.writeRssiFilter.max.ToString() + " dBm";

                    if (App.Nur.Setup.inventoryRssiFilter.min == 0) LblRssiInvMin.Text = "Disabled";
                    else LblRssiInvMin.Text = App.Nur.Setup.inventoryRssiFilter.min.ToString() + " dBm";

                    if (App.Nur.Setup.inventoryRssiFilter.max == 0) LblRssiInvMax.Text = "Disabled";
                    else LblRssiInvMax.Text = App.Nur.Setup.inventoryRssiFilter.max.ToString() + " dBm";  

                   
                }
                catch (Exception e)
                {
                    await DisplayAlert("Cannot read setup!", e.Message, "OK");
                }
            }
            else controlGrid.IsEnabled = false;
        }

        private async void OnButtonQClicked(object sender, EventArgs e)
        {
            string[] arr = { "Auto", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set inventory Q", "Cancel", null, arr);            
            int levelIndex = dict.GetItemIndex(action);           
            if (levelIndex == -1) return; //Possible Cancel pressed  

            try
            {
                App.Nur.InventoryQ = levelIndex;            
                LblQ.Text = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void OnButtonRoundsClicked(object sender, EventArgs e) 
        {
            string[] arr = { "Auto", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set inventory rounds", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed  

            try
            {
                App.Nur.InventoryRounds = levelIndex;               
                LblRounds.Text = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void OnButtonSessionClicked(object sender, EventArgs e)
        {
            string[] arr = { "S0", "S1", "S2", "S3"};
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set inventory session", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                App.Nur.InventorySession = levelIndex;                
                LblSession.Text = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void OnButtonTargetClicked(object sender, EventArgs e)
        {
            string[] arr = { "A", "B", "AB" };
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set inventory target", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                App.Nur.InventoryTarget = levelIndex;               
                LblTarget.Text = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void OnButtonEPCLengthClicked(object sender, EventArgs e) 
        {           
            int val;
            
            string result = await DisplayPromptAsync("EPC length filter in bytes (2-255)", "255 = disabled","OK","Cancel", App.Nur.Setup.inventoryEpcLength.ToString(), maxLength: 3, keyboard: Keyboard.Numeric);

            try
            {
                val=ValidateNumericValue(result, 2, 255);
            }
            catch(Exception ex)
            {
                if(ex.Message.StartsWith("Cancel")) return;
                await DisplayAlert("Invalid value", ex.Message, "OK");
                return;
            }                        
                        
            try
            {
                App.Nur.InventoryEpcLength = val;               
                LblEpcLen.Text = val.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void OnButtonRssiFilterReadMin(object sender, EventArgs e) 
        {
            int val;

            string result = await DisplayPromptAsync("RSSI filter min (Read)", "(-90 dBm - 0 dBm) 0 = disabled", "OK", "Cancel", App.Nur.Setup.readRssiFilter.min.ToString(), maxLength: 3, keyboard: Keyboard.Numeric);

            try
            {
                val = ValidateNumericValue(result, -90, 0);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Cancel")) return;
                await DisplayAlert("Invalid value", ex.Message, "OK");
                return;
            }

            try
            {
                App.Nur.Setup.readRssiFilter.min = val; //Set min to current setup
                App.Nur.ReadRssiFilter = App.Nur.Setup.readRssiFilter; //and Write it to module
                                
                if (val == 0) LblRssiReadMin.Text = "Disabled";
                else LblRssiReadMin.Text = val.ToString() + " dBm";                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void OnButtonRssiFilterReadMax(object sender, EventArgs e) 
        {
            int val;

            string result = await DisplayPromptAsync("RSSI filter max (Read)", "(-90 dBm - 0 dBm) 0 = disabled", "OK", "Cancel", App.Nur.Setup.readRssiFilter.max.ToString(), maxLength: 3, keyboard: Keyboard.Numeric);

            try
            {
                val = ValidateNumericValue(result, -90, 0);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Cancel")) return;
                await DisplayAlert("Invalid value", ex.Message, "OK");
                return;
            }

            try
            {
                App.Nur.Setup.readRssiFilter.max = val;
                App.Nur.ReadRssiFilter = App.Nur.Setup.readRssiFilter; //and Write it to module
               
                if (val == 0) LblRssiReadMax.Text = "Disabled";
                else LblRssiReadMax.Text = val.ToString() + " dBm";                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void OnButtonRssiFilterWriteMin(object sender, EventArgs e) 
        {
            int val;

            string result = await DisplayPromptAsync("RSSI filter min (Write)", "(-90 dBm - 0 dBm) 0 = disabled", "OK", "Cancel", App.Nur.Setup.writeRssiFilter.min.ToString(), maxLength: 3, keyboard: Keyboard.Numeric);

            try
            {
                val = ValidateNumericValue(result, -90, 0);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Cancel")) return;
                await DisplayAlert("Invalid value", ex.Message, "OK");
                return;
            }

            try
            {
                App.Nur.Setup.writeRssiFilter.min = val;
                App.Nur.WriteRssiFilter = App.Nur.Setup.writeRssiFilter; //and Write it to module
               
                if (val == 0) LblRssiWriteMin.Text = "Disabled";
                else LblRssiWriteMin.Text = val.ToString() + " dBm";               
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void OnButtonRssiFilterWriteMax(object sender, EventArgs e) 
        {
            int val;

            string result = await DisplayPromptAsync("RSSI filter max (Write)", "(-90 dBm - 0 dBm) 0 = disabled", "OK", "Cancel", App.Nur.Setup.writeRssiFilter.max.ToString(), maxLength: 3, keyboard: Keyboard.Numeric);

            try
            {
                val = ValidateNumericValue(result, -90, 0);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Cancel")) return;
                await DisplayAlert("Invalid value", ex.Message, "OK");
                return;
            }

            try
            {
                App.Nur.Setup.writeRssiFilter.max = val;
                App.Nur.WriteRssiFilter = App.Nur.Setup.writeRssiFilter; //and Write it to module
                
                if (val == 0) LblRssiWriteMax.Text = "Disabled";
                else LblRssiWriteMax.Text = val.ToString() + " dBm";                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void OnButtonRssiFilterInventoryMin(object sender, EventArgs e)
        {
            int val;

            string result = await DisplayPromptAsync("RSSI filter min (Inventory)", "(-90 dBm - 0 dBm) 0 = disabled", "OK", "Cancel", App.Nur.Setup.inventoryRssiFilter.min.ToString(), maxLength: 3, keyboard: Keyboard.Numeric);

            try
            {
                val = ValidateNumericValue(result, -90, 0);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Cancel")) return;
                await DisplayAlert("Invalid value", ex.Message, "OK");
                return;
            }

            try
            {
                App.Nur.Setup.inventoryRssiFilter.min = val;
                App.Nur.InventoryRssiFilter = App.Nur.Setup.inventoryRssiFilter; //and Write it to module
               
                if (val == 0) LblRssiInvMin.Text = "Disabled";
                else LblRssiInvMin.Text = val.ToString() + " dBm";               
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
        private async void OnButtonRssiFilterInventoryMax(object sender, EventArgs e)
        {
            int val;

            string result = await DisplayPromptAsync("RSSI filter max (Inventory)", "(-90 dBm - 0 dBm) 0 = disabled", "OK", "Cancel", App.Nur.Setup.inventoryRssiFilter.max.ToString(), maxLength: 3, keyboard: Keyboard.Numeric);

            try
            {
                val = ValidateNumericValue(result, -90, 0);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Cancel")) return;
                await DisplayAlert("Invalid value", ex.Message, "OK");
                return;
            }

            try
            {
                App.Nur.Setup.inventoryRssiFilter.max = val;
                App.Nur.InventoryRssiFilter = App.Nur.Setup.inventoryRssiFilter; //and Write it to module
                if (val == 0) LblRssiInvMax.Text = "Disabled";
                else LblRssiInvMax.Text = val.ToString() + " dBm";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        private int ValidateNumericValue(string result,int min,int max)
        {
            int val = 0;

            if (string.IsNullOrEmpty(result))
                throw new Exception("Cancel or no value");

            try
            {
                val = Convert.ToInt32(result);
                if (val < min || val > max)
                    throw new Exception("Value not in range. Must be " + min.ToString() + "-" + max.ToString());

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return val;
        }

        protected override void OnAppearing()
        {          
            base.OnAppearing();

            //Need in order to show status messages bottom of the screen
            App.BindStatusMessage(MyStatusBar);

            PrepareControls();           
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

    }
}