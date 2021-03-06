using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;

namespace LEM_Editor
{

    public class SetGameObjectsActiveNode : InstantEffectNode
    {
        bool m_State = default;

        ArrayObjectDrawer<GameObject> m_GameObjectsToSet = new ArrayObjectDrawer<GameObject>();


        protected override string EffectTypeName => "SetGameObjectsActive";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint,
            Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<string> onDeSelectNode, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, topSkinColour);
            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);
        }

        public override void Draw()
        {
            base.Draw();

            //Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 110f, m_MidRect.width - 20, 20f);
            Rect propertyRect = new Rect(m_MidRect.x + NodeGUIConstants.X_DIST_FROM_MIDRECT, m_MidRect.y + NodeGUIConstants.INSTANT_EFFNODE_Y_DIST_FROM_MIDRECT, m_MidRect.width - NodeGUIConstants.MIDRECT_WIDTH_OFFSET, EditorGUIUtility.singleLineHeight);

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.CurrentLabelColour);
            m_State = EditorGUI.Toggle(propertyRect,"State" ,m_State);
            propertyRect.y += 20f;

            EditorGUI.LabelField(propertyRect, "Object To Set");

            propertyRect.y += 20f;

            //If there is change in array size, update rect
            if (m_GameObjectsToSet.HandleDrawAndProcess(propertyRect, out float propertyHeight))
            {
                SetMidRectSize(NodeTextureDimensions.NORMAL_MID_SIZE + Vector2.up * propertyHeight);
            }

            LEMStyleLibrary.EndEditorLabelColourChange();
        }

        public override LEM_BaseEffect CompileToBaseEffect(GameObject go)
        {
            SetGameObjectsActive eff = go.AddComponent<SetGameObjectsActive>();
            //SetGameObjectsActive eff = ScriptableObject.CreateInstance<SetGameObjectsActive>();

          //  eff.m_Description = m_LemEffectDescription;
            eff.bm_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, TryToSaveNextPointNodeID());

            eff.bm_NodeEffectType = EffectTypeName;

            eff.bm_UpdateCycle = m_UpdateCycle;

            eff.SetUp(m_GameObjectsToSet.GetObjectArray(), m_State);

            return eff;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            SetGameObjectsActive loadFrom = effectToLoadFrom as SetGameObjectsActive;
            loadFrom.UnPack(out GameObject[] t1, out m_State);
            m_GameObjectsToSet.SetObjectArray(t1, out float changeInRectHeight);
            SetMidRectSize(NodeTextureDimensions.NORMAL_MID_SIZE + Vector2.up * changeInRectHeight);

            //Important
            //m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.bm_UpdateCycle;

        }
    }

}