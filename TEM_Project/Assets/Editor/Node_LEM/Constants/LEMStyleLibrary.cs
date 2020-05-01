using UnityEngine;
using UnityEditor;
namespace LEM_Editor
{

    public class LEMStyleLibrary
    {
        public static bool m_SkinsLoaded = false;

        public static Color s_GUIPreviousColour = default;
        //To be pulled by all nodes with top textures 
        public static Color s_CurrentMidSkinColour = default;
        public static Color s_CurrentBezierColour = default;
        public static Color s_CurrentLabelColour = default;

        //public static GUIStyle s_InPointStyle = default;
        //public static GUIStyle s_OutPointStyle = default;
        public static GUIStyle s_ConnectionPointStyleNormal = default;
        public static GUIStyle s_ConnectionPointStyleSelected = default;

        //Node fontstyles
        public static readonly GUIStyle s_NodeHeaderStyle = new GUIStyle();
        public static readonly GUIStyle s_StartEndStyle = new GUIStyle();
        public static GUIStyle s_NodeTextInputStyle = null;
        public static readonly GUIStyle s_NodeParagraphStyle = new GUIStyle();

        //Just a default skin
        public static NodeSkinCollection s_WhiteBackGroundSkin = default;
        const string k_NodeTextureAssetPath = "Assets/Editor/Node_LEM/NodeTextures";

        public static void LoadLibrary()
        {
            //If gui style has not been loaded
            if (!m_SkinsLoaded)
            {
                LoadingNodeSkins(NodeLEM_Editor.s_Settings);
                m_SkinsLoaded = true;
            }
        }

        public static void BeginEditorLabelColourChange(Color colourToChangeTo)
        {
            s_GUIPreviousColour = EditorStyles.label.normal.textColor;
            EditorStyles.label.normal.textColor = s_CurrentLabelColour;
        }
        public static void EndEditorLabelColourChange()
        {
            EditorStyles.label.normal.textColor = s_GUIPreviousColour;
        }


        static void LoadingNodeSkins(NodeLEM_Settings settings)
        {
            NodeLEM_Editor.LoadSettings();
            //Initialise the execution pin style for normal and selected pins
            s_ConnectionPointStyleNormal = new GUIStyle();
            s_ConnectionPointStyleNormal.normal.background = settings.m_EditorTheme == EditorTheme.Light ?
                AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeIcons/light_ExecutionPin.png", typeof(Texture2D)) as Texture2D :
                 AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeIcons/dark_ExecutionPin.png", typeof(Texture2D)) as Texture2D
                ;
            s_ConnectionPointStyleNormal.active.background = settings.m_EditorTheme == EditorTheme.Light ?
                AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeIcons/light_ExecutionPin_Selected.png", typeof(Texture2D)) as Texture2D :
                AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeIcons/dark_ExecutionPin_Selected.png", typeof(Texture2D)) as Texture2D;

            //Invert the two pins' backgrounds so that the user will be able to know what will happen if they press it
            s_ConnectionPointStyleSelected = new GUIStyle();
            s_ConnectionPointStyleSelected.normal.background = s_ConnectionPointStyleNormal.active.background;
            s_ConnectionPointStyleSelected.active.background = s_ConnectionPointStyleNormal.normal.background;


            ////Load the in and out point gui styles
            //s_InPointStyle = new GUIStyle();
            //s_InPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            //s_InPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;

            //s_OutPointStyle = new GUIStyle();
            //s_OutPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
            //s_OutPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;

            s_WhiteBackGroundSkin = new NodeSkinCollection();
            s_WhiteBackGroundSkin.m_MidBackground = AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeBg/White_BackGround.png", typeof(Texture2D)) as Texture2D;
            s_WhiteBackGroundSkin.m_SelectedMidOutline = AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeBg/White_BackGround_Selected.png", typeof(Texture2D)) as Texture2D;
            s_WhiteBackGroundSkin.m_TopBackground = AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeBg/White_Top.png", typeof(Texture2D)) as Texture2D;
            s_WhiteBackGroundSkin.m_SelectedTopOutline = AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeBg/White_Top_Selected.png", typeof(Texture2D)) as Texture2D;

            //Initialising public static node title styles
            s_NodeHeaderStyle.normal.textColor = settings.m_EditorTheme == EditorTheme.Dark ? Color.white : Color.black;

            s_NodeHeaderStyle.fontSize = 13;
            s_NodeHeaderStyle.alignment = TextAnchor.MiddleCenter;

            s_NodeTextInputStyle = GUI.skin.GetStyle("textField");
            s_NodeTextInputStyle.fontSize = 10;
            s_NodeTextInputStyle.wordWrap = true;
            s_NodeTextInputStyle.normal.textColor = Color.black;

            s_NodeParagraphStyle.fontSize = 10;
            s_NodeParagraphStyle.normal.textColor = settings.m_EditorTheme == EditorTheme.Dark ? Color.white : Color.black;

            s_CurrentLabelColour = settings.m_EditorTheme == EditorTheme.Dark ? Color.white : Color.black;

            s_StartEndStyle.fontSize = 20;
            s_StartEndStyle.alignment = TextAnchor.MiddleCenter;

            s_CurrentMidSkinColour = settings.m_EditorTheme == EditorTheme.Dark ? new Color(0.164f, 0.164f, 0.164f) : Color.white;
            s_CurrentBezierColour = settings.m_EditorTheme == EditorTheme.Light ? new Color(0.152f, 0.152f, 0.152f) : Color.white;
            //s_CurrentBezierColour = settings.m_EditorTheme == EditorTheme.Light ? new Color(0.227f, 0.216f, 0.212f) : Color.white;
        }

        public static void RefreshLibrary()
        {
            LoadingNodeSkins(NodeLEM_Editor.s_Settings);
            m_SkinsLoaded = true;
        }


    }

}