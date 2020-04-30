using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class BooleanHideInspectorAttribute : PropertyAttribute
{
    public string m_FieldName = default;
    public bool m_HideInInspector = default;

    //Overloads constructor for boolean hide attributes
    public BooleanHideInspectorAttribute(string fieldName)
    {
        m_FieldName = fieldName;
    }

    public BooleanHideInspectorAttribute(string fieldName,bool hideInInspector)
    {
        m_FieldName = fieldName;
        m_HideInInspector = hideInInspector;
    }

}


[CustomPropertyDrawer(typeof(BooleanHideInspectorAttribute))]
public class BooleanHideInspectorPropertyDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        BooleanHideInspectorAttribute hideATT = (BooleanHideInspectorAttribute)attribute ;







    }

    bool CheckHideInspectorState(SerializedProperty property,BooleanHideInspectorAttribute hideATT)
    {
        //Get original property path
        string propertyPath = property.propertyPath;

        string hideATTPropertyPath = propertyPath.Replace(property.name, hideATT.m_FieldName);

        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(hideATTPropertyPath);

        bool enabled = sourcePropertyValue != null ? sourcePropertyValue.boolValue : true;

        return enabled;
    }


}

#endif
