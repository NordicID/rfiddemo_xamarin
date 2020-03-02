using System;
using System.Collections.Generic;
using System.Text;

namespace nur_tools_rfiddemo_xamarin.Models
{
    public enum MenuItemType
    {         
        Connection,
        About,
        Updates
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
