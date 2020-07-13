using UnityEngine;
using UnityEditor;

namespace RuntimeConsole.Editor
{
    [CustomEditor(typeof(RuntimeConsole.Console))]
    internal class RuntimeEditor : UnityEditor.Editor {
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

        void OnEnable() {
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

        public override void OnInspectorGUI() {

            serializedObject.Update();

            // todo
            // validate components state

            if (IsValidComponents()) {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("All Components are initialized well.");
                EditorGUILayout.Space(10);
            } else {
                if (GUILayout.Button("Find Components", new[] { GUILayout.Height(30)})) {
                    (target as Console).FindComponents();
                    return;
                }
            }
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Assembly", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(m_assemblyFilter);
            switch((Console.EFilterType)m_assemblyFilter.enumValueIndex) {
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
            EditorGUILayout.LabelField("Namespace", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(m_namespaceFilter);
            switch((Console.EFilterType)m_namespaceFilter.enumValueIndex) {
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

        bool IsValidComponents() {
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
    }
}