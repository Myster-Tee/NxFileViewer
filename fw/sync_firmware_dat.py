#!/usr/bin/env python3
from __future__ import annotations

import hashlib
import json
import os
import re
import shutil
import subprocess
import tempfile
import urllib.request
import xml.etree.ElementTree as ET
import zipfile
import zlib
from pathlib import Path
from xml.dom import minidom

REPO = "THZoria/NX_Firmware"
ROOT = Path(__file__).resolve().parent
DAT_PATH = ROOT / "Nintendo Switch Firmware.dat"
INDEX_PATH = ROOT / "archive-index.json"
HASH_DIR = ROOT / "hashes"


def gh_api(path: str):
    cmd = ["gh", "api", path, "--paginate"]
    out = subprocess.check_output(cmd, text=True)
    decoder = json.JSONDecoder()
    pos = 0
    items = []

    while pos < len(out):
        while pos < len(out) and out[pos].isspace():
            pos += 1
        if pos >= len(out):
            break

        obj, pos = decoder.raw_decode(out, pos)
        if isinstance(obj, list):
            items.extend(obj)
        else:
            items.append(obj)

    return items


def load_index() -> dict:
    if not INDEX_PATH.exists():
        return {"source": f"https://github.com/{REPO}", "versions": {}}

    with INDEX_PATH.open("r", encoding="utf-8") as f:
        data = json.load(f)

    data.setdefault("source", f"https://github.com/{REPO}")
    data.setdefault("versions", {})
    return data


def load_dat_root() -> ET.Element:
    if DAT_PATH.exists():
        return ET.parse(DAT_PATH).getroot()

    root = ET.Element("datafile")
    header = ET.SubElement(root, "header")
    ET.SubElement(header, "name").text = "Nintendo Switch Firmware"
    ET.SubElement(header, "description").text = "Nintendo Switch Firmware Database"
    ET.SubElement(header, "author").text = "GitHub Actions"
    ET.SubElement(header, "homepage").text = "THZoria/NX_Firmware"
    ET.SubElement(header, "url").text = f"https://github.com/{REPO}"
    return root


def dat_game_map(root: ET.Element) -> dict[str, ET.Element]:
    result = {}
    prefix = "Nintendo Switch Firmware "
    for game in root.findall("game"):
        name = game.get("name", "")
        if name.startswith(prefix):
            result[name[len(prefix):]] = game
    return result


def release_tag(release: dict) -> str:
    return str(release.get("tag_name") or "").strip()


def release_display_name(release: dict) -> str:
    """
    Human-readable name for the DAT.

    Examples:
      Firmware 14.1.2 (Rebootless Update 3)
        -> 14.1.2 (Rebootless Update 3)

      Firmware 1.0.0 Pre-Release
        -> 1.0.0 Pre-Release
    """
    name = str(release.get("name") or "").strip()
    tag = release_tag(release)

    if name:
        name = re.sub(r"^\s*Firmware\s+", "", name, flags=re.IGNORECASE).strip()
        if name:
            return name

    return tag


def slugify_release(display_name: str, tag: str) -> str:
    # Keep dots in versions, make the rest filesystem-friendly.
    value = display_name.strip().lower()
    value = value.replace("–", "-").replace("—", "-")
    value = re.sub(r"\s*\((.*?)\)\s*", lambda m: "-" + m.group(1), value)
    value = re.sub(r"[^a-z0-9._-]+", "-", value)
    value = re.sub(r"-{2,}", "-", value).strip("-._")

    if not value:
        value = re.sub(r"[^a-z0-9._-]+", "-", tag.lower()).strip("-._")

    return value


def zip_asset(release: dict):
    assets = release.get("assets") or []
    zips = [a for a in assets if str(a.get("name", "")).lower().endswith(".zip")]
    if not zips:
        return None

    # Prefer the largest ZIP. Firmware archives are normally by far the largest
    # ZIP asset and this is safer for special/rebootless releases than matching
    # only the tag string.
    return max(zips, key=lambda a: int(a.get("size") or 0))


def download_asset(asset: dict, target: Path):
    """
    Download through GitHub's release-asset API when an asset id exists.
    This avoids receiving an HTML page instead of the binary ZIP for some
    special/pre-release assets.
    """
    asset_id = asset.get("id")

    if asset_id:
        with target.open("wb") as f:
            subprocess.run(
                [
                    "gh",
                    "api",
                    "-H",
                    "Accept: application/octet-stream",
                    f"repos/{REPO}/releases/assets/{asset_id}",
                ],
                stdout=f,
                check=True,
            )
        return

    url = asset.get("browser_download_url")
    if not url:
        raise RuntimeError("ZIP asset has no downloadable URL")

    req = urllib.request.Request(url, headers={"User-Agent": "firmware-dat-sync"})
    with urllib.request.urlopen(req) as response, target.open("wb") as f:
        shutil.copyfileobj(response, f)


