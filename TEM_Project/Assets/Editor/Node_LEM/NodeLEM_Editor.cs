using System.Collections;
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

    //Collection of nodes and connections
    List<Node> m_AllNodesInEditor = new List<Node>();
    List<Connection> m_AllConnectionsInEditor = default;
    List<Node> m_AllSelectedNodes = new List<Node>();

    Node s_StartNode = default, s_EndNode = default;

    public static Node s_CurrentClickedNode = null;
    public static bool? s_CurrentNodeLastRecordedSelectState = null;
    //Check if there is multiple nodes selected
    public static bool s_HaveMultipleNodeSelected => (instance.m_AllSelectedNodes.Count > 0);

    //Canvas drag and offset
    Vector2 m_AmountOfMouseDragThisUpdate = default;
    Vector2 m_AmountOfOffset = default;

    #region GUI Style References
    bool m_SkinsLoaded = false;

    static Dictionary<string, NodeSkinCollection> s_NodeStyleDictionary = new Dictionary<string, NodeSkinCollection>();


    GUIStyle m_InPointStyle = default;
    GUIStyle m_OutPointStyle = default;
    GUIStyle m_ConnectionPointStyleNormal = default;
    GUIStyle m_ConnectionPointStyleSelected = default;

    //Start End Node
    NodeSkinCollection m_StartNodeSkins = default;
    NodeSkinCollection m_EndNodeSkins = default;


    //Function that sets the connection point's style
    void SetConnectionPointStyles(ConnectionPoint point, bool state)
    {
        if (point != null)
        {
            if (state)
            {
                point.style = m_ConnectionPointStyleSelected;
                return;
            }
            point.style = m_ConnectionPointStyleNormal;
        }
    }

    public static GUIStyle s_NodeHeaderStyle = default;
    public static GUIStyle s_NodeTextInputStyle = default;
    public static GUIStyle s_NodeParagraphStyle = default;

    #endregion

    //Selection box variables
    Vector2? m_InitialClickedPosition = default;
    public static Rect s_SelectionBox = default;

    //Bool to record whether user is dragging the execution pins
    //Currently selected in / out points
    ConnectionPoint m_SelectedInPoint = default;
    ConnectionPoint m_SelectedOutPoint = default;

    //NodeCommandInvoker m_CommandInvoker = default;

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

    void OnEnable()
    {
        instance = this;

        //If gui style has not been loaded
        if (!m_SkinsLoaded)
        {
            LoadingNodeSkins();
            m_SkinsLoaded = true;
        }

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

    void OnDisable()
    {
        ResetDrawingBezierCurve();
        ResetEventVariables();
        s_CurrentNodeLastRecordedSelectState = null;
        m_EditingLinearEvent = null;
    }

    //OnGUI is called for rendering and handling GUI events.
    //Something like onvalidate but it is triggered every time a new event occurs (mouse click, mouse move etc) for GUI rendering
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
        //DrawSaveButton();
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
        //If there is a intiitial click position cached
        if (m_InitialClickedPosition != null)
        {
            //Set colour of the box to light blue
            Color boxColour = new Color(0.6f, 0.8f, 1f, .2f);
            Color boxOutLineColour = new Color(0f, 0.298f, 0.6f, 1f);

            Vector2 initialClickPos = (Vector2)m_InitialClickedPosition;

            s_SelectionBox = new Rect(initialClickPos, currentMousePosition - initialClickPos);
            //The coordinates of the editor window is upside down from the usual 
            //the y value increases the more the point goes downwards
            // the x value increases the more the point goes rightwards
            Handles.DrawSolidRectangleWithOutline(s_SelectionBox, boxColour, boxOutLineColour);
        }
    }

    //void DrawSaveButton()
    //{
    //    if (GUI.Button(new Rect(position.width - 100f, 0, 100f, 50f), "Save Effects"))
    //    {
    //        SaveNodes();
    //    }
    //}

    //Creation use only
    void DrawRefreshButton()
    {
        if (GUI.Button(new Rect(position.width - 215f, 0, 100f, 50f), "Refresh"))
        {
            LoadingNodeSkins();
            OnDisable();
            OnEnable();
        }

    }


    #endregion



    //Checks what the current event is right now, and then execute code accordingly
    void ProcessEvents(Event e)
    {
        m_AmountOfMouseDragThisUpdate = Vector2.zero;

        //Do different things based on the event's type
        switch (e.type)
        {

            //If any of the two mouse buttons are down,
            case EventType.MouseDown:

                //Set the currenly clicked node
                s_CurrentClickedNode = m_AllNodesInEditor.Find(x => x.m_Rect.Contains(e.mousePosition));

                //Check if the mouse button down is the right click button
                if (e.button == 1)
                {
                    //and if that the mouse is not clicking on any nodes currently
                    if (s_CurrentClickedNode == null || !s_CurrentClickedNode.IsSelected )
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
                    if (e.alt && m_InitialClickedPosition == null)
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
                        if(m_AllSelectedNodes.Contains(s_StartNode))
                        {
                            s_StartNode.DeselectNode();
                        }

                        if (m_AllSelectedNodes.Contains(s_EndNode))
                        {
                            s_EndNode.DeselectNode();
                        }

                        while (s_HaveMultipleNodeSelected )
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
        //If nodes collection is not null
        if (m_AllNodesInEditor != null)
        {
            //Since the last node is rendered last and is drawn on the top, it should process its events first
            for (int i = m_AllNodesInEditor.Count - 1; i >= 0; i--)
            {
                //If after processing the node events, there is a need for repainting (due to dragging of nodes etc)
                //reason why it is not dragging is because when the first node drags, it returns the true value and GUI.c
                if (m_AllNodesInEditor[i].ProcessNodeEvents(e))
                {
                    //set static var of GUI Changing to be true
                    GUI.changed = true;
                }
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
        NodeSkinCollection nodeSkin = s_NodeStyleDictionary[nameOfNodeType];

        //Initialise the new node 
        newEffectNode.Initialise
            (mousePosition, nodeSkin, m_ConnectionPointStyleNormal,
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

    void OnClickRemoveNode(Node nodeToRemove)
    {
        //Check if there are any connections collection in editor at all
        if (m_AllConnectionsInEditor != null)
        {
            //So if we want to remove a node, we need to clear its connections first
            //that being said, we need to figure out which connections belong to the node we wanna remove 
            //then we remove all of those connections that meet that criteria

            Connection[] connectionToRemove = m_AllConnectionsInEditor.FindAll(x => x.inPoint == nodeToRemove.m_InPoint || x.outPoint == nodeToRemove.m_OutPoint).ToArray();

            for (int i = 0; i < connectionToRemove.Length; i++)
            {
                //Reset the connections' in and out points to prevent the points to look unchanged
                connectionToRemove[i].inPoint.style = m_ConnectionPointStyleNormal;
                connectionToRemove[i].outPoint.style = m_ConnectionPointStyleNormal;
                //Remove the connections
                m_AllConnectionsInEditor.Remove(connectionToRemove[i]);
            }
        }

        //Remove node from selected collection if it is inside
        RemoveNodeFromSelectedCollection(nodeToRemove);
        //If there isnt any connection collection, that means prolly that there is no connection drawn at all in the very beginning
        m_AllNodesInEditor.Remove(nodeToRemove);

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

    #region Node Connection Delegates

    //Onclick in and out points are to be passed as delegates/actions when creating 
    //new nodes so that they can pass it to their in and oout points
    void OnClickInPoint(ConnectionPoint connectionPoint)
    {
        //Set currently selected inpoint as the inputted connection point
        m_SelectedInPoint = connectionPoint;

        //Set the style to show  that point has been selected
        m_SelectedInPoint.style = m_ConnectionPointStyleSelected;

        //If current selected outpoint is not null
        if (m_SelectedOutPoint != null)
        {
            OutConnectionPoint castedOutPoint = (OutConnectionPoint)m_SelectedOutPoint;

            //Check if selected in point node is same as selected out point npde
            //In this case we dont want them to be the same cause its stupid to 
            //have connection with the same node
            //Another thing to check is if the selected inpoint's connected node is equal to selected output node. If it is then dont bother connecting
            if (m_SelectedOutPoint.parentNode != m_SelectedInPoint.parentNode &&
                castedOutPoint.nextNode != m_SelectedInPoint.parentNode)
            {
                //Set the styles to show  that points has been selected
                m_SelectedInPoint.style = m_ConnectionPointStyleSelected;
                m_SelectedOutPoint.style = m_ConnectionPointStyleSelected;

                //Link
                castedOutPoint.nextNode = m_SelectedInPoint.parentNode;
                CreateConnection();
                ResetDrawingBezierCurve();
            }
            //Else just reset
            else
            {
                //Reset both points' style to normal
                m_SelectedInPoint.style = m_ConnectionPointStyleNormal;
                m_SelectedOutPoint.style = m_ConnectionPointStyleNormal;

                ResetDrawingBezierCurve();
            }
        }

    }

    void OnClickOutPoint(ConnectionPoint connectionPoint)
    {
        //Set currently selected inpoint as the inputted connection point
        m_SelectedOutPoint = connectionPoint;

        //Set the style to show  that point has been selected
        m_SelectedOutPoint.style = m_ConnectionPointStyleSelected;

        if (m_SelectedInPoint != null)
        {
            OutConnectionPoint castedOutPoint = (OutConnectionPoint)m_SelectedOutPoint;

            //Check if selected in point node is same as selected out point npde
            //In this case we dont want them to be the same cause its stupid to 
            //have connection with the same node
            if (m_SelectedOutPoint.parentNode != m_SelectedInPoint.parentNode &&
                castedOutPoint.nextNode != m_SelectedInPoint.parentNode)
            {
                //Set the styles to show  that points has been selected
                m_SelectedInPoint.style = m_ConnectionPointStyleSelected;
                m_SelectedOutPoint.style = m_ConnectionPointStyleSelected;

                //link
                castedOutPoint.nextNode = m_SelectedInPoint.parentNode;
                CreateConnection();
                ResetDrawingBezierCurve();
            }
            //Else just reset
            else
            {
                //Reset both points' style to normal
                m_SelectedInPoint.style = m_ConnectionPointStyleNormal;
                m_SelectedOutPoint.style = m_ConnectionPointStyleNormal;

                ResetDrawingBezierCurve();
            }
        }
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
        castedOutPoint.nextNode = null;

        //Reset the connections' in and out points to prevent the points to look unchanged
        connectionToRemove.inPoint.style = m_ConnectionPointStyleNormal;
        connectionToRemove.outPoint.style = m_ConnectionPointStyleNormal;

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

    void LoadingNodeSkins()
    {
        //Reset dictionary
        s_NodeStyleDictionary.Clear();
        s_NodeStyleDictionary = new Dictionary<string, NodeSkinCollection>();


        //Initialising public static node title styles
        s_NodeHeaderStyle = new GUIStyle();
        s_NodeHeaderStyle.fontSize = 13;

        s_NodeTextInputStyle = GUI.skin.GetStyle("textField");
        s_NodeTextInputStyle.fontSize = 10;

        s_NodeParagraphStyle = new GUIStyle();
        s_NodeParagraphStyle.fontSize = 10;

        string[] namesOfNodeEffectType = LEMDictionary.GetNodeTypeKeys();

        //The number range covers all the skins needed for gameobject effect related nodes
        //Naming convention is very important here
        for (int i = 0; i < namesOfNodeEffectType.Length; i++)
        {
            NodeSkinCollection skinCollection = new NodeSkinCollection();
            //Load the node skins texture
            skinCollection.light_normal = Resources.Load<Texture2D>("NodeBackground/light_" + namesOfNodeEffectType[i]);
            skinCollection.light_selected = Resources.Load<Texture2D>("NodeBackground/light_" + namesOfNodeEffectType[i] + "_Selected");
            skinCollection.textureToRender = skinCollection.light_normal;

            s_NodeStyleDictionary.Add(namesOfNodeEffectType[i], skinCollection);
        }

        m_StartNodeSkins.light_normal = Resources.Load<Texture2D>("StartEnd/start");
        m_StartNodeSkins.light_selected = Resources.Load<Texture2D>("StartEnd/start_Selected");
        m_StartNodeSkins.textureToRender = m_StartNodeSkins.light_normal;

        m_EndNodeSkins.light_normal = Resources.Load<Texture2D>("StartEnd/end");
        m_EndNodeSkins.light_selected = Resources.Load<Texture2D>("StartEnd/end_Selected");
        m_EndNodeSkins.textureToRender = m_EndNodeSkins.light_normal;

        //Initialise the execution pin style for normal and selected pins
        m_ConnectionPointStyleNormal = new GUIStyle();
        m_ConnectionPointStyleNormal.normal.background = Resources.Load<Texture2D>("NodeIcons/light_ExecutionPin");
        m_ConnectionPointStyleNormal.active.background = Resources.Load<Texture2D>("NodeIcons/light_ExecutionPin_Selected");

        //Invert the two pins' backgrounds so that the user will be able to know what will happen if they press it
        m_ConnectionPointStyleSelected = new GUIStyle();
        m_ConnectionPointStyleSelected.normal.background = m_ConnectionPointStyleNormal.active.background;
        m_ConnectionPointStyleSelected.active.background = m_ConnectionPointStyleNormal.normal.background;


        //Load the in and out point gui styles
        m_InPointStyle = new GUIStyle();
        m_InPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        m_InPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;

        m_OutPointStyle = new GUIStyle();
        m_OutPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        m_OutPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;


    }

    void InitialiseStartEndNodes()
    {
        if (s_StartNode == null)
        {
            s_StartNode = new StartNode();
            //Initialise the new node 
            s_StartNode.Initialise
                (new Vector2(EditorGUIUtility.currentViewWidth * 0.5f, 50f), m_StartNodeSkins, m_ConnectionPointStyleNormal,
                OnClickInPoint, OnClickOutPoint, AddNodeToSelectedCollection, RemoveNodeFromSelectedCollection);

            //Add the node into collection in editor
            m_AllNodesInEditor.Add(s_StartNode);
        }

        if (s_EndNode == null)
        {
            s_EndNode = new EndNode();
            //Initialise the new node 
            s_EndNode.Initialise
                (new Vector2(EditorGUIUtility.currentViewWidth * 0.85f, 50f), m_EndNodeSkins, m_ConnectionPointStyleNormal,
                OnClickInPoint, OnClickOutPoint, AddNodeToSelectedCollection, RemoveNodeFromSelectedCollection);

            //Add the node into collection in editor
            m_AllNodesInEditor.Add(s_EndNode);
        }

    }

    #region Saving 

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


}
