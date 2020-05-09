using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
namespace LEM_Editor
{

    public class GroupRectNode : Node
    {
        public static void CalculateGroupRectPosition(Rect[] rects, out Vector2 startVector2Pos, out Vector2 endVector2Pos)
        {
            Vector2 smallestV2 = Vector2.one * float.MaxValue;
            Vector2 biggestV2 = Vector2.one * float.MinValue;
            for (int i = 0; i < rects.Length; i++)
            {
                smallestV2.x = Mathf.Min(rects[i].x, smallestV2.x);
                smallestV2.y = Mathf.Min(rects[i].y, smallestV2.y);

                biggestV2.x = Mathf.Max(rects[i].x + rects[i].width, biggestV2.x);
                biggestV2.y = Mathf.Max(rects[i].y + rects[i].height, biggestV2.y);
            }

            //return avrgSmallestV2;
            //Substract to add a border
            smallestV2 += -Vector2.one * NodeGUIConstants.k_GroupRectExtraBufferSpace;
            startVector2Pos = smallestV2;

            //Add thisborder 
            biggestV2 += Vector2.one * NodeGUIConstants.k_GroupRectExtraBufferSpace;
            endVector2Pos = biggestV2;
        }

        string m_CommentLabel = default;

        Dictionary<string, Node> m_NestedNodesDictionary = new Dictionary<string, Node>();
        string[] NestedNodesKeys => m_NestedNodesDictionary.Keys.ToArray();

        public override NodeType BaseNodeType => NodeType.GroupRectNode;

        Rect m_BorderRect = new Rect();

        public void Initialise(Vector2 startRectPos, Vector2 size, Node[] nestedNodes, NodeSkinCollection nodeSkin, Action<Node> onSelectNode, Action<Node> onDeSelectNode, Color topSkinColour)
        {
            m_TopRect = new Rect();

            for (int i = 0; i < nestedNodes.Length; i++)
            {
                nestedNodes[i].m_GroupedParent = this;
                m_NestedNodesDictionary.Add(nestedNodes[i].NodeID, nestedNodes[i]);
            }

            Vector2 topSize = new Vector2(size.x, NodeTextureDimensions.NORMAL_TOP_SIZE.y);
            SetNodeRects(startRectPos, size, topSize);

            this.m_NodeSkin = nodeSkin;

            //Assign delegates
            this.d_OnSelectNode = onSelectNode;
            this.d_OnDeselectNode = onDeSelectNode;

            m_TopSkinColour = topSkinColour;
        }

        protected override void SetNodeRects(Vector2 startRectPos, Vector2 midSize, Vector2 topSize)
        {
            //Default node size
            m_MidRect.size = midSize;
            m_MidRect.position = startRectPos;

            m_TopRect.size = topSize;
            startRectPos += -Vector2.up * (m_TopRect.size.y - 2);
            m_TopRect.position = startRectPos;

            //Get total size and avrg pos
            m_TotalRect.size = new Vector2(midSize.x, midSize.y + topSize.y - 2);
            m_TotalRect.position = m_TopRect.position;

            //Reuse dummy vector
            startRectPos = NodeGUIConstants.k_GroupRectBorder * Vector2.one;
            m_BorderRect.size = m_TotalRect.size + startRectPos;
            m_BorderRect.position = m_TotalRect.position - startRectPos;
        }

        public override void Drag(Vector2 delta)
        {
            m_TopRect.position += delta;
            m_MidRect.position += delta;
            m_TotalRect.position += delta;
            m_BorderRect.position += delta;

            string[] keys = NestedNodesKeys;

            //If this group rect is not grouped
            for (int i = 0; i < keys.Length; i++)
            {
                m_NestedNodesDictionary[keys[i]].Drag(delta);
            }
        }


        public override void Draw()
        {
            if (m_IsSelected)
            {
                float newWidth = m_TotalRect.width * NodeGUIConstants.k_SelectedNodeTextureScale;
                float newHeight = m_TotalRect.height * NodeGUIConstants.k_SelectedNodeTextureScale;
                GUI.DrawTexture(new Rect(
                    m_TotalRect.x - /*NodeTextureDimensions.EFFECT_NODE_OUTLINE_OFFSET.x*/(newWidth - m_TotalRect.width) * 0.5f,
                    m_TotalRect.y -/* NodeTextureDimensions.EFFECT_NODE_OUTLINE_OFFSET.y*/  (newHeight - m_TotalRect.height) * 0.5f,
                    newWidth, newHeight),
                    m_NodeSkin.m_SelectedMidOutline);
            }

            LEMStyleLibrary.s_GUIPreviousColour = GUI.color;

            //Draw the top of the node
            GUI.color = m_TopSkinColour;
            GUI.DrawTexture(m_TopRect, m_NodeSkin.m_TopBackground, ScaleMode.StretchToFill);

            //Draw the node midskin with its colour
            GUI.color = LEMStyleLibrary.s_CurrentGroupRectMidSkinColour;
            GUI.DrawTexture(m_MidRect, m_NodeSkin.m_MidBackground, ScaleMode.StretchToFill);
            GUI.color = LEMStyleLibrary.s_GUIPreviousColour;

            //Rect labelRect = m_TopRect;
            LEMStyleLibrary.BeginEditorLabelColourChange(LEMStyleLibrary.s_CurrentLabelColour);
            m_CommentLabel = EditorGUI.DelayedTextField(m_TopRect, m_CommentLabel, LEMStyleLibrary.s_NodeHeaderStyle);
            LEMStyleLibrary.EndEditorLabelColourChange();
        }

