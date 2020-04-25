using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
namespace LEM_Editor
{

    public class LEMDictionary
    {
        public static void LoadDictionary()
        {
            List<string> temp = s_NodeTypesDictionary.Keys.ToList();

            //Remove startnode
            temp.RemoveEfficiently(0);
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
        { "AddDelayNode",                       new AddDelayNode() },
        { "ToggleListenToClickNode",                       new ToggleListenToClickNode() },
        { "ToggleListenToTriggerNode",                       new ToggleListenToTriggerNode() },
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

        public static string[] s_NodeTypeKeys = default;

        public static string RemoveNodeWord(string nodeTypeName)
        {
            char[] characters = nodeTypeName.ToCharArray();
            nodeTypeName = nodeTypeName.Remove(characters.Length - 4);
            return nodeTypeName;
        }

        public static string RemoveNameSpace(string nodeTypeName)
        {
            nodeTypeName = nodeTypeName.Remove(0,11);
            return nodeTypeName;
        }

    }

}