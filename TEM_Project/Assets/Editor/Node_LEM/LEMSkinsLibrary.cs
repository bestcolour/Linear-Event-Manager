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
    public bool s_SkinsLoaded = false;

    public Dictionary<string, NodeSkinCollection> s_NodeStyleDictionary = new Dictionary<string, NodeSkinCollection>();

    public GUIStyle s_InPointStyle = default;
    public GUIStyle s_OutPointStyle = default;
    public GUIStyle s_ConnectionPointStyleNormal = default;
    public GUIStyle s_ConnectionPointStyleSelected = default;

    //Start End Node
    public NodeSkinCollection s_StartNodeSkins = default;
    public NodeSkinCollection s_EndNodeSkins = default;

    public void LoadLibrary()
    {
        //If gui style has not been loaded
        if (!s_SkinsLoaded)
        {
            LoadingNodeSkins();
            s_SkinsLoaded = true;
        }
    }


    void LoadingNodeSkins()
    {
        //Reset dictionary
        s_NodeStyleDictionary.Clear();

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

            s_NodeStyleDictionary.Add(namesOfNodeEffectType[i], skinCollection);
        }

        s_StartNodeSkins.light_normal = Resources.Load<Texture2D>("StartEnd/start");
        s_StartNodeSkins.light_selected = Resources.Load<Texture2D>("StartEnd/start_Selected");
        s_StartNodeSkins.textureToRender = s_StartNodeSkins.light_normal;

        s_EndNodeSkins.light_normal = Resources.Load<Texture2D>("StartEnd/end");
        s_EndNodeSkins.light_selected = Resources.Load<Texture2D>("StartEnd/end_Selected");
        s_EndNodeSkins.textureToRender = s_EndNodeSkins.light_normal;

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


    }

}
