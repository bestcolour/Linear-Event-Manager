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
    public static LinearEvent s_EditingLinearEvent = default;

    //For saving 
    List<Node> m_AllNodesInEditor = new List<Node>();

    Dictionary<string, BaseEffectNode> m_AllEffectsNodeInEditor = new Dictionary<string, BaseEffectNode>();

    //RULE: INPOINT'S CONNECTED NODE ID FIRST THEN OUTPOINT CONNECTED NODE ID
    Dictionary<Tuple<string, string>, Connection> m_AllConnectionsDictionary = new Dictionary<Tuple<string, string>, Connection>();

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
    static readonly Color s_SelectionBoxColour = new Color(0.6f, 0.8f, 1f, .2f);
    static readonly Color s_SelectionBoxOutlineColour = new Color(0f, 0.298f, 0.6f, 1f);

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

    ConnectionPoint m_SelectedInPoint = default, m_SelectedOutPoint = default;

    const float k_MinScale = 0.4f, k_MaxScale = 1.0f, k_ScaleChangeRate = 0.2f, k_SlowScaleChangeRate = 0.1f;
    float m_CurrentScaleFactor = 1f;
    float ScaleFactor { get { return m_CurrentScaleFactor; } set { m_CurrentScaleFactor = Mathf.Clamp(value, k_MinScale, k_MaxScale); } }

    #endregion


    //NodeCommandInvoker m_CommandInvoker = default;

    //static readonly LEMStyleLibrary s_SkinsLibrary = new LEMStyleLibrary();
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

        s_EditingLinearEvent = linearEvent;
        instance.LoadFromLinearEvent();
    }

    #region Initialisation

    void OnEnable()
    {
        instance = this;

        LEMStyleLibrary.LoadLibrary();

        InitialiseSkin();

        //Creates instance of invoker
        //if (m_CommandInvoker == null)
        //{
        //    m_CommandInvoker = new NodeCommandInvoker();
        //}

        if (m_AllNodesInEditor == null)
        {
            m_AllNodesInEditor = new List<Node>();
        }

        if (m_AllEffectsNodeInEditor == default)
        {
            m_AllEffectsNodeInEditor = new Dictionary<string, BaseEffectNode>();
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
        if (s_StartNode == null)
            CreateBasicNode(new Vector2(EditorGUIUtility.currentViewWidth * 0.5f, 50f), "StartNode", out s_StartNode);
        if (s_EndNode == null)
            CreateBasicNode(new Vector2(EditorGUIUtility.currentViewWidth * 0.5f, 50f), "EndNode", out s_EndNode);
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
        s_EditingLinearEvent = null;
    }

    //Called when window is closed
    private void OnDestroy()
    {
        s_StartNode = null;
        s_EndNode = null;
        ResetDrawingBezierCurve();
        ResetEventVariables();
        s_CurrentNodeLastRecordedSelectState = null;
        s_EditingLinearEvent = null;
        LEM_InspectorEditor.s_IsLoaded = false;
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
        Tuple<string, string>[] allTupleKeys = m_AllConnectionsDictionary.Keys.ToArray();
        for (int i = 0; i < allTupleKeys.Length; i++)
        {
            m_AllConnectionsDictionary[allTupleKeys[i]].Draw();
        }
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
        {
            SaveToLinearEvent();
        }
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

                //Vector2 zoomSpaceMousePosition = EditorZoomFeature.ConvertScreenSpaceToZoomSpace(ScaleFactor, screenPointToConvert: e.mousePosition, Vector2.zero, m_ZoomCoordinatesOrigin);
                Debug.Log("Mouse Original Position : " + EditorZoomFeature.GetOriginalMousePosition + " Window size is : " + new Vector2(Screen.width, Screen.height) + " Scale Factor : " + ScaleFactor);

                //Set the currenly clicked node
                s_CurrentClickedNode = m_AllNodesInEditor.Find(x => x.m_TotalRect.Contains(currMousePosition));

                //Check if the mouse button down is the right click button
                if (e.button == 1)
                {
                    //and if that the mouse is not clicking on any nodes currently
                    if (s_CurrentClickedNode == null || !s_CurrentClickedNode.IsSelected)
                    {
                        //Open a custom created that allows creation of more nodes
                        ProcessContextMenu(currMousePosition);
                        return;
                    }

                    //Else, open the node's context menu
                    ProcessNodeContextMenu();
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
                            m_InitialClickedPosition = currMousePosition;


                            TrySetConnectionPoint(m_SelectedInPoint);
                            TrySetConnectionPoint(m_SelectedOutPoint);
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

                        TrySetConnectionPoint(m_SelectedInPoint);
                        TrySetConnectionPoint(m_SelectedOutPoint);

                        ResetDrawingBezierCurve();
                        break;


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

                for (int i = m_AllNodesInEditor.Count - 1; i >= 0; i--)
                    if (m_AllNodesInEditor[i].HandleMouseDown(e))
                        GUI.changed = true;
                break;

            case EventType.MouseUp:
                for (int i = m_AllNodesInEditor.Count - 1; i >= 0; i--)
                    if (m_AllNodesInEditor[i].HandleMouseUp(e))
                        GUI.changed = true;
                break;

            case EventType.MouseDrag:
                Vector2 convertedDelta = e.delta / ScaleFactor;

                for (int i = m_AllNodesInEditor.Count - 1; i >= 0; i--)
                    if (m_AllNodesInEditor[i].HandleMouseDrag(e, convertedDelta))
                        GUI.changed = true;
                break;

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
            () => CreateEffectNode(mousePosition, "DestroyGameObjectNode"));
        genericMenu.AddItem(new GUIContent("Add Instantiate GameObject node"), false,
            () => CreateEffectNode(mousePosition, "InstantiateGameObjectNode"));



        //Show the menu
        genericMenu.ShowAsContext();
    }


    //This is used for when you wanna create a new node
    void CreateEffectNode(Vector2 mousePosition, string nameOfNodeType)
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
            AddNodeToSelectedCollection,
            RemoveNodeFromSelectedCollection,
            LEMStyleLibrary.s_NodeColourDictionary[nameOfNodeType]
            );

        newEffectNode.GenerateNodeID();

        //Add the node into collection in editor
        m_AllNodesInEditor.Add(newEffectNode);
        m_AllEffectsNodeInEditor.Add(newEffectNode.NodeID, newEffectNode);
    }

    //This is used for loading from a linear event
    void RecreateEffectNode(Vector2 positionToSet, string nameOfNodeType, string idToSet, out BaseEffectNode effectNode)
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
            AddNodeToSelectedCollection,
            RemoveNodeFromSelectedCollection,
            LEMStyleLibrary.s_NodeColourDictionary[nameOfNodeType]
            );

        newEffectNode.SetNodeID(idToSet);

        //Add the node into collection in editor
        m_AllNodesInEditor.Add(newEffectNode);
        m_AllEffectsNodeInEditor.Add(newEffectNode.NodeID, newEffectNode);
        effectNode = newEffectNode;
    }

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
            AddNodeToSelectedCollection,
            RemoveNodeFromSelectedCollection,
            LEMStyleLibrary.s_NodeColourDictionary[nameOfNodeType]
            );

        basicNode.GenerateNodeID();

        //Add the node into collection in editor
        m_AllNodesInEditor.Add(basicNode);

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
            AddNodeToSelectedCollection,
            RemoveNodeFromSelectedCollection,
            LEMStyleLibrary.s_NodeColourDictionary[nameOfNodeType]
            );

        newNode.SetNodeID(idToSet);

        //Add the node into collection in editor
        m_AllNodesInEditor.Add(newNode);
        newlyCreatedNode = newNode;
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

    void OnClickInPoint(ConnectionPoint connectionPoint)
    {
        //Check if player already has a selected in point and if so set its prev skin to normal
        if (m_SelectedInPoint != null)
        {
            TrySetConnectionPoint(m_SelectedInPoint);
            m_SelectedInPoint = connectionPoint;
            //After dealing with old connectionpt skin,  sets the inpoint to selected skin
            TrySetConnectionPointSkin(m_SelectedInPoint, 1);
            return;
        }

        m_SelectedInPoint = connectionPoint;
        TrySetConnectionPointSkin(m_SelectedInPoint, 1);

        //If current selected outpoint is not null
        if (m_SelectedOutPoint != null)
        {
            //Check if selected in point node is same as selected out point npde
            //Another thing to check is if the selected inpoint's connected node is equal to selected output node. If it is then dont bother connecting
            if (m_SelectedOutPoint.m_ParentNode != m_SelectedInPoint.m_ParentNode &&
                m_SelectedOutPoint.GetConnectedNodeID(0) != m_SelectedInPoint.m_ParentNode.NodeID)
            {
                //Remove the old connection if outpoint has an old connection
                if (m_SelectedOutPoint.IsConnected)
                {
                    OnClickRemoveConnection(m_AllConnectionsDictionary
                          [new Tuple<string, string>(m_SelectedOutPoint.GetConnectedNodeID(0), m_SelectedOutPoint.m_ParentNode.NodeID)]
                          );
                }

                CreateConnection();
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

    }

    void OnClickOutPoint(ConnectionPoint connectionPoint)
    {

        //Check if player already has a selected out point and wishes to choose another one
        if (m_SelectedOutPoint != null)
        {
            TrySetConnectionPoint(m_SelectedOutPoint);
            m_SelectedOutPoint = connectionPoint;
            TrySetConnectionPointSkin(m_SelectedOutPoint, 1);
            return;
        }

        m_SelectedOutPoint = connectionPoint;
        //TrySetConnectionPoint(m_SelectedOutPoint/*, true*/);
        TrySetConnectionPointSkin(m_SelectedOutPoint, 1);

        if (m_SelectedInPoint != null)
        {

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
                       m_AllConnectionsDictionary[new Tuple<string, string>(m_SelectedOutPoint.GetConnectedNodeID(0), m_SelectedOutPoint.m_ParentNode.NodeID)]
                       );
                }

                CreateConnection();
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
    }

    void OnClickRemoveNode(Node nodeToRemove)
    {
        //Check if there is any connections to be removed from this node

        if (m_AllConnectionsDictionary.TryGetValue(
            new Tuple<string, string>(nodeToRemove.m_OutPoint.GetConnectedNodeID(0),
            nodeToRemove.NodeID),
            out Connection connectionToRemove))
        {
            //Remove any connections that is connected to the node's outpoint
            OnClickRemoveConnection(connectionToRemove);
        }

        //Remove any and allconnections connected to the node's inpoint
        string[] allNodesConnectedToInPoint = nodeToRemove.m_InPoint.GetAllConnectedNodeIDs();
        for (int i = 0; i < allNodesConnectedToInPoint.Length; i++)
        {
            OnClickRemoveConnection(m_AllConnectionsDictionary[new Tuple<string, string>(nodeToRemove.NodeID, allNodesConnectedToInPoint[i])]);
        }

        //Remove node from selected collection if it is inside
        RemoveNodeFromSelectedCollection(nodeToRemove);
        //If there isnt any connection collection, that means prolly that there is no connection drawn at all in the very beginning
        m_AllNodesInEditor.Remove(nodeToRemove);

        if (m_AllEffectsNodeInEditor.ContainsKey(nodeToRemove.NodeID))
            m_AllEffectsNodeInEditor.Remove(nodeToRemove.NodeID);
    }

    //Used for clicking on points
    void CreateConnection()
    {
        //Add connection to dual key dictionary
        m_AllConnectionsDictionary.Add(
            new Tuple<string, string>(m_SelectedInPoint.m_ParentNode.NodeID, m_SelectedOutPoint.m_ParentNode.NodeID),
            new Connection(m_SelectedInPoint, m_SelectedOutPoint, OnClickRemoveConnection)
            );

        TrySetConnectionPoint(m_SelectedInPoint);
        TrySetConnectionPoint(m_SelectedOutPoint);
    }

    void RecreateConnection(ConnectionPoint inPoint, ConnectionPoint outPoint)
    {
        //Add connection to dual key dictionary
        m_AllConnectionsDictionary.Add(
            new Tuple<string, string>(inPoint.m_ParentNode.NodeID, outPoint.m_ParentNode.NodeID),
            new Connection(inPoint, outPoint, OnClickRemoveConnection)
            );

        TrySetConnectionPoint(inPoint);
        TrySetConnectionPoint(outPoint);
    }

    void OnClickRemoveConnection(Connection connectionToRemove)
    {
        //Up cast the connection pt so that we can reset the outpt's nextnode
        connectionToRemove.m_InPoint.RemoveConnectedNodeID(connectionToRemove.m_OutPoint.m_ParentNode.NodeID);
        connectionToRemove.m_OutPoint.RemoveConnectedNodeID(null);

        //Reset the connections' in and out points to prevent the points to look unchanged
        TrySetConnectionPoint(connectionToRemove.m_InPoint);
        TrySetConnectionPoint(connectionToRemove.m_OutPoint);

        //Remove from dictionary
        m_AllConnectionsDictionary.Remove(
            new Tuple<string, string>(connectionToRemove.m_InPoint.m_ParentNode.NodeID,
            connectionToRemove.m_OutPoint.m_ParentNode.NodeID)
            );
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


    #region Saving and Loading

    void SaveToLinearEvent()
    {
        m_AllNodesInEditor.Remove(s_StartNode);
        m_AllNodesInEditor.Remove(s_EndNode);

        //Saving starts here
        LEM_BaseEffect[] lemEffects = new LEM_BaseEffect[m_AllNodesInEditor.Count];
        BaseEffectNode[] allEffectNodes = m_AllNodesInEditor.ConvertAll(x => (BaseEffectNode)x).ToArray();

        for (int i = 0; i < m_AllNodesInEditor.Count; i++)
        {
            lemEffects[i] = allEffectNodes[i].CompileToBaseEffect();
        }

        s_EditingLinearEvent.m_AllEffects = lemEffects;

        //Save start and end node data
        s_EditingLinearEvent.m_StartNodeData = s_StartNode.SaveNodeData();
        s_EditingLinearEvent.m_EndNodeData = s_EndNode.SaveNodeData();

        //Saving ends here
        m_AllNodesInEditor.Add(s_StartNode);
        m_AllNodesInEditor.Add(s_EndNode);

    }

    void LoadFromLinearEvent()
    {
        //Load all the nodes into collection first by recreating them
        for (int i = 0; i < s_EditingLinearEvent.m_AllEffects.Length; i++)
        {
            RecreateEffectNode(s_EditingLinearEvent.m_AllEffects[i].m_NodeBaseData.m_Position,
                s_EditingLinearEvent.m_AllEffects[i].m_NodeEffectType,
                s_EditingLinearEvent.m_AllEffects[i].m_NodeBaseData.m_NodeID
                , out BaseEffectNode newEffectNode);

            newEffectNode.LoadFromLinearEvent(s_EditingLinearEvent.m_AllEffects[i]);
        }

        //Do the same for start n end node only if they arent null (they likely wont because onenable runs first)
        //and that there are records of saving them
        if (s_StartNode != null && s_EditingLinearEvent.m_StartNodeData.m_NodeID != string.Empty)
        {
            OnClickRemoveNode(s_StartNode);
            RecreateBasicNode(s_EditingLinearEvent.m_StartNodeData.m_Position, "StartNode", s_EditingLinearEvent.m_StartNodeData.m_NodeID, out s_StartNode);
        }

        if (s_EndNode != null && s_EditingLinearEvent.m_EndNodeData.m_NodeID != string.Empty)
        {
            OnClickRemoveNode(s_EndNode);
            RecreateBasicNode(s_EditingLinearEvent.m_EndNodeData.m_Position, "EndNode", s_EditingLinearEvent.m_EndNodeData.m_NodeID, out s_EndNode);
        }

        //Stitch their connnection back
        for (int i = 0; i < s_EditingLinearEvent.m_AllEffects.Length; i++)
        {
            //if current effect has a m_NextPointNodeID and that the node this effect is assigned to doesnt have a connection on the outpoint,
            if (!String.IsNullOrEmpty(s_EditingLinearEvent.m_AllEffects[i].m_NodeBaseData.m_NextPointNodeID)
                && !m_AllEffectsNodeInEditor[s_EditingLinearEvent.m_AllEffects[i].m_NodeBaseData.m_NodeID].m_OutPoint.IsConnected)
            {
                //If effectnode's out point is connected to the end,
                if (s_EditingLinearEvent.m_AllEffects[i].m_NodeBaseData.m_NextPointNodeID == s_EndNode.NodeID)
                {
                    //Connect with end node
                    RecreateConnection(
                            s_EndNode.m_InPoint,
                             m_AllEffectsNodeInEditor[s_EditingLinearEvent.m_AllEffects[i].m_NodeBaseData.m_NodeID].m_OutPoint
                             );
                    continue;
                }

                RecreateConnection(
                                m_AllEffectsNodeInEditor[s_EditingLinearEvent.m_AllEffects[i].m_NodeBaseData.m_NextPointNodeID].m_InPoint,
                                m_AllEffectsNodeInEditor[s_EditingLinearEvent.m_AllEffects[i].m_NodeBaseData.m_NodeID].m_OutPoint
                                );
            }
        }

        //Do the same for start and end nodes
        //if node has a m_NextPointNodeID and that the next node this node is assigned to doesnt have a connection on the outpoint,
        if (!String.IsNullOrEmpty(s_EditingLinearEvent.m_StartNodeData.m_NextPointNodeID)
            && !s_StartNode.m_OutPoint.IsConnected)
        {

            //If start node's next node is end node,
            if (s_EditingLinearEvent.m_StartNodeData.m_NextPointNodeID == s_EditingLinearEvent.m_EndNodeData.m_NodeID)
            {
                RecreateConnection(s_EndNode.m_InPoint, s_StartNode.m_OutPoint);
                return;
            }

            //Else just find the next node from the dictionary of all effects node
            RecreateConnection(
                             m_AllEffectsNodeInEditor[s_EditingLinearEvent.m_StartNodeData.m_NextPointNodeID].m_InPoint,
                             s_StartNode.m_OutPoint
                            );
        }

    }

    #endregion



}
