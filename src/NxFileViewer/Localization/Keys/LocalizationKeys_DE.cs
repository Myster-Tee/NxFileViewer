using System;
using Emignatik.NxFileViewer.Utils.MVVM.Localization;
using LibHac.Ncm;

namespace Emignatik.NxFileViewer.Localization.Keys;

public class LocalizationKeys_EN : LocalizationKeysBase, ILocalizationKeys
{
    public override bool IsFallback => true;
    public override string DisplayName => "Deutsch";
    public override string CultureName => "de-DE";
    public override string LanguageAuto => "Auto";

    public string FileNotSupported_Log => "«{0}» Datei wird nicht unterstützt.";
    public string OpenFile_Filter => "Nintendo Switch Dateien (*.nsp;*.nsz;*.xci;*.xcz)|*.nsp;*.nsz;*.xci;*.xcz|Alle Dateien (*.*)|*.*";
    public string MenuItem_File => "_Datei";
    public string MenuItem_Open => "_Öffnen...";
    public string MenuItem_OpenLast => "_Letzte öffnen";
    public string MenuItem_Close => "_Schließen";
    public string MenuItem_Exit => "_Beenden";
    public string MenuItem_Tools => "_Werkzeuge";
    public string MenuItem_CheckIntegrity => "_Integrität prüfen";
    public string MenuItem_Options => "_Optionen";
    public string MenuItem_Settings => "_Einstellungen";
    public string MenuItem_ReloadKeys => "Keys Neuladen";
    public string MenuItem_OpenTitleWebPage => "Titel Webseite öffnen...";
    public string MenuItem_ShowRenameToolWindow => "Umbenennen...";

    public string Packages_Title => "Multi-Paket Datei";
    public string DisplayVersion => "Display Version";
    public string Presentation_Title => "Präsentation";
    public string ToolTip_AvailableLanguages => "Titel, Publisher und Icon können sich je nach ausgewählter Sprache ändern.";
    public string AvailableLanguages => "Sprachen";
    public string AppTitle => "Titel";
    public string Publisher => "Publisher";

    public string Lng_AmericanEnglish => "Englisch (US)";
    public string Lng_BritishEnglish => "Englisch (UK)";
    public string Lng_CanadianFrench => "Französisch (Kanada)";
    public string Lng_Dutch => "Niederländisch";
    public string Lng_French => "Französisch";
    public string Lng_German => "Deutsch";
    public string Lng_Italian => "Italienisch";
    public string Lng_Japanese => "Japanisch";
    public string Lng_Korean => "Koreanisch";
    public string Lng_LatinAmericanSpanish => "Spanisch (Lateinamerika)";
    public string Lng_Portuguese => "Portugiesisch";
    public string Lng_Russian => "Russisch";
    public string Lng_SimplifiedChinese => "Chinesisch (vereinfacht)";
    public string Lng_Spanish => "Spanisch";
    public string Lng_TraditionalChinese => "Chinesisch (traditionell)";
    public string Lng_BrazilianPortuguese => "Portugiesisch (Brasilien)";
    public string Lng_Unknown => "Unbekannt";

    public string SettingsView_Title => "Einstellungen";
    public string SettingsView_Button_Apply => "Übernehmen";
    public string SettingsView_Button_Cancel => "Abbrechen";
    public string SettingsView_Button_Reset => "Zurücksetzen";
    public string SettingsView_GroupBoxKeys => "Keys";
    public string SettingsView_Title_KeysEffectiveFilePath => "Effektiver Pfad";
    public string SettingsView_Title_KeysCustomFilePath => "Benutzerdefinierter Pfad";
    public string SettingsView_Title_KeysDownloadUrl => "Download URL";
    public string SettingsView_ToolTip_Keys => """
                                               Keys sind erforderlich, um verschlüsselte Nintendo-Switch-Dateien (XCI, NSP, ...) zu öffnen.
                                               Jede offizielle Nintendo-Switch-Datei ist mit Keys verschlüsselt, die spezifisch für die Switch-Firmware-Version sind, für die sie erstellt wurde.

                                               Um jede Nintendo-Switch-Datei ohne Fehler zu öffnen, stelle sicher, dass du stets eine aktuelle "prod.keys"-Datei mit den Keys aller bestehenden Firmware-Versionen hast.
                                               
                                               Die Datei sollte einen Key pro Zeile enthalten, in der Form «KEY_NAME = HEXADECIMAL_WERT».
                                               """;
    public string SettingsView_ToolTip_ProdKeys => """
                                                   Diese Datei enthält allgemeine Keys, die von allen Switch-Geräten verwendet werden. Sie wird benötigt, um verschlüsselte Titelinhalte zu öffnen.
                                                   Das Programm sucht diese Datei in folgender Reihenfolge:
                                                       1. Im Pfad, der durch diese Einstellung definiert ist
                                                       2. Im Verzeichnis des aktuellen Programms
                                                       3. Im Verzeichnis «%UserProfile%\\.switch»
                                                   
                                                   Beim Start kann das Programm die Keydatei automatisch herunterladen, falls keine auf dem System gefunden wird.
												   Die Keydatei wird im Verzeichnis der aktuellen Anwendung gespeichert.
                                                   """;

