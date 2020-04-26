using System.Collections;
using System.Collections.Generic;
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

            if (GUILayout.Button("Update Editor Skins"))
            {
                LEMStyleLibrary.m_SkinsLoaded = false;
                LEMStyleLibrary.LoadLibrary();
            }

            GUILayout.EndHorizontal();
            base.OnInspectorGUI();

        }

    }

}