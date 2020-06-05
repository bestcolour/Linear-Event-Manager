using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using LEM_Effects.Extensions;
using LEM_Effects;

namespace LEM_Editor
{
    public class LEMDictionary
    {
        public class NodeDictionaryDefinition
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

      

        #region Halt
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

        #region TransformRelated Effects

            

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

             //Note: Effects under lerp, movetowards and Curve are all non-additive effects which means that any other effects which intends to add on to the 
             //lerp/movetowards effects will result in conflicts in which one effect will dominate over the other and apply its transform value changes over the other's
        #region LERP
		
        #region Position
             //Lerps a rectTransform to a vector3 position
             { "LerpRectransToPos", new NodeDictionaryDefinition(new LerpRectransToPosNode()            ,new Color(0.14f,0.44f,0.64f)) },

             //Lerps a rectTransform to another rectTransform's anchored3DPosition
             { "LerpRectransToRectrans", new NodeDictionaryDefinition(new LerpRectransToRectransNode()  ,new Color(0.14f,0.44f,0.64f)) },

             //Lerps a transform to a vector3 position
             { "LerpTransToPos", new NodeDictionaryDefinition(new LerpTransformToPositionNode()         ,new Color(0.14f,0.44f,0.64f)) },

             //Lerps a transform to a transform's world position
             { "LerpTransToTrans", new NodeDictionaryDefinition(new LerpTransformToTransformNode()      ,new Color(0.14f,0.44f,0.64f)) },


             //Lerps a transform to another vector3 position and will reset the transform's position to its original value upon completion before Lerping again
             { "RepeatLerpRectransToPosNode", new NodeDictionaryDefinition(new RepeatLerpRectransformToPositionNode()                   ,new Color(0.12f ,0.38f,0.55f)) },

             //Lerps a transform to a transform's world position and will reset the transform's position to its original value upon completion before Lerping again
             { "RepeatLerpTransToTrans", new NodeDictionaryDefinition(new RepeatLerpTransformToTransformNode()                          ,new Color(0.12f ,0.38f,0.55f)) },

             //Lerps a rectTransform to a vector3 position and will reset the rectTransform's anchored3DPosition to its original value upon completion before Lerping again
             { "RepeatLerpTransToPos", new NodeDictionaryDefinition(new RepeatLerpTransformToPositionNode()                             ,new Color(0.12f ,0.38f,0.55f)) },

             //Lerps a rectTransform to another rectTransform's anchored3DPosition and will reset the rectTransform's anchored3DPosition to its original value upon completion before Lerping again
             { "RepeatLerpRectTransToRectTrans", new NodeDictionaryDefinition(new RepeatLerpRectTransformToRectTransformNode()          ,new Color(0.12f ,0.38f,0.55f)) },

        #endregion

        #region Rotation
		     //Rotation
             //In world rotation means applying the rotation to the transform's rotation property instead of localRotation
             //Note: Lerp Rotation takes in a targeted rotation and hence the TargetTransform will always take the shortest rotational path to achieve that Targeted Rotation

             //Lerps a Transform's rotation/localRotation to a V3 eulerAngle
             { "LerpRotationToV3", new NodeDictionaryDefinition(new LerpRotationToV3Node()                              ,new Color(0.14f,0.44f,0.64f)) },

             //Lerps a Transform's rotation/localRotation to a V3 eulerAngle about a Transform Pivot
             { "LerpRotationToV3AboutTPivot", new NodeDictionaryDefinition(new LerpRotationToV3AboutTPivotNode()        ,new Color(0.14f,0.44f,0.64f)) },

             //Lerps a Transform's rotation/localRotation to a V3 eulerAngle about a V3 Pivot
             { "LerpRotationToV3AboutV3Pivot", new NodeDictionaryDefinition(new LerpRotationToV3AboutV3PivotNode()      ,new Color(0.14f,0.44f,0.64f)) },

             //Lerps a Transform's rotation/local Rotation to match another Transform's Rotational value
             { "LerpRotationToT", new NodeDictionaryDefinition(new LerpRotationToTNode()                                ,new Color(0.14f,0.44f,0.64f)) },

             //Lerps a Transform's rotation/localRotation to match another Transform's Rotational value about a Transform Pivot
             { "LerpRotationToTAboutTPivot", new NodeDictionaryDefinition(new LerpRotationToTAboutTPivotNode()          ,new Color(0.14f,0.44f,0.64f)) },

