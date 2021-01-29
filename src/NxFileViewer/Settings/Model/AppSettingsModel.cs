using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings.Model
{
    /// <summary>
    /// Model for the serialization
    /// </summary>
    public class AppSettingsModel
    {
        public string LastOpenedFile { get; set; }

        public string KeysFilePath { get; set; }

        public string TitleKeysFilePath { get; set; }

        public string ConsoleKeysFilePath { get; set; }

        public string LastSaveDir { get; set; } = "";

        // TODO: sauvegarder a partir du nom
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        public string? ProdKeysDownloadUrl { get; set; }

        public StructureLoadingMode StructureLoadingMode { get; set; }
    }
}
