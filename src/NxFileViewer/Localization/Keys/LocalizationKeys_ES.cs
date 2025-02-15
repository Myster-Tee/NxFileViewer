using System;
using Emignatik.NxFileViewer.Utils.MVVM.Localization;
using LibHac.Ncm;

namespace Emignatik.NxFileViewer.Localization.Keys;

public class LocalizationKeys_ES : LocalizationKeysBase, ILocalizationKeys
{
    public override bool IsFallback => true;
    public override string DisplayName => "Español";
    public override string CultureName => "es-ES";
    public override string LanguageAuto => "Auto";

    public string FileNotSupported_Log => "El Archivo «{0}» no es soportado.";
    public string OpenFile_Filter => "Nintendo Switch files (*.nsp;*.nsz;*.xci;*.xcz)|*.nsp;*.nsz;*.xci;*.xcz|All files (*.*)|*.*";
    public string MenuItem_File => "_Archivo";
    public string MenuItem_Open => "_Abrir...";
    public string MenuItem_OpenLast => "Abrir _reciente";
    public string MenuItem_Close => "_Cerrar";
    public string MenuItem_Exit => "_Salir";
    public string MenuItem_Tools => "_Herramientas";
    public string MenuItem_CheckIntegrity => "Verificar _integridad";
    public string MenuItem_Options => "_Opciones";
    public string MenuItem_Settings => "_Configuraciones";
    public string MenuItem_ReloadKeys => "Recargar llaves";
    public string MenuItem_OpenTitleWebPage => "Abrir sitio web del título...";
    public string MenuItem_ShowRenameToolWindow => "Herramienta de renombrado...";

    public string Packages_Title => "Archivo Multipaquete";
    public string DisplayVersion => "Versión mostrada";
    public string Presentation_Title => "Representación";
    public string ToolTip_AvailableLanguages => "El título, Editor e ícono pueden cambiar según el idioma seleccionado.";
    public string AvailableLanguages => "Idiomas";
    public string AppTitle => "Título";
    public string Publisher => "Editor";

    public string Lng_AmericanEnglish => "Inglés EE.UU.A.";
    public string Lng_BritishEnglish => "Inglés";
    public string Lng_CanadianFrench => "Francés de Canadá";
    public string Lng_Dutch => "Holandés";
    public string Lng_French => "Francés";
    public string Lng_German => "Alemán";
    public string Lng_Italian => "Italiano";
    public string Lng_Japanese => "Japonés";
    public string Lng_Korean => "Coreano";
    public string Lng_LatinAmericanSpanish => "Español Latinoamericano";
    public string Lng_Portuguese => "Portugués";
    public string Lng_Russian => "Ruso";
    public string Lng_SimplifiedChinese => "Chino Simplificado";
    public string Lng_Spanish => "Español";
    public string Lng_TraditionalChinese => "Chino Tradicional";
    public string Lng_BrazilianPortuguese => "Portugués Brasilero";
    public string Lng_Unknown => "Desconocido";

    public string SettingsView_Title => "Configuraciones";
    public string SettingsView_Button_Apply => "Aplicar";
    public string SettingsView_Button_Cancel => "Cancelar";
    public string SettingsView_Button_Reset => "Valores Predeterminados";
    public string SettingsView_GroupBoxKeys => "Llaves";
    public string SettingsView_Title_KeysEffectiveFilePath => "Ruta Actual";
    public string SettingsView_Title_KeysCustomFilePath => "Ruta personalizada";
    public string SettingsView_Title_KeysDownloadUrl => "URL de descarga";
    public string SettingsView_ToolTip_Keys => """
                                               Las llaves son necesarias para poder abrir archivos del formato Nintendo Switch encriptados (XCI, NSP, ...).
                                               Cada archivo oficial con formato Nintendo Switch está encriptado con las llaves específicas del firmware con que fueron construidos.

                                               Asegúrese de contar con el archivo de llaves «prod.keys» más actualizado, para poder abrir archivos con el formato Nintendo Switch sin errores.
                                               
                                               El archivo deberá contener una llave por línea, con el formato «NOMBRE_LLAVE = VALOR_HEXADECIMAL».
                                               """;
    public string SettingsView_ToolTip_ProdKeys => """
                                                   Este archivo contiene las llaves comunes a todas las consolas Switch.  Este archivo se requiere para poder leer el contenido de títulos encriptados.
                                                   NXFileViewer buscará por éste archivo en las siguientes rutas en orden:
                                                       1. La ruta definida en esta configuración.
                                                       2. La carpeta donde se encuentra el programa.
                                                       3. La carpeta «%UserProfile%\\.switch»
                                                   
                                                   Al iniciar, NXFileViewer puede descargar de forma automática el archivo de llaves si no se encuentra uno en el sistema.
                                                   El archivo se descargará en la carpeta donde se encuentra el programa.
                                                   """;

