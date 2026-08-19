using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, AppId);
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, AppId);
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel35;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.forceInternetPermission = true;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;
            PlayerSettings.iOS.targetOSVersionString = "13.0";
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, true);
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, true);
            Debug.Log("GunMobile: Android/iOS player settings applied.");
        }

        [MenuItem("GunMobile/Unpack Full PC Dump")]
        public static void UnpackDump()
        {
            RunRepoPython("tools/unpack_pc_dump.py");
        }

        [MenuItem("GunMobile/Pack StreamingAssets PcData")]
        public static void PackStreaming()
        {
            RunRepoPython("tools/pack_mobile_content.py");
            AssetDatabase.Refresh();
        }

        [MenuItem("GunMobile/Extract SWF living/bomb to PNG")]
        public static void PackSwf()
        {
            RunRepoPython("tools/pack_swf_sprites.py");
            AssetDatabase.Refresh();
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
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException("Android build: " + report.summary.result);
            }
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
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException("iOS build: " + report.summary.result);
            }
        }

        static void RunRepoPython(string relativeScript)
        {
            string repo = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
            string script = Path.Combine(repo, relativeScript);
            var psi = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = "\"" + script + "\"",
                WorkingDirectory = repo,
                UseShellExecute = false
            };
            using (var p = Process.Start(psi))
            {
                p.WaitForExit();
                Debug.Log("GunMobile python " + relativeScript + " exit " + p.ExitCode);
            }
        }
    }

    public sealed class EnsurePcDataOnBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            MobileBuildMenu.ApplyPlayerSettings();
            string pc = Path.Combine(Application.dataPath, "StreamingAssets", "PcData", "content_index.json");
            if (!File.Exists(pc))
            {
                throw new BuildFailedException("Missing StreamingAssets/PcData. Run GunMobile → Pack StreamingAssets PcData.");
            }
        }
    }
}
