using UnityEngine;
using UnityEditor;
namespace LEM_Editor
{

    public class LEMStyleLibrary
    {
        static bool SkinsLoaded { get; set; } = false;

        //Orginal Colour used by EditorGUI
        public static Color GUIOriginalColour_Label { get; private set; } = default;

        //PreviousColour is used by EditorGUI.LabelField and GUI.color (to draw different coloured node skins)
        public static Color GUIPreviousColour { get; set; } = default;
        static Color GUIPreviousColour_BoldLabel { get; set; } = default;
        static Color GUIPreviousColour_FoldOut { get; set; } = default;
        //To be pulled by all nodes with top textures 
        public static Color CurrentMidSkinColour { get; private set; } = default;
        public static Color CurrentBezierColour { get; private set; } = default;
        public static Color CurrentLabelColour { get; private set; } = default;
        public static Color CurrentGroupRectMidSkinColour { get; private set; } = default;
        public static Color CurrentGroupRectTopSkinColour { get; private set; } = default;

        public static GUIStyle ConnectionPointStyleNormal { get; private set; } = null;
        public static GUIStyle ConnectionPointStyleSelected { get; private set; } = null;

        //Node fontstyles
        public static readonly GUIStyle s_NodeHeaderStyle = new GUIStyle();
        public static readonly GUIStyle s_GroupLabelStyle = new GUIStyle();
        public static readonly GUIStyle s_StartEndStyle = new GUIStyle();
        public static readonly GUIStyle s_NodeParagraphStyle = new GUIStyle();

        //Just a default skin
        public static NodeSkinCollection WhiteBackgroundSkin { get; private set; } = null;
        const string k_NodeTextureAssetPath = "Assets/Editor/LEM_Editor_Files/Node_LEM/NodeTextures";


        #region Loading

        public static void LoadLibrary()
        {
            //If gui style has not been loaded
            if (!SkinsLoaded)
            {
                LoadingNodeSkins(NodeLEM_Editor.Settings);
                SkinsLoaded = true;
            }
        }

        public static void ReLoadLibrary()
        {
            LoadingNodeSkins(NodeLEM_Editor.Settings);
            SkinsLoaded = true;
        }


