using System;
using Emignatik.NxFileViewer.Utils.MVVM.Localization;
using LibHac.Ncm;

namespace Emignatik.NxFileViewer.Localization.Keys;

public class LocalizationKeys_FR : LocalizationKeysBase, ILocalizationKeys
{
    public string Nand_Detected => "Signatures NAND détectées. Intégrité non vérifiée.";
    public string Nand_NameCandidate => "Candidat NAND identifié par le nom du fichier. Consultez les informations NAND pour confirmer et obtenir les détails.";
    public string Nand_Installed => "Plugin NxNandManager installé";
    public string Nand_CustomUpdate => "Effacez puis appliquez le chemin EXE personnalisé pour utiliser les téléchargements gérés.";
    public string Nand_Updating => "Téléchargement et vérification de NxNandManager…";
    public string Nand_Open => "Ouvrir un dump NAND…";
    public string Nand_Info => "Informations NAND";
    public string Nand_Export => "Exporter la partition…";
    public string Nand_SettingsTip => "Laissez le chemin EXE vide pour les téléchargements gérés dans Paramètres → Updates → Plugins → NxNandManager. Un EXE personnalisé reste inchangé.";
    public string Nand_BisKeys => "Fichier de clés BIS (facultatif ; vide = prod.keys actif de NxFileViewer)";
    public string Nand_Tip => "Ouvrez un dump NAND (premier fichier pour un dump fractionné). L’export copie la partition telle quelle, sans déchiffrement. Configurez NxNandManager dans Paramètres → Plugins.";
    public string Nand_NewTarget => "Choisissez un nouveau fichier local. Les fichiers existants ne peuvent pas être remplacés.";
    public string Nand_SourceMissing => "Le fichier source NAND est absent ou n’est pas local.";
    public string Nand_NotInstalled => "NxNandManager non installé. Installez-le dans Paramètres → Updates → Plugins.";
    public string Nand_KeysMissing => "Le fichier de clés BIS configuré est introuvable.";
    public string Nand_ExportFailed => "NxNandManager n’a pas produit de fichier de partition non vide.";
    public string Nand_ExportDone => "Partition exportée.";
    public string Nand_Cancelled => "Annulé ou délai de la requête d’informations dépassé.";
    public string DataUpdate_Firmware => "Références firmware";
    public string DataUpdate_Title => "Mises à jour";
    public string DataUpdate_Titles => "Actualiser TitleDB";
    public string BatchNaming_Unchecked => "Non vérifié";
    public string DataUpdate_LocalFirmwareVersion => "Références locales jusqu'au firmware {0}.";
    public string DataUpdate_NoLocalFirmware => "Aucune référence locale de firmware installée.";
    public string DataUpdate_TitleCatalogDate => "TitleDB {0} : cache local actualisé le {1}.";
    public string DataUpdate_TitleCatalogMissing => "TitleDB {0} : aucun catalogue local.";
    public string DataUpdate_OnlineFirmwareVersion => "Références en ligne jusqu'au firmware {0}.";
    public string Keys_ExistingValidation => "Fichier existant :";
    public string Keys_IncomingValidation => "Nouveau fichier :";
    public string Keys_ReplaceDownloaded => "Utiliser les clés téléchargées et remplacer le fichier existant ?";
    public string Keys_SaveTicketKeys => "Enregistrer les clés de ticket manquantes dans title.keys";
    public string Keys_TicketConflict => "Conflit de clé de ticket pour {0} dans {1} : entrée existante conservée.";
    public string Keys_TicketSaved => "Clé de ticket pour {0} enregistrée dans {1}.";
    public string Tinfoil_StabilityHint => "Tinfoil peut être temporairement indisponible ou instable. En cas d echec, réessayez plus tard ou choisissez une autre source.";
    public string DataUpdate_TitleTip => "Actualise GitHub TitleDB pour la région enregistrée et US dans le dossier du programme. Le fournisseur sélectionné reste inchangé.";
    public string DataUpdate_FirmwareTip => "Les vérifications téléchargent les références sans cache permanent. Seul le bouton séparé enregistre le paquet hors ligne et sauvegarde les anciennes références.";
    public string DataUpdate_CheckFirmware => "Vérifier les références en ligne";
    public string DataUpdate_SaveFirmware => "Actualiser les références hors ligne";
    public string DataUpdate_Working => "Actualisation…";
    public string DataUpdate_TitlesDone => "TitleDB {0} actualisée : {1} entrées avec US.";
    public string DataUpdate_FirmwareSaved => "Références hors ligne actualisées : {0} fichiers.";
    public string DataUpdate_FirmwareChecked => "Références en ligne vérifiées : {0} fichiers, rien enregistré.";
    public string Update_Title => "Mises à jour du programme";
    public string Update_IncludePrereleases => "Inclure les préversions dans les mises à jour du programme";
    public string Update_Prerelease => "Préversion";
    public string Update_Auto => "Rechercher les mises à jour au démarrage";
    public string Update_Check => "Rechercher les mises à jour";
    public string Update_Install => "Télécharger et installer";
    public string Update_Checking => "Recherche de mises à jour…";
    public string Update_Current => "Aucune version publiée plus récente.";
    public string Component_UpdateAvailable => "Mise à jour disponible.";
    public string Component_NotInstalled => "Non installé.";
    public string Component_CustomVersion => "Version personnalisée : vérification automatique indisponible.";
    public string Update_Available => "La version {0} est disponible.";
    public string Update_Failed => "Échec de la mise à jour.";
    public string Update_Confirm => "Télécharger et installer la version {0} ? NxFileViewer redémarrera. Les clés, paramètres et plugins sont conservés.";
    public string Update_Downloading => "Téléchargement et vérification…";
    public string Update_Installing => "Installation…";
    public string Update_Cancelled => "Mise à jour annulée.";
    public string Nsz_Mode => "Mode de compression";
    public string Nsz_ModeAuto => "Automatique (NSZ : solid, XCZ : blocs)";
    public string Nsz_ModeSolid => "Solid / sans blocs";
    public string Nsz_ModeBlock => "Compression par blocs";
    public string Nsz_BlockSize => "Taille des blocs";
    public string Nsz_ModeTip => "Solid compresse un peu mieux. Les blocs permettent des lectures rapides en arrière et aléatoires. Ces options concernent uniquement la compression.";
    public string Workspace_Plugins => "Plugins";
    public string Workspace_Home => "Accueil";
    public string Workspace_File => "Vérification de fichier";
    public string Workspace_Menu => "Menu principal";
    public string Nsz_Replace => "Remplacer";
    public string Nsz_Number => "Enregistrer avec un numéro";
    public string TitlePage_Custom => "Personnalisée";
    public string Info_WithRuntime => "Avec .NET intégré";
    public string Info_WithoutRuntime => "Sans .NET intégré — nécessite .NET 8 Desktop Runtime";
    public string Info_Description => "NxFileViewer affiche et vérifie les fichiers Nintendo Switch : NSP, NSZ, XCI, XCZ, NCA, ZIP et 7z, le firmware et la conversion NSZ.";
    public string Info_Shortcuts => "Raccourcis clavier";
    public string BatchHistory_Show => "Afficher";
    public string BatchHistory_Title => "5 dernières vérifications";
    public string BatchHistory_Resume => "Reprendre";
    public string Dialog_Yes => "Oui";
    public string Dialog_No => "Non";
    public string Nsz_Cancel => "Annuler";
    public string Nsz_DeleteSourcePrompt => "Supprimer les sources après conversion et vérification réussies ? Elles sont conservées en cas d’échec.";
    public string Nsz_SourceDeleted => "Source supprimée";
    public string Nsz_SourceDeleteFailed => "Impossible de supprimer la source";
    public string Nsz_Compress => "Compresser et vérifier…";
    public string Nsz_Decompress => "Décompresser et vérifier…";
    public string Nsz_CompressValid => "Compresser les fichiers valides…";
    public string Nsz_DecompressValid => "Décompresser les fichiers valides…";
    public string Nsz_Update => "Installer / mettre à jour le plugin…";
    public string Nsz_Rollback => "Utiliser la version précédente";
    public string Nsz_SelectDestination => "Choisir le dossier de destination";
    public string Nsz_PythonRuntimeFailed => "NSZ ne peut pas charger sa DLL Python intégrée. Le démarrage échoue avant la vérification des clés ou fichiers. Choisissez une autre CLI NSZ fonctionnelle dans Paramètres → Plugin nicoboss/nsz.";
    public string Nsz_NotInstalled => "Plugin nicoboss/nsz non installé.";
    public string Nsz_Updating => "Mise à jour du plugin NSZ…";
    public string Nsz_SourceSize => "Taille originale (octets)";
    public string Nsz_OutputSize => "Taille finale (octets)";
    public string Nsz_Verified => "Résultat vérifié";
    public string Nsz_Summary => "{0} convertis et vérifiés ; {1} échecs ; {2} non traités. Les originaux sont conservés.";
    public string Nsz_SettingsTip => "Chemin facultatif de la CLI NSZ. Laisser vide pour gérer automatiquement les versions officielles. Source et résultat sont vérifiés ; les originaux sont conservés.";
    public string Nsz_CheckUpdates => "Rechercher les mises à jour stables avant conversion";
    public string Nsz_Level => "Niveau de compression (1–22)";
    public string Nsz_OutputExists => "Le fichier cible existe déjà :";
    public string Nsz_SourceInvalid => "Échec de vérification de la source :";
    public string Nsz_OutputInvalid => "Échec de vérification du résultat :";
    public string Nsz_OutputMissing => "NSZ n’a pas créé le fichier attendu.";
    public string Nsz_KeysMissing => "Aucun fichier prod.keys chargé.";
    public string Nsz_Incompatible => "Cette CLI NSZ ne prend pas en charge les options requises.";
    public string Nsz_OfflineFallback => "Mise à jour indisponible ; utilisation de la version NSZ installée.";
    public string Firmware_NoReferences => "Empreintes indisponibles : GitHub inaccessible et références locales absentes ou invalides.";
    public string Firmware_LoadingOnline => "Chargement des empreintes depuis GitHub…";
    public string Firmware_OnlineSource => "Source : GitHub (chargée pour cette vérification).";
    public string Firmware_OfflineSource => "GitHub indisponible. Empreintes fournies utilisées ; des firmwares récents peuvent manquer.";
    public string Firmware_BrowseZip => "Choisir un ZIP / 7z…";
    public string Firmware_NcaMatches => "Cette NCA figure dans les versions suivantes :";
    public string Firmware_Versions => "Version(s) du firmware";
    public string Firmware_Title => "Vérification du firmware";
    public string Firmware_Unknown => "Aucune référence de firmware correspondante.";
    public string Firmware_Summary => "{0}/{1} fichiers valides ; manquants : {2}, modifiés : {3}, supplémentaires : {4}, doublons : {5}.";
    public string Firmware_Missing => "Manquant";
    public string Firmware_Changed => "Modifié (taille/SHA-256)";
    public string Firmware_Renamed => "Nom incorrect (contenu identique)";
    public string Firmware_RenamedSummary => "Noms incorrects : {0}.";
    public string Firmware_Extra => "NCA supplémentaire";
    public string Firmware_Duplicate => "Nom en double";

