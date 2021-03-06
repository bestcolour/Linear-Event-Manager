using UnityEngine;
using UnityEditor;

namespace LEM_Editor
{
    [CustomEditor(typeof(NodeLEM_Settings))]
    public class NodeLEM_SettingsInspector : Editor
    {
        public override void OnInspectorGUI()
        {

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Update Editor Skins",GUILayout.Height(30f)))
            {
                LEMStyleLibrary.ReLoadLibrary();
            }

            GUILayout.EndHorizontal();
            base.OnInspectorGUI();

        }

    }

}