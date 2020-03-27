using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Should only be used by NodeLEMEditor 
/// </summary>
public class LEMSkinsLibrary
{
    public static LEMSkinsLibrary Instance = default;
    public bool m_SkinsLoaded = false;

    public Dictionary<string, NodeSkinCollection> m_NodeStyleDictionary = new Dictionary<string, NodeSkinCollection>();

    public GUIStyle m_InPointStyle = default;
    public GUIStyle m_OutPointStyle = default;
    public GUIStyle m_ConnectionPointStyleNormal = default;
    public GUIStyle m_ConnectionPointStyleSelected = default;

    //Start End Node
    public NodeSkinCollection m_StartNodeSkins = default;
    public NodeSkinCollection m_EndNodeSkins = default;

    public void LoadLibrary()
    {
        //If gui style has not been loaded
        if (!m_SkinsLoaded)
        {
            LoadingNodeSkins();
            m_SkinsLoaded = true;
        }
    }


    void LoadingNodeSkins()
    {
        //Reset dictionary
        m_NodeStyleDictionary.Clear();

        string[] namesOfNodeEffectType = LEMDictionary.GetNodeTypeKeys();

        //The number range covers all the skins needed for gameobject effect related nodes
        //Naming convention is very important here
        for (int i = 0; i < namesOfNodeEffectType.Length; i++)
        {
            NodeSkinCollection skinCollection = new NodeSkinCollection();
            //Load the node skins texture
            skinCollection.light_normal = Resources.Load<Texture2D>("NodeBackground/light_" + namesOfNodeEffectType[i]);
            skinCollection.light_selected = Resources.Load<Texture2D>("NodeBackground/light_" + namesOfNodeEffectType[i] + "_Selected");
            skinCollection.textureToRender = skinCollection.light_normal;

            m_NodeStyleDictionary.Add(namesOfNodeEffectType[i], skinCollection);
        }

        m_StartNodeSkins.light_normal = Resources.Load<Texture2D>("StartEnd/start");
        m_StartNodeSkins.light_selected = Resources.Load<Texture2D>("StartEnd/start_Selected");
        m_StartNodeSkins.textureToRender = m_StartNodeSkins.light_normal;

        m_EndNodeSkins.light_normal = Resources.Load<Texture2D>("StartEnd/end");
        m_EndNodeSkins.light_selected = Resources.Load<Texture2D>("StartEnd/end_Selected");
        m_EndNodeSkins.textureToRender = m_EndNodeSkins.light_normal;

        //Initialise the execution pin style for normal and selected pins
        m_ConnectionPointStyleNormal = new GUIStyle();
        m_ConnectionPointStyleNormal.normal.background = Resources.Load<Texture2D>("NodeIcons/light_ExecutionPin");
        m_ConnectionPointStyleNormal.active.background = Resources.Load<Texture2D>("NodeIcons/light_ExecutionPin_Selected");

        //Invert the two pins' backgrounds so that the user will be able to know what will happen if they press it
        m_ConnectionPointStyleSelected = new GUIStyle();
        m_ConnectionPointStyleSelected.normal.background = m_ConnectionPointStyleNormal.active.background;
        m_ConnectionPointStyleSelected.active.background = m_ConnectionPointStyleNormal.normal.background;


        //Load the in and out point gui styles
        m_InPointStyle = new GUIStyle();
        m_InPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        m_InPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;

        m_OutPointStyle = new GUIStyle();
        m_OutPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        m_OutPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;


    }

}
