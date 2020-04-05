using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Should only be used by NodeLEMEditor 
/// </summary>
public class LEMStyleLibrary
{
    public static LEMStyleLibrary Instance
    {
        get
        {
            //Self initialise
            if (m_Instance == null)
            {
                RefreshLibrary();
            }
            return m_Instance;
        }
    }
    static LEMStyleLibrary m_Instance = null;
    public static bool m_SkinsLoaded = false;

    #region Colours
    //public Dictionary<string, NodeSkinCollection> m_NodeStyleDictionary = new Dictionary<string, NodeSkinCollection>();
    public static Dictionary<string, Color> s_NodeColourDictionary = new Dictionary<string, Color>
    {
        {"StartNode",                  new Color(0.11f, 0.937f, 0.11f) },
        //{"EndNode",                    new Color(0.969f, 0.141f, 0.141f) },
        { "InstantiateGameObjectNode", new Color(0.286f,0.992f,0.733f)},
        { "DestroyGameObjectNode",     new Color(0.796f,0.098f,0.098f) },
         //{"RandomOutComeNode",         new Color(0.286f,0.992f,0.733f) }



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
        //Reset dictionary
        //m_NodeStyleDictionary.Clear();

        //string[] namesOfNodeEffectType = LEMDictionary.GetNodeTypeKeys();

        //The number range covers all the skins needed for gameobject effect related nodes
        //Naming convention is very important here
        //for (int i = 0; i < namesOfNodeEffectType.Length; i++)
        //{
        //    NodeSkinCollection skinCollection = new NodeSkinCollection();
        //    //Load the node skins texture
        //    skinCollection.light_normal = Resources.Load<Texture2D>("NodeBackground/light_" + namesOfNodeEffectType[i]);
        //    skinCollection.light_selected = Resources.Load<Texture2D>("NodeBackground/light_" + namesOfNodeEffectType[i] + "_Selected");
        //    skinCollection.textureToRender = skinCollection.light_normal;

        //    m_NodeStyleDictionary.Add(namesOfNodeEffectType[i], skinCollection);
        //}

        //s_StartNodeSkins.m_MidBackground = Resources.Load<Texture2D>("StartEnd/start");
        //s_StartNodeSkins.m_SelectedMidOutline = Resources.Load<Texture2D>("StartEnd/start_Selected");
        ////s_StartNodeSkins.textureToRender = s_StartNodeSkins.m_NodeBackground;

        //s_EndNodeSkins.m_MidBackground = Resources.Load<Texture2D>("StartEnd/end");
        //s_EndNodeSkins.m_SelectedMidOutline = Resources.Load<Texture2D>("StartEnd/end_Selected");
        //s_EndNodeSkins.textureToRender = s_EndNodeSkins.m_NodeBackground;

        //Initialise the execution pin style for normal and selected pins
        s_ConnectionPointStyleNormal = new GUIStyle();
        s_ConnectionPointStyleNormal.normal.background = Resources.Load<Texture2D>("NodeIcons/light_ExecutionPin");
        s_ConnectionPointStyleNormal.active.background = Resources.Load<Texture2D>("NodeIcons/light_ExecutionPin_Selected");

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
        s_WhiteBackGroundSkin.m_MidBackground = Resources.Load<Texture2D>("NodeBg/White_BackGround");
        s_WhiteBackGroundSkin.m_SelectedMidOutline = Resources.Load<Texture2D>("NodeBg/White_BackGround_Selected");
        s_WhiteBackGroundSkin.m_TopBackground = Resources.Load<Texture2D>("NodeBg/White_Top");
        s_WhiteBackGroundSkin.m_SelectedTopOutline = Resources.Load<Texture2D>("NodeBg/White_Top_Selected");
        //m_WhiteBackGroundSkin.textureToRender = m_WhiteBackGroundSkin.m_NodeBackground;                     



        //Initialising public static node title styles
        s_NodeHeaderStyle.fontSize = 13;
        s_NodeHeaderStyle.alignment = TextAnchor.MiddleCenter;

        s_NodeTextInputStyle = GUI.skin.GetStyle("textField");
        s_NodeTextInputStyle.fontSize = 10;

        s_NodeParagraphStyle.fontSize = 10;

        s_StartEndStyle.fontSize = 20;
        s_StartEndStyle.alignment = TextAnchor.MiddleCenter;

        //For now, just set the current top texture to a light themed colour
        s_CurrentTopTextureColour = new Color(0.862f, 0.894f, 0.862f);

    }

    public static void RefreshLibrary()
    {
        m_Instance = new LEMStyleLibrary();
        LoadLibrary();
    }

}
