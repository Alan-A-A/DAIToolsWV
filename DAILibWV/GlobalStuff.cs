using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAILibWV
{
    /// <summary>
    /// Static class representing a <see cref="Dictionary{TKey, TValue}"/> with helper methods.
    /// </summary>
    public static class GlobalStuff
    {
        public static Dictionary<string, string> settings;

        /// <summary>
        /// Searches the dictionary for an entry with the given key.
        /// </summary>
        /// <param name="name">The key to use for search.</param>
        /// <returns></returns>
        public static string FindSetting(string name)
        {
            foreach (KeyValuePair<string, string> setting in settings)
                if (setting.Key == name)
                    return setting.Value;
            return "";
        }

        /// <summary>
        /// Writes the given value using the given key.
        /// </summary>
        /// <param name="key">The key to be used.</param>
        /// <param name="value">The value to be assigned to the given key.</param>
        public static void AssignSetting(string key, string value)
        {
            settings[key] = value;
            DBAccess.SaveSettings();
        }
    }
}
