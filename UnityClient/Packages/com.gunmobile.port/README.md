# Gun Mobile Port (Unity package)

Drop-in runtime helpers for the PC Flash dump.

- **Res:** zlib XML, Request tables, Starling atlas, `fore.map` collision, `config.xml`, `GameDatabase`, SWF JPEG extract (`SwfImage`)
- **UI:** landscape canvas, safe area, object pool, Morn → uGUI, touch aim/move
- **Logic:** PC 25fps artillery (`PcPhysics` gravity 0.7/frame), wind, turns, bomb table, damage
- **Net:** PhoneRoad TCP 4396/1910 (JSON, magic 0x7D01) — LAN / loopback, not SQL Road.Service

See `docs/UNITY_MOBILE_PORT.md` in the repo root.
