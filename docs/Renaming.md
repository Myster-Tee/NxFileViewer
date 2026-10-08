# Naming patterns and batch naming

Patterns and character replacements are configured under **Settings → Naming settings**. The Renaming page links to this tab. Both sections stay expanded. Use the question-mark help for supported pattern keywords.

## Target folders and simulation

An optional target directory sets the destination root. Without it, each source file's current directory remains the root. Relative subdirectories can be part of a pattern:

- `DLC/{WTitle}.{Ext:L}`
- `{WAppTitle}/DLC/{WTitle}.{Ext:L}`

Only literal pattern separators create directories; separators in metadata are sanitized. Absolute paths, empty path components and traversal through `..` are rejected. Use the target-directory field for an absolute destination.

Simulation reports proposed changes without moving files or creating directories. Actual renaming creates missing folders and never overwrites occupied targets. Failure leaves the source intact and is reported in the result table and Log.

The table displays old path, proposed/new path, status and error. `QUELL::` marks paths relative to the source root; `ZIEL::` marks a different target root. The target uses `QUELL::` when both roots are the same. These are display labels only; actual filesystem paths are unchanged.

## Batch actions

**Check naming** compares eligible physical NSP/NSZ/XCI/XCZ rows with current saved patterns and the target directory. Naming status, proposed path and naming errors are separate from integrity status; additional columns are available through Columns, at the end of the table.

Right-click **Check naming** shows a result dialog with status and paths. Right-click **Rename** first previews those paths and requires **Yes** or **Cancel**. Closing or cancelling does not rename the file.

**Rename all differing files** checks every eligible row again with current settings and renames only differences. Firmware and archive members are excluded. Existing targets are never overwritten. Online title lookup falls back to local NACP names when available.

Hover over a table cell or header to see its full text. Apply/Cancel/Reset in settings manage the settings draft; editing a pattern does not rename files by itself.
