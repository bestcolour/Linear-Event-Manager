using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Should only be used by NodeLEMEditor 
/// </summary>
public class LEMStyleLibrary
{
    public static bool m_SkinsLoaded = false;

    #region Colours
    //public Dictionary<string, NodeSkinCollection> m_NodeStyleDictionary = new Dictionary<string, NodeSkinCollection>();
    public static Dictionary<string, Color> s_NodeColourDictionary = new Dictionary<string, Color>
    {
        {"StartNode",                  new Color(0.11f, 0.937f, 0.11f) },
        //{"EndNode",                    new Color(0.969f, 0.141f, 0.141f) },
        { "InstantiateGameObjectNode", new Color(0.286f,0.992f,0.733f)},
        { "DestroyGameObjectNode",     new Color(0.796f,0.098f,0.098f) },
         {"AddDelayNode",         new Color(1f,0.667f,0.114f) },
         {"ToggleListenToClickNode",         new Color(0.498f,0.471f,1f) },



    };

    public static Color s_GUIPreviousColour = default;
    //To be pulled by all nodes with top textures 
    public static Color s_CurrentTopTextureColour = default;


    #endregion

    public static GUIStyle s_InPointStyle = default;
    public static GUIStyle s_OutPointStyle = default;
    public static GUIStyle s_ConnectionPointStyleNormal = default;
    public static GUIStyle s_ConnectionPointStyleSelected = default;

    //Node fontstyles
    public static readonly GUIStyle s_NodeHeaderStyle = new GUIStyle();
    public static readonly GUIStyle s_StartEndStyle = new GUIStyle();
    public static GUIStyle s_NodeTextInputStyle = null;
    public static readonly GUIStyle s_NodeParagraphStyle = new GUIStyle();


    //Start End Node    
    //public static NodeSkinCollection s_StartNodeSkins = new NodeSkinCollection();
    //public static NodeSkinCollection s_EndNodeSkins = new NodeSkinCollection();

    //Just a default skin
    public static NodeSkinCollection s_WhiteBackGroundSkin = default;
    const string k_NodeTextureAssetPath = "Assets/Editor/Node_LEM/NodeTextures";

    public static void LoadLibrary()
    {
        //If gui style has not been loaded
        if (!m_SkinsLoaded)
        {
            LoadingNodeSkins();
            m_SkinsLoaded = true;
        }
    }


    static void LoadingNodeSkins()
    {
        //Initialise the execution pin style for normal and selected pins
        s_ConnectionPointStyleNormal = new GUIStyle();
        //s_ConnectionPointStyleNormal.normal.background = Resources.Load<Texture2D>("NodeIcons/light_ExecutionPin");
        s_ConnectionPointStyleNormal.normal.background = AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath+ "/NodeIcons/light_ExecutionPin.png", typeof(Texture2D)) as Texture2D;
        //s_ConnectionPointStyleNormal.active.background = Resources.Load<Texture2D>("NodeIcons/light_ExecutionPin_Selected");
        s_ConnectionPointStyleNormal.active.background = AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeIcons/light_ExecutionPin_Selected.png", typeof(Texture2D)) as Texture2D;

        //Invert the two pins' backgrounds so that the user will be able to know what will happen if they press it
        s_ConnectionPointStyleSelected = new GUIStyle();
        s_ConnectionPointStyleSelected.normal.background = s_ConnectionPointStyleNormal.active.background;
        s_ConnectionPointStyleSelected.active.background = s_ConnectionPointStyleNormal.normal.background;


        //Load the in and out point gui styles
        s_InPointStyle = new GUIStyle();
        s_InPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        s_InPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;

        s_OutPointStyle = new GUIStyle();
        s_OutPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        s_OutPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;

        s_WhiteBackGroundSkin = new NodeSkinCollection();
        //s_WhiteBackGroundSkin.m_MidBackground = Resources.Load<Texture2D>("NodeBg/White_BackGround");
        s_WhiteBackGroundSkin.m_MidBackground = AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeBg/White_BackGround.png", typeof(Texture2D)) as Texture2D;
        //s_WhiteBackGroundSkin.m_SelectedMidOutline = Resources.Load<Texture2D>("NodeBg/White_BackGround_Selected");
        s_WhiteBackGroundSkin.m_SelectedMidOutline = AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeBg/White_BackGround_Selected.png", typeof(Texture2D)) as Texture2D;
        //s_WhiteBackGroundSkin.m_TopBackground = Resources.Load<Texture2D>("NodeBg/White_Top");
        s_WhiteBackGroundSkin.m_TopBackground = AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeBg/White_Top.png", typeof(Texture2D)) as Texture2D;
        //s_WhiteBackGroundSkin.m_SelectedTopOutline = Resources.Load<Texture2D>("NodeBg/White_Top_Selected");
        s_WhiteBackGroundSkin.m_SelectedTopOutline = AssetDatabase.LoadAssetAtPath(k_NodeTextureAssetPath + "/NodeBg/White_Top_Selected.png", typeof(Texture2D)) as Texture2D;

        //Initialising public static node title styles
        s_NodeHeaderStyle.fontSize = 13;
        s_NodeHeaderStyle.alignment = TextAnchor.MiddleCenter;

        s_NodeTextInputStyle = GUI.skin.GetStyle("textField");
        s_NodeTextInputStyle.fontSize = 10;
        s_NodeTextInputStyle.wordWrap = true;

        s_NodeParagraphStyle.fontSize = 10;


        s_StartEndStyle.fontSize = 20;
        s_StartEndStyle.alignment = TextAnchor.MiddleCenter;

        //For now, just set the current top texture to a light themed colour
        s_CurrentTopTextureColour = new Color(0.862f, 0.894f, 0.862f);

    }

    public static void RefreshLibrary()
    {
        LoadingNodeSkins();
        m_SkinsLoaded = true;
    }

}
