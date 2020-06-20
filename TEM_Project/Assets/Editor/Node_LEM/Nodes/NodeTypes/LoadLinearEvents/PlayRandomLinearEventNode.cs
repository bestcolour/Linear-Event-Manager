using LEM_Effects;
using System;
using UnityEngine;
using UnityEditor;
namespace LEM_Editor
{

    public class PlayRandomLinearEventNode : InstantEffectNode
    {
        LinearEventObjectDrawer m_ArrayOfGameObjects = new LinearEventObjectDrawer();

        protected override string EffectTypeName => "PlayRandomLinearEvent";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<BaseEffectNodePair> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.SMALL_MID_SIZE, NodeTextureDimensions.SMALL_TOP_SIZE);

        }

        public override void Draw()
        {
            base.Draw();


            //Draw a object field for inputting  the gameobject to destroy
            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.INSTANT_EFFNODE_Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUIUtility.singleLineHeight);

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.CurrentLabelColour);
            EditorGUI.LabelField(propertyRect, "Linear Events To Play");

            propertyRect.y += 20f;

            //If there is change in array size, update rect
            if (m_ArrayOfGameObjects.HandleDrawAndProcess(propertyRect, out float propertyHeight))
            {
                SetMidRectSize(NodeTextureDimensions.SMALL_MID_SIZE + Vector2.up * propertyHeight);
            }

            LEMStyleLibrary.EndEditorLabelColourChange();

        }

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            PlayRandomLinearEvent myEffect = go.AddComponent<PlayRandomLinearEvent>();
            //LoadRandomLinearEvent myEffect = ScriptableObject.CreateInstance<LoadRandomLinearEvent>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs);
            myEffect.SetUp(m_ArrayOfGameObjects.GetObjectArray());
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            PlayRandomLinearEvent loadFrom = effectToLoadFrom as PlayRandomLinearEvent;
            loadFrom.UnPack(out LinearEvent[] t1);
            m_ArrayOfGameObjects.SetObjectArray(t1, out float changeInRectHeight);
            SetMidRectSize(NodeTextureDimensions.SMALL_MID_SIZE + Vector2.up * changeInRectHeight);

            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }
    }

}