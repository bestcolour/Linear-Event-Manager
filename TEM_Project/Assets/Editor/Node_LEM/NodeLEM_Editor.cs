using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System;
using LEM_Effects;
using System.Linq;

public class NodeLEM_Editor : EditorWindow
{
    public static NodeLEM_Editor instance = default;
    public static LinearEvent s_CurrentLE = default;

    //For saving 
    List<Node> m_AllNodesInEditor = new List<Node>();
    List<Node> AllNodesInEditor => instance.m_AllNodesInEditor;

    Dictionary<string, BaseEffectNode> m_AllEffectsNodeInEditor = new Dictionary<string, BaseEffectNode>();
    Dictionary<string, BaseEffectNode> AllEffectsNodeInEditor => instance.m_AllEffectsNodeInEditor;

    //RULE: INPOINT'S CONNECTED NODE ID FIRST THEN OUTPOINT CONNECTED NODE ID
    Dictionary<Tuple<string, string>, Connection> m_AllConnectionsDictionary = new Dictionary<Tuple<string, string>, Connection>();
    Dictionary<Tuple<string, string>, Connection> AllConnectionsDictionary => instance.m_AllConnectionsDictionary;

    Node s_StartNode = default;
    Node StartNode { get { return instance.s_StartNode; } set { instance.s_StartNode = value; } }

    #region Process Event Variables

    List<Node> m_AllSelectedNodes = new List<Node>();

    static Node s_CurrentClickedNode = null;
    public static Node CurrentClickedNode => s_CurrentClickedNode;
    public static bool? CurrentNodeLastRecordedSelectState { get; set; }

    //Check if there is multiple nodes selected
    public static bool s_HaveMultipleNodeSelected => (instance.m_AllSelectedNodes.Count > 0);

    //Canvas drag and offset
    Vector2 m_AmountOfMouseDragThisUpdate = default;
    Vector2 m_AmountOfOffset = default;

    //Selection box variables
    Vector2? m_InitialClickedPosition = default;
    public static Rect s_SelectionBox = default;
    static readonly Color s_SelectionBoxColour = new Color(0.6f, 0.8f, 1f, .2f);
    static readonly Color s_SelectionBoxOutlineColour = new Color(0f, 0.298f, 0.6f, 1f);


    #region Connection Point Variables
    /// <summary>
    /// Sets the skin of a connection point without checking if it is connected. 
    /// </summary>
    /// <param name="connectionPoint"></param>
    /// <param name="isSelected">For Selected skin put 1 else for normal skin, put x != 1 </param>
    void TrySetConnectionPointSkin(ConnectionPoint connectionPoint, int isSelected)
    {
        if (connectionPoint == null)
            return;

        connectionPoint.m_Style = isSelected == 1 ? LEMStyleLibrary.s_ConnectionPointStyleSelected : LEMStyleLibrary.s_ConnectionPointStyleNormal;
    }

    void TrySetConnectionPoint(ConnectionPoint connectionPoint)
    {
        if (connectionPoint == null)
            return;

        connectionPoint.m_Style = connectionPoint.IsConnected ? LEMStyleLibrary.s_ConnectionPointStyleSelected : LEMStyleLibrary.s_ConnectionPointStyleNormal;
    }

    struct ConnectionPointState
    {
        public const int UNSELECTED = 0, SELECTED = 1;
    }

    ConnectionPoint m_SelectedInPoint = default, m_SelectedOutPoint = default;
    #endregion

    #region Zooming Variables

    const float k_MinScale = 0.2f, k_MaxScale = 1.0f, k_ScaleChangeRate = 0.2f, k_SlowScaleChangeRate = 0.05f;
    static float s_CurrentScaleFactor = 1f;
    float ScaleFactor { get { return s_CurrentScaleFactor; } set { s_CurrentScaleFactor = Mathf.Clamp(value, k_MinScale, k_MaxScale); } }
    #endregion

    #region SearchBox
    //Search box variables
    public static bool s_IsSearchBoxActive = false;

    static LEM_SearchBox s_SearchBox = default;

    void OnInputChange(string result)
    {
        s_SearchBox.ClearResults();

        string currentNodeType;

        //If searchbox is drawn for the first time 
        if (String.IsNullOrEmpty(result))
        {
            for (int i = 0; i < LEMDictionary.s_NodeTypeKeys.Length; i++)
            {
                currentNodeType = LEMDictionary.s_NodeTypeKeys[i];
                s_SearchBox.AddResult(currentNodeType);
            }
            return;
        }

        //Else if result isnt empty or null when search box is right clicked to be drawn
        for (int i = 0; i < LEMDictionary.s_NodeTypeKeys.Length; i++)
        {
            currentNodeType = LEMDictionary.s_NodeTypeKeys[i];

            if (currentNodeType.CaseInsensitiveContains(result))
                s_SearchBox.AddResult(currentNodeType);
        }
    }