    public string SettingsView_ToolTip_TitleKeys => """
                                                    Alternativamente se puede indicar la ruta de un archivo que contenga las llaves específicas de juegos.
                                                    NXFileViewer buscará por éste archivo en las siguientes rutas en orden:
                                                        1. La ruta definida en esta configuración.
                                                        2. La carpeta donde se encuentra el programa.
                                                        3. La carpeta «%UserProfile%\\.switch»
                                                    
                                                    Al iniciar, NXFileViewer puede descargar de forma automática el archivo de llaves si no se encuentra uno en el sistema.
                                                    El archivo se descargará en la carpeta donde se encuentra el programa.
                                                    """;

    public string SettingsView_LogLevel => "Nivel de depuración";
    public string SettingsView_ToolTip_LogLevel => "El nivel de depuración indica el mínimo nivel de registros a crear en la bitácora de eventos.";
    public string SettingsView_CheckBox_AlwaysReloadKeysBeforeOpen => "Siempre recargar las llaves antes de abrir un archivo";
    public string SettingsView_CheckBox_InjectTicketKeys => "Inyectar llaves desde los archivos de tiquetes (*.tik)";
    public string SettingsView_Title_Language => "Idioma";
    public string SettingsView_Title_NczOptions => "Configuraciones NSZ/XCZ";

    public string SettingsView_ToolTip_NczBlockLessCompression => """
                                                                  Los archivos NSZ o XCZ están compuestos por archivos del tipo NCZ que a su vez son archivos NCA comprimidos.
                                                                  Los archivos NCZ se pueden comprimir con y sin el método de compresión por bloques, lo que hace la lectura aleatoria de datos imposible.
                                                                  Por lo tanto, si el archivo es grande y se requiere extraer un pedazo pequeño cercano al final del archivo, será necesario descomprimirlo completamente.
                                                                  Por ende, archivos grandes pueden tomar un tiempo en ser abiertos.
                                                                  Se exhorta usar compresión por bloques para archivos grandes.
                                                                  El no seleccionar la opción «Permitir abrir archivos NCZ sin método de compresión de bloques», no afectará las características de verificación de integridad.
                                                                  """;

    public string SettingsView_CheckBox_NczOpenBlocklessCompression => "Permitir abrir archivos NCZ sin método de compresión de bloques";
    public string SettingsView_Title_Integrity => "Integridad";
    public string SettingsView_CheckBox_IgnoreMissingDeltaFragments => "Ignorar fragmentos delta perdidos";
    public string SettingsView_ToolTip_IgnoreMissingDeltaFragments => $"""
                                                                      Los archivos de parches pueden contener archivos completos de actualización o archivos de actualización incrementales (conocidos como «{ContentType.DeltaFragment}»).
                                                                      Dichos fragmentos no son obligatorios para la actualización de un aplicativo y algunas veces son removidos.
                                                                      Seleccione ésta opción si desea ignorar que los fragmentos «{ContentType.DeltaFragment}» no existen durante la verificación de integridad.
                                                                      """;

    public string SettingsView_Miscellaneous => "Misceláneos";
    public string SettingsView_ToolTip_OpenKeysLocation => "Abrir ubicación del archivo.";
    public string SettingsView_ToolTip_BrowseKeys => "Buscar...";
    public string SettingsView_ToolTip_DownloadKeys => "Descargar del URL indicado.";

