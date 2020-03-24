using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//This is a script for the tutorial events manager's inspector editor 
[CanEditMultipleObjects]
[CustomEditor(typeof(LinearEvent))]
public class LEM_InspectorEditor : Editor
{

    //Declare measurements
    float m_LineHeight = default;
    float m_LineHeightSpace = default;


    private void OnEnable()
    {
        //Initialising the measurements' values
        m_LineHeight = EditorGUIUtility.singleLineHeight;
        m_LineHeightSpace = EditorGUIUtility.singleLineHeight * 1.5f;

    }


    //OnGUI for inspector
    public override void OnInspectorGUI()
    {

        serializedObject.Update();

        EditorGUILayout.Space();

        //Creates a button to load the node editor
        if (GUILayout.Button("Load in Node Editor", GUILayout.Height(m_LineHeightSpace * 2)))
        {
            LinearEvent linearEvent = (LinearEvent)target;
            NodeLEM_Editor.LoadNodeEditor(linearEvent);
        }



        EditorGUILayout.Space();

        #region Draw Rest of Default Inspector

        //Get the first property in the targetted object for this inspector (TEM)
        SerializedProperty currentProperty = serializedObject.GetIterator();

        //If the next property that is visible, inclding its children, is there,
        while (currentProperty.NextVisible(true))
        {
            //draw the current property including its child
            EditorGUILayout.PropertyField(currentProperty,false);
        }


        #endregion

        ////Creates a button to load the node editor
        //if (GUILayout.Button("Check Node Editor", GUILayout.Height(m_LineHeightSpace * 2)))
        //{
        //    LinearEvent linearEvent = (LinearEvent)target;
        //    linearEvent.m_EffectsConnected[0].TEM_Update();
        //}

        serializedObject.ApplyModifiedProperties();



    }


}
