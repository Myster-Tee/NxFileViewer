﻿using Emignatik.NxFileViewer.Utils.MVVM.Localization;

// ReSharper disable InconsistentNaming

namespace Emignatik.NxFileViewer.Localization.Keys
{
    public interface ILocalizationKeys : ILocalizationKeysBase
    {
        string FileNotSupported_Log { get; }
        string OpenFile_Filter { get; }

        string MenuItem_File { get; }
        string MenuItem_Open { get; }
        string MenuItem_OpenLast { get; }
        string MenuItem_Close { get; }
        string MenuItem_Exit { get; }
        string MenuItem_Tools { get; }
        string MenuItem_VerifyNcasHash { get; }
        string MenuItem_VerifyNcasHeaderSignature { get; }
        string MenuItem_Options { get; }
        string MenuItem_Settings { get; }
        string MenuItem_ReloadKeys { get; }

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
        string SettingsView_Title_ActualKeysFilePath { get; }
        string SettingsView_Title_KeysCustomFilePath { get; }
        string SettingsView_Title_KeysDownloadUrl { get; }
        string SettingsView_ToolTip_ProdKeys { get; }
        string SettingsView_ToolTip_ConsoleKeys { get; }
        string SettingsView_ToolTip_TitleKeys { get; }
        string SettingsView_LogLevel { get; }
        string SettingsView_ToolTip_LogLevel { get; }
        string SettingsView_CheckBox_AlwaysReloadKeysBeforeOpen { get; }

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
        string LoadingError_FailedToOpenNcaSectionFileSystem { get; }
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
        string LoadingError_FailedToGetNcaSectionFsHeader { get; }
        string LoadingError_FailedToOpenMainFile { get; }
        string LoadingError_FailedToLoadMainFile { get; }
        string LoadingError_FailedToLoadTicketFile { get; }
        string LoadingError_FailedToLoadTitleIdKey { get; }
        string LoadingInfo_TitleIdKeySuccessfullyInjected { get; }
        string LoadingWarning_TitleIdKeyReplaced { get; }
        string LoadingDebug_TitleIdKeyAlreadyExists { get; }

        string KeysFileUsed { get; }
        string NoneKeysFile { get; }

        string Status_DownloadingFile { get; }
        string Log_DownloadingFileFromUrl { get; }
        string Log_FileSuccessfullyDownloaded { get; }
        string Log_FailedToDownloadFileFromUrl { get; }

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

        string NcaHeaderSignature_VerificationStart_Log { get; }
        string NcaHeaderSignature_VerificationEnd_Log { get; }
        string NcaHeaderSignature_Valid_Log { get; }
        string NcaHeaderSignature_Invalid { get; }
        string NcaHeaderSignature_Invalid_Log { get; }
        string NcaHeaderSignature_Error { get; }
        string NcaHeaderSignature_Error_log { get; }
        string NcasHeaderSignature_Error_Log { get; }

        string NcaSectionHash_VerificationStart_Log { get; }
        string NcaSectionHash_VerificationEnd_Log { get; }
        string NcaSectionHash_Valid_Log { get; }
        string NcaSectionHash_Invalid { get; }
        string NcaSectionHash_Invalid_Log { get; }
        string NcaSectionHash_Error { get; }
        string NcaSectionHash_Error_Log { get; }
        string NcasSectionHash_Error_Log { get; }

        string CancelAction { get; }
        string Status_Ready { get; }
        string LoadingFile_PleaseWait { get; }

        string NcasValidity_NoNca { get; }
        string NcasValidity_Unchecked { get; }
        string NcasValidity_InProgress { get; }
        string NcasValidity_Invalid { get; }
        string NcasValidity_Valid { get; }
        string NcasValidity_Error { get; }
        string NcasValidity_Unknown { get; }

        string Status_SavingFile { get; }

        string KeysLoading_Starting_Log { get; }
        string KeysLoading_Successful_Log { get; }
        string KeysLoading_Error_Log { get; }
        string KeysLoading_Error { get; }
        string WarnNoProdKeysFileFound { get; }
        string InvalidSetting_KeysFileNotFound { get; }

        string ToolTip_KeyMissing { get; }

        string EditFile_Failed_Log { get; }
        string MenuItem_CopyTextToClipboard { get; }

        string ContextMenu_OpenFileLocation { get; }
        string OpenFileLocation_Failed_Log { get; }
    }
}
