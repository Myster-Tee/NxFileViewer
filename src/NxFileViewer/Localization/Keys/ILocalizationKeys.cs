using Emignatik.NxFileViewer.Utils.MVVM.Localization;

// ReSharper disable InconsistentNaming

namespace Emignatik.NxFileViewer.Localization.Keys;

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
    string MenuItem_CheckIntegrity { get; }
    string MenuItem_Options { get; }
    string MenuItem_Settings { get; }
    string MenuItem_ReloadKeys { get; }
    string MenuItem_OpenTitleWebPage { get; }
    string MenuItem_ShowRenameToolWindow { get; }

    string Packages_Title { get; }
    string DisplayVersion { get; }
    string Presentation_Title { get; }
    string ToolTip_AvailableLanguages { get; }
    string AvailableLanguages { get; }
    string AppTitle { get; }
    string Publisher { get; }

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
    string Lng_BrazilianPortuguese { get; }
    string Lng_Unknown { get; }

    string SettingsView_Title { get; }
    string SettingsView_Button_Apply { get; }
    string SettingsView_Button_Cancel { get; }
    string SettingsView_Button_Reset { get; }
    string SettingsView_GroupBoxKeys { get; }
    string SettingsView_Title_UsedKeysFilePath { get; }
    string SettingsView_Title_KeysCustomFilePath { get; }
    string SettingsView_Title_KeysDownloadUrl { get; }
    string SettingsView_ToolTip_Keys { get; }
    string SettingsView_ToolTip_ProdKeys { get; }
    string SettingsView_ToolTip_ConsoleKeys { get; }
    string SettingsView_ToolTip_TitleKeys { get; }
    string SettingsView_LogLevel { get; }
    string SettingsView_ToolTip_LogLevel { get; }
    string SettingsView_CheckBox_AlwaysReloadKeysBeforeOpen { get; }
    string SettingsView_Title_Language { get; }

    string SettingsView_Title_NczOptions { get; }
    string SettingsView_ToolTip_NczBlockLessCompression { get; }
    string SettingsView_CheckBox_NczOpenBlocklessCompression { get; }

    string SettingsView_Title_Integrity { get; }
    string SettingsView_CheckBox_IgnoreMissingDeltaFragments { get; }
    string SettingsView_ToolTip_IgnoreMissingDeltaFragments { get; }

    string SettingsView_Miscellaneous { get; }
    string SettingsView_ToolTip_OpenKeysLocation { get; }
    string SettingsView_ToolTip_BrowseKeys { get; }
    string SettingsView_ToolTip_DownloadKeys { get; }

    string BrowseKeysFile_ProdTitle { get; }
    string BrowseKeysFile_ConsoleTitle { get; }
    string BrowseKeysFile_TitleTitle { get; }
    string BrowseKeysFile_Filter { get; }

    string SuspiciousFileExtension { get; }
    string DragMeAFile { get; }

    string MultipleFilesDragAndDropNotSupported { get; }

    string CnmtOverview_Title { get; }
    string CnmtOverview_TitleId { get; }
    string CnmtOverview_ContentType { get; }
    string CnmtOverview_TitleVersion { get; }
    string CnmtOverview_MinimumSystemVersion { get; }
    string CnmtOverview_BuildID { get; }
    string CnmtOverview_BuildID_NotAvailableBecauseSectionIsSparse { get; }
    string CnmtOverview_IsDemo { get; }

    string ContextMenu_SaveImage { get; }
    string CopyTitleImageError { get; }
    string SaveTitleImageError { get; }

    string SaveDialog_Title { get; }
    string SaveDialog_ImageFilter { get; }
    string SaveDialog_AnyFileFilter { get; }
    string SaveFile_Error { get; }

    string ContextMenu_CopyImage { get; }

    string TabOverview { get; }
    string TabContent { get; }
    string GroupBoxStructure { get; }
    string GroupBoxProperties { get; }

    string ContextMenu_ShowItemErrors { get; }
    string ContextMenu_SaveSectionItem { get; }
    string ContextMenu_SaveDirectoryItem { get; }
    string ContextMenu_SaveFileItem { get; }
    string ContextMenu_SavePartitionFileItem { get; }
    string ContextMenu_SaveNcaFileRaw { get; }
    string ContextMenu_SaveNcaFilePlaintext { get; }

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
    string LoadingError_FailedToLoadIcon_Log { get; }
    string LoadingError_NcaFileMissing_Log { get; }
    string LoadingError_NoCnmtFound_Log { get; }
    string LoadingError_NacpFileMissing_Log { get; }
    string LoadingError_NcaMissingSection_Log { get; }
    string LoadingError_MainFileMissing_Log { get; }
    string LoadingError_IconMissing_Log { get; }
    string LoadingError_XciSecurePartitionNotFound_Log { get; }
    string LoadingError_FailedToGetNcaSectionFsHeader { get; }
    string LoadingError_FailedToOpenMainFile { get; }
    string LoadingError_FailedToLoadMainFile { get; }
    string LoadingError_FailedToLoadTicketFile { get; }
    string LoadingError_FailedToLoadTitleIdKey { get; }
    string LoadingError_NczBlocklessCompressionDisabled { get; }

    string LoadingInfo_TitleIdKeySuccessfullyInjected { get; }
    string LoadingWarning_TitleIdKeyReplaced { get; }
    string LoadingDebug_TitleIdKeyAlreadyExists { get; }

    string KeysFileUsed { get; }
    string NoneKeysFile { get; }

    string Status_DownloadingFile { get; }
    string Log_DownloadingFileFromUrl { get; }
    string Log_FileSuccessfullyDownloaded { get; }
    string Log_FailedToDownloadFileFromUrl { get; }

    string ToolTip_PatchNumber { get; }
    string Log_OpeningFile { get; }
    string MainModuleIdTooltip { get; }
    string ATaskIsAlreadyRunning { get; }

    string FileInfo_Title { get; }
    string Title_FileInfo_FileType { get; }
    string Title_FileInfo_Compression { get; }
    string Title_FileInfo_Integrity { get; }
    string ToolTip_NcasIntegrity { get; }

    string AvailableContents { get; }
    string MultiContentPackageToolTip { get; }

    string NcasIntegrity_Error_NcaMissing { get; }
    string NcasIntegrity_Error_Log { get; }
    string NcaIntegrity_GetOriginalNcaError { get; }
    string NcaIntegrity_GetOriginalNcaError_Log { get; }

    string NcaHeaderSignature_Valid_Log { get; }
    string NcaHeaderSignature_Invalid { get; }
    string NcaHeaderSignature_Invalid_Log { get; }
    string NcaHeaderSignature_Error { get; }
    string NcaHeaderSignature_Error_log { get; }

    string NcaHash_VerificationStart_Log { get; }
    string NcaHash_VerificationEnd_Log { get; }
    string NcaHash_NcaItem_CantExtractHashFromName { get; }
    string NcaHash_CantExtractHashFromName_Log { get; }
    string NcaHash_Valid_Log { get; }
    string NcaHash_NcaItem_Invalid { get; }
    string NcaHash_Invalid_Log { get; }
    string NcaHash_NcaItem_Exception { get; }
    string NcaHash_Exception_Log { get; }
    string NcaHash_ProgressText { get; }

    string CancelAction { get; }
    string Status_Ready { get; }
    string LoadingFile_PleaseWait { get; }

    string NcasIntegrity_NoNca { get; }
    string NcasIntegrity_Unchecked { get; }
    string NcasIntegrity_InProgress { get; }
    string NcasIntegrity_Original { get; }
    string NcasIntegrity_Incomplete { get; }
    string NcasIntegrity_Modified { get; }
    string NcasIntegrity_Corrupted { get; }
    string NcasIntegrity_Error { get; }
    string NcasIntegrity_Unknown { get; }

    string Status_SavingFile { get; }

    string KeysLoading_Starting_Log { get; }
    string KeysLoading_Successful_Log { get; }
    string KeysLoading_Error { get; }
    string WarnNoProdKeysFileFound { get; }
    string InvalidSetting_KeysFileNotFound { get; }
    string InvalidSetting_BufferSizeInvalid { get; }
    string InvalidSetting_LanguageNotFound { get; }

    string ToolTip_KeyMissing { get; }

    string MenuItem_CopyTextToClipboard { get; }

    string ContextMenu_OpenFileLocation { get; }
    string OpenFileLocation_Failed_Log { get; }
    string SettingsView_TitlePageUrl { get; }
    string OpenTitleWebPage_Failed { get; }

    string Log_DownloadFileCanceled { get; }
    string Log_SaveToDirCanceled { get; }
    string Log_SaveFileCanceled { get; }
    string Log_SaveStorageCanceled { get; }
    string Log_NcasIntegrityCanceled { get; }

    string RenamingTool_WindowTitle { get; }
    string RenamingTool_Patterns { get; }
    string RenamingTool_ApplicationPattern { get; }
    string RenamingTool_PatchPattern { get; }
    string RenamingTool_AddonPattern { get; }
    string RenamingTool_InputPath { get; }
    string RenamingTool_FileFilters { get; }
    string RenamingTool_ToolTip_Patterns { get; }
    string RenamingTool_ToolTip_BasePattern { get; }
    string RenamingTool_ToolTip_PatchPattern { get; }
    string RenamingTool_ToolTip_AddonPattern { get; }
    string RenamingTool_Button_Cancel { get; }
    string RenamingTool_Button_Rename { get; }
    string RenamingTool_GroupBoxInput { get; }
    string RenamingTool_GroupBoxNamingSettings { get; }
    string RenamingTool_BrowseDirTitle { get; }
    string RenamingTool_GroupBoxOutput { get; }
    string RenamingTool_Miscellaneous { get; }
    string RenamingTool_InvalidWindowsCharReplacement { get; }
    string RenamingTool_ReplaceWhiteSpaceChars { get; }
    string RenamingTool_ReplaceWhiteSpaceCharsWith { get; }
    string RenamingTool_Simulation { get; }
    string RenamingTool_AutoCloseOpenedFile { get; }
    string RenamingTool_IncludeSubDirectories { get; }
    string RenamingTool_ContentTypeNotSupported { get; }
    string RenamingTool_SuperPackageNotSupported { get; }
    string RenamingTool_LogNbFilesToRename { get; }
    string RenamingTool_LogSimulationMode { get; }
    string RenamingTool_LogFileRenamed { get; }
    string RenamingTool_LogFileAlreadyNamedProperly { get; }
    string RenamingTool_LogFailedToRenameFile { get; }
    string RenamingTool_LogRenamingFailed { get; }

    string RenamingTool_BadInvalidFileNameCharReplacement { get; }

    string Exception_UnexpectedDelimiter { get; }
    string Exception_EndDelimiterMissing { get; }
    string FileRenaming_PatternKeywordUnknown { get; }
    string FileRenaming_EmptyPatternNotAllowed { get; }
    string FileRenaming_PatternKeywordNotAllowed { get; }
    string FileRenaming_StringOperatorUnknown { get; }
    string FileRenaming_EmptyDirectoryNotAllowed { get; }

    string Window_Tip_Title { get; }
}