    public override bool IsFallback => true;
    public override string DisplayName => "Français";
    public override string CultureName => "fr-FR";
    public override string LanguageAuto => "Automatique";

    public string FileNotSupported_Log => "Fichier «{0}» non supporté.";
    public string OpenFile_Filter => "Fichiers Nintendo Switch (*.nsp;*.nsz;*.xci;*.xcz;*.nca;*.zip;*.7z;*.bin;*.img;*.00)|*.nsp;*.nsz;*.xci;*.xcz;*.nca;*.zip;*.7z;*.bin;*.img;*.00|Paquets de jeux Switch (*.nsp;*.nsz;*.xci;*.xcz)|*.nsp;*.nsz;*.xci;*.xcz|Archives (*.zip;*.7z)|*.zip;*.7z|Fichiers de contenu Switch (*.nca)|*.nca|Images NAND et dumps fractionnés (*.bin;*.img;*.00)|*.bin;*.img;*.00|Tous les fichiers (*.*)|*.*";
    public string MenuItem_File => "Fichier";
    public string MenuItem_Open => "Ouvrir...";
    public string MenuItem_OpenLast => "Ouvrir le _dernier";
    public string MenuItem_Close => "Fermer";
    public string MenuItem_Exit => "Q_uitter";
    public string MenuItem_Tools => "Outils";
    public string MenuItem_CheckIntegrity => "Vérifier l'_intégrité";
    public string MenuItem_CheckDirectoryIntegrity => "Vérifier l’intégrité du dossier…";
    public string BatchNaming_Title => "Nommage";
    public string BatchNaming_Matches => "Conforme";
    public string BatchNaming_Differs => "Non conforme";
    public string BatchNaming_Check => "Vérifier les noms";
    public string BatchNaming_RenameAll => "Renommer les fichiers non conformes";
    public string BatchNaming_Error => "Erreur de nommage";
    public string BatchIntegrity_Title => "Vérification d’intégrité par lot";
    public string BatchIntegrity_SelectDirectory => "Sélectionner le dossier des fichiers Switch";
    public string BatchIntegrity_Browse => "Parcourir…";
    public string BatchIntegrity_IncludeSubdirectories => "Inclure les sous-dossiers";
    public string BatchTable_Columns => "Colonnes…";
    public string BatchTable_Search => "Rechercher";
    public string BatchTable_All => "Tous";
    public string BatchTable_ResetFilters => "Réinitialiser les filtres";
    public string BatchTable_ResetSort => "Réinitialiser le tri";
    public string BatchIntegrity_File => "Fichier";
    public string BatchIntegrity_Path => "Chemin";
    public string BatchIntegrity_Error => "Erreur";
    public string BatchIntegrity_NszDataCorrupted => "Le flux de données NSZ/NCZ compressé est endommagé ou incomplet (échec de la décompression Zstandard).";
    public string BatchIntegrity_IntegrityFailed => "La vérification d'intégrité n'a pas pu être terminée. Consultez le journal pour plus de détails.";
    public string PackageStructure_Filesystem => "Système de fichiers";
    public string Signature_Title => "Signature NCA";
    public string Signature_Passed => "Réussie";
    public string Signature_NotPassed => "Échouée";
    public string Signature_Unchecked => "Non vérifiée";
    public string ToolTip_PackageStructure => "Structure selon Nx Game Info :\nScene (XCI) : partitions Update, Normal et Secure.\nConverti (XCI) : uniquement Secure ; typique de NSP → XCI.\nScene (NSP) : legalinfo.xml, nacp.xml, programinfo.xml et cardspec.xml ; typique des releases BBB.\nHomebrew (NSP) : authoringtoolinfo.xml présent.\nCDN (NSP) : certificat (.cert) et ticket (.tik) ; typique des dumps eShop CDN.\nConverti (NSP) : sans certificat ni ticket ; typique de XCI → NSP.\nSystème de fichiers : titres NAX0 installés sur une carte SD Switch.\nIncomplet : contenus NCA uniquement. NSZ/XCZ suivent les règles du paquet correspondant ; NCZ compte comme NCA.";
    public string ToolTip_NcaSignature => "Réussie : signatures NCA valides, attendues pour les titres officiels.\nÉchouée : au moins une signature NCA invalide ; possible pour les homebrews, anormal pour les titres officiels.\nNon vérifiée : vérification NCA non terminée. Lancez le contrôle d’intégrité.\nCette indication concerne les en-têtes NCA ; ACID est une signature NPDM distincte.";
    public string ToolTip_Permission => "Sûr : aucun accès aux services de fichiers ou bit 0x8000000000000000 absent.\nNon sûr : accès aux services de fichiers et bit 0x8000000000000000 présent (EraseMmc).\nDangereux : accès aux services de fichiers et masque 0xffffffffffffffff (toutes les permissions).\nNon sûr/Dangereux ne devrait concerner que les homebrews. Disponible uniquement pour les jeux de base et mises à jour. Ce classement ne constitue pas une évaluation complète de sécurité.";
    public string ToolTip_AcidSignature => "Signature de la section ACID de main.npdm, indépendante de la signature de l’en-tête NCA.";
    public string PackageStructure_Title => "Structure du paquet";
    public string PackageStructure_Scene => "Version Scene";
    public string PackageStructure_Cdn => "Copie CDN";
    public string PackageStructure_Converted => "Converti";
    public string PackageStructure_Homebrew => "Homebrew";
    public string PackageStructure_Incomplete => "Incomplet";
    public string PackageStructure_Unknown => "Inconnu";
    public string FileInfo_FileSize => "Taille du fichier";
    public string FileInfo_CompressionRatio => "Taux de compression";
    public string FileInfo_Uncompressed => "décompressé";
    public string FileInfo_SystemUpdate => "Mise à jour système incluse (XCI)";
    public string BatchIntegrity_Export => "Exporter CSV…";
    public string BatchIntegrity_Start => "Démarrer";
    public string BatchIntegrity_OpenSelected => "Ouvrir dans la vérification de fichier";
    public string BatchIntegrity_MoveSelected => "Déplacer…";
    public string BatchIntegrity_MoveValid => "Déplacer les fichiers valides…";
    public string BatchIntegrity_SelectMoveDestination => "Sélectionner la destination des fichiers valides";
    public string BatchIntegrity_Moving => "Déplacement";
    public string MenuItem_Options => "Options";
    public string MenuItem_Settings => "Paramètres";
    public string MenuItem_ReloadKeys => "Recharger les clés";
    public string MenuItem_OpenTitleWebPage => "Ouvrir la page Web du titre...";
    public string MenuItem_ShowRenameToolWindow => "Outil de renommage...";