    public string BrowseKeysFile_ProdTitle => "Seleccionar archivo de llaves «prod.keys»";
    public string BrowseKeysFile_TitleTitle => "Seleccionar archivo de llaves «title.keys»";
    public string BrowseKeysFile_Filter => "Archivos de llaves (*.keys)|*.keys|Todos los archivos (*.*)|*.*";

    public string SuspiciousFileExtension => "La extensión del archivo «{0}» no parece ser válida, Se esperaba «{1}» o «{2}».";
    public string DragMeAFile => "Arrastre aquí un archivo soportado 8-)";
    public string MultipleFilesDragAndDropNotSupported => "No se soporta arrastrar y soltar múltiples archivos, sólo se abrirá el primer archivo.";

    public string CnmtOverview_Title => "Información del Paquete";
    public string CnmtOverview_TitleId => "ID del título";
    public string CnmtOverview_ContentType => "Tipo";
    public string CnmtOverview_TitleVersion => "Versión";
    public string CnmtOverview_MinimumSystemVersion => "Versión mínima del sistema";
    public string CnmtOverview_BuildID => "Build ID";
    public string CnmtOverview_BuildID_NotAvailableBecauseSectionIsSparse => "No disponible (poco contenido)";
    public string CnmtOverview_IsDemo => "Demo";

    public string ContextMenu_SaveImage => "Guardar...";
    public string CopyTitleImageError => "Error al copiar imagen de título: {0}";
    public string SaveTitleImageError => "Error al guardar imagen de título: {0}";

    public string SaveDialog_Title => "Guardar como";
    public string SaveDialog_ImageFilter => "Imagen";
    public string SaveDialog_AnyFileFilter => "Archivo";
    public string SaveFile_Error => "Error al guardar archivo: {0}";

    public string ContextMenu_CopyImage => "Copiar";

    public string TabOverview => "Descripción";
    public string TabContent => "Contenido";
    public string GroupBoxStructure => "Estructura";
    public string GroupBoxProperties => "Propiedades";

    public string ContextMenu_ShowItemErrors => "Mostrar errores...";
    public string ContextMenu_SaveSectionItem => "Guardar sección de contenido...";
    public string ContextMenu_SaveDirectoryItem => "Guardar directorio...";
    public string ContextMenu_SaveFileItem => "Guardar Archivo...";
    public string ContextMenu_SavePartitionFileItem => "Guardar archivo de partición...";
    public string ContextMenu_SaveNcaFileRaw => "Guardar archivo NCA sin procesar...";
    public string ContextMenu_SaveNcaFilePlaintext => "Guardar archivo NCA en texto plano...";

    public string SettingsLoadingError => "Error al cargar configuraciones: {0}";
    public string SettingsSavingError => "Error al guardar configuraciones: {0}";