    public string SettingsView_ToolTip_TitleKeys => """
                                                    Du kannst optional eine Datei angeben, die spielbezogene Keys enthält.
                                                    Das Programm sucht diese Datei in folgender Reihenfolge:
                                                        1. Im Pfad, der durch diese Einstellung definiert ist
                                                        2. Im Verzeichnis des aktuellen Programms
                                                        3. Im Verzeichnis «%UserProfile%\\.switch»
                                                    
                                                    Beim Start kann das Programm die Keydatei automatisch herunterladen, falls keine auf dem System gefunden wird.
                                                    Die Keydatei wird im Verzeichnis der aktuellen Anwendung gespeichert.
                                                    """;

    public string SettingsView_LogLevel => "Protokollierungsgrad ";
    public string SettingsView_ToolTip_LogLevel => "Der Protokollierungsgrad gibt die minimale Ebene an, die protokolliert werden soll.";
    public string SettingsView_CheckBox_AlwaysReloadKeysBeforeOpen => "Keys immer neu laden bevor eine Datei geöffnet wird.";
    public string SettingsView_CheckBox_InjectTicketKeys => "Keys aus Ticket-Dateien (*.tik) injizieren";
    public string SettingsView_Title_Language => "Sprache";
    public string SettingsView_Title_NczOptions => "NSZ/XCZ Einstellungen";

    public string SettingsView_ToolTip_NczBlockLessCompression => """
                                                                  NSZ- oder XCZ-Dateien bestehen aus NCZ-Dateien, die NCA-komprimierte Dateien sind.
																  NCZ-Dateien können ohne die Blockkomprimierungsmethode komprimiert werden, was effizienten zufälligen Lesezugriff unmöglich macht.
																  Daher ist es bei großen Dateien, bei denen ein kleiner Teil am Ende gelesen werden muss, notwendig, den gesamten Stream zu dekomprimieren, um den gewünschten Teil zu erreichen.
																  Große Dateien können daher lange zum Öffnen benötigen.
																  Verwende vorzugsweise die Blockkomprimierung für große Dateien.
																  Beachte, dass es, wenn du das Öffnen von blocklos komprimierten NCZ-Dateien nicht zulässt, keine Auswirkungen auf die Integritätsprüffunktionen hat.
                                                                  """;

    public string SettingsView_CheckBox_NczOpenBlocklessCompression => "Öffne NCZ-komprimierte Dateien ohne Blockkompression.";
    public string SettingsView_Title_Integrity => "Integrität";
    public string SettingsView_CheckBox_IgnoreMissingDeltaFragments => "Fehlende Delta-Fragmente ignorieren";
    public string SettingsView_ToolTip_IgnoreMissingDeltaFragments => $"""
                                                                      Patch-Dateien können vollständige Update-Dateien und inkrementelle Update-Dateien (bekannt als {ContentType.DeltaFragment}) enthalten.
																	  Diese Fragmente sind nicht zwingend erforderlich, um eine Anwendung zu aktualisieren, und werden manchmal absichtlich entfernt.
																	  Aktiviere diese Option, wenn du fehlende {ContentType.DeltaFragment} bei der Integritätsprüfung ignorieren möchtest.
                                                                      """;

    public string SettingsView_Miscellaneous => "Sonstiges";
    public string SettingsView_ToolTip_OpenKeysLocation => "Keydateipfad öffnen";
    public string SettingsView_ToolTip_BrowseKeys => "Suchen...";
    public string SettingsView_ToolTip_DownloadKeys => "Von bestimmter URL herunterladen.";

