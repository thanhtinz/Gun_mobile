#!/usr/bin/env python3
"""TCP smoke test for the standalone GunMobile server.

Starts (or connects to) Road port 4396, sends Login, expects LoginOk + ProfileData.
"""
from __future__ import annotations

import argparse
import os
import socket
import struct
import subprocess
import sys
import time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MAGIC = 0x7D01
MSG_LOGIN = 2
MSG_LOGIN_OK = 3
MSG_PROFILE = 21
ROAD_PORT = 4396


def encode_packet(msg_id: int, json: str) -> bytes:
    body = (json or "{}").encode("utf-8")
    payload = 4 + len(body)
    return struct.pack("<IHH", payload, MAGIC, msg_id) + body


def try_decode(buf: bytes) -> tuple[int, str, int] | None:
    if len(buf) < 8:
        return None
    payload = struct.unpack_from("<I", buf, 0)[0]
    if payload < 4 or payload > 1_000_000:
        raise ValueError("bad payload %s" % payload)
    total = 4 + payload
    if len(buf) < total:
        return None
    magic, msg_id = struct.unpack_from("<HH", buf, 4)
    if magic != MAGIC:
        raise ValueError("bad magic %s" % magic)
    body = buf[8:total].decode("utf-8") if payload > 4 else "{}"
    return msg_id, body, total


def recv_msgs(sock: socket.socket, timeout: float = 8.0) -> list[tuple[int, str]]:
    sock.settimeout(timeout)
    buf = b""
    out: list[tuple[int, str]] = []
    deadline = time.time() + timeout
    while time.time() < deadline:
        try:
            chunk = sock.recv(65536)
        except socket.timeout:
            break
        if not chunk:
            break
        buf += chunk
        while True:
            decoded = try_decode(buf)
            if decoded is None:
                break
            msg_id, body, consumed = decoded
            out.append((msg_id, body))
            buf = buf[consumed:]
        if any(mid in (MSG_LOGIN_OK, MSG_PROFILE) for mid, _ in out):
            if any(mid == MSG_LOGIN_OK for mid, _ in out) and any(
                mid == MSG_PROFILE for mid, _ in out
            ):
                break
    return out


def wait_port(host: str, port: int, timeout: float = 30.0) -> None:
    deadline = time.time() + timeout
    while time.time() < deadline:
        try:
            with socket.create_connection((host, port), timeout=1.0):
                return
        except OSError:
            time.sleep(0.25)
    raise SystemExit("server did not listen on %s:%s" % (host, port))


def start_server(pc_data: Path, data_dir: Path) -> subprocess.Popen:
    env = os.environ.copy()
    env["GUNMOBILE_PC_DATA"] = str(pc_data)
    env["GUNMOBILE_DATA"] = str(data_dir)
    proj = ROOT / "Server" / "GunMobile.Standalone"
    subprocess.check_call(
        ["dotnet", "build", "-c", "Release", "-v", "q"],
        cwd=proj,
        env=env,
    )
    dll = proj / "bin" / "Release" / "net8.0" / "GunMobileServer.dll"
    return subprocess.Popen(
        ["dotnet", str(dll), str(pc_data)],
        cwd=proj,
        env=env,
        stdout=subprocess.PIPE,
        stderr=subprocess.STDOUT,
        text=True,
    )


def run_login(host: str, port: int, nick: str) -> None:
    with socket.create_connection((host, port), timeout=5.0) as sock:
        sock.sendall(encode_packet(MSG_LOGIN, '{"nick":"%s"}' % nick.replace('"', "")))
        msgs = recv_msgs(sock)
    ids = [mid for mid, _ in msgs]
    print("received:", [(mid, body[:80]) for mid, body in msgs])
    if MSG_LOGIN_OK not in ids:
        raise SystemExit("missing LoginOk (3); got %s" % ids)
    if MSG_PROFILE not in ids:
        raise SystemExit("missing ProfileData (21); got %s" % ids)
    print("SMOKE OK login=%s msgs=%s" % (nick, ids))


def main() -> int:
    p = argparse.ArgumentParser()
    p.add_argument("--host", default="127.0.0.1")
    p.add_argument("--port", type=int, default=ROAD_PORT)
    p.add_argument("--nick", default="SmokeBot")
    p.add_argument("--external", action="store_true", help="do not start server")
    p.add_argument(
        "--pc-data",
        default=str(ROOT / "UnityClient" / "Assets" / "StreamingAssets" / "PcData"),
    )
    p.add_argument("--data-dir", default=str(ROOT / ".gunmobile-smoke-data"))
    args = p.parse_args()

    proc = None
    if not args.external:
        proc = start_server(Path(args.pc_data), Path(args.data_dir))
        try:
            wait_port(args.host, args.port, timeout=45.0)
            run_login(args.host, args.port, args.nick)
        finally:
            proc.terminate()
            try:
                proc.wait(timeout=8)
            except subprocess.TimeoutExpired:
                proc.kill()
            if proc.stdout:
                log = proc.stdout.read()
                if log:
                    print("--- server log (tail) ---")
                    print("\n".join(log.splitlines()[-40:]))
    else:
        run_login(args.host, args.port, args.nick)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
