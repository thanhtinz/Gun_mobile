# AGENTS.md

## Cursor Cloud specific instructions

This repo is a **弹弹堂 / DDTank ("Gun Mobile") port**: a Unity 6.3 LTS mobile client
(`UnityClient/`) plus a pure-Python asset/logic toolchain (`tools/`) that unpacks the
original PC dump and repacks a mobile subset into `UnityClient/Assets/StreamingAssets/PcData`.

### What can and cannot run in this VM

- **Python toolchain (in scope):** fully runnable here. Uses **only the Python standard
  library** — there is nothing to `pip install`. Python 3.12 is present.
- **Unity client build (out of scope here):** the Android/iOS/Linux-server builds require the
  Unity 6.3 LTS Editor (`6000.3.22f1`) + a Unity license and run via **GameCI in GitHub Actions**
  (`.github/workflows/build-mobile.yml`). There is no Unity Editor/GPU/license in this VM, so do
  **not** attempt to build or Play the Unity client locally. See `docs/GITHUB_BUILD.md` and
  `UnityClient/README.md`.

### Git LFS is required for the full test/pack path

The original PC dump lives in `legacy/releases/Ok/*.zip` as **Git LFS** objects (~3 GB total).
The startup update script runs `git lfs pull` to materialize them. Without the real zips:

- `tools/test_port_helpers.py` passes only **12/14** (the 2 `Checksums`/`swf_extract` tests need
  the real archives); with them it is **14/14**.
- The asset packers/unpackers (`unpack_pc_dump.py`, `pack_mobile_content.py`, `pack_*`,
  `bootstrap_pc_assets.py`) cannot regenerate content.

If a fresh pod shows those 2 failures, run `git lfs pull` manually — it is idempotent and fast on
a warm cache.

### Test / run commands

- Test (validation suite): `python3 tools/test_port_helpers.py`
- Core asset pipeline (unpack the real dump): `python3 tools/unpack_pc_dump.py`
  - Writes ~15k files (~570 MB) to `legacy/unpacked/`, which is **gitignored** — safe to run.

### Don't commit regenerated assets

`legacy/unpacked/` is gitignored. The packed `StreamingAssets/PcData` and `legacy/data/` trees are
already committed; avoid re-running packers into the working tree and committing the churn unless
that is the actual task.
