# NCZ reader comparison with NSZ 5.0.0 and master

Included in NxFileViewer 3.0.4.

Reviewed on 2026-10-05 against:

- https://github.com/nicoboss/nsz/releases/tag/5.0.0
- https://github.com/nicoboss/nsz/blob/master/nsz/BlockDecompressorReader.py
- https://github.com/nicoboss/nsz/blob/master/nsz/IndependentNczDecompressor.py
- https://github.com/nicoboss/nsz/blob/master/docs/formats.md

Scope: NCZ reading used by NxFileViewer, rather than importing the Python application.

## Applicable upstream changes

- NSZ #191 bounds compressed input to the stored block size. The C# reader previously handed the remaining NCZ source to Zstd, allowing buffered reads into later blocks. It now wraps each compressed block in a bounded stream, retains the shared source, and rejects oversized decoded blocks. No additional compressed-block-sized allocation is required.
- NSZ #211 fixes the final block when payload size is an exact multiple of block size. The existing BlocksBuilder already computes this correctly. Regression tests now cover both a full and a partial final compressed block.
- The documented format forbids stored sizes larger than decoded block sizes. BlocksBuilder now enforces this and verifies the block count matches payload size.

## Local improvements

- Block lookup uses its index directly rather than scanning every preceding block.
- Raw blocks tolerate short source reads. Zero-byte reads do not decompress blocks.
- MaxCacheSize=0 disables caching correctly instead of attempting to dequeue an empty queue.
- The earlier solid-reader change remains: lazy disk-backed prefix caching eliminates repeated decompression on backwards reads. First-time forward skips still require decoding the preceding solid prefix, and temporary storage under `Temp/NCZ` next to the executable can grow to the payload size. Cache files are deleted on close, and an unwritable program directory fails rather than falling back to AppData or the system temp directory.

Changed in this comparison: `src/LibHac.NSZ/Streams/NczBlockDecompressionStream.cs`, new `src/LibHac.NSZ.Test/BlockNczTest.cs`, and this document.

## Already covered or outside scope

ZstdSharp.Port remains 0.8.8. Compressed NACP titles remain supported. NSZ's Python AES backend replacement is not a C# dependency update: this project already uses LibHac's AES transform. Python GUI, Docker, CLI key lookup, compressor and output-container padding/partition-writing changes do not apply to this NCZ reader. No new master keys are embedded.

## Reproduction

```powershell
dotnet build src/NxFileViewer.sln --no-restore
dotnet test src/NxFileViewer.sln --no-restore
```

Synthetic fixtures require no game files or keys. Block tests cover mixed raw/compressed blocks, exact and partial final blocks, reads crossing block boundaries, backwards reads, cache disabled, short reads, truncated raw data, invalid block count and invalid stored size. The source position assertion proves that compressed reads stay inside their block. Existing solid-reader tests check forward reads, backwards reads without further compressed-source reads, shared-source repositioning, EOF and disposal.

Validation at the time of the original reader change: complete solution build succeeded with zero warnings and zero errors; all 28 tests passed (17 LibHac.NSZ and 11 NxFileViewer).

The solid cache plausibly improves the repeated-decoding bottleneck discussed for Issue #52. The block changes align reading with current upstream rules; they do not establish that the specific Until Then file now loads quickly. That file was unavailable for timing.
