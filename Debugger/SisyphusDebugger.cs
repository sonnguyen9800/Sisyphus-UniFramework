using UnityEngine;
using System.Collections;

namespace SisyphusLab.Debugger
{
    public static class SisyphusDebugger
    {
        // Constants for log levels and settings
        public const int MaxDebugLevel = 9;
        public static bool IsDebuggerActive = false;
        public static int DebugLevel = 0; // Range: 0 (most important) to 9 (least important)
        public static string ImportantGameObjectName = string.Empty;
        public static int DebugFlags = 0;

        // Log categories with colors
        public enum Category
        {
            IMPORTANT = 0,
            TRIGGERS,
            INGAMEUI,
            CAMERA,
            SAVEDATA,
            INPUT,
            INVENTORY,
            ANIMATION,
            PLAYER,
            EDITOR,
            GAME,
            UI,
            MONSTER,
            FOGOFWAR
        }

        private static readonly string[] Colors = {
            "FF0000",  // IMPORTANT - Red
            "FF9933",  // TRIGGERS - Orange
            "669999",  // INGAMEUI - Dull Green/Blue
            "52CC29",  // CAMERA - Green
            "99CCFF",  // SAVEDATA - Bright Blue
            "FFFF00",  // INPUT - Yellow
            "CC6699",  // INVENTORY - Dull Red/Purple
            "D96EBF",  // ANIMATION - Purple
            "66FFCC",  // PLAYER - Greenish
            "FFFFFF",  // EDITOR - White
            "666699",  // GAME - Dark Purple
            "AC5930",  // UI - Brown
            "BAC74A",  // MONSTER - Dirty Green
            "E38B81"   // FOGOFWAR - Washed Up Red
        };

        // Log methods
        public static void Log(object message) => Log((int)Category.IMPORTANT, message, 0, null);

        public static void Log(object message, GameObject context) => Log((int)Category.IMPORTANT, message, 0, context);

        public static void Log(int category, object message) => Log(category, message, 0, null);

        public static void Log(int category, object message, int level) => Log(category, message, level, null);

        public static void Log(int category, object message, int level, GameObject context)
        {
            if (!IsDebuggerActive)
            {
                Debug.Log($"{category}> {message}");
                return;
            }

            if (level > DebugLevel) return;

            string logMessage = message.ToString();
            string objInfo = GetObjectInfo(context, ref category);

            // Get color for the log entry
            string color = GetLogColor(category, context);

            // Ensure flags are set for this category
            if (!IsFlagSetForCategory(category)) return;

            // Format the log message with category, level, and context
            string formattedMessage = FormatLogMessage(category, logMessage, level, objInfo, color);

            // Output the formatted message
            Debug.Log(formattedMessage, context);
        }

        // Helper methods
        private static string GetObjectInfo(GameObject context, ref int category)
        {
            if (context == null) return string.Empty;

            string objName = context.transform.root.gameObject.name;
            if (objName == "_Fog of war")
                category = (int)Category.FOGOFWAR;

            return objName;
        }

        private static string GetLogColor(int category, GameObject context)
        {
            string color = Colors[category];
            if (context != null && (context.name == ImportantGameObjectName || context.transform.root.gameObject.name == ImportantGameObjectName))
            {
                color = Colors[(int)Category.IMPORTANT]; // Highlight important objects in red
            }
            return color;
        }

        private static bool IsFlagSetForCategory(int category)
        {
            int flagLayer = 1 << category;
            return (DebugFlags & flagLayer) != 0;
        }

        private static string FormatLogMessage(int category, string message, int level, string objInfo, string color)
        {
            string tabs = new string('\t', level);
            Category categoryName = (Category)category;

            return $"<color=#{color}>[{categoryName}] {tabs}-{level}- <b>{message}</b> <i>\t\t\t[{objInfo}]</i></color>";
        }
    }
}