        static void LoadingNodeSkins(NodeLEM_Settings settings)
        {
            NodeLEM_Editor.LoadSettings();

            //Record original style colours so i dont mess them up
            GUIOriginalColour_Label = EditorStyles.label.normal.textColor;

            //Initialise the execution pin style for normal and selected pins
            ConnectionPointStyleNormal = new GUIStyle();
            ConnectionPointStyleNormal.normal.background = settings.m_EditorTheme == EditorTheme.Light ?
                AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeIcons/light_ExecutionPin.png", typeof(Texture2D)) as Texture2D :
                AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeIcons/dark_ExecutionPin.png", typeof(Texture2D)) as Texture2D;

            ConnectionPointStyleNormal.active.background = settings.m_EditorTheme == EditorTheme.Light ?
                AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeIcons/light_ExecutionPin_Selected.png", typeof(Texture2D)) as Texture2D :
                AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeIcons/dark_ExecutionPin_Selected.png", typeof(Texture2D)) as Texture2D;

            //Invert the two pins' backgrounds so that the user will be able to know what will happen if they press it
            ConnectionPointStyleSelected = new GUIStyle();
            ConnectionPointStyleSelected.normal.background = ConnectionPointStyleNormal.active.background;
            ConnectionPointStyleSelected.active.background = ConnectionPointStyleNormal.normal.background;

            WhiteBackgroundSkin = new NodeSkinCollection();
            WhiteBackgroundSkin.m_MidBackground = AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeBg/White_BackGround.png", typeof(Texture2D)) as Texture2D;
            WhiteBackgroundSkin.m_SelectedMidOutline = AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeBg/White_BackGround_Selected.png", typeof(Texture2D)) as Texture2D;
            WhiteBackgroundSkin.m_TopBackground = AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeBg/White_Top.png", typeof(Texture2D)) as Texture2D;
            WhiteBackgroundSkin.m_SelectedTopOutline = AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeBg/White_Top_Selected.png", typeof(Texture2D)) as Texture2D;

            //s_NodeHeaderStyle.normal.textColor = settings.m_EditorTheme == EditorTheme.Dark ? Color.white : Color.black;

            s_NodeHeaderStyle.fontSize = 13;
            s_NodeHeaderStyle.alignment = TextAnchor.MiddleCenter;
            s_NodeHeaderStyle.wordWrap = true;

            s_GroupLabelStyle.fontSize = 20;
            s_GroupLabelStyle.alignment = TextAnchor.MiddleCenter;
            s_GroupLabelStyle.wordWrap = true;

            //s_NodeTextInputStyle = GUI.skin.GetStyle("textField");
            //s_NodeTextInputStyle.fontSize = 10;
            //s_NodeTextInputStyle.wordWrap = true;
            //s_NodeTextInputStyle.normal.textColor = Color.black;

            s_NodeParagraphStyle.fontSize = 10;
            //s_NodeParagraphStyle.normal.textColor = settings.m_EditorTheme == EditorTheme.Dark ? Color.white : Color.black;

            //s_CurrentLabelColour = settings.m_EditorTheme == EditorTheme.Dark ? Color.white : Color.black;

            s_StartEndStyle.fontSize = 20;
            s_StartEndStyle.alignment = TextAnchor.MiddleCenter;


            //Initialising public static node title styles
            if (settings.m_EditorTheme == EditorTheme.Dark)
            {
                s_NodeHeaderStyle.normal.textColor = Color.white;
                s_NodeParagraphStyle.normal.textColor = Color.white;
                s_GroupLabelStyle.normal.textColor = Color.white;
                CurrentLabelColour = Color.white;
                CurrentMidSkinColour = new Color(0.164f, 0.164f, 0.164f);
                CurrentBezierColour = Color.white;
                CurrentGroupRectMidSkinColour = new Color(0, 0, 0, 0.1f);
                CurrentGroupRectTopSkinColour = new Color(0, 0, 0, 0.5f);

            }
            else
            {
                s_NodeHeaderStyle.normal.textColor = Color.white;
                s_GroupLabelStyle.normal.textColor = new Color(0.152f, 0.152f, 0.152f);
                s_NodeParagraphStyle.normal.textColor = Color.black;
                CurrentLabelColour = Color.black;
                CurrentMidSkinColour = Color.white;
                CurrentBezierColour = new Color(0.152f, 0.152f, 0.152f);
                CurrentGroupRectMidSkinColour = new Color(1, 1, 1, 0.5f);
                CurrentGroupRectTopSkinColour = new Color(1, 1, 1, 0.5f);
            }

        }





        #endregion

        public static void BeginEditorLabelColourChange(Color colourToChangeTo)
        {
            GUIPreviousColour = EditorStyles.label.normal.textColor;
            EditorStyles.label.normal.textColor = colourToChangeTo;
        }
        public static void EndEditorLabelColourChange()
        {
            EditorStyles.label.normal.textColor = GUIPreviousColour;
        }

        public static void AssertEditorLabelColour()
        {
            if (EditorStyles.label.normal.textColor != GUIOriginalColour_Label)
                EditorStyles.label.normal.textColor = GUIOriginalColour_Label;
        }


        public static void BeginEditorBoldLabelColourChange(Color colourToChangeTo)
        {
            GUIPreviousColour_BoldLabel = EditorStyles.boldLabel.normal.textColor;
            EditorStyles.boldLabel.normal.textColor = colourToChangeTo;
        }

        public static void EndEditorBoldLabelColourChange()
        {
            EditorStyles.boldLabel.normal.textColor = GUIPreviousColour_BoldLabel;
        }

        public static void BeginEditorFoldOutLabelColourChange(Color colourToChangeTo)
        {
            GUIPreviousColour_FoldOut = EditorStyles.foldout.normal.textColor;
            EditorStyles.foldout.normal.textColor = colourToChangeTo;
            EditorStyles.foldout.onNormal.textColor = colourToChangeTo;
        }

        public static void EndEditorFoldOutLabelColourChange()
        {
            EditorStyles.foldout.normal.textColor = GUIPreviousColour_FoldOut;
            EditorStyles.foldout.onNormal.textColor = GUIPreviousColour_FoldOut;
        }



    }

}