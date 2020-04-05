using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class LEMDictionary 
{
    //This is a method to bypass the insanity of making a Dictionary that keeps the Generic T as its TValue
    readonly static Dictionary<string, object> s_NodeTypesDictionary = new Dictionary<string, object>
    {
        { "StartNode",                          new StartNode() },
        //{ "EndNode",                            new EndNode() },
        { "InstantiateGameObjectNode",          new InstantiateGameObjectNode() },
        { "DestroyGameObjectNode",              new DestroyGameObjectNode() },
        //{"RandomOutComeNode",                   new RandomOutComeNode() }



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

    public static string[] GetNodeTypeKeys()
    {
       return s_NodeTypesDictionary.Keys.ToArray();
    }

    public static string RemoveNodeWord(string nodeTypeName)
    {
        char[] characters = nodeTypeName.ToCharArray();
        nodeTypeName = nodeTypeName.Remove(characters.Length - 4);
        return nodeTypeName;
    }

}
