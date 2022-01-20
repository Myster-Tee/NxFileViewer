using System;
using Emignatik.NxFileViewer.Utils.MVVM.Localization;

namespace Emignatik.NxFileViewer.Localization.Keys
{
    public class LocalizationKeys_FR : LocalizationKeysBase, ILocalizationKeys
    {
        public override bool IsFallback => true;
        public override string DisplayName => "Français";
        public override string CultureName => "fr-FR";
        public override string LanguageAuto => "Automatique";

        public string FileNotSupported_Log => "Fichier «{0}» non supporté.";
        public string OpenFile_Filter => "Fichiers Nintendo Switch (*.nsp;*.nsz;*.xci;*.xcz)|*.nsp;*.nsz;*.xci;*.xcz|Tous les fichiers (*.*)|*.*";
        public string MenuItem_File => "_Fichier";
        public string MenuItem_Open => "_Ouvrir...";
        public string MenuItem_OpenLast => "Ouvrir le _dernier";
        public string MenuItem_Close => "_Fermer";
        public string MenuItem_Exit => "Q_uitter";
        public string MenuItem_Tools => "_Outils";
        public string MenuItem_VerifyNcasHash => "Vérifier les _hash";
        public string MenuItem_VerifyNcasHeaderSignature => "Verify les _signatures";
        public string MenuItem_Options => "_Options";
        public string MenuItem_Settings => "_Paramètres";
        public string MenuItem_ReloadKeys => "Recharger les clés";
        public string MenuItem_OpenTitleWebPage => "Ouvrir la page Web du titre...";

        public string MultiContentPackage => "Package a contenu multiples";
        public string GeneralInfo => "Informations Générales";
        public string TitleId => "ID du titre";
        public string DisplayVersion => "Version affichée";
        public string Presentation => "Présentation";
        public string ToolTip_AvailableLanguages => "Le titre, l'éditeur et l'icône peuvent changer selon la langue sélectionnée.";
        public string AvailableLanguages => "Langues";
        public string AppTitle => "Titre";
        public string Publisher => "Editeur";

        public string Lng_Unknown => "Inconnue";
        public string Lng_AmericanEnglish => "Americain";
        public string Lng_BritishEnglish => "Anglais";
        public string Lng_CanadianFrench => "Canadien";
        public string Lng_Dutch => "Allemand";
        public string Lng_French => "Français";
        public string Lng_German => "Germain";
        public string Lng_Italian => "Italien";
        public string Lng_Japanese => "Japonais";
        public string Lng_Korean => "Koréen";
        public string Lng_LatinAmericanSpanish => "Amérique Latine";
        public string Lng_Portuguese => "Portuguais";
        public string Lng_Russian => "Russe";
        public string Lng_SimplifiedChinese => "Chinois Simplifié";
        public string Lng_Spanish => "Espagnol";
        public string Lng_TraditionalChinese => "Chinois Traditionnel";

        public string SettingsView_Title => "Paramètres";
        public string SettingsView_Button_Save => "Sauvegarder";
        public string SettingsView_Button_Cancel => "Annuler";

        public string SettingsView_GroupBoxKeys => "Clés";
        public string SettingsView_Title_ActualKeysFilePath => "Chemin courant";
        public string SettingsView_Title_KeysCustomFilePath => "Chemin personnalisé";
        public string SettingsView_Title_KeysDownloadUrl => "URL de téléchargement";

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


        public string SettingsView_ToolTip_ConsoleKeys => "Vous pouvez optionnellement spécifier un fichier contenant les clés uniques de console." + Environment.NewLine +
                                                         "Le programme cherchera les fichiers aux emplacements suivants:" + Environment.NewLine +
                                                         "    1. le chemin défini par ce paramètre" + Environment.NewLine +
                                                         "    2. le répertoire courant du programme" + Environment.NewLine +
                                                         "    3. le dossier «%UserProfile%\\.switch»" + Environment.NewLine + Environment.NewLine +
                                                         "Le fichier doit contenir une clé par ligne, sous la forme «NOM_CLE = VALEUR_HEXADECIMAL».";