    public string BrowseKeysFile_ProdTitle => "Wähle \"prod\" Keydatei";
    public string BrowseKeysFile_TitleTitle => "Wähle \"title\" Keydatei";
    public string BrowseKeysFile_Filter => "Keydateien (*.keys)|*.keys|Alle Dateien (*.*)|*.*";

    public string SuspiciousFileExtension => "Dateierweiterung «{0}» scheint ungültig zu sein, «{1}» oder «{2}» wurde erwartet.";
    public string DragMeAFile => "Ziehe eine unterstützte Datei hierher :)";
    public string MultipleFilesDragAndDropNotSupported => "Mehrere Dateien per Drag & Drop werden nicht unterstützt, nur die erste Datei wird geöffnet.";

    public string CnmtOverview_Title => "Paketinformationen";
    public string CnmtOverview_TitleId => "TitelID";
    public string CnmtOverview_ContentType => "Typ";
    public string CnmtOverview_TitleVersion => "Version";
    public string CnmtOverview_MinimumSystemVersion => "Minimale Systemversion";
    public string CnmtOverview_BuildID => "BuildID";
    public string CnmtOverview_BuildID_NotAvailableBecauseSectionIsSparse => "Nicht verfügbar (Inhalt spärlich)";
    public string CnmtOverview_IsDemo => "Demo";

    public string ContextMenu_SaveImage => "Speichern...";
    public string CopyTitleImageError => "Fehler beim Kopieren der Spieldatei: {0}";
    public string SaveTitleImageError => "Konnte die Spieldatei nicht speichern: {0}";

    public string SaveDialog_Title => "Speichern unter";
    public string SaveDialog_ImageFilter => "Spieldatei";
    public string SaveDialog_AnyFileFilter => "Datei";
    public string SaveFile_Error => "Fehler beim speichern der Datei: {0}";

    public string ContextMenu_CopyImage => "Kopieren";

    public string TabOverview => "Übersicht";
    public string TabContent => "Inhalt";
    public string GroupBoxStructure => "Struktur";
    public string GroupBoxProperties => "Eigenschaften";

    public string ContextMenu_ShowItemErrors => "Zeige Fehler...";
    public string ContextMenu_SaveSectionItem => "Sektion speichern...";
    public string ContextMenu_SaveDirectoryItem => "Verzeichnis speichern...";
    public string ContextMenu_SaveFileItem => "Datei speichern...";
    public string ContextMenu_SavePartitionFileItem => "Partitionsdatei speichern...";
    public string ContextMenu_SaveNcaFileRaw => "NCA RAW speichern...";
    public string ContextMenu_SaveNcaFilePlaintext => "NCA Klartext speichern...";

    public string SettingsLoadingError => "Einstellungen konnten nicht geladen werden: {0}";
    public string SettingsSavingError => "Einstellungen konnten nicht gespeichert werden: {0}";

