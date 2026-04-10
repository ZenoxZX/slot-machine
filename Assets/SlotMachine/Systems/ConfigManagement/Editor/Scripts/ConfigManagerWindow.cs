using System.Collections.Generic;
using System.Linq;
using SlotMachine.Core;
using UnityEditor;
using UnityEngine;

namespace SlotMachine.ConfigManagement.Editor
{
    /// <summary>
    /// Editor window for managing all IVisibleConfig ScriptableObjects in the project.
    /// </summary>
    public class ConfigManagerWindow : EditorWindow
    {
        private const float k_LeftPanelWidth = 250f;
        private const float k_SeparatorWidth = 2f;
        private const float k_Padding = 15f;

        private List<ScriptableObject> m_AllConfigs;
        private ScriptableObject m_SelectedConfig;
        private UnityEditor.Editor m_CachedEditor;
        private Vector2 m_LeftScrollPos;
        private Vector2 m_RightScrollPos;

        [MenuItem("Tools/" + GlobalEnvironmentVariables.AppName + "/Configuration Manager")]
        public static void ShowWindow()
        {
            ConfigManagerWindow window = GetWindow<ConfigManagerWindow>("Config Manager");
            window.minSize = new(600, 400);
        }

        private void OnEnable()
        {
            FindAllConfigs();
        }

        private void OnDisable()
        {
            if (m_CachedEditor != null)
            {
                DestroyImmediate(m_CachedEditor);
            }
        }

        private void FindAllConfigs()
        {
            m_AllConfigs = AssetDatabase.FindAssets("t:ScriptableObject")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
                .Where(so => so is IVisibleConfig)
                .OrderBy(so => ((IVisibleConfig)so).Category)
                .ThenBy(so => ((IVisibleConfig)so).ConfigName)
                .ToList();

            if (m_AllConfigs.Count > 0 && m_SelectedConfig == null)
            {
                m_SelectedConfig = m_AllConfigs[0];
            }
        }