        public string SettingsView_LogLevel => "Niveau de log";
        public string SettingsView_ToolTip_LogLevel => "Le niveau de log indique à partir de quel niveau les messages sont logués.";
        public string SettingsView_CheckBox_AlwaysReloadKeysBeforeOpen => "Toujours recharger les clés avant l'ouverture d'un fichier";
        public string SettingsView_Title_Language => "Langue";

        public string BrowseKeysFile_ProdTitle => "Sélectionnez les clés \"prod\"";
        public string BrowseKeysFile_ConsoleTitle => "Sélectionnez les clés \"console\"";
        public string BrowseKeysFile_TitleTitle => "Sélectionnez les clés \"title\"";
        public string BrowseKeysFile_Filter => "Fichier de clés (*.keys)|*.keys|Tous les fichiers (*.*)|*.*";

        public string SuspiciousFileExtension => "L'extension du fichier «{0}» semble invalide, «{1}» ou «{2}» était attendu.";
        public string DragMeAFile => "Glisse moi un petit fichier supporté par ici :)";
        public string MultipleFilesDragAndDropNotSupported => "L'ouverture de plusieurs fichiers n'est pas supportée, seul le premier sera ouvert.";
        public string ContentType => "Type";
        public string TitleVersion => "Version";
        public string MinimumSystemVersion => "Version minimum du system";

        public string ContextMenu_SaveImage => "Sauvegarder...";
        public string CopyTitleImageError => "Echec de copie du fichier image: {0}";
        public string SaveTitleImageError => "Echec de sauvegarde du fichier image: {0}";

        public string SaveDialog_Title => "Sauvegarder sous";
        public string SaveDialog_ImageFilter => "Image";
        public string SaveDialog_AnyFileFilter => "Fichier";
        public string SaveFile_Error => "Echec de sauvegarde du fichier: {0}";

        public string ContextMenu_CopyImage => "Copier";

        public string TabOverview => "Aperçu";
        public string TabContent => "Contenu";
        public string GroupBoxStructure => "Structure";
        public string GroupBoxProperties => "Propriétés";

        public string ContextMenu_ShowItemErrors => "Montrer les erreurs...";
        public string ContextMenu_SaveSectionItem => "Sauvegarder le contenu de la section...";
        public string ContextMenu_SaveDirectoryItem => "Sauvegarder le répertoire...";
        public string ContextMenu_SaveFileItem => "Sauvegarder le fichier...";
        public string ContextMenu_SavePartitionFileItem => "Sauvegarder le fichier de partition...";
        public string ContextMenu_SaveNcaFileRaw => "Sauvegarder le NCA brute...";
        public string ContextMenu_SaveNcaFilePlaintext => "Sauvegarder le NCA déchiffré...";

        public string SettingsLoadingError => "Echec du chargment des paramètres: {0}";
        public string SettingsSavingError => "Echec de sauvegarde des paramètres: {0}";

