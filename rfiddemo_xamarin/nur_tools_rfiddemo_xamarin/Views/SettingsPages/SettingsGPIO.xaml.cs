using nur_tools_rfiddemo_xamarin.Templates;
using System;
using System.Collections.ObjectModel;
using NurApiDotNet;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;

namespace nur_tools_rfiddemo_xamarin.Views.SettingsPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsGPIO : ContentPage
    {
        ObservableCollection<ListItem> itemList = new ObservableCollection<ListItem>();
        GpioEntry[] gpio;

        public SettingsGPIO()
        {
            InitializeComponent();
        }

        
        private async void PrepareListItems()
        {            
            if (App.Nur.IsConnected())
            {
                itemList.Clear();
                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        ListItemStyle style = new ListItemStyle("ic_settings_black", 10, Color.White, Color.Black, Color.Blue);
                                               
                        gpio = App.Nur.GetGPIOConfig();

                        for(int x=0;x< App.Nur.Capabilites.maxGPIO;x++)
                        {
                            if (gpio[x].available == false)
                                continue;

                            int gpionum = x + 1;
                            ListItem i = new ListItem();
                            i.MyObject = gpionum;
                            i.SingleRow = false;                            
                            i.SetImage("ic_settings_black", 15);

                            i.CellHeight = 60;                                                      

                            if ((GPIOType)gpio[x].type == GPIOType.Output)
                            {
                                //Output
                                i.ItemHeaderText = "GPIO " + gpionum.ToString() + " (Output)";
                                UpdateOutPutUI(i, x);
                            }
                            else
                            {
                                //Input
                                i.ItemHeaderText = "GPIO " + gpionum.ToString() + " (Input)";
                                if (gpio[x].enabled)
                                {
                                    i.ItemValueText = "Edge: " + ((GPIOEdge)gpio[x].edge).ToString() + "     Action: " + ((GPIOAction)gpio[x].action).ToString();
                                }
                                else
                                {
                                    //Disabled
                                    i.ItemValueText = "Disabled";
                                    i.ItemValueColor = Color.Red;
                                }                                
                            }
                            itemList.Add(i);                                                        
                        }

                       
                    }
                    catch (Exception e)
                    {
                        await DisplayAlert("Cannot read GPIO settings!", e.Message, "OK");
                    }
                });
            }
            else
                await DisplayAlert("No reader connection", "Connect to reader first. ", "OK");

        } 

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Need in order to show status messages bottom of the screen
            App.BindStatusMessage(MyStatusBar);

            PrepareListItems();
            //This event needed when user click item
            MySettingsList.OnItemSelected += MySettingsList_OnItemSelected;

            App.Nur.IOChangeEvent += Nur_IOChangeEvent;

            //Assing item list to ListTemplate
            MySettingsList.SetItemsSource(itemList);           
        }

        /// <summary>
        /// GPIO input change event fired when GPIOAction has been configured as 'Notify'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">parameters of IOChange</param>
        private void Nur_IOChangeEvent(object sender, NurApi.IOChangeEventArgs e)
        {
            NurApi.AccessorySensorSource eventSource = (NurApi.AccessorySensorSource)e.data.source;                        
            App.ShowShortStatusMessage("IOChange Event: " + eventSource.ToString() +" Dir=" + e.data.dir.ToString(),3,Color.White,Color.DarkGreen);
        }

        private void UpdateOutPutUI(ListItem item,int gpioIndex)
        {
            //Update UI
            string stateTxt = (gpio[gpioIndex].edge == 0) ? "Low" : "High";
            Color stateColor = (gpio[gpioIndex].edge == 0) ? Color.WhiteSmoke : Color.LightGreen;
            item.ItemValueText = "State: " + stateTxt;
            item.BkColor = stateColor;
        }

        private void MySettingsList_OnItemSelected(object sender, EventArgs e)
        {
            //User tapped item. Selection not shown. If wanted to show selection, it must be done manually like changing Bkcolor of item.
            ItemTappedEventArgs arg = (ItemTappedEventArgs)e;
            ListItem item = (ListItem)arg.Item;
                                   
            if (item.MyObject != null)
            {
                int gpioindex = (int)item.MyObject -1;
                if ((GPIOType)gpio[gpioindex].type == GPIOType.Output)
                {
                    //Output Item clicked so let's toggle state.
                    gpio[gpioindex].edge = (gpio[gpioindex].edge == 0) ? 1 : 0;
                    //Just make sure this output is enabled
                    gpio[gpioindex].enabled = true;
                                        
                    //Set to reader
                    App.Nur.SetGPIOConfig(gpio);
                    UpdateOutPutUI(item,gpioindex);
                }
                else
                    HandleGPIOSettings(gpioindex);
            }
            
        }

        protected override void OnDisappearing()
        {
            MySettingsList.OnItemSelected -= MySettingsList_OnItemSelected;
            App.Nur.IOChangeEvent -= Nur_IOChangeEvent;
            base.OnDisappearing();
        }

        private async void HandleGPIOSettings(int gpioIndex)
        {
            SettingsPages.SettingsGPIOItem gpioItem = new SettingsGPIOItem();
            gpioItem.SetGPIOItem(gpio , gpioIndex);
            await Navigation.PushAsync(gpioItem);
        }

    }
}