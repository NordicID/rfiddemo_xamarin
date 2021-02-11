using System;
using System.Collections.Generic;
using System.Text;

namespace nur_tools_rfiddemo_xamarin
{
    /// <summary>
    /// IndexDictionary is used in settings where user select item from list.
    /// </summary>
    public class IndexDictionary
    {
        readonly Dictionary<string, int> indexDict;

        /// <summary>
        /// Create index dictionary from key strings specified on itemArray
        /// </summary>
        /// <param name="itemArray">key string array</param>
        public IndexDictionary(string[] itemArray)
        {
            indexDict = new Dictionary<string, int>();
            lock (indexDict)
            {
                indexDict.Clear();
                for (int x = 0; x < itemArray.Length; x++)
                    indexDict.Add(itemArray[x], x);
            }
        }
                
        /// <summary>
        /// Get specified index based on key.<br/>
        /// if key value = "Cancel" -1 returned.
        /// </summary>
        /// <param name="itemKey">key string</param>
        /// <returns>index of specified item. -1 if key is "Cancel" or key not found</returns>
        public int GetItemIndex(string itemKey)
        {
            lock (indexDict)
            {
                if (string.IsNullOrEmpty(itemKey))
                    return -1;
                else if (itemKey == "Cancel")
                    return -1;

                return indexDict[itemKey];
            }
        }
    }
}