        public string LoadingError_Failed => "Echec de chargement du fichier «{0}»: {1}";
        public string LoadingError_FailedToCheckIfXciPartitionExists => "Echec de vérification de l'existence de la partition XCI: {0}";
        public string LoadingError_FailedToOpenXciPartition => "Echec d'ouverture de la partition XCI: {0}";
        public string LoadingError_FailedToLoadXciContent => "Echec de chargement du contenu du XCI: {0}";
        public string LoadingError_FailedToOpenPartitionFile => "Echec d'ouverture du fichier de partition: {0}";
        public string LoadingError_FailedToLoadNcaFile => "Echec de chargement du fichier NCA: {0}";
        public string LoadingError_FailedToLoadPartitionFileSystemContent => "Echec du chargement du contenu du fichier de partition système: {0}";
        public string LoadingError_FailedToCheckIfSectionCanBeOpened => "Echec de vérification de la possibilité d'ouverture de la section: {0}";
        public string LoadingError_FailedToOpenNcaSectionFileSystem => "Echec d'ouverture du contenu de la section «{0}» du NCA: {1}";
        public string LoadingError_FailedToLoadSectionContent => "Echec de chargement du contenu de la section: {0}";
        public string LoadingError_FailedToGetFileSystemDirectoryEntries => "Echec de récupération des entrées «répertoire» du système de fichier: {0}";
        public string LoadingError_FailedToOpenNacpFile => "Echec d'ouverture du fichier NACP: {0}";
        public string LoadingError_FailedToLoadNacpFile => "Echec de chargement du fichier NACP: {0}";
        public string LoadingError_FailedToOpenCnmtFile => "Echec d'ouverture du fichier CNMT: {0}";
        public string LoadingError_FailedToLoadCnmtFile => "Echec de chargement du fichier CNMT: {0}";
        public string LoadingError_FailedToLoadNcaContent => "Echec de chargement du contenu du NCA: {0}";
        public string LoadingError_FailedToLoadDirectoryContent => "Echec de chargement du contenu du répertoire: {0}";
        public string LoadingError_FailedToLoadIcon => "Echec de chargement de l'icône: {0}";
        public string LoadingError_NcaFileMissing => "L'entrée NCA «{0}» de type «{1}» est manquante.";
        public string LoadingError_NoCnmtFound => "Auncun CNMT trouvé!";
        public string LoadingError_NacpFileMissing => "Fichier NACP «{0}» non trouvé!";
        public string LoadingError_IconMissing => "Le fichier d'icône «{0}» est manquant.";
        public string LoadingError_XciSecurePartitionNotFound => "Partition sécurisée XCI non trouvée!";
        public string LoadingError_FailedToGetNcaSectionFsHeader => "Echec de récupération de l'entête du système de fichier NCA pour la section «{0}»: {1}";
        public string LoadingError_FailedToOpenMainFile => "Echec d'ouverture du fichier Main: {0}";
        public string LoadingError_FailedToLoadMainFile => "Echec de chargement du fichier Main: {0}";
        public string LoadingError_FailedToLoadTicketFile => "Echec de chargement du fichier ticket: {0}";
        public string LoadingError_FailedToLoadTitleIdKey => "Echec de chargement de la clé du Title ID à partir du fichier ticket «{0}»: {1}";
        public string LoadingInfo_TitleIdKeySuccessfullyInjected => "La clé du Title ID «{0}={1}» trouvée dans le fichier ticket «{2}» a été ajoutée avec succès dans le trousseau de clés.";
        public string LoadingWarning_TitleIdKeyReplaced => "La clé du Title ID «{0}={1}» trouvée dans le fichier ticket «{2}» a été utilisée pour remplacer la clé existante «{0}={2}» du trousseau de clés.";
        public string LoadingDebug_TitleIdKeyAlreadyExists => "La clé du Title ID «{0}={1}» trouvée dans le fichier ticket «{2}» était déjà enregistrée dans le trousseau.";

        public string KeysFileUsed => "Fichier «{0}» utilisé: {1}";
        public string NoneKeysFile => "[aucun]";

        public string Status_DownloadingFile => "Téléchargement du fichier «{0}»...";
        public string Log_DownloadingFileFromUrl => "Téléchargement du fichier «{0}» à partir de l'URL «{1}»...";
        public string Log_FileSuccessfullyDownloaded => "Fichier «{0}» téléchargé avec succès.";
        public string Log_FailedToDownloadFileFromUrl => "Echec de téléchargement du fichier «{0}» à partir de l'URL «{1}»: {2}";

        public string ToolTip_PatchLevel => "Niveau de patch {0}";
        public string Log_OpeningFile => "=====> {0} <=====";
        public string MainModuleIdTooltip => "Egalement connu sous le nom de «Build ID» (ou BID).";
        public string ATaskIsAlreadyRunning => "Une tâche est déjà en cours...";

        public string Integrity => "Integrité";
        public string AvailableContents => "Contenus:";
        public string MultiContentPackageToolTip => "Le package contient plusieurs contenus («{0}» détecté).";

        public string Title_NcasHeaderSignature => "Signature:";
        public string ToolTip_NcasHeaderSignature => "Vérifie la signature de l'entête de chaque NCA.";
        public string Title_NcasHash => "Hash:";
        public string ToolTip_NcasHash => "Vérifie le hash de chaque NCA." + Environment.NewLine +
                                          "Un hash valide permet de garantir que le fichier n'a pas été corrompu.";

