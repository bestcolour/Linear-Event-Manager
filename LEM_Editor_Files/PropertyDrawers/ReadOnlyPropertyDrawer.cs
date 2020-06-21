using UnityEngine;
using UnityEditor;

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