    public string Packages_Title => "Fichier a contenus multiples";
    public string DisplayVersion => "Version affichée";
    public string Presentation_Title => "Présentation";
    public string ToolTip_AvailableLanguages => "Le titre, l'éditeur et l'icône peuvent changer selon la langue sélectionnée.";
    public string AvailableLanguages => "Langues";
    public string AppTitle => "Titre";
    public string Publisher => "Editeur";
    public string Security_Title => "Sécurité du programme";
    public string Security_Level => "Évaluation";
    public string Security_FileSystemPermissions => "Autorisations du système de fichiers";
    public string Security_AcidSignature => "Signature ACID";
    public string Security_Safe => "Sûr";
    public string Security_Unsafe => "Non sûr";
    public string Security_Dangerous => "Dangereux";
    public string Security_Unavailable => "Indisponible";
    public string Security_Details => "Services autorisés ({0}) : {1}";

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
    public string Lng_BrazilianPortuguese => "Brésilien Portugais";
    public string Lng_Unknown => "Inconnue";

    public string SettingsView_Title => "Paramètres";
    public string SettingsView_Button_Apply => "Appliquer";
    public string SettingsView_Button_Cancel => "Annuler";
    public string SettingsView_Button_Reset => "Réinitialiser";
    public string SettingsView_GroupBoxKeys => "Clés";
    public string SettingsView_Title_KeysEffectiveFilePath => "Chemin effectif";
    public string SettingsView_Title_KeysCustomFilePath => "Chemin personnalisé";
    public string SettingsView_Title_KeysDownloadUrl => "URL de téléchargement";
    public string KeysValidation_MissingFile => "Aucun fichier trouvé.";
    public string KeysValidation_ValidEntries => "Valide ({0} entrées).";
    public string KeysValidation_MissingMasterKeys => "Clés principales manquantes : {0}.";
    public string CnmtOverview_BaseTitleId => "ID du titre de base";
    public string CnmtOverview_MasterKey => "Clé principale requise";
    public string CnmtOverview_MinimumApplicationVersion => "Version minimale de l'application (DLC)";
    public string CnmtOverview_Distribution => "Distribution";
    public string KeysValidation_InvalidMasterKeys => "Clés principales non valides : {0}.";
    public string KeysValidation_InvalidLines => "Lignes mal formées : {0}.";
    public string KeysValidation_EmptyFile => "Le fichier ne contient aucune entrée valide.";
    public string KeysValidation_FirmwareEstimate => "Révision valide la plus élevée : {0} — prend en charge les contenus jusqu'au firmware {1}.";
    public string KeysValidation_UnsupportedMasterKeys => "Nouvelle révision de clé principale détectée : {0}. Cette version du programme ne peut pas encore la valider ni l'associer à un firmware ; une mise à jour est nécessaire.";
    public string SettingsView_ToolTip_Keys => """
                                               Les clés sont obligatoires pour pouvoir ouvrir des fichiers Nintendo Switch chiffrés (XCI, NSP, ...).
                                               Chaque fichier Nintendo Switch officiel est chiffré avec des clés spéciques à la version du firmware pour lequel il a été construit.

                                               Afin de pouvoir ouvrir n'importe quel fichier sans erreur, veuillez vous assurer de toujours posséder un fichier "prod.keys" contenant l'ensemble de toutes les clés de tous les firmwares existants.

                                               Les fichiers de clé doivent contenir une clé par ligne, sous la forme «NOM_CLE = VALEUR_HEXADECIMAL»."
                                               """;