    void OnConfirm(string result, Vector2 mousePos)
    {
        mousePos *= 1 / ScaleFactor;
        //instance.CreateEffectNode(mousePos * 1 / ScaleFactor, result);
        s_CommandInvoker.InvokeCommand(new CreateNodeCommand(mousePos, result));
    }
    #endregion

    #endregion

    #region NodeInvoker

    static NodeCommandInvoker s_CommandInvoker = default;
    bool m_IsDragging = default;

    #endregion
    static Texture2D s_EditorBackGroundTexture = default;


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

        s_CurrentLE = linearEvent;
        instance.LoadFromLinearEvent();

    }

    #region Initialisation

    void OnEnable()
    {

        instance = this;

        LEMStyleLibrary.LoadLibrary();
        LEMDictionary.LoadDictionary();

        InitialiseSkin();

        if (s_CommandInvoker == null)
        {
            s_CommandInvoker = new NodeCommandInvoker(CreateEffectNode, RecreateEffectNode, TryToRestichConnections, DeleteNodes, MoveNodes, CreateConnection, TryToRemoveConnection);
        }

        if (m_AllNodesInEditor == null)
        {
            m_AllNodesInEditor = new List<Node>();
        }

        if (m_AllEffectsNodeInEditor == default)
        {
            m_AllEffectsNodeInEditor = new Dictionary<string, BaseEffectNode>();
        }

        if (m_AllConnectionsDictionary == null)
        {
            m_AllConnectionsDictionary = new Dictionary<Tuple<string, string>, Connection>();
        }

        if (m_AllSelectedNodes == null)
        {
            m_AllSelectedNodes = new List<Node>();
        }

        if (s_SearchBox == null)
        {
            s_SearchBox = new LEM_SearchBox(instance.OnInputChange, instance.OnConfirm, 15, 250, 325);
        }

        InitialiseStartEndNodes();

    }

    void InitialiseSkin()
    {
        s_EditorBackGroundTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        s_EditorBackGroundTexture.SetPixel(0, 0, new Color(0.227f, 0.216f, 0.212f));
        s_EditorBackGroundTexture.Apply();
    }

    void InitialiseStartEndNodes()
    {
        if (StartNode == null)
        {
            CreateBasicNode(new Vector2(EditorGUIUtility.currentViewWidth * 0.5f, 50f), "StartNode", out Node startTempNode);
            StartNode = startTempNode;
        }
    }

    #endregion

    #region Resets

    void ResetEventVariables()
    {
        m_InitialClickedPosition = null;
        m_IsDragging = false;
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
        CurrentNodeLastRecordedSelectState = null;
        s_CurrentLE = null;
        s_IsSearchBoxActive = false;
    }

    //Called when window is closed
    private void OnDestroy()
    {
        StartNode = null;
        ResetDrawingBezierCurve();
        ResetEventVariables();
        CurrentNodeLastRecordedSelectState = null;
        s_CurrentLE = null;
        m_AllNodesInEditor = null;
        m_AllEffectsNodeInEditor = null;
        m_AllConnectionsDictionary = null;
        LEM_InspectorEditor.s_IsLoaded = false;
        s_IsSearchBoxActive = false;
        s_CommandInvoker = null;
    }


    #endregion

    void OnGUI()
    {
        Event currentEvent = Event.current;

        //Draw background of for the window
        GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), s_EditorBackGroundTexture, ScaleMode.StretchToFill);


        DrawGrid(20 * ScaleFactor, 0.2f, Color.gray);
        DrawGrid(100 * ScaleFactor, 0.4f, Color.gray);
        //Draw graphics that are zoomable
        EditorZoomFeature.BeginZoom(ScaleFactor, new Rect(0f, 0f, Screen.width, Screen.height));
        Vector2 currMousePos = currentEvent.mousePosition;
        //Draw the nodes first
        DrawNodes();


        DrawConnections();
        DrawConnectionLine(currentEvent);
        DrawSelectionBox(currMousePos);

        EditorZoomFeature.EndZoom();
        HandleSearchBox(currentEvent);


        DrawSaveButton();
        DrawRefreshButton();

        //Then process the events that occured from unity's events (events are like clicks,drag etc)
        ProcessEvents(currentEvent, currMousePos);
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
        if (AllNodesInEditor != null)
            for (int i = 0; i < AllNodesInEditor.Count; i++)
                AllNodesInEditor[i].Draw();
    }

    void DrawConnections()
    {
        Tuple<string, string>[] allTupleKeys = AllConnectionsDictionary.Keys.ToArray();
        for (int i = 0; i < allTupleKeys.Length; i++)
            AllConnectionsDictionary[allTupleKeys[i]].Draw();
    }

    //This is the realtime drawing of a bezier curve line to let users visualise
    //Draws connection lines when the user clickso n a connection point to connect with another
    void DrawConnectionLine(Event e)
    {
        //if player clicked on a inpoint alrdy and havent clikced on a output yet,
        if (m_SelectedInPoint != null && m_SelectedOutPoint == null)
        {
            //Just repeat the code in connection script but replace the variables
            // replace outpoint with mouseposition
            Handles.DrawBezier(
                m_SelectedInPoint.m_Rect.center,
                e.mousePosition,
                m_SelectedInPoint.m_Rect.center + Vector2.left * 50f,
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
                m_SelectedOutPoint.m_Rect.center,
                e.mousePosition + Vector2.left * 50f,
                m_SelectedOutPoint.m_Rect.center - Vector2.left * 50f,
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
            SaveToLinearEvent();
    }

    //Creation use only
    void DrawRefreshButton()
    {
        if (GUI.Button(new Rect(position.width - 215f, 0, 100f, 50f), "Refresh"))
        {
            LEMStyleLibrary.LoadLibrary();
            OnDisable();
            OnEnable();
        }
    }

    void HandleSearchBox(Event e)
    {
        if (s_IsSearchBoxActive)
            s_SearchBox.HandleSearchBox(e);
    }

    #endregion

    //Checks what the current event is right now, and then execute code accordingly
    void ProcessEvents(Event e, Vector2 currMousePosition)
    {
        m_AmountOfMouseDragThisUpdate = Vector2.zero;
        switch (e.type)
        {
            case EventType.ScrollWheel:
                int signOfChange = 0;
                float changeRate = 0f;

                signOfChange = e.delta.y > 0 ? -1 : 1;
                //If alt key is pressed,
                changeRate = e.alt ? k_SlowScaleChangeRate : k_ScaleChangeRate;

                ScaleFactor += signOfChange * changeRate;

                e.Use();
                break;

            case EventType.MouseDown:

                //Set the currenly clicked node
                s_CurrentClickedNode = AllNodesInEditor.Find(x => x.m_TotalRect.Contains(currMousePosition));

                //Check if the mouse button down is the right click button
                if (e.button == 1)
                {

                    //and if that the mouse is not clicking on any nodes currently
                    if (s_CurrentClickedNode == null || !s_CurrentClickedNode.IsSelected)
                    {
                        //Open a custom created that allows creation of more nodes
                        s_SearchBox.Position = currMousePosition * ScaleFactor;
                        s_IsSearchBoxActive = true;
                        s_SearchBox.TriggerOnInputOnStart();

                        e.Use();
                        //ProcessContextMenu(currMousePosition);
                        return;
                    }


                    //Else, open the node's context menu
                    ProcessNodeContextMenu();
                }

                else if (e.button == 0)
                {
                    //If mouse indeed doesnt clicks on a node,
                    if (s_CurrentClickedNode == null)
                    {
                        //Set initial position for drawing selection box
                        if (!e.alt)
                        {
                            //Reset everything
                            m_InitialClickedPosition = currMousePosition;

                            TrySetConnectionPoint(m_SelectedInPoint);
                            TrySetConnectionPoint(m_SelectedOutPoint);
                            ResetDrawingBezierCurve();

                        }
                    }
                    //Else if current clicked node isnt null
                    else
                    {
                        AllNodesInEditor.RearrangeElement(s_CurrentClickedNode, AllNodesInEditor.Count - 1);
                    }

                    //Remove focus on the controls when user clicks on something regardless if it is a node or not because apparently this doesnt get
                    //called when i click on input/text fields
                    GUI.FocusControl(null);

                    s_IsSearchBoxActive = false;

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
                    //If user is currently planning to drag a node and wasnt draggin the previous paint,
                    else if (s_CurrentClickedNode != null && !m_IsDragging)
                    {
                        m_IsDragging = true;
                        s_CommandInvoker.InvokeCommand(new MoveNodeCommand(m_AllSelectedNodes.ToArray()));
                    }
                }

                break;

            //If user releases the mouse
            case EventType.MouseUp:

                ResetEventVariables();

                break;

            //If the user presses a keyboard keybutton
            case EventType.KeyUp:

                //Dont do any commands when u r dragging
                if (m_IsDragging)
                    return;

                if (e.keyCode == KeyCode.Delete)
                {
                    GUI.FocusControl(null);

                    //Remove start and end node 
                    if (m_AllSelectedNodes.Contains(StartNode))
                    {
                        StartNode.DeselectNode();
                    }

                    s_CommandInvoker.InvokeCommand(new DeleteNodeCommand(Array.ConvertAll(m_AllSelectedNodes.ToArray(), x => (BaseEffectNode)x)));
                    //Skip everything else and repaint
                    e.Use();
                }
                else if (e.keyCode == KeyCode.Escape)
                {

                    TrySetConnectionPoint(m_SelectedInPoint);
                    TrySetConnectionPoint(m_SelectedOutPoint);
                    s_IsSearchBoxActive = false;
                    ResetDrawingBezierCurve();
                }
                //Else if control is held down,
                else if (e.control)
                {
                    //Undo
                    if(e.keyCode == KeyCode.Q)
                    {
                        s_CommandInvoker.UndoCommand();
                        e.Use();
                    }
                    //Redo
                    else if (e.keyCode == KeyCode.W)
                    {
                        s_CommandInvoker.RedoCommand();
                        e.Use();
                    }
                    //Copy
                    else if(e.keyCode == KeyCode.C)
                    {
                        //Remove start and end node 
                        if (m_AllSelectedNodes.Contains(StartNode))
                        {
                            StartNode.DeselectNode();
                        }

                        s_CommandInvoker.CopyToClipBoard(Array.ConvertAll(m_AllSelectedNodes.ToArray(), x => (BaseEffectNode)x));
                        e.Use();
                    }
                    //Cut
                    else if (e.keyCode == KeyCode.X)
                    {
                        //Remove start and end node 
                        if (m_AllSelectedNodes.Contains(StartNode))
                        {
                            StartNode.DeselectNode();
                        }

                        s_CommandInvoker.InvokeCommand(new CutCommand(Array.ConvertAll(m_AllSelectedNodes.ToArray(), x => (BaseEffectNode)x)));
                        e.Use();
                    }
                    //Paste
                    else if(e.keyCode == KeyCode.V)
                    {
                        s_CommandInvoker.InvokeCommand(new PasteCommand());
                        e.Use();
                    }
                }

                break;
        }
    }

    void ProcessNodeEvents(Event e)
    {
        //Check current event once and then tell all the nodes to handle that event so they dont have to check
        switch (e.type)
        {
            case EventType.MouseDown:

                for (int i = AllNodesInEditor.Count - 1; i >= 0; i--)
                    if (AllNodesInEditor[i].HandleMouseDown(e))
                        GUI.changed = true;
                break;

            case EventType.MouseUp:
                for (int i = AllNodesInEditor.Count - 1; i >= 0; i--)
                    if (AllNodesInEditor[i].HandleMouseUp(e))
                        GUI.changed = true;
                break;

            case EventType.MouseDrag:
                Vector2 convertedDelta = e.delta / ScaleFactor;

                for (int i = AllNodesInEditor.Count - 1; i >= 0; i--)
                    if (AllNodesInEditor[i].HandleMouseDrag(e, convertedDelta))
                        GUI.changed = true;
                break;

        }
    }

    #region Node Editor Events Functions

    #region Creating and Deleting Node Types
    //This is used for when you wanna create a new node
    BaseEffectNode CreateEffectNode(Vector2 mousePosition, string nameOfNodeType)
    {
        BaseEffectNode newEffectNode = LEMDictionary.GetNodeObject(nameOfNodeType) as BaseEffectNode;

        //Get the respective skin from the collection of nodeskin
        NodeSkinCollection nodeSkin = LEMStyleLibrary.s_WhiteBackGroundSkin;

        //Initialise the new node 
        newEffectNode.Initialise
            (mousePosition,
            nodeSkin,
            LEMStyleLibrary.s_ConnectionPointStyleNormal,
            OnClickInPoint,
            OnClickOutPoint,
            TryToAddNodeToSelectedCollection,
            TryToRemoveNodeFromSelectedCollection,
            LEMStyleLibrary.s_NodeColourDictionary[nameOfNodeType]
            );

        newEffectNode.GenerateNodeID();

        //Add the node into collection in editor
        AllNodesInEditor.Add(newEffectNode);
        AllEffectsNodeInEditor.Add(newEffectNode.NodeID, newEffectNode);
        return newEffectNode;
    }

    //This is used for loading and probably undoing/redoing from a linear event
    BaseEffectNode RecreateEffectNode(Vector2 positionToSet, string nameOfNodeType, string idToSet)
    {
        BaseEffectNode newEffectNode = LEMDictionary.GetNodeObject(nameOfNodeType) as BaseEffectNode;

        //Get the respective skin from the collection of nodeskin
        NodeSkinCollection nodeSkin = LEMStyleLibrary.s_WhiteBackGroundSkin;

        //Initialise the new node 
        newEffectNode.Initialise
            (positionToSet,
            nodeSkin,
            LEMStyleLibrary.s_ConnectionPointStyleNormal,
            OnClickInPoint,
            OnClickOutPoint,
            TryToAddNodeToSelectedCollection,
            TryToRemoveNodeFromSelectedCollection,
            LEMStyleLibrary.s_NodeColourDictionary[nameOfNodeType]
            );

        newEffectNode.SetNodeID(idToSet);

        //Add the node into collection in editor
        AllNodesInEditor.Add(newEffectNode);
        AllEffectsNodeInEditor.Add(newEffectNode.NodeID, newEffectNode);
        return newEffectNode;
    }

    //These two functions are mainly used for creating and loading start node
    void CreateBasicNode(Vector2 mousePosition, string nameOfNodeType, out Node newBasicNode)
    {
        Node basicNode = LEMDictionary.GetNodeObject(nameOfNodeType) as Node;

        //Get the respective skin from the collection of nodeskin
        NodeSkinCollection nodeSkin = LEMStyleLibrary.s_WhiteBackGroundSkin;

        //Initialise the new node 
        basicNode.Initialise
            (mousePosition,
            nodeSkin,
            LEMStyleLibrary.s_ConnectionPointStyleNormal,
            OnClickInPoint,
            OnClickOutPoint,
            TryToAddNodeToSelectedCollection,
            TryToRemoveNodeFromSelectedCollection,
            LEMStyleLibrary.s_NodeColourDictionary[nameOfNodeType]
            );

        basicNode.GenerateNodeID();

        //Add the node into collection in editor
        AllNodesInEditor.Add(basicNode);

        newBasicNode = basicNode;
    }

    void RecreateBasicNode(Vector2 positionToSet, string nameOfNodeType, string idToSet, out Node newlyCreatedNode)
    {
        Node newNode = LEMDictionary.GetNodeObject(nameOfNodeType) as Node;

        //Get the respective skin from the collection of nodeskin
        NodeSkinCollection nodeSkin = LEMStyleLibrary.s_WhiteBackGroundSkin;

        //Initialise the new node 
        newNode.Initialise
            (positionToSet,
            nodeSkin,
            LEMStyleLibrary.s_ConnectionPointStyleNormal,
            OnClickInPoint,
            OnClickOutPoint,
            TryToAddNodeToSelectedCollection,
            TryToRemoveNodeFromSelectedCollection,
            LEMStyleLibrary.s_NodeColourDictionary[nameOfNodeType]
            );

        newNode.SetNodeID(idToSet);

        //Add the node into collection in editor
        AllNodesInEditor.Add(newNode);
        newlyCreatedNode = newNode;
    }

    void DeleteNodes(BaseEffectNode[] nodesToBeDeleted)
    {
        for (int i = 0; i < nodesToBeDeleted.Length; i++)
            OnClickRemoveNode(nodesToBeDeleted[i]);
    }

    #endregion

    void ProcessNodeContextMenu()
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
        //Convert delta value for canvas dragging
        delta /= ScaleFactor;
        m_AmountOfMouseDragThisUpdate = delta;

        //Convert once more for node drag (idk but its a magic number)
        delta /= ScaleFactor;

        //Update all the node's positions as well
        if (AllNodesInEditor != null)
        {
            for (int i = 0; i < AllNodesInEditor.Count; i++)
            {
                AllNodesInEditor[i].Drag(delta);
            }
        }

    }

    void MoveNodes(string[] nodeIDsMoved, ref Vector2[] previousTopRectPositions, ref Vector2[] previousMidRectPositions, ref Vector2[] previousTotalRectPositions)
    {
        Vector2 currentNodePosition;

        //Firstly, check if there is start node in the nodeidsmoved
        #region Start Node Revert to avoid O(n^2) operation

        int startNodeInt = 0;
        while (startNodeInt < nodeIDsMoved.Length)
        {
            if (nodeIDsMoved[startNodeInt] == StartNode.NodeID)
                break;

            startNodeInt++;
        }

        //if startNodeInt is not Length of nodeIDsMoved, that means startNodeint is found inside nodeIDsMoved
        if (startNodeInt != nodeIDsMoved.Length)
        {
            //Do revert/redo movenode command (they r basically the same)
            currentNodePosition = StartNode.m_TopRect.position;
            StartNode.m_TopRect.position = previousTopRectPositions[startNodeInt];
            previousTopRectPositions[startNodeInt] = currentNodePosition;

            currentNodePosition = StartNode.m_MidRect.position;
            StartNode.m_MidRect.position = previousMidRectPositions[startNodeInt];
            previousMidRectPositions[startNodeInt] = currentNodePosition;

            currentNodePosition = StartNode.m_TotalRect.position;
            StartNode.m_TotalRect.position = previousTotalRectPositions[startNodeInt];
            previousTotalRectPositions[startNodeInt] = currentNodePosition;
        }
        else
        {
            //If startnode isnt in the array of moved nodes,
            startNodeInt = -1;
        }

        #endregion

        //All thats left are effect nodes so we can just use the dictionary to get the nodes instead of using AllNodesInEditor.Find()
        //Revert all the node's positions to the prev positions but before that, save that position in a local var to reassign to prev pos 
        for (int i = 0; i < nodeIDsMoved.Length; i++)
        {
            //Skip startnode id 
            if (i == startNodeInt)
                continue;

            currentNodePosition = AllEffectsNodeInEditor[nodeIDsMoved[i]].m_TopRect.position;
            AllEffectsNodeInEditor[nodeIDsMoved[i]].m_TopRect.position = previousTopRectPositions[i];
            previousTopRectPositions[i] = currentNodePosition;

            currentNodePosition = AllEffectsNodeInEditor[nodeIDsMoved[i]].m_MidRect.position;
            AllEffectsNodeInEditor[nodeIDsMoved[i]].m_MidRect.position = previousMidRectPositions[i];
            previousMidRectPositions[i] = currentNodePosition;

            currentNodePosition = AllEffectsNodeInEditor[nodeIDsMoved[i]].m_TotalRect.position;
            AllEffectsNodeInEditor[nodeIDsMoved[i]].m_TotalRect.position = previousTotalRectPositions[i];
            previousTotalRectPositions[i] = currentNodePosition;
        }
    }

    void TryToRestichConnections(LEM_BaseEffect currentEffect)
    {
        if (!currentEffect.m_NodeBaseData.HasAtLeastOneNextPointNode)
            return;

        //And if there is one next point node
        if (currentEffect.m_NodeBaseData.HasOnlyOneNextPointNode)
        {
            if (!String.IsNullOrEmpty(currentEffect.m_NodeBaseData.m_NextPointsIDs[0])
                &&
                !AllEffectsNodeInEditor[currentEffect.m_NodeBaseData.m_NodeID].m_OutPoint.IsConnected)
            {
                CreateConnection(
                          AllEffectsNodeInEditor[currentEffect.m_NodeBaseData.m_NextPointsIDs[0]].m_InPoint,
                          AllEffectsNodeInEditor[currentEffect.m_NodeBaseData.m_NodeID].m_OutPoint
                          );
            }

        }
        else
        {
            //that means that this effect node is a special node which has multiple outputs
            //Restich for those nodes
            for (int n = 0; n < currentEffect.m_NodeBaseData.m_NextPointsIDs.Length; n++)
                //If the keys current next point id is not empty or null and the outpoint  isnt connected,
                if (!String.IsNullOrEmpty(currentEffect.m_NodeBaseData.m_NextPointsIDs[n])
                    &&
                    !AllEffectsNodeInEditor[currentEffect.m_NodeBaseData.m_NodeID].m_OutPoint.IsConnected)
                {
                    CreateConnection(
                    AllEffectsNodeInEditor[currentEffect.m_NodeBaseData.m_NextPointsIDs[n]].m_InPoint,
                    AllEffectsNodeInEditor[currentEffect.m_NodeBaseData.m_NodeID].m_OutPoint
                    );
                }

        }
    }

    void OnClickInPoint(ConnectionPoint connectionPoint)
    {
        //Check if player already has a selected in point and if so set its prev skin to normal
        if (m_SelectedInPoint != null)
        {
            TrySetConnectionPoint(m_SelectedInPoint);
            m_SelectedInPoint = connectionPoint;
            //After dealing with old connectionpt skin,  sets the inpoint to selected skin
            TrySetConnectionPointSkin(m_SelectedInPoint, ConnectionPointState.SELECTED);
            return;
        }

        m_SelectedInPoint = connectionPoint;
        TrySetConnectionPointSkin(m_SelectedInPoint, ConnectionPointState.SELECTED);

        //If current selected outpoint is not null
        if (m_SelectedOutPoint == null)
            return;

        //Check if selected in point node is same as selected out point npde
        //Another thing to check is if the selected inpoint's connected node is equal to selected output node. If it is then dont bother connecting
        if (m_SelectedOutPoint.m_ParentNode != m_SelectedInPoint.m_ParentNode &&
            m_SelectedOutPoint.GetConnectedNodeID(0) != m_SelectedInPoint.m_ParentNode.NodeID)
        {
            //Remove the old connection if outpoint has an old connection
            if (m_SelectedOutPoint.IsConnected)
            {
                OnClickRemoveConnection(AllConnectionsDictionary
                      [new Tuple<string, string>(m_SelectedOutPoint.GetConnectedNodeID(0), m_SelectedOutPoint.m_ParentNode.NodeID)]
                      );
            }

            s_CommandInvoker.InvokeCommand(new CreateConnectionCommand(m_SelectedInPoint.m_ParentNode.NodeID, m_SelectedOutPoint.m_ParentNode.NodeID));

            ResetDrawingBezierCurve();
        }
        else
        {
            //Reset both points' style to normal
            TrySetConnectionPoint(m_SelectedInPoint);
            TrySetConnectionPoint(m_SelectedOutPoint);

            ResetDrawingBezierCurve();
        }

    }

    void OnClickOutPoint(ConnectionPoint connectionPoint)
    {

        //Check if player already has a selected out point and wishes to choose another one
        if (m_SelectedOutPoint != null)
        {
            TrySetConnectionPoint(m_SelectedOutPoint);
            m_SelectedOutPoint = connectionPoint;
            TrySetConnectionPointSkin(m_SelectedOutPoint, ConnectionPointState.SELECTED);
            return;
        }

        m_SelectedOutPoint = connectionPoint;
        //TrySetConnectionPoint(m_SelectedOutPoint/*, true*/);
        TrySetConnectionPointSkin(m_SelectedOutPoint, ConnectionPointState.SELECTED);

        if (m_SelectedInPoint == null)
            return;

        //Check if selected in point node is same as selected out point npde
        //In this case we dont want them to be the same cause its stupid to 
        //have connection with the same node
        if (m_SelectedOutPoint.m_ParentNode != m_SelectedInPoint.m_ParentNode &&
            m_SelectedOutPoint.GetConnectedNodeID(0) != m_SelectedInPoint.m_ParentNode.NodeID)
        {
            //Remove the old connection if outpoint has an old connection
            if (m_SelectedOutPoint.IsConnected)
            {
                OnClickRemoveConnection(
                   AllConnectionsDictionary[new Tuple<string, string>(m_SelectedOutPoint.GetConnectedNodeID(0), m_SelectedOutPoint.m_ParentNode.NodeID)]
                   );
            }

            s_CommandInvoker.InvokeCommand(new CreateConnectionCommand(m_SelectedInPoint.m_ParentNode.NodeID, m_SelectedOutPoint.m_ParentNode.NodeID));

            ResetDrawingBezierCurve();
        }
        //Else just reset
        else
        {
            //Reset both points' style to normal
            TrySetConnectionPoint(m_SelectedInPoint);
            TrySetConnectionPoint(m_SelectedOutPoint);
            ResetDrawingBezierCurve();

        }
    }

    void OnClickRemoveNode(Node nodeToRemove)
    {
        //Check if there is any connections to be removed from this node

        TryToRemoveConnection(nodeToRemove.m_OutPoint.GetConnectedNodeID(0), nodeToRemove.NodeID);

        //Remove any and allconnections connected to the node's inpoint
        string[] allNodesConnectedToInPoint = nodeToRemove.m_InPoint.GetAllConnectedNodeIDs();
        for (int i = 0; i < allNodesConnectedToInPoint.Length; i++)
        {
            OnClickRemoveConnection(AllConnectionsDictionary[new Tuple<string, string>(nodeToRemove.NodeID, allNodesConnectedToInPoint[i])]);
        }

        //Remove node from selected collection if it is inside
        TryToRemoveNodeFromSelectedCollection(nodeToRemove);

        //O(n) operation only, inother words same as list.Remove( )
        //Need nodeid to be checked cause Node references are lost during command invoker
        int indexOfNodeToRemove = AllNodesInEditor.FindIndex(x => x.NodeID == nodeToRemove.NodeID);
        AllNodesInEditor.RemoveEfficiently(indexOfNodeToRemove);

        if (AllEffectsNodeInEditor.ContainsKey(nodeToRemove.NodeID))
            AllEffectsNodeInEditor.Remove(nodeToRemove.NodeID);
    }

    //Use this if you know exactly what nodes to connect
    void CreateConnection(ConnectionPoint inPoint, ConnectionPoint outPoint)
    {
        //Add connection to dual key dictionary
        AllConnectionsDictionary.Add(
            new Tuple<string, string>(inPoint.m_ParentNode.NodeID, outPoint.m_ParentNode.NodeID),
            new Connection(inPoint, outPoint, OnClickRemoveConnection)
            );

        TrySetConnectionPoint(inPoint);
        TrySetConnectionPoint(outPoint);
    }

    //Use this if you have the ID of the nodes you wish to connect but dont know their identities
    void CreateConnection(string inPointNodeID, string outPointNodeID)
    {
        ConnectionPoint inPoint = default;
        ConnectionPoint outPoint = default;

        //Assign the inpoint n outpoint by first checking if either one of them are start nodes
        if (inPointNodeID == StartNode.NodeID)
        {
            inPoint = StartNode.m_InPoint;
            outPoint = AllEffectsNodeInEditor[outPointNodeID].m_OutPoint;
        }
        else if (outPointNodeID == StartNode.NodeID)
        {
            outPoint = StartNode.m_OutPoint;
            inPoint = AllEffectsNodeInEditor[inPointNodeID].m_InPoint;
        }
        //Else if both em arent start nodes that means both of em are effecNodes
        else
        {
            outPoint = AllEffectsNodeInEditor[outPointNodeID].m_OutPoint;
            inPoint = AllEffectsNodeInEditor[inPointNodeID].m_InPoint;
        }

        //Add connection to dual key dictionary
        AllConnectionsDictionary.Add(
            new Tuple<string, string>(inPointNodeID, outPointNodeID),
            new Connection(inPoint, outPoint, OnClickRemoveConnection)
            );

        TrySetConnectionPoint(inPoint);
        TrySetConnectionPoint(outPoint);
    }

    void OnClickRemoveConnection(Connection connectionToRemove)
    {
        connectionToRemove.m_InPoint.RemoveConnectedNodeID(connectionToRemove.m_OutPoint.m_ParentNode.NodeID);
        connectionToRemove.m_OutPoint.RemoveConnectedNodeID(connectionToRemove.m_InPoint.m_ParentNode.NodeID);

        //Reset the connections' in and out points to prevent the points to look unchanged
        TrySetConnectionPoint(connectionToRemove.m_InPoint);
        TrySetConnectionPoint(connectionToRemove.m_OutPoint);

        //Remove from dictionary
        AllConnectionsDictionary.Remove(
            new Tuple<string, string>(connectionToRemove.m_InPoint.m_ParentNode.NodeID,
            connectionToRemove.m_OutPoint.m_ParentNode.NodeID)
            );
    }

    void TryToRemoveConnection(string inPointNodeID, string outPointNodeID)
    {
        if (AllConnectionsDictionary.TryGetValue(
           new Tuple<string, string>(inPointNodeID,
           outPointNodeID),
           out Connection connectionToRemove))
        {
            //Remove any connections that is connected to the node's outpoint
            OnClickRemoveConnection(connectionToRemove);
        }
    }

    void TryToAddNodeToSelectedCollection(Node nodeToAdd)
    {
        //If all selected doesnt contain this node, add it
        if (!m_AllSelectedNodes.Contains(nodeToAdd))
            m_AllSelectedNodes.Add(nodeToAdd);
    }

    void TryToRemoveNodeFromSelectedCollection(Node nodeToRemove)
    {
        //If all selected doesnt contain this node, add it
        if (m_AllSelectedNodes.Contains(nodeToRemove))
            m_AllSelectedNodes.Remove(nodeToRemove);
    }

    #endregion


    #region Saving and Loading

    void SaveToLinearEvent()
    {
        AllNodesInEditor.Remove(StartNode);

        //Saving starts here
        LEM_BaseEffect[] lemEffects = new LEM_BaseEffect[AllNodesInEditor.Count];
        BaseEffectNode[] allEffectNodes = AllNodesInEditor.ConvertAll(x => (BaseEffectNode)x).ToArray();

        //Clear the dictionary of the currently editting linear event
        s_CurrentLE.m_AllEffectsDictionary = new Dictionary<string, LEM_BaseEffect>();

        //This saves all events regardless of whether they are connected singularly, plurally or disconnected
        for (int i = 0; i < AllNodesInEditor.Count; i++)
        {
            lemEffects[i] = allEffectNodes[i].CompileToBaseEffect();

            //Populate the dictionary in the linear event
            s_CurrentLE.m_AllEffectsDictionary.Add(lemEffects[i].m_NodeBaseData.m_NodeID, lemEffects[i]);
        }


        s_CurrentLE.m_AllEffects = lemEffects;

        //Save start and end node data
        s_CurrentLE.m_StartNodeData = StartNode.SaveNodeData();

        //Saving ends here
        AllNodesInEditor.Add(StartNode);

    }

    void LoadFromLinearEvent()
    {
        #region Loading Events from Dictionary

        if (s_CurrentLE.m_AllEffectsDictionary == null)
            return;

        string[] allKeys = s_CurrentLE.m_AllEffectsDictionary.Keys.ToArray();

        BaseEffectNode newEffectNode;

        //Recreate all the nodes from the dictionary
        for (int i = 0; i < allKeys.Length; i++)
        {
            newEffectNode = RecreateEffectNode(s_CurrentLE.m_AllEffectsDictionary[allKeys[i]].m_NodeBaseData.m_Position,
                s_CurrentLE.m_AllEffectsDictionary[allKeys[i]].m_NodeEffectType,
                s_CurrentLE.m_AllEffectsDictionary[allKeys[i]].m_NodeBaseData.m_NodeID);

            //Load the new node with saved node values values
            newEffectNode.LoadFromBaseEffect(s_CurrentLE.m_AllEffectsDictionary[allKeys[i]]);
        }

        #endregion
        //Do the same for start n end node only if they arent null (they likely wont because onenable runs first)
        //and that there are records of saving them
        if (StartNode != null && s_CurrentLE.m_StartNodeData.m_NodeID != string.Empty)
        {
            OnClickRemoveNode(StartNode);
            RecreateBasicNode(s_CurrentLE.m_StartNodeData.m_Position, "StartNode", s_CurrentLE.m_StartNodeData.m_NodeID, out Node startTempNode);
            StartNode = startTempNode;
        }

        #region Loading Connections From Dictionary

        //Stitch dictionary's their connnection back
        for (int i = 0; i < allKeys.Length; i++)
        {
            //If this nodebase data doesnt even have one nextpoint node id, skip this loop
            if (!s_CurrentLE.m_AllEffectsDictionary[allKeys[i]].m_NodeBaseData.HasAtLeastOneNextPointNode)
                continue;

            TryToRestichConnections(s_CurrentLE.m_AllEffectsDictionary[allKeys[i]]);
        }
        #endregion

        //Dont stitch up start node if it isnt connected to at least one point
        if (!s_CurrentLE.m_StartNodeData.HasAtLeastOneNextPointNode)
        {
            return;
        }

        //Do the same for start and end nodes
        //if node has a m_NextPointNodeID and that the next node this node is assigned to doesnt have a connection on the outpoint,
        if (!String.IsNullOrEmpty(s_CurrentLE.m_StartNodeData.m_NextPointsIDs[0])
            && !StartNode.m_OutPoint.IsConnected)
        {
            //Else just find the next node from the dictionary of all effects node
            CreateConnection(
                             AllEffectsNodeInEditor[s_CurrentLE.m_StartNodeData.m_NextPointsIDs[0]].m_InPoint,
                             StartNode.m_OutPoint
                            );
        }

    }

    #endregion

}
