# Gun Mobile — PC dump

Original DDTank / 弹弹堂 client + server archives from GitHub Release **Ok**.

| File | SHA-256 |
| --- | --- |
| Archive.zip | `41d3bee0e4330ba3f7af26bdac9c4a82c6e7d0fe23f9a89f90a3f791f57092ba` |
| Archive.2.zip | `83764cf7c01b93c3f3371abbce1bea3064a5518f2437b3d1f4a6da0090e922e3` |
| Archive.3.zip | `fe6f77910190c22c3e40ef7892a3df7153cda46690007e47dc4181fe7dc083c9` |

Re-download:

```bash
tools/fetch_ok_release.sh
```

Unpack configs used by the Unity helpers:

```bash
python3 tools/extract_legacy.py
```

`Archive.zip` holds SQL backups and GM tools. `Archive.2.zip` holds Flash + NewPanel. `Archive.3.zip` holds Request XML, Road/Fight/Center services, and `Resource/image`.
