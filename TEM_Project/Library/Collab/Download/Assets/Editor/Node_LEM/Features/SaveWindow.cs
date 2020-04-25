using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveWindow : EditorWindow
{
    Action d_YesButton = null;
    Action d_NoButton = null;
    Action d_CancelButton = null;
    string m_YesString = default;
    string m_NoString = default;

    public static SaveWindow OpenWindow(Action yesButton, Action noButton, Action cancelButton,string yesButtonString = "Yes", string noButtonString = "No")
    {
        SaveWindow window = GetWindow<SaveWindow>(utility: true);
        window.d_YesButton = yesButton;
        window.d_NoButton = noButton;
        window.d_CancelButton = cancelButton;

        window.m_YesString = yesButtonString;
        window.m_NoString = noButtonString;
        return window;
    }


    private void OnGUI()
    {

        GUILayout.Label("Do you want to save before leaving LEM Node Editor?");

        GUILayout.Space(20f);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button(m_YesString,GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true)))
        {
            d_YesButton?.Invoke();
            Close();
        }

        if (GUILayout.Button(m_NoString,GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true)))
        {
            d_NoButton?.Invoke();
            Close();
        }
        GUILayout.EndHorizontal();

    }

    private void OnDestroy()
    {
        d_CancelButton?.Invoke();
    }

}
