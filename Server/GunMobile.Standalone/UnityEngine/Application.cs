using System;
using System.IO;

namespace UnityEngine
{
    public static class Application
    {
        public static bool isBatchMode => true;

        public static string streamingAssetsPath =>
            Environment.GetEnvironmentVariable("GUNMOBILE_PC_DATA")
            ?? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "PcData"));

        public static string persistentDataPath =>
            Environment.GetEnvironmentVariable("GUNMOBILE_DATA")
            ?? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "data"));
    }
}