    public string LoadingError_Failed => "Fehler beim laden der Datei «{0}»: {1}";
    public string LoadingError_FailedToCheckIfXciPartitionExists => "Fehler beim Überprüfen, ob die XCI Partition existiert: {0}";
    public string LoadingError_FailedToOpenXciPartition => "XCI Partition konnte nicht geöffnet werden: {0}";
    public string LoadingError_FailedToLoadXciContent => "XCI Partition konnte nicht geladen werden: {0}";
    public string LoadingError_FailedToOpenPartitionFile => "Fehler beim öffnen der Partitionsdatei: {0}";
    public string LoadingError_FailedToLoadNcaFile => "NCA Datei konnt nicht geladen werden: {0}";
    public string LoadingError_FailedToLoadPartitionFileSystemContent => "Fehler beim Laden der Inhalte des Partition-Dateisystems: {0}";
    public string LoadingError_FailedToCheckIfSectionCanBeOpened => "Fehler beim Überprüfen, ob die Sektion geöffnet werden kann.: {0}";
    public string LoadingError_FailedToOpenNcaSectionFileSystem => "NCA Sektionsinhalt konnte nicht geladen werden «{0}»: {1}";
    public string LoadingError_FailedToLoadSectionContent => "Sektionsinhalt konnte nicht geladen werden: {0}";
    public string LoadingError_FailedToGetFileSystemDirectoryEntries => "Fehler beim Abrufen der Verzeichniseinträge des Dateisystems: {0}";
    public string LoadingError_FailedToOpenNacpFile => "Fehler beim öffnen der NACP Datei: {0}";
    public string LoadingError_FailedToLoadNacpFile => "Fehler beim laden der NACP Datei: {0}";
    public string LoadingError_FailedToOpenCnmtFile => "Fehler beim öffnen der CNMT Datei: {0}";
    public string LoadingError_FailedToLoadCnmtFile => "Fehler beim laden der CNMT Datei: {0}";
    public string LoadingError_FailedToLoadNcaContent => "Fehler beim laden des NCA Inhalts: {0}";
    public string LoadingError_FailedToLoadDirectoryContent => "Fehler beim Laden des Verzeichnisinhalts: {0}";
    public string LoadingError_FailedToLoadIcon_Log => "Fehler beim laden des Icons: {0}";
    public string LoadingError_NcaFileMissing_Log => "NCA-Eintrag «{0}» vom Typ «{1}» fehlt.";
    public string LoadingError_NoCnmtFound_Log => "Kein CNMT Eintrag gefunden!";
    public string LoadingError_NacpFileMissing_Log => "NACP Datei «{0}» nicht gefunden!";
    public string LoadingError_NcaMissingSection_Log => "NCA mit Inhalts-Typ «{0}» fehlt eine Sektion vom Typ «{0}».";
    public string LoadingError_MainFileMissing_Log => "Datei «{0}» nicht gefunden!";
    public string LoadingError_IconMissing_Log => "Erwartete Icondatei «{0}» fehlt.";
    public string LoadingError_XciSecurePartitionNotFound_Log => "Sichere Partition der XCI nicht gefunden!";
    public string LoadingError_FailedToGetNcaSectionFsHeader => "Fehler beim Abrufen des NCA-Dateisystem-Headers von Sektion «{0}»: {1}";
    public string LoadingError_FailedToOpenMainFile => "Fehler beim öffnen der Hauptdatei: {0}";
    public string LoadingError_FailedToLoadMainFile => "Fehler beim laden der Hauptdatei: {0}";
    public string LoadingError_FailedToLoadTicketFile => "Fehler beim laden der Ticket Datei: {0}";
    public string LoadingError_FailedToLoadTitleIdKey => "Failed to load TitleID key from ticket file «{0}»: {1}";
    public string LoadingError_NczBlocklessCompressionDisabled => "Das Öffnen von NCZ Dateien ohne Blockkompression ist in den Einstellungen deaktiviert.";

    public string LoadingInfo_TitleIdKeySuccessfullyInjected => "TitleID Key «{0}={1}» aus der Ticket-Datei «{2}» wurde erfolgreich zu den Keys hinzugefügt.";
    public string LoadingWarning_TitleIdKeyReplaced => "TitleID Key «{0}={1}» aus der Ticket-Datei «{2}» wurde als Ersatz für den bestehenden TitleID Key «{0}={2}» in den Keys verwendet.";
    public string LoadingDebug_TitleIdKeyAlreadyExists => "TitleID Key «{0}={1}» aus der Ticket-Datei «{2}» war bereits in den Keys vorhanden.";

    public string KeysFileUsed => "Die Datei «{0}» wurde verwendet: {1}";
    public string NoneKeysFile => "[keine]";

    public string Status_DownloadingFile => "Lade Datei «{0}» herunter...";
    public string Log_DownloadingFileFromUrl => "Lade «{0}» von der URL «{1}» herunter...";
    public string Log_FileSuccessfullyDownloaded => "Datei «{0}» wurde erfolgreich heruntergeladen.";
    public string Log_FailedToDownloadFileFromUrl => "Fehler beim Herunterladen von «{0}» von der URL «{1}»: {2}";

