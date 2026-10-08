# Firmware verification

Use File check to open a firmware ZIP/7z, a firmware folder or an individual NCA. Batch check detects firmware candidates in directories and mixed archives. Select a firmware result to view Firmware verification details; CSV export includes the report.

## Reference sources

Checks load current JSON references from [the repository's firmware hashes](https://github.com/sandmaennchen5/fork_NxFileViewer/tree/master/fw/hashes) into memory when firmware candidates are found or explicitly selected. Ordinary game folders make no reference request. Each firmware check fetches fresh data without a disk cache; Git blob hashes detect changes during download.

On network or validation failure, locally installed `fw/hashes/*.json` references provide an explicitly reported offline fallback. Local references are optional. If neither source is available, firmware candidates remain visible with an unavailable-reference error; game checks continue.

Development builds include references. Published application ZIPs exclude them: extract `NxFileViewer_v<version>_firmware-hashes.zip` beside the executable to install `fw/hashes/*.json`. The same add-on works with x64 and x86.

## Matching and integrity

NCA files are matched by size and SHA-256. A complete reference match is reported as Original. Incomplete sets show the best matching version, which does not prove completeness. Identical matching reference sets are listed together.

Missing, modified, additional or duplicate NCAs are errors. Non-NCA files are ignored. Archive members are matched by basename; folder sets are checked separately rather than merging files across directories. Repacking an archive does not change content hashes.

Missing filenames can be paired with additional NCAs only when both size and SHA-256 match. The report shows actual → expected names instead of counting the same file as both missing and extra. Each physical file is used once, and filename similarity alone is insufficient. Incorrect names remain an integrity error; verification never renames source files.

Individual firmware NCAs are identified by content and can list multiple matching firmware releases, including shared files and renamed files. Keys are not required for firmware hash checks.

## Nested and mixed archives

ZIP/7z containers can include game packages, an inner firmware archive and firmware folders. These appear as separate selector entries and batch rows. Recognized firmware folders group their NCA files into one set. Nested archives are processed up to eight levels; paths show chains such as `outer.zip::firmware.zip`.

The first extraction of an archive retains all contents in a shared session under `Temp/ZIP`. Nested archives are extracted when processed. Visited package files and loaded models are retained until the archive is closed or reopened; returning to them does not decompress them again. Batch types show origin, for example `NSP (ZIP)`.

Direct ZIP firmware hashing can read entries without extraction; navigating mixed/nested containers and opening 7z uses the shared extraction workflow. Allow sufficient temporary disk space. Closing or reopening releases retained resources. Firmware rows are excluded from game-package conversion, moving and naming actions.

## Reference actions under Settings → Updates

**Check online hashes** loads and validates references in memory and reports their count. It does not verify firmware files or save references.

**Update offline hashes** explicitly downloads and validates the complete reference set before replacing local `fw/hashes`. The previous folder remains as `fw/hashes-backup-<id>`. Download failures, validation errors and cancellation before publication retain the current set. Normal firmware checks still prefer fresh online references.

See [reference maintenance](../fw/README.md) for database generation.

## Tests

Synthetic fixtures cover complete and incomplete sets, renamed files, changed content, duplicates, archive subdirectories, mixed/nested firmware and game entries, unavailable references, cancellation and resource cleanup. Tests do not require real firmware archives or keys.

Settings → Updates shows the highest firmware version covered by valid locally installed hash manifests. Checking online references also reports the highest available version; a missing local reference directory is shown explicitly.
