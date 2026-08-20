using System;

namespace UnityEngine
{
    public static class Debug
    {
        public static void Log(object message) => Console.WriteLine(message);
        public static void LogWarning(object message) => Console.WriteLine("[WARN] " + message);
        public static void LogError(object message) => Console.Error.WriteLine("[ERROR] " + message);
    }
}