def file_hashes(path: Path):
    md5 = hashlib.md5()
    sha1 = hashlib.sha1()
    sha256 = hashlib.sha256()
    crc = 0
    size = 0

    with path.open("rb") as f:
        for chunk in iter(lambda: f.read(1024 * 1024), b""):
            size += len(chunk)
            crc = zlib.crc32(chunk, crc)
            md5.update(chunk)
            sha1.update(chunk)
            sha256.update(chunk)

    return {
        "size": size,
        "crc": f"{crc & 0xffffffff:08x}",
        "md5": md5.hexdigest(),
        "sha1": sha1.hexdigest(),
        "sha256": sha256.hexdigest(),
    }


def write_pretty_xml(root: ET.Element):
    raw = ET.tostring(root, encoding="utf-8")
    pretty = minidom.parseString(raw).toprettyxml(indent="  ", encoding="UTF-8")
    lines = [line for line in pretty.decode("utf-8").splitlines() if line.strip()]
    DAT_PATH.write_text("\n".join(lines) + "\n", encoding="utf-8")


def roms_from_game(game: ET.Element) -> list[dict]:
    roms = []

    for rom in game.findall("rom"):
        roms.append(
            {
                "name": rom.get("name", ""),
                "size": int(rom.get("size", "0")),
                "crc": rom.get("crc", ""),
                "md5": rom.get("md5", ""),
                "sha1": rom.get("sha1", ""),
                "sha256": rom.get("sha256", ""),
            }
        )

    roms.sort(key=lambda r: r["name"])
    return roms


def write_hash_files(tag: str, display_name: str, roms: list[dict], metadata: dict):
    HASH_DIR.mkdir(parents=True, exist_ok=True)
    slug = slugify_release(display_name, tag)

    json_path = HASH_DIR / f"{slug}.json"
    sums_path = HASH_DIR / f"{slug}-SHA256SUMS.txt"

    payload = {
        "tag": tag,
        "name": display_name,
        "source": metadata.get("release_url"),
        "asset_name": metadata.get("asset_name"),
        "archive_sha256": metadata.get("zip_sha256"),
        "file_count": len(roms),
        "files": {
            item["name"]: {
                "size": item["size"],
                "crc32": item["crc"],
                "md5": item["md5"],
                "sha1": item["sha1"],
                "sha256": item["sha256"],
            }
            for item in roms
        },
    }

    json_path.write_text(
        json.dumps(payload, indent=2, sort_keys=True) + "\n",
        encoding="utf-8",
    )

    with sums_path.open("w", encoding="utf-8", newline="\n") as f:
        for item in roms:
            f.write(f'{item["sha256"]}  {item["name"]}\n')

    return slug


def ensure_existing_hash_files(index: dict, games: dict[str, ET.Element]) -> int:
    """
    Create missing per-release JSON/SHA256SUMS files from the existing DAT.
    No firmware ZIP is downloaded and no file is re-hashed.
    """
    rebuilt = 0

    for tag, meta in index["versions"].items():
        display = str(meta.get("display_name") or tag)
        game = games.get(display) or games.get(tag)
        if game is None:
            # Keep historical index data untouched, but report the mismatch.
            print(f"{tag}: warning: present in index but no matching DAT entry")
            continue

        slug = str(meta.get("slug") or slugify_release(display, tag))
        json_path = HASH_DIR / f"{slug}.json"
        sums_path = HASH_DIR / f"{slug}-SHA256SUMS.txt"

        if json_path.exists() and sums_path.exists():
            continue

        roms = roms_from_game(game)
        slug = write_hash_files(tag, display, roms, meta)
        meta["display_name"] = display
        meta["slug"] = slug
        rebuilt += 1
        print(f"{tag}: rebuilt missing hash files from DAT (no download/hash)")

    return rebuilt


def release_sort_key(release: dict):
    # Stable historical order. GitHub timestamps are ISO-8601 and sort directly.
    return str(release.get("published_at") or release.get("created_at") or "")


