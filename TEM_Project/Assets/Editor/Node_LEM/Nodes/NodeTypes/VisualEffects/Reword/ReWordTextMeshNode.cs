using UnityEngine;
using UnityEditor;
using System;
using LEM_Effects;
namespace LEM_Editor
{

    public class ReWordTextMeshNode : InstantEffectNode
    {
        TextMesh m_TargetText = default;

        string m_NewString = default;

        protected override string EffectTypeName => "ReWordTextMesh";

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
            // EditorGUI.TextField(propertyRect, "Node ID : ", NodeID);
            m_TargetText = (TextMesh)EditorGUI.ObjectField(propertyRect, "Target Text", m_TargetText, typeof(TextMesh), true);
            propertyRect.y += 20f;
            m_NewString = EditorGUI.TextField(propertyRect, "New Text", m_NewString);



            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            ReWordTextMesh myEffect = ScriptableObject.CreateInstance<ReWordTextMesh>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs/*, connectedPrevPointNodeIDs*/);
            myEffect.SetUp(m_TargetText, m_NewString);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            ReWordTextMesh loadFrom = effectToLoadFrom as ReWordTextMesh;
            loadFrom.UnPack(out m_TargetText, out m_NewString);
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }
    }

}