    public string LoadingError_Failed => "Error al cargar el archivo «{0}»: {1}";
    public string LoadingError_FailedToCheckIfXciPartitionExists => "Error al verificar que la partición XCI existe: {0}";
    public string LoadingError_FailedToOpenXciPartition => "Error al abrir la partición XCI: {0}";
    public string LoadingError_FailedToLoadXciContent => "Error al abrir el contenido XCI: {0}";
    public string LoadingError_FailedToOpenPartitionFile => "Error al abrir el archivo de partición: {0}";
    public string LoadingError_FailedToLoadNcaFile => "Error al cargar el archivo NCA: {0}";
    public string LoadingError_FailedToLoadPartitionFileSystemContent => "Error al cargar contenido de la partición de sistema de archivos: {0}";
    public string LoadingError_FailedToCheckIfSectionCanBeOpened => "Error al verificar si la sección puede ser abierta: {0}";
    public string LoadingError_FailedToOpenNcaSectionFileSystem => "Error al abrir el contenido de la sección NCA «{0}»: {1}";
    public string LoadingError_FailedToLoadSectionContent => "Error al cargar el contenido de la sección: {0}";
    public string LoadingError_FailedToGetFileSystemDirectoryEntries => "Error al cargar las entradas de archivo del directorio de sistema: {0}";
    public string LoadingError_FailedToOpenNacpFile => "Error al abrir el archivo NACP: {0}";
    public string LoadingError_FailedToLoadNacpFile => "Error al cargar el archivo NACP: {0}";
    public string LoadingError_FailedToOpenCnmtFile => "Error al abrir el archivo CNMT: {0}";
    public string LoadingError_FailedToLoadCnmtFile => "Error al cargar el archivo CNMT: {0}";
    public string LoadingError_FailedToLoadNcaContent => "Error al cargar el contenido NCA: {0}";
    public string LoadingError_FailedToLoadDirectoryContent => "Error al cargar el contenido del directorio: {0}";
    public string LoadingError_FailedToLoadIcon_Log => "Error al cargar el ícono: {0}";
    public string LoadingError_NcaFileMissing_Log => "No existe La entrada NCA «{0}» del tipo «{1}».";
    public string LoadingError_NoCnmtFound_Log => "¡No se encontró entrada CNMT!";
    public string LoadingError_NacpFileMissing_Log => "¡No se encontró el archivo NACP «{0}!";
    public string LoadingError_NcaMissingSection_Log => "A el contenido NCA del tipo «{0}» le falta la sección del tipo «{0}».";
    public string LoadingError_MainFileMissing_Log => "¡No se encontró el archivo «{0}»!";
    public string LoadingError_IconMissing_Log => "No se encuentra el archivo de ícono «{0}».";
    public string LoadingError_XciSecurePartitionNotFound_Log => "¡No se encontró la partición segura XCI!";
    public string LoadingError_FailedToGetNcaSectionFsHeader => "Error al obtener el encabezado de sistema NCA para la sección «{0}»: {1}";
    public string LoadingError_FailedToOpenMainFile => "Error al abrir el Archivo Principal: {0}";
    public string LoadingError_FailedToLoadMainFile => "Error al cargar el Archivo Principal: {0}";
    public string LoadingError_FailedToLoadTicketFile => "Error al cargar el archivo de tiquete: {0}";
    public string LoadingError_FailedToLoadTitleIdKey => "Error al cargar el ID de Título desde el archivo de tiquete «{0}»: {1}";
    public string LoadingError_NczBlocklessCompressionDisabled => "La apertura de archivos NCZ con compresión sin bloques está deshabilitada en las configuraciones.";

    public string LoadingInfo_TitleIdKeySuccessfullyInjected => "Se encontró la llave de título ID «{0}={1}» en el archivo de tiquete «{2}», Se adicionó satisfactoriamente al conjunto de llaves.";
    public string LoadingWarning_TitleIdKeyReplaced => "Se encontró la llave de título ID «{0}={1}» en el archivo de tiquete «{2}», se ha usado como reemplazo de la llave de título ID «{0}={2}» que había en el conjunto de llaves.";
    public string LoadingDebug_TitleIdKeyAlreadyExists => "Se encontró la llave de título ID «{0}={1}» en el archivo de tiquete «{2}», y ya estaba registrada en el conjunto de llaves existente.";

    public string KeysFileUsed => "«{0}» archivo utilizado: {1}";
    public string NoneKeysFile => "[ninguno]";

    public string Status_DownloadingFile => "Descargando el archivo «{0}»...";
    public string Log_DownloadingFileFromUrl => "Descargando «{0}» desde el URL «{1}»...";
    public string Log_FileSuccessfullyDownloaded => "Archivo «{0}» descargado correctamente.";
    public string Log_FailedToDownloadFileFromUrl => "Error al descargar «{0}» desde el URL «{1}»: {2}";

