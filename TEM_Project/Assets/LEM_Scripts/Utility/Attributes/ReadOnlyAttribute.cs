using System;
using UnityEngine;
using UnityEditor;


[AttributeUsage(AttributeTargets.Field ,Inherited = true)]
public class ReadOnlyAttribute : PropertyAttribute{}

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyPropertyDrawer : PropertyDrawer
{

#if UNITY_EDITOR
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        bool prevEnabledState = GUI.enabled;
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = prevEnabledState;
    } 
#endif

}

