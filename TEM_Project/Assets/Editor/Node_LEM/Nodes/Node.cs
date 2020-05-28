using System;
using UnityEngine;
using LEM_Effects;

namespace LEM_Editor
{
    public abstract class Node
    {
        public Rect m_MidRect = new Rect();
        public Rect m_TopRect = default;
        public Rect m_TotalRect = new Rect();
        //Guid m_NodeID = default;
        public string NodeID
        {
            get; private set;
        }

        public void GenerateNodeID()
        {
            //m_NodeID = Guid.NewGuid();
            NodeID = ID_Initial + Guid.NewGuid().ToString();
        }

        public void SetNodeID(string id)
        {
            NodeID = id;
        }

        protected bool m_IsDragged = default;
        public GroupRectNode m_GroupedParent = default;


        public abstract string ID_Initial { get; }
        public bool IsGrouped => m_GroupedParent != null;

        //protected bool IsSelected = false;
        public bool IsSelected { get; protected set; } = false;
        //public bool IsSelected { get { return m_IsSelected; } }

        public bool IsWithinWindowScreen { get; private set; } = false;
        public bool IsWorthProcessingEventFor { get; private set; } = false;

        //public bool IsWorthProcessingEventFor => IsWithinWindowScreen || (!IsWithinWindowScreen && (IsSelected || (IsGrouped && GetRootParent.IsSelected)));

        protected NodeSkinCollection m_NodeSkin = default;
        //Top skin will pull from a static cache
        protected Color m_TopSkinColour = default;

        public virtual Node GetRootParent => IsGrouped ? m_GroupedParent.GetRootParent : this;

        protected Action<Node> d_OnSelectNode = null;
        protected Action<string> d_OnDeselectNode = null;

        //public bool IsWithinWindowScreen => m_TotalRect.position.x + m_TotalRect.width > 0 && m_TotalRect.position.x < NodeLEM_Editor.instance.position.width * NodeLEM_Editor.InverseScaleFactor
        //     &&
        //     m_TotalRect.position.y + m_TotalRect.height > 0 && m_TotalRect.position.y < NodeLEM_Editor.instance.position.height * NodeLEM_Editor.InverseScaleFactor;


        public virtual void Initialise(Vector2 position, NodeSkinCollection nodeSkin /*, GUIStyle connectionPointStyle,
            Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint*/
            , Action<Node> onSelectNode, Action<string> onDeSelectNode, Color topSkinColour)
        {
            m_TopRect = new Rect();

            SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);

            this.m_NodeSkin = nodeSkin;

            //Assign delegates
            this.d_OnSelectNode = onSelectNode;
            this.d_OnDeselectNode = onDeSelectNode;

            m_TopSkinColour = topSkinColour;
        }

        //Delta here is a finite increment (eg time.delta time, mouse movement delta(Event.delta), rectransform's delta x and y)
        public virtual void Drag(Vector2 delta)
        {
            m_TopRect.position += delta;
            m_MidRect.position += delta;
            m_TotalRect.position += delta;
        }

        public void DetermineStatus()
        {
            IsWithinWindowScreen = m_TotalRect.position.x + m_TotalRect.width > 0 && m_TotalRect.position.x < NodeLEM_Editor.instance.position.width * NodeLEM_Editor.InverseScaleFactor
             &&
             m_TotalRect.position.y + m_TotalRect.height > 0 && m_TotalRect.position.y < NodeLEM_Editor.instance.position.height * NodeLEM_Editor.InverseScaleFactor;

            IsWorthProcessingEventFor = IsWithinWindowScreen || (!IsWithinWindowScreen && (IsSelected || (IsGrouped && GetRootParent.IsSelected))); 
        }


        //Draws the node using its position, dimensions and style
        //only start & end node completely overrides the draw method
        public virtual void Draw()
        {
            if (IsSelected)
            {
                float newWidth = m_TotalRect.width * NodeGUIConstants.k_SelectedNodeTextureScale;
                float newHeight = m_TotalRect.height * NodeGUIConstants.k_SelectedNodeTextureScale;
                GUI.DrawTexture(new Rect(
                    m_TotalRect.x - (newWidth - m_TotalRect.width) * 0.5f,
                    m_TotalRect.y - (newHeight - m_TotalRect.height) * 0.5f,
                    newWidth, newHeight),
                    m_NodeSkin.m_SelectedMidOutline);
            }

            LEMStyleLibrary.s_GUIPreviousColour = GUI.color;

            //Draw the top of the node
            GUI.color = m_TopSkinColour;
            GUI.DrawTexture(m_TopRect, m_NodeSkin.m_TopBackground, ScaleMode.StretchToFill);

            //Draw the node midskin with its colour
            GUI.color = LEMStyleLibrary.s_CurrentMidSkinColour;
            GUI.DrawTexture(m_MidRect, m_NodeSkin.m_MidBackground, ScaleMode.StretchToFill);
            GUI.color = LEMStyleLibrary.s_GUIPreviousColour;



        }

        #region Process Events

