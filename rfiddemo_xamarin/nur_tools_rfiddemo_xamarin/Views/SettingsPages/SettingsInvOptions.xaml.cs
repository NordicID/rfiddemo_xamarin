using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace nur_tools_rfiddemo_xamarin.Views.SettingsPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsInvOptions : PopupPage
    {
        public SettingsInvOptions()
        {
            InitializeComponent();
        }

        async void OnSwitchEnableIRChanged(object sender, EventArgs e)
        {
            try
            {
                App.Nur.InventoryReadCtl = EnableIR.IsToggled;
                App.IsInventoryReadEnabled = EnableIR.IsToggled;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
                EnableIR.IsToggled = false;
            }
        }

        void OnSwitchShowGS1Changed(object sender, EventArgs e)
        {
            App.IsShowGS1CodedTags = SwitchShowGs1.IsToggled;
            SwitchShowOnlyGs1.IsEnabled = SwitchShowGs1.IsToggled;
        }

        void OnSwitchShowOnlyGS1Changed(object sender, EventArgs e)
        {
            App.IsShowOnlyGS1CodedTags = SwitchShowOnlyGs1.IsToggled;
        }

        void OnSwitchEnableFilterChanged(object sender, EventArgs e)
        {
            App.IsInventoryExEnabled = EnableInvEx.IsToggled;
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
            base.OnDisappearing();   
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            try
            {
                EnableIR.IsToggled = App.Nur.InventoryReadCtl;
                EnableInvEx.IsToggled = App.IsInventoryExEnabled;
                SwitchShowGs1.IsToggled = App.IsShowGS1CodedTags;
                SwitchShowOnlyGs1.IsToggled  = App.IsShowOnlyGS1CodedTags;
                SwitchShowOnlyGs1.IsEnabled = SwitchShowGs1.IsToggled;
            }
            catch (Exception e)
            {
                //Probably disconnected or read info settings on module not valid
                Debug.WriteLine("InventoryPage OnAppearing ERR=" + e.Message);

            }
        }
    }
}