using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NurApiDotNet;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using nur_tools_rfiddemo_xamarin.Models;
using System.Collections.ObjectModel;
using static NurApiDotNet.NurApi;
using Rg.Plugins.Popup.Services;
using NordicID.NurApi.Support;
using nur_tools_rfiddemo_xamarin.Views.SettingsPages;

namespace nur_tools_rfiddemo_xamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryPage : ContentPage
    {        
        ObservableCollection<TagDetails> tagDetails = new ObservableCollection<TagDetails>();
        
        public InventoryPage()
        {
            InitializeComponent();           
        }

        private async void WriteEPC(Tag curTag)
        {            
            string newEPC = await DisplayPromptAsync("New EPC", " Type HEX strings using WORD boundary", "OK", "Cancel", curTag.GetEpcString(), maxLength: 40,null);
            if (newEPC == null) return;

            if (newEPC.Length > 0)
            {
                try
                {
                    //Let's try write singulated using selected EPC
                    App.Nur.WriteEPCByEPC(0, false, curTag.GetEpcString(), newEPC);
                    await DisplayAlert("Write succeed!", "New EPC succesfully written", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Write Failed!", ex.Message, "OK");
                }
            }
        }

        private async void KillTag(Tag curTag)
        {            
            string killPWD = await DisplayPromptAsync("Kill tag", "Type Kill password", "OK", "Cancel", null, maxLength: 20, Keyboard.Numeric);
            if (killPWD == null) return;

            if (killPWD.Length > 0)
            {
                try
                {
                    //Convert password string to UINT
                    uint pwd = Convert.ToUInt32(killPWD);
                    App.Nur.KillTagByEPC(pwd, curTag.epc);
                    await DisplayAlert("Operation succeed!", "Tag killed", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Operation Failed!", ex.Message, "OK");
                }

            }
        }
                
        private async void SetAccessPassword(Tag curTag)
        {            
            string accPWD = await DisplayPromptAsync("Set access password", "Type Access password", "OK", "Cancel", null, maxLength: 20, Keyboard.Numeric);
            if (accPWD == null) return;

            if (accPWD.Length > 0)
            {
                try
                {
                    //Convert password string to UINT
                    uint pwd = Convert.ToUInt32(accPWD);

                    //Set new access password assuming operation not need to be secured.
                    App.Nur.SetAccessPasswordByEPC(0,false,curTag.epc,pwd);

                    await DisplayAlert("Operation succeed!", "Access password set.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Operation Failed!", ex.Message, "OK");
                }

            }
        }

        private async void SetKillPassword(Tag curTag)
        {            
            string killPWD = await DisplayPromptAsync("Set kill password", "Type kill password", "OK", "Cancel", null, maxLength: 20, Keyboard.Numeric);
            if (killPWD == null) return;

            if (killPWD.Length > 0)
            {
                try
                {
                    //Convert password string to UINT
                    uint pwd = Convert.ToUInt32(killPWD);
                    //Set new access password assuming operation not need to be secured.
                    App.Nur.SetKillPasswordByEPC(0, false, curTag.epc, pwd);

                    await DisplayAlert("Operation succeed!", "Tag Kill password set", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Operation Failed!", ex.Message, "OK");
                }
            }
        }

        async void OnItemTapped(object sender, ItemTappedEventArgs args)
        {            
            Tag item = App.Nur.GetTagStorage().Get(args.ItemIndex);

            if (item == null)
                return;            

            if (App.Nur.IsInventoryStreamRunning())
                InventoryEnable(false); //Stop inventory and wait user selection

            string[] arr = { "Locate", "Write EPC", "Set Access password", "Set Kill password", "Kill" };
            string action = await DisplayActionSheet("Select Tag action", "Cancel", null, arr);

            if (action == "Locate")
            {
                //Show LocateTag popup
                await PopupNavigation.Instance.PushAsync(new LocateTagPage(item.epc));
            }
            if (action == "Write EPC")
            {
                WriteEPC(item);
            }
            else if (action == "Set Access password")
            {
                SetAccessPassword(item);
            }
            else if (action == "Set Kill password")
            {
                SetKillPassword(item);
            }
            else if (action == "Kill")
            {
                KillTag(item);
            }

        }
                           
        private void InventoryEnable(bool enable)
        {
            if (enable==false)
            {
                //Stop it.                               
                App.Nur.StopInventoryStream();                
                ButInvStart.Text = "START";
                ButInvStart.BackgroundColor = Color.LightGreen;               
            }
            else
            {
                //Start      
                if (App.IsInventoryExEnabled)
                {
                    //Do InventoryEx streaming
                    //This demo using only one filter but there could be more than one.
                    //..and only if its enabled
                    InventoryExFilter[] filterArray = new InventoryExFilter[1];
                    filterArray[0] = App.InvExtFilter;

                    if (App.IsExtFilterEnabled)
                        App.Nur.StartInventoryEx(App.InvExtParams, filterArray); //Yes, filter enabled
                    else App.Nur.StartInventoryEx(App.InvExtParams, null); //No, pass null to filter param.

                    //Reading result goes in to the Nur_InventoryExEvent
                }
                else
                {
                    //Make simple inventory.
                    App.Nur.StartInventoryStream();
                }
                                
                ButInvStart.Text = "STOP";
                ButInvStart.BackgroundColor = Color.IndianRed;              
            }
        }

        async void OnStartInventoryClicked(object sender, EventArgs e)
        {
            try
            {
                if (App.Nur.IsInventoryStreamRunning())
                {
                    InventoryEnable(false); //Stop it
                }
                else
                {
                    InventoryEnable(true);
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void OnInvOptionsClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new SettingsInvOptions());
        }
        
        async void OnClearInventoryClicked(object sender, EventArgs e)
        {
            try
            {
                App.Nur.ClearTagsEx(); //Clear tags from NurApi internal and from module.
                tagDetails.Clear();
            }
            catch(Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        private string BuildEpcAndDataKeyString(Tag tag)
        {
            string key;

            if (string.IsNullOrEmpty(tag.EpcString))
                key = ":" + tag.DataString;
            else
            {
                key = tag.EpcString;
                if (string.IsNullOrEmpty(tag.DataString) == false)
                    key += ":" + tag.DataString;
            }

            return key;
        }

        private void UpdateObservable(Tag tag)
        {
            var item = tagDetails.FirstOrDefault(i => i.EPCKey == BuildEpcAndDataKeyString(tag));
            if (item != null)
            {                
               item.Antenna = App.AntennaList[(int)tag.antennaId].name;              
               item.RSSI = tag.rssi.ToString();
               item.RSSIScaled = (double)(tag.scaledRssi)/100.0;                
            }
        }       

        private TagDetails BuildObservable(Tag tag)
        {
            TagDetails detail = new TagDetails();
            try
            {                
                detail.EPCKey = BuildEpcAndDataKeyString(tag);
                detail.EPC = detail.EPCKey; //may change..
                detail.EPCColor = Color.DarkGreen;
                detail.Antenna = App.AntennaList[(int)tag.antennaId].name;
                detail.RSSI = tag.rssi.ToString();
                detail.RSSIScaled = (double)(tag.scaledRssi) / 100.0;
                if (App.IsShowGS1CodedTags)
                {
                    try
                    {
                        TDT.EPCTagEngine eng = new TDT.EPCTagEngine(tag.EpcString);                        
                        detail.EPC = eng.BuildPureIdentityURI();
                        detail.EPCColor = Color.Blue;
                    }
                    catch (Exception) 
                    {
                        if (App.IsShowOnlyGS1CodedTags)
                            return null; //We'll not show any other tags than GS1 coded.
                    }
                }
                else                               
                    detail.DATA = tag.DataString;             
                
            }
            catch(Exception e)
            {
                Debug.WriteLine("BuildObservable ERR=" + e.Message + " AntID=" + tag.antennaId.ToString());
            }

            return detail;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Need in order to show status messages bottom of the screen
            App.BindStatusMessage(MyStatusBar);

            //Copy tag storage items in to the Observable collection
            tagDetails.Clear();
            TagStorage st = App.Nur.GetTagStorage();

            lock (st)
            {
                for (int x = 0; x < st.Count; x++)
                {
                    tagDetails.Add(BuildObservable(st.Get(x)));
                }
            }

            TagsView.ItemsSource = tagDetails;                     

            App.Nur.InventoryStreamEvent += Nur_InventoryStreamEvent;
            App.Nur.InventoryExEvent += Nur_InventoryExEvent;                        
        }

        private void HandleInventoryStreamEvent(InventoryStreamEventArgs e)
        {
            //Inventoried tags are now added or updated in to the internal TagStore
            TagStorage storage = App.Nur.GetTagStorage();

            try
            {
                //Get list of tags added and tags just updated.
                List<Tag> added = storage.GetAddedTags();
                List<Tag> updated = storage.GetUpdatedTags();

                // Update tag information in to the UI                          
                Device.BeginInvokeOnMainThread(async () =>
                {
                    lock (storage)
                    {
                        for (int n = 0; n < updated.Count; n++)
                        {
                            UpdateObservable(updated[n]);
                        }

                        // Add inventoried tags to our unique tag storage
                        for (int n = 0; n < added.Count; n++)
                        {
                            TagDetails td = BuildObservable(added[n]);
                            if(td != null)
                                tagDetails.Add(td);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            if (e.data.stopped)
            {
                try
                {
                    if (App.IsExtFilterEnabled)
                        App.Nur.InventoryEx(); //This just rerun InventoryExStream
                    else App.Nur.StartInventoryStream(); //Start simple inventory again

                    Debug.WriteLine("RESTARTED");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception " + ex.Message);
                }
            }
        }

        private void Nur_InventoryExEvent(object sensder, InventoryStreamEventArgs e)
        {
            HandleInventoryStreamEvent(e);
        }

        private void Nur_InventoryStreamEvent(object sender, InventoryStreamEventArgs e)
        {
            HandleInventoryStreamEvent(e);
        }

        protected override void OnDisappearing()
        {
            try
            {
                if (App.Nur.IsInventoryStreamRunning())
                {
                    InventoryEnable(false); //Stop it
                }                               
            }
            catch (Exception) { }

            base.OnDisappearing();

            App.Nur.InventoryStreamEvent -= Nur_InventoryStreamEvent;
            App.Nur.InventoryExEvent -= Nur_InventoryExEvent;


        }

    }
}