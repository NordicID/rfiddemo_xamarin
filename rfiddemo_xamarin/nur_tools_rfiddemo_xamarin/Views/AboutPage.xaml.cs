using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NurApiDotNet;

namespace nur_tools_rfiddemo_xamarin.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();

            verApp.Text = typeof(App).Assembly.GetName().Version.ToString();
            verNurApi.Text = typeof(NurApi).Assembly.GetName().Version.ToString();
        }
    }
}