    public string ToolTip_PatchNumber => "Patchnummer {0}";
    public string Log_OpeningFile => "=====> {0} <=====";
    public string MainModuleIdTooltip => "Auch bekannt als «BuildID» (oder BID).";
    public string ATaskIsAlreadyRunning => "Ein Task wird bereits ausgeführt...";
    public string FileInfo_Title => "Datei";
    public string Title_FileInfo_FileType => "Typ";
    public string Title_FileInfo_Compression => "Komprimierung";
    public string Title_FileInfo_Integrity => "Integrität";
    public string ToolTip_NcasIntegrity => $"""
                                           Die Integritätsprüfung besteht darin, die Integrität jeder NCA (oder NCZ) zu überprüfen.
										   
										   Das Ergebnis der Integritätsprüfung kann wie folgt aussehen:
										   {NcasIntegrity_NoNca}: Keine NCA-Datei gefunden.
										   {NcasIntegrity_Unchecked}: Integrität nicht geprüft.
										   {NcasIntegrity_InProgress}: Integritätsprüfung läuft.
										   {NcasIntegrity_Original}: Alle NCAs sind original (Signatur und Hash in Ordnung).
										   {NcasIntegrity_Incomplete}: Alle NCAs sind original, aber einige fehlen.
										   {NcasIntegrity_Modified}: Mindestens eine NCA ist modifiziert (Signatur nicht in Ordnung, aber Hash in Ordnung).
										   {NcasIntegrity_Corrupted}: Mindestens eine NCA ist beschädigt (Hash ungültig).
										   {NcasIntegrity_Error}: Ein Fehler ist während der Integritätsprüfung aufgetreten.
										   
										   Details zu jeder analysierten NCA findest du im Tab «Inhalt».
                                           """;

    public string AvailableContents => "Inhalt:";
    public string MultiContentPackageToolTip => "Das aktuelle Paket enthält mehrere Inhalte («{0}» erkannt).";

    public string NcasIntegrity_Error_NcaMissing => "Die Integrität der NCA «{0}» kann nicht überprüft werden, NCA fehlt.";
    public string NcasIntegrity_Error_Log => "Fehler bei der Überprüfung der NCA-Integrität: {0}";
    public string NcaIntegrity_GetOriginalNcaError => "Fehler beim Abrufen der originalen NCA: {0}";
    public string NcaIntegrity_GetOriginalNcaError_Log => "Fehler beim Abrufen der originalen NCA aus der NCA «{0}»: {1}";

    public string NcaHeaderSignature_Valid_Log => "Die Header-Signatur der NCA «{0}» ist gültig.";
    public string NcaHeaderSignature_Invalid => "Die Überprüfung der NCA-Header-Signatur ist mit dem Status «{0}» fehlgeschlagen.";
    public string NcaHeaderSignature_Invalid_Log => "Die Überprüfung der Header-Signatur der NCA «{0}» ist mit dem Status «{1}» fehlgeschlagen.";
    public string NcaHeaderSignature_Error => "Fehler bei der Überprüfung der NCA-Header-Signatur: {0}.";
    public string NcaHeaderSignature_Error_log => "Fehler bei der Überprüfung der Signatur des NCA-Headers «{0}»: {1}";

    public string NcaHash_VerificationStart_Log => ">>> Die Hash-Überprüfung der NCAs wird gestartet...";
    public string NcaHash_VerificationEnd_Log => ">>> Die Hash-Überprüfung der NCAs ist abgeschlossen.";
    public string NcaHash_NcaItem_CantExtractHashFromName => "Fehler beim Extrahieren des erwarteten Hashs aus dem NCA-Namen.";
    public string NcaHash_CantExtractHashFromName_Log => "Fehler beim Extrahieren des erwarteten Hashs aus dem NCA-Namen «{0}».";
    public string NcaHash_Valid_Log => "Der Hash der NCA «{0}» ist gültig.";
    public string NcaHash_NcaItem_Invalid => "Hash ist ungültig.";
    public string NcaHash_Invalid_Log => "Der Hash der NCA «{0}» ist ungültig.";
    public string NcaHash_NcaItem_Exception => "Fehler bei der Überprüfung des Hashs: {0}";
    public string NcaHash_Exception_Log => "Fehler bei der Überprüfung des Hashs der NCA «{0}»: {1}";
    public string NcaHash_ProgressText => "Hashing der NCA {0}/{1}...";

    public string CancelAction => "Abbrechen";
    public string Status_Ready => "Bereit.";
    public string LoadingFile_PleaseWait => "Lade, bitte warten...";

    public string NcasIntegrity_NoNca => "Kein NCA";
    public string NcasIntegrity_Unchecked => "Nicht überprüft";
    public string NcasIntegrity_InProgress => "In Bearbeitung";
    public string NcasIntegrity_Original => "Original";
    public string NcasIntegrity_Incomplete => "Unvollständig";
    public string NcasIntegrity_Modified => "Modifiziert";
    public string NcasIntegrity_Corrupted => "Korrupt";
    public string NcasIntegrity_Error => "Fehler";
    public string NcasIntegrity_Unknown => "Unbekannt";