def main():
    ROOT.mkdir(parents=True, exist_ok=True)
    HASH_DIR.mkdir(parents=True, exist_ok=True)

    index = load_index()
    root = load_dat_root()
    games = dat_game_map(root)

    # First restore any missing JSON/SHA256SUMS files for versions that are
    # already archived. This does NOT redownload or rehash firmware.
    rebuilt = ensure_existing_hash_files(index, games)

    releases = gh_api(f"repos/{REPO}/releases?per_page=100")
    releases = [r for r in releases if not r.get("draft")]
    releases.sort(key=release_sort_key)

    added = 0
    skipped_invalid = 0

    for release in releases:
        tag = release_tag(release)
        if not tag:
            continue

        display = release_display_name(release)

        # Exact GitHub release tag is the permanent unique archive key.
        # This allows normal + rebootless + pre-release variants to coexist.
        if tag in index["versions"]:
            print(f"{tag}: already archived, skipping download/hash")
            continue

        asset = zip_asset(release)
        if not asset:
            print(f"{tag}: no ZIP asset, skipping")
            continue

        print(f"{tag}: downloading {asset.get('name')}")

        with tempfile.TemporaryDirectory(prefix="fwdat-") as tmp:
            tmpdir = Path(tmp)
            zip_path = tmpdir / "firmware.zip"
            extract_dir = tmpdir / "firmware"

            try:
                download_asset(asset, zip_path)
            except Exception as exc:
                print(f"{tag}: download failed: {exc}; skipping")
                skipped_invalid += 1
                continue

            if not zipfile.is_zipfile(zip_path):
                print(f"{tag}: downloaded asset is not a valid ZIP, skipping")
                skipped_invalid += 1
                continue

            zip_sha256 = hashlib.sha256()
            with zip_path.open("rb") as f:
                for chunk in iter(lambda: f.read(1024 * 1024), b""):
                    zip_sha256.update(chunk)
            zip_sha256_hex = zip_sha256.hexdigest()

            try:
                with zipfile.ZipFile(zip_path, "r") as zf:
                    bad_file = zf.testzip()
                    if bad_file is not None:
                        print(f"{tag}: corrupt ZIP, bad file: {bad_file}; skipping")
                        skipped_invalid += 1
                        continue
                    zf.extractall(extract_dir)
            except zipfile.BadZipFile:
                print(f"{tag}: invalid ZIP, skipping")
                skipped_invalid += 1
                continue

            files = sorted(p for p in extract_dir.rglob("*") if p.is_file())
            if not files:
                print(f"{tag}: ZIP was empty, skipping")
                skipped_invalid += 1
                continue

            # If a display name somehow collides with an existing release,
            # append the exact tag so no release overwrites another.
            dat_display = display
            if dat_display in games:
                dat_display = f"{display} [{tag}]"

            game = ET.SubElement(
                root,
                "game",
                {"name": f"Nintendo Switch Firmware {dat_display}"},
            )
            ET.SubElement(game, "description").text = (
                f"Nintendo Switch Firmware {dat_display}"
            )

            rom_records = []

            for path in files:
                rel = path.relative_to(extract_dir).as_posix()
                h = file_hashes(path)

                ET.SubElement(
                    game,
                    "rom",
                    {
                        "name": rel,
                        "size": str(h["size"]),
                        "crc": h["crc"],
                        "md5": h["md5"],
                        "sha1": h["sha1"],
                        "sha256": h["sha256"],
                    },
                )

                rom_records.append(
                    {
                        "name": rel,
                        "size": h["size"],
                        "crc": h["crc"],
                        "md5": h["md5"],
                        "sha1": h["sha1"],
                        "sha256": h["sha256"],
                    }
                )

            metadata = {
                "display_name": dat_display,
                "release_name": release.get("name"),
                "release_url": release.get("html_url"),
                "prerelease": bool(release.get("prerelease")),
                "asset_name": asset.get("name"),
                "asset_size": asset.get("size"),
                "zip_sha256": zip_sha256_hex,
                "file_count": len(files),
            }

            slug = write_hash_files(tag, dat_display, rom_records, metadata)
            metadata["slug"] = slug
            index["versions"][tag] = metadata

            games[dat_display] = game
            added += 1

            kind = []
            if release.get("prerelease"):
                kind.append("pre-release")
            if "rebootless" in str(release.get("name") or "").lower():
                kind.append("rebootless")
            kind_text = f" ({', '.join(kind)})" if kind else ""

            print(f"{tag}: archived {len(files)} files as {dat_display}{kind_text}")

    # Never delete historical entries that disappeared upstream.
    write_pretty_xml(root)
    INDEX_PATH.write_text(
        json.dumps(index, indent=2, sort_keys=True) + "\n",
        encoding="utf-8",
    )

    print(
        f"Done. New releases added: {added}; "
        f"hash sets rebuilt from DAT: {rebuilt}; "
        f"invalid/unavailable releases skipped: {skipped_invalid}"
    )


if __name__ == "__main__":
    main()
