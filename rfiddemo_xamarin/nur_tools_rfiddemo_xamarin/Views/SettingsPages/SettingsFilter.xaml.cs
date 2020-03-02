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
    public partial class SettingsFilter : ContentPage
    {
        public SettingsFilter()
        {
            InitializeComponent();
            
        }

        private void PrepareControls()
        {
            SwitchEnable.IsToggled = App.IsExtFilterEnabled;
            LblEnabled.Text = SwitchEnable.IsToggled.ToString();

            FilterAction action = (FilterAction)App.InvExtFilter.action;
            LblAction.Text = action.ToString();

            Bank bank =  (Bank)App.InvExtFilter.bank;
            LblBank.Text = bank.ToString();
            TargetSession target = (TargetSession)App.InvExtFilter.target;
            LblTarget.Text = target.ToString();
            LblAddr.Text = App.InvExtFilter.address.ToString();


            if(App.InvExtFilter.maskData != null)
                LblMask.Text = NurApi.BinToHexString(App.InvExtFilter.maskData);
            LblLength.Text = App.InvExtFilter.maskBitLength.ToString();           
        }

        void OnFilterEnableSwitch(object sender, EventArgs e)
        {
            LblEnabled.Text = SwitchEnable.IsToggled.ToString();
            App.IsExtFilterEnabled = SwitchEnable.IsToggled;
        }

        async void OnButtonActionClicked(object sender, EventArgs e)
        {
            /*
            string[] arr = new string[8];
            
            arr[0] = "assert SL or inventoried session flag -> A.Non - matching: deassert SL or inventoried session flag -> B. (action = 0)";
            arr[1] = "assert SL or inventoried session flag -> A.Non - matching: do nothing. (action = 1)";
            arr[2] = "do nothing.Non - matching: deassert SL or inventoried session->B. (action = 2)";
            arr[3] = "negate SL or invert inventoried session flag(A->B, B->A).Non - matching: do nothing. (action = 3)";
            arr[4] = "deassert SL or inventoried session flag->B.Non - matching: assert SL or inventoried session flag -> A. (action = 4)";
            arr[5] = "deassert SL or inventoried session flag -> B.Non - matching: do nothing. (action = 5)";
            arr[6] = "do nothing.Non - matching: assert SL or inventoried session flag -> A. (action = 6)";
            arr[7] = "do nothing.Non - matching: negate SL or invert inventoried session flag(A->B, B->A). (action = 7)";
            */
                        
            string[] arr = Enum.GetNames(typeof(FilterAction)); //Just name selection as Action0, Action1...
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set Filter action", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                Array actionValues = Enum.GetValues(typeof(FilterAction));
                FilterAction b = (FilterAction)actionValues.GetValue(levelIndex);
                App.InvExtFilter.action = (byte)b;
                LblAction.Text = b.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
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
                App.InvExtFilter.bank = (byte)b;
                LblBank.Text = b.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void OnButtonTargetClicked(object sender, EventArgs e)
        {
            string[] arr = Enum.GetNames(typeof(TargetSession));
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set Target", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                Array targetValues = Enum.GetValues(typeof(TargetSession));
                TargetSession b = (TargetSession)targetValues.GetValue(levelIndex);
                App.InvExtFilter.target = (byte)b;
                LblTarget.Text = b.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void OnButtonAddrClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Set address", "(bits)", "OK", "Cancel", App.InvExtFilter.address.ToString(), maxLength: 3, keyboard: Keyboard.Numeric);

            if (result == "Cancel") return;
            if (string.IsNullOrEmpty(result)) return;

            try
            {
                App.InvExtFilter.address = Convert.ToUInt32(result);
                LblAddr.Text = App.InvExtFilter.address.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void OnButtonMaskClicked(object sender, EventArgs e)
        {
            string placeHolder = "";
            if(App.InvExtFilter.maskData != null)
            {
                placeHolder = NurApi.BinToHexString(App.InvExtFilter.maskData);
            }
            string result = await DisplayPromptAsync("Set Mask", "(hex)", "OK", "Cancel", placeHolder, maxLength: 20, keyboard: Keyboard.Text);

            if (result == "Cancel") return;
            if (string.IsNullOrEmpty(result)) return;

            try
            {
                App.InvExtFilter.maskData = NurApi.HexStringToBin(result);                               
                LblMask.Text = result;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void OnButtonLengthClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Set Length", "(bits)", "OK", "Cancel", App.InvExtFilter.maskBitLength.ToString(), maxLength: 3, keyboard: Keyboard.Numeric);

            if (result == "Cancel") return;
            if (string.IsNullOrEmpty(result)) return;

            try
            {
                App.InvExtFilter.maskBitLength = Convert.ToInt32(result);
                LblLength.Text = App.InvExtFilter.maskBitLength.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
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