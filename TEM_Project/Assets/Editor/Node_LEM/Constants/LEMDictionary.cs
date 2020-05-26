using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using LEM_Effects.Extensions;
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

        public struct NodeIDs_Initials
        {
            public const string k_BaseEffectInital = "BEN_", k_StartNodeInitial = "SN_", k_GroupRectNodeInitial = "GRN_";
        }

        public static string[] s_NodeTypeKeys = default;

        public static readonly Dictionary<string, NodeDictionaryDefinition> s_NodeTypesDictionary = new Dictionary<string, NodeDictionaryDefinition>
        {
             { "StartNode", new NodeDictionaryDefinition(new StartNode(),  new Color(0.11f, 0.937f, 0.11f)) },

        #region On their own for now
             { "CustomVoidFunction", new NodeDictionaryDefinition(new CustomVoidFunctionNode()                      ,new Color(0.76f,0.15f,0.7f)) },

             { "LoadLinearEvent", new NodeDictionaryDefinition(new LoadNewLinearEventNode()                      ,new Color(0.76f,0.15f,0.7f)) },
             { "LoadLinearEvents", new NodeDictionaryDefinition(new LoadNewLinearEventsNode()                      ,new Color(0.76f,0.15f,0.7f)) },
             { "LoadRandomLinearEvent", new NodeDictionaryDefinition(new LoadRandomLinearEventNode()                      ,new Color(0.76f,0.15f,0.7f)) },
             { "LoadBiasedLinearEvent", new NodeDictionaryDefinition(new LoadBiasedLinearEventNode()                      ,new Color(0.76f,0.15f,0.7f)) },


             { "SetButtonInteractivityState", new NodeDictionaryDefinition(new SetButtonInteractivityStateNode()    ,new Color(0.4f,0.66f,0.18f)) },
	    #endregion


#region Random
             { "EqualRandomOutCome", new NodeDictionaryDefinition(new EqualProbabilityOutComeNode()                 ,new Color(0.302f,0.216f,0.851f)) },
             { "BiasedRandomOutcome", new NodeDictionaryDefinition(new BiasedRandomOutcomeNode()                    ,new Color(0.302f,0.216f,0.851f)) },
	#endregion


        #region GameObject
		    { "InstantiateGameObject", new NodeDictionaryDefinition(   new InstantiateGameObjectNode(),new Color(0.04f,0.65f,0.2f)) },
             { "DestroyGameObject", new NodeDictionaryDefinition(  new DestroyGameObjectNode(),  new Color(0.796f,0.098f,0.098f)) },
             { "DestroyGameObjects", new NodeDictionaryDefinition(  new DestroyGameObjectsNode(),  new Color(0.796f,0.098f,0.098f)) },
             { "SetGameObjectsActive", new NodeDictionaryDefinition(new SetGameObjectsActiveNode(),new Color(0.64f,0.09f,0.39f)) },
             { "SetGameObjectActive", new NodeDictionaryDefinition(new SetGameObjectActiveNode(),new Color(0.64f,0.09f,0.39f)) },

	#endregion

        #region SetTransform
             { "ReposTrans", new NodeDictionaryDefinition(new RepositionTransformNode()         ,new Color(0.15f ,0.68f,0.38f)) },
             { "ReposRectTrans", new NodeDictionaryDefinition(new RepositionRectTransformNode() ,new Color(0.15f ,0.68f,0.38f)) },
             { "ReScaleTrans", new NodeDictionaryDefinition(new RescaleTransformNode()          ,new Color(0.15f ,0.68f,0.38f)) },
             { "ReRotTrans", new NodeDictionaryDefinition(new ReRotateTransformNode()           ,new Color(0.15f ,0.68f,0.38f)) },
             { "ReSizeRectTrans", new NodeDictionaryDefinition(new ReSizeRectTransformNode()    ,new Color(0.15f ,0.68f,0.38f)) },
             { "SetTransParent", new NodeDictionaryDefinition(new SetTransformParentNode()      ,new Color(0.15f ,0.68f,0.38f)) },

	    #endregion

             
        #region OffSet Transform
             { "OffsetTransPos", new NodeDictionaryDefinition(new OffsetTransformPositionNode()         ,new Color(0.12f ,0.52f,0.29f)) },
             { "OffsetTransRot", new NodeDictionaryDefinition(new OffsetTransformRotationNode()         ,new Color(0.12f ,0.52f,0.29f)) },
             { "OffsetTransScale", new NodeDictionaryDefinition(new OffsetTransformScaleNode()          ,new Color(0.12f ,0.52f,0.29f)) },
             { "OffsetRectTransSize", new NodeDictionaryDefinition(new OffsetRectTransformSizeNode()    ,new Color(0.12f ,0.52f,0.29f)) },

	    #endregion


        #region Halt
             //{ "AddGlobalDelay",  new NodeDictionaryDefinition(    new AddGlobalDelayNode()             ,new Color(0.85f,0.64f,0.13f))},
             { "AddDelay",  new NodeDictionaryDefinition(    new AddDelayNode()                                         ,new Color(0.85f,0.64f,0.13f))},
             { "AddDelayAt",  new NodeDictionaryDefinition(    new AddDelayAtNode()                                     ,new Color(0.85f,0.64f,0.13f))},

             { "SetDelay",  new NodeDictionaryDefinition(    new SetDelayNode()                                     ,new Color(0.85f,0.64f,0.13f))},
             { "SetDelayAt",  new NodeDictionaryDefinition(    new SetDelayAtNode()                                     ,new Color(0.85f,0.64f,0.13f))},

             { "AwaitKeyCodeInput",   new NodeDictionaryDefinition( new AwaitKeyCodeInputNode()                           ,new Color(0.59f,0.24f,0.75f)) },
             { "AwaitAxisInput",   new NodeDictionaryDefinition( new AwaitAxisInputNode()                           ,new Color(0.59f,0.24f,0.75f)) },

             { "StopUpdateEffect", new NodeDictionaryDefinition(new StopUpdateEffectNode()                              ,new Color(1f ,0.55f,0f)) },

             { "GlobalPause", new NodeDictionaryDefinition(new PauseAllLinearEventsNode()                               ,new Color(1f ,0.55f,0f)) },
             { "Pause", new NodeDictionaryDefinition(new PauseCurrentLinearEventNode()                                  ,new Color(1f ,0.55f,0f)) },
             { "PauseLinearEvent", new NodeDictionaryDefinition(new PauseLinearEventNode()                              ,new Color(1f ,0.55f,0f)) },

        #endregion

        #region Animator
             { "SetAnimatorBool", new NodeDictionaryDefinition(new SetAnimatorBoolNode()            ,new Color(0.64f,0.55f,0.76f)) },
             { "SetAnimatorFloat", new NodeDictionaryDefinition(new SetAnimatorFloatNode()          ,new Color(0.64f,0.55f,0.76f)) },
             { "SetAnimatorInt", new NodeDictionaryDefinition(new SetAnimatorIntNode()              ,new Color(0.64f,0.55f,0.76f)) },
             { "SetAnimatorTrigger", new NodeDictionaryDefinition(new SetAnimatorTriggerNode()      ,new Color(0.64f,0.55f,0.76f)) },

	    #endregion

        #region Lerp and Movetowards
             //Position
             { "LerpRectransToPos", new NodeDictionaryDefinition(new LerpRectransToPosNode()            ,new Color(0.14f,0.44f,0.64f)) },
             { "LerpRectransToRectrans", new NodeDictionaryDefinition(new LerpRectransToRectransNode()  ,new Color(0.14f,0.44f,0.64f)) },
             { "LerpTransToPos", new NodeDictionaryDefinition(new LerpTransformToPositionNode()         ,new Color(0.14f,0.44f,0.64f)) },
             { "LerpTransToTrans", new NodeDictionaryDefinition(new LerpTransformToTransformNode()      ,new Color(0.14f,0.44f,0.64f)) },

             //Scale
             { "LerpScale", new NodeDictionaryDefinition(new LerpScaleNode()                            ,new Color(0.14f,0.44f,0.64f)) },
             { "LerpScaleRelativeToV3", new NodeDictionaryDefinition(new LerpScaleRelativeToV3Node()    ,new Color(0.14f,0.44f,0.64f)) },
             { "LerpScaleRelativeToT", new NodeDictionaryDefinition(new LerpScaleRelativeToTNode()      ,new Color(0.14f,0.44f,0.64f)) },

             //Rotation
             { "LerpRotation", new NodeDictionaryDefinition(new LerpRotationNode()                      ,new Color(0.14f,0.44f,0.64f)) },
             { "LerpRotationRelativeToT", new NodeDictionaryDefinition(new LerpRotationRelativeToTNode()                      ,new Color(0.14f,0.44f,0.64f)) },
             { "LerpRotationRelativeToV3", new NodeDictionaryDefinition(new LerpRotationRelativeToV3Node()                      ,new Color(0.14f,0.44f,0.64f)) },


             //Position
             { "MoveTowRectransToPos", new NodeDictionaryDefinition(new MoveTowRectransToPosNode()              ,new Color(0.49f,0.24f,0.60f)) },
             { "MoveTowRectransToRectrans", new NodeDictionaryDefinition(new MoveTowRectransToRectransNode()    ,new Color(0.49f,0.24f,0.60f)) },
             { "MoveTowTransToPos", new NodeDictionaryDefinition(new MoveTowardsTransformToPositionNode()       ,new Color(0.49f,0.24f,0.60f)) },
             { "MoveTowTransToTrans", new NodeDictionaryDefinition(new MoveTowardsTransformToTransformNode()    ,new Color(0.49f,0.24f,0.60f)) }, 
	    #endregion

        #region Repeat Transform
             { "RepeatLerpRectransToPosNode", new NodeDictionaryDefinition(new RepeatLerpRectransformToPositionNode()                   ,new Color(0.12f ,0.38f,0.55f)) },
             { "RepeatLerpTransToTrans", new NodeDictionaryDefinition(new RepeatLerpTransformToTransformNode()                          ,new Color(0.12f ,0.38f,0.55f)) },
             { "RepeatLerpTransToPos", new NodeDictionaryDefinition(new RepeatLerpTransformToPositionNode()                             ,new Color(0.12f ,0.38f,0.55f)) },
             { "RepeatLerpRectTransToRectTrans", new NodeDictionaryDefinition(new RepeatLerpRectTransformToRectTransformNode()          ,new Color(0.12f ,0.38f,0.55f)) },

             { "RepeatMoveTowTransToTrans", new NodeDictionaryDefinition(new RepeatMoveTowardsTransformToTransformNode()                ,new Color(0.36f ,0.17f,0.44f)) },
             { "RepeatMoveTowTransToPos", new NodeDictionaryDefinition(new RepeatMoveTowardsTransformToPositionNode()                   ,new Color(0.36f ,0.17f,0.44f)) },
             { "RepeatMoveTowRectransToRectrans", new NodeDictionaryDefinition(new RepeatMoveTowardsRectransformToRectransformNode()    ,new Color(0.36f ,0.17f,0.44f)) },
             { "RepeatMoveTowRectTransToPos", new NodeDictionaryDefinition(new RepeatMoveTowardsRectTransformToPositionNode()           ,new Color(0.36f ,0.17f,0.44f)) },

	    #endregion

#region Translate Transform
             { "TranslateRelativeToTransform", new NodeDictionaryDefinition(new TranslateRelativeToTransformNode()                    ,new Color(0.302f,0.216f,0.851f)) },
             { "TranslateRelativeToSpace", new NodeDictionaryDefinition(new TranslateRelativeToSpaceNode()                    ,new Color(0.302f,0.216f,0.851f)) },

	#endregion

        #region Fade Alpha
		     { "FadeToAlphaImage", new NodeDictionaryDefinition(new FadeToAlphaImageComponentNode()                                     ,new Color(0.19f ,0.74f,0.67f)) },
             { "FadeToAlphaImages", new NodeDictionaryDefinition(new FadeToAlphaImagesComponentNode()                                   ,new Color(0.19f ,0.74f,0.67f)) },

             { "FadeToAlphaRenderer", new NodeDictionaryDefinition(new FadeToAlphaRendererComponentNode()                               ,new Color(0.07f ,0.55f,0.46f)) },
             { "FadeToAlphaRenderers", new NodeDictionaryDefinition(new FadeToAlphaRenderersComponentNode()                             ,new Color(0.07f ,0.55f,0.46f)) },
             { "FadeToAlphaSpriteRenderer", new NodeDictionaryDefinition(new FadeToAlphaSpriteRendererComponentNode()                   ,new Color(0.067f ,0.48f,0.4f)) },
             { "FadeToAlphaSpriteRenderers", new NodeDictionaryDefinition(new FadeToAlphaSpriteRenderersComponentNode()                 ,new Color(0.067f ,0.48f,0.4f)) },

             { "FadeToAlphaText", new NodeDictionaryDefinition(new  FadeToAlphaTextComponentNode()                                      ,new Color(0.16f ,0.45f,0.65f)) },
             { "FadeToAlphaTexts", new NodeDictionaryDefinition(new FadeToAlphaTextsComponentNode()                                     ,new Color(0.16f ,0.45f,0.65f)) },

             { "FadeToAlphaTextMeshPro", new NodeDictionaryDefinition(new FadeToAlphaTextMeshProComponentNode()                         ,new Color(0.13f ,0.38f,0.55f)) },
             { "FadeToAlphaTextMeshPros", new NodeDictionaryDefinition(new FadeToAlphaTextMeshProsComponentNode()                       ,new Color(0.13f ,0.38f,0.55f)) },

             { "FadeToAlphaTextMeshProGUI", new NodeDictionaryDefinition(new FadeToAlphaTextMeshProGUIComponentNode()                   ,new Color(0.11f ,0.31f,0.45f)) },
             { "FadeToAlphaTextMeshProGUIs", new NodeDictionaryDefinition(new FadeToAlphaTextMeshProGUIsComponentNode()                 ,new Color(0.11f ,0.31f,0.45f)) }, 
       

	    #endregion

        #region Reword
             { "ReWordText", new NodeDictionaryDefinition(new ReWordTextComponentNode()                             ,new Color(0.16f ,0.45f,0.65f)) },

             { "ReWordTextMeshPro", new NodeDictionaryDefinition(new ReWordTextMeshProComponentNode()               ,new Color(0.13f ,0.38f,0.55f)) },

             { "ReWordTextMeshProGUI", new NodeDictionaryDefinition(new ReWordTextMeshProGUIComponentNode()         ,new Color(0.11f ,0.31f,0.45f)) }, 
	#endregion


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