using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using LEM_Effects;
using System.Linq;
namespace LEM_Editor
{
    public struct NodeDictionaryStruct
    {
        public BaseEffectNode effectNode;
        public OutConnectionPoint[] outConnectionPoints;

        public NodeDictionaryStruct(BaseEffectNode effectNode, OutConnectionPoint[] outConnectionPoints)
        {
            this.effectNode = effectNode;
            this.outConnectionPoints = outConnectionPoints;
        }

        public bool IsMultiOutPointNode => outConnectionPoints.Length > 1;

    }

    public class NodeLEM_Editor : EditorWindow
    {
        #region Loading States
        struct EDITORSTATE
        {
            //UNLOADED = there is no linear event loaded yet, LOADED = there is linear event loaded but there is also changes made
            //SAVED = linear event was just saved, SAVING = linear event is current in the midsts of saving
            public const int UNLOADED = -1, LOADED = 0, LOADING = 1, SAVED = 2, SAVING = 3;
            public const string SAVED_STRING = "Saved!", LOADED_STRING = "Save"/* Effects \n (Crlt + S)"*/, AUTOSAVE_STRING = "Auto Save On";
        }

        int m_EditorState = EDITORSTATE.UNLOADED;

        public static NodeLEM_Editor instance = default;
        public static LinearEvent s_CurrentLE = default;
        public static NodeLEM_Settings s_Settings = default;

        const string k_EditorPref_LinearEventKey = "linearEventScenePath";
        const string k_EditorPref_SettingsKey = "currentSettings";
        const string k_DefaultSettingsFolderAssetPath = "Assets/Editor/Node_LEM/Settings";

        #endregion

        //For saving 
        List<Node> m_AllNodesInEditor = new List<Node>();
        List<Node> AllNodesInEditor => instance.m_AllNodesInEditor;

        //Dictionary<string, BaseEffectNode> m_AllEffectsNodeInEditor = new Dictionary<string, BaseEffectNode>();
        //Dictionary<string, BaseEffectNode> AllEffectsNodeInEditor => instance.m_AllEffectsNodeInEditor;

        Dictionary<string, NodeDictionaryStruct> m_AllEffectsNodeInEditor = new Dictionary<string, NodeDictionaryStruct>();
        Dictionary<string, NodeDictionaryStruct> AllEffectsNodeInEditor => instance.m_AllEffectsNodeInEditor;

        void EditEffectNodeStruct(NodeDictionaryStruct nodeStruct)
        {
            //Do the service of removing connection when edit occurs
            //Only when new node struct has lesser outpoints than current nodestruct
            int currentNsOutPtLength = AllEffectsNodeInEditor[nodeStruct.effectNode.NodeID].outConnectionPoints.Length;

            if (currentNsOutPtLength > nodeStruct.outConnectionPoints.Length)
            {
                //Remove connection
                TryToRemoveConnection(AllEffectsNodeInEditor[nodeStruct.effectNode.NodeID].outConnectionPoints[currentNsOutPtLength - 1].GetConnectedNodeID(0),
                    nodeStruct.effectNode.NodeID
                    );
            }

            AllEffectsNodeInEditor[nodeStruct.effectNode.NodeID] = nodeStruct;
        }


        //RULE: INPOINT'S CONNECTED NODE ID FIRST THEN OUTPOINT CONNECTED NODE ID
        Dictionary<Tuple<string, string>, Connection> m_AllConnectionsDictionary = new Dictionary<Tuple<string, string>, Connection>();
        Dictionary<Tuple<string, string>, Connection> AllConnectionsDictionary => instance.m_AllConnectionsDictionary;

        Node m_StartNode = default;
        Node StartNode { get { return instance.m_StartNode; } set { instance.m_StartNode = value; } }

        Action d_OnGUI = null;

        #region Process Event Variables

        List<Node> m_AllSelectedNodes = new List<Node>();
        List<Node> AllSelectedNodes => instance.m_AllSelectedNodes;

        Node s_CurrentClickedNode = null;
        public static Node CurrentClickedNode => instance.s_CurrentClickedNode;
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

        const float k_MinScale = 0.15f, k_MaxScale = 1.0f, k_ScaleChangeRate = 0.2f, k_SlowScaleChangeRate = 0.05f;
        static float s_CurrentScaleFactor = 1f;
        float ScaleFactor { get { return s_CurrentScaleFactor; } set { s_CurrentScaleFactor = Mathf.Clamp(value, k_MinScale, k_MaxScale); } }

        public static float InverseScaleFactor => instance.m_InverseScaleFactorOnEveryGUIFrame;
        float m_InverseScaleFactorOnEveryGUIFrame = default;
        #endregion

        #region SearchBox
        //Search box variables
        bool m_IsSearchBoxActive = false;

        LEM_SearchBox m_SearchBox = default;

        void OnInputChange(string result)
        {
            m_SearchBox.ClearResults();

            string currentNodeType;

            //If searchbox is drawn for the first time 
            if (String.IsNullOrEmpty(result))
            {
                for (int i = 0; i < LEMDictionary.s_NodeTypeKeys.Length; i++)
                {
                    currentNodeType = LEMDictionary.s_NodeTypeKeys[i];
                    m_SearchBox.AddResult(currentNodeType);
                }
                return;
            }

            //Else if result isnt empty or null when search box is right clicked to be drawn
            for (int i = 0; i < LEMDictionary.s_NodeTypeKeys.Length; i++)
            {
                currentNodeType = LEMDictionary.s_NodeTypeKeys[i];

                if (currentNodeType.CaseInsensitiveContains(result))
                    m_SearchBox.AddResult(currentNodeType);
            }
        }

        void OnConfirm(string result, Vector2 mousePos)
        {
            mousePos *= 1 / ScaleFactor;
            CommandInvoker.InvokeCommand(new CreateNodeCommand(mousePos, result));
            instance.m_IsSearchBoxActive = false;
        }

        GenericMenu m_NodeContextMenu = default;
        #endregion

        #endregion

        #region NodeInvoker

        NodeCommandInvoker m_CommandInvoker = default;
        static NodeCommandInvoker CommandInvoker => instance.m_CommandInvoker;
        bool m_IsDragging = default;
        public static int s_MaxActions = 100;

        #endregion

