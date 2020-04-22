using NurApiDotNet.DotnetCore;


namespace nur_tools_rfiddemo_xamarin.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new nur_tools_rfiddemo_xamarin.App());
            nur_tools_rfiddemo_xamarin.App.Nur.Init();
            //nur_tools_rfiddemo_xamarin.App.NurDeviceSearch.Init();
        }
    }
}