        void UpdateNestedNode()
        {
            string[] keys = NestedNodesKeys;

            //Remove any nodes whose rects do not overlap
            for (int i = 0; i < keys.Length; i++)
            {
                //If any of its grouped child is another group rect, force it to update
                if (m_NestedNodesDictionary[keys[i]].BaseNodeType == NodeType.GroupRectNode)
                {
                    GroupRectNode forceUpdateGrpNode = m_NestedNodesDictionary[keys[i]] as GroupRectNode;
                    forceUpdateGrpNode.UpdateNestedNode();
                }

                if (!m_TotalRect.Overlaps(m_NestedNodesDictionary[keys[i]].m_TotalRect, true))
                {
                    m_NestedNodesDictionary[keys[i]].m_GroupedParent = null;
                    m_NestedNodesDictionary.Remove(keys[i]);
                }

            }

            //Add any nodes whose rects do overlap which at the same time are not inside the nested dictionary and is not grouped b4
            for (int i = 0; i < NodeLEM_Editor.AllConnectableNodesInEditor.Count; i++)
            {
                if (!NodeLEM_Editor.AllConnectableNodesInEditor[i].IsGrouped && !m_NestedNodesDictionary.ContainsKey(NodeLEM_Editor.AllConnectableNodesInEditor[i].NodeID) && m_TotalRect.Overlaps(NodeLEM_Editor.AllConnectableNodesInEditor[i].m_TotalRect, true))
                {
                    NodeLEM_Editor.AllConnectableNodesInEditor[i].m_GroupedParent = this;
                    m_NestedNodesDictionary.Add(NodeLEM_Editor.AllConnectableNodesInEditor[i].NodeID, NodeLEM_Editor.AllConnectableNodesInEditor[i]);
                }
            }
        }

        void DeselectNestedNodes()
        {
            string[] keys = NestedNodesKeys;
            for (int i = 0; i < keys.Length; i++)
            {
                m_NestedNodesDictionary[keys[i]].DeselectNode();
            }
        }
        
        public override bool HandleLeftMouseDown(Event e)
        {
            //Check if mouseposition is within the bounds of the node's rect body
            Node currentClickedNode = NodeLEM_Editor.CurrentClickedNode;

            //Check if it is the left mousebutton that was pressed
            if (currentClickedNode != null)
            {
                //If mouse overlapps this node
                if (currentClickedNode == this)
                {
                    //Record the state of the current node last recorded
                    NodeLEM_Editor.CurrentNodeLastRecordedSelectState = currentClickedNode.IsSelected;

                    //if node has not been selected
                    if (!m_IsSelected)
                    {
                        SelectByClicking();
                        UpdateNestedNode();
                        DeselectNestedNodes();
                        return true;
                    }

                    //Else if mouse clicks on a selected node

                    //that means i want to deselect it with shift click 
                    if (e.shift)
                    {
                        DeselectNode();
                        return true;
                    }

                    // or i want to drag this selected nodes 
                    m_IsDragged = true;
                    UpdateNestedNode();
                    DeselectNestedNodes();
                    return false;
                }

                //else if mouse doesnt overlapp this node
                //If this node is selected
                if (m_IsSelected)
                {
                    //If shift click is pressed , dont run the code below
                    if (e.shift)
                    {
                        return false;
                    }

                    //Deselect if this node is selected but there isnt multiple selected nodes
                    // or if there is no node clicked
                    if (currentClickedNode.IsSelected && NodeLEM_Editor.CurrentNodeLastRecordedSelectState == false)
                    {
                        DeselectNode();
                        return true;
                    }
                    //when there is another node clicked in the window,
                    //as well as having multiple nodes selected
                    else if (currentClickedNode.IsSelected && NodeLEM_Editor.s_HaveMultipleNodeSelected && NodeLEM_Editor.CurrentNodeLastRecordedSelectState == true)
                    {
                        m_IsDragged = true;
                    }

                }


                return false;
            }

            //Record the state of the current node last recorded
            NodeLEM_Editor.CurrentNodeLastRecordedSelectState = null;

            if (!e.alt)
            {
                DeselectNode();
            }
            return true;
        }

    }

}