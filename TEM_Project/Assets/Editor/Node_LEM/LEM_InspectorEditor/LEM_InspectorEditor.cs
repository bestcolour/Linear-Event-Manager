using UnityEngine;
using UnityEditor;


//This is a script for the tutorial events manager's inspector editor 
[CanEditMultipleObjects]
[CustomEditor(typeof(LinearEvent))]
public class LEM_InspectorEditor : Editor
{
    //Declare measurements
    float m_LineHeight = EditorGUIUtility.singleLineHeight;
    float m_LineHeightSpace = EditorGUIUtility.singleLineHeight * 1.5f;

    //OnGUI for inspector
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        LinearEvent linearEvent = (LinearEvent)target;

        //Creates a button to load the node editor
        if (GUILayout.Button("Load in Node Editor", GUILayout.Height(m_LineHeightSpace * 2)))
        {
            NodeLEM_Editor.LoadNodeEditor(linearEvent);
        }

        EditorGUILayout.Space();

        //Draw Rest of Default Inspector
        base.OnInspectorGUI();

    }

}
