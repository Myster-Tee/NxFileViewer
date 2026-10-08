# Changelog

All notable changes to NxFileViewer will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [4.0.0] - 2026-10-07

### Added

- Add an optional NxNandManager plugin with verified managed downloads, manual updates and rollback, custom EXE/BIS-key settings, and a NAND workspace for dump information and partition export. It can reuse BIS keys from the viewer's active prod.keys; CLI work is cancellable, and staged exports preserve encryption and never overwrite existing files.
- Recognize NAND signatures and named encrypted-partition candidates in individual/batch mode and ZIP/7z archives, retaining split-dump siblings during extraction. Show NAND details and exclude these entries from NCA verification and game-package actions.
- Support ZIP/7z archives in individual and batch mode, including nested archives up to eight levels and mixed game/firmware contents. Individual mode offers an entry selector and retains extraction/content models until close or reopen. Batch types and CSV identify archive origin; archive members are excluded from conversion, move and rename actions.
- Verify firmware folders and archives by NCA size and SHA-256 without keys. Identify firmware versions and renamed/shared individual NCAs; report missing, changed, additional, duplicate and incorrectly named files without changing sources. Renamed files show actual → expected names instead of being counted as both missing and extra.
- Fetch current firmware references from GitHub for detected or explicitly selected firmware, with local offline fallback. Explicit offline updates retain the previous reference folder as a backup; ordinary game scans make no firmware-reference request.
- Add a central update check for NxFileViewer, installed managed plugins, regional/fallback TitleDB catalogs and firmware hash references. Forced TitleDB refresh validates catalogs before replacement and clears cached title responses; local refresh dates and local/online firmware versions are displayed.
- Add optional automatic startup checks and verified application download/install/restart, with SHA-256, full product-version and architecture checks, backup and early-start recovery. Preserve keys, settings and plugins. Published pre-releases require explicit opt-in; stable-only remains the default.
- Publish standard and compressed self-contained x64/x86 ZIPs, corresponding GitHub Actions artifacts and a separate optional firmware-hashes ZIP. Updates preserve architecture and runtime variant; Info displays both.
- Add descriptive file-dialog filters for game packages, ZIP/7z archives, NCA content and NAND images/split dumps.
- Support standalone NCA loading and signature/hash checks. Add package-structure detection for Scene releases, CDN rips, converted, Homebrew and incomplete packages, plus file size, reconstructed compression ratio and the system-update version in XCI/XCZ update partitions.
- Parse main.npdm and display permission levels, raw filesystem masks, authorized services and ACID signature state. Classifications consider filesystem service access and are shown for base titles and updates. Localized tooltips explain structure, permissions and separate NCA header signature results following Nx Game Info.
- Extend the title overview with base title ID, required master-key revision/key generation, minimum application version for DLC and NCA distribution type.
- Add selectable batch overview columns, sorting/reordering, combined search/type/integrity/naming filters and reset controls. CSV exports the filtered, sorted results with metadata and naming diagnostics; column choices persist during the current session.
- Save the last five batch sessions and resume interrupted work using source fingerprints; history can be disabled in Settings → Program. Historical game overviews show persisted metadata even when package/archive sources are unavailable.
- Add renaming and batch naming result tables, naming checks, proposed paths/status/error columns, an optional target directory and relative pattern subfolders without overwriting occupied targets. Individual checks show a status dialog; individual renaming requires Yes/Cancel confirmation.
- Add NSZ Automatic/Solid/Block modes and block sizes from 16 KiB to 4 GiB. Conversion verifies source and output, uses the viewer's active prod.keys, offers replacement/numbering/cancellation for existing output, and optionally deletes sources only after successful verification.
- Validate prod.keys and title.keys for malformed lines, missing master-key revisions, known master-key CRC32 values through master_key_16, and structurally valid Rights ID/title-key pairs. Estimate the newest supported content firmware and report revisions that cannot yet be validated or mapped.
- Display key validation separately for program-local, .switch and custom files, with active-file indicators and location buttons. Start shows prod.keys problems with Settings and Reload keys actions. Messages support English, German, French and Spanish.
- Add anonymous FTP support and editable Sphaira FTP presets to the HTTP/HTTPS key downloader, with a shared IP/hostname and {IP} URL templates. Downloads can target custom paths or the program folder and compare existing/incoming validation before replacement.
- Optionally save missing ticket keys to title.keys without duplicates or overwriting conflicts; disabled by default.
- Offer Tinfoil, regional GitHub TitleDB with offline cache, NLib API and custom title metadata sources, with independently selectable title websites.
- Write immediately flushed UTF-8 session logs with exception details, redacted key values and configurable retention of 1–100 launches (default 5).

### Changed

