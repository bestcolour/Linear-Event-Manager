using UnityEngine;

namespace LEM_Editor
{
    [CreateAssetMenu(fileName = k_DefaultFileName, menuName = "NodeLEM Settings"),]
    public class NodeLEM_Settings : ScriptableObject
    {
        public const string k_DefaultFileName = "NewNodeEditorSettings";

        [Header("Theme")]
        public EditorTheme m_EditorTheme = EditorTheme.Light;

        [Header("Saving"), Tooltip("Enabling auto save makes the editor save whenver the window is closed or before you load another Linear Event")]
        public bool m_AutoSave = true;
        public bool m_SaveSceneWhenSavingLinearEvent = true;

        [Header("ToolBar")]
        public bool m_ShowToolBar = true;
        [Range(50f,150f)]
        public float m_ButtonSize = 50f;

    }

    public enum EditorTheme { Light, Dark }

}