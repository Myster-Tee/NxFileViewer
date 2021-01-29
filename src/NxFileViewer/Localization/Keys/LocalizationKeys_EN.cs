using System;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Utils.MVVM.Localization;

namespace Emignatik.NxFileViewer.Localization.Keys
{
    public class LocalizationKeys_EN : LocalizationKeysBase, ILocalizationKeys
    {
        public override bool IsFallback => true;
        public override string DisplayName => "English";
        public override string CultureName => "en-US";
        public override string LanguageAuto => "Auto";

        public string ErrKeysLoadingFailed => "Failed to load keys: {0}.";
        public string WarnNoProdKeysFileFound => "No prod.keys file found.";

        public string InvalidSetting_KeysFileNotFound => "Keys file «{0}» defined in the settings doesn't exist.";

        public string ErrFileNotSupported => "File «{0}» not supported.";
        public string OpenFile_Filter => "Nintendo Switch files (*.nsp;*.nsz;*.xci;*.xcz)|*.nsp;*.nsz;*.xci;*.xcz|All files (*.*)|*.*";
        public string MenuItemFile => "_File";
        public string MenuItemOpen => "_Open...";
        public string MenuItemOpenLast => "Open _last";
        public string MenuItemClose => "_Close";
        public string MenuItemExit => "E_xit";
        public string MenuItemOptions => "_Options";
        public string MenuItemSettings => "_Settings";

        public string AvailableContents => "Contents";
        public string GeneralInfo => "General Info";
        public string TitleId => "Title ID";
        public string DisplayVersion => "Display Version";
        public string Presentation => "Presentation";
        public string AvailableLanguages => "Languages";
        public string AppTitle => "Title";
        public string Publisher => "Publisher";

        public string Lng_Unknown => "Unknown";
        public string Lng_AmericanEnglish => "American";
        public string Lng_BritishEnglish => "English";
        public string Lng_CanadianFrench => "Canadian French";
        public string Lng_Dutch => "Dutch";
        public string Lng_French => "French";
        public string Lng_German => "German";
        public string Lng_Italian => "Italian";
        public string Lng_Japanese => "Japanese";
        public string Lng_Korean => "Korean";
        public string Lng_LatinAmericanSpanish => "Latin America";
        public string Lng_Portuguese => "Portuguese";
        public string Lng_Russian => "Russian";
        public string Lng_SimplifiedChinese => "Simplified Chinese";
        public string Lng_Spanish => "Spanish";
        public string Lng_TraditionalChinese => "Traditional Chinese";

        public string SettingsWindowTitle => "Settings";
        public string SettingsButton_Save => "Save";
        public string SettingsButton_Cancel => "Cancel";

        public string SettingsView_ProdKeys => KeySetProviderService.DefaultProdKeysFileName;
        public string SettingsView_ProdKeysToolTip => "This file contains common keys used by all Switch devices. This file is required for opening encrypted title files." + Environment.NewLine +
                                                      "The program will search this file in the following order:" + Environment.NewLine +
                                                      "    1. the path defined by this setting" + Environment.NewLine +
                                                      "    2. the current program's directory" + Environment.NewLine +
                                                      "    3. the «%UserProfile%\\.switch» directory";

        public string SettingsView_ConsoleKeys => KeySetProviderService.DefaultConsoleKeysFileName;
        public string SettingsView_ConsoleKeysToolTip => "You can optionally specify a file containing console-unique keys." + Environment.NewLine +
                                                         "The program will search this file in the following locations:" + Environment.NewLine +
                                                         "    1. the path defined by this setting" + Environment.NewLine +
                                                         "    2. the current program's directory" + Environment.NewLine +
                                                         "    3. the «%UserProfile%\\.switch» directory";

        public string SettingsView_TitleKeys => KeySetProviderService.DefaultTitleKeysFileName;
        public string SettingsView_TitleKeysToolTip => "You can optionally specify a file containing game-specific keys." + Environment.NewLine +
                                                       "The program will search this file in the following locations:" + Environment.NewLine +
                                                       "    1. the path defined by this setting" + Environment.NewLine +
                                                       "    2. the current program's directory" + Environment.NewLine +
                                                       "    3. the «%UserProfile%\\.switch» directory";

        public string SettingsView_GroupBoxKeys => "Keys";
        public string SettingsView_StructureLoadingMode => "Structure loading mode";
        public string SettingsView_StructureLoadingModeToolTip => "Lazy: child items will be loaded on expand" + Environment.NewLine +
                                                                  "Full: child items will be loaded immediately";

