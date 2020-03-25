﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using LEM_Effects;
using System.Linq;

public class NodeLEM_Editor : EditorWindow
{
    public static NodeLEM_Editor instance = default;
    public static LinearEvent m_EditingLinearEvent = default;

    //For saving 
    List<Node> m_AllNodesInEditor = new List<Node>();
    public BaseEffectNode[] AllEffectNodesInEditor
    {
        get
        {
            m_AllNodesInEditor.Remove(s_StartNode);
            m_AllNodesInEditor.Remove(s_EndNode);

            BaseEffectNode[] allEffectNodes = Array.ConvertAll(m_AllNodesInEditor.ToArray(), x => (BaseEffectNode)x).ToArray();

            m_AllNodesInEditor.Add(s_StartNode);
            m_AllNodesInEditor.Add(s_EndNode);
            return allEffectNodes;
        }
    }

    List<Connection> m_AllConnectionsInEditor = default;
    Node s_StartNode = default, s_EndNode = default;

    #region Process Event Variables

    List<Node> m_AllSelectedNodes = new List<Node>();

    public static Node s_CurrentClickedNode = null;
    public static bool? s_CurrentNodeLastRecordedSelectState = null;
    //Check if there is multiple nodes selected
    public static bool s_HaveMultipleNodeSelected => (instance.m_AllSelectedNodes.Count > 0);

    //Canvas drag and offset
    Vector2 m_AmountOfMouseDragThisUpdate = default;
    Vector2 m_AmountOfOffset = default;

    //Selection box variables
    Vector2? m_InitialClickedPosition = default;
    public static Rect s_SelectionBox = default;
    static Color s_SelectionBoxColour = new Color(0.6f, 0.8f, 1f, .2f);
    static Color s_SelectionBoxOutlineColour = new Color(0f, 0.298f, 0.6f, 1f);

    //Currently selected in / out points
    ConnectionPoint m_SelectedInPoint = default;
    ConnectionPoint m_SelectedOutPoint = default;

    #endregion


    //NodeCommandInvoker m_CommandInvoker = default;

    static LEMSkinsLibrary s_SkinsLibrary = new LEMSkinsLibrary();


    #region GUI Style
    //Function that sets the connection point's style
    void SetConnectionPointStyles(ConnectionPoint point, bool state)
    {
        if (point != null)
        {
            if (state)
            {
                point.style = s_SkinsLibrary.s_ConnectionPointStyleSelected;
                return;
            }
            point.style = s_SkinsLibrary.s_ConnectionPointStyleNormal;
        }
    }

    public static GUIStyle s_NodeHeaderStyle = default;
    public static GUIStyle s_NodeTextInputStyle = default;
    public static GUIStyle s_NodeParagraphStyle = default;


    #endregion

    public static void OpenWindow()
    {
        //Get window 
        NodeLEM_Editor editorWindow = GetWindow<NodeLEM_Editor>();

        //Set the title of gui for the window to be TEM Node Editor
        editorWindow.titleContent = new GUIContent("TEM Node Editor");

    }

    public static void LoadNodeEditor(LinearEvent linearEvent)
    {
        OpenWindow();
        m_EditingLinearEvent = linearEvent;
    }

    #region Initialisation

    void OnEnable()
    {
        instance = this;

        s_SkinsLibrary.LoadLibrary();

        InitialiseTextStyle();

        //Creates instance of invoker
        //if (m_CommandInvoker == null)
        //{
        //    m_CommandInvoker = new NodeCommandInvoker();
        //}

        if (m_AllNodesInEditor == null)
        {
            m_AllNodesInEditor = new List<Node>();
        }

        if (m_AllConnectionsInEditor == null)
        {
            m_AllConnectionsInEditor = new List<Connection>();
        }

        InitialiseStartEndNodes();

    }

    void InitialiseTextStyle()
    {
        //Initialising public static node title styles
        s_NodeHeaderStyle = new GUIStyle();
        s_NodeHeaderStyle.fontSize = 13;

        s_NodeTextInputStyle = GUI.skin.GetStyle("textField");
        s_NodeTextInputStyle.fontSize = 10;

        s_NodeParagraphStyle = new GUIStyle();
        s_NodeParagraphStyle.fontSize = 10;
    }

