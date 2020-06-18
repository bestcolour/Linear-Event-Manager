using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ConditionalReadOnlyAttribute))]
public class ConditionalReadOnlyPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalReadOnlyAttribute condReadATT = (ConditionalReadOnlyAttribute)attribute;

        //Check if the conditionalfield path is fullproperty path. If it isnt get full propertypath
        string fullPropPathToConditionalField = condReadATT.m_IsFullPropertyPath ? condReadATT.m_ConditionalFieldPropPath : GetFullPropertyPath(property, condReadATT);

        bool conditionalFieldState = CheckConditionalFieldState(property, fullPropPathToConditionalField);

        bool wasEnabled = GUI.enabled;
        GUI.enabled = conditionalFieldState == condReadATT.m_ConditionToMeet;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = wasEnabled;
    }

    string GetFullPropertyPath(SerializedProperty property, ConditionalReadOnlyAttribute condReadATT)
    {
        //Get propertypath of the field u wanna hide
        string propertyPath = property.propertyPath;

        //Assuming that the masterswitch/conditional field is in the same propertypath level (foreseeing structs and classes)
        string conditionalFieldPropPath = propertyPath.Replace(property.name, condReadATT.m_ConditionalFieldPropPath);

        return conditionalFieldPropPath;
    }

    bool CheckConditionalFieldState(SerializedProperty property, string fullPropPathToConditionalField)
    {
        //Find the property 
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(fullPropPathToConditionalField);

        bool enabled;
        if (sourcePropertyValue != null)
            enabled = sourcePropertyValue.boolValue;
        else
        {
            Debug.LogWarning("There is no conditional field at propertypath: " + fullPropPathToConditionalField);
            enabled = true;
        }

        return enabled;
    }




}
