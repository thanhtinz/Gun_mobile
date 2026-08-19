using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GunMobile.EditorTools
{
    /// <summary>
    /// iOS 14+ blocks LAN sockets until the local-network usage string exists.
    /// Patches the generated Info.plist without UnityEditor.iOS.Xcode.
    /// </summary>
    public sealed class IosLanPlist : IPostprocessBuildWithReport
    {
        public int callbackOrder => 40;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.iOS)
            {
                return;
            }

            string plist = Path.Combine(report.summary.outputPath, "Info.plist");
            if (!File.Exists(plist))
            {
                return;
            }

            string text = File.ReadAllText(plist);
            if (text.Contains("NSLocalNetworkUsageDescription"))
            {
                return;
            }

            const string keys =
                "  <key>NSLocalNetworkUsageDescription</key>\n" +
                "  <string>LAN Road/Fight with another phone on ports 4396 and 1910.</string>\n" +
                "  <key>NSAppTransportSecurity</key>\n" +
                "  <dict>\n" +
                "    <key>NSAllowsArbitraryLoads</key>\n" +
                "    <true/>\n" +
                "  </dict>\n";
            int insert = text.LastIndexOf("</dict>");
            if (insert < 0)
            {
                Debug.LogWarning("GunMobile: iOS Info.plist has no root dict");
                return;
            }

            File.WriteAllText(plist, text.Insert(insert, keys));
            Debug.Log("GunMobile: wrote iOS local-network plist keys");
        }
    }
}
