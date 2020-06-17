using UnityEngine;
using UnityEditor;
using LEM_Effects;

namespace LEM_Editor
{

    //This is a script for the tutorial events manager's inspector editor 
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LinearEvent))]
    public class LinearEvent_Inspector : Editor
    {
        //Declare measurements
        float m_LineHeightSpace = EditorGUIUtility.singleLineHeight * 1.5f;

        //OnGUI for inspector
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            LinearEvent linearEvent = (LinearEvent)target;

            GUILayout.BeginHorizontal();

            DrawLoadButton(linearEvent);
            DrawClearButton(linearEvent);

            GUILayout.EndHorizontal();

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

        void DrawClearButton(LinearEvent linearEvent)
        {
            //Creates a button to load the node editor
            if (GUILayout.Button("Clear All Effects", GUILayout.Height(m_LineHeightSpace * 2)))
            {
                linearEvent.ClearAllEffects();
            }
        }

        void DrawInspector()
        {
            bool wasDrawn = false;
            string effectsName = "m_AllEffects";

            SerializedProperty p = serializedObject.GetIterator();
            //Skip drawing the monobehaviour object field
            p.NextVisible(true);

            while (p.NextVisible(true))
            {
                if (p.propertyPath.Contains(effectsName))
                {
                    //If the effects array was not drawn,
                    if (!wasDrawn)
                    {
                        wasDrawn = GUI.enabled;
                        GUI.enabled = false;
                        EditorGUILayout.LabelField("Number of Events");
                        EditorGUILayout.IntField(p.arraySize);
                        GUI.enabled = wasDrawn;

                        wasDrawn = true;
                        continue;
                    }
                    continue;
                }


                EditorGUILayout.PropertyField(p);
            }

            #region OldCode
            //bool wasEnabled = GUI.enabled;
            //GUI.enabled = false;

            //EditorGUILayout.LabelField("Number of Events");
            //EditorGUILayout.IntField(serializedObject.FindProperty("m_AllEffects").arraySize);

            //GUI.enabled = wasEnabled;

            #endregion
        }

    }

}