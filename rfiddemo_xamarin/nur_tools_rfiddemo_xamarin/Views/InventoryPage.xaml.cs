﻿using nur_tools_rfiddemo_xamarin.Models;
using nur_tools_rfiddemo_xamarin.Views.SettingsPages;
using NurApiDotNet;
using NurApiDotNet.TagCodec.TDT;
using NurApiDotNet.TidUtils;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static NurApiDotNet.NurApi;

namespace nur_tools_rfiddemo_xamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryPage : ContentPage
    {
        ObservableCollection<TagDetails> tagDetails = new ObservableCollection<TagDetails>();
        Stats stats = new Stats(); //For calculating statistic information about reading speed.


        ConcurrentQueue<Tag> tagsAdded = new ConcurrentQueue<Tag>();
        ConcurrentQueue<Tag> tagsUpdated = new ConcurrentQueue<Tag>();

        CancellationTokenSource cts;
        AutoResetEvent kickTagHandleTask;
        private Task tagHandleTask;
        bool inventoryRunning = false;

        public InventoryPage()
        {
            InitializeComponent();
            kickTagHandleTask = new AutoResetEvent(false);
        }

        private async void WriteEPC(Tag curTag)
        {
            string newEPC = await DisplayPromptAsync("New EPC", "Type HEX strings using WORD boundary", "OK", "Cancel", curTag.GetEpcString(), maxLength: 40, null, curTag.GetEpcString());
            if (newEPC == null) return;

            if (newEPC.Length > 0)
            {
                //When writing to Tag memory, its recommended to provide enough Tx-power to the tag for reliable write operation.
                int origTx = App.Nur.TxLevel; //Get original TxLevel
                App.Nur.TxLevel = 0; //Set full power. (0= no attenuation)                

                try
                {

                    //Let's try first write a new EPC without secure parameters. Singulated using selected EPC
                    App.Nur.WriteEPCByEPC(0, false, curTag.GetEpcString(), newEPC);

                    await DisplayAlert("Write succeed!", "New EPC succesfully written", "OK");
                }
                catch (NurApiException ex)
                {
                    if (ex.error == 4111)
                    {
                        //Cannot write because EPC secured. Let's ask password and try again..
                        string accPWD = await DisplayPromptAsync("EPC memory locked by password!", "Type password", "OK", "Cancel", "", maxLength: 10, Keyboard.Numeric, "");

                        if (string.IsNullOrEmpty(accPWD))
                            return;

                        if (accPWD.Length > 0)
                        {
                            try
                            {
                                //Convert password string to UINT
                                uint pwd = Convert.ToUInt32(accPWD);

                                //Let's try write using selected EPC and password
                                App.Nur.WriteEPCByEPC(pwd, true, curTag.GetEpcString(), newEPC);
                                await DisplayAlert("Write succeed!", "New EPC succesfully written", "OK");
                            }
                            catch (Exception exx)
                            {
                                await DisplayAlert("Operation Failed!", exx.Message, "OK");
                            }
                        }
                    }
                    else
                        await DisplayAlert("Write Failed!", ex.Message, "OK");
                }

                App.Nur.TxLevel = origTx; //Put original back.
            }
        }

        private async void WriteUSER(Tag curTag)
        {
            // Show current user data if available.
            // Required Settings --> InventoryRead: Bank=User, Type=EPCData, Start address=0, Word count = <number of words to be read> (max 32)"            

            string curUser = "";
            if (curTag.irData != null)
            {
                curUser = new string(BinToHexString(curTag.irData).ToCharArray(), 0, (int)App.InvReadParams.wLength * 4);
            }

            string dataToWrite = await DisplayPromptAsync("Write to USER memory", "Type HEX strings using WORD boundary", "OK", "Cancel", curUser, maxLength: 64, null, curUser);
            if (dataToWrite == null) return;
            if (dataToWrite.Length == 0) return;

            string wrAddress = await DisplayPromptAsync("Write address", "Type write address", "OK", "Cancel", "Write address", maxLength: 64,Keyboard.Numeric,"0");
            if (wrAddress == null) return;
            if (wrAddress.Length == 0) return;

            uint wrAddr = 0;

            try
            {
                wrAddr = Convert.ToUInt32(wrAddress);

            }
            catch(Exception e)
            {
                await DisplayAlert("Invalid address!", e.Message, "OK");
                return;
            }

            //When writing to Tag memory, its recommended to provide enough Tx-power to the tag for reliable write operation.
            int origTx = App.Nur.TxLevel; //Get original TxLevel
            App.Nur.TxLevel = 0; //Set full power. (0= no attenuation)  

            try
            {
                //Let's try write to user mem. Singulate using selected EPC.
                App.Nur.WriteTagByEPC(0, false, curTag.epc, BANK_USER, wrAddr, HexStringToBin(dataToWrite));
                await DisplayAlert("Write succeed!", "Data to USER memory succesfully written", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Write Failed!", ex.Message, "OK");
            }

            App.Nur.TxLevel = origTx; //Put original back.
            
        }

        private async void ReadUSER(Tag curTag)
        {
            try
            {
                string wrAddress = await DisplayPromptAsync("Read address", "Type read address", "OK", "Cancel", "Read address", maxLength: 64, Keyboard.Numeric, "0");
                if (wrAddress == null) return;
                if (wrAddress.Length == 0) return;
                                
                uint rdAddr = 0;

                try
                {
                    rdAddr = Convert.ToUInt32(wrAddress);

                }
                catch (Exception e)
                {
                    await DisplayAlert("Invalid address!", e.Message, "OK");
                    return;
                }

                string bytesRd = await DisplayPromptAsync("Words to read", "Type count of words to read", "OK", "Cancel", "Count of words to read", maxLength: 64, Keyboard.Numeric, "2");
                if (bytesRd == null) return;
                if (bytesRd.Length == 0) return;

                int rdWordsToRead = 2;

                try
                {
                    rdWordsToRead = Convert.ToInt32(bytesRd) * 2;

                }
                catch (Exception e)
                {
                    await DisplayAlert("Not valid!", e.Message, "OK");
                    return;
                }

                //Let's try read user mem of current tag.
                byte[] result = curTag.ReadTag(0, false, BANK_USER, rdAddr, rdWordsToRead);
                await DisplayAlert("Read succeed!", NurApi.BinToHexString(result), "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Read Failed!", ex.Message, "OK");
            }
        }

        private async void KillTag(Tag curTag)
        {
            string killPWD = await DisplayPromptAsync("Kill tag", "Type Kill password", "OK", "Cancel", null, maxLength: 20, Keyboard.Numeric, "");
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

        private async void SetAccessPasswordAndLock(Tag curTag)
        {
            string accPWD = await DisplayPromptAsync("Set access password & lock EPC memory", "Type Access password", "OK", "Cancel", null, maxLength: 20, Keyboard.Numeric, "");
            if (accPWD == null) return;

            if (accPWD.Length > 0)
            {
                try
                {
                    //Convert password string to UINT
                    uint pwd = Convert.ToUInt32(accPWD);

                    //Set new access password assuming operation not need to be secured.
                    App.Nur.SetAccessPasswordByEPC(0, false, curTag.epc, pwd);
                    App.Nur.SetLockByEPC(pwd, curTag.epc, LOCK_EPCMEM, LOCK_SECURED);

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
            string killPWD = await DisplayPromptAsync("Set kill password", "Type kill password", "OK", "Cancel", null, maxLength: 20, Keyboard.Numeric, "");
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
            Tag item = App.Nur.GetTagStorage()[args.ItemIndex];

            if (item == null)
                return;

            if (App.Nur.IsInventoryStreamRunning())
                InventoryEnable(false); //Stop inventory and wait user selection

            string[] arr = { "Locate", "TagInfo", "Write EPC", "Write USER mem", "Read USER mem", "Lock EPC memory", "Set Kill password", "Kill" };
            string action = await DisplayActionSheet("Select Tag action", "Cancel", null, arr);

            if (action == "Locate")
            {
                //Show LocateTag popup
                await PopupNavigation.Instance.PushAsync(new LocateTagPage(item.epc));
            }
            else if (action == "TagInfo")
            {
                //Read TID memory from selected tag and show TagInformation window.
                try
                {
                    TagInformation tagInfo = TIDUtils.GetTagInfo(App.Nur, item);
                    await Navigation.PushAsync(new TagInformationPage(tagInfo, item));
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Operation Failed!", ex.Message, "OK");
                }
            }
            else if (action == "Write EPC")
            {
                WriteEPC(item);
            }
            else if (action == "Write USER mem")
            {
                WriteUSER(item);
            }
            else if (action == "Read USER mem")
            {
                ReadUSER(item);
            }
            else if (action == "Lock EPC memory")
            {
                SetAccessPasswordAndLock(item);
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

        private void StopTagHandleTask()
        {
            inventoryRunning = false;
            cts?.Cancel();
            kickTagHandleTask.Set();
        }

        private async void InventoryEnable(bool enable)
        {
            if (App.Nur.IsConnected() == false)
            {
                await DisplayAlert("Operation failed!", "Transport not connected", "OK");
                return;
            }


            if (enable == false)
            {
                //Stop it.
                try
                {
                    if (App.IsInventoryExEnabled)
                        App.Nur.StopInventoryEx();
                    else
                        App.Nur.StopInventoryStream();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error while stopping!", ex.Message, "OK");
                }

                StopTagHandleTask();

                Device.BeginInvokeOnMainThread(() =>
                {
                    ButInvStart.Text = "START";
                    ButInvStart.BackgroundColor = Color.LightGreen;
                    stats.Stop();
                    DeviceDisplay.KeepScreenOn = false;
                });
            }
            else
            {
                if (App.InvReadParams.active)
                    App.Nur.SetInventoryRead(App.InvReadParams); //Make sure inventory read settings set

                //Start      

                tagHandleTask = Task.Factory.StartNew(async () =>
                {
                    await TagHandleTask().ConfigureAwait(false);
                }, TaskCreationOptions.LongRunning);

                if (App.IsInventoryExEnabled)
                {
                    //Do InventoryEx streaming
                    //This demo using only one filter but there could be more than one.
                    //..and only if its enabled
                    int filterCount = 0;
                    if (App.IsExtFilter1Enabled) filterCount++;
                    if (App.IsExtFilter2Enabled) filterCount++;

                    if (filterCount > 0)
                    {
                        int fltIndex = 0;
                        InventoryExFilter[] filterArray = new InventoryExFilter[filterCount];
                        if (App.IsExtFilter1Enabled)
                            filterArray[fltIndex++] = App.InvExtFilter1;
                        if (App.IsExtFilter2Enabled)
                            filterArray[fltIndex++] = App.InvExtFilter2;

                        //Go inventroy with filters
                        try
                        {
                            App.Nur.StartInventoryEx(App.InvExtParams, filterArray);
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Error while starting!", ex.Message, "OK");
                            return;
                        }
                    }
                    else
                    {
                        try
                        {
                            App.Nur.StartInventoryEx(App.InvExtParams, null); //No filters, pass null to filter param.
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Error while starting!", ex.Message, "OK");
                            return;
                        }
                    }

                    //Reading result goes in to the Nur_InventoryExEvent
                }
                else
                {
                    //Make simple inventory.
                    App.Nur.StartInventoryStream();
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    ButInvStart.Text = "STOP";
                    ButInvStart.BackgroundColor = Color.IndianRed;
                    stats.Start();
                    DeviceDisplay.KeepScreenOn = true;
                });
            }

        }

        async void OnStartInventoryClicked(object sender, EventArgs e)
        {
            try
            {
                if (App.Nur.IsInventoryExRunning() || App.Nur.IsInventoryStreamRunning())
                {
                    InventoryEnable(false); //Stop it
                }
                else
                {
                    InventoryEnable(true);
                }
            }
            catch (Exception ex)
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
                stats.Clear();
                ShowStats(0);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }

        async void OnExportClicked(object sender, EventArgs e)
        {
            try
            {
                //Stop invectory if running
                if (App.Nur.IsInventoryExRunning() || App.Nur.IsInventoryStreamRunning())
                {
                    InventoryEnable(false);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }

            if (tagDetails.Count == 0)
            {
                await DisplayAlert("Export", "Nothing to export!", "OK");
                return;
            }

            //Gather inventory results
            StringBuilder sb = new StringBuilder();

            foreach (TagDetails td in tagDetails)
            {
                sb.Append(td.EPC);
                sb.Append(",");
                sb.Append(td.Antenna);
                sb.Append(",");
                sb.Append(td.RSSI);
                sb.AppendLine();
            }

            await Utils.Export(this, "Inventory", sb.ToString());

        }

        private string BuildEpcAndDataKeyString(Tag tag)
        {
            string key;

            if (string.IsNullOrEmpty(tag.GetEpcString()))
                key = ":" + tag.GetDataString();
            else
            {
                key = tag.GetEpcString();
                if (string.IsNullOrEmpty(tag.GetDataString()) == false)
                    key += ":" + tag.GetDataString();
            }

            return key;
        }

        private void UpdateObservable(Tag tag)
        {
            var item = tagDetails.FirstOrDefault(i => i.EPCKey == BuildEpcAndDataKeyString(tag));
            if (item != null)
            {
                item.Antenna = App.AntennaList[(int)tag.antennaId].Name;
                item.RSSI = tag.rssi.ToString();
                item.RSSIScaled = (double)(tag.scaledRssi) / 100.0;
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
                detail.Antenna = App.AntennaList[(int)tag.antennaId].Name;
                detail.RSSI = tag.rssi.ToString();
                detail.RSSIScaled = (double)(tag.scaledRssi) / 100.0;
                if (App.IsShowGS1CodedTags)
                {
                    try
                    {
                        EPCTagEngine eng = new EPCTagEngine(tag.epc, tag.epc.Length);

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
                    detail.DATA = tag.GetDataString();

            }
            catch (Exception e)
            {
                Debug.WriteLine("BuildObservable ERR=" + e.Message + " AntID=" + tag.antennaId.ToString());
            }

            return detail;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Copy tag storage items in to the Observable collection
            tagDetails.Clear();
            TagStorage st = App.Nur.GetTagStorage();

            lock (st)
            {
                for (int x = 0; x < st.Count; x++)
                {
                    TagDetails td = BuildObservable(st[x]);
                    if (td != null)
                        tagDetails.Add(td);
                }
            }

            TagsView.ItemsSource = tagDetails;

            //Need in order to show status messages bottom of the screen
            App.BindStatusMessage(MyStatusBar);

            App.Nur.InventoryStreamEvent += Nur_InventoryStreamEvent;
            App.Nur.InventoryExEvent += Nur_InventoryExEvent;
        }

        static int statTagCount = 0;
        static int statTagPerSec = 0;
        static int statMaxTagPerSec = 0;
        static int statRounds = 0;
        private void ShowStats(int tagCount)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (statTagCount != tagCount)
                {
                    LabelUniqueTags.Text = tagCount.ToString();
                    LabelUniqueTagsInTime.Text = "(in " + stats.GetElapsedSecs().ToString("#.#") + " sec)";
                    statTagCount = tagCount;
                }

                if (statTagPerSec != stats.GetTagsPerSec())
                {
                    LabelTagsPerSec.Text = stats.GetTagsPerSec().ToString();
                    statTagPerSec = stats.GetTagsPerSec();
                }

                if (statMaxTagPerSec != stats.GetMaxTagsPerSec())
                {
                    LabelMaxTagsPerSec.Text = stats.GetMaxTagsPerSec().ToString();
                    statMaxTagPerSec = stats.GetMaxTagsPerSec();
                }

                if (statRounds != stats.GetInventoryRounds())
                {
                    LabelRounds.Text = stats.GetInventoryRounds().ToString();
                    statRounds = stats.GetInventoryRounds();
                }
            });
        }

        private void HandleInventoryStreamEvent(InventoryStreamEventArgs e)
        {
            //Inventoried tags are now added or updated in to the internal TagStore
            TagStorage storage = App.Nur.GetTagStorage();

            try
            {
                // Update tag information in to the UI                          

                lock (storage)
                {
                    //Get list of tags added and tags just updated.
                    Dictionary<byte[], Tag> added = storage.GetAddedTags();
                    Dictionary<byte[], Tag> updated = storage.GetUpdatedTags();

                    //..for statistic purpose
                    stats.UpdateStats(e.data.roundsDone, added.Count + updated.Count);
                    ShowStats(storage.Count);

                    //Just push tags to Queue and handle them later in TagHandleTask

                    foreach (KeyValuePair<byte[], Tag> atag in added)
                        tagsAdded.Enqueue(atag.Value);

                    foreach (KeyValuePair<byte[], Tag> utag in updated)
                        tagsUpdated.Enqueue(utag.Value);

                    if (added.Count > 0 || updated.Count > 0)
                        kickTagHandleTask.Set();

                    added.Clear();
                    updated.Clear();
                }


                //Make sure lists are cleared so we can receive fresh added/updated information next time                   

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            if (e.data.stopped)
            {
                try
                {
                    if (App.IsInventoryExEnabled)
                        App.Nur.InventoryEx(); //This just rerun InventoryExStream
                    else
                        App.Nur.StartInventoryStream(); //Start simple inventory again

                    Debug.WriteLine("RESTARTED");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception " + ex.Message);
                }
            }
        }

        private async Task TagHandleTask()
        {
            Debug.WriteLine("ENTER TagHandleTask");

            inventoryRunning = true;
            cts = new CancellationTokenSource();

            while (inventoryRunning)
            {
                if (cts.IsCancellationRequested)
                    break;

                kickTagHandleTask.WaitOne();

                if (!inventoryRunning)
                    break;

                if (cts.IsCancellationRequested)
                    break;

                Debug.WriteLine("added.." + tagsAdded.Count.ToString());
                Debug.WriteLine("updated.." + tagsUpdated.Count.ToString());

                //Note: things may getting slow while tag count rises if need to update UI

                while (tagsAdded.Count > 0)
                {
                    Tag atag;
                    if (tagsAdded.TryDequeue(out atag))
                    {
                        //This sample just add new tag item to list
                        TagDetails td = BuildObservable(atag);
                        if (td != null)
                            tagDetails.Add(td);
                    }
                }

                while (tagsUpdated.Count > 0)
                {

                    Tag utag;
                    if (tagsUpdated.TryDequeue(out utag))
                    {
                        //This sample just update existing tag information in the list
                        UpdateObservable(utag);
                    }
                }
            }

            Debug.WriteLine("EXIT TagHandleTask");
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