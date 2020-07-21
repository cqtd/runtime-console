using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace RuntimeConsole
{
    namespace Editor
    {
        using Context;

        [CustomEditor(typeof(RuntimeConsole.Console))]
        internal class RuntimeEditor : UnityEditor.Editor
        {
            // @TODO
            // 1. custom selector to add whitelist easily.
            // 2. save configuration of foldable fields.

            SerializedProperty m_inputField;
            SerializedProperty m_cellPrefab;
            SerializedProperty m_root;
            SerializedProperty m_canvas;


            SerializedProperty m_assemblyFilter;
            SerializedProperty m_assemblyWhitelist;
            SerializedProperty m_assemblyBlacklist;

            SerializedProperty m_namespaceFilter;
            SerializedProperty m_namespaceWhitelist;
            SerializedProperty m_namespaceBlacklist;

            void OnEnable()
            {
                m_inputField = serializedObject.FindProperty("inputField");
                m_cellPrefab = serializedObject.FindProperty("cellPrefab");
                m_root = serializedObject.FindProperty("root");
                m_canvas = serializedObject.FindProperty("canvas");

                m_assemblyFilter = serializedObject.FindProperty("assemblyFilterType");
                m_assemblyWhitelist = serializedObject.FindProperty("m_assemblyWhitelist");
                m_assemblyBlacklist = serializedObject.FindProperty("m_assemblyBlacklist");

                m_namespaceFilter = serializedObject.FindProperty("namespaceFilterType");
                m_namespaceWhitelist = serializedObject.FindProperty("m_namespaceWhitelist");
                m_namespaceBlacklist = serializedObject.FindProperty("m_namespaceBlacklist");
            }

            public override void OnInspectorGUI()
            {

                serializedObject.Update();

                // todo
                // validate components state

                if (IsValidComponents())
                {
                    EditorGUILayout.Space(10);
                    EditorGUILayout.LabelField(Editor.initialize_alert);
                    EditorGUILayout.Space(10);
                }
                else
                {
                    if (GUILayout.Button(Editor.find_button_text, new[] {GUILayout.Height(30)}))
                    {
                        (target as Console)?.FindComponents();
                        return;
                    }
                }

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField(Editor.label_assembly, EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.PropertyField(m_assemblyFilter);
                switch ((Console.EFilterType) m_assemblyFilter.enumValueIndex)
                {
                    case Console.EFilterType.WhiteList:
                        EditorGUILayout.PropertyField(m_assemblyWhitelist);
                        break;
                    case Console.EFilterType.BlackList:
                        EditorGUILayout.PropertyField(m_assemblyBlacklist);
                        break;
                    default: break;
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.Space(10);
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField(Editor.label_namespace, EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.PropertyField(m_namespaceFilter);
                switch ((Console.EFilterType) m_namespaceFilter.enumValueIndex)
                {
                    case Console.EFilterType.WhiteList:
                        EditorGUILayout.PropertyField(m_namespaceWhitelist);
                        break;
                    case Console.EFilterType.BlackList:
                        EditorGUILayout.PropertyField(m_namespaceBlacklist);
                        break;
                    default: break;
                }

                EditorGUILayout.EndVertical();

                serializedObject.ApplyModifiedProperties();
            }

            bool IsValidComponents()
            {
                if (m_inputField.objectReferenceValue as UnityEngine.UI.InputField == null)
                    return false;

                if (m_cellPrefab.objectReferenceValue as UnityEngine.UI.Text == null)
                    return false;

                if (m_root.objectReferenceValue as UnityEngine.RectTransform == null)
                    return false;

                if (m_canvas.objectReferenceValue as UnityEngine.Canvas == null)
                    return false;

                return true;
            }
            
            #region SCENE

            const string subsceneName = "ConsoleSubscene";
            const string subscenePath = "Packages/Console/Scenes/" + subsceneName + ".unity";

            static bool loaded = false;
		
#if UNITY_EDITOR
            [UnityEditor.MenuItem(Editor.menu_item_0)]
#endif
            public static void LoadConsole()
            {
                if (loaded)
                {
                    Debug.LogWarning(Editor.load_alert);
                    return;
                }
			
#if UNITY_EDITOR
                if (!HasRegistered())
                {
                    bool add = UnityEditor.EditorUtility.DisplayDialog(Editor.add_caption, Editor.add_msg, Editor.add_yes, Editor.add_no);
                    if (add)
                    {
                        RegisterSceneToBuildSetting();
                    }

                    UnityEditor.EditorUtility.DisplayDialog(Editor.load_caption,
                        add ? Editor.load_msg_0 : Editor.load_msg_1, Editor.load_msg_2);

                    return;
                }
#endif

                SceneManager.LoadScene(subsceneName, LoadSceneMode.Additive);
                loaded = true;
            }
		
#if UNITY_EDITOR
            [UnityEditor.MenuItem(Editor.menu_item_1)]
#endif
            static void RegisterSceneToBuildSetting()
            {
#if UNITY_EDITOR
                var scenes = UnityEditor.EditorBuildSettings.scenes.ToList();
                scenes.Add(new UnityEditor.EditorBuildSettingsScene(subscenePath, true));
                UnityEditor.EditorBuildSettings.scenes = scenes.ToArray();
#endif
            }

            static bool HasRegistered()
            {
#if UNITY_EDITOR
                return UnityEditor.EditorBuildSettings.scenes.Any(e => e.path == subscenePath);
#else
			return true;
#endif
            }

            /// <summary>
            /// Fast Enter Play Mode 대응
            /// </summary>
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
            static void ResetDomain()
            {
                loaded = false;
            }

            #endregion
        }
    }
}