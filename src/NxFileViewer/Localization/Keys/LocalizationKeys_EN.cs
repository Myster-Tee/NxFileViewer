using System;
using Emignatik.NxFileViewer.Utils.MVVM.Localization;
using LibHac.Ncm;

namespace Emignatik.NxFileViewer.Localization.Keys;

public class LocalizationKeys_EN : LocalizationKeysBase, ILocalizationKeys
{
    public override bool IsFallback => true;
    public override string DisplayName => "English";
    public override string CultureName => "en-US";
    public override string LanguageAuto => "Auto";

    public string FileNotSupported_Log => "File «{0}» not supported.";
    public string OpenFile_Filter => "Nintendo Switch files (*.nsp;*.nsz;*.xci;*.xcz)|*.nsp;*.nsz;*.xci;*.xcz|All files (*.*)|*.*";
    public string MenuItem_File => "_File";
    public string MenuItem_Open => "_Open...";
    public string MenuItem_OpenLast => "Open _last";
    public string MenuItem_Close => "_Close";
    public string MenuItem_Exit => "E_xit";
    public string MenuItem_Tools => "_Tools";
    public string MenuItem_VerifyNcasHash => "Verify _hashes";
    public string MenuItem_VerifyNcasHeaderSignature => "Verify _signatures";
    public string MenuItem_Options => "_Options";
    public string MenuItem_Settings => "_Settings";
    public string MenuItem_ReloadKeys => "Reload keys";
    public string MenuItem_OpenTitleWebPage => "Open title Web page...";
    public string MenuItem_ShowRenameToolWindow => "Renaming tool...";

    public string MultiContentPackage => "Multi-content Package";
    public string GeneralInfo => "General Info";
    public string TitleId => "Title ID";
    public string DisplayVersion => "Display Version";
    public string Presentation => "Presentation";
    public string ToolTip_AvailableLanguages => "Title, Publisher and Icon may change according to the selected language.";
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

    public string SettingsView_Title => "Settings";
    public string SettingsView_Button_Apply => "Apply";
    public string SettingsView_Button_Cancel => "Cancel";
    public string SettingsView_Button_Reset => "Reset";
    public string SettingsView_GroupBoxKeys => "Keys";
    public string SettingsView_Title_UsedKeysFilePath => "Used path";
    public string SettingsView_Title_KeysCustomFilePath => "Custom path";
    public string SettingsView_Title_KeysDownloadUrl => "Download URL";

    public string SettingsView_ToolTip_ProdKeys => "This file contains common keys used by all Switch devices. This file is required for opening encrypted title files." + Environment.NewLine +
                                                   "The program will search this file in the following order:" + Environment.NewLine +
                                                   "    1. the path defined by this setting" + Environment.NewLine +
                                                   "    2. the current program's directory" + Environment.NewLine +
                                                   "    3. the «%UserProfile%\\.switch» directory" + Environment.NewLine + Environment.NewLine +
                                                   "At startup, the program can automatically download the keys file when none is found on the system." + Environment.NewLine +
                                                   "The keys file will be downloaded to the current application's directory." + Environment.NewLine + Environment.NewLine +
                                                   "File should contain one key per line in form of «KEY_NAME = HEXADECIMAL_KEY».";

    public string SettingsView_ToolTip_TitleKeys => "You can optionally specify a file containing game-specific keys." + Environment.NewLine +
                                                    "The program will search this file in the following locations:" + Environment.NewLine +
                                                    "    1. the path defined by this setting" + Environment.NewLine +
                                                    "    2. the current program's directory" + Environment.NewLine +
                                                    "    3. the «%UserProfile%\\.switch» directory" + Environment.NewLine + Environment.NewLine +
                                                    "At startup, the program can automatically download the keys file when none is found on the system." + Environment.NewLine +
                                                    "The keys file will be downloaded to the current application's directory." + Environment.NewLine + Environment.NewLine +
                                                    "File should contain one key per line in form of «KEY_NAME = HEXADECIMAL_KEY».";


    public string SettingsView_ToolTip_ConsoleKeys => "You can optionally specify a file containing console-unique keys." + Environment.NewLine +
                                                      "The program will search this file in the following locations:" + Environment.NewLine +
                                                      "    1. the path defined by this setting" + Environment.NewLine +
                                                      "    2. the current program's directory" + Environment.NewLine +
                                                      "    3. the «%UserProfile%\\.switch» directory" + Environment.NewLine + Environment.NewLine +
                                                      "File should contain one key per line in form of «KEY_NAME = HEXADECIMAL_KEY».";


