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
    public partial class SettingsInventoryExtended : ContentPage
    {
        public SettingsInventoryExtended()
        {
            InitializeComponent();
        }

        private void PrepareControls()
        {      
            if (App.InvExtParams.Q == 0) LblQ.Text = "Auto";
            else LblQ.Text = App.InvExtParams.Q.ToString();

            if (App.InvExtParams.rounds == 0) LblRounds.Text = "Auto";
            else LblRounds.Text = App.InvExtParams.rounds.ToString();
                                               
            LblSession.Text = App.InvExtParams.session.ToString();
            InventoryTarget target = (InventoryTarget)App.InvExtParams.inventoryTarget;              
            LblTarget.Text = target.ToString();                
            LblTransitTime.Text = App.InvExtParams.transitTime.ToString();
            InventorySelectState sel = (InventorySelectState)App.InvExtParams.inventorySelState;
            LblSelectState.Text =sel.ToString();

            if(App.IsExtFilterEnabled)
                LblFilter.Text = "Enabled";
            else LblFilter.Text = "Disabled";
        }

        async void OnButtonQClicked(object sender, EventArgs e)
        {
            string[] arr = { "Auto", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set inventory Q", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed  

            try
            { 
                App.InvExtParams.Q = levelIndex;
                LblQ.Text = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void OnButtonRoundsClicked(object sender, EventArgs e)
        {
            string[] arr = { "Auto", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set inventory rounds", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed  

            try
            {
                App.InvExtParams.rounds = levelIndex;               
                LblRounds.Text = action;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void OnButtonSessionClicked(object sender, EventArgs e)
        {
            
            string[] arr = Enum.GetNames(typeof(TargetSession));
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set Session", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                Array bankValues = Enum.GetValues(typeof(TargetSession));
                TargetSession t = (TargetSession)bankValues.GetValue(levelIndex);
                App.InvExtParams.session = (int)t;
                LblSession.Text = t.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void OnButtonTransitTimeClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Transit time", "0 = max 1000ms", "OK", "Cancel", App.InvExtParams.transitTime.ToString(), maxLength: 3, keyboard: Keyboard.Numeric);

            if (result == "Cancel") return;
            if (string.IsNullOrEmpty(result)) return;

            try
            {
                App.InvExtParams.transitTime = Convert.ToInt32(result);
                LblTransitTime.Text = App.InvExtParams.transitTime.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void OnButtonFilterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPages.SettingsFilter());
        }

        async void OnButtonTargetClicked(object sender, EventArgs e)
        {
            string[] arr = Enum.GetNames(typeof(InventoryTarget));
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set Inventory Target", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                Array values = Enum.GetValues(typeof(InventoryTarget));
                InventoryTarget tg = (InventoryTarget)values.GetValue(levelIndex);
                App.InvExtParams.inventoryTarget = (int)tg;
                LblTarget.Text = tg.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void OnButtonSelectStateClicked(object sender, EventArgs e)
        {
            string[] arr = Enum.GetNames(typeof(InventorySelectState));
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Select state", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                Array values = Enum.GetValues(typeof(InventorySelectState));
                InventorySelectState sel = (InventorySelectState)values.GetValue(levelIndex);
                App.InvExtParams.inventorySelState = (int)sel;
                LblSelectState.Text = sel.ToString();
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