    public string ToolTip_PatchNumber => "Número de parche {0}";
    public string Log_OpeningFile => "=====> {0} <=====";
    public string MainModuleIdTooltip => "También conocido como «Build ID» (or BID).";
    public string ATaskIsAlreadyRunning => "Ya existe una tarea ejecutándose...";
    public string FileInfo_Title => "Archivo";
    public string Title_FileInfo_FileType => "Tipo";
    public string Title_FileInfo_Compression => "Compresión";
    public string Title_FileInfo_Integrity => "Integridad";
    public string ToolTip_NcasIntegrity => $"""
                                           Una verificación de integridad consiste en verificar la integridad de cada NCA (o NCZ).
                                           
                                           El resultado de la Verificación de Integridad puede ser uno de los siguientes:
                                           - {NcasIntegrity_NoNca}: No se encuentra archivo NCA.
                                           - {NcasIntegrity_Unchecked}: Verificación de Integridad no realizada.
                                           - {NcasIntegrity_InProgress}: Verificación de Integridad en progreso.
                                           - {NcasIntegrity_Original}: Todos los NCAs son originales (las firmas son correctas).
                                           - {NcasIntegrity_Incomplete}: Todos los NCAs son originales, sin embargo, algunos no pueden ser encontrados.
                                           - {NcasIntegrity_Modified}: Por lo menos un NCA ha sido modificado (la firma no es correcta pero el hash es correcto).
                                           - {NcasIntegrity_Corrupted}: Por lo menos un NCA está corrupto (el hash es inválido).
                                           - {NcasIntegrity_Error}: Ha ocurrido un error durante la verificación de Integridad.
                                           
                                           Los detalles del análisis de cada NCA se encuentran en la ficha «{TabContent}».
                                           """;

    public string AvailableContents => "Contenidos:";
    public string MultiContentPackageToolTip => "El paquete actual consta de múltiples contenidos (se detectaron «{0}»).";

    public string NcasIntegrity_Error_NcaMissing => "La integridad del NCA «{0}» no puede ser verificada, No existe el NCA.";
    public string NcasIntegrity_Error_Log => "Error al verificar la integridad de los NCAs: {0}";
    public string NcaIntegrity_GetOriginalNcaError => "Error al obtener el NCA original: {0}";
    public string NcaIntegrity_GetOriginalNcaError_Log => "Error al obtener el NCA original del NCA «{0}»: {1}";

    public string NcaHeaderSignature_Valid_Log => "La firma del encabezado del NCA «{0}» es inválida.";
    public string NcaHeaderSignature_Invalid => "La verificación de la firma del encabezado del NCA falló con el código de estado «{0}».";
    public string NcaHeaderSignature_Invalid_Log => "La verificación de la firma del NCA «{0}» falló con el código de estado «{1}».";
    public string NcaHeaderSignature_Error => "Error al verificar el encabezado de la firma NCA: {0}.";
    public string NcaHeaderSignature_Error_log => "Error al verificar la firma del encabezado NCA «{0}»: {1}";

    public string NcaHash_VerificationStart_Log => ">>> Verificación del hash de los NCAs ha iniciado...";
    public string NcaHash_VerificationEnd_Log => ">>> Verificación del hash de los NCAs ha terminado.";
    public string NcaHash_NcaItem_CantExtractHashFromName => "Error al extraer el has esperado del nombre del NCA.";
    public string NcaHash_CantExtractHashFromName_Log => "Error al extraer el has esperado del nombre del NCA «{0}».";
    public string NcaHash_Valid_Log => "El hash del NCA «{0}» no es válido.";
    public string NcaHash_NcaItem_Invalid => "El hash es inválido.";
    public string NcaHash_Invalid_Log => "El hash del NCA «{0}» no es válido.";
    public string NcaHash_NcaItem_Exception => "Error al verificar el hash: {0}";
    public string NcaHash_Exception_Log => "Error al verificar el hash del NCA «{0}»: {1}";
    public string NcaHash_ProgressText => "Verificando el hash del NCA {0}/{1}...";

    public string CancelAction => "Cancelar";
    public string Status_Ready => "Completado.";
    public string LoadingFile_PleaseWait => "Por favor espere, Cargando...";

