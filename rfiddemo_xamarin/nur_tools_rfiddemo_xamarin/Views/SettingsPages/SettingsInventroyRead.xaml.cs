using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class SettingsInventroyRead : ContentPage
    {
        IrInformation mInvReadParams = new IrInformation();

        public SettingsInventroyRead()
        {
            InitializeComponent();
            
        }

        private void PrepareControls()
        {
            if (App.Nur.IsConnected() && App.Nur.Capabilites.HasInventoryRead())
            {
                controlGrid.IsEnabled = true;

                Bank bank = (Bank)mInvReadParams.bank;
                LblBank.Text = bank.ToString();
                
                InventoryReadType itype = (InventoryReadType)mInvReadParams.type;
                LblType.Text = itype.ToString();

                LblStartAddr.Text = mInvReadParams.wAddress.ToString();
                LblWordCount.Text = mInvReadParams.wLength.ToString();                                                                           
            }
            else controlGrid.IsEnabled = false;
        }

        async void OnButtonBankClicked(object sender, EventArgs e)
        {
            string[] arr = Enum.GetNames(typeof(Bank));
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set Bank", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {                
                Array bankValues = Enum.GetValues(typeof(Bank));
                Bank b = (Bank)bankValues.GetValue(levelIndex);
                mInvReadParams.bank = (uint)b;
                LblBank.Text = b.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void OnButtonTypeClicked(object sender, EventArgs e)
        {
            string[] arr = Enum.GetNames(typeof(InventoryReadType));
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set Inventory Type", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                Array values = Enum.GetValues(typeof(InventoryReadType));
                InventoryReadType type = (InventoryReadType)values.GetValue(levelIndex);
                mInvReadParams.type = (uint)type;
                LblType.Text = type.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void OnButtonAddrClicked(object sender, EventArgs e)
        {            
            string result = await DisplayPromptAsync("Set start address", "(word)", "OK", "Cancel", mInvReadParams.wAddress.ToString(), maxLength: 2, keyboard: Keyboard.Numeric);

            //TODO: Validating

            try
            {
                mInvReadParams.wAddress = Convert.ToUInt32(result);
                LblStartAddr.Text = mInvReadParams.wAddress.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void OnButtonwordsClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Set word count", "Num of words to read", "OK", "Cancel", mInvReadParams.wLength.ToString(), maxLength: 2, keyboard: Keyboard.Numeric);

            //TODO: Validating

            try
            {
                mInvReadParams.wLength = Convert.ToUInt32(result);
                LblWordCount.Text = mInvReadParams.wLength.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void OnButtonSaveIRClicked(object sender, EventArgs e)
        {
            try
            {
                App.Nur.SetInventoryRead(mInvReadParams);
                await DisplayAlert("Success", "InventoryRead settings stored to module", "OK");                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Cannot set IR settings!", ex.Message, "OK");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //Need in order to show status messages bottom of the screen
            App.BindStatusMessage(MyStatusBar);

            try
            {
                mInvReadParams = App.Nur.GetInventoryRead();                
            }
            catch(Exception ex)
            {               
                await DisplayAlert("Cannot read IR settings from module!", ex.Message, "OK");
            }

            PrepareControls();            
        }

        protected override void OnDisappearing()
        {
                        
            base.OnDisappearing();
        }

    }
}