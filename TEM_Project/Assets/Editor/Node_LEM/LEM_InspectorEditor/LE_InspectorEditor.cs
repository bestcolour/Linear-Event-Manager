using UnityEngine;
using UnityEditor;
using LEM_Effects;

namespace LEM_Editor
{

    //This is a script for the tutorial events manager's inspector editor 
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LinearEvent))]
    public class LE_InspectorEditor : Editor
    {
        //Declare measurements
        float m_LineHeightSpace = EditorGUIUtility.singleLineHeight * 1.5f;
        //public static bool s_IsLoaded = false;
        SerializedProperty m_ArraySizeProperty = default;

        //OnGUI for inspector
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            LinearEvent linearEvent = (LinearEvent)target;

            //GUILayout.BeginHorizontal();

            DrawLoadButton(linearEvent);
            //DrawRemoveUnusedEventsButton(linearEvent);

            //GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            DrawInspector();
            //base.OnInspectorGUI();

        }

        void DrawLoadButton(LinearEvent linearEvent)
        {
            //Creates a button to load the node editor
            if (GUILayout.Button("Load in Node Editor", GUILayout.Height(m_LineHeightSpace * 2)))
            {
                NodeLEM_Editor.LoadNodeEditor(linearEvent);
            }
        }

        //void DrawRemoveUnusedEventsButton(LinearEvent linearEvent)
        //{
        //    //Creates a button to load the node editor
        //    if (GUILayout.Button("Remove Unused Events", GUILayout.Height(m_LineHeightSpace * 2)))
        //    {
        //        linearEvent.RemoveUnusedEvents();
        //    }
        //}

        void DrawInspector()
        {
            bool wasEnabled = GUI.enabled;
            GUI.enabled = false;

            EditorGUILayout.LabelField("Number of Events");
            EditorGUILayout.IntField(serializedObject.FindProperty("m_AllEffects").arraySize);

            GUI.enabled = wasEnabled;
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("m_AllEffects"));
        }


    }

}