    void InitialiseStartEndNodes()
    {
        if (s_StartNode == null)
        {
            s_StartNode = new StartNode();
            //Initialise the new node 
            s_StartNode.Initialise
                (new Vector2(EditorGUIUtility.currentViewWidth * 0.5f, 50f), s_SkinsLibrary.s_StartNodeSkins, s_SkinsLibrary.s_ConnectionPointStyleNormal,
                OnClickInPoint, OnClickOutPoint, AddNodeToSelectedCollection, RemoveNodeFromSelectedCollection);

            //Add the node into collection in editor
            m_AllNodesInEditor.Add(s_StartNode);
        }

        if (s_EndNode == null)
        {
            s_EndNode = new EndNode();
            //Initialise the new node 
            s_EndNode.Initialise
                (new Vector2(EditorGUIUtility.currentViewWidth * 0.85f, 50f), s_SkinsLibrary.s_EndNodeSkins, s_SkinsLibrary.s_ConnectionPointStyleNormal,
                OnClickInPoint, OnClickOutPoint, AddNodeToSelectedCollection, RemoveNodeFromSelectedCollection);

            //Add the node into collection in editor
            m_AllNodesInEditor.Add(s_EndNode);
        }

    }

    #endregion

    #region Resets

    void ResetEventVariables()
    {
        m_InitialClickedPosition = null;
        s_SelectionBox = default;
        s_CurrentClickedNode = null;
        GUI.changed = true;
    }

    void ResetDrawingBezierCurve()
    {
        //Reset bezierline drawing
        m_SelectedOutPoint = null;
        m_SelectedInPoint = null;
    }

    void OnDisable()
    {
        ResetDrawingBezierCurve();
        ResetEventVariables();
        s_CurrentNodeLastRecordedSelectState = null;
        m_EditingLinearEvent = null;
    }
    #endregion

    void OnGUI()
    {
        Event currentEvent = Event.current;

        //Draw a grid background
        //Draw this one to show every increment of 20 distance
        DrawGrid(20, 0.2f, Color.gray);
        //Draw this one to show every increment of 100 distance
        //also make this more opaque to show this every 5 increments
        DrawGrid(100, 0.4f, Color.gray);


        //Draw the nodes first
        DrawNodes();
        DrawConnections();
        DrawConnectionLine(currentEvent);
        DrawSelectionBox(currentEvent.mousePosition);
        DrawSaveButton();
        DrawRefreshButton();


        //Then process the events that occured from unity's events (events are like clicks,drag etc)
        ProcessEvents(currentEvent);
        ProcessNodeEvents(currentEvent);
        //If there is any value change in the gui,repaint it
        if (GUI.changed)
        {
            Repaint();
        }
    }

    #region Draw Functions

    void DrawNodes()
    {
        //If nodes collection is not null
        if (m_AllNodesInEditor != null)
        {
            for (int i = 0; i < m_AllNodesInEditor.Count; i++)
            {
                m_AllNodesInEditor[i].Draw();
            }
        }
    }

    void DrawConnections()
    {
        //If connections is not equal to null
        if (m_AllConnectionsInEditor != null)
        {
            for (int i = 0; i < m_AllConnectionsInEditor.Count; i++)
            {
                //Ask its connections to draw itself
                m_AllConnectionsInEditor[i].Draw();
            }
        }
    }