    public string SettingsView_ToolTip_ProdKeys => """
                                                   Ce fichier contient les clés communes à toutes les consoles Switch. Ce fichier est requis pour ouvrir les contenus chiffrés.
                                                   Le programme cherchera la présence du fichier dans l'ordre suivant:
                                                       1. le chemin défini par ce paramètre
                                                       2. le répertoire courant du programme
                                                       3. le dossier «%UserProfile%\\.switch»

                                                   Au démarrage, le programme peut automatiquement télécharger le fichier de clés quand aucun n'est trouvé sur le système.
                                                   Le fichier de clés sera téléchargé dans le répertoire courant de l'application.
                                                   """;

    public string SettingsView_ToolTip_TitleKeys => """
                                                    Vous pouvez optionnellement spécifier un fichier contenant les clés spécifiques de certains jeux.
                                                    Le programme cherchera la présence du fichier dans l'ordre suivant:
                                                        1. le chemin défini par ce paramètre
                                                        2. le répertoire courant du programme
                                                        3. le dossier «%UserProfile%\\.switch»

                                                    Au démarrage, le programme peut automatiquement télécharger le fichier de clés quand aucun n'est trouvé sur le système.
                                                    Le fichier de clés sera téléchargé dans le répertoire courant de l'application.
                                                    """;