        Texture2D m_EditorBackGroundTexture = default;
        Texture2D EditorBackGroundTexture => instance.m_EditorBackGroundTexture;


        #region Initialisation

        //Form of intialisation from pressing Load Linear Event
        public static void InitialiseWindow()
        {
            //Get window and this will trigger OnEnable
            NodeLEM_Editor editorWindow = GetWindow<NodeLEM_Editor>();

            //Set the title of gui for the window to be TEM Node Editor
            editorWindow.titleContent = new GUIContent("TEM Node Editor");

            editorWindow.d_OnGUI = editorWindow.Initialise;

        }

        //Form of intialisation from pressing Open Window
        [MenuItem("Window/Lem Node Editor")]
        public static void OpenWindow()
        {
            //Get window and this will trigger OnEnable
            NodeLEM_Editor editorWindow = GetWindow<NodeLEM_Editor>();

            //Set the title of gui for the window to be TEM Node Editor
            editorWindow.titleContent = new GUIContent("TEM Node Editor");

        }


        void OnEnable()
        {
            //If this window is opened on project launch or smth (basically u dint press "LoadNodes" button to enter window
            if (s_CurrentLE == null)
            {
                m_EditorState = EDITORSTATE.UNLOADED;
                d_OnGUI = EmptyEditorUpdate;
            }

            LoadSettings();

            //Well regardless of it being empty or not, ensure that node editor saves before reloading assembly
            //AssemblyReloadEvents.beforeAssemblyReload += SaveToLinearEvent;
            AssemblyReloadEvents.beforeAssemblyReload += SaveToLinearEvent;

            //Due to beforeAssemblyReload being called when player enters play mode but doesnt save values, this needs to be added
            EditorApplication.playModeStateChanged += SaveBeforeEnterPlayMode;
            EditorApplication.playModeStateChanged += LoadAfterExitingPlayMode;
            EditorApplication.quitting += TryToSaveLinearEvent;
            //EditorApplication.quitting += SaveToLinearEvent;

        }

        public static void LoadSettings()
        {
            string pathToSettings = EditorPrefs.GetString(k_EditorPref_SettingsKey);
            NodeLEM_Settings settings;

            //If there is a path recorded,
            if (!string.IsNullOrEmpty(pathToSettings))
            {
                //Check if path is of object still valid
                settings = AssetDatabase.LoadAssetAtPath(pathToSettings + ".asset", typeof(NodeLEM_Settings)) as NodeLEM_Settings;

                //If there is a setting found, set s_Settings to that n return
                if (settings != null)
                {
                    s_Settings = settings;
                    return;
                }
            }

            //else if settings is null, we need to search inside the default settings folder
            string[] guidOfAssets = AssetDatabase.FindAssets("t:NodeLEM_Settings", new string[1] { k_DefaultSettingsFolderAssetPath });

            //Attempt to get the first default settings you can find if there is more than 0 searches found
            if (guidOfAssets.Length > 0)
            {
                for (int i = 0; i < guidOfAssets.Length; i++)
                {
                    pathToSettings = AssetDatabase.GUIDToAssetPath(guidOfAssets[0]) + ".asset";
                    settings = AssetDatabase.LoadAssetAtPath(pathToSettings, typeof(NodeLEM_Settings)) as NodeLEM_Settings;

                    //If there is a setting found, set s_Settings to that n return
                    if (settings != null)
                    {
                        s_Settings = settings;
                        //Set this setting as the next loaded settings
                        EditorPrefs.SetString(k_EditorPref_SettingsKey, pathToSettings);
                        return;
                    }
                }
            }

            //If there isnt any settings found or if for some god forsaken reason, you got the guid but the assetdatabase cant load the asset
            //create a new settings
            settings = ScriptableObject.CreateInstance<NodeLEM_Settings>();
            pathToSettings = k_DefaultSettingsFolderAssetPath + "/" + NodeLEM_Settings.k_DefaultFileName + ".asset";
            AssetDatabase.CreateAsset(settings, pathToSettings);
            AssetDatabase.SaveAssets();
            EditorPrefs.SetString(k_EditorPref_SettingsKey, pathToSettings);
            s_Settings = settings;

        }

        public static void LoadNodeEditor(LinearEvent linearEvent)
        {
            //This will be a key identifier for whether LoadNodeEditor button was pressed
            LinearEvent prevLE = s_CurrentLE;
            s_CurrentLE = linearEvent;

            //If this is the first time you are opening the window, or if u have previously closed the window and you wish to reopen it
            if (instance == null)
                InitialiseWindow();
            else
            {
                //Save the prev lienarevent
                s_CurrentLE = prevLE;
                instance.TryToSaveLinearEvent();

                //Load the new one
                s_CurrentLE = linearEvent;
                instance.ResetandLoadEditor();
            }

        }


        //To be called on the very first time of pressing "LoadNodeEditor"? 
        //This is also called when you hv the window docked in ur panels but u dont give focus on it and u just upen ur project
        void Initialise()
        {
            //Call these only once in the flow of usage until the window is closed
            instance = this;

            LEMStyleLibrary.LoadLibrary();
            LEMDictionary.LoadDictionary();

            instance.m_EditorBackGroundTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            instance.m_EditorBackGroundTexture.SetPixel(0, 0, new Color(0.227f, 0.216f, 0.212f));
            instance.m_EditorBackGroundTexture.Apply();

            if (instance.m_CommandInvoker == null)
            {
                instance.m_CommandInvoker = new NodeCommandInvoker(s_MaxActions, CreateEffectNode, RecreateEffectNode, TryToRestichConnections, DeleteNodes, CompileNodeToEffect, MoveNodes, CreateConnection, TryToRemoveConnection, DeselectAllNodes, () => m_EditorState = EDITORSTATE.LOADED);
            }

            if (instance.m_AllNodesInEditor == null)
            {
                instance.m_AllNodesInEditor = new List<Node>();
            }

            if (instance.m_AllEffectsNodeInEditor == default)
            {
                instance.m_AllEffectsNodeInEditor = new Dictionary<string, NodeDictionaryStruct>();
                //instance.m_AllEffectsNodeInEditor = new Dictionary<string, BaseEffectNode>();
            }

            if (instance.m_AllConnectionsDictionary == null)
            {
                instance.m_AllConnectionsDictionary = new Dictionary<Tuple<string, string>, Connection>();
            }

            if (instance.m_AllSelectedNodes == null)
            {
                instance.m_AllSelectedNodes = new List<Node>();
            }

            if (instance.m_SearchBox == null)
            {
                instance.m_SearchBox = new LEM_SearchBox(instance.OnInputChange, instance.OnConfirm, 250, 325);
            }

            if (instance.m_NodeContextMenu == null)
            {
                SetupProcessNodeContextMenu();
            }

            //Regardless, just initialise strt end nodes
            instance.InitialiseStartEndNodes();
            instance.LoadFromLinearEvent();

            //After finishing all the intialisation and loading of linearevent,
            d_OnGUI = UpdateGUI;

            instance.m_EditorState = EDITORSTATE.SAVED;
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
            instance.m_InitialClickedPosition = null;
            instance.m_IsDragging = false;
            s_SelectionBox = default;
            s_CurrentClickedNode = null;
            GUI.changed = true;
        }

