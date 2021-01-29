using Emignatik.NxFileViewer.Utils.MVVM.Localization;

namespace Emignatik.NxFileViewer.Localization.Keys
{
    public interface ILocalizationKeys : ILocalizationKeysBase
    {
        string ErrKeysLoadingFailed { get; }
        string WarnNoProdKeysFileFound { get; }

        string InvalidSetting_KeysFileNotFound { get; }

        string ErrFileNotSupported { get; }
        string OpenFile_Filter { get; }
        string MenuItemFile { get; }
        string MenuItemOpen { get; }
        string MenuItemOpenLast { get; }
        string MenuItemClose { get; }
        string MenuItemExit { get; }
        string MenuItemOptions { get; }
        string MenuItemSettings { get; }

        string AvailableContents { get; }
        string GeneralInfo { get; }
        string TitleId { get; }
        string DisplayVersion { get; }
        string Presentation { get; }
        string AvailableLanguages { get; }
        string AppTitle { get; }
        string Publisher { get; }

        string Lng_Unknown { get; }
        string Lng_AmericanEnglish { get; }
        string Lng_BritishEnglish { get; }
        string Lng_CanadianFrench { get; }
        string Lng_Dutch { get; }
        string Lng_French { get; }
        string Lng_German { get; }
        string Lng_Italian { get; }
        string Lng_Japanese { get; }
        string Lng_Korean { get; }
        string Lng_LatinAmericanSpanish { get; }
        string Lng_Portuguese { get; }
        string Lng_Russian { get; }
        string Lng_SimplifiedChinese { get; }
        string Lng_Spanish { get; }
        string Lng_TraditionalChinese { get; }

        string SettingsWindowTitle { get; }
        string SettingsButton_Save { get; }
        string SettingsButton_Cancel { get; }
        string SettingsView_ProdKeys { get; }
        string SettingsView_ProdKeysToolTip { get; }
        string SettingsView_ConsoleKeys { get; }
        string SettingsView_ConsoleKeysToolTip { get; }
        string SettingsView_TitleKeys { get; }
        string SettingsView_TitleKeysToolTip { get; }
        string SettingsView_GroupBoxKeys { get; }
        string SettingsView_StructureLoadingMode { get; }
        string SettingsView_StructureLoadingModeToolTip { get; }

        string BrowseKeysFile_ProdTitle { get; }
        string BrowseKeysFile_ConsoleTitle { get; }
        string BrowseKeysFile_TitleTitle { get; }
        string BrowseKeysFile_Filter { get; }

        string SuspiciousFileExtension { get; }
        string DragMeAFile { get; }
        string MultipleFilesDragAndDropNotSupported { get; }
        string ContentType { get; }
        string TitleVersion { get; }
        string MinimumSystemVersion { get; }

        string ContextMenu_SaveImage { get; }
        string CopyTitleImageError { get; }
        string SaveTitleImageError { get; }
        string SaveFileDialog_SaveImageTitle { get; }
        string SaveFileDialog_JpegImageFilter { get; }
        string ContextMenuCopyImage { get; }

        string TabOverview { get; }
        string TabContent { get; }
        string GroupBoxStructure { get; }
        string GroupBoxProperties { get; }

        string ContextMenu_SaveFile { get; }
        string SaveFileDialog_SaveFileTitle { get; }
        string SaveFileDialog_AnyFileFilter { get; }
        string SaveFileTitleError { get; }
        string SettingsLoadingError { get; }
        string SettingsSavingError { get; }

        string LoadingError_Failed { get; }
        string LoadingError_FailedToCheckIfXciPartitionExists { get; }
        string LoadingError_FailedToOpenXciPartition { get; }
        string LoadingError_FailedToLoadXciContent { get; }
        string LoadingError_FailedToOpenPartitionFile { get; }
        string LoadingError_FailedToLoadNcaFile { get; }
        string LoadingError_FailedToLoadPartitionFileSystemContent { get; }
        string LoadingError_FailedToCheckIfSectionCanBeOpened { get; }
        string LoadingError_FailedToOpenNcaFileSystem { get; }
        string LoadingError_FailedToLoadSectionContent { get; }
        string LoadingError_FailedToGetFileSystemDirectoryEntries { get; }
        string LoadingError_FailedToOpenNacpFile { get; }
        string LoadingError_FailedToLoadNacpFile { get; }
        string LoadingError_FailedToOpenCnmtFile { get; }
        string LoadingError_FailedToLoadCnmtFile { get; }
        string LoadingError_FailedToLoadNcaContent { get; }
        string LoadingError_FailedToLoadDirectoryContent { get; }
        string LoadingError_FailedToLoadIcon { get; }
        string LoadingError_NcaFileMissing { get; }
        string LoadingError_NoCnmtFound { get; }
        string LoadingError_NacpFileMissing { get; }
        string LoadingError_IconMissing { get; }
        string LoadingError_XciSecurePartitionNotFound { get; }
        string LoadingError_FailedToGetNcaFsHeader { get; }

        string KeysFileUsed { get; }
        string NoneKeysFile { get; }
        string FailedToDownloadProdKeysFromUrl { get; }
        string DownloadingProdKeysFromUrl { get; }
        string ProdKeysSuccessfullyDownloaded { get; }
        string ToolTip_PatchLevel { get; }
    }
}
