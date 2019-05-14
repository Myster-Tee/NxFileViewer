using System;
using System.Collections.Generic;
using System.IO;
using Emignatik.NxFileViewer.Logging;
using Emignatik.NxFileViewer.NxFormats.NACP.Models;
using Emignatik.NxFileViewer.NxFormats.NACP.Structs;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.NxFormats.NACP
{
    /// <inheritdoc />
    /// <summary>
    /// A NACP file contains various information about an application (like localized app name and publisher).
    /// 
    /// Based on SwitchBrew specification here: "https://switchbrew.org/wiki/Control.nacp"
    /// </summary>
    public class NacpReader : IDisposable
    {
        private const int NACP_FILE_SIZE = 0x4000;
        private readonly Stream _stream;

        public NacpReader(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));

            if (_stream.Length != NACP_FILE_SIZE)
                throw new Exception($"Unexpected NACP file size, expected \"{NACP_FILE_SIZE}\" but was \"{_stream.Length}\".");
        }

        public NacpContent ReadContent()
        {
            _stream.Position = 0;
            var nacpStruct = _stream.ReadStruct<NacpStruct>();
            return BuildContent(nacpStruct);
        }

        private NacpContent BuildContent(NacpStruct nacpStruct)
        {
            var titles = nacpStruct.Titles;
            if (titles == null)
                throw new Exception($"Invalid NACP struct, \"{nameof(nacpStruct.Titles)}\" is null.");

            var languages = (NacpLanguage[])Enum.GetValues(typeof(NacpLanguage));
            if (titles.Length != languages.Length)
                throw new Exception($"Invalid NACP struct, number of \"{nameof(nacpStruct.Titles)}\" ({titles.Length}) should match the number of \"{nameof(NacpLanguage)}\" ({languages.Length}).");

            var nacpTitles = new List<NacpTitle>();
            for (var i = 0; i < titles.Length; i++)
            {
                var appName = titles[i].AppName.AsNullTerminatedString();
                var publisher = titles[i].Publisher.AsNullTerminatedString();

                if (string.IsNullOrEmpty(appName) && string.IsNullOrEmpty(publisher))
                    continue;

                nacpTitles.Add(new NacpTitle
                {
                    AppName = appName,
                    Publisher = publisher,
                    Language = languages[i],
                });
            }

            return new NacpContent
            {
                Titles = nacpTitles.ToArray(),
                RawStruct = nacpStruct
            };
        }

        public void Dispose()
        {
            try
            {
                _stream.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to dispose NACP stream.", ex);
            }
        }

        public static NacpReader FromFile(string filePath)
        {
            return new NacpReader(File.OpenRead(filePath));
        }
    }
}