    public string SettingsView_LogFileRetention => "Conserver les journaux (1–100 lancements)";
    public string SettingsView_LogLevel => "Niveau de log";
    public string SettingsView_ToolTip_LogLevel => "Le niveau de log indique à partir de quel niveau les messages sont logués.";
    public string SettingsView_CheckBox_AlwaysReloadKeysBeforeOpen => "Toujours recharger les clés avant l'ouverture d'un fichier";
    public string SettingsView_CheckBox_InjectTicketKeys => "Injecter les clés depuis les fichiers ticket (*.tik)";
    public string SettingsView_Title_Language => "Langue";
    public string SettingsView_Title_Theme => "Thème";
    public string SettingsView_Title_NczOptions => "Paramètres NSZ/XCZ";
    public string SettingsView_ToolTip_NczBlockLessCompression => """
                                                                  Les fichiers NSZ ou XCZ sont composés de fichiers NCZ qui sont des fichiers NCA compressés.
                                                                  Les fichiers NCZ peuvent être compressés sans utiliser la méthode de compression par bloc, ce qui rend impossible les accès en lecture aléatoire de façon efficace.
                                                                  Ainsi, si le fichier est volumineux et qu'il faut lire une petite partie vers la fin, il sera nécessaire de décompresser tout le flux jusqu'à atteindre la partie souhaitée.
                                                                  Les gros fichiers peuvent donc prendre beaucoup de temps avant d'être ouverts.
                                                                  Privilégiez l'utilisation de la compression par bloc pour les fichiers volumineux.
                                                                  Notez que si vous choisissez de ne pas permettre l'ouverture des fichiers compressés sans bloc, cela n'affectera pas les fonctionnalités de contrôle d'intégrité.
                                                                  """;

    public string SettingsView_CheckBox_NczOpenBlocklessCompression => "Ouvrir les NCZ compressés sans bloc";
    public string SettingsView_Title_Integrity => "Intégrité";
    public string SettingsView_CheckBox_IgnoreMissingDeltaFragments => "Ignorer les fragments de delta manquants";
    public string SettingsView_ToolTip_IgnoreMissingDeltaFragments => $"""
                                                                       Les fichiers de patch peuvent contenir des fichiers de mise à jour incrémentielle (connu sous le nom de {ContentType.DeltaFragment}).
                                                                       Ces fragments ne sont pas obligatoires pour mettre à jour une application, et sont parfois retirés volontairement.
                                                                       Cochez cette option si vous voulez ignorer les {ContentType.DeltaFragment} absents lors de la vérification de l'intégrité.
                                                                       """;

    public string SettingsView_Miscellaneous => "Divers";
    public string SettingsView_ToolTip_OpenKeysLocation => "Ouvrir l'emplacement du fichier de clés.";
    public string SettingsView_ToolTip_BrowseKeys => "Parcourir...";
    public string SettingsView_ToolTip_DownloadKeys => "Télécharger à partir de l'URL spécifiée.";

    public string BrowseKeysFile_ProdTitle => "Sélectionnez les clés \"prod\"";
    public string BrowseKeysFile_TitleTitle => "Sélectionnez les clés \"title\"";
    public string BrowseKeysFile_Filter => "Fichier de clés (*.keys)|*.keys|Paquets de jeux Switch (*.nsp;*.nsz;*.xci;*.xcz)|*.nsp;*.nsz;*.xci;*.xcz|Archives (*.zip;*.7z)|*.zip;*.7z|Fichiers de contenu Switch (*.nca)|*.nca|Images NAND et dumps fractionnés (*.bin;*.img;*.00)|*.bin;*.img;*.00|Tous les fichiers (*.*)|*.*";

    public string SuspiciousFileExtension => "L'extension du fichier «{0}» semble invalide, «{1}» ou «{2}» était attendu.";
    public string DragMeAFile => "Glisse moi un petit fichier supporté par ici :)";
    public string MultipleFilesDragAndDropNotSupported => "L'ouverture de plusieurs fichiers n'est pas supportée, seul le premier sera ouvert.";

    public string CnmtOverview_Title => "Informations du package";
    public string CnmtOverview_TitleId => "ID du titre";
    public string CnmtOverview_ContentType => "Type";
    public string CnmtOverview_TitleVersion => "Version";
    public string CnmtOverview_MinimumSystemVersion => "Version minimum du système";
    public string CnmtOverview_BuildID => "Build ID";
    public string CnmtOverview_BuildID_NotAvailableBecauseSectionIsSparse => "Non disponible (contenu dispersé)";
    public string CnmtOverview_IsDemo => "Démo";

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
    public string LoadingError_FailedToLoadIcon_Log => "Echec de chargement de l'icône: {0}";
    public string LoadingError_NcaFileMissing_Log => "L'entrée NCA «{0}» de type «{1}» est manquante.";
    public string LoadingError_NoCnmtFound_Log => "Auncun CNMT trouvé!";
    public string LoadingError_NacpFileMissing_Log => "Fichier NACP «{0}» non trouvé!";
    public string LoadingError_NcaMissingSection_Log => "Le fichier NCA de type de contenu «{0}» ne contient pas la section de type «{0}».";
    public string LoadingError_MainFileMissing_Log => "Fichier «{0}» non trouvé!";
    public string LoadingError_IconMissing_Log => "Le fichier d'icône «{0}» est manquant.";
    public string LoadingError_XciSecurePartitionNotFound_Log => "Partition sécurisée XCI non trouvée!";
    public string LoadingError_FailedToGetNcaSectionFsHeader => "Echec de récupération de l'entête du système de fichier NCA pour la section «{0}»: {1}";
    public string LoadingError_FailedToOpenMainFile => "Echec d'ouverture du fichier Main: {0}";
    public string LoadingError_FailedToLoadMainFile => "Echec de chargement du fichier Main: {0}";
    public string LoadingError_FailedToOpenNpdmFile => "Echec d'ouverture de main.npdm : {0}";
    public string LoadingError_FailedToLoadNpdmFile => "Echec de l'analyse de main.npdm : {0}";
    public string LoadingError_FailedToLoadTicketFile => "Echec de chargement du fichier ticket: {0}";
    public string LoadingError_FailedToLoadTitleIdKey => "Echec de chargement de la clé du Title ID à partir du fichier ticket «{0}»: {1}";
    public string LoadingError_NczBlocklessCompressionDisabled => "L'ouverture de NCZ sans compression par bloc est désactivé dans les paramètres.";

