using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LinearDescriptionAttribute))]
public class LinearDescriptionPropertyDrawer : PropertyDrawer
{

    bool m_Occupied = false;
    

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        if (!property.type.Contains("LinearEvent"))
        {
#if UNITY_EDITOR
            Debug.LogWarning("The property " + property.name + " is not of LinearEvent type! Use LinearDescriptionPropertyDrawer attribute only on LinearEvent fields!");
#endif
            EditorGUI.PropertyField(position, property, label);
            m_Occupied = false;
            return;
        }

        //If there is no value in this field, just draw it as normal
        if (property.objectReferenceValue == null)
        {
            EditorGUI.PropertyField(position, property, label);
            m_Occupied = false;
            return;
        }

        //Else display the description text
        SerializedObject linearEventObject = new SerializedObject(property.objectReferenceValue);
        position.y -= EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(position, "Name Of Linear Description: " + linearEventObject.FindProperty("m_LinearDescription").stringValue,EditorStyles.boldLabel);
        position.y +=  2*EditorGUIUtility.singleLineHeight;
        position.height = EditorGUIUtility.singleLineHeight;

        label.text = property.displayName;
        EditorGUI.PropertyField(position, property, label);
        m_Occupied = true;

    }

    //Override this
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = base.GetPropertyHeight(property, label);
        if (!m_Occupied)
            return height;
        else
        {
            return height + EditorGUIUtility.singleLineHeight *2f;
        }
    }




}

