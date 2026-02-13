using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class DisableErrorPause
{
    static DisableErrorPause()
    {
        // Disable Error Pause using the ConsoleWindow internal API
        System.Type logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor");
        if (logEntries != null)
        {
            var getConsoleFlags = logEntries.GetMethod("GetConsoleFlags",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var setConsoleFlags = logEntries.GetMethod("SetConsoleFlags",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            if (getConsoleFlags != null && setConsoleFlags != null)
            {
                int flags = (int)getConsoleFlags.Invoke(null, null);
                // Bit 2 (value 4) is Error Pause
                if ((flags & 4) != 0)
                {
                    flags &= ~4;
                    setConsoleFlags.Invoke(null, new object[] { flags });
                    Debug.Log("[DisableErrorPause] Error Pause has been disabled via LogEntries.");
                }
                else
                {
                    Debug.Log("[DisableErrorPause] Error Pause was already disabled.");
                }
            }
        }

        // Also hook into play mode to ensure it stays disabled
        EditorApplication.playModeStateChanged += (state) =>
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                var logEntriesType = System.Type.GetType("UnityEditor.LogEntries,UnityEditor");
                if (logEntriesType != null)
                {
                    var getFlags = logEntriesType.GetMethod("GetConsoleFlags",
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    var setFlags = logEntriesType.GetMethod("SetConsoleFlags",
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (getFlags != null && setFlags != null)
                    {
                        int f = (int)getFlags.Invoke(null, null);
                        if ((f & 4) != 0)
                        {
                            f &= ~4;
                            setFlags.Invoke(null, new object[] { f });
                        }
                    }
                }
            }
        };
    }
}