    public string SettingsView_LogLevel => "Log level";
    public string SettingsView_ToolTip_LogLevel => "The log level specifies the minimum level to log.";
    public string SettingsView_CheckBox_AlwaysReloadKeysBeforeOpen => "Always reload keys before opening a file";
    public string SettingsView_Title_Language => "Language";
    public string SettingsView_ToolTip_OpenKeysLocation => "Open keys file location.";
    public string SettingsView_ToolTip_BrowseKeys => "Browse...";
    public string SettingsView_ToolTip_DownloadKeys => "Download from specified URL.";

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

    public string SaveDialog_Title => "Save as";
    public string SaveDialog_ImageFilter => "Image";
    public string SaveDialog_AnyFileFilter => "File";
    public string SaveFile_Error => "Failed to save file: {0}";

    public string ContextMenu_CopyImage => "Copy";

    public string TabOverview => "Overview";
    public string TabContent => "Content";
    public string GroupBoxStructure => "Structure";
    public string GroupBoxProperties => "Properties";

    public string ContextMenu_ShowItemErrors => "Show errors...";
    public string ContextMenu_SaveSectionItem => "Save section content...";
    public string ContextMenu_SaveDirectoryItem => "Save directory...";
    public string ContextMenu_SaveFileItem => "Save file...";
    public string ContextMenu_SavePartitionFileItem => "Save partition file...";
    public string ContextMenu_SaveNcaFileRaw => "Save NCA raw...";
    public string ContextMenu_SaveNcaFilePlaintext => "Save NCA plaintext...";

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
    public string LoadingError_FailedToOpenNcaSectionFileSystem => "Failed to open content of NCA section «{0}»: {1}";
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
    public string LoadingError_NacpFileMissing => "NACP file «{0}» not found!";
    public string LoadingError_IconMissing => "Expected icon file «{0}» missing.";
    public string LoadingError_XciSecurePartitionNotFound => "XCI secure partition not found!";
    public string LoadingError_FailedToGetNcaSectionFsHeader => "Failed to get NCA file system header of section «{0}»: {1}";
    public string LoadingError_FailedToOpenMainFile => "Failed to open Main file: {0}";
    public string LoadingError_FailedToLoadMainFile => "Failed to load Main file: {0}";
    public string LoadingError_FailedToLoadTicketFile => "Failed to load ticket file: {0}";
    public string LoadingError_FailedToLoadTitleIdKey => "Failed to load Title ID key from ticket file «{0}»: {1}";
    public string LoadingInfo_TitleIdKeySuccessfullyInjected => "Title ID key «{0}={1}» found in ticket file «{2}» successfully added to the set of keys.";
    public string LoadingWarning_TitleIdKeyReplaced => "Title ID key «{0}={1}» found in ticket file «{2}» has been used as replacement of the existing title ID key «{0}={2}» found in the set of keys.";
    public string LoadingDebug_TitleIdKeyAlreadyExists => "Title ID key «{0}={1}» found in ticket file «{2}» was already registered in the set of keys.";

    public string KeysFileUsed => "«{0}» file used: {1}";
    public string NoneKeysFile => "[none]";

    public string Status_DownloadingFile => "Downloading file «{0}»...";
    public string Log_DownloadingFileFromUrl => "Downloading «{0}» from URL «{1}»...";
    public string Log_FileSuccessfullyDownloaded => "File «{0}» successfully downloaded.";
    public string Log_FailedToDownloadFileFromUrl => "Failed to download «{0}» from URL «{1}»: {2}";

    public string ToolTip_PatchNumber => "Patch number {0}";
    public string Log_OpeningFile => "=====> {0} <=====";
    public string MainModuleIdTooltip => "Also known as «Build ID» (or BID).";
    public string ATaskIsAlreadyRunning => "A task is already running...";

    public string Integrity => "Integrity";
    public string AvailableContents => "Contents:";
    public string MultiContentPackageToolTip => "Current package contains multiple contents («{0}» detected).";

    public string Title_NcasHeaderSignature => "Signature:";
    public string ToolTip_NcasHeaderSignature => "Verifies the signature of each NCA header.";
    public string Title_NcasHash => "Hash:";
    public string ToolTip_NcasHash => "Verifies the hash of each NCA." + Environment.NewLine +
                                      "A valid hash ensures that a file has not been corrupted.";

    public string NcaHeaderSignature_VerificationStart_Log => ">>> Signature verification starting...";
    public string NcaHeaderSignature_VerificationEnd_Log => ">>> Signature verification finished.";
    public string NcaHeaderSignature_Valid_Log => "Header signature of NCA «{0}» is valid.";
    public string NcaHeaderSignature_Invalid => "NCA header signature verification failed with status «{0}».";
    public string NcaHeaderSignature_Invalid_Log => "Header signature verification of NCA «{0}» failed with status «{1}».";
    public string NcaHeaderSignature_Error => "Failed to verify NCA header signature: {0}.";
    public string NcaHeaderSignature_Error_log => "Failed to verify header signature of NCA «{0}»: {1}";
    public string NcasHeaderSignature_Error_Log => "Failed to verify NCA's headers signature: {0}";

