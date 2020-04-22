using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
