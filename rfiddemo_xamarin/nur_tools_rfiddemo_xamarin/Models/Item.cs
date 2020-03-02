using System;

namespace nur_tools_rfiddemo_xamarin.Models
{
    public enum MainPageItem
    {
        Connection,
        Settings,
        Inventory,
        SensorTag,
        Barcode
    }

    public enum SettingsPageItem
    {
        RFID,
        Reader,
        Inventory,
        InventoryRead,
        InventoryEx,
        Sensors,
        Antenna,
        Barcode,
        Application,
        StoreSetup
    }

    public class Item
    {
        public MainPageItem itemMain { get; set; }
        public SettingsPageItem itemSettings { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
    }
}