        public string NcaHeaderSignature_VerificationStart_Log => ">>> La vérification de la signature débute...";
        public string NcaHeaderSignature_VerificationEnd_Log => ">>> La vérification de la signature est terminée.";
        public string NcaHeaderSignature_Valid_Log => "La signature de l'entête du NCA «{0}» est valide.";
        public string NcaHeaderSignature_Invalid => "La vérification de la signature de l'entête du NCA a échoué avec le statut «{0}».";
        public string NcaHeaderSignature_Invalid_Log => "La vérification de la signature de l'entête du NCA «{0}» a échoué avec le statut «{1}».";
        public string NcaHeaderSignature_Error => "Echec de vérification de la signature de l'entête du NCA: {0}.";
        public string NcaHeaderSignature_Error_log => "Echec de vérification de la signature de l'entête du NCA «{0}»: {1}";
        public string NcasHeaderSignature_Error_Log => "Echec de la vérification de la signature des entêtes des NCAs: {0}";

        public string NcaHash_VerificationStart_Log => ">>> La vérification du hash des NCAs débute...";
        public string NcaHash_VerificationEnd_Log => ">>> La vérification du hash des NCAs est terminée.";
        public string NcaHash_Valid_Log => "La hash du NCA «{0}» est valide.";
        public string NcaHash_NcaItem_Invalid => "Hash non valide.";
        public string NcaHash_Invalid_Log => "Le hash du NCA «{0}» n'est pas valide.";
        public string NcaHash_CnmtItem_Error_NcaMissing => "Impossible de vérifier le hash du NCA «{0}», le fichier n'a pas été trouvé.";
        public string NcaHash_NcaItem_Exception => "Echec de vérification du hash: {0}";
        public string NcaHash_Exception_Log => "Echec de vérification du hash du NCA «{0}»: {1}";
        public string NcasHash_Error_Log => "Echec de vérification du hash des NCAs: {0}";
        public string NcaHash_ProgressText => "Hashage du NCA {0}/{1}...";

        public string CancelAction => "Annuler";
        public string Status_Ready => "Prêt.";
        public string LoadingFile_PleaseWait => "Chargement, veuillez patienter...";

        public string NcasValidity_NoNca => "Aucun NCA";
        public string NcasValidity_Unchecked => "Non vérifié";
        public string NcasValidity_InProgress => "En cours";
        public string NcasValidity_Invalid => "Invalide";
        public string NcasValidity_Valid => "Valide";
        public string NcasValidity_Error => "Erreur";
        public string NcasValidity_Unknown => "Inconnu";

        public string Status_SavingFile => "Sauvegarde du fichier «{0}»...";

        public string KeysLoading_Starting_Log => ">>> Chargement des clés...";
        public string KeysLoading_Successful_Log => ">>> Clés chargées avec succès.";
        public string KeysLoading_Error => "Echec de chargement des clés: {0}.";
        public string WarnNoProdKeysFileFound => "Aucun fichier «prod.keys» trouvé.";
        public string InvalidSetting_KeysFileNotFound => "Le fichier de clé «{0}» défini dans les paramètres n'existe pas.";
        public string InvalidSetting_BufferSizeInvalid => "La taille du buffer «{0}» défini dans les paramètres n'est pas valide, la valeur doit être strictement supérieure à 0.";
        public string InvalidSetting_LanguageNotFound => "La langue «{0}» définie dans les paramètres n'existe pas.";

        public string ToolTip_KeyMissing => "La clé «{0}» de type «{1}» est manquante.";

        public string EditFile_Failed_Log => "Echec d'édition du fichier «{0}»: {1}";
        public string MenuItem_CopyTextToClipboard => "Copier";
        public string ContextMenu_OpenFileLocation => "Ouvrir l'emplacement...";
        public string OpenFileLocation_Failed_Log => "Echec d'ouverture de l'emplacement du fichier «{0}»: {1}";
        public string SettingsView_TitlePageUrl => "URL du titre";
        public string OpenTitleWebPage_Failed => "Echec d'ouverture de la page Web: {0}";

        public string Log_DownloadFileCanceled => "Téléchargement annulé.";
        public string Log_SaveToDirCanceled => "Sauvegarde du répertoire annulé.";
        public string Log_SaveFileCanceled => "Sauvegarde du fichier annulé.";
        public string Log_SaveStorageCanceled => "Sauvegarde du stockage annulé.";
        public string Log_NcaHashCanceled => "Hash des NCAs annulé.";
    }
}