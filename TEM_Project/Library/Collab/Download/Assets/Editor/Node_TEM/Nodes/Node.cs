using System;
using UnityEngine;
using UnityEditor;
using TEM_Effects;

//The node class for NodeTEM_Editor
public abstract class Node 
{
    //Node physical properties
    //Rect is the reference to the rectangle body of the node
    public Rect rect = default;

    public string title = default;
    protected bool isDragged = default;

    protected bool isSelected = false;

    public InConnectionPoint inPoint = new InConnectionPoint();
    public OutConnectionPoint outPoint = new OutConnectionPoint();

    //All the necessary gui style is stored here
    protected NodeSkinCollection nodeSkin = default;

    protected Action<Node> onSelectNode = null;
    protected Action<Node> onDeSelectNode = null;

    protected NodeEffectType nodeEffect { private set;  get; }

    //TEM effect related variables
    public string temEffectDescription = default;
    public TEM_BaseEffect baseEffect = default;

    //Node constructor for easier assigning
    public virtual void Initialise(Vector2 position, NodeSkinCollection nodeSkin,GUIStyle inPointStyle, GUIStyle outPointStyle,
        Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint>onClickOutPoint
        , Action<Node> onSelectNode, Action<Node> onDeSelectNode,NodeEffectType nodeEffect)
    {
        rect = new Rect(position.x, position.y, 200f, 50f);

        this.nodeSkin = nodeSkin;

        //Assign delegates
        this.onSelectNode = onSelectNode;
        this.onDeSelectNode = onDeSelectNode;

        title = rect.ToString();

        //Initialise in and out points
        inPoint.Initialise(this, inPointStyle, onClickInPoint);
        outPoint.Initialise(this, outPointStyle, onClickOutPoint);

        this.nodeEffect = nodeEffect;

    }

    //Delta here is a finite increment (eg time.delta time, mouse movement delta(Event.delta), rectransform's delta x and y)
    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    //Draws the node using its position, dimensions and style
    public virtual void Draw()
    {
        //Draw the in out points as well
        inPoint.Draw();
        outPoint.Draw();
        title = rect.position.ToString();
        GUI.Box(rect, title, nodeSkin.style);
        temEffectDescription = EditorGUI.TextArea(new Rect(rect.x+10f,rect.y+10f,rect.width-20f,30f), temEffectDescription);

    }

    //Process events and return a boolean to check if we need to repaint its GUI or not
    public bool ProcessNodeEvents(Event e)
    {
        switch (e.type)
        {
            //If mouse was press down,
            case EventType.MouseDown:

                //Check if mouseposition is within the bounds of the node's rect body
                Node currentClickedNode = NodeTEM_Editor.s_CurrentClickedNode;

                //Check if it is the left mousebutton that was pressed
                if (e.button == 0)
                {
                    if (currentClickedNode != null)
                    {
                        //If mouse overlapps this node
                        if (currentClickedNode == this)
                        {
                            //Record the state of the current node last recorded
                            NodeTEM_Editor.s_CurrentNodeLastRecordedSelectState = currentClickedNode.isSelected;

                            //if node has not been selected
                            if (!isSelected)
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
                            isDragged = true;
                            return false;
                        }


                        //else if mouse doesnt overlapp this node
                        //If this node is selected
                        if (isSelected)
                        {
                            //If shift click is pressed , dont run the code below
                            if (e.shift)
                            {
                                return false;
                            }

                            //Deselect if this node is selected but there isnt multiple selected nodes
                            // or if there is no node clicked
                            if (currentClickedNode.isSelected && NodeTEM_Editor.s_CurrentNodeLastRecordedSelectState == false)
                            {
                                DeselectNode();
                                return true;
                            }
                            //when there is another node clicked in the window,
                            //as well as having multiple nodes selected
                            else if (currentClickedNode.isSelected && NodeTEM_Editor.s_HaveMultipleNodeSelected && NodeTEM_Editor.s_CurrentNodeLastRecordedSelectState == true)
                            {
                                isDragged = true;
                            }

                        }

                        return false;
                    }

                    //Record the state of the current node last recorded
                    NodeTEM_Editor.s_CurrentNodeLastRecordedSelectState = null;

                    DeselectNode();
                    return true;
                }

            break;
           
            //If mouse was released
            case EventType.MouseUp:

                //Reset draggin bool
                isDragged = false;
                break;

            //If mouse was press down and then dragged around,
            case EventType.MouseDrag:

                if (e.button == 0)
                {
                    if (isDragged)
                    {
                        Drag(e.delta);
                        //Tell the system that you need to redraw this GUI
                        return true;
                    }
                    //Check if node is within selection box of editor
                    else if (NodeTEM_Editor.s_SelectionBox.Overlaps(rect, true))
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
        nodeSkin.style = nodeSkin.light_selected;

        //Invoke onselect delegate
        onSelectNode?.Invoke(this);

        isSelected = true;

    }

    public void SelectByClicking()
    {
        //Change the visual to indicate that node has been selected
        nodeSkin.style = nodeSkin.light_selected;
        isDragged = true;

        //Invoke onselect delegate
        onSelectNode?.Invoke(this);

        isSelected = true;

    }

    public void DeselectNode()
    {

        onDeSelectNode?.Invoke(this);

        isSelected = false;
        nodeSkin.style = nodeSkin.light_normal;
    }

    #endregion

    public virtual void SaveEffectData() { }

    public virtual void ConnectNextEffect() { }

    public virtual void SaveNodeData(){ }
}
