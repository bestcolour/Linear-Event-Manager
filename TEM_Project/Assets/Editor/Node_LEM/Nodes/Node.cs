﻿using System;
using UnityEngine;

public abstract class Node
{
    public Rect m_MidRect = default;
    public Rect m_TopRect = default;

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

    protected Action<Node> d_OnSelectNode = null;
    protected Action<Node> d_OnDeselectNode = null;

    //public NodeBaseData m_NodeBaseDataSaveFile = default;

    public virtual void Initialise(Vector2 position, NodeSkinCollection nodeSkin, GUIStyle connectionPointStyle,
        Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint
        , Action<Node> onSelectNode, Action<Node> onDeSelectNode)
    {
        m_MidRect = new Rect(position.x, position.y, 200f, 50f);
        m_TopRect = new Rect(position.x, position.y + 25f, 200f, 25f);


        this.m_NodeSkin = nodeSkin;

        //Assign delegates
        this.d_OnSelectNode = onSelectNode;
        this.d_OnDeselectNode = onDeSelectNode;

        //Initialise in and out points
        m_InPoint.Initialise(this, connectionPointStyle, onClickInPoint);
        m_OutPoint.Initialise(this, connectionPointStyle, onClickOutPoint);
    }

    //Delta here is a finite increment (eg time.delta time, mouse movement delta(Event.delta), rectransform's delta x and y)
    public void Drag(Vector2 delta)
    {
        m_MidRect.position += delta;
        m_MidRect.position += delta;
    }

    //Draws the node using its position, dimensions and style
    //only start & end node completely overrides the draw method
    public virtual void Draw()
    {

        //Draw the top of the node
        GUI.DrawTexture(m_TopRect, m_NodeSkin.m_TopBackground);

        //Draw the node background
        GUI.DrawTexture(m_MidRect, m_NodeSkin.m_NodeBackground);



        //Draw the in out points as well
        m_InPoint.Draw();
        m_OutPoint.Draw();

    }

    #region Process Events

    public bool HandleMouseDown(Event e)
    {
        //Check if mouseposition is within the bounds of the node's rect body
        Node currentClickedNode = NodeLEM_Editor.s_CurrentClickedNode;

        //Check if it is the left mousebutton that was pressed
        if (e.button == 0)
        {
            if (currentClickedNode != null)
            {
                //If mouse overlapps this node
                if (currentClickedNode == this)
                {
                    //Record the state of the current node last recorded
                    NodeLEM_Editor.s_CurrentNodeLastRecordedSelectState = currentClickedNode.m_IsSelected;

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
                    if (currentClickedNode.m_IsSelected && NodeLEM_Editor.s_CurrentNodeLastRecordedSelectState == false)
                    {
                        DeselectNode();
                        return true;
                    }
                    //when there is another node clicked in the window,
                    //as well as having multiple nodes selected
                    else if (currentClickedNode.m_IsSelected && NodeLEM_Editor.s_HaveMultipleNodeSelected && NodeLEM_Editor.s_CurrentNodeLastRecordedSelectState == true)
                    {
                        m_IsDragged = true;
                    }

                }

                return false;
            }

            //Record the state of the current node last recorded
            NodeLEM_Editor.s_CurrentNodeLastRecordedSelectState = null;

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

    public bool HandleMouseDrag(Event e)
    {
        if (e.button == 0)
        {
            if (m_IsDragged)
            {
                Drag(e.delta);
                //Tell the system that you need to redraw this GUI
                return true;
            }
            //Check if node is within selection box of editor
            else if (NodeLEM_Editor.s_SelectionBox.Overlaps(m_MidRect, true))
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
        m_NodeSkin.textureToRender = m_NodeSkin.m_SelectedOutline;

        //Invoke onselect delegate
        d_OnSelectNode?.Invoke(this);

        m_IsSelected = true;

    }

    void SelectByClicking()
    {
        //Change the visual to indicate that node has been selected
        //nodeSkin.style = nodeSkin.light_selected;
        m_NodeSkin.textureToRender = m_NodeSkin.m_SelectedOutline;

        m_IsDragged = true;

        //Invoke onselect delegate
        d_OnSelectNode?.Invoke(this);

        m_IsSelected = true;

    }

    public void DeselectNode()
    {
        d_OnDeselectNode?.Invoke(this);
        m_IsSelected = false;
        m_NodeSkin.textureToRender = m_NodeSkin.m_NodeBackground;
    }

    #endregion

    //Returns only NodeBaseData (use for non effect nodes)
    public virtual NodeBaseData SaveNodeData()
    {
        return new NodeBaseData(m_MidRect.position, NodeID, m_OutPoint.GetConnectedNodeID(0)/*, m_InPoint.m_ConnectedNodeID*/);
    }

    //Connect connections with the node's in out points if there is any
    //public virtual void ConnectConnections() { }

}