    //This is the realtime drawing of a bezier curve line to let users visualise
    //where the connections are going
    void DrawConnectionLine(Event e)
    {
        //if player clicked on a inpoint alrdy and havent clikced on a output yet,
        if (m_SelectedInPoint != null && m_SelectedOutPoint == null)
        {
            //Just repeat the code in connection script but replace the variables
            // replace outpoint with mouseposition
            Handles.DrawBezier(
                m_SelectedInPoint.rect.center,
                e.mousePosition,
                m_SelectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;

        }
        //Else if player clicked a out point first then wants to click a inpoint
        else if (m_SelectedOutPoint != null && m_SelectedInPoint == null)
        {
            //Just repeat the code in connection script but replace the variables
            // replace inpoint with mouseposition
            Handles.DrawBezier(
                e.mousePosition,
                m_SelectedOutPoint.rect.center,
                e.mousePosition + Vector2.left * 50f,
                m_SelectedOutPoint.rect.center - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;

        }
    }

    void DrawGrid(float gridSpacing, float gridOpacity, Color gridColour)
    {
        //Get the number of lines to draw by dividing the dimension of the window with the gridspacing
        int numberOfLinesToDrawX = Mathf.CeilToInt(position.width / gridSpacing);
        int numberOfLinesToDrawY = Mathf.CeilToInt(position.height / gridSpacing);

        //Begin drawing the 2d grid
        Handles.BeginGUI();

        //Set canvas colour and opactiy
        Handles.color = new Color(gridColour.r, gridColour.g, gridColour.b, gridOpacity);

        //Multiple amt of mouse drag this update by half b4 adding it to offset to ensure that
        //the distance dragged for the node matches that of the canvas drawn
        m_AmountOfOffset += m_AmountOfMouseDragThisUpdate * 0.5f;

        //Divide the total amount of offset by gridspacing.
        //if remainder is 0, that means we dont need to draw new lines cause the canvas moved the exactly the same
        //distance as gridspacing multiplied by a factor. So it is as if we didnt move the canvas
        //however, if remainder is not zero, we have an offset value of ranging from 
        // 0 < value < gridspacing
        //with that offset, we can draw lines at a new position inbetween the usual grid lines positions
        Vector3 newOffset = new Vector3(m_AmountOfOffset.x % gridSpacing, m_AmountOfOffset.y % gridSpacing, 0);

        for (int x = 0; x < numberOfLinesToDrawX; x++)
        {
            //Draw line at x value of the x axis (imagine a 2d graph) where start origin is bottom left of screen
            //point 1 = gridspace * x at incremental value , -gridsapcing (that means it starts below the window screen)
            //point 2 = gridspace * at incremental value, position.height (that means the heighest part of the window screen)s
            //both points to + offsets to allow the starting point of drawing the graph to be different
            Handles.DrawLine(new Vector3(gridSpacing * x, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * x, position.height, 0f) + newOffset);
        }

        //Repeat this for Y axis just that u swap the values positions
        for (int y = 0; y < numberOfLinesToDrawY; y++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * y, 0) + newOffset, new Vector3(position.width, gridSpacing * y, 0f) + newOffset);
        }


        Handles.color = Color.white;
        Handles.EndGUI();
    }

    void DrawSelectionBox(Vector2 currentMousePosition)
    {
        if (m_InitialClickedPosition != null)
        {
            Vector2 initialClickPos = (Vector2)m_InitialClickedPosition;

            s_SelectionBox = new Rect(initialClickPos, currentMousePosition - initialClickPos);
            Handles.DrawSolidRectangleWithOutline(s_SelectionBox, s_SelectionBoxColour, s_SelectionBoxOutlineColour);
        }
    }

    void DrawSaveButton()
    {
        if (GUI.Button(new Rect(position.width - 100f, 0, 100f, 50f), "Save Effects"))
        {
            //SaveNodes();
            TransferNodeData();
        }
    }

    //Creation use only
    void DrawRefreshButton()
    {
        if (GUI.Button(new Rect(position.width - 215f, 0, 100f, 50f), "Refresh"))
        {
            //LoadingNodeSkins();
            s_SkinsLibrary.LoadLibrary();
            OnDisable();
            OnEnable();
        }

    }


    #endregion

