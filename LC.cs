/*
 * LiteConfig (.lc) Library
 *
 * Description:
 *     LiteConfig (.lc) is a lightweight configuration file format designed for ease of use.
 *     This library provides functionalities to read values in standard types like int, float, double,
 *     bool, and string with dedicated methods. Additional types may be dynamically interpreted.
 *     The LC class offers an optional dictionary caching mechanism using the Data property for dictionary access.
 *     The format is designed to be simple and barebones with minimal "extras".
 *
 * Features:
 *     - Supports standard types: int, float, double, bool, and string.
 *     - Supports additional types: DateTime and List<T> (where T is a supported standard type or DateTime).
 *     - Dictionary caching using the Data property.
 *     - Simplified barebones design.
 *
 * Installation:
 *     Via NuGet: `Install-Package LiteConfig`
 *
 * Usage Rights:
 *     Users are granted full freedom to distribute, decompile, bundle, or repurpose this library as needed.
 *     See the readme for detailed rights and permissions.
 *
 * License:
 *     GPL License: https://www.gnu.org/licenses/gpl-3.0.en.html#license-text
 *
 * Repository:
 *     GitHub: https://github.com/SteveOhIo/LiteConfig
 * 
 * Author: SteveOh
 * Date: 21/10/2023
 * Version: 1.0.2
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LiteConfig
{
    /// <summary>
    /// Represents the LiteConfig (.lc) class, a lightweight configuration file format designed for ease of use. 
    /// Provides functionalities to read values in standard types like int, float, double, bool, and string with dedicated methods.
    /// Other types will attempt to be dynamically understood. The class also offers an optional dictionary caching mechanism
    /// using the Data property for dictionary access. Designed to be barebones with very few "extras" for simplicity.
    /// </summary>
    public class LC
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        private static LC LoadFile(string filePath)
        {
            LC lc = new LC();
            lc.LoadFromFile(filePath);
            return lc;
        }

        #region Read/Write Methods

        // ReSharper disable MemberCanBePrivate.Global

        /// <summary>
        /// Reads a value of a specified type from a specified LC file
        /// </summary>
        /// <param name="filePath">The path to the LC file</param>
        /// <param name="key">The key of the value to read</param>
        /// <typeparam name="T">The type of the value to read</typeparam>
        /// <returns>The value of the specified type</returns>
        /// <exception cref="InvalidDataException">Thrown when the value is not of the specified type</exception>
        public static T ReadValue<T>(string filePath, string key)
        {
            LC lc = LoadFile(filePath);
            try
            {
                return (T)Convert.ChangeType(lc.Data[key], typeof(T));
            }
            catch
            {
                throw new InvalidDataException(
                    $"Invalid data type for key \"{key}\". Attempted to convert \"{lc.Data[key]}\" to type \"{typeof(T)}\"");
            }
        }

        /// <summary>
        /// Writes a value with a specified key to a specified LC file
        /// </summary>
        /// <param name="filePath">The path to the LC file</param>
        /// <param name="key">The key of the value to write</param>
        /// <param name="value">The value to write</param>
        /// <typeparam name="T">The type of the value to write</typeparam>
        public static void WriteValue<T>(string filePath, string key, T value)
        {
            LC lc = LoadFile(filePath);
            lc.Data[key] = value.ToString();
            lc.WriteToFile(filePath);
        }

        // Shortcut methods for reading values of a specific type
        public static string ReadString(string filePath, string key) => LoadFile(filePath).Data[key] as string;

        public static int ReadInt(string filePath, string key) => ReadValue<int>(filePath, key);

        public static float ReadFloat(string filePath, string key) => ReadValue<float>(filePath, key);

        public static double ReadDouble(string filePath, string key) => ReadValue<double>(filePath, key);

        public static bool ReadBool(string filePath, string key) => ReadValue<bool>(filePath, key);

        /// <summary>
        /// Reads a DateTime value from a specified LC file
        /// </summary>
        /// <param name="filePath">The path to the LC file</param>
        /// <param name="key">The key of the value to read</param>
        /// <returns>The DateTime value</returns>
        public static DateTime ReadDateTime(string filePath, string key) => ReadValue<DateTime>(filePath, key);

        /// <summary>
        /// Reads a DateTime value from a specified LC file with a specified input format
        /// </summary>
        /// <param name="filePath">The path to the LC file</param>
        /// <param name="key">The key of the value to read</param>
        /// <param name="inputFormat">The input format of the DateTime value</param>
        /// <returns>The DateTime value</returns>
        public static DateTime ReadDateTime(string filePath, string key, string inputFormat)
            => DateTime.ParseExact(ReadString(filePath, key), inputFormat, CultureInfo.InvariantCulture);

        /// <summary>
        /// Reads a list of a specified type from an LC file
        /// </summary>
        /// <param name="filePath">The path to the LC file</param>
        /// <param name="key">The key of the list to read</param>
        /// <typeparam name="T">The type of the list to read</typeparam>
        /// <returns>The list of the specified type</returns>
        /// <exception cref="InvalidDataException">Thrown when the list is not of the specified type</exception>
        public static List<T> ReadList<T>(string filePath, string key)
        {
            string listString = ReadString(filePath, key);
            switch (typeof(T))
            {
                // Depending on the desired type of list, parse the string into a list of that type
                case var _ when typeof(T) == typeof(string):
                    return listString.Split(',').Select(s => s.Trim()).Cast<T>().ToList();
                case var _ when typeof(T) == typeof(int):
                    return listString.Split(',').Select(int.Parse).Cast<T>().ToList();
                case var _ when typeof(T) == typeof(float):
                    return listString.Split(',').Select(float.Parse).Cast<T>().ToList();
                case var _ when typeof(T) == typeof(double):
                    return listString.Split(',').Select(double.Parse).Cast<T>().ToList();
                case var _ when typeof(T) == typeof(bool):
                    return listString.Split(',').Select(bool.Parse).Cast<T>().ToList();
                case var _ when typeof(T) == typeof(DateTime):
                    return listString.Split(',').Select(DateTime.Parse).Cast<T>().ToList();
                default:
                    throw new InvalidDataException(
                        $"Invalid data type for key \"{key}\". Attempted to convert \"{listString}\" to type \"List<{typeof(T)}>\"");
            }
        }

        // ReSharper restore MemberCanBePrivate.Global

        #endregion

        /// <summary>
        /// Loads and parses data from a specified LC file
        /// </summary>
        /// <param name="filePath">The path to the LC file</param>
        /// <exception cref="InvalidDataException">Thrown when the file is not a valid LC file</exception>
        /// <exception cref="FileNotFoundException">Thrown when the file does not exist</exception>
        public void LoadFromFile(string filePath)
        {
            // Verify file validity
            if (filePath == null) throw new InvalidDataException("File path cannot be null");
            if (!filePath.EndsWith(".lc"))
                throw new InvalidDataException("File \"" + filePath + "\" is not a valid LC file");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File \"" + filePath + "\" not found");
            }

            // Ensure that the StreamReader is disposed of properly
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                string currentKey = null;
                string currentValue = "";
                bool isMultiLine = false;
                bool isList = false;
                List<string> currentList = new List<string>();

                // Reading each line until the end
                while ((line = sr.ReadLine()) != null)
                {
                    ParseLine(line);
                }

                if (isList)
                {
                    Data[currentKey] = currentList;
                }

                // Parses a line of text
                void ParseLine(string lineToParse)
                {
                    // Ignore whitespace and comments
                    if (string.IsNullOrWhiteSpace(lineToParse) || lineToParse.StartsWith("#")) return;

                    // DEPRECATED: This is legacy code from when lists were treated as multiline items, now lists are one line comma-separated
                    if (isList)
                    {
                        if (lineToParse.Trim().StartsWith("-"))
                        {
                            currentList.Add(lineToParse.Trim().Substring(1).Trim());
                        }
                        else
                        {
                            Data[currentKey] = currentList;
                            currentList = new List<string>();
                            isList = false;
                            ParseLine(lineToParse); // Parse the line again
                        }
                    }
                    else if (!isMultiLine)
                    {
                        // If the line is missing a colon, throw an exception
                        int colonIndex = lineToParse.IndexOf(":", StringComparison.Ordinal);
                        if (colonIndex == -1)
                            throw new InvalidDataException("Invalid line format on line \"" + lineToParse + "\'");

                        currentKey = lineToParse.Substring(0, colonIndex).Trim();
                        currentValue = lineToParse.Substring(colonIndex + 1).Trim();

                        // Multi/single line check
                        if (currentValue.StartsWith("\""))
                        {
                            isMultiLine = true;
                            currentValue = currentValue.Substring(1);
                        }
                        else if (currentValue == "")
                        {
                            isList = true;
                        }
                        else
                        {
                            Data[currentKey] = currentValue;
                        }
                    }
                    else
                    {
                        if (lineToParse.EndsWith("\""))
                        {
                            currentValue += "\n" + lineToParse.Substring(0, lineToParse.Length - 1);
                            Data[currentKey] = currentValue;
                            isMultiLine = false;
                        }
                        else
                        {
                            currentValue += "\n" + lineToParse;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes data to a file. Data will be formatted based on type.
        /// </summary>
        /// <param name="filePath">The path to the file where the data will be written</param>
        private void WriteToFile(string filePath)
        {
            if (!File.Exists(filePath)) File.Create(filePath);

            // Ensure that the StreamWriter is disposed of properly
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                foreach (KeyValuePair<string, object> kvp in Data) // Iterate through each key-value pair
                {
                    switch (kvp.Value)
                    {
                        // If the value is a string and contains a newline, wrap it in quotes
                        case string stringValue when stringValue.Contains("\n"):
                            sw.WriteLine($"{kvp.Key}: \"{stringValue}\"");
                            break;
                        case string stringValue: // If the value is a string, write it as-is
                            sw.WriteLine($"{kvp.Key}: {stringValue}");
                            break;
                        case List<string> listValue: // If the value is a list, write the items comma-separated
                            sw.WriteLine($"{kvp.Key}: {string.Join(", ", listValue)}");
                            break;
                    }
                }
            }
        }
    }
}