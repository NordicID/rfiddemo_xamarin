using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace nur_tools_rfiddemo_xamarin.Templates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListTemplate : Frame
    {
        public event EventHandler OnItemSelected;
            
        public ListTemplate()
        {
            InitializeComponent();          
        }


        public void SetItemsSource(ObservableCollection<ListItem> items)
        {            
            ListItemsView.ItemsSource = items;               
        }            

        void OnItemTapped(object sender, ItemTappedEventArgs args)
        {
            //Toggle selected item
            ListItem item = (ListItem)args.Item;
            if (item.mSelected)
            {
                item.mSelected = false;                
            }
            else
            {
                item.mSelected = true;                
            }                       

            EventHandler handler = OnItemSelected;
            handler?.Invoke(this, args);
            ((ListView)sender).SelectedItem = null; // de-select the row
        }                
    }

    public class ListItemStyle
    {
        public bool styleSingleRow;       
        public Color styleBkColor;
        public string styleImageSource;
        public string styleImageWidth;       
        public double styleCellHeight;        
        public Color styleHdrColor;
        public double styleHdrFontSize;
        public FontAttributes styleHdrFontAttribute;
        public string styleHdrFontFamily;

        public Color styleValueColor;
        public double styleValueFontSize;
        public FontAttributes styleValueFontAttribute;
        public string styleValueFontFamily;
        public double styleColSpan;

        public ListItemStyle()
        {
            Init();
        }

        private void Init()
        {
            styleSingleRow = true;          
            styleBkColor = Color.White;
            styleImageSource = "";
            styleImageWidth = "25";
            styleCellHeight = -1; // 40;
            styleHdrColor = Color.Black;
            styleValueColor = Color.Blue;
            styleHdrFontSize = 16;
            styleValueFontSize = 14;
            styleHdrFontFamily = Font.Default.FontFamily;
            styleValueFontFamily = Font.Default.FontFamily;
            styleHdrFontAttribute = Font.Default.FontAttributes;
            styleValueFontAttribute = Font.Default.FontAttributes;
            styleColSpan = 1;
        }

        public ListItemStyle(string imageSource, int imageWidth,Color bkColor, Color headerColor, Color valueColor)
        {
            Init();
                        
            styleImageSource = imageSource;
            styleImageWidth = imageWidth.ToString();
            styleBkColor = bkColor;
            styleHdrColor = headerColor;
            styleValueColor = valueColor;            
        }
    }

    public class ListItem : INotifyPropertyChanged
    {
        ListItemStyle mStyle;
        internal bool mSelected;        

        string mHeaderText;        
        string mValueText;        
        object mObject;


        public ListItem()
        {
            mStyle = new ListItemStyle();
            Init();

            ApplyStyle();          
        }             
        
        public ListItem(ListItemStyle style, string hdrText, string valueText,object myObject=null)
        {
            mStyle = style;
            Init();
                        
            ItemHeaderText = hdrText;
            ItemValueText = valueText;            
            mObject = myObject;
            ApplyStyle();
        }

        private void Init()
        {
            mSelected = false;           

            ItemHeaderText = "N/A";
            ItemValueText = "N/A";
            mObject = null;
        }

        private void ApplyStyle()
        {
            SingleRow = mStyle.styleSingleRow;
            BkColor = mStyle.styleBkColor;
            ItemHeaderTextColor = mStyle.styleHdrColor;
            ItemValueColor = mStyle.styleValueColor;            
            ImageSource = mStyle.styleImageSource;
            CellHeight = mStyle.styleCellHeight;
            ImageWidth = mStyle.styleImageWidth;
            ItemHeaderFontSize = mStyle.styleHdrFontSize;
            ItemValueFontSize = mStyle.styleValueFontSize;
            ItemValueFontFamily = mStyle.styleValueFontFamily;
            ItemValueFontAttribute = mStyle.styleValueFontAttribute;
            ItemHeaderFontFamily = mStyle.styleHdrFontFamily;
            ItemHeaderFontAttribute = mStyle.styleHdrFontAttribute;
            ItemHeaderColSpan = mStyle.styleColSpan;
        }

        /// <summary>
        /// Select item. When user click item next time, value will toggled
        /// </summary>
        public bool Selected { get { return mSelected; } set { mSelected = value; } }       

        public double ItemHeaderColSpan
        {
            get { return mStyle.styleColSpan; }
            set
            {
                if (value != mStyle.styleColSpan)
                {
                    mStyle.styleColSpan = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool SingleRow
        {
            get { return mStyle.styleSingleRow; }
            set
            {
                if (value != mStyle.styleSingleRow)
                {
                    mStyle.styleSingleRow = value;                                       
                    NotifyPropertyChanged();
                }
            }
        }
                
        /// <summary>
        /// Set background color for item
        /// </summary>
        public Color BkColor
        {
            get { return mStyle.styleBkColor; }
            set
            {
                if (value != mStyle.styleBkColor)
                {
                    mStyle.styleBkColor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Set image for list item
        /// </summary>
        /// <param name="imageSource">name of image asset</param>
        /// <param name="width">requested width of image</param>
        public void SetImage(string imageSource, int width)
        {
            ImageSource = imageSource;
            ImageWidth = width.ToString();
        }

        public string ImageSource
        {
            get { return mStyle.styleImageSource; }
            set
            {
                if (value != mStyle.styleImageSource)
                {
                    mStyle.styleImageSource = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ImageWidth
        {
            get { return mStyle.styleImageWidth; }
            set
            {
                if (value != mStyle.styleImageWidth)
                {
                    mStyle.styleImageWidth = value;
                    NotifyPropertyChanged();
                }
            }
        }
                
        public object MyObject
        {
            get { return mObject; }
            set
            {               
               mObject = value;                    
            }
        }


        public double CellHeight
        {
            get { return mStyle.styleCellHeight; }
            set
            {                
                if (value != mStyle.styleCellHeight)
                {
                    mStyle.styleCellHeight = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ItemHeaderText
        {
            get { return mHeaderText; }
            set
            {
                if (value != mHeaderText)
                {
                    mHeaderText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double ItemHeaderFontSize
        {
            get { return mStyle.styleHdrFontSize; }
            set
            {
                if (value != mStyle.styleHdrFontSize)
                {
                    mStyle.styleHdrFontSize = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FontAttributes ItemHeaderFontAttribute
        {
            get { return mStyle.styleHdrFontAttribute; }
            set
            {
                if (value != mStyle.styleHdrFontAttribute)
                {
                    mStyle.styleHdrFontAttribute = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ItemHeaderFontFamily
        {
            get { return mStyle.styleHdrFontFamily; }
            set
            {
                if (value != mStyle.styleHdrFontFamily)
                {
                    mStyle.styleHdrFontFamily = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ItemValueText
        {
            get { return mValueText; }
            set
            {
                if (value != mValueText)
                {
                    mValueText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double ItemValueFontSize
        {
            get { return mStyle.styleValueFontSize; }
            set
            {
                if (value != mStyle.styleValueFontSize)
                {
                    mStyle.styleValueFontSize = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public FontAttributes ItemValueFontAttribute
        {
            get { return mStyle.styleValueFontAttribute; }
            set
            {
                if (value != mStyle.styleValueFontAttribute)
                {
                    mStyle.styleValueFontAttribute = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ItemValueFontFamily
        {
            get { return mStyle.styleValueFontFamily; }
            set
            {
                if (value != mStyle.styleValueFontFamily)
                {
                    mStyle.styleValueFontFamily = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Color ItemHeaderTextColor
        {
            get { return mStyle.styleHdrColor; }
            set
            {
                if (value != mStyle.styleHdrColor)
                {
                    mStyle.styleHdrColor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Color ItemValueColor
        {
            get { return mStyle.styleValueColor; }
            set
            {
                if (value != mStyle.styleValueColor)
                {
                    mStyle.styleValueColor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class InverseBoolConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
            //throw new NotImplementedException();
        }


        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

}