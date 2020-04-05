//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//public class RandomOutComeNode : BaseEffectNode
//{
//    public int m_SizeOfOutComes = 2;
//    public List<float> m_OutComeProbabilities = new List<float>() { 0.5f,0.5f};
//    public List<string> m_NamesOfOutCome = new List<string>() { "OutCome 0", "OutCome 1" };

//    public override void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Color midSkinColour)
//    {
//        base.Initialise(position, nodeSkin, connectionPointStyle, onClickInPoint, onClickOutPoint, onSelectNode, onDeSelectNode, midSkinColour);

//        //Override the rect size n pos
//        SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);
//    }

//    public override void Draw()
//    {
//        #region Node class's Draw function

//        if (m_IsSelected)
//        {
//            GUI.DrawTexture(new Rect(
//                m_TotalRect.x - NodeTextureDimensions.EFFECT_NODE_OUTLINE_OFFSET.x,
//                m_TotalRect.y - NodeTextureDimensions.EFFECT_NODE_OUTLINE_OFFSET.y,
//                m_TotalRect.width * 1.075f, m_TotalRect.height * 1.075f),
//                m_NodeSkin.m_SelectedMidOutline);
//        }

//        LEMStyleLibrary.s_GUIPreviousColour = GUI.color;

//        //Draw the top of the node
//        GUI.color = LEMStyleLibrary.s_CurrentTopTextureColour;
//        GUI.DrawTexture(m_TopRect, m_NodeSkin.m_TopBackground, ScaleMode.StretchToFill);

//        //Draw the node midskin with its colour
//        GUI.color = m_MidSkinColour;
//        GUI.DrawTexture(m_MidRect, m_NodeSkin.m_MidBackground, ScaleMode.StretchToFill);
//        GUI.color = LEMStyleLibrary.s_GUIPreviousColour;

//        //Draw the in out points as well
//        m_InPoint.Draw();


//        #endregion

//        #region BaseEffecNode class's Draw function
//        GUI.Label(new Rect(m_MidRect.x + 10, m_MidRect.y + 35, m_MidRect.width, 30f), "Description", LEMStyleLibrary.s_NodeParagraphStyle);
//        GUI.Label(m_TopRect, m_Title, LEMStyleLibrary.s_NodeHeaderStyle);

//        //Draw the description text field
//        m_LemEffectDescription = EditorGUI.TextArea(new Rect(m_MidRect.x + 10f, m_MidRect.y + 50f, m_MidRect.width - 20f, 40f), m_LemEffectDescription, LEMStyleLibrary.s_NodeTextInputStyle);
//        #endregion


//        #region This Class's Draw Function

//        Rect propertyRect = new Rect(m_MidRect.x + 10, m_MidRect.y + 100f, m_MidRect.width - 50, 15f);

//        //Draw Size int field
//        m_SizeOfOutComes = EditorGUI.IntField(propertyRect, m_SizeOfOutComes);

//        for (int i = 0; i < m_SizeOfOutComes; i++)
//        {
//            if(m_NamesOfOutCome[i] != null)
//            {
//                //Draw text field
//                m_NamesOfOutCome[i] = EditorGUI.TextField(propertyRect, m_NamesOfOutCome[i]);
//                //Increment rect pos
//                propertyRect.y += 15;
//                m_OutComeProbabilities[i] = EditorGUI.FloatField(propertyRect, m_OutComeProbabilities[i]);
//                propertyRect.y += 20;
//            }
//            else
//            {

//            }
           

//        }

//        #endregion

//    }

//}
