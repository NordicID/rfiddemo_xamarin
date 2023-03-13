using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace nur_tools_rfiddemo_xamarin
{
    class Utils
    {
        /// <summary>
		/// Validate given string value and convert to int
		/// </summary>
		/// <param name="result">string to validate</param>
		/// <param name="min">min accepted value</param>
		/// <param name="max">max accepted value</param>
		/// <returns>converted int value</returns>
		/// <exception cref="Exception" if validate fails></exception>
		public static int ValidateAndConvertToInt(string result, int min, int max)
        {
            int val;

            if (string.IsNullOrEmpty(result))
                throw new Exception("Cancel or no value");

            try
            {
                val = Convert.ToInt32(result);
                if (val < min || val > max)
                    throw new Exception("Value not in range. Must be " + min.ToString() + "-" + max.ToString());

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return val;
        }

        /// <summary>
        /// Convert and validate string representation of int value
        /// </summary>
        /// <param name="intString">string value to validate</param>
        /// <param name="min">min acceptable value</param>
        /// <param name="max">max acceptable value</param>
        /// <returns>converted value as int</returns>
        /// <exception cref="Exception" if value not in range or cannot convert to int></exception>
        public static int ConvertAndValidate(string intString, int min, int max)
        {
            int val = 0;

            if (string.IsNullOrEmpty(intString))
                throw new Exception("Cancel or no value");

            try
            {
                val = Convert.ToInt32(intString);
                if (val < min || val > max)
                    throw new Exception("Value not in range. Must be " + min.ToString() + "-" + max.ToString());

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return val;
        }

        public static async Task Export(ContentPage page, string headerText, string items, string filetype = ".csv")
        {
            string fn = headerText + "_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + filetype;
            fn = fn.Replace(' ', '_');
                        
            if (DeviceInfo.Model.StartsWith("HH"))
            {
                var sharePath = "/storage/emulated/0/RFID_EXPORT/";

                if (!File.Exists(sharePath))
                {
                    Directory.CreateDirectory(sharePath);
                }

                string filePath = Path.Combine(sharePath, fn);
                File.WriteAllText(filePath, items);

                bool response = await page.DisplayAlert("File write success!", headerText + Environment.NewLine + "wrote to " + Environment.NewLine + filePath, "OK", "Email..");

                if (response == false)
                {
                    //If HH8x device has email account defined, inventory results are added to the body of email
                    EmailMessage message = new EmailMessage();
                    message.Subject = headerText;
                    //List<string> list = new List<string>();
                    //list.Add("mail@domain.com");
                    //message.To = list;
                    message.Body = items;
                    await Email.ComposeAsync(message);
                }

                return;                               
            }
            else
            {
                //Allow user to select destination device offer..(whatsApp, email Dropbox..)
                var file = Path.Combine(FileSystem.CacheDirectory, fn);
                try
                {
                    File.WriteAllText(file, items);                               
                    ShareFileRequest share = new ShareFileRequest();
                    share.Title = "Share " + headerText;
                    share.File = new ShareFile(file);
                    await Share.RequestAsync(share);
                }
                catch (Exception ex1)
                {
                    await page.DisplayAlert("Share Error", ex1.Message, "OK");
                    return;
                }
            }
        }
    }
}
