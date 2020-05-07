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
             { "InstantiateGameObject", new NodeDictionaryDefinition(   new InstantiateGameObjectNode(),new Color(0.04f,0.65f,0.2f)) },
             { "DestroyGameObject", new NodeDictionaryDefinition(  new DestroyGameObjectNode(),  new Color(0.796f,0.098f,0.098f)) },
             { "DestroyGameObjects", new NodeDictionaryDefinition(  new DestroyGameObjectsNode(),  new Color(0.796f,0.098f,0.098f)) },
             { "AddDelay",  new NodeDictionaryDefinition(    new AddDelayNode() , new Color(0.85f,0.64f,0.13f))},
             { "ToggleListenToClick",   new NodeDictionaryDefinition( new ToggleListenToClickNode(), new Color(0.59f,0.24f,0.75f)) },
             { "ToggleListenToTrigger", new NodeDictionaryDefinition(new ToggleListenToTriggerNode(),new Color(0.25f,0.54f,0.75f)) },
             { "EqualRandomOutCome", new NodeDictionaryDefinition(new EqualRandomOutComeNode(),new Color(0.302f,0.216f,0.851f)) },
             { "SetGameObjectsActive", new NodeDictionaryDefinition(new SetGameObjectsActiveNode(),new Color(0.64f,0.09f,0.39f)) },
             { "SetGameObjectActive", new NodeDictionaryDefinition(new SetGameObjectActiveNode(),new Color(0.64f,0.09f,0.39f)) },
             { "SetAnimatorBool", new NodeDictionaryDefinition(new SetAnimatorBoolNode(),new Color(0.64f,0.55f,0.76f)) },
             { "SetAnimatorFloat", new NodeDictionaryDefinition(new SetAnimatorFloatNode(),new Color(0.64f,0.55f,0.76f)) },
             { "SetAnimatorInt", new NodeDictionaryDefinition(new SetAnimatorIntNode(),new Color(0.64f,0.55f,0.76f)) },
             { "SetAnimatorTrigger", new NodeDictionaryDefinition(new SetAnimatorTriggerNode(),new Color(0.64f,0.55f,0.76f)) },
             { "CustomVoidFunction", new NodeDictionaryDefinition(new CustomVoidFunctionNode(),new Color(0.76f,0.15f,0.7f)) },
             { "SetButtonInteractivityState", new NodeDictionaryDefinition(new SetButtonInteractivityStateNode(),new Color(0.4f,0.66f,0.18f)) },
             { "LerpRectransToPos", new NodeDictionaryDefinition(new LerpRectransToPosNode(),new Color(0.09f,0.34f,0.75f)) },
             { "LerpRectransToRectrans", new NodeDictionaryDefinition(new LerpRectransToRectransNode(),new Color(0.09f,0.34f,0.75f)) },
             { "MoveTowRectransToPos", new NodeDictionaryDefinition(new MoveTowRectransToPosNode(),new Color(0.09f,0.34f,0.75f)) },
             { "MoveTowRectransToRectrans", new NodeDictionaryDefinition(new MoveTowRectransToRectransNode(),new Color(0.09f,0.34f,0.75f)) },
             { "LerpTransToPos", new NodeDictionaryDefinition(new LerpTransformToPositionNode(),new Color(0.48f,0.04f,0.69f)) },
             { "LerpTransToTrans", new NodeDictionaryDefinition(new LerpTransformToTransformNode(),new Color(0.48f,0.04f,0.69f)) },
             { "MoveTowTransToPos", new NodeDictionaryDefinition(new MoveTowardsTransformToPositionNode(),new Color(0.48f,0.04f,0.69f)) },
             { "MoveTowTransToTrans", new NodeDictionaryDefinition(new MoveTowardsTransformToTransformNode(),new Color(0.48f,0.04f,0.69f)) },
             { "OffsetTransPos", new NodeDictionaryDefinition(new OffsetTransformPositionNode(),new Color(0.19f ,0.74f,0.67f)) },
             { "OffsetTransRot", new NodeDictionaryDefinition(new OffsetTransformRotationNode(),new Color(0.19f ,0.74f,0.67f)) },
             { "OffsetTransScale", new NodeDictionaryDefinition(new OffsetTransformScaleNode(),new Color(0.19f ,0.74f,0.67f)) },
            
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

    }

}