    public string LoadingInfo_TitleIdKeySuccessfullyInjected => "La clé du Title ID «{0}={1}» trouvée dans le fichier ticket «{2}» a été ajoutée avec succès dans le trousseau de clés.";
    public string LoadingWarning_TitleIdKeyReplaced => "La clé du Title ID «{0}={1}» trouvée dans le fichier ticket «{2}» a été utilisée pour remplacer la clé existante «{0}={2}» du trousseau de clés.";
    public string LoadingDebug_TitleIdKeyAlreadyExists => "La clé du Title ID «{0}={1}» trouvée dans le fichier ticket «{2}» était déjà enregistrée dans le trousseau.";

    public string KeysFileUsed => "Fichier «{0}» utilisé: {1}";
    public string NoneKeysFile => "[aucun]";

    public string Status_DownloadingFile => "Téléchargement du fichier «{0}»...";
    public string Log_DownloadingFileFromUrl => "Téléchargement du fichier «{0}» à partir de l'URL «{1}»...";
    public string Log_FileSuccessfullyDownloaded => "Fichier «{0}» téléchargé avec succès.";
    public string Log_FailedToDownloadFileFromUrl => "Echec de téléchargement du fichier «{0}» à partir de l'URL «{1}»: {2}";

    public string ToolTip_PatchNumber => "Numéro de patch {0}";
    public string Log_OpeningFile => "=====> {0} <=====";
    public string MainModuleIdTooltip => "Egalement connu sous le nom de «Build ID» (ou BID).";
    public string ATaskIsAlreadyRunning => "Une tâche est déjà en cours...";
    public string FileInfo_Title => "Fichier";
    public string Title_FileInfo_FileType => "Type";
    public string Title_FileInfo_Compression => "Compression";
    public string Title_FileInfo_Integrity => "Intégrité";
    public string ToolTip_NcasIntegrity => $"""
                                            La vérification de l'intégrité consiste à vérifier l'intégrité de chaque NCA (ou NCZ).

                                            Le résultat de l'intégrité peut être l'une des valeurs suivantes:
                                            - {NcasIntegrity_NoNca}: Aucun NCA trouvé.
                                            - {NcasIntegrity_Unchecked}: Intégrité non vérifiée.
                                            - {NcasIntegrity_InProgress}: Vérification de l'intégrité en cours.
                                            - {NcasIntegrity_Original}: Tous les NCAs sont originaux (signature et hash ok).
                                            - {NcasIntegrity_Incomplete}: Tous les NCAs sont originaux, mais certains sont manquants.
                                            - {NcasIntegrity_Modified}: Au moins un NCA est modifié (signature pas ok, mais hash ok).
                                            - {NcasIntegrity_Corrupted}: Au moins un NCA est corrompu (hash incorrect).
                                            - {NcasIntegrity_Error}: Une erreur est survenue pendant la vérification de l'intégrité.

                                            Le détail de chaque NCA analysé peut être trouvé dans l'onglet «Contenu».
                                            """;

    public string AvailableContents => "Contenus:";
    public string MultiContentPackageToolTip => "Le package contient plusieurs contenus («{0}» détecté).";

    public string NcasIntegrity_Error_NcaMissing => "L'intégrité du NCA «{0}» ne peut être vérifiée, NCA manquant.";
    public string NcasIntegrity_Error_Log => "Echec de vérification de l'intégrité des NCAs: {0}";
    public string NcaIntegrity_GetOriginalNcaError => "Echec de récupération du NCA original: {0}";
    public string NcaIntegrity_GetOriginalNcaError_Log => "Echec de récupération du NCA original à partir du NCA «{0}»: {1}";

    public string NcaHeaderSignature_Valid_Log => "La signature de l'entête du NCA «{0}» est valide.";
    public string NcaHeaderSignature_Invalid => "La vérification de la signature de l'entête du NCA a échoué avec le statut «{0}».";
    public string NcaHeaderSignature_Invalid_Log => "La vérification de la signature de l'entête du NCA «{0}» a échoué avec le statut «{1}».";
    public string NcaHeaderSignature_Error => "Echec de vérification de la signature de l'entête du NCA: {0}.";
    public string NcaHeaderSignature_Error_log => "Echec de vérification de la signature de l'entête du NCA «{0}»: {1}";

    public string NcaHash_VerificationStart_Log => ">>> La vérification du hash des NCAs débute...";
    public string NcaHash_VerificationEnd_Log => ">>> La vérification du hash des NCAs est terminée.";
    public string NcaHash_NcaItem_CantExtractHashFromName => "Echec d'extraction du hash attendu à partir du nom du NCA.";
    public string NcaHash_CantExtractHashFromName_Log => "Echec d'extraction du hash attendu à partir du nom du NCA «{0}».";
    public string NcaHash_Valid_Log => "Le hash du NCA «{0}» est valide.";
    public string NcaHash_NcaItem_Invalid => "Hash non valide.";
    public string NcaHash_Invalid_Log => "Le hash du NCA «{0}» n'est pas valide.";
    public string NcaHash_NcaItem_Exception => "Echec de vérification du hash: {0}";
    public string NcaHash_Exception_Log => "Echec de vérification du hash du NCA «{0}»: {1}";
    public string NcaHash_ProgressText => "Hashage du NCA {0}/{1}...";

    public string CancelAction => "Annuler";
    public string Status_Ready => "Prêt.";
    public string LoadingFile_PleaseWait => "Chargement, veuillez patienter...";

    public string NcasIntegrity_NoNca => "Aucun NCA";
    public string NcasIntegrity_Unchecked => "Non vérifié";
    public string NcasIntegrity_InProgress => "En cours";
    public string NcasIntegrity_Original => "Original";
    public string NcasIntegrity_Incomplete => "Incomplet";
    public string NcasIntegrity_Modified => "Modifié";
    public string NcasIntegrity_Corrupted => "Corrompu";
    public string NcasIntegrity_Error => "Erreur";
    public string NcasIntegrity_Unknown => "Inconnu";

