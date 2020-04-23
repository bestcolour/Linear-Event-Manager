using UnityEngine;

[CreateAssetMenu(fileName = k_DefaultFileName, menuName ="NodeLEM Settings"),]
public class NodeLEM_Settings : ScriptableObject
{
    public const string k_DefaultFileName = "NewNodeEditorSettings";

    [Header("Theme")]
    public EditorTheme m_EditorTheme = EditorTheme.Light;

    [Header("Enable Auto Save"),Tooltip("Enabling auto save makes the editor save whenver the window is closed or before you load another Linear Event")]
    public bool m_AutoSave = true;

    [Header("ToolBar")]
    public bool m_ShowToolBar = true;

}

public enum EditorTheme { Light,Dark}
