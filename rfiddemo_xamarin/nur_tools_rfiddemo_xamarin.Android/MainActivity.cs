using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android;
using System.Collections.Generic;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using NurApiDotNet;
using NurApiDotNet.Android;

namespace nur_tools_rfiddemo_xamarin.Droid
{
    [Activity(
        Label = "RFID Demo Xamarin", 
        Icon = "@drawable/ic_launcherweb",
        Theme = "@style/MainTheme", 
        MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden
        )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {       
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
          
            LoadApplication(new App());

            //Prepare NurApi and NurDeviceDiscovery instances for this platform.           
            App.Nur.Init(Application.Context);

            RequestPermissionsManually();
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }

        /*
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        */

        List<string> _permission = new List<string>();

        private void RequestPermissionsManually()
        {
            try
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
                    _permission.Add(Manifest.Permission.AccessCoarseLocation);

                if (_permission.Count > 0)
                {
                    string[] array = _permission.ToArray();
                    ActivityCompat.RequestPermissions(this, array, array.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        override public void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            //location || storage
            if (requestCode == 2 || requestCode == 5)
            {
                if (grantResults.Length == _permission.Count)
                {
                    for (int i = 0; i < requestCode; i++)
                    {
                        if (grantResults[i] == Permission.Granted)
                        {
                            //do nothing, we already have permissions granted
                        }
                        else
                        {
                            _permission = new List<string>();
                            RequestPermissionsManually();
                            break;
                        }
                    }
                }

            }
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        

    }
}