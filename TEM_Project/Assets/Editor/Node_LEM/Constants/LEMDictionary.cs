using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class LEMDictionary
{
    public static void LoadDictionary()
    {
        List<string> temp = s_NodeTypesDictionary.Keys.ToList();

        //Remove startnode
        string stringToReplace = temp[temp.Count - 1];
        temp[0] = stringToReplace;
        temp.RemoveAt(temp.Count - 1);

        //Sort everything into alphabetical order
        temp.Sort();


        s_NodeTypeKeys = temp.ToArray();

    }


    //This is a method to bypass the insanity of making a Dictionary that keeps the Generic T as its TValue
    readonly static Dictionary<string, object> s_NodeTypesDictionary = new Dictionary<string, object>
    {
        { "StartNode",                          new StartNode() },
        //{ "EndNode",                            new EndNode() },
        { "InstantiateGameObjectNode",          new InstantiateGameObjectNode() },
        { "DestroyGameObjectNode",              new DestroyGameObjectNode() },
        //{"RandomOutComeNode",                   new RandomOutComeNode() }
        { "DestroyGameObjectNode1",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode2",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode3",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode4",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode5",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode6",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode7",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode8",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode9",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode10",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode11",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode12",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode13",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode14",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode15",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode16",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode17",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode18",              new DestroyGameObjectNode() },
        { "DestroyGameObjectNode19",              new DestroyGameObjectNode() },

    };

    public static Dictionary<string, NodeSkinCollection> s_NodeStyleDictionary = new Dictionary<string, NodeSkinCollection>();


    //Return a object prefab to instantiate
    public static object GetNodeObject(string nodeObjectType)
    {
        s_NodeTypesDictionary.TryGetValue(nodeObjectType, out object value);
        //Instantiate the object's type using an alternative method.
        //the usual "new" keyword to instantitate is faster but Activator aint too bad either
        object instantiatedObject = Activator.CreateInstance(value.GetType());
        return instantiatedObject;
    }

    public static string[] s_NodeTypeKeys = default;

    public static string RemoveNodeWord(string nodeTypeName)
    {
        char[] characters = nodeTypeName.ToCharArray();
        nodeTypeName = nodeTypeName.Remove(characters.Length - 4);
        return nodeTypeName;
    }

}
