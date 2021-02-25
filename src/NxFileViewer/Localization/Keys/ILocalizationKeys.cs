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
        string MenuItemTools { get; }
        string MenuItem_VerifyNcasHash { get; }
        string MenuItem_VerifyNcasHeaderSignature { get; }
        string MenuItemOptions { get; }
        string MenuItemSettings { get; }

        string MultiContentPackage { get; }
        string GeneralInfo { get; }
        string TitleId { get; }
        string DisplayVersion { get; }
        string Presentation { get; }
        string ToolTip_AvailableLanguages { get; }
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

        string SettingsView_Title { get; }
        string SettingsView_Button_Save { get; }
        string SettingsView_Button_Cancel { get; }
        string SettingsView_GroupBoxKeys { get; }
        string SettingsView_Title_KeysFilePath { get; }
        string SettingsView_Title_KeysDownloadUrl { get; }
        string SettingsView_ToolTip_ProdKeys { get; }
        string SettingsView_ToolTip_ConsoleKeys { get; }
        string SettingsView_ToolTip_TitleKeys { get; }
        string SettingsView_LogLevel { get; }
        string SettingsView_ToolTip_LogLevel { get; }

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

        string SaveDialog_Title { get; }
        string SaveDialog_ImageFilter { get; }
        string SaveDialog_AnyFileFilter { get; }
        string SaveFile_Error { get; }

        string ContextMenuCopyImage { get; }

        string TabOverview { get; }
        string TabContent { get; }
        string GroupBoxStructure { get; }
        string GroupBoxProperties { get; }

        string ContextMenu_SaveItemFile { get; }
        string ContextMenu_ShowItemErrors { get; }

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
        string LoadingError_FailedToOpenNcaSection { get; }
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
        string LoadingError_FailedToOpenMainFile { get; }
        string LoadingError_FailedToLoadMainFile { get; }

        string KeysFileUsed { get; }
        string NoneKeysFile { get; }

        string FailedToDownloadProdKeysFromUrl { get; }
        string DownloadingProdKeysFromUrl { get; }
        string ProdKeysSuccessfullyDownloaded { get; }
        string ToolTip_PatchLevel { get; }
        string Log_OpeningFile { get; }
        string MainModuleIdTooltip { get; }
        string ATaskIsAlreadyRunning { get; }

        string Integrity { get; }
        string AvailableContents { get; }
        string MultiContentPackageToolTip { get; }

        string Title_NcasHeaderSignature { get; }
        string ToolTip_NcasHeaderSignature { get; }
        string Title_NcasHash { get; }
        string ToolTip_NcasHash { get; }
        string NcaHeaderSignatureValid_Log { get; }
        string NcaHeaderSignatureInvalid { get; }
        string NcaHeaderSignatureInvalid_Log { get; }
        string NcaHashValid_Log { get; }
        string NcaHashInvalid { get; }
        string NcaHashInvalid_Log { get; }
        string NcasHashError_Log { get; }
        string NcasHeaderSignatureError_Log { get; }

        string CancelAction { get; }
        string Ready { get; }
        string LoadingFile_PleaseWait { get; }

        string NcasValidity_NoNca { get; }
        string NcasValidity_Unchecked { get; }
        string NcasValidity_InProgress { get; }
        string NcasValidity_Invalid { get; }
        string NcasValidity_Valid { get; }
        string NcasValidity_Error { get; }
        string NcasValidity_Unknown { get; }

        string Status_SavingFile { get; }

        string Log_KeysLoadingStarting { get; }
        string Log_KeysLoadingSuccessful { get; }
    }
}