             //Lerps a Transform's rotation/localRotation to match another Transform's Rotational value about a Vector3 Pivot
             { "LerpRotationToTAboutV3Pivot", new NodeDictionaryDefinition(new LerpRotationToTAboutV3PivotNode()        ,new Color(0.14f,0.44f,0.64f)) },



              //Lerps a Transform's rotation/localRotation to a V3 eulerAngle and will reset the Transform's rotation/local to its original value upon completion before Lerping again
             { "RepeatLerpRotationToV3", new NodeDictionaryDefinition(new RepeatLerpRotationToV3Node()                                  ,new Color(0.12f ,0.38f,0.55f)) },

              //Lerps a Transform's rotation/localRotation to a V3 eulerAngle about a Transform Pivot and will reset the Transform's rotation/local to its original value upon completion before Lerping again
             { "RepeatLerpRotationToV3AboutTPivot", new NodeDictionaryDefinition(new RepeatLerpRotationToV3AboutTPivotNode()            ,new Color(0.12f ,0.38f,0.55f)) },

              //Lerps a Transform's rotation/localRotation to a V3 eulerAngle about a V3 Pivot Pivot and will reset the Transform's rotation/local to its original value upon completion before Lerping again
             { "RepeatLerpRotationToV3AboutV3Pivot", new NodeDictionaryDefinition(new RepeatLerpRotationToV3AboutV3PivotNode()          ,new Color(0.12f ,0.38f,0.55f)) },

              //Lerps a Transform's rotation/local Rotation to match another Transform's Rotational value and will reset the Transform's rotation/local to its original value upon completion before Lerping again
             { "RepeatLerpRotationToT", new NodeDictionaryDefinition(new RepeatLerpRotationToTNode()                                  ,new Color(0.12f ,0.38f,0.55f)) },

              //Lerps a Transform's rotation/local Rotation to match another Transform's Rotational value about a Transform Pivot
              //and will reset the Transform's rotation/local to its original value upon completion before Lerping again
             { "RepeatLerpRotationToTAboutTPivot", new NodeDictionaryDefinition(new RepeatLerpRotationToTAboutTPivotNode()                                  ,new Color(0.12f ,0.38f,0.55f)) },

             //Lerps a Transform's rotation/local Rotation to match another Transform's Rotational value about a Vector3 Pivot
              //and will reset the Transform's rotation/local to its original value upon completion before Lerping again
             { "RepeatLerpRotationToTAboutV3Pivot", new NodeDictionaryDefinition(new RepeatLerpRotationToTAboutV3PivotNode()                                  ,new Color(0.12f ,0.38f,0.55f)) },
        #endregion

        #region Scale
		  //Lerp a Transform's scale to a vector3 value
             { "LerpScaleToV3", new NodeDictionaryDefinition(new LerpScaleToV3Node()                            ,new Color(0.14f,0.44f,0.64f)) },

             //Lerp a Transform's scale to a vector3 value about a Transform pivot
             { "LerpScaleToV3AboutTPivot", new NodeDictionaryDefinition(new LerpScaleToV3AboutTPivotNode()      ,new Color(0.14f,0.44f,0.64f)) },

             //Lerp a Transform's scale to a vector3 value about a vector3 pivot
             { "LerpScaleToV3AboutV3Pivot", new NodeDictionaryDefinition(new LerpScaleToV3AboutV3PivotNode()    ,new Color(0.14f,0.44f,0.64f)) },

             //Lerp a Transform's scale to a reference Transform's scale value 
             { "LerpScaleToT", new NodeDictionaryDefinition(new LerpScaleToTNode()    ,new Color(0.14f,0.44f,0.64f)) },

             //Lerp a Transform's scale to a refernce Transform's scale value about a Transform Pivot
             { "LerpScaleToTAboutTPivot", new NodeDictionaryDefinition(new LerpScaleToTAboutTPivotNode()    ,new Color(0.14f,0.44f,0.64f)) },

             //Lerp a Transform's scale to a refernce Transform's scale value about a Vector3 Pivot
             { "LerpScaleToTAboutV3Pivot", new NodeDictionaryDefinition(new LerpScaleToTAboutV3PivotNode()    ,new Color(0.14f,0.44f,0.64f)) },


             //RepeatScale
             //Lerp a Transform's scale to a vector3 value and reset the Transform's scale to its original value upon completion before Lerping again
             { "RepeatLerpScaleToV3", new NodeDictionaryDefinition(new RepeatLerpScaleToV3Node()                            ,new Color(0.14f,0.44f,0.64f)) },