- Consolidate navigation into one window with Start, File check, Batch check, Renaming, NAND, Settings, Log and Info tabs. Retain results and drafts while switching pages; Settings Apply/Cancel no longer closes a separate window.
- Place NAND after Renaming and add a Start shortcut with a RAM-style icon. The second Start button row opens installed NxNandManager/NSZ GUI builds without downloading them. Start lists only available updates; Settings shows one complete update-check result.
- Organize Settings into Program, Naming settings, Updates and Plugins sections. Keep plugin/general drafts independent; installation/update/rollback actions are under Settings → Updates. Group the three key/ticket checkboxes beside the download controls.
- Restrict navigation to the active page and Log during background work, support cancelling unfinished single-file loads, and make Log follow new messages automatically. Start maximized on first launch while respecting saved placement.
- Combine batch scanning/verification and per-row actions; arrange filter checkboxes and action buttons consistently. Automatically select the appropriate Overview/Firmware/NAND details tab, while allowing manual tab selection. Add horizontal scrolling with Shift + mouse wheel and full-text cell/header tooltips.
- Move naming options to Settings → Naming settings, keep pattern/replacement sections visible, and remove the renaming output divider. Restore tinfoil.media presets with availability hints while preserving custom URLs.
- Copy active key files to .switch with replacement confirmation and validation comparisons, retaining source files. Remove empty program subdirectories on exit while preserving files, links and private test/development directories.
- Cache reconstructed compressed sizes and use queue-based tree traversal to avoid repeated scans and quadratic list shifting. Update README and supporting documentation for the final interface and release variants.

### Fixed

- Follow the current game package during active scans and select its completed result. Reuse captured overviews without reopening/decompressing completed packages; batch preview errors no longer invalidate otherwise valid results.
- Avoid classifying an NSP with only a ticket or only a certificate as converted.
- Rediscover and validate key locations on explicit reload, including newly added program-local files. Preserve existing key files on transfer failure, cancellation or declined replacement.
- Compare complete semantic versions, including beta/RC identifiers and numeric preview ordering; invalidate offers when the channel changes and verify the full downloaded product version.
- Accept Windows-style paths in single-executable application-update ZIPs.
- Retain shared archive extraction while entries are open and recognize firmware alongside games even when references are unavailable.
- Retain verified plugin downloads after startup failures, preserve NSZ GUI fallback runtime files and reuse successful compatibility checks.
- Use temporary ASCII input/key aliases for bundled NSZ executables, restore output names, detect missing output even after exit code 0 and log bounded sanitized diagnostics. Show conversion phases and CLI percentage/speed/ETA when available.
- Cache solid NCZ decoded prefixes to avoid full decompression restarts on backward/random reads (issue #52). Correct block NCZ boundaries, lookup, short reads, EOF, zero-length reads and disabled-cache handling.
- Report underlying NCA errors and identify broken NSZ/NCZ Zstandard streams as corrupted or incomplete compressed data.
- Prevent double release of main.npdm, bound malformed NPDM sizes and snapshot security data inside protected parsing. Use one-way bindings for read-only security fields to avoid UI crashes.
- Apply themes consistently to dialogs, title bars, tabs and status colors. Hide key warnings without a selected file; distinguish required-key errors from permitted missing delta fragments and unknown additional key names.
- Fall back to local NACP titles after online renaming failures, including HTTP 503.
- Refresh localized idle status after language changes (upstream #40) and tolerate malformed optional title ratings (upstream #18).

### Notes

- Firmware hash references are a separate optional download, not bundled into application ZIPs. Installed local references support offline verification. Repacked firmware archives remain valid when their NCA contents match.
- ZstdSharp.Port remains at 0.8.8; compressed NACP titles remain supported.
- The firmware estimate for prod.keys describes supported content firmware, not the exact firmware on which the keys were dumped. The master_key_16 mapping to 23.0.0 uses a locally derived CRC32 reference rather than independent upstream confirmation.
- title.keys supports structural validation; title-key values cannot be checked against a universal expected-value list.

## [3.0.3]

This release adds batch integrity checks, modernizes the interface and incorporates improvements from community forks.

### Batch integrity checks

- Verify complete folders containing NSP, NSZ, XCI and XCZ files, optionally including subdirectories.
- Display results during verification, including file type, package type, compression, integrity status, errors and full paths.
- Filter errors, preview the current file, report progress and support cancellation.
- Export results as CSV and remember the last batch directory.
- Move valid files to a selected destination while preserving subdirectories.
- Never overwrite existing targets or automatically delete/move invalid files.

### Interface and settings

- Add dark, light and system themes, including dark title bars for the main, settings, renaming and batch windows.
- Save window size and position and improve dark-mode batch presentation.
- Configure Tinfoil title-page and API URLs independently; migrate from tinfoil.media to tinfoil.io.

### NACP improvements

- Support compressed NACP title blocks and up to 32 languages instead of 16.
- Expand displayed NACP information.

### Renaming

- Fix GitHub issue #46.
- Restore missing spaces or separators in online titles and improve normalization of incorrectly combined filenames.

### Dependencies and build

- Update .NET 8-compatible dependencies, ZstdSharp.Port, Microsoft.Extensions packages, the test SDK, xUnit and Coverlet.
- Keep LibHac at 0.19.0 because a fully compatible direct replacement is unavailable.

Thanks to the original NxFileViewer project and all community contributors.
