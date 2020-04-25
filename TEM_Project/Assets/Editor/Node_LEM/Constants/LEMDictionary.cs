using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
namespace LEM_Editor
{
    public class LEMDictionary
    {
        public struct NodeDictionaryDefinition
        {
            public object m_Type;
            public Color m_Colour;

            public NodeDictionaryDefinition(object type, Color color)
            {
                m_Type = type;
                m_Colour = color;
            }

        }

        public static string[] s_NodeTypeKeys = default;

        public static readonly Dictionary<string, NodeDictionaryDefinition> s_NodeTypesDictionary = new Dictionary<string, NodeDictionaryDefinition>
        {
             { "StartNode", new NodeDictionaryDefinition(new StartNode(),  new Color(0.11f, 0.937f, 0.11f)) },
             //{ "EndNode",                            new EndNode() },
             { "InstantiateGameObjectNode", new NodeDictionaryDefinition(   new InstantiateGameObjectNode(),new Color(0.286f,0.992f,0.733f)) },
             { "DestroyGameObjectNode", new NodeDictionaryDefinition(  new DestroyGameObjectNode(),  new Color(0.796f,0.098f,0.098f)) },
             { "AddDelayNode",  new NodeDictionaryDefinition(    new AddDelayNode() , new Color(1f,0.667f,0.114f))},
             { "ToggleListenToClickNode",   new NodeDictionaryDefinition( new ToggleListenToClickNode(), new Color(0.498f,0.471f,1f)) },
             { "ToggleListenToTriggerNode", new NodeDictionaryDefinition(new ToggleListenToTriggerNode(),new Color(0.361f,0.82f,1f)) },
             { "EqualRandomOutComeNode", new NodeDictionaryDefinition(new EqualRandomOutComeNode(),new Color(0.302f,0.216f,0.851f)) },
             //{"RandomOutComeNode",                   new RandomOutComeNode() }
            
        };

        public static void LoadDictionary()
        {
            List<string> temp = s_NodeTypesDictionary.Keys.ToList();

            //Remove startnode
            temp.RemoveEfficiently(0);
            //Sort everything into alphabetical order
            temp.Sort();


            s_NodeTypeKeys = temp.ToArray();

        }

        //Return a object prefab to instantiate
        public static object GetNodeObject(string nodeObjectType)
        {
            s_NodeTypesDictionary.TryGetValue(nodeObjectType, out NodeDictionaryDefinition value);
            //Instantiate the object's type using an alternative method.
            //the usual "new" keyword to instantitate is faster but Activator aint too bad either
            object instantiatedObject = Activator.CreateInstance(value.m_Type.GetType());
            return instantiatedObject;
        }

        //Return a object prefab to instantiate
        public static Color GetNodeColour(string nodeObjectType)
        {
            s_NodeTypesDictionary.TryGetValue(nodeObjectType, out NodeDictionaryDefinition value);
            //Instantiate the object's type using an alternative method.
            //the usual "new" keyword to instantitate is faster but Activator aint too bad either
            return value.m_Colour;
        }


        public static string RemoveNodeWord(string nodeTypeName)
        {
            char[] characters = nodeTypeName.ToCharArray();
            nodeTypeName = nodeTypeName.Remove(characters.Length - 4);
            return nodeTypeName;
        }


        //    //This is a method to bypass the insanity of making a Dictionary that keeps the Generic T as its TValue
        //    readonly static Dictionary<string, object> s_NodeTypesDictionary = new Dictionary<string, object>
        //{
        //    { "StartNode",                          new StartNode() },
        //    //{ "EndNode",                            new EndNode() },
        //    { "InstantiateGameObjectNode",          new InstantiateGameObjectNode() },
        //    { "DestroyGameObjectNode",              new DestroyGameObjectNode() },
        //    { "AddDelayNode",                       new AddDelayNode() },
        //    { "ToggleListenToClickNode",                       new ToggleListenToClickNode() },
        //    { "ToggleListenToTriggerNode",                       new ToggleListenToTriggerNode() },
        //    //{"RandomOutComeNode",                   new RandomOutComeNode() }


        //};

        //public static Dictionary<string, NodeSkinCollection> s_NodeStyleDictionary = new Dictionary<string, NodeSkinCollection>();




        //public static string RemoveNameSpace(string nodeTypeName)
        //{
        //    nodeTypeName = nodeTypeName.Remove(0,11);
        //    return nodeTypeName;
        //}

    }

}