using System;
using NurApiDotNet;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using nur_tools_rfiddemo_xamarin.Templates;

namespace nur_tools_rfiddemo_xamarin.Views.SettingsPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsGPIOItem : ContentPage
    {
        ObservableCollection<ListItem> itemList = new ObservableCollection<ListItem>();

        int mGpioIndex;
        GpioEntry [] mGpioEntries;

        public SettingsGPIOItem()
        {
            InitializeComponent();            
        }

        public void SetGPIOItem(GpioEntry [] gpioEntries, int gpioIndex)
        {
            mGpioIndex = gpioIndex;
            mGpioEntries = gpioEntries;
        }

        private void PrepareListItems()
        {           
            itemList.Clear();

            Device.BeginInvokeOnMainThread(() =>
            {
                int gpioNum = mGpioIndex + 1;
                GPIOHeaderText.Text = "GPIO " + gpioNum.ToString() + " settings";

                ListItemStyle style = new ListItemStyle("ic_settings_black", 20, Color.White, Color.Black, Color.Blue);

                ListItem enabledItem = new ListItem();
                enabledItem.Selected = mGpioEntries[mGpioIndex].enabled;
                enabledItem.ItemHeaderText = "Enable GPIO " + gpioNum.ToString();
                GPIOEnable(enabledItem);
                itemList.Add(enabledItem);

                GPIOEdge edge = (GPIOEdge)mGpioEntries[mGpioIndex].edge;
                itemList.Add(new ListItem(style, "Edge", edge.ToString()));

                GPIOAction action = (GPIOAction)mGpioEntries[mGpioIndex].action;
                itemList.Add(new ListItem(style, "Action", action.ToString()));

            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Fill items to list
            PrepareListItems();

            //This event needed when user click item
            MySettingsList.OnItemSelected += MySettingsList_OnItemSelected; ;

            //Assing item list to ListTemplate
            MySettingsList.SetItemsSource(itemList);
        }

        private void MySettingsList_OnItemSelected(object sender, EventArgs e)
        {
            //User tapped item. Selection not shown. If wanted to show selection, it must be done manually like changing Bkcolor of item.
            ItemTappedEventArgs arg = (ItemTappedEventArgs)e;
            ListItem item = (ListItem)arg.Item;

            switch (arg.ItemIndex)
            {
                case 0:
                    GPIOEnable(item);
                    mGpioEntries[mGpioIndex].enabled = item.Selected;
                    App.Nur.SetGPIOConfig(mGpioEntries);
                    break;                
                case 1:                                    
                    EdgeSelection(item);
                    break;
                case 2:
                    ActionSelection(item);
                    break;               
            }
        }

        void GPIOEnable(ListItem enabledItem)
        {
            if (enabledItem.Selected)
            {
                enabledItem.SetImage("ic_enabled.png", 20);
                enabledItem.ItemValueText = "Enabled";
                enabledItem.ItemValueColor = Color.Green;                
            }
            else
            {
                enabledItem.SetImage("ic_disabled.png", 20);
                enabledItem.ItemValueText = "Disabled";
                enabledItem.ItemValueColor = Color.Red;
            }            
        }

        async void ActionSelection(ListItem item)
        {            
            string[] arr = Enum.GetNames(typeof(GPIOAction)); //Just name selection as Action0, Action1...
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set GPIO action", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                Array actionValues = Enum.GetValues(typeof(GPIOAction));
                GPIOAction b = (GPIOAction)actionValues.GetValue(levelIndex);
                mGpioEntries[mGpioIndex].action = (byte)b;              
                App.Nur.SetGPIOConfig(mGpioEntries);

                //Exception not thrown if configurable item is not suitable for target reader
                //Therefore, we need to read GPIO configuration back and make some comparing..
                mGpioEntries = App.Nur.GetGPIOConfig();
                if (mGpioEntries[mGpioIndex].action != (byte)b)
                {
                    await DisplayAlert("Operation failed!", "Cannot set " + " GPIO " + (mGpioIndex+1).ToString() + " Action to " + b.ToString() + " for this reader.", "OK");
                }
                else item.ItemValueText = b.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }

        }
                
        async void EdgeSelection(ListItem item)
        {
            string[] arr = Enum.GetNames(typeof(GPIOEdge));
            IndexDictionary dict = new IndexDictionary(arr);
            string action = await DisplayActionSheet("Set GPIO Edge", "Cancel", null, arr);
            int levelIndex = dict.GetItemIndex(action);
            if (levelIndex == -1) return; //Possible Cancel pressed

            try
            {
                Array actionValues = Enum.GetValues(typeof(GPIOEdge));
                GPIOEdge b = (GPIOEdge)actionValues.GetValue(levelIndex);
                mGpioEntries[mGpioIndex].edge = (byte)b;                
                App.Nur.SetGPIOConfig(mGpioEntries);

                //Exception not thrown if configurable item is not suitable for target reader
                //Therefore, we need to read GPIO configuration back and make some comparing..
                mGpioEntries = App.Nur.GetGPIOConfig();
                if(mGpioEntries[mGpioIndex].edge != (byte)b)
                {
                    await DisplayAlert("Operation failed!", "Cannot set " + " GPIO " + (mGpioIndex + 1).ToString() + " Edge to " + b.ToString() + " for this reader.", "OK");
                }
                else item.ItemValueText = b.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Operation failed!", ex.Message, "OK");
            }
        }
                
        protected override void OnDisappearing()
        {            
            MySettingsList.OnItemSelected -= MySettingsList_OnItemSelected;
            base.OnDisappearing();

        }
    }
}