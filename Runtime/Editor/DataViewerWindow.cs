#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SisyphusFramework.Data.Editor
{
    public class DataViewerWindow : EditorWindow
    {
        private int selectedTab;
        private string[] tabNames = Array.Empty<string>();
        private IVisualizer[] _visualizers = Array.Empty<IVisualizer>();

        private Vector2 scrollPos = Vector2.zero;
        private Vector2 horizontalTabScrollPos = Vector2.zero;
        private Vector2 horizontalContentScrollPos = Vector2.zero;

        [MenuItem("SisyphusFramework/DataViewer")]
        public static void ShowWindow()
        {
            GetWindow<DataViewerWindow>("DataViewer");
        }

        private void OnEnable()
        {
            FindDataHubs();
        }

        private void OnInspectorUpdate()
        {
            FindDataHubs();
            Repaint();
        }

        private void FindDataHubs()
        {
            _visualizers = VisualizerRegistry.GetAllVisualizers();
            tabNames = _visualizers
                .Select(hub => hub.GetType().Name)
                .Distinct()
                .ToArray();

            if (selectedTab >= tabNames.Length) selectedTab = 0;
        }


        // The variable to control where the scrollview 'looks' into its child elements.
        private Vector2 scrollPosition;
        
        private void OnGUI()
        {
            if (tabNames.Length > 0)
            {
                // Draw tabs outside the main scroll view
                DrawTabs();
                foreach (var hub in _visualizers)
                {
                    if (hub.GetType().Name == tabNames[selectedTab])
                    {
                        DrawHubData(hub: hub);
                        break;
                    }
                }

            }
            else
            {
                GUILayout.Label("No DataHub instances found.");
            }
        }

        private void DrawTabs()
        {
            EditorGUILayout.BeginHorizontal();

            // Back Button
            if (GUILayout.Button("<", GUILayout.Width(30))) selectedTab = Mathf.Max(selectedTab - 1, 0);

            // Horizontal scroll view for tabs
            horizontalTabScrollPos = EditorGUILayout.BeginScrollView(
                horizontalTabScrollPos,
                true,
                false,
                GUILayout.Height(40),
                GUILayout.Width(position.width - 60) // Adjust width for buttons
            );

            GUILayout.BeginHorizontal();

            for (var i = 0; i < tabNames.Length; i++)
            {
                var tabStyle = i == selectedTab ? EditorStyles.toolbarButton : EditorStyles.miniButton;

                if (GUILayout.Button(tabNames[i], tabStyle, GUILayout.Width(CalculateTabWidth(tabNames[i]))))
                    selectedTab = i;
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

            // Forward Button
            if (GUILayout.Button(">", GUILayout.Width(30)))
                selectedTab = Mathf.Min(selectedTab + 1, tabNames.Length - 1);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawHubData(IVisualizer hub)
        {
            // Outer vertical scroll view to handle vertical overflow
            scrollPos = EditorGUILayout.BeginScrollView(
                scrollPos,
                true,
                true,
                GUILayout.Width(position.width),
                GUILayout.Height(position.height - 50) // Adjust height to account for the tab height
            );
            EditorGUILayout.LabelField("Table name: " + tabNames[selectedTab], EditorStyles.boldLabel);

            hub.ShowData();

            GUILayout.Space(100);
            EditorGUILayout.EndScrollView();
        }


        private float CalculateTabWidth(string tabName)
        {
            // Dynamically calculate the width of each tab based on its content
            return EditorStyles.miniButton.CalcSize(new GUIContent(tabName)).x + 10; // Add padding
        }
    }
}
#endif