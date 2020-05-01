using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(BoolHideIfAttribute))]
public class BoolHideIfPropertyDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        BoolHideIfAttribute hideATT = (BoolHideIfAttribute)attribute;

        //Check if the conditionalfield path is fullproperty path. If it isnt get full propertypath
        string fullPropPathToConditionalField = hideATT.m_IsFullPropertyPath ? hideATT.m_ConditionalFieldPropPath : GetFullPropertyPath(property, hideATT);

        bool conditionalFieldState = CheckConditinalFieldState(property, fullPropPathToConditionalField);

        bool wasEnabled = GUI.enabled;

        #region Draw Field
        //If the conditionalfield state is the same as the state to observe
        if (conditionalFieldState == hideATT.m_StateToObserve)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.ArraySize:
                    for (int i = 0; i < property.arraySize; i++)
                    {
                        EditorGUI.PropertyField(position, property, label, true);
                        position.y += EditorGUIUtility.singleLineHeight;
                    }

                    break;


                default:
                    EditorGUI.PropertyField(position, property, label, true);
                    break;
            }
        }




        #endregion

        GUI.enabled = wasEnabled;
    }

    //Edit the height of the space the property occupies based on the state of whether the property being drawwn or not
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        BoolHideIfAttribute hideATT = (BoolHideIfAttribute)attribute;

        string fullPropPathToConditionalField = hideATT.m_IsFullPropertyPath ? hideATT.m_ConditionalFieldPropPath : GetFullPropertyPath(property, hideATT);

        bool conditionalFieldState = CheckConditinalFieldState(property, fullPropPathToConditionalField);

        float heightOfProperty = conditionalFieldState == hideATT.m_StateToObserve ? EditorGUI.GetPropertyHeight(property, label, true) : -EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.singleLineHeight;
        return heightOfProperty;
    }

    string GetFullPropertyPath(SerializedProperty property, BoolHideIfAttribute hideATT)
    {
        //Get propertypath of the field u wanna hide
        string propertyPath = property.propertyPath;

        //Assuming that the masterswitch/conditional field is in the same propertypath level (foreseeing structs and classes)
        string conditionalFieldPropPath = propertyPath.Replace(property.name, hideATT.m_ConditionalFieldPropPath);

        return conditionalFieldPropPath;
    }

    bool CheckConditinalFieldState(SerializedProperty property, string fullPropPathToConditionalField)
    {
        //Find the 
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

#endif
