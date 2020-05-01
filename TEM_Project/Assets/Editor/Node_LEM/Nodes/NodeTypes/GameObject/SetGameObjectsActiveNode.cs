using System;
using UnityEngine;
using UnityEditor;
using LEM_Effects;

namespace LEM_Editor
{

    public class SetGameObjectsActiveNode : BaseEffectNode
    {
        bool m_State = default;

        ArrayObjectDrawer<GameObject> m_GameObjectsToSet = new ArrayObjectDrawer<GameObject>();


        protected override string EffectTypeName => "SetGameObjectsActiveNode";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, topSkinColour);
            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);
        }

        public override void Draw()
        {
            base.Draw();

            Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 110f, m_MidRect.width - 20, 20f);

            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
            m_State = EditorGUI.Toggle(propertyRect,"State" ,m_State);
            propertyRect.y += 20f;

            EditorGUI.LabelField(propertyRect, "Object To Set");

            propertyRect.y += 20f;
            propertyRect.height = 15;

            //If there is change in array size, update rect
            if (m_GameObjectsToSet.HandleDrawAndProcess(propertyRect, out float propertyHeight))
            {
                SetMidRectSize(NodeTextureDimensions.NORMAL_MID_SIZE + Vector2.up * propertyHeight);
            }

            LEMStyleLibrary.EndEditorLabelColourChange();
        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            SetGameObjectsActive eff = ScriptableObject.CreateInstance<SetGameObjectsActive>();

            eff.m_Description = m_LemEffectDescription;
            eff.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, TryToSaveNextPointNodeID());

            eff.m_NodeEffectType = EffectTypeName;

            eff.m_UpdateCycle = m_UpdateCycle;

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
            m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }
    }

}