    public string Status_SavingFile => "Sauvegarde du fichier «{0}»...";

    public string KeysLoading_Starting_Log => ">>> Chargement des clés...";
    public string KeysLoading_Successful_Log => ">>> Clés chargées avec succès.";
    public string KeysLoading_UnusedKey_Log => "Information : la clé supplémentaire «{0}» n’est pas utilisée par cette version du programme.";
    public string KeysLoading_Error => "Echec de chargement des clés: {0}.";
    public string WarnNoProdKeysFileFound => "Aucun fichier «prod.keys» trouvé.";
    public string InvalidSetting_KeysFileNotFound => "Le fichier de clé «{0}» défini dans les paramètres n'existe pas.";
    public string InvalidSetting_BufferSizeInvalid => "La taille du buffer «{0}» défini dans les paramètres n'est pas valide, la valeur doit être strictement supérieure à 0.";
    public string InvalidSetting_LanguageNotFound => "La langue «{0}» définie dans les paramètres n'existe pas.";

    public string ToolTip_KeyMissing => "La clé «{0}» de type «{1}» est manquante.";

    public string MenuItem_CopyTextToClipboard => "Copier";
    public string ContextMenu_OpenFileLocation => "Ouvrir l'emplacement...";
    public string OpenFileLocation_Failed_Log => "Echec d'ouverture de l'emplacement du fichier «{0}»: {1}";
    public string SettingsView_TitlePageUrl => "URL du titre";
    public string SettingsView_TitleInfoApiUrl => "URL de l’API des titres";
    public string SettingsView_TitleInfoProvider => "Source des noms de titres";
    public string SettingsView_TitleDbRegion => "Région / langue TitleDB";
    public string SettingsView_TitleDbCacheTip => "TitleDB est conservée localement et actualisée chaque jour. Le cache reste disponible en cas de panne. Les titres manquants sont aussi recherchés dans US.en.";
    public string BatchIntegrity_FileType => "Type de fichier";
    public string BatchIntegrity_PackageType => "Type de paquet";
    public string BatchIntegrity_ShowOnlyErrors => "Afficher uniquement les fichiers défectueux";
    public string OpenTitleWebPage_Failed => "Echec d'ouverture de la page Web: {0}";

    public string Log_DownloadFileCanceled => "Téléchargement annulé.";
    public string Log_SaveToDirCanceled => "Sauvegarde du répertoire annulé.";
    public string Log_SaveFileCanceled => "Sauvegarde du fichier annulé.";
    public string Log_SaveStorageCanceled => "Sauvegarde du stockage annulé.";
    public string Log_NcasIntegrityCanceled => "Intégrité des NCAs annulé.";

    public string RenamingTool_TargetDirectory => "Dossier cible (vide = dossier actuel)";
    public string RenamingTool_FolderTip => "Utilisez / pour les dossiers, par exemple DLC/{WTitle}.{Ext:L}.";
    public string RenamingTool_OldName => "Ancien nom";
    public string RenamingTool_NewName => "Nouveau nom";
    public string RenamingTool_StatusError => "Erreur";
    public string RenamingTool_StatusUnchanged => "Inchangé";
    public string RenamingTool_StatusSimulation => "Simulation";
    public string RenamingTool_StatusRenamed => "Renommé";
    public string RenamingTool_WindowTitle => "Outil de renommage";
    public string RenamingTool_Patterns => "Patterns";
    public string RenamingTool_ApplicationPattern => "Pattern d'application";
    public string RenamingTool_PatchPattern => "Pattern de patch";
    public string RenamingTool_AddonPattern => "Pattern d'add-on";
    public string RenamingTool_InputPath => "Chemin d'entrée";
    public string RenamingTool_FileFilters => "Filtres";
    public string RenamingTool_ToolTip_Patterns =>
        $$"""
         Dossier cible et sous-dossiers:
           Un dossier cible vide utilise le dossier actuel du fichier.
           Séparez les sous-dossiers par /, par exemple:
             DLC/{WTitle}.{Ext:L}
             {WAppTitle}/DLC/{WTitle}.{Ext:L}
           Utilisez uniquement des dossiers relatifs, sans chemin absolu ni .. .
           La simulation affiche les chemins complets sans créer de dossiers.
           Le renommage crée les dossiers manquants sans écraser les fichiers.

         Syntaxe d'un mot clé:
            {<MotClé>[:<Format>]}

         Le format est facultatif, et peut valoir:
            - U: Majuscule
            - L: Minuscule

         Exemples:
            {Title} => Le titre original
            {Title:U} => Le titre en majuscule

         Liste des mots clés:
           • TitleId:
              - L'id du contenu.
           • AppId:
              - L'id de l'{{nameof(ContentMetaType.Application)}} correspondante (pour des contenus de type {{nameof(ContentMetaType.Application)}}, cette valeur est égale à {TitleId}).
           • PatchId:
              - Si le contenu est une {{nameof(ContentMetaType.Application)}}, cette valeur est égale à l'id du contenu de {{nameof(ContentMetaType.Patch)}} correspondant, sinon zéro.
           • PatchNum:
              - Si le contenu est une {{nameof(ContentMetaType.Application)}}, cette valeur vaut généralement 0.
              - Si le contenu est un {{nameof(ContentMetaType.Patch)}}, cette valeur correspond au numéro du patch.
              - Si le contenu est un {{nameof(ContentMetaType.AddOnContent)}}, cette valeur correspond au numéro de patch de l'add-on.
           • Title:
              - Le premier titre parmi la liste des titres définis.
              - Cette valeur n'existe que pour des contenus de type {{nameof(ContentMetaType.Application)}} ou {{nameof(ContentMetaType.Patch)}}, mais pas pour des contenus de type {{nameof(ContentMetaType.AddOnContent)}}.
           • Ext:
              - L'extension correspondant au type de fichier détecté.
           • VerNum:
              - Le numéro de version du contenu.
           • VerDsp:
              - La version affichée.
           • WTitle:
              - Le titre du contenu récupéré depuis Internet.
           • WAppTitle:
              - Le titre de l'{{nameof(ContentMetaType.Application)}} correspondante, récupéré depuis Internet.

         Utilisez \{ ou \} pour écrire littéralement les caractères { ou }.
         """;

