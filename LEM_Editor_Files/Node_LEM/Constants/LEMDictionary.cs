using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using LEM_Effects.Extensions;

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
             //Invoke a UnityEvent 
             { "CustomVoidFunction", new NodeDictionaryDefinition(new CustomVoidFunctionNode()                      ,new Color(0.337f, 0.396f, 0.451f)) },

	    #endregion

        #region Loading Linear Events
             
             //Play an already initialised Linear Event
             {"PlayLinearEvent", new NodeDictionaryDefinition(new PlayLinearEventNode()                               ,new Color(0.804f, 0.38f, 0.333f)) },

             //Play multiple already initialised Linear Events
             { "PlayLinearEvents", new NodeDictionaryDefinition(new PlayLinearEventsNode()                             ,new Color(0.804f, 0.38f, 0.333f)) },

             //Play with equal probability an already initialised Linear Event from a list of Linear Events
             { "PlayRandomLinearEvent", new NodeDictionaryDefinition(new PlayRandomLinearEventNode()                      ,new Color(0.804f, 0.38f, 0.333f)) },

             //Play with determined probabilities an already initialised Linear Event from a list of Linear Events
             { "PlayBiasedLinearEvent", new NodeDictionaryDefinition(new PlayBiasedLinearEventNode()                       ,new Color(0.804f, 0.38f, 0.333f)) },

             //{ "LoadLinearEvent", new NodeDictionaryDefinition(new LoadLinearEventNode()                        ,new Color(0.76f,0.15f,0.7f)) },

             //{ "LoadLinearEvents", new NodeDictionaryDefinition(new LoadLinearEventsNode()                        ,new Color(0.76f,0.15f,0.7f)) },


	    #endregion

        #region Random

             //Play the next effect randomly with equal probablity 
             { "EqualRandomOutCome", new NodeDictionaryDefinition(new EqualProbabilityOutComeNode()                        ,new Color(0.514f, 0.569f, 0.573f)) },

             //Play the next effect randomly with determined probablities
             { "BiasedRandomOutcome", new NodeDictionaryDefinition(new BiasedRandomOutcomeNode()                         ,new Color(0.514f, 0.569f, 0.573f)) },

	    #endregion

        #region GameObject

             //Instantiate a GameObject
		     { "InstantiateGameObject", new NodeDictionaryDefinition(   new InstantiateGameObjectNode()                     ,new Color(0.137f, 0.608f, 0.337f)) },

             //Destroy a GameObject
             { "DestroyGameObject", new NodeDictionaryDefinition(  new DestroyGameObjectNode()                              ,new Color(0.753f, 0.224f, 0.169f)) },

             //Destroy GameObjects
             { "DestroyGameObjects", new NodeDictionaryDefinition(  new DestroyGameObjectsNode()                            ,new Color(0.753f, 0.224f, 0.169f)) },

             //Set a GameObject's active state
             { "SetGameObjectActive", new NodeDictionaryDefinition(new SetGameObjectActiveNode()                            ,new Color(0.482f, 0.141f, 0.11f)) },

             //Set GameObjects' active state
             { "SetGameObjectsActive", new NodeDictionaryDefinition(new SetGameObjectsActiveNode()                          ,new Color(0.482f, 0.141f, 0.11f)) },

	#endregion

        #region Halt
             //Adds a delay before playing the next effect in the current Linear Event
             { "AddDelay",  new NodeDictionaryDefinition(    new AddDelayNode()                                             ,new Color(0.85f,0.64f,0.13f))},

             //Adds a delay before playing the next effect in a specific Linear Event
             { "AddDelayAt",  new NodeDictionaryDefinition(    new AddDelayAtNode()                                         ,new Color(0.85f,0.64f,0.13f))},

             //Sets the delay before playing the next effect in the current Linear Event
             { "SetDelay",  new NodeDictionaryDefinition(    new SetDelayNode()                                             ,new Color(0.85f,0.64f,0.13f))},

             //Sets the delay before playing the next effect in the current Linear Event
             { "SetDelayAt",  new NodeDictionaryDefinition(    new SetDelayAtNode()                                         ,new Color(0.85f,0.64f,0.13f))},

             //Halts the playing of effects on the current Linear Event until a certain KeyCode input condition is met
             { "AwaitKeyCodeInput",   new NodeDictionaryDefinition( new AwaitKeyCodeInputNode()                             ,new Color(0.59f,0.24f,0.75f)) },

             //Halts the playing of effects on a specific Linear Event until a certain KeyCode input condition is met
             { "AwaitKeyCodeInputAt",   new NodeDictionaryDefinition( new AwaitKeyCodeInputAtNode()                             ,new Color(0.59f,0.24f,0.75f)) },

             //Halts the playing of effects on the current Linear Event until a certain Axis input condition is met
             { "AwaitAxisInput",   new NodeDictionaryDefinition( new AwaitAxisInputNode()                                   ,new Color(0.59f,0.24f,0.75f)) },

             //Halts the playing of effects on a specific Linear Event until a certain Axis input condition is met
             { "AwaitAxisInputAt",   new NodeDictionaryDefinition( new AwaitAxisInputAtNode()                                   ,new Color(0.59f,0.24f,0.75f)) },

             //Stops any effect which requires to be updated over multiple frames
             { "StopUpdateEffect", new NodeDictionaryDefinition(new StopUpdateEffectNode()                                  ,new Color(0.922f, 0.596f, 0.306f)) },

             //Pauses all Linear Events playing in the LinearEventManager
             { "PauseAllPlayingLinearEvents", new NodeDictionaryDefinition(new PauseAllPlayingLinearEventsNode()                                   ,new Color(1f ,0.55f,0f)) },

             //Pauses the playing of effects on the current Linear Event
             { "PauseThisLinearEvent", new NodeDictionaryDefinition(new PauseThisLinearEventNode()                                      ,new Color(1f ,0.55f,0f)) },

             //Pauses the playing of effects on a specific Linear Event
             { "PauseLinearEvent", new NodeDictionaryDefinition(new PauseLinearEventNode()                                  ,new Color(1f ,0.55f,0f)) },

             //Pause the playing of effects of many specific Linear Events
             { "PauseLinearEvents", new NodeDictionaryDefinition(new PauseLinearEventsNode()                                  ,new Color(1f ,0.55f,0f)) },

             //Stops and resets the current Linear Event
             { "StopThisLinearEvent", new NodeDictionaryDefinition(new StopThisLinearEventNode()                                 ,new Color(0.839f, 0.537f, 0.063f)) },

             //Stops and resets a specific Linear Event
             { "StopLinearEvent", new NodeDictionaryDefinition(new StopLinearEventNode()                                 ,new Color(0.839f, 0.537f, 0.063f)) },   
             
             //Stops and resets LinearEvents
             { "StopLinearEvents", new NodeDictionaryDefinition(new StopLinearEventsNode()                                 ,new Color(0.839f, 0.537f, 0.063f)) },

             //Stops and resets all LinearEvents which are playing in the LinearEventManager
             { "StopAllPlayingLinearEvents", new NodeDictionaryDefinition(new StopAllPlayingLinearEventsNode()                                 ,new Color(0.839f, 0.537f, 0.063f)) },


        #endregion

        #region Animator
             //Sets an Animator's bool parameter
             { "SetAnimatorBool", new NodeDictionaryDefinition(new SetAnimatorBoolNode()                                    ,new Color(0.64f,0.55f,0.76f)) },

             //Sets an Animator's float parameter
             { "SetAnimatorFloat", new NodeDictionaryDefinition(new SetAnimatorFloatNode()                                  ,new Color(0.64f,0.55f,0.76f)) },

             //Sets an Animator's int parameter
             { "SetAnimatorInt", new NodeDictionaryDefinition(new SetAnimatorIntNode()                                      ,new Color(0.64f,0.55f,0.76f)) },

             //Sets an Animator's trigger parameter
             { "SetAnimatorTrigger", new NodeDictionaryDefinition(new SetAnimatorTriggerNode()                              ,new Color(0.64f,0.55f,0.76f)) },



	    #endregion

        #region TransformRelated Effects

        #region SetTransform
             //Sets a Transform's position value
             { "SetPosition", new NodeDictionaryDefinition(new SetPositionNode()                                            ,new Color(0.15f ,0.68f,0.38f)) },

             //Sets a RectTransform's anchoredPosition3D value
             { "SetAnchPos", new NodeDictionaryDefinition(new SetAnchPosNode()                                              ,new Color(0.15f ,0.68f,0.38f)) },

             //Sets a Transform's Scale value
             { "SetScale", new NodeDictionaryDefinition(new SetScaleNode()                                                  ,new Color(0.15f ,0.68f,0.38f)) },

             //Sets rotation of a Transform's rotation value
             { "SetRotation", new NodeDictionaryDefinition(new SetRotationNode()                                            ,new Color(0.15f ,0.68f,0.38f)) },

             //Sets width and height of a RectTransform's width and height values
             { "SetWidthHeight", new NodeDictionaryDefinition(new SetWidthHeightNode()                                      ,new Color(0.15f ,0.68f,0.38f)) },

             //Sets parent of a Transform. Setting Sibling Index to -1 will set the transform as first sibling and -2 will set the transform as last sibling
             { "SetParent", new NodeDictionaryDefinition(new SetParentNode()                                                ,new Color(0.15f ,0.68f,0.38f)) },

             //Sets Sibling index of a Transform. Setting Sibling Index to -1 will set the transform as first sibling and -2 will set the transform as last sibling
             { "SetSiblingIndex", new NodeDictionaryDefinition(new SetSiblingIndexNode()                                    ,new Color(0.15f ,0.68f,0.38f)) },

	    #endregion

             
        #region OffSet Transform

             //Adds values to the Transform's position
             { "OffsetPos"          , new NodeDictionaryDefinition(new OffsetPosNode()                                                ,new Color(0.12f ,0.52f,0.29f)) },

             //Adds values to the RectTransform's dimensions
             { "OffsetAnchPos"      , new NodeDictionaryDefinition(new OffsetAnchPosNode()                                        ,new Color(0.12f ,0.52f,0.29f)) },

             //Adds values to the Transform's rotation
             { "OffsetRotation"     , new NodeDictionaryDefinition(new OffsetRotationNode()                                      ,new Color(0.12f ,0.52f,0.29f)) },

             //Adds values to the Transform's scale
             { "OffsetScale"        , new NodeDictionaryDefinition(new OffsetScaleNode()                                            ,new Color(0.12f ,0.52f,0.29f)) },

             //Adds values to the RectTransform's width and height
             { "OffsetWidthHeight"  , new NodeDictionaryDefinition(new OffsetWidthHeightNode()                                ,new Color(0.12f ,0.52f,0.29f)) },


        #endregion

             //Note: Effects under lerp, movetowards and Curve are all non-additive effects which means that any other effects which intends to add on to the 
             //lerp/movetowards effects will result in conflicts in which one effect will dominate over the other and apply its transform value changes over the other's
        #region LERP
		
        #region Position
             //Lerps a rectTransform to a vector3 position
             { "LerpAnchPosToV3", new NodeDictionaryDefinition(new LerpAnchPosToV3Node()                                ,new Color(0.14f,0.44f,0.64f)) },

             //Lerps a rectTransform to another rectTransform's anchored3DPosition
             { "LerpAnchPosToRtT", new NodeDictionaryDefinition(new LerpAnchPosToRtTNode()                                  ,new Color(0.14f,0.44f,0.64f)) },

             //Lerps a transform to a vector3 position
             { "LerpPosToV3", new NodeDictionaryDefinition(new LerpPosToV3Node()                                            ,new Color(0.14f,0.44f,0.64f)) },

             //Lerps a transform to a transform's world position
             { "LerpPosToT", new NodeDictionaryDefinition(new LerpPosToTNode()                                              ,new Color(0.14f,0.44f,0.64f)) },



             //Lerps a transform to a transform's world position and will reset the transform's position to its original value upon completion before Lerping again
             { "RepeatLerpPosToT", new NodeDictionaryDefinition(new RepeatLerpPosToTNode()                                  ,new Color(0.12f ,0.38f,0.55f)) },

             //Lerps a transform to a vector3 position and will reset the rectTransform's anchored3DPosition to its original value upon completion before Lerping again
             { "RepeatLerpPosToV3", new NodeDictionaryDefinition(new RepeatLerpPosToV3Node()                                ,new Color(0.12f ,0.38f,0.55f)) },

             //Lerps a rectTransform to another rectTransform's anchored3DPosition and will reset the rectTransform's anchored3DPosition to its original value upon completion before Lerping again
             { "RepeatLerpAnchPosToRtT", new NodeDictionaryDefinition(new RepeatLerpAnchPosToRtTNode()                      ,new Color(0.12f ,0.38f,0.55f)) },

             //Lerps a rectTransform to another vector3 position and will reset the rectTransform's position to its original value upon completion before Lerping again
             { "RepeatLerpAnchPosToV3", new NodeDictionaryDefinition(new RepeatLerpAnchPosToV3Node()                        ,new Color(0.12f ,0.38f,0.55f)) },

        #endregion

        #region Rotation
		     //Rotation
             //In world rotation means applying the rotation to the transform's rotation property instead of localRotation
             //Note: Lerp Rotation takes in a targeted rotation and hence the TargetTransform will always take the shortest rotational path to achieve that Targeted Rotation

             //Lerps a Transform's rotation/localRotation to a V3 eulerAngle
             { "LerpRotationToV3", new NodeDictionaryDefinition(new LerpRotationToV3Node()                                  ,new Color(0.14f,0.44f,0.64f)) },

             //Lerps a Transform's rotation/localRotation to a V3 eulerAngle about a Transform Pivot
             { "LerpRotationToV3AboutTPivot", new NodeDictionaryDefinition(new LerpRotationToV3AboutTPivotNode()            ,new Color(0.14f,0.44f,0.64f)) },

             //Lerps a Transform's rotation/localRotation to a V3 eulerAngle about a V3 Pivot
             { "LerpRotationToV3AboutV3Pivot", new NodeDictionaryDefinition(new LerpRotationToV3AboutV3PivotNode()          ,new Color(0.14f,0.44f,0.64f)) },

             //Lerps a Transform's rotation/local Rotation to match another Transform's Rotational value
             { "LerpRotationToT", new NodeDictionaryDefinition(new LerpRotationToTNode()                                    ,new Color(0.14f,0.44f,0.64f)) },

             //Lerps a Transform's rotation/localRotation to match another Transform's Rotational value about a Transform Pivot
             { "LerpRotationToTAboutTPivot", new NodeDictionaryDefinition(new LerpRotationToTAboutTPivotNode()              ,new Color(0.14f,0.44f,0.64f)) },

             //Lerps a Transform's rotation/localRotation to match another Transform's Rotational value about a Vector3 Pivot
             { "LerpRotationToTAboutV3Pivot", new NodeDictionaryDefinition(new LerpRotationToTAboutV3PivotNode()            ,new Color(0.14f,0.44f,0.64f)) },



              //Lerps a Transform's rotation/localRotation to a V3 eulerAngle and will reset the Transform's rotation/local to its original value upon completion before Lerping again
             { "RepeatLerpRotationToV3", new NodeDictionaryDefinition(new RepeatLerpRotationToV3Node()                                  ,new Color(0.12f ,0.38f,0.55f)) },

              //Lerps a Transform's rotation/localRotation to a V3 eulerAngle about a Transform Pivot and will reset the Transform's rotation/local to its original value upon completion before Lerping again
             { "RepeatLerpRotationToV3AboutTPivot", new NodeDictionaryDefinition(new RepeatLerpRotationToV3AboutTPivotNode()            ,new Color(0.12f ,0.38f,0.55f)) },

              //Lerps a Transform's rotation/localRotation to a V3 eulerAngle about a V3 Pivot Pivot and will reset the Transform's rotation/local to its original value upon completion before Lerping again
             { "RepeatLerpRotationToV3AboutV3Pivot", new NodeDictionaryDefinition(new RepeatLerpRotationToV3AboutV3PivotNode()          ,new Color(0.12f ,0.38f,0.55f)) },

              //Lerps a Transform's rotation/local Rotation to match another Transform's Rotational value and will reset the Transform's rotation/local to its original value upon completion before Lerping again
             { "RepeatLerpRotationToT", new NodeDictionaryDefinition(new RepeatLerpRotationToTNode()                                    ,new Color(0.12f ,0.38f,0.55f)) },

              //Lerps a Transform's rotation/local Rotation to match another Transform's Rotational value about a Transform Pivot
              //and will reset the Transform's rotation/local to its original value upon completion before Lerping again
             { "RepeatLerpRotationToTAboutTPivot", new NodeDictionaryDefinition(new RepeatLerpRotationToTAboutTPivotNode()              ,new Color(0.12f ,0.38f,0.55f)) },

             //Lerps a Transform's rotation/local Rotation to match another Transform's Rotational value about a Vector3 Pivot
              //and will reset the Transform's rotation/local to its original value upon completion before Lerping again
             { "RepeatLerpRotationToTAboutV3Pivot", new NodeDictionaryDefinition(new RepeatLerpRotationToTAboutV3PivotNode()            ,new Color(0.12f ,0.38f,0.55f)) },
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
             { "LerpScaleToTAboutTPivot", new NodeDictionaryDefinition(new LerpScaleToTAboutTPivotNode()        ,new Color(0.14f,0.44f,0.64f)) },

             //Lerp a Transform's scale to a refernce Transform's scale value about a Vector3 Pivot
             { "LerpScaleToTAboutV3Pivot", new NodeDictionaryDefinition(new LerpScaleToTAboutV3PivotNode()      ,new Color(0.14f,0.44f,0.64f)) },


             //RepeatScale
             //Lerp a Transform's scale to a vector3 value and reset the Transform's scale to its original value upon completion before Lerping again
             { "RepeatLerpScaleToV3", new NodeDictionaryDefinition(new RepeatLerpScaleToV3Node()                            ,new Color(0.12f ,0.38f,0.55f)) },

             //Lerp a Transform's scale to a vector3 value about a Vector3 pivot and reset the Transform's scale to its original value upon completion before Lerping again
             { "RepeatLerpScaleToV3AboutV3Pivot", new NodeDictionaryDefinition(new RepeatLerpScaleToV3AboutV3PivotNode()    ,new Color(0.12f ,0.38f,0.55f)) },

             //Lerp a Transform's scale to a vector3 value about a Transform pivot and reset the Transform's scale to its original value upon completion before Lerping again
             { "RepeatLerpScaleToV3AboutTPivot", new NodeDictionaryDefinition(new RepeatLerpScaleToV3AboutTPivotNode()      ,new Color(0.12f ,0.38f,0.55f)) },

             //Lerp a Transform's scale to a Reference Transform's value and reset the Transform's scale to its original value upon completion before Lerping again
             { "RepeatLerpScaleToT", new NodeDictionaryDefinition(new RepeatLerpScaleToTNode()                              ,new Color(0.12f ,0.38f,0.55f)) },

             //Lerp a Transform's scale to a Reference Transform's value about a Transform pivot and reset the Transform's scale to its original value upon completion before Lerping again
             { "RepeatLerpScaleToTAboutTPivot", new NodeDictionaryDefinition(new RepeatLerpScaleToTAboutTPivotNode()        ,new Color(0.12f ,0.38f,0.55f)) },

             //Lerp a Transform's scale to a Reference Transform's value about a Vector3 pivot and reset the Transform's scale to its original value upon completion before Lerping again
             { "RepeatLerpScaleToTAboutV3Pivot", new NodeDictionaryDefinition(new RepeatLerpScaleToTAboutV3PivotNode()      ,new Color(0.12f ,0.38f,0.55f)) },

        #endregion


            #endregion

        //Movetowards effects all uses Lerping but at a constant rate therefore on every update, the amount Lerped will be the same
        #region MoveTowards
		
        #region Position

		     //Position
             //Move a RectTransform's anchoredPosition towards a Vector3 value
             { "MoveAnchPosToV3", new NodeDictionaryDefinition(new MoveAnchPosToV3Node()                                    ,new Color(0.49f,0.24f,0.60f)) },

             //Move a RectTransform's anchoredPosition towards another RectTransform's anchoredPosition value
             { "MoveAnchPosToRtT", new NodeDictionaryDefinition(new MoveAnchPosToRtTNode()                                  ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's position to a vector3
             { "MovePosToV3", new NodeDictionaryDefinition(new MovePosToV3Node()                                            ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's position to a vector3
             { "MovePosToT", new NodeDictionaryDefinition(new MovePosToTNode()                                              ,new Color(0.49f,0.24f,0.60f)) }, 


             //RepeatPosition

             //Move a Transform's position to another Transform's position value and reset the Transform's position to its original value upon completion before Moving again
             { "RepeatMovePosToT", new NodeDictionaryDefinition(new RepeatMovePosToTNode()                      ,new Color(0.36f ,0.17f,0.44f)) },

             //Move a Transform's position to a vector3 value and reset the Transform's position to its original value upon completion before Moving again
             { "RepeatMovePosToV3", new NodeDictionaryDefinition(new RepeatMovePosToV3Node()                    ,new Color(0.36f ,0.17f,0.44f)) },

             //Move a RectTransform's anchoredPosition towards another RectTransform anchoredPosition value and reset the rectTransform's anchoredPosition to its original value upon completion before Moving again
             { "RepeatMoveAnchPosToRtT", new NodeDictionaryDefinition(new RepeatMoveAnchPosToRtTNode()          ,new Color(0.36f ,0.17f,0.44f)) },

               //Move a RectTransform's anchoredPosition towards a Vector3 value and reset the rectTransform's anchoredPosition to its original value upon completion before Moving again
             { "RepeatMoveAnchPosToV3", new NodeDictionaryDefinition(new RepeatMoveAnchPosToV3Node()        ,new Color(0.36f ,0.17f,0.44f)) },

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
             { "RepeatMoveRotationToV3", new NodeDictionaryDefinition(new RepeatMoveRotationToV3Node()                                                  ,new Color(0.36f ,0.17f,0.44f)) },

             //Move a Transform's rotation/localRotation towards a V3 eulerAngle about a Transform pivot and reset to its original value upon completion before repeating movetowards again
             { "RepeatMoveRotationToV3AboutTPivot", new NodeDictionaryDefinition(new RepeatMoveRotationToV3AboutTPivotNode()                            ,new Color(0.36f ,0.17f,0.44f)) },

             //Move a Transform's rotation/localRotation towards a V3 eulerAngle about a Vector3 pivot and reset to its original value upon completion before repeating movetowards again
             { "RepeatMoveRotationToV3AboutV3Pivot", new NodeDictionaryDefinition(new RepeatMoveRotationToV3AboutV3PivotNode()                          ,new Color(0.36f ,0.17f,0.44f)) },

             
             //Move a Transform's rotation/localRotation towards a Reference Transform's eulerAngle
             { "MoveRotationToT", new NodeDictionaryDefinition(new MoveRotationToTNode()                        ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's rotation/localRotation towards a Reference Transform's eulerAngle about a Transform pivot
             { "MoveRotationToTAboutTPivot", new NodeDictionaryDefinition(new MoveRotationToTAboutTPivotNode()      ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's rotation/localRotation towards a Reference Transform's eulerAngle about a vector3 pivot
             { "MoveRotationToTAboutV3Pivot", new NodeDictionaryDefinition(new MoveRotationToTAboutV3PivotNode()    ,new Color(0.49f,0.24f,0.60f)) },

             //Repeat Rotation
             //Move a Transform's rotation/localRotation towards a Reference Transform's eulerAngle and reset to its original value upon completion before repeating movetowards again
             { "RepeatMoveRotationToT", new NodeDictionaryDefinition(new RepeatMoveRotationToTNode()                ,new Color(0.36f ,0.17f,0.44f)) },

             //Move a Transform's rotation/localRotation towards a Reference Transform's eulerAngle about a Transform pivot and reset to its original value upon completion before repeating movetowards again
             { "RepeatMoveRotationToTAboutTPivot", new NodeDictionaryDefinition(new RepeatMoveRotationToTAboutTPivotNode()                            ,new Color(0.36f ,0.17f,0.44f)) },

             //Move a Transform's rotation/localRotation towards a Reference Transform's eulerAngle about a Vector3 pivot and reset to its original value upon completion before repeating movetowards again
             { "RepeatMoveRotationToTAboutV3Pivot", new NodeDictionaryDefinition(new RepeatMoveRotationToTAboutV3PivotNode()                            ,new Color(0.36f ,0.17f,0.44f)) },


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
             {"RepeatMoveScaleToV3", new NodeDictionaryDefinition(new RepeatMoveScaleToV3Node()                             ,new Color(0.36f ,0.17f,0.44f)) },

             //Move a Transform's scale towards a Vector3 value about a Transform pivot and reset the Transform's value upon completion before repeating the move to
             { "RepeatMoveScaleToV3AboutTPivot", new NodeDictionaryDefinition(new RepeatMoveScaleToV3AboutTPivotNode()      ,new Color(0.36f ,0.17f,0.44f)) },

             //Move a Transform's scale towards a Vector3 value about a Vector3 pivot and reset the Transform's value upon completion before repeating the move to
             { "RepeatMoveScaleToV3AboutV3Pivot", new NodeDictionaryDefinition(new RepeatMoveScaleToV3AboutV3PivotNode()    ,new Color(0.36f ,0.17f,0.44f)) },


             //Move a Transform's scale towards a Transform's Scale value
             { "MoveScaleToT", new NodeDictionaryDefinition(new MoveScaleToTNode()                          ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's scale towards a Transform's Scale value about a Transform Pivot
             { "MoveScaleToTAboutTPivot", new NodeDictionaryDefinition(new MoveScaleToTAboutTPivotNode()    ,new Color(0.49f,0.24f,0.60f)) },

             //Move a Transform's scale towards a Transform's Scale value about a Vector3 Pivot
             { "MoveScaleToTAboutV3Pivot", new NodeDictionaryDefinition(new MoveScaleToTAboutV3PivotNode()    ,new Color(0.49f,0.24f,0.60f)) },


             //Repeat Scale
             //Move a Transform's scale towards a Transform's Scale value and reset the Transform's value upon completion before repeating the move to
             {"RepeatMoveScaleToT", new NodeDictionaryDefinition(new RepeatMoveScaleToTNode()    ,new Color(0.36f ,0.17f,0.44f)) },

             //Move a Transform's scale towards a Transform's Scale value about a Transform pivot and reset the Transform's value upon completion before repeating the move to
             { "RepeatMoveScaleToTAboutTPivot", new NodeDictionaryDefinition(new RepeatMoveScaleToTAboutTPivotNode()   ,new Color(0.36f ,0.17f,0.44f)) },

             //Move a Transform's scale towards a Transform's Scale value about a Vector3 pivot and reset the Transform's value upon completion before repeating the move to
             { "RepeatMoveScaleToTAboutV3Pivot", new NodeDictionaryDefinition(new RepeatMoveScaleToTAboutV3PivotNode()    ,new Color(0.36f ,0.17f,0.44f)) },


	    #endregion
	    #endregion

        #region Translate 
             //Translate a Transform in a direction and distance relative to another Transform
             { "TranslateRelativeToTransform", new NodeDictionaryDefinition(new TranslateRelativeToTransformNode()                  ,new Color(0.302f,0.216f,0.851f)) },

             //Translate a Transform in a direction and distance relative to itself or the world coordinates
             { "TranslateRelativeToSpace", new NodeDictionaryDefinition(new TranslateRelativeToSpaceNode()                          ,new Color(0.302f,0.216f,0.851f)) },
        #endregion

        #region Curve Transform Values

             
        #region Displacement
		     //CurvePos and CurveAnchPos effects are not additive. In other words, you can't add CurvePosX and CurvePosY together to expect them to move the transform in both the X and Y direction.

             //Control the value of a transform's X position with an AnimationCurve
             { "CurvePosX", new NodeDictionaryDefinition(new CurvePosXNode()                        ,new Color(0.18f, 0.251f, 0.325f)) },

             //Control the value of a RectTransform's X anchoredPosition3D with an AnimationCurve
             { "CurveAnchPosX", new NodeDictionaryDefinition(new CurveAnchPosXNode()                  ,new Color(0.18f, 0.251f, 0.325f)) },

             //Control the value of a transform's Y position with an AnimationCurve
             { "CurvePosY", new NodeDictionaryDefinition(new CurvePosYNode()                        ,new Color(0.18f, 0.251f, 0.325f)) },

             //Control the value of a RectTransform's Y anchoredPosition3D with an AnimationCurve
             { "CurveAnchPosY", new NodeDictionaryDefinition(new CurveAnchPosYNode()                  ,new Color(0.18f, 0.251f, 0.325f)) },

             //Control the value of a transform's Z position with an AnimationCurve
             { "CurvePosZ", new NodeDictionaryDefinition(new CurvePosZNode()                        ,new Color(0.18f, 0.251f, 0.325f)) },

             //Control the value of a RectTransform's Z anchoredPosition3D with an AnimationCurve
             { "CurveAnchPosZ", new NodeDictionaryDefinition(new CurveAnchPosZNode()                 ,new Color(0.18f, 0.251f, 0.325f)) },

             //Control the value of a transform's XYZ position with an AnimationCurve
             { "CurvePosXYZ", new NodeDictionaryDefinition(new CurvePosXYZNode()                 ,new Color(0.18f, 0.251f, 0.325f)) },

             //Control the value of a RectTransform's XYZ anchoredPosition3D with an AnimationCurve
             { "CurveAnchPosXYZ", new NodeDictionaryDefinition(new CurveAnchPosXYZNode()                  ,new Color(0.18f, 0.251f, 0.325f)) },

        #endregion

        #region Velocity

             //Additive Effects (you can add effects one on top of the other for a combination) 
             //Eg. Applying CurveRateOfChangeAnchPosX + CurveRateOfChangeAnchPosY will result in Applying rate of change of a RectTransform's x and y values of AnchoredPosition3D
             //The area under the curve/lines = Total amount of Displacement the transform has moved
             //Absolute Value of areas under the curve lines = Total amount of Distance travelled by the transform

             //Control the rate of change of a RectTransform's AnchoredPosition X value over time with an AnimationCurve
             { "CurveRateOfChangeAnchPosX", new NodeDictionaryDefinition(new CurveRateOfChangeAnchPosXNode()                  ,new Color(0.129f, 0.184f, 0.239f)) },

             //Control the rate of change of a RectTransform's AnchoredPosition Y value over time with an AnimationCurve
             { "CurveRateOfChangeAnchPosY", new NodeDictionaryDefinition(new CurveRateOfChangeAnchPosYNode()                  ,new Color(0.129f, 0.184f, 0.239f)) },

             //Control the rate of change of a RectTransform's AnchoredPosition Z value over time with an AnimationCurve
             { "CurveRateOfChangeAnchPosZ", new NodeDictionaryDefinition(new CurveRateOfChangeAnchPosZNode()                  ,new Color(0.129f, 0.184f, 0.239f)) },

             //Control the rate of change of a Transform's Position X value over time with an AnimationCurve
             { "CurveRateOfChangePosX", new NodeDictionaryDefinition(new CurveRateOfChangePosXNode()                 ,new Color(0.129f, 0.184f, 0.239f)) },

             //Control the rate of change of a Transform's Position Y value over time with an AnimationCurve
             { "CurveRateOfChangePosY", new NodeDictionaryDefinition(new CurveRateOfChangePosYNode()                 ,new Color(0.129f, 0.184f, 0.239f)) },

             //Control the rate of change of a Transform's Position Z value over time with an AnimationCurve
             { "CurveRateOfChangePosZ", new NodeDictionaryDefinition(new CurveRateOfChangePosZNode()                 ,new Color(0.129f, 0.184f, 0.239f)) },



        #endregion


        #region Rotation Over Time
             //CurveRotation effects are not additive. In other words, you can't add CurveRotationX and CurveRotationY together to expect them to rotate in both X and Y axis

             //Control the value of a transform's X rotation/localRotation with an AnimationCurve
             { "CurveRotationX", new NodeDictionaryDefinition(new CurveRotationXNode()                  ,new Color(0.18f, 0.251f, 0.325f)) },

             //Control the value of a transform's Y rotation/localRotation with an AnimationCurve
             { "CurveRotationY", new NodeDictionaryDefinition(new CurveRotationYNode()                  ,new Color(0.18f, 0.251f, 0.325f)) },

             //Control the value of a transform's Z rotation/localRotation with an AnimationCurve
             { "CurveRotationZ", new NodeDictionaryDefinition(new CurveRotationZNode()                  ,new Color(0.18f, 0.251f, 0.325f)) },

             //Control the value of a transform's XYZ rotation/localRotation with an AnimationCurve
             { "CurveRotationXYZ", new NodeDictionaryDefinition(new CurveRotationXYZNode()                  ,new Color(0.18f, 0.251f, 0.325f)) },


        #endregion
             
        #region Rate of Change Rotation

             //Additive Effects (you can add effects one on top of the other for a combination) 
             //Eg. Applying CurveRateOfChangeRotationX + CurveRateOfChangeRotationY will result in Applying rate of change of a Trasform's x and y values of Rotation
		     //The area under the curve/lines  (+ve Area + -ve Area) = Net Gain Rotation the transform has rotated
             //Absolute Value of areas under the curve lines (|+ve Area| + |-ve Area|) =Total amount of rotation rotated by transform

             //Control the rate of change of a Transform's Rotation X value over time with an AnimationCurve
             { "CurveRateOfChangeRotationX", new NodeDictionaryDefinition(new CurveRateOfChangeRotationXNode()                  ,new Color(0.129f, 0.184f, 0.239f)) },

             //Control the rate of change of a Transform's Rotation Y value over time with an AnimationCurve
             { "CurveRateOfChangeRotationY", new NodeDictionaryDefinition(new CurveRateOfChangeRotationYNode()                  ,new Color(0.129f, 0.184f, 0.239f)) },

             //Control the rate of change of a Transform's Rotation Z value over time with an AnimationCurve
             { "CurveRateOfChangeRotationZ", new NodeDictionaryDefinition(new CurveRateOfChangeRotationZNode()                  ,new Color(0.129f, 0.184f, 0.239f)) },


	    #endregion


        #region Scale Over Time
             //CurveScale effects are not additive. In other words, you can't add CurveScaleX and CurveScaleY together to expect them to scale in respect to both X and Y axis

             //Control the x value of a Transform's scale using an AnimationCurve
             { "CurveScaleX", new NodeDictionaryDefinition(new CurveScaleXNode()                  ,new Color(0.18f, 0.251f, 0.325f)) },

             //Control the y value of a Transform's scale using an AnimationCurve
             { "CurveScaleY", new NodeDictionaryDefinition(new CurveScaleYNode()                  ,new Color(0.18f, 0.251f, 0.325f)) },

             //Control the z value of a Transform's scale using an AnimationCurve
             { "CurveScaleZ", new NodeDictionaryDefinition(new CurveScaleZNode()                  ,new Color(0.18f, 0.251f, 0.325f)) },

              //Control the xyz value of a Transform's scale using an AnimationCurve
             { "CurveScaleXYZ", new NodeDictionaryDefinition(new CurveScaleXYZNode()                  ,new Color(0.18f, 0.251f, 0.325f)) },

        #endregion


        #region Rate of Change Scale

             //Additive Effects (you can add effects one on top of the other for a combination) 
             //Eg. Applying CurveRateOfChangeScaleX + CurveRateOfChangeScaleY will result in Applying rate of change of a Trasform's x and y values of Scale
		     //The area under the curve/lines  (+ve Area + -ve Area) = Net Gain of Scale the transform has been received
             //Absolute Value of areas under the curve lines (|+ve Area| + |-ve Area|) =Total amount of change in scale by transform

             //Control the rate of change of a Transform's Scale X value over time with an AnimationCurve
             { "CurveRateOfChangeScaleX", new NodeDictionaryDefinition(new CurveRateOfChangeScaleXNode()                  ,new Color(0.129f, 0.184f, 0.239f)) },

             //Control the rate of change of a Transform's Scale Y value over time with an AnimationCurve
             { "CurveRateOfChangeScaleY", new NodeDictionaryDefinition(new CurveRateOfChangeScaleYNode()                  ,new Color(0.129f, 0.184f, 0.239f)) },

             //Control the rate of change of a Transform's Scale Z value over time with an AnimationCurve
             { "CurveRateOfChangeScaleZ", new NodeDictionaryDefinition(new CurveRateOfChangeScaleZNode()                  ,new Color(0.129f, 0.184f, 0.239f)) },

	    #endregion



        #endregion


        #endregion


        #region Fade Alpha
             //Fade Alpha effects fade various classes with the color property

             //Fades Alpha of the Graphic (Image, Text , TextMeshPro, TextMeshProGUI or anything that inherits from Graphic class) to a target alpha within a given duration
             { "FadeToAlphaGraphic", new NodeDictionaryDefinition(new FadeToAlphaGraphicNode()                               ,new Color(0.102f, 0.737f, 0.612f)) },

             //Fades Alpha of the Graphics (Image, Text , TextMeshPro, TextMeshProGUI or anything that inherits from Graphic class) to a target alpha within a given duration
             { "FadeToAlphaGraphics", new NodeDictionaryDefinition(new FadeToAlphaGraphicsNode()                               ,new Color(0.102f, 0.737f, 0.612f)) },

             //Fades the Alpha of CanvasGroup to a target alpha within a given duration
             { "FadeToAlphaCanvasGroup", new NodeDictionaryDefinition(new FadeToAlphaCanvasGroupNode()                                      ,new Color(0.102f, 0.737f, 0.612f)) },

             //Fades the Alpha of CanvasGroups to a target alpha within a given duration
             { "FadeToAlphaCanvasGroups", new NodeDictionaryDefinition(new FadeToAlphaCanvasGroupsNode()                                     ,new Color(0.102f, 0.737f, 0.612f)) },

             //Fades the Alpha of Renderer to a target alpha within a given duration
             { "FadeToAlphaRenderer", new NodeDictionaryDefinition(new FadeToAlphaRendererNode()                               ,new Color(0.078f, 0.561f, 0.467f)) },

             //Fades the Alpha of Renderers to a target alpha within a given duration
             { "FadeToAlphaRenderers", new NodeDictionaryDefinition(new FadeToAlphaRenderersNode()                             ,new Color(0.078f, 0.561f, 0.467f)) },

             //Fades the Alpha of SpriteRenderer to a target alpha within a given duration
             { "FadeToAlphaSpriteRenderer", new NodeDictionaryDefinition(new FadeToAlphaSpriteRendererNode()                   ,new Color(0.078f, 0.561f, 0.467f)) },

             //Fades the Alpha of SpriteRenderers to a target alpha within a given duration
             { "FadeToAlphaSpriteRenderers", new NodeDictionaryDefinition(new FadeToAlphaSpriteRenderersNode()                 ,new Color(0.078f, 0.561f, 0.467f)) },

             //Fades the Alpha of TextMesh to a target alpha within a given duration
             { "FadeToAlphaTextMesh", new NodeDictionaryDefinition(new  FadeToAlphaTextMeshNode()                                       ,new Color(0.102f, 0.737f, 0.612f)) },

             //Fades the Alpha of TextMeshes to a target alpha within a given duration
             { "FadeToAlphaTextMeshes", new NodeDictionaryDefinition(new FadeToAlphaTextMeshesNode()                                      ,new Color(0.102f, 0.737f, 0.612f)) },


	    #endregion

        #region Curve Alpha
             //Curves the Alpha of a Graphic to a target alpha within a given duration
             { "CurveAlphaToGraphic", new NodeDictionaryDefinition(new  CurveAlphaToGraphicNode()                                      ,new Color(0.075f, 0.553f, 0.459f)) },

             //Curves the Alpha of Graphics to a target alpha within a given duration
             { "CurveAlphaToGraphics", new NodeDictionaryDefinition(new  CurveAlphaToGraphicsNode()                                      ,new Color(0.075f, 0.553f, 0.459f)) },

             //Curves the Alpha of Renderer to a target alpha within a given duration
             { "CurveAlphaToRenderer", new NodeDictionaryDefinition(new  CurveAlphaToRendererNode()                                      ,new Color(0.055f, 0.4f, 0.333f)) },

             //Curves the Alpha of Renderers to a target alpha within a given duration
             { "CurveAlphaToRenderers", new NodeDictionaryDefinition(new  CurveAlphaToRenderersNode()                                      ,new Color(0.055f, 0.4f, 0.333f)) },

             //Curves the Alpha of SpriteRenderer to a target alpha within a given duration
             { "CurveAlphaToSpriteRenderer", new NodeDictionaryDefinition(new  CurveAlphaToSpriteRendererNode()                                     ,new Color(0.055f, 0.4f, 0.333f)) },

             //Curves the Alpha of SpriteRenderers to a target alpha within a given duration
             { "CurveAlphaToSpriteRenderers", new NodeDictionaryDefinition(new  CurveAlphaToSpriteRenderersNode()                                      ,new Color(0.055f, 0.4f, 0.333f)) },

             //Curves the Alpha of TextMesh to a target alpha within a given duration
             { "CurveAlphaToTextMesh", new NodeDictionaryDefinition(new  CurveAlphaToTextMeshNode()                                      ,new Color(0.075f, 0.553f, 0.459f)) },

             //Curves the Alpha of TextMeshes to a target alpha within a given duration
             { "CurveAlphaToTextMeshes", new NodeDictionaryDefinition(new  CurveAlphaToTextMeshesNode()                                      ,new Color(0.075f, 0.553f, 0.459f)) },

             //Curves the Alpha of CanvasGroup to a target alpha within a given duration
             { "CurveAlphaToCanvasGroup", new NodeDictionaryDefinition(new  CurveAlphaToCanvasGroupNode()                                      ,new Color(0.075f, 0.553f, 0.459f)) },

             //Curves the Alpha of CanvasGroups to a target alpha within a given duration
             { "CurveAlphaToCanvasGroups", new NodeDictionaryDefinition(new  CurveAlphaToCanvasGroupsNode()                                      ,new Color(0.075f, 0.553f, 0.459f)) },
	#endregion

        #region Reword
             //Sets the text of the different Text Classes to a new text

             //Sets the text of a Text class to a new text
             { "ReWordText", new NodeDictionaryDefinition(new ReWordTextNode()                             ,new Color(0.863f, 0.463f, 0.2f)) },

              //Sets the text of a TextMesh class to a new text
             { "ReWordTextMesh", new NodeDictionaryDefinition(new ReWordTextMeshNode()                      ,new Color(0.863f, 0.463f, 0.2f)) },

             //Sets the text of a TextMeshPro, TextMeshProGUI or any class that inherits from TMP_Text class to a new text
             { "ReWordTextMeshPro", new NodeDictionaryDefinition(new ReWordTextMeshProNode()               ,new Color(0.863f, 0.463f, 0.2f)) },

	#endregion


        };

        public static void LoadDictionary()
        {
            List<string> temp = s_NodeTypesDictionary.Keys.ToList();

            //Remove startnode
            temp.RemoveEfficiently(0);
            //Sort everything into alphabetical order
            temp.Sort();

            //Debug.Log("There are " + temp.Count + " effects here");

            s_NodeTypeKeys = temp.ToArray();

        }

        //Return a object prefab to instantiate
        public static object GetNodeObject(string nodeObjectType)
        {
            s_NodeTypesDictionary.TryGetValue(nodeObjectType, out NodeDictionaryDefinition value);
            //Debug.Log("Node Object Type " + nodeObjectType);
            //Debug.Log(" Value = " + value.m_Type);
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