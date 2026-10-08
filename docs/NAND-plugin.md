# NxNandManager plugin

The **NAND** workspace uses [THZoria/NxNandManager](https://github.com/THZoria/NxNandManager) as a separate CLI process. It reads NAND dump information and exports a selected partition from local files or ZIP/7z members. Physical drives, restore, resizing, AutoRCM changes and decryption are not exposed.

## Setup

Under **Settings → Updates → Plugins → NxNandManager**, choose **Install / update…**. The latest stable release is downloaded from THZoria/NxNandManager, checked against GitHub's SHA-256 digest, extracted with its bundled DLLs and checked for CLI compatibility before activation. Installations live in `Plugins/NxNandManager` beside NxFileViewer. The application directory must be writable. The previous version remains available through **Use previous version**; failed updates preserve the active version. Updates are requested manually; opening a dump also installs a missing managed plugin.

Leave the EXE path empty under **Settings → Plugins** to use managed installations. A custom EXE path overrides them and disables managed update/rollback buttons; custom executables are never replaced. Clear the path and apply settings to return to managed installations.

The upstream EXE requests elevation and can fail with a host-dependent C++ locale error. File-only CLI processes receive `RunAsInvoker` and `LC_ALL/LANG=C` in their own environment, leaving Windows settings unchanged. The CLI usage probe accepts upstream's documented usage exit `-1001` and checks required options. Unsupported architectures or startup failures prevent activation. No Dokan driver is installed.

By default, the plugin uses NxFileViewer's active `prod.keys` file if it contains valid-format `bis_key_00`–`bis_key_03` entries. The viewer's normal key discovery and selected-file changes apply automatically. If no BIS entries are available, basic NAND information remains available without a keyset. BIS keys must belong to the source console; ordinary game keys alone cannot decrypt its NAND.

Optionally select a separate BIS key file to override the active `prod.keys`. NxNandManager accepts Lockpick-style `bis_key_00` entries or biskeydump format. Keys are passed by file path for information requests; values are not logged. BIS keys are required for extended information such as firmware and serial number, but not for copying encrypted partitions unchanged.

## Usage

The general **File check** and **Batch check** recognize NAND dumps alongside game packages. ZIP, solid 7z and nested archives can contain both. Header probes identify Switch GPT images by signature and Nintendo partition names, plus known BOOT/PRODINFO signatures. Generic PC GPT images are excluded. Exact conventional names such as `SYSTEM`, `USER`, `BOOT0.bin` or `PRODINFO.enc` with block-aligned data are shown as **candidates** when encryption hides signatures; use **NAND information** to confirm through NxNandManager. Basic recognition does not install the plugin or access firmware/title servers.

The individual file view has a NAND details page with **NAND information** and partition export. The archive entry selector switches between packages and NAND files. The dedicated NAND workspace's Open command also accepts ZIP and 7z and opens their entries in File check. Candidate files require a successful CLI information request before partition export is enabled.

For split dumps, select the first file (`rawnand.bin.00`, `full.00.bin`, `00`, etc.). Batch results list the first part once. Extraction preserves sibling files in the same temporary directory, including continuation parts, so NxNandManager receives the complete available sequence. Different archive folders remain separate. Changing a split companion invalidates saved batch fingerprints.

Batch NAND results show type, byte size and detection details, retained in history and CSV. They remain **Unchecked**: NAND detection is not NCA integrity verification. Package conversion, naming, moving and NCA verification actions exclude these rows. Open a row in File check for CLI information and export. Archive contents are extracted under the existing `Temp/ZIP` session and released on close. Allow disk space for the uncompressed archive, especially large NAND dumps. Listing probes bounded headers; selecting members reuses the existing extracted session.

Open a dump in **NAND**; for split dumps, select the first file. The page displays upstream CLI information. The partition selector recognizes storage types and partition rows in the documented English CLI output. Unknown output formats remain visible but may have no selectable partitions. Information requests time out after 60 seconds and can be cancelled.

Select a partition and export to a **new local file**. The plugin copies bytes as stored, including encryption. Existing files are never replaced, even if another process creates the destination during export. Export writes to a unique `.nxfv-nand-…` directory beside the destination. Only a successful CLI exit and a non-empty file allow publication; NxNandManager's default MD5 verification remains enabled. NxFileViewer does not perform an independent NAND integrity check.

The main background task runner displays progress percentages when reported by the CLI. Cancel terminates the process tree. Failed or cancelled exports discard temporary files; temporary directories can remain if Windows denies cleanup. Sources are never deleted. Information text can contain console identifiers and stays on the NAND page rather than being copied to session logs.

Auf der Startseite öffnet die zweite Button-Reihe installierte Plugin-GUIs in einem eigenen Fenster. NxNandManager wird mit --gui gestartet. NSZ ist nur mit einer installierten nsz-gui-Version verfügbar; CLI-Versionen bleiben deaktiviert. Der GUI-Start lädt keine Plugins herunter. Windows verwendet die vom Plugin angeforderten Berechtigungen.