        public string BrowseKeysFile_ProdTitle => "Select \"prod\" keys file";
        public string BrowseKeysFile_ConsoleTitle => "Select \"console\" keys file";
        public string BrowseKeysFile_TitleTitle => "Select \"title\" keys file";
        public string BrowseKeysFile_Filter => "Keys files (*.keys)|*.keys|All files (*.*)|*.*";

        public string SuspiciousFileExtension => "File extension «{0}» seems invalid, «{1}» or «{2}» was expected.";
        public string DragMeAFile => "Drag me any supported file here :)";
        public string MultipleFilesDragAndDropNotSupported => "Multiple files drag and drop not supported, only the first file will be opened.";
        public string ContentType => "Type";
        public string TitleVersion => "Version";
        public string MinimumSystemVersion => "Minimum system version";

        public string ContextMenu_SaveImage => "Save...";
        public string CopyTitleImageError => "Failed to copy title image: {0}";
        public string SaveTitleImageError => "Failed to save title image: {0}";
        public string SaveFileDialog_SaveImageTitle => "Save image";
        public string SaveFileDialog_JpegImageFilter => "Image";
        public string ContextMenuCopyImage => "Copy";

        public string TabOverview => "Overview";
        public string TabContent => "Content";
        public string GroupBoxStructure => "Structure";
        public string GroupBoxProperties => "Properties";
        public string ContextMenu_SaveFile => "Save...";
        public string SaveFileDialog_SaveFileTitle => "Save file";
        public string SaveFileDialog_AnyFileFilter => "File";
        public string SaveFileTitleError => "Failed to save file: {0}";
        public string SettingsLoadingError => "Failed to load settings: {0}";
        public string SettingsSavingError => "Failed to save settings: {0}";

        public string LoadingError_Failed => "Failed to load file «{0}»: {1}";
        public string LoadingError_FailedToCheckIfXciPartitionExists => "Failed to check if XCI partition exists: {0}";
        public string LoadingError_FailedToOpenXciPartition => "Failed to open XCI partition: {0}";
        public string LoadingError_FailedToLoadXciContent => "Failed to load XCI content: {0}";
        public string LoadingError_FailedToOpenPartitionFile => "Failed to open partition file: {0}";
        public string LoadingError_FailedToLoadNcaFile => "Failed to load NCA file: {0}";
        public string LoadingError_FailedToLoadPartitionFileSystemContent => "Failed to load partition file system content: {0}";
        public string LoadingError_FailedToCheckIfSectionCanBeOpened => "Failed to check if section can be opened: {0}";
        public string LoadingError_FailedToOpenNcaFileSystem => "Failed to open NCA file system: {0}";
        public string LoadingError_FailedToLoadSectionContent => "Failed to load section content: {0}";
        public string LoadingError_FailedToGetFileSystemDirectoryEntries => "Failed to get file system directory entries: {0}";
        public string LoadingError_FailedToOpenNacpFile => "Failed to open NACP file: {0}";
        public string LoadingError_FailedToLoadNacpFile => "Failed to load NACP file: {0}";
        public string LoadingError_FailedToOpenCnmtFile => "Failed to open CNMT file: {0}";
        public string LoadingError_FailedToLoadCnmtFile => "Failed to load CNMT file: {0}";
        public string LoadingError_FailedToLoadNcaContent => "Failed to load NCA content: {0}";
        public string LoadingError_FailedToLoadDirectoryContent => "Failed to load directory content: {0}";
        public string LoadingError_FailedToLoadIcon => "Failed to load icon: {0}";
        public string LoadingError_NcaFileMissing => "NCA entry «{0}» of type «{1}» missing.";
        public string LoadingError_NoCnmtFound => "No CNMT entry found!";
        public string LoadingError_NacpFileMissing => "NCAP file «{0}» not found!";
        public string LoadingError_IconMissing => "Expected icon file «{0}» missing.";
        public string LoadingError_XciSecurePartitionNotFound => "XCI secure partition not found!";
        public string LoadingError_FailedToGetNcaFsHeader => "Failed to get NCA File System Header of section «{0}»: {1}";

        public string KeysFileUsed => "«{0}» file used: {1}";
        public string NoneKeysFile => "[none]";

        public string FailedToDownloadProdKeysFromUrl => $"Failed to download «{KeySetProviderService.DefaultProdKeysFileName}» from URL «{{0}}»: {{1}}";
        public string DownloadingProdKeysFromUrl => $"Downloading «{KeySetProviderService.DefaultProdKeysFileName}» from URL «{{0}}»...";
        public string ProdKeysSuccessfullyDownloaded => $"«{KeySetProviderService.DefaultProdKeysFileName}» successfully downloaded.";
        public string ToolTip_PatchLevel => "Patch level {0}";
        public string Log_OpeningFile => "=====> {0} <=====";
    }
}