    public string NcasIntegrity_NoNca => "Sin NCA";
    public string NcasIntegrity_Unchecked => "No verificado";
    public string NcasIntegrity_InProgress => "En progreso";
    public string NcasIntegrity_Original => "Original";
    public string NcasIntegrity_Incomplete => "Incompleto";
    public string NcasIntegrity_Modified => "Modificado";
    public string NcasIntegrity_Corrupted => "Corrupto";
    public string NcasIntegrity_Error => "Error";
    public string NcasIntegrity_Unknown => "Desconocido";

    public string Status_SavingFile => "Guardando archivo «{0}»...";

    public string KeysLoading_Starting_Log => ">>> Cargando llaves...";
    public string KeysLoading_Successful_Log => ">>> Llaves cargadas.";
    public string KeysLoading_Error => "Error al cargar llaves: {0}.";
    public string WarnNoProdKeysFileFound => "No se encontró el archivo «prod.keys».";
    public string InvalidSetting_KeysFileNotFound => "El archivo de llaves «{0}» definido en las configuraciones, no existe.";
    public string InvalidSetting_BufferSizeInvalid => "El tamaño de Buffer «{0}» definido en las configuraciones no es un valor válido, debe ser mayor a 0.";
    public string InvalidSetting_LanguageNotFound => "El idioma «{0}» definido en las configuraciones no existe.";

    public string ToolTip_KeyMissing => "No existe La llave «{0}» del tipo «{1}».";

    public string MenuItem_CopyTextToClipboard => "Copiar";
    public string ContextMenu_OpenFileLocation => "Abrir ubicación del archivo...";
    public string OpenFileLocation_Failed_Log => "Error al abrir la ubicación del archivo «{0}»: {1}";
    public string SettingsView_TitlePageUrl => "URL de la página de Títulos";
    public string OpenTitleWebPage_Failed => "Error al abrir la página Web de títulos: {0}";

    public string Log_DownloadFileCanceled => "Descarga cancelada.";
    public string Log_SaveToDirCanceled => "Guardado de directorio cancelado.";
    public string Log_SaveFileCanceled => "Guardado de archivo cancelado.";
    public string Log_SaveStorageCanceled => "Guardado de almacenamiento cancelado.";
    public string Log_NcasIntegrityCanceled => "Verificación de Integridad de NCAs cancelada.";