    //Checks what the current event is right now, and then execute code accordingly
    void ProcessEvents(Event e)
    {
        m_AmountOfMouseDragThisUpdate = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:

                //Set the currenly clicked node
                s_CurrentClickedNode = m_AllNodesInEditor.Find(x => x.m_Rect.Contains(e.mousePosition));

                //Check if the mouse button down is the right click button
                if (e.button == 1)
                {
                    //and if that the mouse is not clicking on any nodes currently
                    if (s_CurrentClickedNode == null || !s_CurrentClickedNode.IsSelected)
                    {
                        //Open a custom created that allows creation of more nodes
                        ProcessContextMenu(e.mousePosition);
                        return;
                    }

                    //Else, open the node's context menu
                    ProcessNodeContextMenu(m_AllSelectedNodes.ToArray());
                }

                else if (e.button == 0)
                {
                    //Set current clicked node to be the first to be updated
                    ReOrderSelectedNode(s_CurrentClickedNode, m_AllNodesInEditor.Count - 1);

                    //If mouse indeed clicks on a node,
                    if (s_CurrentClickedNode == null)
                    {
                        //Set initial position for drawing selection box
                        if (!e.alt)
                        {
                            m_InitialClickedPosition = e.mousePosition;

                            SetConnectionPointStyles(m_SelectedInPoint, false);
                            SetConnectionPointStyles(m_SelectedOutPoint, false);

                            ResetDrawingBezierCurve();
                        }

                    }

                }

                break;

            case EventType.MouseDrag:

                //If user is draggin the mouse,
                //with alt pressed, then drag the canvas
                if (e.button == 0)
                {
                    if (e.alt && m_InitialClickedPosition == null && s_CurrentClickedNode == null)
                    {
                        OnDrag(e.delta);
                        GUI.changed = true;
                    }
                }

                break;

            //If user releases the mouse
            case EventType.MouseUp:

                ResetEventVariables();
                break;

            //If the user presses a keyboard keybutton
            case EventType.KeyUp:

                //Check the keycode type
                switch (e.keyCode)
                {
                    //If delete key or backspace key is pressed,
                    case KeyCode.Delete:

                        //Remove start and end node 
                        if (m_AllSelectedNodes.Contains(s_StartNode))
                        {
                            s_StartNode.DeselectNode();
                        }

                        if (m_AllSelectedNodes.Contains(s_EndNode))
                        {
                            s_EndNode.DeselectNode();
                        }

                        while (s_HaveMultipleNodeSelected)
                        {
                            OnClickRemoveNode(m_AllSelectedNodes[0]);
                        }

                        //Skip everything else and repaint
                        e.Use();

                        break;

                    case KeyCode.Escape:

                        SetConnectionPointStyles(m_SelectedInPoint, false);
                        SetConnectionPointStyles(m_SelectedOutPoint, false);

                        ResetDrawingBezierCurve();
                        break;

                }

                break;

        }

    }

    void ProcessNodeEvents(Event e)
    {
        if (m_AllNodesInEditor != null)
        {
            //Check current event once and then tell all the nodes to handle that event so they dont have to check
            switch(e.type)
            {
                case EventType.MouseDown:

                    for (int i = m_AllNodesInEditor.Count - 1; i >= 0; i--)
                        if (m_AllNodesInEditor[i].HandleMouseDown(e))
                        {
                            GUI.changed = true;
                        }

                    break;
                case EventType.MouseUp:
                    for (int i = m_AllNodesInEditor.Count - 1; i >= 0; i--)
                        if (m_AllNodesInEditor[i].HandleMouseUp(e))
                        {
                            GUI.changed = true;
                        }
                    break;

                case EventType.MouseDrag:
                    for (int i = m_AllNodesInEditor.Count - 1; i >= 0; i--)
                        if (m_AllNodesInEditor[i].HandleMouseDrag(e))
                        {
                            GUI.changed = true;
                        }
                    break;

            }
        }
    }

    #region Node Editor Events Functions

    #region Creating Node Types
    //When you right click in the editor window, you open a menu, this code handles that
    void ProcessContextMenu(Vector2 mousePosition)
    {
        //Create and add a new item into the menu, giving it a label of "Add a TEM node"
        GenericMenu genericMenu = new GenericMenu();
        //While also delegating that item's function to be OnClickAddNode
        genericMenu.AddItem(new GUIContent("Add Destroy GameObject node"), false,
            () => AddNode(mousePosition, "DestroyGameObjectNode"));
        genericMenu.AddItem(new GUIContent("Add Instantiate GameObject node"), false,
            () => AddNode(mousePosition, "InstantiateGameObjectNode"));



        //Show the menu
        genericMenu.ShowAsContext();
    }



    ////When the add TEM node option is clicked, this function runs
    //the T type inserted must be of tem_baseeffect class and must be initialisable 
    //node effect is incase i need to use it for later. If not, remove it
    void AddNode(Vector2 mousePosition, string nameOfNodeType)
    {
        BaseEffectNode newEffectNode = LEMDictionary.GetNodeObject(nameOfNodeType) as BaseEffectNode;

        //Get the respective skin from the collection of nodeskin
        NodeSkinCollection nodeSkin = s_SkinsLibrary.s_NodeStyleDictionary[nameOfNodeType];

        //Initialise the new node 
        newEffectNode.Initialise
            (mousePosition, nodeSkin, s_SkinsLibrary.s_ConnectionPointStyleNormal,
            OnClickInPoint, OnClickOutPoint, AddNodeToSelectedCollection, RemoveNodeFromSelectedCollection);


        //Add the node into collection in editor
        m_AllNodesInEditor.Add(newEffectNode);
    }

    #endregion

    void ProcessNodeContextMenu(Node[] currentlySelectedNodes)
    {
        //and then add an button option with the name "Remove node"
        GenericMenu genericMenu = new GenericMenu();

        //Add remove node function to the context menu option
        genericMenu.AddItem(new GUIContent("Remove node"), false,
            //Remove all the selected nodes 
            delegate
            {
                //Remove all the nodes that are selected until there are none left
                while (s_HaveMultipleNodeSelected)
                {
                    OnClickRemoveNode(m_AllSelectedNodes[0]);
                }
            });

        //Display the editted made menu
        genericMenu.ShowAsContext();
    }




    //Drags the window canvas (think like animator window)
    void OnDrag(Vector2 delta)
    {
        //Record the amount of drag there is changed 
        m_AmountOfMouseDragThisUpdate = delta;

        //Update all the node's positions as well
        if (m_AllNodesInEditor != null)
        {
            for (int i = 0; i < m_AllNodesInEditor.Count; i++)
            {
                m_AllNodesInEditor[i].Drag(delta);
            }
        }

    }

    void ReOrderSelectedNode(Node nodeToReorder, int index)
    {
        //If all selected doesnt contain this node, reorder it
        if (m_AllNodesInEditor.Contains(nodeToReorder))
        {
            //Copy the current element u wanna replace at to the last index of the list
            m_AllNodesInEditor.Add(m_AllNodesInEditor[index]);

            //Remove that node that just moved
            m_AllNodesInEditor.Remove(nodeToReorder);

            //Set that node to that index
            m_AllNodesInEditor[index] = nodeToReorder;
        }
    }

    #region Delegates

    //Onclick in and out points are to be passed as delegates/actions when creating 
    //new nodes so that they can pass it to their in and out points
    void OnClickInPoint(ConnectionPoint connectionPoint)
    {
        //Set currently selected inpoint as the inputted connection point
        m_SelectedInPoint = connectionPoint;

        //Set the style to show  that point has been selected
        m_SelectedInPoint.style = s_SkinsLibrary.s_ConnectionPointStyleSelected;

        //If current selected outpoint is not null
        if (m_SelectedOutPoint != null)
        {
            OutConnectionPoint castedOutPoint = (OutConnectionPoint)m_SelectedOutPoint;

            //Check if selected in point node is same as selected out point npde
            //In this case we dont want them to be the same cause its stupid to 
            //have connection with the same node
            //Another thing to check is if the selected inpoint's connected node is equal to selected output node. If it is then dont bother connecting
            if (m_SelectedOutPoint.parentNode != m_SelectedInPoint.parentNode &&
                castedOutPoint.m_ConnectedNodeID != m_SelectedInPoint.parentNode.NodeID)
            {
                //Set the styles to show  that points has been selected
                m_SelectedInPoint.style = s_SkinsLibrary.s_ConnectionPointStyleSelected;
                m_SelectedOutPoint.style = s_SkinsLibrary.s_ConnectionPointStyleSelected;

                //Link
                castedOutPoint.m_ConnectedNodeID = m_SelectedInPoint.parentNode.NodeID;
                m_SelectedInPoint.m_ConnectedNodeID = castedOutPoint.parentNode.NodeID;
                CreateConnection();
                ResetDrawingBezierCurve();
            }
            //Else just reset
            else
            {
                //Reset both points' style to normal
                m_SelectedInPoint.style = s_SkinsLibrary.s_ConnectionPointStyleNormal;
                m_SelectedOutPoint.style = s_SkinsLibrary.s_ConnectionPointStyleNormal;

                ResetDrawingBezierCurve();
            }
        }

    }

    void OnClickOutPoint(ConnectionPoint connectionPoint)
    {
        //Set currently selected inpoint as the inputted connection point
        m_SelectedOutPoint = connectionPoint;

        //Set the style to show  that point has been selected
        m_SelectedOutPoint.style = s_SkinsLibrary.s_ConnectionPointStyleSelected;

        if (m_SelectedInPoint != null)
        {
            OutConnectionPoint castedOutPoint = (OutConnectionPoint)m_SelectedOutPoint;

            //Check if selected in point node is same as selected out point npde
            //In this case we dont want them to be the same cause its stupid to 
            //have connection with the same node
            if (m_SelectedOutPoint.parentNode != m_SelectedInPoint.parentNode &&
                castedOutPoint.m_ConnectedNodeID != m_SelectedInPoint.parentNode.NodeID)
            {
                //Set the styles to show  that points has been selected
                m_SelectedInPoint.style = s_SkinsLibrary.s_ConnectionPointStyleSelected;
                m_SelectedOutPoint.style = s_SkinsLibrary.s_ConnectionPointStyleSelected;

                //link
                castedOutPoint.m_ConnectedNodeID = m_SelectedInPoint.parentNode.NodeID;
                m_SelectedInPoint.m_ConnectedNodeID = castedOutPoint.parentNode.NodeID;

                CreateConnection();
                ResetDrawingBezierCurve();
            }
            //Else just reset
            else
            {
                //Reset both points' style to normal
                m_SelectedInPoint.style = s_SkinsLibrary.s_ConnectionPointStyleNormal;
                m_SelectedOutPoint.style = s_SkinsLibrary.s_ConnectionPointStyleNormal;

                ResetDrawingBezierCurve();
            }
        }
    }

    void OnClickRemoveNode(Node nodeToRemove)
    {
        if (m_AllConnectionsInEditor != null)
        {
            Connection[] connectionToRemove = m_AllConnectionsInEditor.FindAll(x => x.inPoint == nodeToRemove.m_InPoint || x.outPoint == nodeToRemove.m_OutPoint).ToArray();

            for (int i = 0; i < connectionToRemove.Length; i++)
            {
                //Reset the connections' in and out points to prevent the points to look unchanged
                connectionToRemove[i].inPoint.style = s_SkinsLibrary.s_ConnectionPointStyleNormal;
                connectionToRemove[i].outPoint.style = s_SkinsLibrary.s_ConnectionPointStyleNormal;
                //Remove the connections
                m_AllConnectionsInEditor.Remove(connectionToRemove[i]);
            }
        }

        //Remove node from selected collection if it is inside
        RemoveNodeFromSelectedCollection(nodeToRemove);
        //If there isnt any connection collection, that means prolly that there is no connection drawn at all in the very beginning
        m_AllNodesInEditor.Remove(nodeToRemove);

    }

    //Cr8 connection list if there arent any and add a new connection into the list
    void CreateConnection()
    {
        //Init the new connection
        m_AllConnectionsInEditor.Add(new Connection(m_SelectedInPoint, m_SelectedOutPoint, OnClickRemoveConnection));
    }

    void OnClickRemoveConnection(Connection connectionToRemove)
    {
        //Up cast the connection pt so that we can reset the outpt's nextnode
        OutConnectionPoint castedOutPoint = (OutConnectionPoint)connectionToRemove.outPoint;
        castedOutPoint.m_ConnectedNodeID = null;

        //Reset the connections' in and out points to prevent the points to look unchanged
        connectionToRemove.inPoint.style = s_SkinsLibrary.s_ConnectionPointStyleNormal;
        connectionToRemove.outPoint.style = s_SkinsLibrary.s_ConnectionPointStyleNormal;

        m_AllConnectionsInEditor.Remove(connectionToRemove);
    }

    void AddNodeToSelectedCollection(Node nodeToAdd)
    {
        //If all selected doesnt contain this node, add it
        if (!m_AllSelectedNodes.Contains(nodeToAdd))
            m_AllSelectedNodes.Add(nodeToAdd);
    }

    void RemoveNodeFromSelectedCollection(Node nodeToRemove)
    {
        //If all selected doesnt contain this node, add it
        if (m_AllSelectedNodes.Contains(nodeToRemove))
            m_AllSelectedNodes.Remove(nodeToRemove);
    }

    #endregion


    #endregion


    #region Saving 

    void TransferNodeData()
    {
        m_AllNodesInEditor.Remove(s_StartNode);
        m_AllNodesInEditor.Remove(s_EndNode);

        LEM_BaseEffect[] lemEffects = new LEM_BaseEffect[m_AllNodesInEditor.Count];
        BaseEffectNode[] allEffectNodes = m_AllNodesInEditor.ConvertAll(x => (BaseEffectNode)x).ToArray();

        for (int i = 0; i < m_AllNodesInEditor.Count; i++)
        {
            lemEffects[i] = allEffectNodes[i].CompileToBaseEffect();
        }

        m_EditingLinearEvent.m_AllEffects = lemEffects;

        m_AllNodesInEditor.Add(s_StartNode);
        m_AllNodesInEditor.Add(s_EndNode);

    }



    ////Function that firstly causes all the nodes in the editor to save themselves and output a TEM_BaseEffect
    //void SaveNodes()
    //{
    //    //Remove Start and End Node from list of all nodes to make the nodes in the list all be effectNodes
    //    m_AllNodesInEditor.Remove(s_StartNode);
    //    m_AllNodesInEditor.Remove(s_EndNode);

    //    //Save all effect nodes' effect references into their own lem_effect cache
    //    //in addition, save all effect nodes' position on the window
    //    for (int i = 0; i < m_AllNodesInEditor.Count; i++)
    //        m_AllNodesInEditor[i].SaveNodeData();

    //    //Then, connect the next effects of all the nodes connected to the Start node (Using recursive function)
    //    StartNode lemStartNode = s_StartNode as StartNode;
    //    List<BaseEffectNode> allConnectedEffectNodes = new List<BaseEffectNode>();
    //    lemStartNode.StartConnection(ref allConnectedEffectNodes);

    //    //Cr8 a temp base eff array to transfer connected base effects to the LE
    //    LEM_BaseEffect[] allConnectedEffects = allConnectedEffectNodes.ConvertAll(x => x.m_BaseEffectSaveFile).ToArray();

    //    //Convert all the nodes in editor (which would be effect nodes by now,) into baseEffectNodes
    //    BaseEffectNode[] allEffectNodesInEditor = Array.ConvertAll(m_AllNodesInEditor.ToArray(), x => (BaseEffectNode)x);

    //    //Then, using the Select function which outputs a ienumerable filled with base effect files, populate allBaseEffects
    //    LEM_BaseEffect[] allUnConnectedEffects = (allEffectNodesInEditor.Select(x => x.m_BaseEffectSaveFile).ToArray().Except(allConnectedEffects)).ToArray();


    //    //Now Save Node Data for start and end nodes
    //    s_StartNode.SaveNodeData();
    //    s_EndNode.SaveNodeData();


    //    //Assign LE with the results depending on whether there is any
    //    m_EditingLinearEvent.m_EffectsConnected = allConnectedEffects == default ? default : allConnectedEffects;

    //    //Debug.Log(m_EditingLinearEvent.m_EffectsConnected[0].m_NodeBaseData.m_NodeID);
    //    //Debug.Log(m_EditingLinearEvent.m_EffectsConnected[0].TEM_Update());

    //    //DestroyGameObject d = (DestroyGameObject)m_EditingLinearEvent.m_EffectsConnected[0];
    //    //Debug.Log(d.m_TargetObject, d.m_TargetObject);


    //    m_EditingLinearEvent.m_EffectsUnConnected = allUnConnectedEffects == default ? default : allUnConnectedEffects;

    //    //Readd the start and end node
    //    m_AllNodesInEditor.Add(s_StartNode);
    //    m_AllNodesInEditor.Add(s_EndNode);

    //}



    #endregion



}