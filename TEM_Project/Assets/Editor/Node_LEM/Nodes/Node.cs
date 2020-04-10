using System;
using System.Collections.Generic;
using UnityEngine;

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
        NodeID = Guid.NewGuid().ToString();
    }

    public void SetNodeID(string id)
    {
        NodeID = id;
    }

    protected bool m_IsDragged = default;

    protected bool m_IsSelected = false;
    public bool IsSelected { get { return m_IsSelected; } }

    public InConnectionPoint m_InPoint = new InConnectionPoint();
    public OutConnectionPoint m_OutPoint = new OutConnectionPoint();

    protected NodeSkinCollection m_NodeSkin = default;
    //Top skin will pull from a static cache
    protected Color m_MidSkinColour = default;

    protected Action<Node> d_OnSelectNode = null;
    protected Action<Node> d_OnDeselectNode = null;

    public virtual void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle,
        Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint
        , Action<Node> onSelectNode, Action<Node> onDeSelectNode,Color midSkinColour )
    {
        m_TopRect = new Rect();

        SetNodeRects(position, NodeTextureDimensions.NORMAL_MID_SIZE, NodeTextureDimensions.NORMAL_TOP_SIZE);

        this.m_NodeSkin = nodeSkin;

        //Assign delegates
        this.d_OnSelectNode = onSelectNode;
        this.d_OnDeselectNode = onDeSelectNode;

        //Initialise in and out points
        m_InPoint.Initialise(this, connectionPointStyle, onClickInPoint);
        m_OutPoint.Initialise(this, connectionPointStyle, onClickOutPoint);

        m_MidSkinColour = midSkinColour;
    }

    //Delta here is a finite increment (eg time.delta time, mouse movement delta(Event.delta), rectransform's delta x and y)
    public virtual void Drag(Vector2 delta)
    {
        m_TopRect.position += delta;
        m_MidRect.position += delta;
        m_TotalRect.position += delta;
    }

    //Draws the node using its position, dimensions and style
    //only start & end node completely overrides the draw method
    public virtual void Draw()
    {
        if (m_IsSelected)
        {
            GUI.DrawTexture(new Rect(
                m_TotalRect.x - NodeTextureDimensions.EFFECT_NODE_OUTLINE_OFFSET.x,
                m_TotalRect.y - NodeTextureDimensions.EFFECT_NODE_OUTLINE_OFFSET.y,
                m_TotalRect.width * 1.075f, m_TotalRect.height * 1.075f),
                m_NodeSkin.m_SelectedMidOutline);
        }

        LEMStyleLibrary.s_GUIPreviousColour = GUI.color;

        //Draw the top of the node
        GUI.color =LEMStyleLibrary.s_CurrentTopTextureColour;
        GUI.DrawTexture(m_TopRect, m_NodeSkin.m_TopBackground,ScaleMode.StretchToFill);

        //Draw the node midskin with its colour
        GUI.color = m_MidSkinColour;
        GUI.DrawTexture(m_MidRect, m_NodeSkin.m_MidBackground,ScaleMode.StretchToFill);
        GUI.color = LEMStyleLibrary.s_GUIPreviousColour;

        //Draw the in out points as well
        m_InPoint.Draw();
        m_OutPoint.Draw();

    }

    #region Process Events

    public bool HandleMouseDown(Event e)
    {
        //Check if mouseposition is within the bounds of the node's rect body
        Node currentClickedNode = NodeLEM_Editor.CurrentClickedNode;

        //Check if it is the left mousebutton that was pressed
        if (e.button == 0)
        {
            if (currentClickedNode != null)
            {
                //If mouse overlapps this node
                if (currentClickedNode == this)
                {
                    //Record the state of the current node last recorded
                    NodeLEM_Editor.CurrentNodeLastRecordedSelectState = currentClickedNode.m_IsSelected;

                    //if node has not been selected
                    if (!m_IsSelected)
                    {
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

                    // or i want to drag this selected nodes 
                    m_IsDragged = true;
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
                    if (currentClickedNode.m_IsSelected && NodeLEM_Editor.CurrentNodeLastRecordedSelectState == false)
                    {
                        DeselectNode();
                        return true;
                    }
                    //when there is another node clicked in the window,
                    //as well as having multiple nodes selected
                    else if (currentClickedNode.m_IsSelected && NodeLEM_Editor.s_HaveMultipleNodeSelected && NodeLEM_Editor.CurrentNodeLastRecordedSelectState == true)
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

        return false;
    }

    public bool HandleMouseUp(Event e)
    {
        //Reset draggin bool
        m_IsDragged = false;
        return false;
    }

    public bool HandleMouseDrag(Event e,Vector2 dragDelta)
    {
        if (e.button == 0)
        {
            if (m_IsDragged)
            {
                Drag(dragDelta);
                //Drag(e.delta);
                //Tell the system that you need to redraw this GUI
                return true;
            }
            //Check if node is within selection box of editor
            else if (NodeLEM_Editor.s_SelectionBox.Overlaps(m_TotalRect, true))
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
        }
        return false;
    }
    #endregion

    #region Modes of Selection

    void SelectBySelectionBox()
    {
        //Change the visual to indicate that node has been selected
        //m_NodeSkin.textureToRender = m_NodeSkin.m_SelectedOutline;

        //Invoke onselect delegate
        d_OnSelectNode?.Invoke(this);

        m_IsSelected = true;

    }

    void SelectByClicking()
    {
        //Change the visual to indicate that node has been selected
        //nodeSkin.style = nodeSkin.light_selected;
        //m_NodeSkin.textureToRender = m_NodeSkin.m_SelectedOutline;

        m_IsDragged = true;

        //Invoke onselect delegate
        d_OnSelectNode?.Invoke(this);

        m_IsSelected = true;

    }

    public void DeselectNode()
    {
        d_OnDeselectNode?.Invoke(this);
        m_IsSelected = false;
        //m_NodeSkin.textureToRender = m_NodeSkin.m_NodeBackground;
    }

    #endregion

    protected string[] TryToSaveConnectedNodeID()
    {
        //Returns true value of saved state
        string[] connectedNodeIDs = m_OutPoint.IsConnected ? new string[1] { m_OutPoint.GetConnectedNodeID(0) } : new string[0];

        return connectedNodeIDs;
    }

    //Returns only NodeBaseData (use for non effect nodes)
    public virtual NodeBaseData SaveNodeData()
    {
        string[] connectedNodeIDs = TryToSaveConnectedNodeID();
        return new NodeBaseData(m_MidRect.position, NodeID, connectedNodeIDs /*, m_InPoint.m_ConnectedNodeID*/);
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


}