             //Lerp a Transform's scale to a vector3 value about a Vector3 pivot and reset the Transform's scale to its original value upon completion before Lerping again
             { "RepeatLerpScaleToV3AboutV3Pivot", new NodeDictionaryDefinition(new RepeatLerpScaleToV3AboutV3PivotNode()    ,new Color(0.14f,0.44f,0.64f)) },

             //Lerp a Transform's scale to a vector3 value about a Transform pivot and reset the Transform's scale to its original value upon completion before Lerping again
             { "RepeatLerpScaleToV3AboutTPivot", new NodeDictionaryDefinition(new RepeatLerpScaleToV3AboutTPivotNode()      ,new Color(0.14f,0.44f,0.64f)) },

             //Lerp a Transform's scale to a Reference Transform's value and reset the Transform's scale to its original value upon completion before Lerping again
             { "RepeatLerpScaleToT", new NodeDictionaryDefinition(new RepeatLerpScaleToTNode()      ,new Color(0.14f,0.44f,0.64f)) },

             //Lerp a Transform's scale to a Reference Transform's value about a Transform pivot and reset the Transform's scale to its original value upon completion before Lerping again
             { "RepeatLerpScaleToTAboutTPivot", new NodeDictionaryDefinition(new RepeatLerpScaleToTAboutTPivotNode()      ,new Color(0.14f,0.44f,0.64f)) },

             //Lerp a Transform's scale to a Reference Transform's value about a Vector3 pivot and reset the Transform's scale to its original value upon completion before Lerping again
             { "RepeatLerpScaleToTAboutV3Pivot", new NodeDictionaryDefinition(new RepeatLerpScaleToTAboutV3PivotNode()      ,new Color(0.14f,0.44f,0.64f)) },

	    #endregion


        #endregion


        //Movetowards effects all uses Lerping but at a constant rate therefore on every update, the amount Lerped will be the same
        #region MoveTowards
		
        #region Position
		  //Position
             { "MoveTowRectransToPos", new NodeDictionaryDefinition(new MoveTowRectransToPosNode()              ,new Color(0.49f,0.24f,0.60f)) },
             { "MoveTowRectransToRectrans", new NodeDictionaryDefinition(new MoveTowRectransToRectransNode()    ,new Color(0.49f,0.24f,0.60f)) },
             { "MoveTowTransToPos", new NodeDictionaryDefinition(new MoveTowardsTransformToPositionNode()       ,new Color(0.49f,0.24f,0.60f)) },
             { "MoveTowTransToTrans", new NodeDictionaryDefinition(new MoveTowardsTransformToTransformNode()    ,new Color(0.49f,0.24f,0.60f)) }, 
             //RepeatPosition
             { "RepeatMoveTowTransToTrans", new NodeDictionaryDefinition(new RepeatMoveTowardsTransformToTransformNode()                ,new Color(0.36f ,0.17f,0.44f)) },
             { "RepeatMoveTowTransToPos", new NodeDictionaryDefinition(new RepeatMoveTowardsTransformToPositionNode()                   ,new Color(0.36f ,0.17f,0.44f)) },
             { "RepeatMoveTowRectransToRectrans", new NodeDictionaryDefinition(new RepeatMoveTowardsRectransformToRectransformNode()    ,new Color(0.36f ,0.17f,0.44f)) },
             { "RepeatMoveTowRectTransToPos", new NodeDictionaryDefinition(new RepeatMoveTowardsRectTransformToPositionNode()           ,new Color(0.36f ,0.17f,0.44f)) },

	    #endregion

