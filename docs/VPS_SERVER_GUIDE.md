# Chạy GunMobile Server trên VPS

## Yêu cầu
- VPS Linux (Ubuntu 20.04+ / Debian 11+), tối thiểu 1 vCPU + 512MB RAM
- Firewall mở port **4396** (Road) và **1910** (Fight)
- PC data đã unpack vào thư mục server

## Cách 1: Unity headless build (khuyến nghị)

### Build trên máy local hoặc CI
1. Mở `UnityClient/` bằng Unity 6.3 LTS
2. **File → Build Settings → Dedicated Server → Linux**
3. Build ra thư mục (ví dụ `build/LinuxServer/`)
4. Upload thư mục build lên VPS

Hoặc dùng GitHub Actions: push tag `v*` hoặc chạy workflow `Build APK and IPA` → artifact `GunMobileServer-Linux` sẽ có file tar.gz.

### Chạy trên VPS
```bash
# Giải nén
tar -xzf GunMobileServer-Linux.tar.gz
cd StandaloneLinux64

# Copy PC data (StreamingAssets) nếu chưa có
# cp -r /path/to/PcData ./GunMobileServer_Data/StreamingAssets/PcData

# Chạy headless
chmod +x GunMobileServer
./GunMobileServer -batchmode -nographics -logFile server.log &

# Xem log
tail -f server.log
```

Server sẽ tự khởi động `MobileGameServer` lắng nghe:
- **TCP 4396** (Road — hall/login/shop/quest/...)
- **TCP 1910** (Fight — battle relay)

Headless build **không** khởi động client `GameApp` (tránh trùng port). Log sẽ báo số map có `fore.map` collision.

### Systemd service (optional)
```ini
[Unit]
Description=GunMobile Dedicated Server
After=network.target

[Service]
Type=simple
User=gunmobile
WorkingDirectory=/opt/gunmobile
ExecStart=/opt/gunmobile/GunMobileServer -batchmode -nographics
Restart=always
RestartSec=5

[Install]
WantedBy=multi-user.target
```

```bash
sudo cp gunmobile.service /etc/systemd/system/
sudo systemctl daemon-reload
sudo systemctl enable --now gunmobile
```

## Cách 2: Standalone .NET console (không cần Unity)

Project: `Server/GunMobile.Standalone/` — chạy trực tiếp trên VPS với .NET 8.

### Yêu cầu
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- PC data tại `UnityClient/Assets/StreamingAssets/PcData` (hoặc path tùy chỉnh)

### Smoke test (TCP login)
```bash
python3 tools/smoke_standalone_server.py
# Kết nối server đang chạy:
python3 tools/smoke_standalone_server.py --external --host 127.0.0.1
```
Script gửi `Login` (PhoneMsg 2) tới port **4396**, kỳ vọng `LoginOk` + `ProfileData`.

### Chạy nhanh
```bash
bash tools/run_standalone_server.sh
```

Hoặc thủ công:
```bash
export GUNMOBILE_PC_DATA=/path/to/PcData
export GUNMOBILE_DATA=/var/lib/gunmobile
cd Server/GunMobile.Standalone
dotnet run -c Release -- /path/to/PcData
```

Server lắng nghe **TCP 4396** (Road) và **1910** (Fight). Player save: `$GUNMOBILE_DATA/server_players/*.json`

### Systemd (standalone)
```ini
[Unit]
Description=GunMobile .NET Server
After=network.target

[Service]
Type=simple
User=gunmobile
WorkingDirectory=/opt/gunmobile/Server/GunMobile.Standalone
Environment=GUNMOBILE_PC_DATA=/opt/gunmobile/PcData
Environment=GUNMOBILE_DATA=/var/lib/gunmobile
ExecStart=/usr/bin/dotnet /opt/gunmobile/Server/GunMobile.Standalone/bin/Release/net8.0/GunMobileServer.dll
Restart=always
RestartSec=5

[Install]
WantedBy=multi-user.target
```

## Client kết nối

Trên điện thoại:
1. Mở game → vào Room
2. Nhập **public IP** của VPS vào ô IP (ví dụ `103.xx.xx.xx`)
3. Bấm **Host** hoặc **Join** → game sẽ connect tới server VPS
4. Chọn map và chơi online!

## Lưu ý
- Player data lưu tại `~/.config/unity3d/GunMobile/GunMobile/server_players/*.json`
- Mỗi player được nhận diện theo **nick** (login)
- Server chưa có RSA/encryption như PC — chỉ dùng cho private server
- Nếu cần nhiều người chơi (>10), cân nhắc VPS 2+ vCPU

## Firewall
```bash
# UFW
sudo ufw allow 4396/tcp
sudo ufw allow 1910/tcp

# iptables
sudo iptables -A INPUT -p tcp --dport 4396 -j ACCEPT
sudo iptables -A INPUT -p tcp --dport 1910 -j ACCEPT
```
