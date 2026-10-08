# Title metadata and website sources

Choose **Title name source** under **Settings → Program**. Presets are named choices; editable URL fields are shown where applicable.

- **Tinfoil** uses the built-in Tinfoil API template.
- **TitleDB (GitHub)** downloads the selected regional catalog (default DE.de) and looks up embedded title IDs, with US.en fallback. Valid catalogs are cached under `Cache/TitleDB` beside the executable and refreshed after 24 hours. Failed refreshes retain old catalogs and retry after five minutes when cached data exists.
- **NLib API** uses `https://api.nlib.cc/nx/{TitleId}?lang={Language}`. Language follows the interface/system language, with English fallback for unsupported languages. NLib's icon field is mapped to the title image.
- **Custom** allows editing a title API template, initially based on Tinfoil.

Applying settings changes subsequent requests. The per-title memory cache includes provider, URL, region and language, and expires after 24 hours. Optional numeric/string ratings are accepted; malformed optional ratings do not discard otherwise valid title information.

If online information is unavailable, renaming uses local NACP titles when present. DLC without either source uses the existing NO_TITLE placeholder. Unwritable cache storage does not prevent using downloaded titles in memory; no alternative storage location is selected.

## Title websites

The independent title-page selector offers **Tinfoil**, **NX Content** and **Custom**. Preset URLs are fixed; Custom shows an editable template initialized from Tinfoil. `{TitleId}` is replaced when opening the browser. Website selection does not change the metadata provider used for renaming.

## Manual refresh

Use **Settings → Updates → Refresh TitleDB** to download the saved region and US.en fallback even when existing catalogs are fresh. Valid catalogs replace cache files atomically; failures retain the previous catalog. Per-title responses are invalidated afterwards, including partial refresh success. The selected provider is unchanged.

## Sources and validation

Catalogs use [blawar/TitleDB](https://github.com/blawar/titledb); the NLib adapter targets [NLib API](https://github.com/ghost-land/Nlib-API).

Automated tests cover routing, language/icon mapping, embedded title-ID lookup, persistent/stale caches, malformed updates, fallback, settings changes, optional ratings and offline renaming. Tests use synthetic or mocked responses rather than requiring live services.

The Updates page shows when each local regional catalog was last refreshed, including the US.en fallback. This is the local cache timestamp, not the upstream catalog publication date. Missing catalogs are shown explicitly.

Tinfoil presets use https://tinfoil.media for title pages and the title API. Tinfoil can be temporarily unavailable or unstable; retry later or select another provider. Previous standard tinfoil.io presets migrate automatically; custom API URLs are preserved.
