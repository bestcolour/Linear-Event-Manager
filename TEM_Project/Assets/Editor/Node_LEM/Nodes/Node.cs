using System;
using UnityEngine;

public abstract class Node
{
    public Rect m_Rect = default;

    Guid m_NodeID = default;
    public string NodeID
    {
        get; private set;
    }

    void GenerateNodeID()
    {
        m_NodeID = Guid.NewGuid();
        NodeID = m_NodeID.ToString();
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
        m_Rect = new Rect(position.x, position.y, 200f, 50f);

        this.m_NodeSkin = nodeSkin;

        //Assign delegates
        this.d_OnSelectNode = onSelectNode;
        this.d_OnDeselectNode = onDeSelectNode;

        //Initialise in and out points
        m_InPoint.Initialise(this, connectionPointStyle, onClickInPoint);
        m_OutPoint.Initialise(this, connectionPointStyle, onClickOutPoint);


        GenerateNodeID();
    }

    //Delta here is a finite increment (eg time.delta time, mouse movement delta(Event.delta), rectransform's delta x and y)
    public void Drag(Vector2 delta)
    {
        m_Rect.position += delta;
    }

    //Draws the node using its position, dimensions and style
    public virtual void Draw()
    {
        //Draw the node box,description and title
        GUI.DrawTexture(m_Rect, m_NodeSkin.textureToRender);

        //Draw the in out points as well
        m_InPoint.Draw();
        m_OutPoint.Draw();

    }

    //Process events and return a boolean to check if we need to repaint its GUI or not
    public bool ProcessNodeEvents(Event e)
    {
        switch (e.type)
        {
            //If mouse was press down,
            case EventType.MouseDown:

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

                    DeselectNode();
                    return true;
                }

                break;

            //If mouse was released
            case EventType.MouseUp:

                //Reset draggin bool
                m_IsDragged = false;
                break;

            //If mouse was press down and then dragged around,
            case EventType.MouseDrag:

                if (e.button == 0)
                {
                    if (m_IsDragged)
                    {
                        Drag(e.delta);
                        //Tell the system that you need to redraw this GUI
                        return true;
                    }
                    //Check if node is within selection box of editor
                    else if (NodeLEM_Editor.s_SelectionBox.Overlaps(m_Rect, true))
                    {
                        SelectBySelectionBox();
                        return true;
                    }
                    else
                    {
                        DeselectNode();
                        return true;
                    }

                }

                break;

        }

        return false;
    }

    #region Modes of Selection

    public void SelectBySelectionBox()
    {
        //Change the visual to indicate that node has been selected
        m_NodeSkin.textureToRender = m_NodeSkin.light_selected;

        //Invoke onselect delegate
        d_OnSelectNode?.Invoke(this);

        m_IsSelected = true;

    }

    public void SelectByClicking()
    {
        //Change the visual to indicate that node has been selected
        //nodeSkin.style = nodeSkin.light_selected;
        m_NodeSkin.textureToRender = m_NodeSkin.light_selected;

        m_IsDragged = true;

        //Invoke onselect delegate
        d_OnSelectNode?.Invoke(this);

        m_IsSelected = true;

    }

    public void DeselectNode()
    {
        d_OnDeselectNode?.Invoke(this);
        m_IsSelected = false;
        m_NodeSkin.textureToRender = m_NodeSkin.light_normal;
    }

    #endregion

    /// <summary>
    /// This function will be overrided in BaseEffect child node Types to save effect data. In addition, there will be just position saving for both normal and Effect nodes 
    /// </summary>
    /// (Add it here for the position saving,connections,etc)
    //public virtual void SaveNodeData()
    //{
    //    m_NodeBaseDataSaveFile = new NodeBaseData(m_Rect.position, NodeID, m_OutPoint.parentNode.NodeID, m_InPoint.parentNode.NodeID);
    //}

    //Connect connections with the node's in out points if there is any
    public virtual void ConnectConnections() { }

}
