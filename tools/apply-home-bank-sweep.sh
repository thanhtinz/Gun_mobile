#!/bin/bash
set -euo pipefail
cd /workspace

# PhoneMsg 170-173
python3 << 'PY'
from pathlib import Path
p = Path("UnityClient/Packages/com.gunmobile.port/Runtime/Net/PhonePacket.cs")
t = p.read_text()
old = "        public const ushort SuperLuckerDraw = 169;\n        public const ushort RoomReady = 86;"
new = """        public const ushort SuperLuckerDraw = 169;
        public const ushort HomeTemplePractice = 170;
        public const ushort HomeTempleAdvance = 171;
        public const ushort BankDeposit = 172;
        public const ushort SweepMission = 173;
        public const ushort RoomReady = 86;"""
if old not in t:
    raise SystemExit("PhonePacket pattern missing")
p.write_text(t.replace(old, new, 1))
PY

# BankTermDeposit in PlayerExtras
python3 << 'PY'
from pathlib import Path
p = Path("UnityClient/Packages/com.gunmobile.port/Runtime/Net/PlayerExtras.cs")
t = p.read_text()
needle = "    [System.Serializable]\n    public sealed class AuctionListing"
insert = """    [System.Serializable]
    public sealed class BankTermDeposit { public int TemplateId; public int Amount; public int DepositDay; }

    [System.Serializable]
    public sealed class AuctionListing"""
if "BankTermDeposit" not in t:
    if needle not in t:
        raise SystemExit("PlayerExtras pattern missing")
    p.write_text(t.replace(needle, insert, 1))
PY

echo "Applied PhonePacket + PlayerExtras"
