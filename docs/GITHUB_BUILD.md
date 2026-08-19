# Build APK / IPA on GitHub

GitHub Actions dùng [GameCI](https://game.ci/docs/github/builder) + Unity **6.3 LTS** (`6000.3.22f1`).

## 1. License Unity (bắt buộc)

Unity không build CI nếu chưa kích hoạt license.

1. Cài [Unity Hub](https://unity.com/download), đăng nhập, **Preferences → Licenses → Add** (Personal miễn phí).
2. Copy file `.ulf`:
   - Windows: `C:\ProgramData\Unity\Unity_lic.ulf`
   - macOS: `/Library/Application Support/Unity/Unity_lic.ulf`
   - Linux: `~/.local/share/unity3d/Unity/Unity_lic.ulf`
3. Repo → **Settings → Secrets and variables → Actions** → New repository secret:

| Secret | Giá trị |
| --- | --- |
| `UNITY_LICENSE` | **Toàn bộ** nội dung file `.ulf` |
| `UNITY_EMAIL` | Email Unity |
| `UNITY_PASSWORD` | Mật khẩu Unity |

Tài khoản Pro/Plus: dùng `UNITY_SERIAL` + email + password (không cần `UNITY_LICENSE`).

## 2. Chạy build

**Actions → Build APK and IPA → Run workflow.**

Hoặc `git tag v1.0.0 && git push origin v1.0.0`.

Khi xong:

- **Artifacts** trên run: `GunMobile-Android-APK`, `GunMobile-iOS-Xcode` (và `GunMobile-iOS-IPA` nếu có cert Apple)
- **Releases** nếu bật *Attach APK/IPA to a GitHub Release*

APK cài được (ký keystore CI `UnityClient/ci/android-debug.keystore`, **không** dùng cho Google Play).

## 3. IPA (cần Apple Developer)

Unity trên Linux chỉ xuất **Xcode project** (`GunMobile-iOS-Xcode.tar.gz`). Để ra `.ipa` cài iPhone, thêm secrets:

| Secret | Giá trị |
| --- | --- |
| `IOS_CERTIFICATE_BASE64` | File `.p12` encode `base64` |
| `IOS_CERTIFICATE_PASSWORD` | Password của `.p12` |
| `IOS_PROVISIONING_PROFILE_BASE64` | File `.mobileprovision` encode `base64` |
| `IOS_TEAM_ID` | Team ID (10 ký tự) |
| `IOS_EXPORT_METHOD` | `ad-hoc` (mặc định), `development`, hoặc `app-store` |

Không có các secret này thì job IPA **bỏ qua**; vẫn có file Xcode để Archive trên Mac.

```bash
base64 -i Certificates.p12 | pbcopy    # macOS
base64 -w0 profile.mobileprovision
```

## 4. Thời gian / dung lượng

Lần đầu ~30–90 phút/job (kéo image Unity ~5GB + IL2CPP). APK khoảng **170MB+** vì StreamingAssets.

Repo private: GitHub trừ phút Actions. macOS (job IPA) trừ đắt hơn Ubuntu.