    public string RenamingTool_ToolTip_BasePattern => $"Le pattern à utiliser pour des contenus de type {nameof(ContentMetaType.Application)}.";
    public string RenamingTool_ToolTip_PatchPattern => $"Le pattern à utiliser pour des contenus de type {nameof(ContentMetaType.Patch)}.";
    public string RenamingTool_ToolTip_AddonPattern => $"Le pattern à utiliser pour des contenus de type {nameof(ContentMetaType.AddOnContent)}.";
    public string RenamingTool_Button_Cancel => "Annuler";
    public string RenamingTool_Button_Rename => "Renommer";
    public string RenamingTool_GroupBoxInput => "Entrée";
    public string RenamingTool_GroupBoxNamingSettings => "Paramètres de nommage";
    public string RenamingTool_BrowseDirTitle => "Sélectionnez un répertoire";
    public string RenamingTool_GroupBoxOutput => "Sortie";
    public string RenamingTool_Miscellaneous => "Divers";
    public string RenamingTool_InvalidWindowsCharReplacement => "Remplacer les caractères Windows non autorisés avec";
    public string RenamingTool_ReplaceWhiteSpaceChars => "Remplacer les espaces blancs";
    public string RenamingTool_ReplaceWhiteSpaceCharsWith => "Remplacer les espaces blancs avec";
    public string RenamingTool_Simulation => "Simulation";
    public string RenamingTool_AutoCloseOpenedFile => "Fermer automatiquement le fichier ouvert";
    public string RenamingTool_IncludeSubDirectories => "Inclure les sous répertoires";
    public string RenamingTool_ContentTypeNotSupported => "Type de contenu «{0}» non supporté.";
    public string RenamingTool_SuperPackageNotSupported => "Super package non supporté.";
    public string RenamingTool_LogNbFilesToRename => ">>> {0} fichier(s) à renommer...";

    public string RenamingTool_LogSimulationMode => "[SIMULATION] ";
    public string RenamingTool_LogFileRenamed => $"• {{0}}Ficher renommé de{Environment.NewLine}\t«{{1}}» à{Environment.NewLine}\t«{{2}}».";
    public string RenamingTool_LogFileAlreadyNamedProperly => "• {0}«{1}» déjà nommé correctement.";
    public string RenamingTool_LogFailedToRenameFile => "• {0}«{1}»Echec de renommage: {2}";
    public string RenamingTool_LogRenamingFailed => "Echec de renommage: {0}";
    public string RenamingTool_BadInvalidFileNameCharReplacement => "La chaine de remplacement «{0}» (caractères interdits dans les noms de fichiers), ne peut contenir le caractère interdit «{1}».";

    public string Exception_UnexpectedDelimiter => "Délimiteur {0} non attendu à la position {1}, utilisez {2}{0} à la place.";
    public string Exception_EndDelimiterMissing => "Le délimiteur de fin {0} est manquant.";
    public string FileRenaming_PatternKeywordUnknown => "Mot clé «{0}» inconnu, liste des mots clés autorisés: «{1}».";
    public string FileRenaming_EmptyPatternNotAllowed => "La pattern ne peut être vide.";
    public string FileRenaming_PatternKeywordNotAllowed => "Le mot clé «{0}» n'est pas autorisé pour les patterns de type «{1}».";
    public string FileRenaming_StringOperatorUnknown => "L'opérateur «{0}» n'est pas reconnu, les opérateurs autorisés sont «{1}».";
    public string FileRenaming_EmptyDirectoryNotAllowed => "Le répertoire d'entrée ne peut être vide.";
    public string Window_Tip_Title => "Astuce";
    public string Nsz_Installed => "Plugin NSZ installé";
    public string Nsz_CustomExecutable => "Exécutable personnalisé";
    public string Settings_Program => "Programme";
    public string Nsz_PhaseSource => "Vérifier la source";
    public string Nsz_PhaseOutput => "Vérifier le résultat";
    public string Nsz_PhasePublish => "Enregistrer le résultat";
    public string Batch_IncludeArchives => "Inclure ZIP / 7z";
    public string Batch_Scan => "Analyser les fichiers";
    public string Batch_VerifyAll => "Vérifier tous les fichiers";
    public string File_MissingKeys => "Clés requises manquantes. Le contenu ne peut pas être entièrement lu. Vérifiez prod.keys / title.keys.";
    public string File_CopyMissingKeys => "Copier les noms des clés manquantes";
    public string Keys_ProgramFolder => "Dossier du programme";
    public string Keys_SharedFolder => "Profil utilisateur (.switch)";
    public string Keys_InUse => "Utilisé";
    public string Keys_DownloadAll => "Télécharger les clés";
    public string Keys_DownloadHost => "IP / nom d’hôte du téléchargement";
    public string Keys_DownloadHostTip => "{IP} dans les URL est remplacé par cette adresse. Destination : chemin personnalisé si défini, sinon dossier du programme.";
    public string Keys_CopyToSwitch => @"Copier les clés actuelles vers %USERPROFILE%\.switch";
    public string Keys_ReplaceShared => "Remplacer les clés existantes ? Les fichiers sources seront conservés.";
    public string Keys_SharedCopied => "Clés disponibles dans le dossier partagé .switch.";
    public string Batch_ScanAndVerify => "Analyser les fichiers et vérifier leur intégrité";
}
