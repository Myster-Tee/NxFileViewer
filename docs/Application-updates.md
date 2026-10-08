# Application updates and release packages

Open **Settings → Updates** to check for an application update or download and install an offered release. Start also links to this page. Automatic startup checks are optional and never install without confirmation. Network failures are logged and leave the viewer usable.

By default only stable releases are offered. Enable **Include pre-releases in application updates** and apply settings to include published previews. Drafts and older/equal semantic versions are ignored; changing the channel invalidates the current offer.

## Download, verification and replacement

Installation requires an idle viewer and a writable program directory. The matching x64/x86 release ZIP is staged under `Updates/<unique-id>` beside the executable. GitHub's SHA-256 asset digest, the full EXE product version (including preview suffix) and PE architecture are checked before installation. Missing digests are rejected.

The ZIP must contain `NxFileViewer.exe`, at its root or under a matching versioned application folder. Debug symbols are embedded in the executable; additional PDB files are not accepted. Unexpected files, unsafe paths and incompatible architectures are rejected; general multi-file application distributions and native ARM64 application releases are unsupported.

A hidden Windows PowerShell helper initializes before the viewer closes, allowing settings to save. It waits for process exit, rechecks the staged EXE hash, backs up and replaces the application, then restarts it. Keys, settings, Plugins, Cache, Temp, History, Logs and firmware references are retained. No AppData fallback or privilege elevation is used.

Replacement errors preserve the existing EXE. If the new process fails to launch or exits within two seconds, the helper attempts to restore the previous EXE. This checks early startup only. Staging data, the backup and `error.txt` remain under Updates for recovery or diagnosis.

## Publishing 4.0.0

The project uses the valid package version `4.0.0`. Run `Publish.ps1`, create a GitHub Release with tag `v4.0.0`, as a stable release (leave the pre-release option disabled). Attach:

- `NxFileViewer_v4.0.0_x64.zip`
- `NxFileViewer_v4.0.0_x86.zip`
- `NxFileViewer_v4.0.0_self-contained_x64.zip` (compressed, includes .NET)
- `NxFileViewer_v4.0.0_self-contained_x86.zip` (compressed, includes .NET)
- `NxFileViewer_v4.0.0_firmware-hashes.zip` (optional reference add-on)

Application ZIPs include the portable executable with embedded debug symbols; firmware references stay in their separate ZIP. GitHub Actions artifacts alone are not releases available to the updater. Architecture selection follows the running viewer process.

## Preview version ordering

The updater uses the installed assembly's informational version, including its beta/RC suffix, and compares it with the release tag using semantic precedence. Examples:

- 4.0.0-beta.2 is newer than 4.0.0-beta.1.
- 4.0.0-beta.10 is newer than 4.0.0-beta.2.
- 4.0.0-rc.1 follows 4.0.0-beta.10; stable 4.0.0 follows all 4.0.0 previews.
- Build metadata after + does not affect ordering.
- Equal or older versions are never offered.

Preview updates require Include pre-releases to be enabled. Stable-only mode can still offer the final release at the same numeric version to an installed beta. The downloaded EXE must match the full offered version, not just its numeric part.

Older Beta 1 builds still contain the numeric-only comparison and cannot discover Beta 2 automatically. Install Beta 2 manually once; subsequent beta suffix updates can be offered by the updated viewer.

## Validation

Tests use mocked releases/downloads, corrupt and unsafe archives, and isolated dummy EXEs for replacement and backup checks. They never replace an installed user application. A complete live GitHub upgrade/restart has not been validated during development.

## Runtime variants

Each x64/x86 release offers a standard ZIP requiring .NET 8 Desktop Runtime and a compressed self-contained ZIP with .NET included. Standard names remain NxFileViewer_v<VERSION>_<ARCH>.zip; bundled-runtime names are NxFileViewer_v<VERSION>_self-contained_<ARCH>.zip. Both contain only NxFileViewer.exe.

The executable records its runtime distribution at build time. Updates preserve this distribution and architecture, including preview releases. A missing compatible package is reported or skipped while listing previews; another runtime distribution is never substituted. Previously installed standard builds continue using the original ZIP names.

The Info tab displays the application version, process architecture and whether .NET is bundled. Standard builds explicitly show the .NET 8 Desktop Runtime requirement.
