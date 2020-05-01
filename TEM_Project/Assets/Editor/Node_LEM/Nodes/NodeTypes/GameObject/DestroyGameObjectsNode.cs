using LEM_Effects;
using System;
using UnityEngine;
using UnityEditor;
namespace LEM_Editor
{

    public class DestroyGameObjectsNode : BaseEffectNode
    {
        int m_ArraySize = default;
    
        GameObject[] m_ObjectsToDestroy = default;

        protected override string EffectTypeName => "DestroyGameObjectsNode";

        public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Action<NodeDictionaryStruct> updateEffectNodeInDictionary, Color topSkinColour)
        {
            base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, updateEffectNodeInDictionary, topSkinColour);

            //Override the rect size n pos
            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);

        }

        public override void Draw()
        {
            base.Draw();


            //Draw a object field for inputting  the gameobject to destroy
            Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 110f, m_MidRect.width - 20, 20f);

            LEMStyleLibrary.s_GUIPreviousColour = GUI.skin.label.normal.textColor;
            GUI.skin.label.normal.textColor = LEMStyleLibrary.s_CurrentLabelColour;
            GUI.Label(propertyRect, "Object To Destroy");
            GUI.skin.label.normal.textColor = LEMStyleLibrary.s_GUIPreviousColour;

            propertyRect.y += 20f;
            propertyRect.height = 15;
            LEMStyleLibrary.s_GUIPreviousColour = EditorStyles.label.normal.textColor;
            EditorStyles.label.normal.textColor = LEMStyleLibrary.s_CurrentLabelColour;
            m_ArraySize = EditorGUI.IntField(propertyRect, "Size", m_ArraySize);
            EditorStyles.label.normal.textColor = LEMStyleLibrary.s_GUIPreviousColour;


            //m_ObjectsToDestroy = (GameObject[])EditorGUI.ObjectField(propertyRect, "", m_ObjectsToDestroy, typeof(GameObject[]), true);



        }

        public override LEM_BaseEffect CompileToBaseEffect()
        {
            DestroyGameObjects myEffect = ScriptableObject.CreateInstance<DestroyGameObjects>();
            myEffect.m_NodeEffectType = EffectTypeName;

            myEffect.m_Description = m_LemEffectDescription;
            myEffect.m_UpdateCycle = m_UpdateCycle;


            string[] connectedNextPointNodeIDs = TryToSaveNextPointNodeID();

            myEffect.m_NodeBaseData = new NodeBaseData(m_MidRect.position, NodeID, connectedNextPointNodeIDs);
            myEffect.SetUp(m_ObjectsToDestroy);
            return myEffect;

        }

        public override void LoadFromBaseEffect(LEM_BaseEffect effectToLoadFrom)
        {
            DestroyGameObjects loadFrom = effectToLoadFrom as DestroyGameObjects;
            loadFrom.UnPack(out m_ObjectsToDestroy);

            //Important
            m_LemEffectDescription = effectToLoadFrom.m_Description;
            m_UpdateCycle = effectToLoadFrom.m_UpdateCycle;

        }
    }

}