    public string Status_SavingFile => "Speichere Datei «{0}»...";

    public string KeysLoading_Starting_Log => ">>> Lade Keys...";
    public string KeysLoading_Successful_Log => ">>> Keys erfolgreich geladen.";
    public string KeysLoading_Error => "Keys konnten nicht geladen werden: {0}.";
    public string WarnNoProdKeysFileFound => "Keine «prod.keys» Datei gefunden.";
    public string InvalidSetting_KeysFileNotFound => "In den Einstellungen definierte Keydatei «{0}» existiert nicht.";
    public string InvalidSetting_BufferSizeInvalid => "In den Einstellungen definierte Puffergröße «{0}» ist ungültig, Wert sollte größer 0 sein.";
    public string InvalidSetting_LanguageNotFound => "In den Einstellungen definierte Sprache «{0}» existiert nicht.";

    public string ToolTip_KeyMissing => "Key «{0}» vom Typ «{1}» fehlt.";

    public string MenuItem_CopyTextToClipboard => "Kopieren";
    public string ContextMenu_OpenFileLocation => "Verzeichnis öffnen...";
    public string OpenFileLocation_Failed_Log => "Fehler beim Öffnen des Speicherorts der Datei «{0}»: {1}";
    public string SettingsView_TitlePageUrl => "Titel Seiten-URL";
    public string OpenTitleWebPage_Failed => "Fehler beim Öffnen der Titel Webseite: {0}";

    public string Log_DownloadFileCanceled => "Herunterladen abgebrochen.";
    public string Log_SaveToDirCanceled => "Speichern des Verzeichnisses abgebrochen.";
    public string Log_SaveFileCanceled => "Speichern der Datei abgebrochen.";
    public string Log_SaveStorageCanceled => "Speicherung des Speichers abgebrochen.";
    public string Log_NcasIntegrityCanceled => "Integritätsprüfung der NCAs abgebrochen.";

