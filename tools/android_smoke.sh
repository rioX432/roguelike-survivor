#!/usr/bin/env bash
set -euo pipefail

# GlitchClaw — Android Smoke Test Script
# Usage: tools/android_smoke.sh
# Called by OpenClaw agents after builds to verify on-device behavior.

# --- Unity path detection ---
# Priority: $UNITY_PATH env > Unity Hub default locations > PATH
if [ -n "${UNITY_PATH:-}" ]; then
    UNITY="$UNITY_PATH"
elif [ -x "/Applications/Unity/Hub/Editor/6000.3.9f1/Unity.app/Contents/MacOS/Unity" ]; then
    UNITY="/Applications/Unity/Hub/Editor/6000.3.9f1/Unity.app/Contents/MacOS/Unity"
elif command -v unity-editor &>/dev/null; then
    UNITY="$(command -v unity-editor)"
else
    # Fallback: search common workspace locations
    for candidate in "$HOME/workspace"/6000.*/Unity.app/Contents/MacOS/Unity; do
        if [ -x "$candidate" ]; then UNITY="$candidate"; break; fi
    done
fi

if [ -z "${UNITY:-}" ] || [ ! -x "$UNITY" ]; then
    echo "FAIL: Unity editor not found."
    echo "Set UNITY_PATH env or install via Unity Hub."
    exit 1
fi

PROJECT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
GAME_DIR="$PROJECT_DIR/roguelike-survivor-game"
BUILD_DIR="$PROJECT_DIR/build/android"
APK_PATH="$BUILD_DIR/GlitchClaw.apk"
PKG="com.glitchclaw.survivor"
LOG_DIR="$BUILD_DIR/logs"

mkdir -p "$BUILD_DIR" "$LOG_DIR"

echo "============================================"
echo "  GlitchClaw Android Smoke Test"
echo "============================================"

# Step 1: Check device
echo ""
echo "[1/6] Checking connected device..."
if ! adb devices | grep -q "device$"; then
    echo "FAIL: No Android device connected"
    echo "Connect a device via USB or ensure adb is configured"
    exit 1
fi
DEVICE=$(adb devices | grep "device$" | head -1 | awk '{print $1}')
echo "OK: Device $DEVICE"

# Step 2: Unity Android build (batchmode)
echo ""
echo "[2/6] Building Android APK (batchmode)..."
"$UNITY" -batchmode -nographics -quit \
    -projectPath "$GAME_DIR" \
    -executeMethod BuildScript.BuildAndroid \
    -logFile "$LOG_DIR/unity_build.log" \
    2>&1 || {
    echo "FAIL: Unity build failed"
    echo "See: $LOG_DIR/unity_build.log"
    tail -20 "$LOG_DIR/unity_build.log" 2>/dev/null
    exit 1
}

if [ ! -f "$APK_PATH" ]; then
    echo "FAIL: APK not found at $APK_PATH"
    exit 1
fi
echo "OK: APK built at $APK_PATH"

# Step 3: Install APK
echo ""
echo "[3/6] Installing APK..."
adb install -r "$APK_PATH" 2>&1 || {
    echo "FAIL: APK install failed"
    exit 1
}
echo "OK: APK installed"

# Step 4: Launch app
echo ""
echo "[4/6] Launching app..."
adb shell am start -n "$PKG/com.unity3d.player.UnityPlayerGameActivity" 2>&1 || {
    echo "WARN: Launch failed, trying alternative activity..."
    adb shell monkey -p "$PKG" -c android.intent.category.LAUNCHER 1 2>&1
}
echo "OK: App launched"

# Step 5: Wait and collect logs
echo ""
echo "[5/6] Waiting 30 seconds for smoke test..."
sleep 30

echo "Collecting logcat..."
adb logcat -d -s Unity ActivityManager DEBUG | tail -100 > "$LOG_DIR/logcat.txt"

# Step 6: Take screenshot
echo ""
echo "[6/6] Taking screenshot..."
adb exec-out screencap -p > "$LOG_DIR/screenshot.png" 2>/dev/null || echo "WARN: Screenshot failed"

# Check for crashes
echo ""
echo "============================================"
if grep -qi "FATAL\|crash\|ANR\|SIGABRT\|SIGSEGV" "$LOG_DIR/logcat.txt"; then
    echo "  RESULT: FAIL (crash detected)"
    echo "  See: $LOG_DIR/logcat.txt"
    grep -i "FATAL\|crash\|ANR\|SIGABRT\|SIGSEGV" "$LOG_DIR/logcat.txt" | head -5
    exit 1
else
    echo "  RESULT: PASS"
    echo "  APK: $APK_PATH"
    echo "  Logs: $LOG_DIR/logcat.txt"
    echo "  Screenshot: $LOG_DIR/screenshot.png"
fi
echo "============================================"