        private void OnGUI()
        {
            if (m_AllConfigs == null || m_AllConfigs.Count == 0)
            {
                EditorGUILayout.HelpBox("No configs found implementing IVisibleConfig.", MessageType.Info);
                if (GUILayout.Button("Refresh"))
                {
                    FindAllConfigs();
                }
                return;
            }

            EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
            {
                // Left Panel - Config List
                DrawLeftPanel();

                // Separator
                DrawSeparator();

                // Right Panel - Inspector
                DrawRightPanel();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawLeftPanel()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(k_LeftPanelWidth));
            {
                GUIStyle headerStyle = new(EditorStyles.boldLabel)
                {
                    fontSize = 14,
                    padding = new(5, 5, 10, 10)
                };
                EditorGUILayout.LabelField("Configurations", headerStyle);

                if (GUILayout.Button("Refresh", GUILayout.Height(25)))
                {
                    FindAllConfigs();
                }

                EditorGUILayout.Space(k_Padding);

                m_LeftScrollPos = EditorGUILayout.BeginScrollView(m_LeftScrollPos);
                {
                    string currentCategory = null;

                    foreach (ScriptableObject config in m_AllConfigs)
                    {
                        if (config is not IVisibleConfig visibleConfig)
                            continue;

                        // Draw category header
                        if (currentCategory != visibleConfig.Category)
                        {
                            currentCategory = visibleConfig.Category;
                            EditorGUILayout.Space(k_Padding);

                            GUIStyle categoryStyle = new(EditorStyles.boldLabel)
                            {
                                fontSize = 11,
                                normal = { textColor = new(0.7f, 0.7f, 0.7f) }
                            };

                            EditorGUILayout.LabelField(currentCategory.ToUpper(), categoryStyle);
                            EditorGUILayout.Space(3);
                        }

                        // Draw config button
                        bool isSelected = m_SelectedConfig == config;
                        GUIStyle buttonStyle;

                        if (isSelected)
                        {
                            buttonStyle = new(GUI.skin.button)
                            {
                                fontStyle = FontStyle.Bold,
                                normal =
                                {
                                    textColor = Color.white,
                                    background = MakeTex(2, 2, new(0.24f, 0.48f, 0.90f, 1f)) // Unity blue
                                }
                            };
                        }
                        else
                        {
                            buttonStyle = new(GUI.skin.button)
                            {
                                alignment = TextAnchor.MiddleLeft,
                                padding = new(10, 10, 5, 5)
                            };
                        }

                        if (GUILayout.Button(visibleConfig.ConfigName, buttonStyle, GUILayout.Height(28)))
                        {
                            SelectConfig(config);
                        }
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawSeparator()
        {
            Rect separatorRect = EditorGUILayout.GetControlRect(GUILayout.Width(k_SeparatorWidth), GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(separatorRect, new(0.13f, 0.13f, 0.13f));
        }

        private void DrawRightPanel()
        {
            EditorGUILayout.BeginVertical();
            {
                if (m_SelectedConfig != null)
                {
                    IVisibleConfig visibleConfig = (IVisibleConfig)m_SelectedConfig;

                    // Header
                    GUIStyle titleStyle = new(EditorStyles.largeLabel)
                    {
                        fontSize = 18,
                        fontStyle = FontStyle.Bold,
                        padding = new(10, 10, 10, 5)
                    };

                    EditorGUILayout.LabelField($"{visibleConfig.ConfigName}", titleStyle);
                    GUIStyle subtitleStyle = new(EditorStyles.label)
                    {
                        fontSize = 11,
                        padding = new(10, 10, 0, 10),
                        normal = { textColor = new(0.6f, 0.6f, 0.6f) }
                    };

                    EditorGUILayout.LabelField($"Category: {visibleConfig.Category}", subtitleStyle);
                    EditorGUILayout.Space(k_Padding);

                    // Inspector
                    m_RightScrollPos = EditorGUILayout.BeginScrollView(m_RightScrollPos);
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(10); // Left padding
                        EditorGUILayout.BeginVertical();
                        {
                            if (m_CachedEditor == null || m_CachedEditor.target != m_SelectedConfig)
                            {
                                if (m_CachedEditor != null)
                                {
                                    DestroyImmediate(m_CachedEditor);
                                }
                                m_CachedEditor = UnityEditor.Editor.CreateEditor(m_SelectedConfig);
                            }

                            if (m_CachedEditor != null)
                            {
                                // Increase label width for better readability
                                float oldLabelWidth = EditorGUIUtility.labelWidth;
                                EditorGUIUtility.labelWidth = 180;

                                m_CachedEditor.OnInspectorGUI();

                                EditorGUIUtility.labelWidth = oldLabelWidth;
                            }
                        }
                        EditorGUILayout.EndVertical();
                        GUILayout.Space(10); // Right padding
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();

                    // Footer buttons
                    EditorGUILayout.Space(k_Padding);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUIStyle buttonStyle = new(GUI.skin.button)
                        {
                            fontSize = 12,
                            fontStyle = FontStyle.Bold
                        };

                        if (GUILayout.Button("Ping in Project", buttonStyle, GUILayout.Height(32)))
                        {
                            EditorGUIUtility.PingObject(m_SelectedConfig);
                        }

                        GUILayout.Space(10);

                        if (GUILayout.Button("Select in Project", buttonStyle, GUILayout.Height(32)))
                        {
                            Selection.activeObject = m_SelectedConfig;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.HelpBox("Please select a configuration from the left panel.", MessageType.Info);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void SelectConfig(ScriptableObject config)
        {
            if (m_SelectedConfig == config)
                return;

            m_SelectedConfig = config;

            if (m_CachedEditor == null)
                return;

            DestroyImmediate(m_CachedEditor);
            m_CachedEditor = null;
        }

        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }

            Texture2D result = new(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