    public string NcaHash_VerificationStart_Log => ">>> NCAs hash verification starting...";
    public string NcaHash_VerificationEnd_Log => ">>> NCAs hash verification finished.";
    public string NcaHash_Valid_Log => "Hash of NCA «{0}» is valid.";
    public string NcaHash_NcaItem_Invalid => "Hash is not valid.";
    public string NcaHash_Invalid_Log => "Hash of NCA «{0}» is not valid.";
    public string NcaHash_CnmtItem_Error_NcaMissing => "Hash can't be verified, NCA «{0}» is missing.";
    public string NcaHash_NcaItem_Exception => "Failed to verify hash: {0}";
    public string NcaHash_Exception_Log => "Failed to verify hash of NCA «{0}»: {1}";
    public string NcasHash_Error_Log => "Failed to verify NCAs hashes: {0}";
    public string NcaHash_ProgressText => "Hashing NCA {0}/{1}...";

    public string CancelAction => "Cancel";
    public string Status_Ready => "Ready.";
    public string LoadingFile_PleaseWait => "Loading, please wait...";

    public string NcasValidity_NoNca => "No NCA";
    public string NcasValidity_Unchecked => "Unchecked";
    public string NcasValidity_InProgress => "In progress";
    public string NcasValidity_Invalid => "Invalid";
    public string NcasValidity_Valid => "Valid";
    public string NcasValidity_Error => "Error";
    public string NcasValidity_Unknown => "Unknown";

    public string Status_SavingFile => "Saving file «{0}»...";

    public string KeysLoading_Starting_Log => ">>> Loading Keys...";
    public string KeysLoading_Successful_Log => ">>> Keys successfully loaded.";
    public string KeysLoading_Error => "Failed to load keys: {0}.";
    public string WarnNoProdKeysFileFound => "No «prod.keys» file found.";
    public string InvalidSetting_KeysFileNotFound => "Keys file «{0}» defined in the settings doesn't exist.";
    public string InvalidSetting_BufferSizeInvalid => "Buffer size «{0}» defined in the settings is not valid, value should be strictly greater than 0.";
    public string InvalidSetting_LanguageNotFound => "Language «{0}» defined in the settings doesn't exist.";

    public string ToolTip_KeyMissing => "Key «{0}» of type «{1}» is missing.";

    public string MenuItem_CopyTextToClipboard => "Copy";
    public string ContextMenu_OpenFileLocation => "Open location...";
    public string OpenFileLocation_Failed_Log => "Failed to open location of file «{0}»: {1}";
    public string SettingsView_TitlePageUrl => "Title page URL";
    public string OpenTitleWebPage_Failed => "Failed to open title Web page: {0}";

    public string Log_DownloadFileCanceled => "Download canceled.";
    public string Log_SaveToDirCanceled => "Directory saving canceled.";
    public string Log_SaveFileCanceled => "File saving canceled.";
    public string Log_SaveStorageCanceled => "Storage saving canceled.";
    public string Log_NcaHashCanceled => "NCAs hash canceled.";

