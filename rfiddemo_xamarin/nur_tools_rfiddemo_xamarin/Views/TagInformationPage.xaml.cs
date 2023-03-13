using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nur_tools_rfiddemo_xamarin.Templates;
using NurApiDotNet;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NurApiDotNet.TidUtils;
using nur_tools_rfiddemo_xamarin.Models;

namespace nur_tools_rfiddemo_xamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TagInformationPage : ContentPage
    {
        readonly ObservableCollection<ListItem> itemList = new ObservableCollection<ListItem>();
        TagInformation tagInfo;
        NurApi.Tag tag;

        public TagInformationPage(TagInformation ti, NurApi.Tag selectedtag)
        {
            tagInfo = ti;
            tag = selectedtag;
            InitializeComponent();
            Title = "Tag Information";
        }

        private void PrepareListItems()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                itemList.Clear();

                ListItemStyle style = new ListItemStyle("", 0, Color.White, Color.Black, Color.Blue);
                style.styleCellHeight = -1;
                                
                itemList.Add(new ListItem(style, "Company", tagInfo.Company));
                itemList.Add(new ListItem(style, "Model", tagInfo.TagModel));                
                itemList.Add(new ListItem(style, "Mask Designer Id (MDID)", tagInfo.MDID_valid ? tagInfo.MDID.ToString("X2") : "N/A"));
                itemList.Add(new ListItem(style, "TagModelNumber (TMN)", tagInfo.TMN_valid ? tagInfo.TMN.ToString("X2") : "N/A"));
                itemList.Add(new ListItem(style, "Extended ID (XTID)", tagInfo.XTID_valid ? tagInfo.XTID.ToString("X2") : "N/A"));
                itemList.Add(new ListItem(style, "MCS", tagInfo.MCS_valid ? tagInfo.MCS.ToString("X2") : "N/A"));
                itemList.Add(new ListItem(style, "Header", tagInfo.Header_valid ? tagInfo.Header.ToString("X2") : "N/A"));
               
                itemList.Add(new ListItem(style, "FullTIDMem", NurApi.BinToHexString(tagInfo.FullTIDMemory)));

                itemList.Add(new ListItem(style, "UsrMemBlkAndPermaLockPresent", tagInfo.XTID_UserMemBlkAndPermaLockPresent.ToString()));
                itemList.Add(new ListItem(style, "BlkWriteEraseSegPresent", tagInfo.XTID_BlkWriteEraseSegPresent.ToString()));
                itemList.Add(new ListItem(style, "OptCmdSupportSegPresent", tagInfo.XTID_OptCmdSupportSegPresent.ToString()));
                itemList.Add(new ListItem(style, "XTID SerialPresent", tagInfo.XTID_SerialPresent.ToString()));
                itemList.Add(new ListItem(style, "XTID SerialBitLength", tagInfo.XTID_SerialBitLength.ToString()));
                try
                {
                    itemList.Add(new ListItem(style, "XTID Serial", NurApi.BinToHexString(tagInfo.XTID_Serial)));
                }
                catch(Exception ex)
                {
                    itemList.Add(new ListItem(style, "XTID Serial", ex.Message));
                }

                try
                {
                    itemList.Add(new ListItem(style, "Has_G2X_Serial", tagInfo.Has_G2X_Serial.ToString()));
                    itemList.Add(new ListItem(style, "NXP G2X Serial", tagInfo.NXP_G2X_Serial.ToString("X2")));
                }
                catch (Exception ex)
                {
                    itemList.Add(new ListItem(style, "NXP G2X Serial", ex.Message));
                }

                try
                {
                    itemList.Add(new ListItem(style, "Has_G2i_Serial", tagInfo.Has_G2i_Serial.ToString()));
                    itemList.Add(new ListItem(style, "NXP G2i Serial", tagInfo.NXP_G2X_Serial.ToString("X2")));
                }
                catch (Exception ex)
                {
                    itemList.Add(new ListItem(style, "NXP G2i Serial", ex.Message));
                }

                itemList.Add(new ListItem(style, "EPC", tag.GetEpcString()));
                itemList.Add(new ListItem(style, "DATA", tag.GetDataString()));
            });
        }

        async void OnExportClicked(object sender, EventArgs e)
        {              
            StringBuilder sb = new StringBuilder();

            foreach (ListItem td in itemList)
            {
                sb.Append(td.ItemHeaderText);
                sb.Append(",");
                sb.Append(td.ItemValueText);                
                sb.AppendLine();
            }

            await Utils.Export(this, "TagInfo", sb.ToString());

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Fill items to list               
            PrepareListItems();           
            //Assing item list to ListTemplate
            TagInfoList.SetItemsSource(itemList);

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

    }
}