using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Emignatik.NxFileViewer.Logging;

namespace Emignatik.NxFileViewer.KeysParsing
{
    public class KeysParser
    {
        public KeySet ParseKeys(string keysFilePath)
        {
            var dictionary = new Dictionary<string, byte[]>();

            var lineNumber = 0;

            foreach (var line in File.ReadLines(keysFilePath))
            {
                lineNumber++;

                var regex = new Regex("^\\s*(\\w+)\\s*=\\s*([0-9a-f]+)\\s*$", RegexOptions.IgnoreCase);
                var match = regex.Match(line);
                if (!match.Success) continue;

                var keyName = match.Groups[1].Value;

                if (dictionary.ContainsKey(keyName))
                {
                    Logger.LogError($"Duplicated key \"{keyName}\" found at line \"{lineNumber}\" of file \"{keysFilePath}\"."); //TODO : to localize
                    continue;
                }

                try
                {
                    var value = match.Groups[2].Value;
                    var keyBytes = KeyParser.HexStringToByteArray(value);
                    dictionary.Add(keyName, keyBytes);
                }
                catch
                {
                    Logger.LogError($"Invalid key found at line \"{lineNumber}\" of file \"{keysFilePath}\"."); //TODO : to localize
                }
            }

            return new KeySet(dictionary);
        }
    }
}