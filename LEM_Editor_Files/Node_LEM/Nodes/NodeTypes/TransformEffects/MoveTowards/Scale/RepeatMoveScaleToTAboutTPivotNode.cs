using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;
namespace LEM_Editor
{

    public class RepeatMoveScaleToTAboutTPivotNode : UpdateEffectNode
    {
        [SerializeField]
        Transform m_TargetedTransform = default;

        [SerializeField]
        Transform m_ReferenceTransform = default;

        [SerializeField]
        Transform m_Pivot = default;

        [SerializeField]
        float m_Duration = 0f;

        protected override string EffectTypeName => "RepeatMoveScaleToTAboutTPivot";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Action<BaseEffectNodePair> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.BIG_MID_SIZE, NodeTextureDimensions.BIG_TOP_SIZE);
        }

        public override void Draw()
        {
            base.Draw();

            //Draw a object field for inputting  the gameobject to destroy
            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.UPDATE_EFFNODE_Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUIUtility.singleLineHeight);

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.CurrentLabelColour);

            m_TargetedTransform = (Transform)EditorGUI.ObjectField(propertyRect, "Targeted Transform", m_TargetedTransform, typeof(Transform), true);
            propertyRect.y += 20f;
            m_ReferenceTransform = (Transform)EditorGUI.ObjectField(propertyRect, "Reference Transform", m_ReferenceTransform, typeof(Transform), true);
            propertyRect.y += 20f;
            m_Pivot = (Transform)EditorGUI.ObjectField(propertyRect, "Pivot Transform", m_Pivot, typeof(Transform), true);
            propertyRect.y += 20f;
            m_Duration = EditorGUI.FloatField(propertyRect, "Duration", m_Duration);


            LEMStyleLibrary.EndEditorLabelColourChange();


        }

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            RepeatMoveScaleToTAboutTPivot myEffect = go.AddComponent<RepeatMoveScaleToTAboutTPivot>();
            //RepeatMoveScaleToTAboutTPivot myEffect = ScriptableObject.CreateInstance<RepeatMoveScaleToTAboutTPivot>();
            myEffect.bm_NodeEffectType = EffectTypeName;

            //myEffect.m_Description = m_LemEffectDescription;
            myEffect.bm_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs);
            myEffect.SetUp(m_TargetedTransform, m_ReferenceTransform, m_Pivot, m_Duration);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            RepeatMoveScaleToTAboutTPivot loadFrom = effectToLoadFrom as RepeatMoveScaleToTAboutTPivot;
            loadFrom.UnPack(out m_TargetedTransform, out m_ReferenceTransform, out m_Pivot, out m_Duration);

            //Important
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }

    }

}