        void ResetDrawingBezierCurve()
        {
            //Reset bezierline drawing
            instance.m_SelectedOutPoint = null;
            instance.m_SelectedInPoint = null;
        }

        //Time taken: Instant
        void ResetandLoadEditor()
        {
            StartNode = null;
            ResetDrawingBezierCurve();
            ResetEventVariables();
            CurrentNodeLastRecordedSelectState = null;
            m_IsSearchBoxActive = false;


            m_AllNodesInEditor = new List<Node>();
            m_AllEffectsNodeInEditor = new Dictionary<string, NodeDictionaryStruct>();
            m_AllConnectionsDictionary = new Dictionary<Tuple<string, string>, Connection>();
            m_CommandInvoker.ResetHistory();


            //Regardless, just initialise strt end nodes
            instance.InitialiseStartEndNodes();
            instance.LoadFromLinearEvent();

            //After finishing all the intialisation and loading of linearevent,
            d_OnGUI = UpdateGUI;

            m_EditorState = EDITORSTATE.SAVED;
        }

        //Called when window is closed
        void OnDestroy()
        {
            //Before closing the window, save the le if it wasnt saved
            //if (instance != null && m_EditorState == EDITORSTATE.LOADED /*&& m_SaveWindow != null*/)
            //{
            //    //Really shitty way to close the popup window cause onlost focus is called b4 ondestroy and there is no way to differentiate between them
            //    SaveToLinearEvent();
            //}

            TryToSaveLinearEvent();

            //Unsubscribe b4 closing window
            AssemblyReloadEvents.beforeAssemblyReload -= SaveToLinearEvent;
            EditorApplication.playModeStateChanged -= SaveBeforeEnterPlayMode;
            EditorApplication.playModeStateChanged -= LoadAfterExitingPlayMode;
            EditorApplication.quitting -= TryToSaveLinearEvent;

            EditorPrefs.SetString(k_EditorPref_LinearEventKey, "");

            if (s_Settings != null)
                EditorPrefs.SetString(k_EditorPref_SettingsKey, k_DefaultSettingsFolderAssetPath + "/" + s_Settings.name);

            s_CurrentLE = null;
        }

        #endregion

        #region Updates and Events


