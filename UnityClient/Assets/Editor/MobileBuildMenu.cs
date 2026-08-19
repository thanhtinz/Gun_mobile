using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GunMobile.EditorTools
{
    public static class MobileBuildMenu
    {
        const string AppId = "com.gunmobile.client";
        const string Product = "Gun Mobile";

        [MenuItem("GunMobile/Apply Android + iOS Player Settings")]
        public static void ApplyPlayerSettings()
        {
            PlayerSettings.companyName = "GunMobile";
            PlayerSettings.productName = Product;
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, AppId);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, AppId);
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;
            PlayerSettings.iOS.targetOSVersionString = "12.0";
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, true);
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, true);
            Debug.Log("GunMobile: Android/iOS player settings applied.");
        }

        [MenuItem("GunMobile/Build Android APK")]
        public static void BuildAndroid()
        {
            ApplyPlayerSettings();
            Directory.CreateDirectory("Builds");
            var opts = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Boot.unity" },
                locationPathName = "Builds/GunMobile.apk",
                target = BuildTarget.Android,
                options = BuildOptions.CompressWithLz4HC
            };
            BuildReport report = BuildPipeline.BuildPlayer(opts);
            Debug.Log("Android build: " + report.summary.result + " " + report.summary.outputPath);
        }

        [MenuItem("GunMobile/Build iOS Xcode Project")]
        public static void BuildIos()
        {
            ApplyPlayerSettings();
            Directory.CreateDirectory("Builds");
            var opts = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Boot.unity" },
                locationPathName = "Builds/ios",
                target = BuildTarget.iOS,
                options = BuildOptions.CompressWithLz4HC
            };
            BuildReport report = BuildPipeline.BuildPlayer(opts);
            Debug.Log("iOS build: " + report.summary.result + " " + report.summary.outputPath);
        }
    }
}
