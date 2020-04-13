﻿using UnityEngine;
using UnityEditor;


//This is a script for the tutorial events manager's inspector editor 
[CanEditMultipleObjects]
[CustomEditor(typeof(LinearEvent))]
public class LEM_InspectorEditor : Editor
{
    //Declare measurements
    float m_LineHeight = EditorGUIUtility.singleLineHeight;
    float m_LineHeightSpace = EditorGUIUtility.singleLineHeight * 1.5f;
    public static bool s_IsLoaded = false;

    //OnGUI for inspector
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        LinearEvent linearEvent = (LinearEvent)target;

        DrawLoadButton(ref linearEvent);

        EditorGUILayout.Space();

        DrawRemoveUnusedEventsButton(ref linearEvent);

        EditorGUILayout.Space();

        //Draw Rest of Default Inspector
        base.OnInspectorGUI();

     

    }


    void DrawLoadButton(ref LinearEvent linearEvent)
    {
        //bool wasEnabled = GUI.enabled;
        //GUI.enabled = !s_IsLoaded;

        //Creates a button to load the node editor
        if (GUILayout.Button("Load in Node Editor", GUILayout.Height(m_LineHeightSpace)))
        {
            NodeLEM_Editor.LoadNodeEditor(linearEvent);
            s_IsLoaded = true;
        }

        //GUI.enabled = wasEnabled;

    }

    void DrawRemoveUnusedEventsButton(ref LinearEvent linearEvent)
    {
        //Creates a button to load the node editor
        if (GUILayout.Button("Remove Unused Events", GUILayout.Height(m_LineHeightSpace)))
        {
            linearEvent.RemoveUnusedEvents();
        }
    }




}