        void EmptyEditorUpdate()
        {
            Rect propertyRect = new Rect(10f, 10f, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            GUI.Label(propertyRect, "There is no Linear Event Loaded, please choose one");

            propertyRect.y += EditorGUIUtility.singleLineHeight * 2f;
            propertyRect.height *= 1.75f;
            propertyRect.width *= 0.9f;

            s_CurrentLE = (LinearEvent)EditorGUI.ObjectField(propertyRect, s_CurrentLE, typeof(LinearEvent), true);

            propertyRect.y += EditorGUIUtility.singleLineHeight * 2f;
            GUI.Label(propertyRect, "Settings");
            propertyRect.y += EditorGUIUtility.singleLineHeight * 2f;
            s_Settings = (NodeLEM_Settings)EditorGUI.ObjectField(propertyRect, s_Settings, typeof(NodeLEM_Settings), false);

            //Then once Current Linear Event is selected,
            if (s_CurrentLE != null && s_Settings != null)
            {
                EditorPrefs.SetString(k_EditorPref_SettingsKey, k_DefaultSettingsFolderAssetPath + "/" + s_Settings.name);
                LoadNodeEditor(s_CurrentLE);
            }
        }

        void UpdateGUI()
        {
            m_InverseScaleFactorOnEveryGUIFrame = 1 / ScaleFactor;
            Rect dummyRect = Rect.zero;
            Event currentEvent = Event.current;

            //Draw background of for the window
            dummyRect.width = maxSize.x;
            dummyRect.height = maxSize.y;
            GUI.DrawTexture(dummyRect, EditorBackGroundTexture, ScaleMode.StretchToFill);


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
            bool isMouseInSearchBox = HandleSearchBox(currentEvent);


            if (s_Settings.m_ShowToolBar)
            {
                DrawToolButtons(dummyRect);
            }
            HandleCurrentLinearEventLabel(dummyRect, currentEvent);

            //DrawDebugLists();

            //Then process the events that occured from unity's events (events are like clicks,drag etc)
            ProcessEvents(currentEvent, currMousePos, isMouseInSearchBox);
            ProcessNodeEvents(currentEvent);

            //If there is any value change in the gui,repaint it
            if (GUI.changed)
            {
                Repaint();
            }

        }



        void OnGUI()
        {
            d_OnGUI?.Invoke();
        }

        private void OnLostFocus()
        {
            //if (m_EditorState == EDITORSTATE.LOADED)
            //{
            //    SaveToLinearEvent();
            //}

            TryToSaveLinearEvent();
        }

        #endregion

        #region Draw Functions

        void DrawNodes()
        {
            //If nodes collection is not null
            if (AllNodesInEditor != null)
                for (int i = 0; i < AllNodesInEditor.Count; i++)
                    if (AllNodesInEditor[i].IsWithinWindowScreen)
                        AllNodesInEditor[i].Draw();

        }

        void DrawConnections()
        {
            Tuple<string, string>[] allTupleKeys = AllConnectionsDictionary.Keys.ToArray();
            for (int i = 0; i < allTupleKeys.Length; i++)
                if (AllConnectionsDictionary[allTupleKeys[i]].IsWithinWindowScreen)
                    AllConnectionsDictionary[allTupleKeys[i]].Draw();
              

            //foreach (Connection connection in AllConnectionsDictionary.Values)
            //{
            //    connection.Draw();
            //}
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

        void DrawToolButtons(Rect propertyRect)
        {
            float buttonWidth = s_Settings.m_ButtonSize;
            //Align button to the right of the screen
            propertyRect.x = position.width - buttonWidth;
            propertyRect.y = 0f;
            propertyRect.width = buttonWidth;
            propertyRect.height = 50f;

            #region Drawing Save Button

            if (!s_Settings.m_AutoSave)
            {
                if (GUI.Button(propertyRect, EDITORSTATE.LOADED_STRING))
                {
                    //m_EditorState = EDITORSTATE.SAVING;
                    SaveToLinearEvent();
                    //m_EditorState = EDITORSTATE.SAVED;
                }
                ////Prevents double clicking on saving
                //if (m_EditorState == EDITORSTATE.LOADED)
                //{

                //}
                ////else if m_EditorState == EditorState.Saved cause for Unloaded to occur u have no linear event 
                ////but that means save button wont even be drawn due to it not belonging in the same delegate
                //else
                //{
                //    bool wasEnabled = GUI.enabled;
                //    GUI.enabled = false;
                //    GUI.Button(propertyRect, EDITORSTATE.SAVED_STRING);
                //    GUI.enabled = wasEnabled;
                //}
            }
            else
            {
                bool wasEnabled = GUI.enabled;
                GUI.enabled = false;
                GUI.Button(propertyRect, EDITORSTATE.AUTOSAVE_STRING);
                GUI.enabled = wasEnabled;
            }


            #endregion

            Event e = Event.current;

            propertyRect.x -= buttonWidth;

            if (GUI.Button(propertyRect, "Paste"))
            {
                //Else if there is stuff copied on the clipboard of the nodeinvoker then you can paste 
                if (NodeCommandInvoker.s_ClipBoard.Count > 0)
                {
                    //If player had cut 
                    if (CommandInvoker.m_HasCutButNotCutPaste)
                    {
                        CommandInvoker.InvokeCommand(new CutPasteCommand());
                        GUI.changed = true;
                        return;
                    }

                    CommandInvoker.InvokeCommand(new PasteCommand());
                    GUI.changed = true;
                }

            }

            propertyRect.x -= buttonWidth;

            if (GUI.Button(propertyRect, "Copy"))
            {
                //Remove start and end node 
                if (m_AllSelectedNodes.Contains(StartNode))
                {
                    StartNode.DeselectNode();
                }

                CommandInvoker.CopyToClipBoard(Array.ConvertAll(m_AllSelectedNodes.ToArray(), x => (BaseEffectNode)x));
                e.Use();
            }

            propertyRect.x -= buttonWidth;

            if (GUI.Button(propertyRect, "Cut"))
            {
                //Remove start and end node 
                if (m_AllSelectedNodes.Contains(StartNode))
                {
                    StartNode.DeselectNode();
                }

                //CommandInvoker.InvokeCommand(new CutCommand(Array.ConvertAll(m_AllSelectedNodes.ToArray(), x => (BaseEffectNode)x)));
                CommandInvoker.InvokeCommand(new CutCommand(m_AllSelectedNodes.Select(x => x.NodeID).ToArray()));
                //e.Use();
                GUI.changed = true;
            }

            propertyRect.x -= buttonWidth;

            if (GUI.Button(propertyRect, "Delete"))
            {
                GUI.FocusControl(null);

                //Remove start and end node 
                if (m_AllSelectedNodes.Contains(StartNode))
                {
                    StartNode.DeselectNode();
                }

                CommandInvoker.InvokeCommand(new DeleteNodeCommand(m_AllSelectedNodes.Select(x => x.NodeID).ToArray()));
                //Skip everything else and repaint
                e.Use();

            }

            propertyRect.x -= buttonWidth;

            if (GUI.Button(propertyRect, "Redo"))
            {
                CommandInvoker.RedoCommand();
                GUI.changed = true;
            }

            propertyRect.x -= buttonWidth;

            if (GUI.Button(propertyRect, "Undo"))
            {
                CommandInvoker.UndoCommand();
                GUI.changed = true;
            }

        }

        void HandleCurrentLinearEventLabel(Rect propertyRect, Event currentEvent)
        {
            string label = "Linear Event : " + s_CurrentLE.name;
            propertyRect.size = GUI.skin.label.CalcSize(new GUIContent(label, "The Linear Event you are currently editting"));
            propertyRect.x = 0;
            propertyRect.y = 0;

            LEMStyleLibrary.s_GUIPreviousColour = GUI.skin.label.normal.textColor;
            GUI.skin.label.normal.textColor = Color.yellow;
            GUI.Label(propertyRect, label);
            GUI.skin.label.normal.textColor = LEMStyleLibrary.s_GUIPreviousColour;
            //If current event type is a mouse click and mouse click is within the propertyrect,
            if (currentEvent.type == EventType.MouseDown && propertyRect.Contains(currentEvent.mousePosition))
            {
                EditorGUIUtility.PingObject(s_CurrentLE);
            }
        }

        private void DrawDebugLists()
        {
            //Copy previous colour and set the color red
            LEMStyleLibrary.s_GUIPreviousColour = GUI.skin.label.normal.textColor;
            GUI.skin.label.normal.textColor = Color.red;

            Rect propertyRect = Rect.zero;
            propertyRect.y += EditorGUIUtility.singleLineHeight;
            propertyRect.height += EditorGUIUtility.singleLineHeight;
            propertyRect.width += EditorGUIUtility.currentViewWidth;

            #region All Nodes In Editor Debug

            GUI.Label(propertyRect, "All Nodes in Editor");
            propertyRect.y += EditorGUIUtility.singleLineHeight;

            for (int i = 0; i < AllNodesInEditor.Count; i++)
            {
                GUI.Label(propertyRect, i + ") " + AllNodesInEditor[i].NodeID);
                propertyRect.y += EditorGUIUtility.singleLineHeight;
            }

            #endregion

            propertyRect.y += EditorGUIUtility.singleLineHeight;

            GUI.skin.label.normal.textColor = Color.cyan;
            Tuple<string, string>[] inPointID_outPointID = AllConnectionsDictionary.Keys.ToArray();

            GUI.Label(propertyRect, "All Connections");
            propertyRect.y += EditorGUIUtility.singleLineHeight;

            for (int i = 0; i < inPointID_outPointID.Length; i++)
            {
                GUI.Label(propertyRect, i + ") " + inPointID_outPointID[i]);
                propertyRect.y += EditorGUIUtility.singleLineHeight;
            }


            GUI.skin.label.normal.textColor = LEMStyleLibrary.s_GUIPreviousColour;


        }

        bool HandleSearchBox(Event e)
        {
            if (m_IsSearchBoxActive)
                return m_SearchBox.HandleSearchBox(e);

            return false;
        }

        #endregion

        //Checks what the current event is right now, and then execute code accordingly
        void ProcessEvents(Event e, Vector2 currMousePosition, bool isMouseInSearchBox)
        {
            m_AmountOfMouseDragThisUpdate = Vector2.zero;
            switch (e.type)
            {
                case EventType.ScrollWheel:

                    if (!isMouseInSearchBox)
                    {
                        int signOfChange = 0;
                        float changeRate = 0f;

                        signOfChange = e.delta.y > 0 ? -1 : 1;
                        //If alt key is pressed,
                        changeRate = e.alt ? k_SlowScaleChangeRate : k_ScaleChangeRate;

                        ScaleFactor += signOfChange * changeRate;

                        e.Use();
                    }

                    break;

                case EventType.MouseDown:

                    //Set the currenly clicked node
                    //s_CurrentClickedNode = AllNodesInEditor.Find(x => x.m_TotalRect.Contains(currMousePosition));
                    s_CurrentClickedNode = AllNodesInEditor.FindFromLastIndex(x => x.m_TotalRect.Contains(currMousePosition));

                    //Check if the mouse button down is the right click button
                    if (e.button == 1)
                    {

                        //and if that the mouse is not clicking on any nodes currently
                        if (s_CurrentClickedNode == null || !s_CurrentClickedNode.IsSelected)
                        {
                            //Open a custom created that allows creation of more nodes
                            m_SearchBox.Position = currMousePosition * ScaleFactor;
                            m_IsSearchBoxActive = true;
                            m_SearchBox.TriggerOnInputOnStart();

                            e.Use();
                            return;
                        }


                        //Else, open the node's context menu
                        //ProcessNodeContextMenu();
                        m_NodeContextMenu.ShowAsContext();
                    }

                    else if (e.button == 0)
                    {
                        //If mouse indeed doesnt clicks on a node,
                        if (s_CurrentClickedNode == null)
                        {
                            //Set initial position for drawing selection box if alt is not pressed
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

                        m_IsSearchBoxActive = false;

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
                            //GUI.changed = true;
                            e.Use();
                        }
                        //If user is currently planning to drag a node and wasnt draggin the previous paint,
                        else if (s_CurrentClickedNode != null && !m_IsDragging)
                        {
                            m_IsDragging = true;
                            CommandInvoker.InvokeCommand(new MoveNodeCommand(m_AllSelectedNodes.ToArray()));
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

                        CommandInvoker.InvokeCommand(new DeleteNodeCommand(m_AllSelectedNodes.Select(x => x.NodeID).ToArray()));
                        //Skip everything else and repaint
                        e.Use();
                    }
                    else if (e.keyCode == KeyCode.Escape)
                    {

                        TrySetConnectionPoint(m_SelectedInPoint);
                        TrySetConnectionPoint(m_SelectedOutPoint);
                        m_IsSearchBoxActive = false;
                        ResetDrawingBezierCurve();
                    }
                    //Else if control is held down,
                    else if (e.control)
                    {
                        //Undo
                        if (e.keyCode == KeyCode.Q)
                        {
                            CommandInvoker.UndoCommand();
                            GUI.changed = true;
                        }
                        //Redo
                        else if (e.keyCode == KeyCode.W)
                        {
                            CommandInvoker.RedoCommand();
                            GUI.changed = true;
                        }
                        //Copy
                        else if (e.keyCode == KeyCode.C)
                        {
                            //Remove start and end node 
                            if (m_AllSelectedNodes.Contains(StartNode))
                            {
                                StartNode.DeselectNode();
                            }

                            CommandInvoker.CopyToClipBoard(Array.ConvertAll(m_AllSelectedNodes.ToArray(), x => (BaseEffectNode)x));
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

                            //CommandInvoker.InvokeCommand(new CutCommand(Array.ConvertAll(m_AllSelectedNodes.ToArray(), x => (BaseEffectNode)x)));
                            CommandInvoker.InvokeCommand(new CutCommand(m_AllSelectedNodes.Select(x => x.NodeID).ToArray()));
                            //e.Use();
                            GUI.changed = true;
                        }
                        //Paste only when there is no foccus on anyother keyboard demanding control,
                        else if (e.keyCode == KeyCode.V && GUIUtility.keyboardControl == 0)
                        {

                            //Else if there is stuff copied on the clipboard of the nodeinvoker then you can paste 
                            if (NodeCommandInvoker.s_ClipBoard.Count > 0)
                            {
                                //If player had cut 
                                if (CommandInvoker.m_HasCutButNotCutPaste)
                                {
                                    CommandInvoker.InvokeCommand(new CutPasteCommand());
                                    GUI.changed = true;
                                    return;
                                }

                                CommandInvoker.InvokeCommand(new PasteCommand());
                                GUI.changed = true;
                            }


                        }
                        //Select all
                        else if (e.keyCode == KeyCode.A)
                        {
                            m_AllSelectedNodes.Clear();

                            for (int i = 0; i < m_AllNodesInEditor.Count; i++)
                            {
                                m_AllNodesInEditor[i].SelectNode();
                            }

                            GUI.changed = true;

                        }
                        //Save
                        else if (!s_Settings.m_AutoSave && e.keyCode == KeyCode.S)
                        {
                            SaveToLinearEvent();
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
                        if (AllNodesInEditor[i].HandleMouseUp())
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
            NodeDictionaryStruct nodeStruct = new NodeDictionaryStruct();
            BaseEffectNode newEffectNode = LEMDictionary.GetNodeObject(nameOfNodeType) as BaseEffectNode;

            //Get the respective skin from the collection of nodeskin
            NodeSkinCollection nodeSkin = LEMStyleLibrary.s_WhiteBackGroundSkin;

            newEffectNode.GenerateNodeID();

            //Initialise the new node 
            newEffectNode.Initialise
                (mousePosition,
                nodeSkin,
                LEMStyleLibrary.s_ConnectionPointStyleNormal,
                OnClickInPoint,
                OnClickOutPoint,
                TryToAddNodeToSelectedCollection,
                TryToRemoveNodeFromSelectedCollection,
                EditEffectNodeStruct,
                LEMDictionary.GetNodeColour(nameOfNodeType)
                );

            nodeStruct.effectNode = newEffectNode;
            nodeStruct.outConnectionPoints = newEffectNode.GetOutConnectionPoints;

            //Add the node into collection in editor
            AllNodesInEditor.Add(newEffectNode);
            AllEffectsNodeInEditor.Add(newEffectNode.NodeID, nodeStruct);
            return newEffectNode;
        }

        //This is used for loading and probably undoing/redoing from a linear event
        BaseEffectNode RecreateEffectNode(Vector2 positionToSet, string nameOfNodeType, string idToSet)
        {
            NodeDictionaryStruct nodeStruct = new NodeDictionaryStruct();
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
                EditEffectNodeStruct,
                LEMDictionary.GetNodeColour(nameOfNodeType)
                );

            newEffectNode.SetNodeID(idToSet);
            nodeStruct.effectNode = newEffectNode;
            nodeStruct.outConnectionPoints = newEffectNode.GetOutConnectionPoints;

            //Add the node into collection in editor
            AllNodesInEditor.Add(newEffectNode);
            AllEffectsNodeInEditor.Add(newEffectNode.NodeID, nodeStruct);
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
                LEMDictionary.GetNodeColour(nameOfNodeType)
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
                LEMDictionary.GetNodeColour(nameOfNodeType)
                );

            newNode.SetNodeID(idToSet);

            //Add the node into collection in editor
            AllNodesInEditor.Add(newNode);
            newlyCreatedNode = newNode;
        }

        void DeleteNodes(NodeBaseData[] nodesToBeDeleted)
        {
            for (int i = 0; i < nodesToBeDeleted.Length; i++)
                OnClickRemoveNode(nodesToBeDeleted[i]);
        }


        #endregion

        void SetupProcessNodeContextMenu()
        {
            //and then add an button option with the name "Remove node"
            GenericMenu genericMenu = new GenericMenu();

            //Add remove node function to the context menu option
            //Remove all the selected nodes 
            //Remove all the nodes that are selected until there are none left
            genericMenu.AddItem(new GUIContent("Undo   (Crlt + Q)"), false, delegate { CommandInvoker.UndoCommand(); Repaint(); });
            genericMenu.AddItem(new GUIContent("Redo   (Crlt + W)"), false, delegate { CommandInvoker.RedoCommand(); Repaint(); });
            genericMenu.AddItem(new GUIContent("Copy   (Crlt + C)"), false, delegate
            {
                if (AllSelectedNodes.Contains(StartNode)) { StartNode.DeselectNode(); }
                CommandInvoker.CopyToClipBoard(Array.ConvertAll(AllSelectedNodes.ToArray(), x => (BaseEffectNode)x)); Repaint();
            });

            genericMenu.AddItem(new GUIContent("Cut   (Crlt + X)"), false, delegate
            {
                //Remove start and end node 
                if (AllSelectedNodes.Contains(StartNode)) { StartNode.DeselectNode(); }
                CommandInvoker.InvokeCommand(new CutCommand(m_AllSelectedNodes.Select(x => x.NodeID).ToArray()));
                Repaint();
            });
            genericMenu.AddItem(new GUIContent("Paste   (Crlt + V)"), false, delegate { CommandInvoker.InvokeCommand(new PasteCommand()); Repaint(); });
            genericMenu.AddItem(new GUIContent("Select All   (Crlt + A)"), false, delegate
            {
                AllSelectedNodes.Clear();
                for (int i = 0; i < AllNodesInEditor.Count; i++)
                    AllNodesInEditor[i].SelectNode();
                Repaint();
            });

            genericMenu.AddItem(new GUIContent("Delete   (Del)"), false, delegate
            {
                GUI.FocusControl(null);

                //Remove start and end node 
                if (m_AllSelectedNodes.Contains(StartNode))
                {
                    StartNode.DeselectNode();
                }

                CommandInvoker.InvokeCommand(new DeleteNodeCommand(m_AllSelectedNodes.Select(x => x.NodeID).ToArray()));
            });

            //Display the editted made menu
            instance.m_NodeContextMenu = genericMenu;
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
            for (int i = 0; i < AllNodesInEditor.Count; i++)
            {
                AllNodesInEditor[i].Drag(delta);
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

                currentNodePosition = AllEffectsNodeInEditor[nodeIDsMoved[i]].effectNode.m_TopRect.position;
                AllEffectsNodeInEditor[nodeIDsMoved[i]].effectNode.m_TopRect.position = previousTopRectPositions[i];
                previousTopRectPositions[i] = currentNodePosition;

                currentNodePosition = AllEffectsNodeInEditor[nodeIDsMoved[i]].effectNode.m_MidRect.position;
                AllEffectsNodeInEditor[nodeIDsMoved[i]].effectNode.m_MidRect.position = previousMidRectPositions[i];
                previousMidRectPositions[i] = currentNodePosition;

                currentNodePosition = AllEffectsNodeInEditor[nodeIDsMoved[i]].effectNode.m_TotalRect.position;
                AllEffectsNodeInEditor[nodeIDsMoved[i]].effectNode.m_TotalRect.position = previousTotalRectPositions[i];
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
                    !AllEffectsNodeInEditor[currentEffect.m_NodeBaseData.m_NodeID].outConnectionPoints[0].IsConnected)
                {
                    CreateConnection(
                              AllEffectsNodeInEditor[currentEffect.m_NodeBaseData.m_NextPointsIDs[0]].effectNode.m_InPoint,
                              AllEffectsNodeInEditor[currentEffect.m_NodeBaseData.m_NodeID].effectNode.m_OutPoint
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
                        !AllEffectsNodeInEditor[currentEffect.m_NodeBaseData.m_NodeID].outConnectionPoints[n].IsConnected)
                    {
                        CreateConnection(
                        AllEffectsNodeInEditor[currentEffect.m_NodeBaseData.m_NextPointsIDs[n]].effectNode.m_InPoint,
                        AllEffectsNodeInEditor[currentEffect.m_NodeBaseData.m_NodeID].outConnectionPoints[n]
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
                //else
                //if (m_SelectedInPoint.IsConnected)
                //{
                //    OnClickRemoveConnection(
                //      AllConnectionsDictionary[new Tuple<string, string>(m_SelectedInPoint.m_ParentNode.NodeID, m_SelectedOutPoint.m_ParentNode.NodeID)]
                //      );
                //}

                //Check if inpoint is connected. If not, then heck anyone can go for it
                if (!m_SelectedInPoint.IsConnected)
                {
                    CommandInvoker.InvokeCommand(new CreateConnectionCommand(m_SelectedInPoint.m_ParentNode.NodeID, m_SelectedOutPoint.m_ParentNode.NodeID, m_SelectedOutPoint.Index));
                }
                else
                {
                    //Else, check if outpoint is a multiOutpoint node. If it isnt, then go right ahead
                    if (!AllEffectsNodeInEditor[m_SelectedOutPoint.m_ParentNode.NodeID].IsMultiOutPointNode)
                        CommandInvoker.InvokeCommand(new CreateConnectionCommand(m_SelectedInPoint.m_ParentNode.NodeID, m_SelectedOutPoint.m_ParentNode.NodeID, m_SelectedOutPoint.Index));
                    else
                    {
                        //else just warn the user not to do that n reset the outpoint skin
                        Debug.LogWarning("Cant have a node with multiple outpoints to have shared inpoint!");
                        TrySetConnectionPointSkin(m_SelectedOutPoint, 0);
                    }

                }

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
                //else
                //if (m_SelectedInPoint.IsConnected)
                //{
                //    OnClickRemoveConnection(
                //      AllConnectionsDictionary[new Tuple<string, string>(m_SelectedInPoint.m_ParentNode.NodeID, m_SelectedOutPoint.m_ParentNode.NodeID)]
                //      );
                //}

                //Check if inpoint is connected. If not, then heck anyone can go for it
                if (!m_SelectedInPoint.IsConnected)
                {
                    CommandInvoker.InvokeCommand(new CreateConnectionCommand(m_SelectedInPoint.m_ParentNode.NodeID, m_SelectedOutPoint.m_ParentNode.NodeID, m_SelectedOutPoint.Index));
                }
                else
                {
                    //Else, check if outpoint is a multiOutpoint node. If it isnt, then go right ahead
                    if (!AllEffectsNodeInEditor[m_SelectedOutPoint.m_ParentNode.NodeID].IsMultiOutPointNode)
                        CommandInvoker.InvokeCommand(new CreateConnectionCommand(m_SelectedInPoint.m_ParentNode.NodeID, m_SelectedOutPoint.m_ParentNode.NodeID, m_SelectedOutPoint.Index));
                    else
                    {
                        //else just warn the user not to do that n reset the outpoint skin
                        Debug.LogWarning("Cant have a node with multiple outpoints to have shared inpoint!");
                        TrySetConnectionPointSkin(m_SelectedOutPoint, 0);
                    }

                }


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

        //For now used for startnode removal
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

        //Second form of remove node function where it uses lem_Baseeffects instead of nodes cause nodes' referrecnes arent the same during command invokaton
        void OnClickRemoveNode(NodeBaseData nB)
        {
            //Check if there is any connections to be removed from this node's outpoint
            if (nB.HasAtLeastOneNextPointNode)
            {
                for (int i = 0; i < nB.m_NextPointsIDs.Length; i++)
                {
                    TryToRemoveConnection(nB.m_NextPointsIDs[i], nB.m_NodeID);
                }
            }

            //Remove any and allconnections connected to the node's inpoint
            string[] allNodesConnectedToInPoint = AllEffectsNodeInEditor[nB.m_NodeID].effectNode.m_InPoint.GetAllConnectedNodeIDs();

            for (int i = 0; i < allNodesConnectedToInPoint.Length; i++)
            {
                OnClickRemoveConnection(AllConnectionsDictionary[new Tuple<string, string>(nB.m_NodeID, allNodesConnectedToInPoint[i])]);
            }

            //Remove node from selected collection if it is inside
            TryToRemoveNodeFromSelectedCollection(nB.m_NodeID);

            //O(n) operation only, inother words same as list.Remove( )
            //Need nodeid to be checked cause Node references are lost during command invoker
            int indexOfNodeToRemove = AllNodesInEditor.FindIndex(x => x.NodeID == nB.m_NodeID);
            AllNodesInEditor.RemoveEfficiently(indexOfNodeToRemove);

            if (AllEffectsNodeInEditor.ContainsKey(nB.m_NodeID))
                AllEffectsNodeInEditor.Remove(nB.m_NodeID);
        }

        //Use this if you know exactly what nodes to connect
        //For now only used for saving and loading
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
        void CreateConnection(string inPointNodeID, string outPointNodeID, int outPointIndex)
        {
            ConnectionPoint inPoint = default;
            ConnectionPoint outPoint = default;

            //Assign the inpoint n outpoint by first checking if either one of them are start nodes
            if (inPointNodeID == StartNode.NodeID)
            {
                inPoint = StartNode.m_InPoint;
                outPoint = AllEffectsNodeInEditor[outPointNodeID].outConnectionPoints[0];
            }
            else if (outPointNodeID == StartNode.NodeID)
            {
                outPoint = StartNode.m_OutPoint;
                inPoint = AllEffectsNodeInEditor[inPointNodeID].effectNode.m_InPoint;
            }
            //Else if both em arent start nodes that means both of em are effecNodes
            else
            {
                OutConnectionPoint[] outs = AllEffectsNodeInEditor[outPointNodeID].outConnectionPoints;
                outPoint = outs.Length == 1 ? AllEffectsNodeInEditor[outPointNodeID].effectNode.m_OutPoint : AllEffectsNodeInEditor[outPointNodeID].outConnectionPoints[outPointIndex];
                inPoint = AllEffectsNodeInEditor[inPointNodeID].effectNode.m_InPoint;
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
            m_AllSelectedNodes.Remove(nodeToRemove);
        }

        void TryToRemoveNodeFromSelectedCollection(string id)
        {
            //If all selected doesnt contain this node, add it
            int removeAt = m_AllSelectedNodes.FindIndex(node => node.NodeID == id);
            if (removeAt >= 0)
                m_AllSelectedNodes.RemoveEfficiently(removeAt);
        }

        void DeselectAllNodes()
        {
            for (int i = 0; i < AllNodesInEditor.Count; i++)
            {
                AllNodesInEditor[i].DeselectNode();
            }
        }

        #endregion


        #region Saving and Loading

        //To be called before user presses "Play Button"
        void SaveBeforeEnterPlayMode(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode)
                return;

            if (instance == null || s_CurrentLE == null)
                return;

            SaveToLinearEvent();

            //Save string path of current LE to editor pref
            //string sceneAssetBasePath = EditorSceneManager.GetActiveScene().path;
            string linearEventScenePath = s_CurrentLE.transform.GetGameObjectPath();

            //EditorPrefs.SetString("sceneAssetBasePath", sceneAssetBasePath);
            EditorPrefs.SetString(k_EditorPref_LinearEventKey, linearEventScenePath);
        }

        void TryToSaveLinearEvent()
        {
            if (!s_Settings.m_AutoSave)
                return;

            if (m_EditorState == EDITORSTATE.LOADED)
                SaveToLinearEvent();
        }

        //To be called when player presses "Save button" or when assembly reloads every time a script changes (when play mode is entered this will get called also but it doesnt save the values to the LE)
        void SaveToLinearEvent()
        {
            m_EditorState = EDITORSTATE.SAVING;

            AllNodesInEditor.Remove(StartNode);

            LEM_BaseEffect[] lemEffects = new LEM_BaseEffect[AllNodesInEditor.Count];
            BaseEffectNode[] allEffectNodes = AllNodesInEditor.ConvertAll(x => (BaseEffectNode)x).ToArray();

            //Clear the dictionary of the currently editting linear event
            //s_CurrentLE.m_AllEffectsDictionary = new Dictionary<string, LEM_BaseEffect>();
            float intToFloatConverter = AllNodesInEditor.Count;

            //This saves all events regardless of whether they are connected singularly, plurally or disconnected
            for (int i = 0; i < AllNodesInEditor.Count; i++)
            {
                lemEffects[i] = allEffectNodes[i].CompileToBaseEffect();
            }

            //Save to serializable array of effects
            s_CurrentLE.m_AllEffects = lemEffects;

            //Save start and end node data
            s_CurrentLE.m_StartNodeData = StartNode.SaveNodeData();

            //Saving ends here
            AllNodesInEditor.Add(StartNode);

            //Finished loading
            Repaint();
            Debug.Log("Saved Linear Event File " + s_CurrentLE.name, s_CurrentLE);
            m_EditorState = EDITORSTATE.SAVED;
        }

        void LoadAfterExitingPlayMode(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredEditMode)
                return;

            //Get string paths from editorprefs
            //string sceneAssetBasePath = EditorPrefs.GetString("sceneAssetBasePath");
            string linearEventPath = EditorPrefs.GetString(k_EditorPref_LinearEventKey);

            if (string.IsNullOrEmpty(linearEventPath))
                return;

            LinearEvent prevLE = GameObject.Find(linearEventPath).GetComponent<LinearEvent>();

            NodeLEM_Editor.LoadNodeEditor(prevLE);
        }

        void LoadFromLinearEvent()
        {
            if (instance == null || s_CurrentLE == null)
                return;

            m_EditorState = EDITORSTATE.LOADING;
            #region Loading Events from Dictionary

            //Dont do any thing if there is no effects in the dicitionary
            Dictionary<string, LEM_BaseEffect> allEffectsDictInLinearEvent = s_CurrentLE.AllEffectsDictionary;

            if (allEffectsDictInLinearEvent == null)
            {
                Repaint();
                return;
            }

            string[] allKeys = allEffectsDictInLinearEvent.Keys.ToArray();

            BaseEffectNode newEffectNode;

            //Recreate all the nodes from the dictionary
            for (int i = 0; i < allKeys.Length; i++)
            {
                newEffectNode = RecreateEffectNode(allEffectsDictInLinearEvent[allKeys[i]].m_NodeBaseData.m_Position,
                    allEffectsDictInLinearEvent[allKeys[i]].m_NodeEffectType,
                    allEffectsDictInLinearEvent[allKeys[i]].m_NodeBaseData.m_NodeID);

                //Load the new node with saved node values values
                newEffectNode.LoadFromBaseEffect(allEffectsDictInLinearEvent[allKeys[i]]);
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
                if (!allEffectsDictInLinearEvent[allKeys[i]].m_NodeBaseData.HasAtLeastOneNextPointNode)
                    continue;

                TryToRestichConnections(allEffectsDictInLinearEvent[allKeys[i]]);
            }
            #endregion

            //Dont stitch up start node if it isnt connected to at least one point
            if (s_CurrentLE.m_StartNodeData.HasAtLeastOneNextPointNode)
            {
                //Do the same for start and end nodes
                //if node has a m_NextPointNodeID and that the next node this node is assigned to doesnt have a connection on the outpoint,
                if (!String.IsNullOrEmpty(s_CurrentLE.m_StartNodeData.m_NextPointsIDs[0])
                    && !StartNode.m_OutPoint.IsConnected)
                {
                    //Else just find the next node from the dictionary of all effects node
                    CreateConnection(
                                     AllEffectsNodeInEditor[s_CurrentLE.m_StartNodeData.m_NextPointsIDs[0]].effectNode.m_InPoint,
                                     StartNode.m_OutPoint
                                    );
                }
            }

            Repaint();
            m_EditorState = EDITORSTATE.LOADED;
        }

        LEM_BaseEffect CompileNodeToEffect(string nodeID)
        {
            return AllEffectsNodeInEditor[nodeID].effectNode.CompileToBaseEffect();
        }

        #endregion

    }

}