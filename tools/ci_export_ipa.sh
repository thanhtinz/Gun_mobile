#!/usr/bin/env bash
# Archive a Unity-exported Xcode project into dist/GunMobile.ipa.
# Requires a signing identity already imported into the default keychain
# and a provisioning profile in ~/Library/MobileDevice/Provisioning Profiles.
set -euo pipefail

ROOT="${1:-.}"
OUT_DIR="${OUT_DIR:-dist}"
TEAM_ID="${IOS_TEAM_ID:-}"
METHOD="${IOS_EXPORT_METHOD:-ad-hoc}"
mkdir -p "$OUT_DIR"

PROJ=$(find "$ROOT" -name 'Unity-iPhone.xcodeproj' -print -quit)
if [ -z "$PROJ" ]; then
  echo "Unity-iPhone.xcodeproj not found under $ROOT" >&2
  find "$ROOT" -maxdepth 4 -type d >&2 || true
  exit 1
fi

SRC=$(cd "$(dirname "$PROJ")" && pwd)
echo "Using Xcode project at $SRC"

if [ -z "$TEAM_ID" ]; then
  TEAM_ID=$(security find-certificate -c "iPhone Distribution" -p 2>/dev/null | openssl x509 -noout -text 2>/dev/null | tr -d '\n' | sed -n 's/.*OU=\([A-Z0-9]\{10\}\).*/\1/p' | head -n 1 || true)
fi

PLIST="$SRC/ExportOptions.plist"
cat > "$PLIST" <<EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
  <key>method</key>
  <string>${METHOD}</string>
  <key>compileBitcode</key>
  <false/>
  <key>signingStyle</key>
  <string>manual</string>
  <key>stripSwiftSymbols</key>
  <true/>
  <key>teamID</key>
  <string>${TEAM_ID}</string>
</dict>
</plist>
EOF

ARCHIVE="$SRC/GunMobile.xcarchive"
xcodebuild \
  -project "$SRC/Unity-iPhone.xcodeproj" \
  -scheme Unity-iPhone \
  -configuration Release \
  -destination 'generic/platform=iOS' \
  -archivePath "$ARCHIVE" \
  DEVELOPMENT_TEAM="$TEAM_ID" \
  CODE_SIGN_STYLE=Manual \
  archive

EXPORT="$SRC/ipa"
rm -rf "$EXPORT"
mkdir -p "$EXPORT"
xcodebuild -exportArchive \
  -archivePath "$ARCHIVE" \
  -exportPath "$EXPORT" \
  -exportOptionsPlist "$PLIST"

IPA=$(find "$EXPORT" -name '*.ipa' | head -n 1)
if [ -z "$IPA" ]; then
  echo "xcodebuild did not produce an .ipa" >&2
  ls -la "$EXPORT" >&2
  exit 1
fi

cp "$IPA" "$OUT_DIR/GunMobile.ipa"
ls -lh "$OUT_DIR/GunMobile.ipa"