        public virtual bool HandleLeftMouseDown(Event e)
        {
            //Check if mouseposition is within the bounds of the node's rect body
            Node currentClickedNode = NodeLEM_Editor.CurrentClickedNode;

            if (currentClickedNode != null)
            {
                //If mouse overlapps this node
                if (currentClickedNode == this)
                {
                    //Record the state of the current node last recorded
                    NodeLEM_Editor.CurrentNodeLastRecordedSelectState = currentClickedNode.IsSelected;

                    //if node has not been selected
                    if (!IsSelected)
                    {
                        DeselectAllParentGroupNodes();
                        SelectByClicking();
                        return true;
                    }

                    //Else if mouse clicks on a selected node

                    //that means i want to deselect it with shift click 
                    if (e.shift)
                    {
                        DeselectNode();
                        return true;
                    }

                    //DeselectAllParentGroupNodes();
                    // or i want to drag this selected nodes 
                    m_IsDragged = true;
                    return false;
                }

                //else if mouse doesnt overlapp this node
                //If this node is selected
                if (IsSelected)
                {
                    //If shift click is pressed , dont run the code below
                    if (e.shift)
                    {
                        return false;
                    }


                    //Deselect if this node is selected but there isnt multiple selected nodes
                    // or if there is no node clicked

                    if (currentClickedNode.IsSelected)
                    {
                        if (NodeLEM_Editor.CurrentNodeLastRecordedSelectState == false)
                        {
                            DeselectAllParentGroupNodes();
                            DeselectNode();
                            return true;
                        }
                        //when there is another node clicked in the window,
                        //as well as having multiple nodes selected
                        else if (NodeLEM_Editor.s_HaveMultipleNodeSelected && NodeLEM_Editor.CurrentNodeLastRecordedSelectState == true)
                        {
                            m_IsDragged = true;
                        }
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

        public virtual bool HandleMouseUp()
        {
            //Reset draggin bool
            m_IsDragged = false;
            return false;
        }

        public virtual bool HandleLeftMouseButtonDrag(Event e, Vector2 dragDelta)
        {
            //You can only drag this node when node is not grouped or the parent is not selected
            if (m_IsDragged && (!IsGrouped || !m_GroupedParent.IsSelected))
            {
                Drag(dragDelta);
                //Drag(e.delta);
                //Tell the system that you need to redraw this GUI
                return true;
            }
            //Check if node is within selection box of editor
            else if (NodeLEM_Editor.s_SelectionBox != Rect.zero && NodeLEM_Editor.s_SelectionBox.Overlaps(m_TotalRect, true))
            {
                SelectBySelectionBox();
                return true;
            }
            //Else if user isnt dragging the canvas
            else if (!e.alt)
            {
                DeselectNode();
                return true;
            }
            return false;
        }
        #endregion

        #region Modes of Selection

        protected void SelectBySelectionBox()
        {
            //Invoke onselect delegate
            d_OnSelectNode?.Invoke(this);

            IsSelected = true;

        }

        protected void SelectByClicking()
        {
            m_IsDragged = true;

            //Invoke onselect delegate
            d_OnSelectNode?.Invoke(this);

            IsSelected = true;

        }

        public virtual void DeselectNode()
        {
            d_OnDeselectNode?.Invoke(NodeID);
            IsSelected = false;
            m_IsDragged = false;
        }

        public virtual void DeselectAllParentGroupNodes()
        {
            if (!IsGrouped)
                return;

            m_GroupedParent.DeselectNode();
            m_GroupedParent.DeselectAllParentGroupNodes();
        }

        public void SelectNode()
        {

            m_IsDragged = true;

            //Invoke onselect delegate
            d_OnSelectNode?.Invoke(this);

            IsSelected = true;
        }

        #endregion

        //protected virtual string[] TryToSaveNextPointNodeID()
        //{
        //    //Returns true value of saved state
        //    string[] connectedNodeIDs = m_OutPoint.IsConnected ? new string[1] { m_OutPoint.GetConnectedNodeID(0) } : new string[0];

        //    return connectedNodeIDs;
        //}

        //protected string[] TryToSavePrevPointNodeID()
        //{
        //    //Returns true value of saved state
        //    string[] connectedNodeIDs = m_InPoint.IsConnected ? m_InPoint.GetAllConnectedNodeIDs() : new string[0];

        //    return connectedNodeIDs;
        //}


        //Returns only NodeBaseData (use for non effect nodes)
        public virtual NodeBaseData SaveNodeData()
        {
            return new NodeBaseData(m_MidRect.position, NodeID, new string[0]);
        }

        //Change values here
        protected virtual void SetNodeRects(Vector2 mousePosition, Vector2 midSize, Vector2 topSize)
        {
            //Default node size
            m_MidRect.size = midSize;
            m_MidRect.position = mousePosition;

            m_TopRect.size = topSize;
            mousePosition += -Vector2.up * (m_TopRect.size.y - 2);
            m_TopRect.position = mousePosition;

            //Get total size and avrg pos
            m_TotalRect.size = new Vector2(midSize.x, midSize.y + topSize.y - 2);
            m_TotalRect.position = m_TopRect.position;

        }

        protected void AddMidRectSize(Vector2 midSizeAddition)
        {
            //Default node size
            m_MidRect.size += midSizeAddition;
            m_TotalRect.size = new Vector2(m_MidRect.size.x, m_MidRect.size.y + m_TopRect.size.y - 2);

        }

        protected void SetMidRectSize(Vector2 sizeToSet)
        {
            //Default node size
            m_MidRect.size = sizeToSet;
            m_TotalRect.size = new Vector2(m_MidRect.size.x, m_MidRect.size.y + m_TopRect.size.y - 2);

        }

        protected void AddTopRectSize(Vector2 topSizeAddition)
        {
            m_TopRect.size += topSizeAddition;
            m_TotalRect.size = new Vector2(m_MidRect.size.x, m_MidRect.size.y + m_TopRect.size.y - 2);
        }

    }

}