        #region Rotation
		     //Rotation
             //Move a Transform's rotation/localRotation towards a V3 eulerAngle
             { "MoveRotationToV3", new NodeDictionaryDefinition(new MoveRotationToV3Node()                            ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's rotation/localRotation towards a V3 eulerAngle about a Transform pivot
             { "MoveRotationToV3AboutTPivot", new NodeDictionaryDefinition(new MoveRotationToV3AboutTPivotNode()      ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's rotation/localRotation towards a V3 eulerAngle about a vector3 pivot
             { "MoveRotationToV3AboutV3Pivot", new NodeDictionaryDefinition(new MoveRotationToV3AboutV3PivotNode()    ,new Color(0.49f,0.24f,0.60f)) },

             //Repeat Rotation
             //Move a Transform's rotation/localRotation towards a V3 eulerAngle and reset to its original value upon completion before repeating movetowards again
             { "RepeatMoveRotationToV3", new NodeDictionaryDefinition(new RepeatMoveRotationToV3Node()                            ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's rotation/localRotation towards a V3 eulerAngle about a Transform pivot and reset to its original value upon completion before repeating movetowards again
             { "RepeatMoveRotationToV3AboutTPivot", new NodeDictionaryDefinition(new RepeatMoveRotationToV3AboutTPivotNode()                            ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's rotation/localRotation towards a V3 eulerAngle about a Vector3 pivot and reset to its original value upon completion before repeating movetowards again
             { "RepeatMoveRotationToV3AboutV3Pivot", new NodeDictionaryDefinition(new RepeatMoveRotationToV3AboutV3PivotNode()                            ,new Color(0.49f,0.24f,0.60f)) },


             
             //Move a Transform's rotation/localRotation towards a Reference Transform's eulerAngle
             { "MoveRotationToTNode", new NodeDictionaryDefinition(new MoveRotationToTNode()                            ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's rotation/localRotation towards a Reference Transform's eulerAngle about a Transform pivot
             { "MoveRotationToTAboutTPivot", new NodeDictionaryDefinition(new MoveRotationToTAboutTPivotNode()      ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's rotation/localRotation towards a Reference Transform's eulerAngle about a vector3 pivot
             { "MoveRotationToTAboutV3Pivot", new NodeDictionaryDefinition(new MoveRotationToTAboutV3PivotNode()    ,new Color(0.49f,0.24f,0.60f)) },

             //Repeat Rotation
             //Move a Transform's rotation/localRotation towards a Reference Transform's eulerAngle and reset to its original value upon completion before repeating movetowards again
             { "RepeatMoveRotationToT", new NodeDictionaryDefinition(new RepeatMoveRotationToTNode()                            ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's rotation/localRotation towards a Reference Transform's eulerAngle about a Transform pivot and reset to its original value upon completion before repeating movetowards again
             { "RepeatMoveRotationToTAboutTPivot", new NodeDictionaryDefinition(new RepeatMoveRotationToTAboutTPivotNode()                            ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's rotation/localRotation towards a Reference Transform's eulerAngle about a Vector3 pivot and reset to its original value upon completion before repeating movetowards again
             { "RepeatMoveRotationToTAboutV3Pivot", new NodeDictionaryDefinition(new RepeatMoveRotationToTAboutV3PivotNode()                            ,new Color(0.49f,0.24f,0.60f)) },


	    #endregion

        #region Scale
		
             //Scale
             //Move a Transform's scale towards a Vector3 value
             { "MoveScaleToV3", new NodeDictionaryDefinition(new MoveScaleToV3Node()                          ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's scale towards a Vector3 value about a Transform Pivot
             { "MoveScaleToV3AboutTPivot", new NodeDictionaryDefinition(new MoveScaleToV3AboutTPivotNode()    ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's scale towards a Vector3 value about a Vector3 Pivot
             { "MoveScaleToV3AboutV3Pivot", new NodeDictionaryDefinition(new MoveScaleToV3AboutV3PivotNode()    ,new Color(0.49f,0.24f,0.60f)) },


             //Repeat Scale
             //Move a Transform's scale towards a Vector3 value and reset the Transform's value upon completion before repeating the move to
             {"RepeatMoveScaleToV3", new NodeDictionaryDefinition(new RepeatMoveScaleToV3Node()    ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's scale towards a Vector3 value about a Transform pivot and reset the Transform's value upon completion before repeating the move to
             { "RepeatMoveScaleToV3AboutTPivot", new NodeDictionaryDefinition(new RepeatMoveScaleToV3AboutTPivotNode()    ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's scale towards a Vector3 value about a Vector3 pivot and reset the Transform's value upon completion before repeating the move to
             { "RepeatMoveScaleToV3AboutV3Pivot", new NodeDictionaryDefinition(new RepeatMoveScaleToV3AboutV3PivotNode()    ,new Color(0.49f,0.24f,0.60f)) },


             //Move a Transform's scale towards a Transform's Scale value
             { "MoveScaleToT", new NodeDictionaryDefinition(new MoveScaleToTNode()                          ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's scale towards a Transform's Scale value about a Transform Pivot
             { "MoveScaleToTAboutTPivot", new NodeDictionaryDefinition(new MoveScaleToTAboutTPivotNode()    ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's scale towards a Transform's Scale value about a Vector3 Pivot
             { "MoveScaleToTAboutV3Pivot", new NodeDictionaryDefinition(new MoveScaleToTAboutV3PivotNode()    ,new Color(0.49f,0.24f,0.60f)) },


             //Repeat Scale
             //Move a Transform's scale towards a Transform's Scale value and reset the Transform's value upon completion before repeating the move to
             {"RepeatMoveScaleToT", new NodeDictionaryDefinition(new RepeatMoveScaleToTNode()    ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's scale towards a Transform's Scale value about a Transform pivot and reset the Transform's value upon completion before repeating the move to
             { "RepeatMoveScaleToTAboutTPivot", new NodeDictionaryDefinition(new RepeatMoveScaleToTAboutTPivotNode()    ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's scale towards a Transform's Scale value about a Vector3 pivot and reset the Transform's value upon completion before repeating the move to
             { "RepeatMoveScaleToTAboutV3Pivot", new NodeDictionaryDefinition(new RepeatMoveScaleToTAboutV3PivotNode()    ,new Color(0.49f,0.24f,0.60f)) },


	    #endregion
	    #endregion

        #region Translate 
             { "TranslateRelativeToTransform", new NodeDictionaryDefinition(new TranslateRelativeToTransformNode()                  ,new Color(0.302f,0.216f,0.851f)) },
             { "TranslateRelativeToSpace", new NodeDictionaryDefinition(new TranslateRelativeToSpaceNode()                          ,new Color(0.302f,0.216f,0.851f)) },
        #endregion

        #region Curve Transform Values

             //Position related
             //Displacement
             { "CurveDisplaceXTransformToPosition", new NodeDictionaryDefinition(new CurveDisplaceXTransformToPositionNode()                        ,new Color(0.302f,0.216f,0.851f)) },
             { "CurveDisplaceXRectransformToPosition", new NodeDictionaryDefinition(new CurveDisplaceXRectransformToPositionNode()                  ,new Color(0.302f,0.216f,0.851f)) },
             { "CurveDisplaceYTransformToPosition", new NodeDictionaryDefinition(new CurveDisplaceYTransformToPositionNode()                        ,new Color(0.302f,0.216f,0.851f)) },
             { "CurveDisplaceYRectransformToPosition", new NodeDictionaryDefinition(new CurveDisplaceYRectransformToPositionNode()                  ,new Color(0.302f,0.216f,0.851f)) },
             { "CurveDisplaceZTransformToPosition", new NodeDictionaryDefinition(new CurveDisplaceZTransformToPositionNode()                        ,new Color(0.302f,0.216f,0.851f)) },
             { "CurveDisplaceZRectransformToPosition", new NodeDictionaryDefinition(new CurveDisplaceZRectransformToPositionNode()                  ,new Color(0.302f,0.216f,0.851f)) },
             { "CurveDisplaceXYZTransformToPosition", new NodeDictionaryDefinition(new CurveDisplaceXYZTransformToPositionNode()                  ,new Color(0.302f,0.216f,0.851f)) },


             //Additive Effects (you can add effects one on top of the other for these few effects
             //The area under the curve/lines = Total amount of Displacement the transform has moved
             //Absolute Value of areas under the curve lines = Total amount of Distance travelled by the transform
             //Velocity
             { "CurveVelocityXRectransform", new NodeDictionaryDefinition(new CurveVelocityXRectransformNode()                  ,new Color(0.302f,0.216f,0.851f)) },
             { "CurveVelocityYRectransform", new NodeDictionaryDefinition(new CurveVelocityYRectransformNode()                  ,new Color(0.302f,0.216f,0.851f)) },
             { "CurveVelocityZRectransform", new NodeDictionaryDefinition(new CurveVelocityZRectransformNode()                  ,new Color(0.302f,0.216f,0.851f)) },
             { "CurveVelocityXTransform", new NodeDictionaryDefinition(new CurveVelocityXTransformNode()                  ,new Color(0.302f,0.216f,0.851f)) },
             { "CurveVelocityYTransform", new NodeDictionaryDefinition(new CurveVelocityYTransformNode()                  ,new Color(0.302f,0.216f,0.851f)) },
             { "CurveVelocityZTransform", new NodeDictionaryDefinition(new CurveVelocityZTransformNode()                  ,new Color(0.302f,0.216f,0.851f)) },

             //The area under the curve/lines  (+ve Area + -ve Area) = Net Gain Rotation the transform has rotated
             //Absolute Value of areas under the curve lines (|+ve Area| + |-ve Area|) =Total amount of rotation rotated by transform
             //Rate of Change in Rotation 
             { "CurveRotationXTransform", new NodeDictionaryDefinition(new CurveRotationXTransformNode()                  ,new Color(0.302f,0.216f,0.851f)) },





	    #endregion

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