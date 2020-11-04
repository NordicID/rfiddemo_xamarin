using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace nur_tools_rfiddemo_xamarin.Views.SettingsPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsExport :  ContentPage
    {
        string exportPath = "";

        public SettingsExport()
        {
            InitializeComponent();
            

        }

        private void UpdateStatusText(string status)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                statusTxt.Text = status;
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            exportPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            UpdateStatusText(exportPath);

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }
    }
}