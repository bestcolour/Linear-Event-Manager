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
    //public static bool s_IsLoaded = false;

    //OnGUI for inspector
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        LinearEvent linearEvent = (LinearEvent)target;

        GUILayout.BeginHorizontal();

        DrawLoadButton(linearEvent);
        DrawRemoveUnusedEventsButton(linearEvent);

        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //Draw Rest of Default Inspector
        base.OnInspectorGUI();

    }


    void DrawLoadButton(LinearEvent linearEvent)
    {
        //Creates a button to load the node editor
        if (GUILayout.Button("Load in Node Editor", GUILayout.Height(m_LineHeightSpace * 2)))
        {
            NodeLEM_Editor.LoadNodeEditor(linearEvent);
        }
    }

    void DrawRemoveUnusedEventsButton(LinearEvent linearEvent)
    {
        //Creates a button to load the node editor
        if (GUILayout.Button("Remove Unused Events", GUILayout.Height(m_LineHeightSpace * 2)))
        {
            linearEvent.RemoveUnusedEvents();
        }
    }




}