    public string RenamingTool_WindowTitle => "Umbenennungswerkzeug";
    public string RenamingTool_Patterns => "Muster";
    public string RenamingTool_ApplicationPattern => "Anwendungsmuster";
    public string RenamingTool_PatchPattern => "Patch Muster";
    public string RenamingTool_AddonPattern => "Add-on Muster";
    public string RenamingTool_InputPath => "Eingabepfad";
    public string RenamingTool_FileFilters => "Filter";
    public string RenamingTool_ToolTip_Patterns =>
        $$"""
         Schlüsselwort-Syntax: 
            {<Keyword>[:<Format>]}
         
         Das Format ist optional und kann sein:
         - U: Großbuchstaben
         - L: Kleinbuchstaben
         
         Beispiele:
           {Title} => Der original Titel
           {Title:U} => Der Titel in Großbuchstaben
         
         Unterstützte Schlüsselwörter:
           • TitleID:
              - Die Inhalts-ID.
           • AppID:
              - Die ID der entsprechenden {{nameof(ContentMetaType.Application)}} (für {{nameof(ContentMetaType.Application)}} Inhalte ist dieser Wert gleich der {TitleID}).
           • PatchId:
              - Wenn der Inhalt eine {{nameof(ContentMetaType.Application)}} ist, entspricht dieser Wert der ID des entsprechenden {{nameof(ContentMetaType.Patch)}} Inhalts, andernfalls ist der Wert null.
           • PatchNum:
              - Wenn der Inhalt eine {{nameof(ContentMetaType.Application)}} ist, ist der Wert in der Regel 0.
              - Wenn der Inhalt ein {{nameof(ContentMetaType.Patch)}} ist, entspricht der Wert der Patch-Nummer.
              - Wenn der Inhalt ein {{nameof(ContentMetaType.AddOnContent)}} ist, entspricht der Wert der Add-On-Patch-Nummer.
           • Title:
              - Der erste Titel aus der Liste der deklarierten Titel.
              - Dieser Wert existiert nur für Inhalte vom Typ {{nameof(ContentMetaType.Application)}} oder {{nameof(ContentMetaType.Patch)}}, jedoch nicht für {{nameof(ContentMetaType.AddOnContent)}}.
           • Ext:
              - Die Erweiterung, die dem erkannten Dateityp entspricht.
           • VerNum:
              - Die Versionsnummer des Inhalts.
           • VerDsp:
              - Die angezeigte Version.
           • WTitle:
              - Der Titel des Inhalts, der aus dem Internet abgerufen wurde.
           • WAppTitle: 
              - Der Titel der entsprechenden {{nameof(ContentMetaType.Application)}}, der aus dem Internet abgerufen wurde.
         
         Verwende  \{ oder \}, um die Zeichen { oder } zu schreiben.
         """;
    public string RenamingTool_ToolTip_BasePattern => $"Das Muster, das für Inhalte des Typs {nameof(ContentMetaType.Application)} verwendet werden soll.";
    public string RenamingTool_ToolTip_PatchPattern => $"Das Muster, das für Inhalte des Typs {nameof(ContentMetaType.Patch)} verwendet werden soll.";
    public string RenamingTool_ToolTip_AddonPattern => $"Das Muster, das für Inhalte des Typs {nameof(ContentMetaType.AddOnContent)} verwendet werden soll.";
    public string RenamingTool_Button_Cancel => "Abbrechen";
    public string RenamingTool_Button_Rename => "Umbenennen";
    public string RenamingTool_GroupBoxInput => "Eingabe";
    public string RenamingTool_GroupBoxNamingSettings => "Benennungseinstellungen";
    public string RenamingTool_BrowseDirTitle => "Verzeichnis auswählen";
    public string RenamingTool_GroupBoxOutput => "Ausgabe";
    public string RenamingTool_Miscellaneous => "Sonstiges";
    public string RenamingTool_InvalidWindowsCharReplacement => "Ersetze unzulässige Windows-Zeichen durch";
    public string RenamingTool_ReplaceWhiteSpaceChars => "Ersetze Leerzeichen-Zeichen";
    public string RenamingTool_ReplaceWhiteSpaceCharsWith => "Ersetze Leerzeichen-Zeichen durch";
    public string RenamingTool_Simulation => "Simulation";
    public string RenamingTool_AutoCloseOpenedFile => "Geöffnete Datei automatisch schließen";
    public string RenamingTool_IncludeSubDirectories => "Unterverzeichnisse einbeziehen";
    public string RenamingTool_ContentTypeNotSupported => "Inhaltstyp «{0}» wird nicht unterstützt.";
    public string RenamingTool_SuperPackageNotSupported => "Super-Paket wird nicht unterstützt.";
    public string RenamingTool_LogNbFilesToRename => ">>> {0} Datei(en) zum Umbenennen...";
    public string RenamingTool_LogSimulationMode => $"[SIMULATION] ";
    public string RenamingTool_LogFileRenamed => $"• {{0}}Datei wurde umbenannt von{Environment.NewLine}\t«{{1}}» zu{Environment.NewLine}\t«{{2}}».";
    public string RenamingTool_LogFileAlreadyNamedProperly => "• {0}«{1}» ist bereits korrekt benannt.";
    public string RenamingTool_LogFailedToRenameFile => "• {0}«{1}»Umbenennen fehlgeschlagen: {2}";
    public string RenamingTool_LogRenamingFailed => "Umbenennen fehlgeschlagen: {0}";
    public string RenamingTool_BadInvalidFileNameCharReplacement => "Die Ersetzungszeichenfolge «{0}» (für ungültige Dateinamenzeichen) darf das ungültige Zeichen «{1}» nicht enthalten.";

    public string Exception_UnexpectedDelimiter => "Unerwarteter Trenner {0} an Position {1} gefunden, verwende stattdessen {2}{0}.";
    public string Exception_EndDelimiterMissing => "End-Trenner {0} fehlt.";
    public string FileRenaming_PatternKeywordUnknown => "Schlüsselwort «{0}» ist unbekannt, erlaubte Schlüsselwörter sind «{1}».";
    public string FileRenaming_EmptyPatternNotAllowed => "Muster darf nicht leer sein.";
    public string FileRenaming_PatternKeywordNotAllowed => "Schlüsselwort «{0}» ist für Muster vom Typ «{1}» nicht erlaubt.";
    public string FileRenaming_StringOperatorUnknown => "Operator «{0}» wird nicht erkannt, erlaubte Operatoren sind «{1}».";
    public string FileRenaming_EmptyDirectoryNotAllowed => "Eingabeverzeichnis darf nicht leer sein.";
    public string Window_Tip_Title => "Hinweis";
}
