using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//This is a script for the tutorial events manager's inspector editor 
[CanEditMultipleObjects]
[CustomEditor(typeof(TutorialEventsManager))]
public class TEM_InspectorEditor : Editor
{

    //Declare measurements
    float lineHeight = default;
    float lineHeightSpace = default;


    private void OnEnable()
    {
        //Initialising the measurements' values
        lineHeight = EditorGUIUtility.singleLineHeight;
        lineHeightSpace = EditorGUIUtility.singleLineHeight * 1.5f;

    }


    //OnGUI for inspector
    public override void OnInspectorGUI()
    {

        serializedObject.Update();

        EditorGUILayout.Space();

        //Creates a button to load the node editor
        if (GUILayout.Button("Load in Node Editor", GUILayout.Height(lineHeightSpace * 2)))
        {
            TutorialEventsManager tem = (TutorialEventsManager)target;
            NodeTEM_Editor.LoadNodeEditor(tem.AllEffects);
        }

        EditorGUILayout.Space();

        #region Draw Rest of Default Inspector

        //Get the first property in the targetted object for this inspector (TEM)
        SerializedProperty currentProperty = serializedObject.GetIterator();

        //If the next property that is visible, inclding its children, is there,
        while (currentProperty.NextVisible(true))
        {
            //draw the current property including its child
            EditorGUILayout.PropertyField(currentProperty);
        }


        #endregion

        serializedObject.ApplyModifiedProperties();

    }


}