    public string RenamingTool_WindowTitle => "Renaming tool";
    public string RenamingTool_Patterns => "Patterns";
    public string RenamingTool_ApplicationPattern => "Application pattern";
    public string RenamingTool_PatchPattern => "Patch pattern";
    public string RenamingTool_AddonPattern => "Add-on pattern";
    public string RenamingTool_InputPath => "Input path";
    public string RenamingTool_FileFilters => "Filters";
    public string RenamingTool_ToolTip_Patterns => $"Allowed keywords:{Environment.NewLine}" +
                                                   $"  • {{TitleId}}{Environment.NewLine}" +
                                                   $"       - The content id.{Environment.NewLine}" +
                                                   $"  • {{AppId}}{Environment.NewLine}" +
                                                   $"       - The id of the corresponding {nameof(ContentMetaType.Application)} (for {nameof(ContentMetaType.Application)} contents, this value is equal to the {{TitleId}}).{Environment.NewLine}" +
                                                   $"  • {{PatchId}}:{Environment.NewLine}" +
                                                   $"       - If content is an {nameof(ContentMetaType.Application)}, this value is equal to the id of the corresponding {nameof(ContentMetaType.Patch)} content, otherwise zero.{Environment.NewLine}" +
                                                   $"  • {{PatchNum}}:{Environment.NewLine}" +
                                                   $"       - If content is an {nameof(ContentMetaType.Application)}, value is normally 0.{Environment.NewLine}" +
                                                   $"       - If content is a {nameof(ContentMetaType.Patch)}, value corresponds to the patch number.{Environment.NewLine}" +
                                                   $"       - If content is an {nameof(ContentMetaType.AddOnContent)}, value corresponds to the add-on patch number.{Environment.NewLine}" +
                                                   $"  • {{Title}}:{Environment.NewLine}" +
                                                   $"       - The first title among the list of declared titles.{Environment.NewLine}" +
                                                   $"       - This value exists only for contents of type {nameof(ContentMetaType.Application)} or {nameof(ContentMetaType.Patch)}, but not for {nameof(ContentMetaType.AddOnContent)}.{Environment.NewLine}" +
                                                   $"  • {{Ext}}:{Environment.NewLine}" +
                                                   $"       - The extension corresponding to the detected file type.{Environment.NewLine}" +
                                                   $"  • {{VerNum}}:{Environment.NewLine}" +
                                                   $"       - The content version number.{Environment.NewLine}" +
                                                   $"  • {{WTitle}}:{Environment.NewLine}" +
                                                   $"       - The content title retrieved from the Internet.{Environment.NewLine}" +
                                                   $"  • {{WAppTitle}}: {Environment.NewLine}" +
                                                   $"       - The title of the corresponding {nameof(ContentMetaType.Application)}, retrieved from the Internet.{Environment.NewLine}" +
                                                   $"{Environment.NewLine}" +
                                                   $"Use \\{{ or \\}} to write the literal chars {{ or }}.";
    public string RenamingTool_ToolTip_BasePattern => $"The pattern to use for contents of type {nameof(ContentMetaType.Application)}.";
    public string RenamingTool_ToolTip_PatchPattern => $"The pattern to use for contents of type {nameof(ContentMetaType.Patch)}.";
    public string RenamingTool_ToolTip_AddonPattern => $"The pattern to use for contents of type {nameof(ContentMetaType.AddOnContent)}.";
    public string RenamingTool_Button_Cancel => "Cancel";
    public string RenamingTool_Button_Rename => "Rename";
    public string RenamingTool_GroupBoxInput => "Input";
    public string RenamingTool_GroupBoxNamingSettings => "Naming settings";
    public string RenamingTool_BrowseDirTitle => "Select a directory";
    public string RenamingTool_GroupBoxOutput => "Output";
    public string RenamingTool_Miscellaneous => "Miscellaneous";
    public string RenamingTool_InvalidWindowsCharReplacement => "Replace unauthorized Windows characters with";
    public string RenamingTool_ReplaceWhiteSpaceChars => "Replace white space chars";
    public string RenamingTool_ReplaceWhiteSpaceCharsWith => "Replace white space chars with";
    public string RenamingTool_Simulation => "Simulation";
    public string RenamingTool_IncludeSubDirectories => "Include subdirectories";
    public string RenamingTool_ContentTypeNotSupported => "Content type «{0}» not supported.";
    public string RenamingTool_SuperPackageNotSupported => "Super package not supported.";
    public string RenamingTool_LogNbFilesToRename => ">>> {0} file(s) to rename...";
    public string RenamingTool_LogSimulationMode => $"[SIMULATION] ";
    public string RenamingTool_LogFileRenamed => $"• {{0}}File renamed from{Environment.NewLine}\t«{{1}}» to{Environment.NewLine}\t«{{2}}».";
    public string RenamingTool_LogFileAlreadyNamedProperly => "• {0}«{1}» already named correctly.";
    public string RenamingTool_LogFailedToRenameFile => "• {0}«{1}»Renaming failed: {2}";
    public string RenamingTool_LogRenamingFailed => "Renaming failed: {0}";
    public string RenamingTool_BadInvalidFileNameCharReplacement => "The replacement string «{0}» (for invalid file name chars), can't contain invalid char «{1}».";

    public string Exception_UnexpectedDelimiter => "Unexpected delimiter {0} found at position {1}, use {2}{0} instead.";
    public string Exception_EndDelimiterMissing => "End delimiter {0} is missing.";
    public string FileRenaming_PatternKeywordUnknown => "Keyword «{0}» is unknown, allowed keywords are «{1}».";
    public string FileRenaming_EmptyPatternNotAllowed => "Pattern can't be empty.";
    public string FileRenaming_PatternKeywordNotAllowed => "Keyword «{0}» not allowed for patters of type «{1}».";
    public string FileRenaming_StringOperatorUnknown => "Operator «{0}» is not recognized, allowed operators are «{1}».";
    public string FileRenaming_EmptyDirectoryNotAllowed => "Input directory can't be empty.";
}