# Nintendo Switch Firmware DAT sync

This folder maintains one cumulative DAT for **all** firmware releases discovered in `THZoria/NX_Firmware`, including normal releases, rebootless updates and pre-releases.

## Verification in NxFileViewer

NxFileViewer copies `hashes/*.json` into development builds and distributes them in a separate optional firmware-hashes ZIP, excluded from x64/x86 application ZIPs. The viewer uses file sizes and SHA-256 to check firmware ZIPs or folders of loose NCAs. Open firmware through File check or Batch check; batch scans recognize firmware candidates automatically. Hash verification requires no keys. Direct ZIP checks can hash entries without extraction, while mixed/nested containers and 7z use shared temporary extraction. Missing, modified, extra and duplicate NCA entries prevent a successful result. Firmware checks fetch current references from this GitHub repository into memory without a disk cache. Pure game folders make no request. Locally installed add-on references remain an explicitly reported offline fallback; no firmware archives are downloaded by the viewer. See [verification details](../docs/Firmware-verification.md).

Extract `NxFileViewer_v<version>_firmware-hashes.zip` into the directory containing `NxFileViewer.exe`; it creates `fw/hashes/*.json` and works with either architecture. Online firmware checks do not require the add-on.

## Files

- `Nintendo Switch Firmware.dat` — one cumulative DAT containing every archived release; original filenames/paths are preserved exactly as found in each ZIP.
- `archive-index.json` — permanent archive index keyed by immutable GitHub release ID.
- `hashes/<tag>.json` — complete per-release file database with size, CRC32, MD5, SHA-1 and SHA-256.
- `hashes/<tag>-SHA256SUMS.txt` — simple SHA-256 list for all files in that release.
- `sync_firmware_dat.py` — sync/generation script used by GitHub Actions.

Examples of distinct releases that are all kept:

```text
Nintendo Switch Firmware 14.1.2
Nintendo Switch Firmware 14.1.2 (Rebootless Update 3)
Nintendo Switch Firmware 16.0.3
Nintendo Switch Firmware 16.0.3 (Rebootless Update)
Nintendo Switch Firmware 16.0.3 (Rebootless Update 2)
Nintendo Switch Firmware 1.0.0
Nintendo Switch Firmware 1.0.0 Pre-Release
```

The per-release filenames use the GitHub tag, so variants remain separate, for example:

```text
hashes/14.1.2.json
hashes/14.1.2-SHA256SUMS.txt
hashes/14.1.2r3.json
hashes/14.1.2r3-SHA256SUMS.txt
hashes/1.0.0pr.json
hashes/1.0.0pr-SHA256SUMS.txt
```

## Behaviour

- Fetches all non-draft releases currently available from `THZoria/NX_Firmware`.
- Includes normal firmware releases, rebootless updates and pre-releases.
- Each GitHub release is identified by its immutable release ID, so releases sharing the same base firmware version do not collide.
- If a release already exists in `archive-index.json`, its ZIP is **not downloaded and its files are not re-hashed**.
- New releases are downloaded once, extracted once and hashed once.
- The ZIP is downloaded through the GitHub release-asset API and validated before extraction.
- Invalid/unavailable ZIP assets are skipped with a warning instead of aborting the whole run, so successfully processed releases can still be committed.
- Each new release is added to the single cumulative DAT and gets its own JSON plus `SHA256SUMS.txt` file.
- If a per-release JSON/SHA256SUMS file is accidentally missing for an already archived release, it is rebuilt from the hashes already stored in the DAT — **without downloading or re-hashing the firmware**.
- Existing DAT/index/hash entries are never deleted merely because an upstream release later disappears.
- Existing archived releases are never silently replaced.
- File names and paths stay unchanged; no `.cnmt.nca` renaming is performed.
- The GitHub Action commits only when something under `fw/` actually changed.

## Repository layout

```text
fw/
├── Nintendo Switch Firmware.dat
├── archive-index.json
├── hashes/
│   ├── 1.0.0.json
│   ├── 1.0.0-SHA256SUMS.txt
│   ├── 1.0.0pr.json
│   ├── 1.0.0pr-SHA256SUMS.txt
│   ├── ...
│   ├── 22.5.0.json
│   └── 22.5.0-SHA256SUMS.txt
├── sync_firmware_dat.py
└── README.md

.github/
└── workflows/
    └── sync-firmware-dat.yml
```

GitHub discovers workflows only under the repository-root `.github/workflows/` directory. The copy under `fw/.github/workflows/` is a template/package copy. Copy it to:

```text
.github/workflows/sync-firmware-dat.yml
```

The workflow runs daily at `06:15 UTC` and can also be started manually.
