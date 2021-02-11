using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NordicID.UpdateLib;
using Plugin.FilePicker;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static NordicID.UpdateLib.NurUpdate;

namespace nur_tools_rfiddemo_xamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdatesPage : ContentPage
    {
        NurUpdate upd;
        
        string mTxtHdr;
        string mTxtItem;
        Color mColorHdr;
        bool isAvailUpdatesChecked;
        bool isUpdatesAvailable;
        bool isUpdateFromFile;
        int mProgress;
                        
        public UpdatesPage()
        {
            InitializeComponent();

            upd = new NurUpdate();            
            upd.SetNurApi(App.Nur);

            isAvailUpdatesChecked = false;
            isUpdatesAvailable = false;
            isUpdateFromFile = false;

            mProgress = 0;

            progressUpdate.GaugeValueTextColor = Color.Black;
            progressUpdate.GaugeRadialWidth = 50;
            progressUpdate.RangeMax = 100;
            progressUpdate.RangeMin = 0;
            progressUpdate.GaugeUnitTextColor = Color.DarkRed;
            progressUpdate.GaugeUnitText = "%";

            progressUpdate.IsVisible = false;

            buttonUpdate.Clicked += ButtonUpdate_Clicked;
            buttonLocal.Clicked += ButtonLocal_Clicked;
        }

        private async void ButtonLocal_Clicked(object sender, EventArgs e)
        {
            string[] fileTypes = null;
            if (Device.RuntimePlatform == Device.Android)
            {
                fileTypes = new string[] { "Update files/zip" };               
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                fileTypes = new string[] { "ZipArchive" }; //Most probably not work
            }

            if (Device.RuntimePlatform == Device.UWP)
            {
                fileTypes = new string[] { ".zip" };
            }

            await PickAndShowFile(fileTypes);
        }

        private async Task StartUpdate()
        {
            await Task.Run(() =>
            {
                try
                {
                    upd.StartUpdate();
                }
                catch (Exception e)
                {
                    mTxtHdr = "Error (StartUpdate)";
                    mColorHdr = Color.Red;
                    mTxtItem = e.Message;
                    Console.WriteLine("EXCEPTION STARTUPDATE:" + e.Message);
                }

                UpdateUI();
            });
        }

        private void Upd_OnUpdatingEvent(object sender, UpdatingEventArgs e)
        {
            switch(e.niduEvent)
            {
                case Event.PROGRESS:
                    mProgress = e.prg;
                    mTxtHdr = "Programming.. "; // + e.prg.ToString()+"%";
                    break;
                case Event.LOG:
                    Debug.WriteLine(e.msg);
                    break;
                case Event.PRG_BEGIN:
                    mProgress = 0;
                    mTxtHdr = "Programming..";
                    mTxtItem = "";
                    break;
                case Event.PRG_END:
                    //Programming end for all item. e.prg indicates number of errors occured
                    if (e.prg > 0)
                    {
                        mTxtHdr = "Errors:" + e.prg.ToString();
                        mColorHdr = Color.Red;
                        mTxtItem = upd.LASTERROR;                        
                    }
                    else
                    {
                        mTxtHdr = "Update completed!";
                        mColorHdr = Color.Green;
                        mTxtItem = "";
                        mProgress = 0;

                        isUpdatesAvailable = false;
                        isUpdateFromFile = false;
                        isAvailUpdatesChecked = false;
                    }

                    //Do the connect cycle to get fresh information from reader.
                    App.Nur.Disconnect();
                    App.Nur.Connect();

                    break;
                case Event.PRG_ITEM_START:
                    //Specific update item is about to start programming.
                    //User can abort this update by callig nidu.Abort(e.prg); 
                    //If there are no internal function to do FW update, user can do what ever like with UpdateItem -->data
                    //e.prg indicates index to UpdateItem

                    mTxtItem = upd.GetUpdateItem(e.prg).filename;                    
                  
                    break;
                case Event.PRG_ERROR:
                    mTxtHdr = "Programming error";
                    mColorHdr = Color.Red;
                    mTxtItem = upd.LASTERROR;
                    break;
                case Event.STATUS:
                    Debug.WriteLine("Status change:" + e.msg);
                    break;

            }

            UpdateUI();
           
        }

        private async void ButtonUpdate_Clicked(object sender, EventArgs e)
        {
            if(isUpdateFromFile)
            {
                //Zip file already loaded and validated
                Debug.WriteLine("Start update from file");
                await StartUpdate();

            }
            else
            {
                //Load Zip from server and validate
                upd.LoadZipFromNordicIDServer();
                Error err = upd.Validate();
                
                if (err == Error.NONE)
                {
                    await StartUpdate();
                }
                else
                {
                    mTxtHdr = "Error validating zip";
                    mColorHdr = Color.Red;
                    mTxtItem = err.ToString() + ":" + upd.LASTERROR;
                }
                
            }
        }

        private async Task PickAndShowFile(string[] fileTypes)
        {
            try
            {
                var pickedFile = await CrossFilePicker.Current.PickFile(fileTypes);

                if (pickedFile != null)
                {
                    mTxtHdr = "Update from file";
                    mTxtItem = pickedFile.FileName;
                    
                    if (pickedFile.FileName.EndsWith("zip", StringComparison.OrdinalIgnoreCase))                  
                    {
                        Debug.WriteLine("Picked File=" + pickedFile.FilePath);
                        byte[] arr = new byte[pickedFile.DataArray.Length];
                        Debug.WriteLine("FileLen=" + arr.Length.ToString());
                        Array.Copy(pickedFile.DataArray, arr,arr.Length);
                        upd.LoadZip(arr);                       
                        Error err = upd.Validate();
                        Debug.WriteLine("Validate=" +err.ToString());

                        if (err == Error.NONE)
                        {
                            mTxtItem = "";
                            for (int x=0;x<upd.GetItemCount();x++)
                            {
                                UpdateItem upi = upd.GetUpdateItem(x);
                                Debug.WriteLine(upi.filename + " Status=" + upi.status.ToString());
                                if(upi.status == Status.READY)
                                {
                                    mTxtItem += upi.filename + "\n";
                                }
                            }

                            if (mTxtItem.Length > 0)
                            {
                                isUpdatesAvailable = true;
                                isUpdateFromFile = true;
                            }
                            else
                                isUpdatesAvailable = false;
                        }
                        else
                        {
                            mTxtHdr = "Error loading from file";
                            mColorHdr = Color.Red;
                            mTxtItem = err.ToString() + ":" + upd.LASTERROR;
                        }
                        
                        //FileImagePreview.Source = ImageSource.FromStream(() => pickedFile.GetStream());
                        //FileImagePreview.IsVisible = true;
                    }
                    else
                    {
                        mTxtHdr = "Not valid zip file";
                        mColorHdr = Color.Red;
                        mTxtItem = "";
                    }
                }
                else
                    Debug.WriteLine("Picked File FAIL");
            }
            catch (Exception ex)
            {
                mTxtHdr = "Cannot load file";
                mColorHdr = Color.Red;
                mTxtItem = ex.Message;
            }

            UpdateUI();
        }

        private async Task LoadAvailUpdatesAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    List<UpdateItem> items = upd.GetAvailableUpdatesFromNordicIDServer();
                    mTxtItem = "";

                    if (items.Count > 0)
                    {
                        //There are updates available. Show them.  
                        foreach (UpdateItem ui in items)
                        {
                            mTxtItem += ui.filename;
                            mTxtItem += '\n';
                        }

                        mTxtHdr = "Available Updates";
                        mColorHdr = Color.Black;
                        isUpdatesAvailable = true;
                    }
                    else
                    {
                        mTxtHdr = "Device UP-TO-DATE";
                        mColorHdr = Color.Green;
                        mTxtItem = "";
                        isUpdatesAvailable = false;
                    }
                }
                catch (Exception e)
                {
                    mTxtHdr = "Cannot load available updates";
                    mColorHdr = Color.Red;
                    mTxtItem = e.Message;
                    isUpdatesAvailable = false;
                }

                UpdateUI();
            });
        }

        private void UpdateUI()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lblHeader.Text = mTxtHdr;
                lblHeader.TextColor = mColorHdr;
                lblFWItems.Text = mTxtItem;                
                buttonUpdate.IsVisible = isUpdatesAvailable;
                if(mProgress>0)
                {
                    if(!progressUpdate.IsVisible)
                        progressUpdate.IsVisible = true;
                    progressUpdate.SetProgress(mProgress);                                       
                }
                else
                    progressUpdate.IsVisible = false;
            });
        }        

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            upd.OnUpdatingEvent += Upd_OnUpdatingEvent;

            if (!isAvailUpdatesChecked)
            {
                //Latest firmware's for reader are available from Nordic ID server.
                //This function retrieve only names of firmware(s) available for the currently connected device.

                mTxtHdr = "Checking updates..";
                mColorHdr = Color.Black;
                mTxtItem = "";
                isUpdatesAvailable = false;
                UpdateUI();

                actIndicator.IsVisible = true;
                actIndicator.IsRunning = true;

                await LoadAvailUpdatesAsync();

                actIndicator.IsRunning = false;
                actIndicator.IsVisible = false;
                isAvailUpdatesChecked = true;
            }
        }

        protected override void OnDisappearing()
        {            
            base.OnDisappearing();

            upd.OnUpdatingEvent -= Upd_OnUpdatingEvent;
        }
    }
}