    public string RenamingTool_WindowTitle => "Herramienta de renombrado";
    public string RenamingTool_Patterns => "Patrones";
    public string RenamingTool_ApplicationPattern => "Patrón de aplicativo";
    public string RenamingTool_PatchPattern => "Patrón de parche";
    public string RenamingTool_AddonPattern => "Patrón de adiciones";
    public string RenamingTool_InputPath => "Ruta con archivos";
    public string RenamingTool_FileFilters => "Filtros";
    public string RenamingTool_ToolTip_Patterns =>
        $$"""
         Sintáxis de las llaves: 
            {<Llave>[:<Formato>]}
         
         El formato opcional puede ser:
         - U: Mayúsculas
         - L: Minúsculas
         
         Ejemplos:
           {Title} => Título original
           {Title:U} => Título en mayúsculas
         
         Llaves soportadas:
           • TitleId:
              - El identificador del contenido.
           • AppId:
              - El identificador  correspondiente a {{nameof(ContentMetaType.Application)}} (para los contenidos {{nameof(ContentMetaType.Application)}}, éste valor es igual a {TitleId}).
           • PatchId:
              - Si el contenido es {{nameof(ContentMetaType.Application)}}, este valor será igual al identificador del contenido correspondiente {{nameof(ContentMetaType.Patch)}}, o de lo contrario cero.
           • PatchNum:
              - Si el contenido es una {{nameof(ContentMetaType.Application)}}, normalmente el valor es 0.
              - Si el contenido es un {{nameof(ContentMetaType.Patch)}}, el valor corresponde al número del parche.
              - Si el contenido es un {{nameof(ContentMetaType.AddOnContent)}}, el valor corresponde al número de adición del parche.
           • Title:
              - El primer título de los títulos declarados.
              - Este valor existe solo en contenidos del tipo {{nameof(ContentMetaType.Application)}} o {{nameof(ContentMetaType.Patch)}}, pero no para los tipos {{nameof(ContentMetaType.AddOnContent)}}.
           • Ext:
              - La extensión correspondiente al tipo de archivo detectado.
           • VerNum:
              - El número de versión del contenido.
           • VerDsp:
              - La versión a mostrar.
           • WTitle:
              - El título consultado desde la Internet.
           • WAppTitle: 
              - El título de la {{nameof(ContentMetaType.Application)}} correspondiente, consultado desde la Internet.
         
         Utilice las secuencias \{ o \} para escribir caracteres literales { o }.
         """;
    public string RenamingTool_ToolTip_BasePattern => $"El patrón a utilizar para contenidos del tipo {nameof(ContentMetaType.Application)}.";
    public string RenamingTool_ToolTip_PatchPattern => $"El patrón a utilizar para contenidos del tipo {nameof(ContentMetaType.Patch)}.";
    public string RenamingTool_ToolTip_AddonPattern => $"El patrón a utilizar para contenidos del tipo {nameof(ContentMetaType.AddOnContent)}.";
    public string RenamingTool_Button_Cancel => "Cancelar";
    public string RenamingTool_Button_Rename => "Renombrar";
    public string RenamingTool_GroupBoxInput => "Archivos origen";
    public string RenamingTool_GroupBoxNamingSettings => "Confirmación de nombrado";
    public string RenamingTool_BrowseDirTitle => "Seleccione una carpeta";
    public string RenamingTool_GroupBoxOutput => "Resultado";
    public string RenamingTool_Miscellaneous => "Misceláneos";
    public string RenamingTool_InvalidWindowsCharReplacement => "Carácter para reemplazar símbolos inválidos:";
    public string RenamingTool_ReplaceWhiteSpaceChars => "Reemplazar espacios en blanco";
    public string RenamingTool_ReplaceWhiteSpaceCharsWith => "Reemplazar espacios en blanco con:";
    public string RenamingTool_Simulation => "Simular";
    public string RenamingTool_AutoCloseOpenedFile => "Cerrar archivos abiertos";
    public string RenamingTool_IncludeSubDirectories => "Incluir subcarpetas";
    public string RenamingTool_ContentTypeNotSupported => "No se soporta contenido del tipo «{0}».";
    public string RenamingTool_SuperPackageNotSupported => "Los súper paquetes no son soportados.";
    public string RenamingTool_LogNbFilesToRename => ">>> faltan {0} archivo(s) para renombrar...";
    public string RenamingTool_LogSimulationMode => $"[SIMULACIÓN] ";
    public string RenamingTool_LogFileRenamed => $"• {{0}}Archivo renombrado{Environment.NewLine}\tde: «{{1}}» a: {Environment.NewLine}\t«{{2}}».";
    public string RenamingTool_LogFileAlreadyNamedProperly => "• {0}«{1}» Ya tenían el nombre seleccionado.";
    public string RenamingTool_LogFailedToRenameFile => "• {0}«{1}»Renombrado fallido: {2}";
    public string RenamingTool_LogRenamingFailed => "Renombrado fallido: {0}";
    public string RenamingTool_BadInvalidFileNameCharReplacement => "La cadena para renombrar «{0}» Contiene el carácter inválido «{1}».";

    public string Exception_UnexpectedDelimiter => "Delimitador inválido {0} se encontró en la posición {1}, cámbielo por {2}{0}.";
    public string Exception_EndDelimiterMissing => "No se encuentra el delimitador final {0}.";
    public string FileRenaming_PatternKeywordUnknown => "La llave «{0}» es desconocida, las llaves permitidas son: «{1}».";
    public string FileRenaming_EmptyPatternNotAllowed => "El patrón no puede estar vacío.";
    public string FileRenaming_PatternKeywordNotAllowed => "No se permite la llave «{0}» en patrones del tipo «{1}».";
    public string FileRenaming_StringOperatorUnknown => "No se reconoce el operador «{0}» los operadores permitidos son: «{1}».";
    public string FileRenaming_EmptyDirectoryNotAllowed => "La carpeta origen no puede estar vacía.";
    public string Window_Tip_Title => "Consejo";
}