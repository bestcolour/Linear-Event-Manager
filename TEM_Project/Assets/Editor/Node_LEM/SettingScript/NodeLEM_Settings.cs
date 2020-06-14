using UnityEngine;

namespace LEM_Editor
{
    [CreateAssetMenu(fileName = k_DefaultFileName, menuName = "NodeLEM Settings"),]
    public class NodeLEM_Settings : ScriptableObject
    {
        // <summary>
        // DontSave = NodeEditor doesnt save itself at all,
        // AlwaysSave = NodeEditor will always save itself whenever Editor loses windowfocus or before loading a new lienaar event
        // SaveWhenCommandChange = NodeEditor will only save when there is a change in the editor base on its commands ie. Move Node, Cut, Paste, Create Node etc
        // </summary>
        public enum SaveSettings { DontSave, AlwaysSave, SaveWhenCommandChange };

        public const string k_DefaultFileName = "NewNodeEditorSettings";

        [Header("Theme")]
        public EditorTheme m_EditorTheme = EditorTheme.Light;

        [Header("Saving"), Tooltip("DontSave = NodeEditor doesnt save itself at all\nAlwaysSave = NodeEditor will always save itself whenever Editor loses windowfocus or before loading a new linear event\nSaveWhenCommandChange = NodeEditor will only save when there is a change in the editor base on its commands ie. Move Node, Cut, Paste, Create Node etc")]
        public SaveSettings m_SaveSettings = SaveSettings.DontSave;
        //public bool m_AutoSave = true;

        [Tooltip("Hide saved effects on the Linear Event?")]
        public bool m_ShowMonoBehaviours = false;

        [Tooltip("Save scene when saving LinearEvent")]
        public bool m_SaveSceneTogether = true;

        [Header("History")]
        public int m_HistoryLength = 100;

        [Header("ToolBar")]
        public bool m_ShowToolBar = true;
        [Range(50f, 150f)]
        public float m_ButtonSize = 50f;

    }

    public enum EditorTheme { Light, Dark }

}