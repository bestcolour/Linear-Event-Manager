using UnityEditor.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using LEM_Effects;
using LEM_Effects.Extensions;
using System.Linq;

namespace LEM_Editor
{
    public class BaseEffectNodePair
    {
        public BaseEffectNode effectNode;
        public OutConnectionPoint[] outConnectionPoints;

        public BaseEffectNodePair(BaseEffectNode effectNode, OutConnectionPoint[] outConnectionPoints)
        {
            this.effectNode = effectNode;
            this.outConnectionPoints = outConnectionPoints;
        }
    }

    public class NodeLEM_Editor : EditorWindow
    {
        #region Loading States
        struct EDITORSTATE
        {
            //UNLOADED = there is no linear event loaded yet, LOADED = there is linear event loaded but there is also changes made
            //SAVED = linear event was just saved, SAVING = linear event is current in the midsts of saving
            public const int UNLOADED = -1, LOADED = 0, LOADING = 1, SAVED = 2, SAVING = 3;
            public const string SAVED_STRING = "Saved!", LOADED_STRING = "Save"/* Effects \n (Crlt + S)"*/, AUTOSAVE_STRING = "Auto\nSave\nOn";
        }

        int m_EditorState = EDITORSTATE.UNLOADED;

        public static NodeLEM_Editor Instance { get; private set; } = default;
        public static LinearEvent CurrentLE { get; private set; } = default;
        public static NodeLEM_Settings Settings { get; private set; } = default;

        //Gameobject container which stores all the nodecommandinvoker's data 
        public static GameObject EditorEffectsContainer { get; private set; } = null;

        //const string k_EditorPref_EditorEffectsContainerKey = "effectsContainerPath";
        const string k_EditorPref_LinearEventKey = "linearEventScenePath";
        const string k_EditorPref_SettingsKey = "currentSettings";
        const string k_DefaultSettingsFolderAssetPath = "Assets/Editor/Node_LEM/Settings";

        #endregion

        //For saving 
        List<Node> m_AllConnectableNodesInEditor = new List<Node>();
        public static List<Node> AllConnectableNodesInEditor => Instance.m_AllConnectableNodesInEditor;

        List<Node> m_AllGroupRectNodesInEditor = new List<Node>();
        public static List<Node> AllGroupRectNodesInEditor => Instance.m_AllGroupRectNodesInEditor;


        Dictionary<string, GroupRectNode> m_AllGroupRectsInEditorDictionary = new Dictionary<string, GroupRectNode>();
        public static Dictionary<string, GroupRectNode> AllGroupRectsInEditorDictionary => Instance.m_AllGroupRectsInEditorDictionary;
        public static string[] AllGroupRectsInEditorNodeIDs => AllGroupRectsInEditorDictionary.Keys.ToArray();

        Dictionary<string, BaseEffectNodePair> m_AllEffectsNodeInEditor = new Dictionary<string, BaseEffectNodePair>();
        public static Dictionary<string, BaseEffectNodePair> AllEffectsNodeInEditor => Instance.m_AllEffectsNodeInEditor;

        void EditEffectNodeStruct(BaseEffectNodePair nodeStruct)
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

        public static string GetInitials(string nodeID)
        {
            char[] chars = nodeID.ToCharArray();
            string idInitials = "";

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '_')
                {
                    idInitials += chars[i];
                    break;
                }

                idInitials += chars[i];
            }

            return idInitials;
        }

        //RULE: INPOINT'S CONNECTED NODE ID FIRST THEN OUTPOINT CONNECTED NODE ID
        Dictionary<Tuple<string, string>, Connection> m_AllConnectionsDictionary = new Dictionary<Tuple<string, string>, Connection>();
        static Dictionary<Tuple<string, string>, Connection> AllConnectionsDictionary => Instance.m_AllConnectionsDictionary;

        ConnectableNode m_StartNode = default;
        public static ConnectableNode StartNode { get { return Instance.m_StartNode; } set { Instance.m_StartNode = value; } }

        Action d_OnGUI = null;

        #region Process Event Variables

        List<Node> m_AllSelectedNodes = new List<Node>();
        public static List<Node> AllSelectedNodes => Instance.m_AllSelectedNodes;

        Node s_CurrentClickedNode = null;
        public static Node CurrentClickedNode => Instance.s_CurrentClickedNode;
        public static bool? CurrentNodeLastRecordedSelectState { get; set; }

        //Check if there is multiple nodes selected
        public static bool s_HaveMultipleNodeSelected => (Instance.m_AllSelectedNodes.Count > 0);

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

            connectionPoint.m_Style = isSelected == 1 ? LEMStyleLibrary.ConnectionPointStyleSelected : LEMStyleLibrary.ConnectionPointStyleNormal;
        }

        void TrySetConnectionPoint(ConnectionPoint connectionPoint)
        {
            if (connectionPoint == null)
                return;

            connectionPoint.m_Style = connectionPoint.IsConnected ? LEMStyleLibrary.ConnectionPointStyleSelected : LEMStyleLibrary.ConnectionPointStyleNormal;
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

        public static float InverseScaleFactor => Instance.m_InverseScaleFactorOnEveryGUIFrame;
        float m_InverseScaleFactorOnEveryGUIFrame = default;

        //Vector2 m_PreviousZoomMousePosition = Vector2.zero;
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

        void OnConfirm(string result, Vector2 mousePos, bool searchBoxState)
        {
            mousePos *= 1 / ScaleFactor;
            CommandInvoker.InvokeCommand(new CreateNodeCommand(mousePos, result));
            Instance.m_IsSearchBoxActive = searchBoxState;
        }

        //GenericMenu m_NodeContextMenu = default;
        #endregion

        #endregion

        #region NodeInvoker

        NodeCommandInvoker m_CommandInvoker = default;
        static NodeCommandInvoker CommandInvoker => Instance.m_CommandInvoker;
        bool m_IsDragging = default;
        //public static int s_MaxActions = 100;

        void DoCutCommand()
        {
            if (AllSelectedNodes.Contains(StartNode))
            {
                StartNode.DeselectNode();
            }

            CommandInvoker.InvokeCommand(new CutCommand(m_AllSelectedNodes.ToArray()));
        }

        void DoPasteCommand()
        {
            //Else if there is stuff copied on the clipboard of the nodeinvoker then you can paste 
            if (NodeCommandInvoker.HasEffectsCopied || NodeCommandInvoker.HasGroupRectsCopied)
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

        void DoDeleteCommand()
        {
            GUI.FocusControl(null);

            //Remove start node
            if (StartNode.IsSelected)
            {
                StartNode.DeselectNode();
            }

            CommandInvoker.InvokeCommand(new DeleteNodeCommand(m_AllSelectedNodes.Select(x => x.NodeID).ToArray()));
        }

        void DoCopy()
        {
            //Remove start and end node 
            //Filter out alll the grouprect nodes in the selectednodes
            for (int i = 0; i < AllSelectedNodes.Count; i++)
                if (AllSelectedNodes[i].ID_Initial != LEMDictionary.NodeIDs_Initials.k_BaseEffectInital)
                    AllSelectedNodes[i].DeselectNode();

            CommandInvoker.CopyToClipBoard(AllSelectedNodes.Select(x => x.NodeID).ToArray());

        }

        void DoGroup()
        {
            CommandInvoker.InvokeCommand(new GroupCommand(AllSelectedNodes));
        }

        #endregion

        Texture2D m_EditorBackGroundTexture = default;
        Texture2D EditorBackGroundTexture => Instance.m_EditorBackGroundTexture;


        #region Initialisation

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
            if (CurrentLE == null)
            {
                m_EditorState = EDITORSTATE.UNLOADED;
                d_OnGUI = EmptyEditorUpdate;
            }

            LoadSettings();

            //Well regardless of it being empty or not, ensure that node editor saves before reloading assembly
            //AssemblyReloadEvents.beforeAssemblyReload += SaveToLinearEvent;
            AssemblyReloadEvents.beforeAssemblyReload += SaveToLinearEvent;
            AssemblyReloadEvents.beforeAssemblyReload += DeleteEditorContainer;

            //Due to beforeAssemblyReload being called when player enters play mode but doesnt save values, this needs to be added
            EditorApplication.playModeStateChanged += SaveBeforeEnterPlayMode;
            EditorApplication.playModeStateChanged += LoadAfterExitingPlayMode;
            EditorApplication.quitting += TryToSaveLinearEvent;

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
                    Settings = settings;
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
                        Settings = settings;
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
            Settings = settings;

        }

        //Called only when you pressed the Load Linear Event button
        public static void LoadNodeEditor(LinearEvent linearEvent)
        {

            //This will be a key identifier for whether LoadNodeEditor button was pressed
            LinearEvent prevLE = CurrentLE;
            CurrentLE = linearEvent;



            //If this is the first time you are opening the window, or if u have previously closed the window and you wish to reopen it
            if (Instance == null)
                InitialiseWindow();
            else
            {
                //Save the prev lienarevent
                CurrentLE = prevLE;
                Instance.TryToSaveLinearEvent();

                //if (prevLE)
                    //prevLE.CurrentlyBeingEdited = false;

                //Load the new one
                CurrentLE = linearEvent;
                //CurrentLE.CurrentlyBeingEdited = true;
                Instance.ResetandLoadNewEvent();
            }

            //Create the gameobjects which will store the monobehaviours for the various effects
            if (EditorEffectsContainer == null)
            {
                EditorEffectsContainer = new GameObject();
                EditorEffectsContainer.name = "EditorEffectsContainer";
                EditorEffectsContainer.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        //Form of intialisation from pressing Load Linear Event and that it is the first time/you plan to reopen the node editor
        public static void InitialiseWindow()
        {
            //Get window and this will trigger OnEnable
            NodeLEM_Editor editorWindow = GetWindow<NodeLEM_Editor>();

            //Set the title of gui for the window to be TEM Node Editor
            editorWindow.titleContent = new GUIContent("TEM Node Editor");

            editorWindow.d_OnGUI = editorWindow.Initialise;

        }


        //To be called on the very first time of pressing "LoadNodeEditor"? 
        //This is also called when you hv the window docked in ur panels but u dont give focus on it and u just open ur project
        void Initialise()
        {
            //Call these only once in the flow of usage until the window is closed
            Instance = this;

            LEMStyleLibrary.LoadLibrary();
            LEMDictionary.LoadDictionary();

            //Intialising background
            Instance.m_EditorBackGroundTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            Color bgColour = Settings.m_EditorTheme == EditorTheme.Dark ? new Color(0.227f, 0.216f, 0.212f) : new Color(0.8f, 0.8f, 0.8f);
            Instance.m_EditorBackGroundTexture.SetPixel(0, 0, bgColour);
            Instance.m_EditorBackGroundTexture.Apply();

            if (Instance.m_CommandInvoker == null)
            {
                Instance.m_CommandInvoker = new NodeCommandInvoker(Settings.m_HistoryLength,/* CreateEffectNode,*/ /*RecreateEffectNode,*//* CreateGroupNode,*/ TryToRestichConnections,/* CompileNodeToEffect, *//*MoveNodes,*/ /*CreateConnection, TryToRemoveConnection,*/ /*DeselectAllNodes, */() => m_EditorState = EDITORSTATE.LOADED);
            }

            if (Instance.m_AllConnectableNodesInEditor == null)
            {
                Instance.m_AllConnectableNodesInEditor = new List<Node>();
            }

            if (Instance.m_AllGroupRectNodesInEditor == null)
            {
                Instance.m_AllGroupRectNodesInEditor = new List<Node>();
            }

            if (Instance.m_AllEffectsNodeInEditor == null)
            {
                Instance.m_AllEffectsNodeInEditor = new Dictionary<string, BaseEffectNodePair>();
            }

            if (Instance.m_AllGroupRectsInEditorDictionary == null)
            {
                Instance.m_AllGroupRectsInEditorDictionary = new Dictionary<string, GroupRectNode>();
            }

            if (Instance.m_AllConnectionsDictionary == null)
            {
                Instance.m_AllConnectionsDictionary = new Dictionary<Tuple<string, string>, Connection>();
            }

            if (Instance.m_AllSelectedNodes == null)
            {
                Instance.m_AllSelectedNodes = new List<Node>();
            }

            if (Instance.m_SearchBox == null)
            {
                Instance.m_SearchBox = new LEM_SearchBox(Instance.OnInputChange, Instance.OnConfirm, 250, 325);
            }

            //Regardless, just initialise strt end nodes
            Instance.InitialiseStartEndNodes();
            Instance.LoadFromLinearEvent();
            GUIUtility.keyboardControl = 0;

            //After finishing all the intialisation and loading of linearevent,
            d_OnGUI = UpdateGUI;

            Instance.m_EditorState = EDITORSTATE.SAVED;
        }

        void InitialiseStartEndNodes()
        {
            if (StartNode == null)
            {
                CreateConnectableNode(new Vector2(EditorGUIUtility.currentViewWidth * 0.5f, 50f), "StartNode", out ConnectableNode startTempNode);
                StartNode = startTempNode;
            }
        }

        #endregion

        #region Resets

        void ResetEventVariables()
        {
            Instance.m_InitialClickedPosition = null;
            Instance.m_IsDragging = false;
            s_SelectionBox = Rect.zero;
            ResetSelectedNode();
            GUI.changed = true;
        }

        void ResetDrawingBezierCurve()
        {
            //Reset bezierline drawing
            Instance.m_SelectedOutPoint = null;
            Instance.m_SelectedInPoint = null;
        }

        void ResetandLoadNewEvent()
        {
            ResetAll();

            //Regardless, just initialise strt end nodes
            Instance.InitialiseStartEndNodes();
            Instance.LoadFromLinearEvent();

            //After finishing all the intialisation and loading of linearevent,
            d_OnGUI = UpdateGUI;

            m_EditorState = EDITORSTATE.SAVED;
        }

        void ResetandLoadEmptyEvent()
        {
            ResetAll();

            d_OnGUI = EmptyEditorUpdate;

            m_EditorState = EDITORSTATE.UNLOADED;
        }




        void ResetAll()
        {
            //All the resets
            StartNode = null;
            ResetDrawingBezierCurve();
            ResetEventVariables();
            CurrentNodeLastRecordedSelectState = null;
            m_IsSearchBoxActive = false;

            m_AllConnectableNodesInEditor = new List<Node>();
            m_AllGroupRectNodesInEditor = new List<Node>();
            m_AllGroupRectsInEditorDictionary = new Dictionary<string, GroupRectNode>();

            m_AllEffectsNodeInEditor = new Dictionary<string, BaseEffectNodePair>();
            m_AllConnectionsDictionary = new Dictionary<Tuple<string, string>, Connection>();
            m_CommandInvoker.ResetHistory();
        }

        //Called when window is closed
        void OnDestroy()
        {
            TryToSaveLinearEvent();

            //Unsubscribe b4 closing window
            AssemblyReloadEvents.beforeAssemblyReload -= SaveToLinearEvent;
            AssemblyReloadEvents.beforeAssemblyReload -= DeleteEditorContainer;

            EditorApplication.playModeStateChanged -= SaveBeforeEnterPlayMode;
            EditorApplication.playModeStateChanged -= LoadAfterExitingPlayMode;
            EditorApplication.quitting -= TryToSaveLinearEvent;

            EditorPrefs.SetString(k_EditorPref_LinearEventKey, "");

            if (Settings != null)
                EditorPrefs.SetString(k_EditorPref_SettingsKey, k_DefaultSettingsFolderAssetPath + "/" + Settings.name);

            DeleteEditorContainer();
            CurrentLE = null;
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

            CurrentLE = (LinearEvent)EditorGUI.ObjectField(propertyRect, CurrentLE, typeof(LinearEvent), true);

            propertyRect.y += EditorGUIUtility.singleLineHeight * 2f;
            GUI.Label(propertyRect, "Settings");
            propertyRect.y += EditorGUIUtility.singleLineHeight * 2f;
            Settings = (NodeLEM_Settings)EditorGUI.ObjectField(propertyRect, Settings, typeof(NodeLEM_Settings), false);

            //Then once Current Linear Event is selected,
            if (CurrentLE != null && Settings != null)
            {
                EditorPrefs.SetString(k_EditorPref_SettingsKey, k_DefaultSettingsFolderAssetPath + "/" + Settings.name);
                LoadNodeEditor(CurrentLE);
            }
        }

        void UpdateGUI()
        {
            //To check if reference is destroyed or smth
            if (CurrentLE == null)
            {
                ResetandLoadEmptyEvent();
                return;
            }

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
            //EditorZoomFeature.BeginZoom(ScaleFactor, new Rect(0f, 0f, Screen.width, Screen.height), m_PreviousZoomMousePosition);
            Vector2 currMousePos = currentEvent.mousePosition;
            DetermineAllNodes();

            DrawNodes();

            DrawConnections();
            DrawConnectionLine(currentEvent);
            DrawSelectionBox(currMousePos);

            EditorZoomFeature.EndZoom();
            bool isMouseInSearchBox = HandleSearchBox(currentEvent);


            if (Settings.m_ShowToolBar)
            {
                DrawToolButtons(dummyRect);
            }
            HandleCurrentLinearEventLabel(dummyRect, currentEvent);

            //DrawDebugLists();

            //Then process the events that occured from unity's events (events are like clicks,drag etc)
            ProcessEvents(currentEvent, currMousePos, isMouseInSearchBox);
            ProcessNodeEvents(currentEvent);

            LEMStyleLibrary.AssertEditorLabelColour();

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

        void OnLostFocus()
        {
            TryToSaveLinearEvent();
        }

        #endregion

        #region Draw Functions

        void DetermineAllNodes()
        {
            for (int i = 0; i < AllConnectableNodesInEditor.Count; i++)
            {
                AllConnectableNodesInEditor[i].DetermineStatus();
            }

            for (int i = 0; i < AllGroupRectNodesInEditor.Count; i++)
            {
                AllGroupRectNodesInEditor[i].DetermineStatus();
            }

            if (s_CurrentClickedNode != null)
                s_CurrentClickedNode.DetermineStatus();
        }

        void DrawNodes()
        {
            if (s_CurrentClickedNode != null)
            {
                if (s_CurrentClickedNode.ID_Initial != LEMDictionary.NodeIDs_Initials.k_GroupRectNodeInitial)
                {
                    for (int i = AllGroupRectNodesInEditor.Count - 1; i > -1; i--)
                        if (AllGroupRectNodesInEditor[i].IsWithinWindowScreen)
                            AllGroupRectNodesInEditor[i].Draw();

                    //If nodes collection is not null
                    for (int i = 0; i < AllConnectableNodesInEditor.Count; i++)
                        if (AllConnectableNodesInEditor[i].IsWithinWindowScreen)
                            AllConnectableNodesInEditor[i].Draw();

                    //Draw the selected node last cause it shuld be rendered last since it is a connectable node
                    if (s_CurrentClickedNode.IsWithinWindowScreen)
                        s_CurrentClickedNode.Draw();
                }
                //Else selected node is grouprectnode,
                else
                {
                    for (int i = AllGroupRectNodesInEditor.Count - 1; i > -1; i--)
                        if (AllGroupRectNodesInEditor[i].IsWithinWindowScreen)
                            AllGroupRectNodesInEditor[i].Draw();

                    //Draw the selected groupNode on top of the other group rects but not over the connectablenodes
                    if (s_CurrentClickedNode.IsWithinWindowScreen)
                        s_CurrentClickedNode.Draw();

                    //If nodes collection is not null
                    for (int i = 0; i < AllConnectableNodesInEditor.Count; i++)
                        if (AllConnectableNodesInEditor[i].IsWithinWindowScreen)
                            AllConnectableNodesInEditor[i].Draw();

                }

            }
            //Else if theres no selected node just continue normally
            else
            {
                for (int i = AllGroupRectNodesInEditor.Count - 1; i > -1; i--)
                    if (AllGroupRectNodesInEditor[i].IsWithinWindowScreen)
                        AllGroupRectNodesInEditor[i].Draw();

                //If nodes collection is not null
                for (int i = 0; i < AllConnectableNodesInEditor.Count; i++)
                    if (AllConnectableNodesInEditor[i].IsWithinWindowScreen)
                        AllConnectableNodesInEditor[i].Draw();
            }

        }

        void DrawConnections()
        {
            Tuple<string, string>[] allTupleKeys = AllConnectionsDictionary.Keys.ToArray();
            for (int i = 0; i < allTupleKeys.Length; i++)
                if (AllConnectionsDictionary[allTupleKeys[i]].IsWithinWindowScreen)
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
                   LEMStyleLibrary.CurrentBezierColour,
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
                    LEMStyleLibrary.CurrentBezierColour,
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
            float buttonWidth = Settings.m_ButtonSize;
            //Align button to the right of the screen
            propertyRect.x = position.width - buttonWidth;
            propertyRect.y = 0f;
            propertyRect.width = buttonWidth;
            propertyRect.height = 50f;

            #region Drawing Save Button

            if (Settings.m_SaveSettings != NodeLEM_Settings.SaveSettings.AlwaysSave)
            {
                if (GUI.Button(propertyRect, EDITORSTATE.LOADED_STRING))
                {
                    //m_EditorState = EDITORSTATE.SAVING;
                    SaveToLinearEvent();
                    //m_EditorState = EDITORSTATE.SAVED;
                }
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
                DoPasteCommand();
            }

            propertyRect.x -= buttonWidth;

            if (GUI.Button(propertyRect, "Copy"))
            {
                DoCopy();
                e.Use();
            }

            propertyRect.x -= buttonWidth;

            if (GUI.Button(propertyRect, "Cut"))
            {
                DoCutCommand();
                //e.Use();
                GUI.changed = true;
            }

            propertyRect.x -= buttonWidth;

            if (GUI.Button(propertyRect, "Delete"))
            {
                DoDeleteCommand();
                e.Use();

            }

            propertyRect.x -= buttonWidth;

            if (GUI.Button(propertyRect, "Group"))
            {
                DoGroup();
                GUI.changed = true;
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
            string label = "Linear Event : " + CurrentLE.name;
            propertyRect.size = GUI.skin.label.CalcSize(new GUIContent(label, "The Linear Event you are currently editting"));
            propertyRect.x = 0;
            propertyRect.y = 0;

            LEMStyleLibrary.BeginEditorLabelColourChange(Color.red);
            EditorGUI.LabelField(propertyRect, label);
            LEMStyleLibrary.EndEditorLabelColourChange();

            //If current event type is a mouse click and mouse click is within the propertyrect,
            if (currentEvent.type == EventType.MouseDown && propertyRect.Contains(currentEvent.mousePosition))
            {
                EditorGUIUtility.PingObject(CurrentLE);
            }
        }

        private void DrawDebugLists()
        {
            //Copy previous colour and set the color red
            LEMStyleLibrary.GUIPreviousColour = GUI.skin.label.normal.textColor;
            GUI.skin.label.normal.textColor = Color.red;

            Rect propertyRect = Rect.zero;
            propertyRect.y += EditorGUIUtility.singleLineHeight;
            propertyRect.height += EditorGUIUtility.singleLineHeight;
            propertyRect.width += EditorGUIUtility.currentViewWidth;

            #region S_CurrentlyClickedNode

            GUI.Label(propertyRect, "Currently clicked node ");
            propertyRect.y += EditorGUIUtility.singleLineHeight;
            GUI.Label(propertyRect, s_CurrentClickedNode?.NodeID);
            propertyRect.y += EditorGUIUtility.singleLineHeight;

            #endregion


            #region All Nodes In Editor Debug

            GUI.skin.label.normal.textColor = Color.yellow;

            GUI.Label(propertyRect, "All Connectable Nodes in Editor");
            propertyRect.y += EditorGUIUtility.singleLineHeight;

            for (int i = 0; i < AllConnectableNodesInEditor.Count; i++)
            {
                GUI.Label(propertyRect, i + ") " + AllConnectableNodesInEditor[i].NodeID);
                propertyRect.y += EditorGUIUtility.singleLineHeight;
            }

            GUI.skin.label.normal.textColor = Color.green;

            GUI.Label(propertyRect, "All GroupRectList Nodes in Editor");
            propertyRect.y += EditorGUIUtility.singleLineHeight;

            for (int i = 0; i < AllGroupRectNodesInEditor.Count; i++)
            {
                GUI.Label(propertyRect, i + ") " + AllGroupRectNodesInEditor[i].NodeID);
                propertyRect.y += EditorGUIUtility.singleLineHeight;
            }

            #endregion


            //#region All GroupRect Nodes
            //GUI.Label(propertyRect, "All GroupRect Nodes in Editor");
            //propertyRect.y += EditorGUIUtility.singleLineHeight;
            //string[] keys = AllGroupRectsInEditorNodeIDs;
            //for (int i = 0; i < keys.Length; i++)
            //{
            //    GUI.Label(propertyRect, i + ") " + AllGroupRectsInEditorDictionary[keys[i]].NodeID);
            //    propertyRect.y += EditorGUIUtility.singleLineHeight;
            //}

            //#endregion

            #region SelectedNodes

            GUI.skin.label.normal.textColor = Color.magenta;

            propertyRect.y += EditorGUIUtility.singleLineHeight;
            GUI.Label(propertyRect, "All SelectedNodes in Editor");
            propertyRect.y += EditorGUIUtility.singleLineHeight;

            for (int i = 0; i < AllSelectedNodes.Count; i++)
            {
                GUI.Label(propertyRect, i + ") " + AllSelectedNodes[i].NodeID);
                propertyRect.y += EditorGUIUtility.singleLineHeight;
            }
            #endregion

            #region All Connections

            //propertyRect.y += EditorGUIUtility.singleLineHeight;

            //GUI.skin.label.normal.textColor = Color.cyan;
            //Tuple<string, string>[] inPointID_outPointID = AllConnectionsDictionary.Keys.ToArray();

            //GUI.Label(propertyRect, "All Connections");
            //propertyRect.y += EditorGUIUtility.singleLineHeight;

            //for (int i = 0; i < inPointID_outPointID.Length; i++)
            //{
            //    GUI.Label(propertyRect, i + ") " + inPointID_outPointID[i]);
            //    propertyRect.y += EditorGUIUtility.singleLineHeight;
            //}

            #endregion

            GUI.skin.label.normal.textColor = LEMStyleLibrary.GUIPreviousColour;


        }

        bool HandleSearchBox(Event e)
        {
            if (m_IsSearchBoxActive)
                return m_SearchBox.HandleSearchBox(e);

            return false;
        }

        #endregion

        #region Process Events
        //Checks what the current event is right now, and then execute code accordingly
        void ProcessEvents(Event e, Vector2 currMousePosition, bool isMouseInSearchBox)
        {
            m_AmountOfMouseDragThisUpdate = Vector2.zero;
            switch (e.type)
            {
                case EventType.ScrollWheel:

                    if (!isMouseInSearchBox)
                    {
                        //m_PreviousZoomMousePosition = currMousePosition;

                        int signOfChange;
                        float changeRate;

                        signOfChange = e.delta.y > 0 ? -1 : 1;
                        //If alt key is pressed,
                        changeRate = e.alt ? k_SlowScaleChangeRate : k_ScaleChangeRate;

                        ScaleFactor += signOfChange * changeRate;

                        e.Use();
                    }

                    break;

                case EventType.MouseDown:

                    //Set the currenly clicked node
                    GetNewClickedNode(currMousePosition);

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
                        //m_NodeContextMenu.ShowAsContext();
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

                        //Remove focus on the controls when user clicks on something regardless if it is a node or not because apparently this doesnt get
                        //called when i click on input/text fields
                        GUIUtility.keyboardControl = 0;
                        GUI.FocusControl(null);

                        if (!isMouseInSearchBox)
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
                        //If user is currently planning to drag a node and wasnt draggin the previous gui loop, (that means this is the first drag event after on clickevent)
                        else if (s_CurrentClickedNode != null && !m_IsDragging)
                        {
                            m_IsDragging = true;
                            //Since selected nodes will clear itself of the rect node before moving the nodes,
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
                        DoDeleteCommand();
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
                            DoCopy();
                            e.Use();
                        }
                        //Cut
                        else if (e.keyCode == KeyCode.X)
                        {
                            DoCutCommand();
                            GUI.changed = true;
                        }
                        //Paste only when there is no foccus on anyother keyboard demanding control,
                        else if (e.keyCode == KeyCode.V && GUIUtility.keyboardControl == 0)
                        {
                            DoPasteCommand();
                        }
                        //Select all
                        else if (e.keyCode == KeyCode.A)
                        {
                            //Only when the focused control is null,
                            if (GUIUtility.keyboardControl == 0)
                            {
                                SelectAllNodes();
                                GUI.changed = true;
                            }

                        }
                        //Save
                        else if (Settings.m_SaveSettings != NodeLEM_Settings.SaveSettings.AlwaysSave && e.keyCode == KeyCode.S)
                        {
                            SaveToLinearEvent();
                            e.Use();
                        }
                        //Group Comment
                        else if (s_HaveMultipleNodeSelected && e.keyCode == KeyCode.G)
                        {
                            DoGroup();
                            GUI.changed = true;
                        }
                        //Spawn all node types
                        else if (e.keyCode == KeyCode.M)
                        {
                            for (int i = 0; i < LEMDictionary.s_NodeTypeKeys.Length; i++)
                            {
                                CommandInvoker.InvokeCommand(new CreateNodeCommand(currMousePosition, LEMDictionary.s_NodeTypeKeys[i]));
                            }
                            GUI.changed = true;
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
                    //Check if it is the left mousebutton that was pressed
                    if (e.button == 0)
                    {
                        //Always call current clicked node's event first regardless if it is  groupnode or connectable node
                        if (s_CurrentClickedNode != null)
                            if (s_CurrentClickedNode.HandleLeftMouseDown(e))
                                GUI.changed = true;

                        for (int i = 0; i < AllConnectableNodesInEditor.Count; i++)
                            if (AllConnectableNodesInEditor[i].IsWorthProcessingEventFor && AllConnectableNodesInEditor[i].HandleLeftMouseDown(e))
                                GUI.changed = true;

                        for (int i = 0; i < AllGroupRectNodesInEditor.Count; i++)
                            if (AllGroupRectNodesInEditor[i].IsWorthProcessingEventFor && AllGroupRectNodesInEditor[i].HandleLeftMouseDown(e))
                                GUI.changed = true;

                    }
                    break;

                case EventType.MouseUp:
                    //Always call current clicked node's event first regardless if it is  groupnode or connectable node
                    if (s_CurrentClickedNode != null)
                        if (s_CurrentClickedNode.HandleMouseUp())
                            GUI.changed = true;

                    for (int i = 0; i < AllConnectableNodesInEditor.Count; i++)
                        if (AllConnectableNodesInEditor[i].IsWorthProcessingEventFor && AllConnectableNodesInEditor[i].HandleMouseUp())
                            GUI.changed = true;

                    for (int i = 0; i < AllGroupRectNodesInEditor.Count; i++)
                        if (AllGroupRectNodesInEditor[i].IsWorthProcessingEventFor && AllGroupRectNodesInEditor[i].HandleMouseUp())
                            GUI.changed = true;

                    break;

                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        Vector2 convertedDelta = e.delta / ScaleFactor;

                        //Always call current clicked node's event first regardless if it is  groupnode or connectable node
                        if (s_CurrentClickedNode != null)
                            if (s_CurrentClickedNode.HandleLeftMouseButtonDrag(e, convertedDelta))
                                GUI.changed = true;

                        for (int i = 0; i < AllConnectableNodesInEditor.Count; i++)
                            if (AllConnectableNodesInEditor[i].IsWorthProcessingEventFor && AllConnectableNodesInEditor[i].HandleLeftMouseButtonDrag(e, convertedDelta))
                                GUI.changed = true;

                        for (int i = 0; i < AllGroupRectNodesInEditor.Count; i++)
                            if (AllGroupRectNodesInEditor[i].IsWorthProcessingEventFor && AllGroupRectNodesInEditor[i].HandleLeftMouseButtonDrag(e, convertedDelta))
                                GUI.changed = true;

                    }
                    break;

            }
        }

        #endregion

        #region Node Editor Events Functions

        #region Creating Node Types
        //This is used for when you wanna create a new node
        public static BaseEffectNode CreateEffectNode(Vector2 mousePosition, string nameOfNodeType)
        {
            //NodeDictionaryStruct nodeStruct = new NodeDictionaryStruct();
            BaseEffectNode newEffectNode = LEMDictionary.GetNodeObject(nameOfNodeType) as BaseEffectNode;

            //Get the respective skin from the collection of nodeskin
            NodeSkinCollection nodeSkin = LEMStyleLibrary.WhiteBackgroundSkin;

            newEffectNode.GenerateNodeID();

            //Initialise the new node 
            newEffectNode.Initialise
                (mousePosition,
                nodeSkin,
                LEMStyleLibrary.ConnectionPointStyleNormal,
                Instance.OnClickInPoint,
                Instance.OnClickOutPoint,
                Instance.TryToAddNodeToSelectedCollection,
                Instance.TryToRemoveNodeFromSelectedCollection,
                Instance.EditEffectNodeStruct,
                LEMDictionary.GetNodeColour(nameOfNodeType)
                );

            //nodeStruct.effectNode = newEffectNode;
            //nodeStruct.outConnectionPoints = newEffectNode.GetOutConnectionPoints;

            //Add the node into collection in editor
            AllConnectableNodesInEditor.Add(newEffectNode);
            AllEffectsNodeInEditor.Add(newEffectNode.NodeID, new BaseEffectNodePair(newEffectNode, newEffectNode.GetOutConnectionPoints));
            return newEffectNode;
        }

        //This is used for loading and probably undoing/redoing from a linear event
        public static BaseEffectNode RecreateEffectNode(Vector2 positionToSet, string nameOfNodeType, string idToSet)
        {
            //NodeDictionaryStruct nodeStruct = new NodeDictionaryStruct();
            BaseEffectNode newEffectNode = LEMDictionary.GetNodeObject(nameOfNodeType) as BaseEffectNode;

            //Get the respective skin from the collection of nodeskin
            NodeSkinCollection nodeSkin = LEMStyleLibrary.WhiteBackgroundSkin;

            //Initialise the new node 
            newEffectNode.Initialise
                (positionToSet,
                nodeSkin,
                LEMStyleLibrary.ConnectionPointStyleNormal,
                Instance.OnClickInPoint,
                Instance.OnClickOutPoint,
                Instance.TryToAddNodeToSelectedCollection,
                Instance.TryToRemoveNodeFromSelectedCollection,
                Instance.EditEffectNodeStruct,
                LEMDictionary.GetNodeColour(nameOfNodeType)
                );

            newEffectNode.SetNodeID(idToSet);
            //nodeStruct.effectNode = newEffectNode;
            //nodeStruct.outConnectionPoints = newEffectNode.GetOutConnectionPoints;

            //Add the node into collection in editor
            AllConnectableNodesInEditor.Add(newEffectNode);
            AllEffectsNodeInEditor.Add(newEffectNode.NodeID, new BaseEffectNodePair(newEffectNode, newEffectNode.GetOutConnectionPoints));
            return newEffectNode;
        }

        //These two functions are mainly used for creating and loading start node
        void CreateConnectableNode(Vector2 mousePosition, string nameOfNodeType, out ConnectableNode newBasicNode)
        {
            ConnectableNode basicNode = LEMDictionary.GetNodeObject(nameOfNodeType) as ConnectableNode;

            //Get the respective skin from the collection of nodeskin
            NodeSkinCollection nodeSkin = LEMStyleLibrary.WhiteBackgroundSkin;

            //Initialise the new node 
            basicNode.Initialise
                (mousePosition,
                nodeSkin,
                LEMStyleLibrary.ConnectionPointStyleNormal,
                OnClickInPoint,
                OnClickOutPoint,
                TryToAddNodeToSelectedCollection,
                TryToRemoveNodeFromSelectedCollection,
                LEMDictionary.GetNodeColour(nameOfNodeType)
                );

            basicNode.GenerateNodeID();

            //Add the node into collection in editor
            AllConnectableNodesInEditor.Add(basicNode);

            newBasicNode = basicNode;
        }

        void RecreateConnectableNode(Vector2 positionToSet, string nameOfNodeType, string idToSet, out ConnectableNode newlyCreatedNode)
        {
            ConnectableNode newNode = LEMDictionary.GetNodeObject(nameOfNodeType) as ConnectableNode;

            //Get the respective skin from the collection of nodeskin
            NodeSkinCollection nodeSkin = LEMStyleLibrary.WhiteBackgroundSkin;

            //Initialise the new node 
            newNode.Initialise
                (positionToSet,
                nodeSkin,
                LEMStyleLibrary.ConnectionPointStyleNormal,
                OnClickInPoint,
                OnClickOutPoint,
                TryToAddNodeToSelectedCollection,
                TryToRemoveNodeFromSelectedCollection,
                LEMDictionary.GetNodeColour(nameOfNodeType)
                );

            newNode.SetNodeID(idToSet);

            //Add the node into collection in editor
            AllConnectableNodesInEditor.Add(newNode);

            newlyCreatedNode = newNode;
        }

        public static GroupRectNode CreateGroupRectNode(Rect[] allSelectedNodesTotalRect, Node[] allSelectedNodes)
        {
            GroupRectNode groupRect;

            GroupRectNode.CalculateGroupRectPosition(allSelectedNodesTotalRect, out Vector2 startVector2Pos, out Vector2 endVector2Pos);

            //Size vector
            endVector2Pos.x = Mathf.Abs(endVector2Pos.x - startVector2Pos.x);
            endVector2Pos.y = Mathf.Abs(endVector2Pos.y - startVector2Pos.y);

            //Get the respective skin from the collection of nodeskin
            NodeSkinCollection nodeSkin = LEMStyleLibrary.WhiteBackgroundSkin;

            groupRect = new GroupRectNode();

            //Initialise the new node 
            groupRect.Initialise
                (startVector2Pos,
                endVector2Pos,
                allSelectedNodes,
                nodeSkin,
                Instance.TryToAddNodeToSelectedCollection,
                Instance.TryToRemoveNodeFromSelectedCollection,
                LEMStyleLibrary.CurrentGroupRectTopSkinColour
                );

            groupRect.GenerateNodeID();
            //Add the node into collection in editor
            AllGroupRectNodesInEditor.Add(groupRect);
            AllGroupRectsInEditorDictionary.Add(groupRect.NodeID, groupRect);

            return groupRect;
        }

        //Recreategroup if there is nestednodes in this group node
        public static GroupRectNode ReCreateGroupNode(string[] allNestedNodesIDs, string idToSet, string labelText)
        {
            //Get the respective skin from the collection of nodeskin
            NodeSkinCollection nodeSkin = LEMStyleLibrary.WhiteBackgroundSkin;

            Node[] allSelectedNodesWithNoGroups = new Node[allNestedNodesIDs.Length];
            Rect[] allNestedNodesRects = new Rect[allNestedNodesIDs.Length];

            string initials;

            for (int i = 0; i < allSelectedNodesWithNoGroups.Length; i++)
            {
                initials = GetInitials(allNestedNodesIDs[i]);

                if (initials == LEMDictionary.NodeIDs_Initials.k_StartNodeInitial)
                    allSelectedNodesWithNoGroups[i] = StartNode;
                else if (initials == LEMDictionary.NodeIDs_Initials.k_BaseEffectInital)
                    allSelectedNodesWithNoGroups[i] = AllEffectsNodeInEditor[allNestedNodesIDs[i]].effectNode;
                else if (initials == LEMDictionary.NodeIDs_Initials.k_GroupRectNodeInitial)
                    allSelectedNodesWithNoGroups[i] = AllGroupRectsInEditorDictionary[allNestedNodesIDs[i]];

                allNestedNodesRects[i] = allSelectedNodesWithNoGroups[i].m_TotalRect;
            }

            GroupRectNode.CalculateGroupRectPosition(allNestedNodesRects, out Vector2 spawnPos, out Vector2 endPos);

            endPos.x = Mathf.Abs(endPos.x - spawnPos.x);
            endPos.y = Mathf.Abs(endPos.y - spawnPos.y);


            GroupRectNode groupRect = new GroupRectNode();

            //Initialise the new node 
            groupRect.Initialise
                (spawnPos,
                endPos,
                allSelectedNodesWithNoGroups,
                nodeSkin,
                Instance.TryToAddNodeToSelectedCollection,
                Instance.TryToRemoveNodeFromSelectedCollection,
                LEMStyleLibrary.CurrentGroupRectTopSkinColour
                );

            groupRect.CommentLabel = labelText;
            groupRect.SetNodeID(idToSet);

            //Add the node into collection in editor
            AllGroupRectNodesInEditor.Add(groupRect);
            AllGroupRectsInEditorDictionary.Add(groupRect.NodeID, groupRect);
            return groupRect;
        }

        //Recreategroup if there is nestednodes in this group node
        public static GroupRectNode ReCreateGroupNode(Vector2 rectGroupPos, Vector2 rectGroupSize, string idToSet, string labelText)
        {
            //Get the respective skin from the collection of nodeskin
            NodeSkinCollection nodeSkin = LEMStyleLibrary.WhiteBackgroundSkin;
            GroupRectNode groupRect = new GroupRectNode();

            Node[] noNested = new Node[0];
            //Initialise the new node 
            groupRect.Initialise
                (rectGroupPos,
                rectGroupSize,
                noNested
                ,
                nodeSkin,
                Instance.TryToAddNodeToSelectedCollection,
                Instance.TryToRemoveNodeFromSelectedCollection,
                LEMStyleLibrary.CurrentGroupRectTopSkinColour
                );

            groupRect.CommentLabel = labelText;
            groupRect.SetNodeID(idToSet);

            //Add the node into collection in editor
            AllGroupRectNodesInEditor.Add(groupRect);
            AllGroupRectsInEditorDictionary.Add(groupRect.NodeID, groupRect);
            return groupRect;
        }

        #endregion

        #region General Functions 
        //Drags the window canvas (think like animator window)
        void OnDrag(Vector2 delta)
        {
            //Record the amount of drag there is changed 
            //Convert delta value for canvas dragging
            //delta /= ScaleFactor;
            m_AmountOfMouseDragThisUpdate = delta;

            //Convert once more for node drag (idk but its a magic number)
            delta /= ScaleFactor;

            //Proceess this first
            if (s_CurrentClickedNode != null)
                if (!s_CurrentClickedNode.IsGrouped)
                    s_CurrentClickedNode.Drag(delta);

            //Update all the node's positions as well
            for (int i = 0; i < AllConnectableNodesInEditor.Count; i++)
            {
                if (!AllConnectableNodesInEditor[i].IsGrouped)
                    AllConnectableNodesInEditor[i].Drag(delta);
            }

            for (int i = 0; i < AllGroupRectNodesInEditor.Count; i++)
            {
                if (!AllGroupRectNodesInEditor[i].IsGrouped)
                    AllGroupRectNodesInEditor[i].Drag(delta);
            }

            CommandInvoker.ProcessHandleDrag(delta);

        }

        void TryToRestichConnections(LEM_BaseEffect currentEffect)
        {
            if (!currentEffect.bm_NodeBaseData.HasAtLeastOneNextPointNode)
                return;

            //And if there is one next point node
            if (currentEffect.bm_NodeBaseData.HasOnlyOneNextPointNode)
            {
                if (!String.IsNullOrEmpty(currentEffect.bm_NodeBaseData.m_NextPointsIDs[0])
                    &&
                    !AllEffectsNodeInEditor[currentEffect.bm_NodeBaseData.m_NodeID].outConnectionPoints[0].IsConnected)
                {
                    CreateConnection(
                              AllEffectsNodeInEditor[currentEffect.bm_NodeBaseData.m_NextPointsIDs[0]].effectNode.m_InPoint,
                              AllEffectsNodeInEditor[currentEffect.bm_NodeBaseData.m_NodeID].effectNode.m_OutPoint
                              );
                }

            }
            else
            {
                //that means that this effect node is a special node which has multiple outputs
                //Restich for those nodes
                for (int n = 0; n < currentEffect.bm_NodeBaseData.m_NextPointsIDs.Length; n++)
                    //If the keys current next point id is not empty or null and the outpoint  isnt connected,
                    if (!String.IsNullOrEmpty(currentEffect.bm_NodeBaseData.m_NextPointsIDs[n])
                        &&
                        !AllEffectsNodeInEditor[currentEffect.bm_NodeBaseData.m_NodeID].outConnectionPoints[n].IsConnected)
                    {
                        CreateConnection(
                        AllEffectsNodeInEditor[currentEffect.bm_NodeBaseData.m_NextPointsIDs[n]].effectNode.m_InPoint,
                        AllEffectsNodeInEditor[currentEffect.bm_NodeBaseData.m_NodeID].outConnectionPoints[n]
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


                //Else check if current outpoint's node is still being connected to the inpoint node (this may be a case in multioutput nodes)
                if (AllConnectionsDictionary.ContainsKey(new Tuple<string, string>(m_SelectedInPoint.m_ParentNode.NodeID, m_SelectedOutPoint.m_ParentNode.NodeID)))
                {
                    //warn the user not to do that n reset the outpoint skin
                    Debug.LogWarning("Cant have a node with multiple outpoints to have shared inpoint!");
                    TrySetConnectionPointSkin(m_SelectedOutPoint, 0);
                }
                else
                {
                    CommandInvoker.InvokeCommand(new CreateConnectionCommand(m_SelectedInPoint.m_ParentNode.NodeID, m_SelectedOutPoint.m_ParentNode.NodeID, m_SelectedOutPoint.Index));
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

                //Else check if current outpoint's node is still being connected to the inpoint node (this may be a case in multioutput nodes)
                if (AllConnectionsDictionary.ContainsKey(new Tuple<string, string>(m_SelectedInPoint.m_ParentNode.NodeID, m_SelectedOutPoint.m_ParentNode.NodeID)))
                {
                    //warn the user not to do that n reset the outpoint skin
                    Debug.LogWarning("Cant have a node with multiple outpoints to have shared inpoint!");
                    TrySetConnectionPointSkin(m_SelectedOutPoint, 0);
                }
                else
                {
                    CommandInvoker.InvokeCommand(new CreateConnectionCommand(m_SelectedInPoint.m_ParentNode.NodeID, m_SelectedOutPoint.m_ParentNode.NodeID, m_SelectedOutPoint.Index));
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
        void OnClickRemoveNode(ConnectableNode nodeToRemove)
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
            int indexOfNodeToRemove = AllConnectableNodesInEditor.FindIndex(x => x.NodeID == nodeToRemove.NodeID);
            AllConnectableNodesInEditor.RemoveEfficiently(indexOfNodeToRemove);

            if (AllEffectsNodeInEditor.ContainsKey(nodeToRemove.NodeID))
                AllEffectsNodeInEditor.Remove(nodeToRemove.NodeID);
        }

        //Second form of remove node function where it uses lem_Baseeffects instead of nodes cause nodes' referrecnes arent the same during command invokaton
        void DeleteConnectableNode(NodeBaseData nB)
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
            int indexOfNodeToRemove = AllConnectableNodesInEditor.FindIndex(x => x.NodeID == nB.m_NodeID);
            AllConnectableNodesInEditor.RemoveEfficiently(indexOfNodeToRemove);

            if (AllEffectsNodeInEditor.ContainsKey(nB.m_NodeID))
                AllEffectsNodeInEditor.Remove(nB.m_NodeID);
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

        void SelectAllNodes()
        {
            AllSelectedNodes.Clear();

            for (int i = 0; i < m_AllConnectableNodesInEditor.Count; i++)
                m_AllConnectableNodesInEditor[i].SelectNode();

            for (int i = 0; i < m_AllGroupRectNodesInEditor.Count; i++)
                m_AllGroupRectNodesInEditor[i].SelectNode();
        }

        void GetNewClickedNode(Vector2 currMousePosition)
        {
            ResetSelectedNode();

            int clickedNodeIndex;
            //Allow clicking on the nodes which are at the top of the list 
            clickedNodeIndex = AllConnectableNodesInEditor.FindIndexFromLastIndex(x => x.m_TotalRect.Contains(currMousePosition));

            //If node was found
            if (clickedNodeIndex != -1)
            {
                s_CurrentClickedNode = AllConnectableNodesInEditor[clickedNodeIndex];
                AllConnectableNodesInEditor.RemoveEfficiently(clickedNodeIndex);
                return;
            }

            //Search for group rects after searching for nodes
            clickedNodeIndex = AllGroupRectNodesInEditor.FindIndex(x => x.m_TotalRect.Contains(currMousePosition));

            //If node was found
            if (clickedNodeIndex != -1)
            {
                s_CurrentClickedNode = AllGroupRectNodesInEditor[clickedNodeIndex];
                AllGroupRectNodesInEditor.RemoveEfficiently(clickedNodeIndex);
            }

        }

        void ResetSelectedNode()
        {
            if (s_CurrentClickedNode == null)
                return;

            //Return the current clicked node back to its list if there was one previously
            if (s_CurrentClickedNode.ID_Initial == LEMDictionary.NodeIDs_Initials.k_GroupRectNodeInitial)
                AllGroupRectNodesInEditor.Add(s_CurrentClickedNode);
            else
                AllConnectableNodesInEditor.Add(s_CurrentClickedNode);

            s_CurrentClickedNode = null;
        }

        #endregion

        #endregion


        #region Saving and Loading

        void DeleteEditorContainer()
        {
            UnityEngine.Object.DestroyImmediate(EditorEffectsContainer);
            //CurrentLE.CurrentlyBeingEdited = false;
        }

        //To be subscribed to the event which will be called before user presses "Play Button"
        void SaveBeforeEnterPlayMode(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode)
                return;

            if (Instance == null || CurrentLE == null)
                return;

            SaveToLinearEvent();

            //Save string path of current LE to editor pref
            //string sceneAssetBasePath = EditorSceneManager.GetActiveScene().path;
            string linearEventScenePath = CurrentLE.transform.GetGameObjectPath();

            //EditorPrefs.SetString("sceneAssetBasePath", sceneAssetBasePath);
            EditorPrefs.SetString(k_EditorPref_LinearEventKey, linearEventScenePath);

            //linearEventScenePath = EditorEffectsContainer.transform.GetGameObjectPath();
            //EditorPrefs.SetString(k_EditorPref_EditorEffectsContainerKey, linearEventScenePath);

            DeleteEditorContainer();
        }


        void TryToSaveLinearEvent()
        {

            switch (Settings.m_SaveSettings)
            {
                case NodeLEM_Settings.SaveSettings.DontSave:
                    return;

                case NodeLEM_Settings.SaveSettings.AlwaysSave:
                    SaveToLinearEvent();
                    break;

                case NodeLEM_Settings.SaveSettings.SaveWhenCommandChange:
                    if (m_EditorState == EDITORSTATE.LOADED)
                        SaveToLinearEvent();
                    break;

            }
        }

        //To be called when player presses "Save button" or when assembly reloads every time a script changes (when play mode is entered this will get called also but it doesnt save the values to the LE)
        void SaveToLinearEvent()
        {
            m_EditorState = EDITORSTATE.SAVING;

            //To get rid of all the annoying red errors the editor throws whenever it attempts to save dururing assembly reload
            if (Instance == null)
                return;

            LEM_BaseEffect[] lemEffects = new LEM_BaseEffect[AllEffectsNodeInEditor.Count];
            BaseEffectNode[] allEffectNodes = AllEffectsNodeInEditor.Values.Select(x => x.effectNode).ToArray();

            CurrentLE.ClearAllEffects();
            //HideFlags flag = CurrentLE.FlagToFollow = Settings.m_ShowMonoBehaviours ? HideFlags.None : HideFlags.HideInInspector;
            //HideFlags hideOrNot = Settings.m_ShowMonoBehaviours? HideFlags.NotEditable : HideFlags.HideInInspector;

            //This saves all events regardless of whether they are connected singularly, plurally or disconnected
            for (int i = 0; i < lemEffects.Length; i++)
            {
                lemEffects[i] = allEffectNodes[i].CompileToBaseEffect(CurrentLE.gameObject);
                lemEffects[i].hideFlags = HideFlags.HideInInspector;
            }

            if (AllGroupRectsInEditorDictionary.Count > 0)
            {
                GroupRectNodeBase[] allGroupRects = DeleteGroupRectNodeData.SortGroupRectNodeBasesForSaving(AllGroupRectsInEditorNodeIDs);
                CurrentLE.m_AllGroupRectNodes = allGroupRects;
            }

            //Save to serializable array of effects
            CurrentLE.m_AllEffects = lemEffects;

            //Save start and end node data
            CurrentLE.m_StartNodeData = StartNode.SaveNodeData();

            //Finished loading
            Repaint();

            //if (Settings.m_SaveSceneTogether)
            //    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

            EditorUtility.SetDirty(CurrentLE);
            //EditorSceneManager.MarkSceneDirty(CurrentLE.gameObject.scene);

            Debug.Log("Saved Linear Event File " + CurrentLE.name, CurrentLE);
            m_EditorState = EDITORSTATE.SAVED;
        }

        //Loads the previously editing linear event after exiting playmode
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

            //linearEventPath = EditorPrefs.GetString(k_EditorPref_EditorEffectsContainerKey);
            //EditorEffectsContainer = GameObject.Find(linearEventPath);

            NodeLEM_Editor.LoadNodeEditor(prevLE);
        }

        void LoadFromLinearEvent()
        {
            if (Instance == null || CurrentLE == null)
                return;

            m_EditorState = EDITORSTATE.LOADING;

            #region Loading Events from Dictionary
            //Dont do any thing if there is no effects in the dicitionary
            Dictionary<string, LEM_BaseEffect> allEffectsDictInLinearEvent = CurrentLE.GetAllEffectsDictionary;

            if (allEffectsDictInLinearEvent == null)
            {
                Repaint();
                return;
            }

            string[] allKeys = allEffectsDictInLinearEvent.Keys.ToArray();

            //Recreate all the nodes from the dictionary
            for (int i = 0; i < allKeys.Length; i++)
            {
                //Load the new node with saved node values values
                RecreateEffectNode(allEffectsDictInLinearEvent[allKeys[i]].bm_NodeBaseData.m_Position,
                     allEffectsDictInLinearEvent[allKeys[i]].bm_NodeEffectType,
                     allEffectsDictInLinearEvent[allKeys[i]].bm_NodeBaseData.m_NodeID).LoadFromBaseEffect(allEffectsDictInLinearEvent[allKeys[i]]);
            }

            #endregion

            //Do the same for start n end node only if they arent null (they likely wont because onenable runs first)
            //and that there are records of saving them
            if (StartNode != null && CurrentLE.m_StartNodeData.m_NodeID != string.Empty)
            {
                OnClickRemoveNode(StartNode);
                RecreateConnectableNode(CurrentLE.m_StartNodeData.m_Position, "StartNode", CurrentLE.m_StartNodeData.m_NodeID, out ConnectableNode startTempNode);
                StartNode = startTempNode;
            }

            //Recr8 group nodes
            for (int i = CurrentLE.m_AllGroupRectNodes.Length - 1; i > -1; i--)
            //for (int i = 0; i < s_CurrentLE.m_AllGroupRectNodes.Length; i++)
            {
                if (CurrentLE.m_AllGroupRectNodes[i].HasAtLeastOneNestedNode)
                    ReCreateGroupNode(
                        CurrentLE.m_AllGroupRectNodes[i].m_NestedNodeIDs,
                        CurrentLE.m_AllGroupRectNodes[i].m_NodeID,
                        CurrentLE.m_AllGroupRectNodes[i].m_LabelText);
                else
                    ReCreateGroupNode(
                        CurrentLE.m_AllGroupRectNodes[i].m_Position,
                        CurrentLE.m_AllGroupRectNodes[i].m_Size,
                        CurrentLE.m_AllGroupRectNodes[i].m_NodeID,
                        CurrentLE.m_AllGroupRectNodes[i].m_LabelText);
            }

            //Restitch their parent connections
            for (int i = 0; i < CurrentLE.m_AllGroupRectNodes.Length; i++)
            {
                if (!string.IsNullOrEmpty(CurrentLE.m_AllGroupRectNodes[i].m_ParentNodeID))
                    AllGroupRectsInEditorDictionary[CurrentLE.m_AllGroupRectNodes[i].m_NodeID].m_GroupedParent = AllGroupRectsInEditorDictionary[CurrentLE.m_AllGroupRectNodes[i].m_ParentNodeID];
            }


            #region Stitch Connections

            //Stitch dictionary's their connnection back
            for (int i = 0; i < allKeys.Length; i++)
            {
                //If this nodebase data doesnt even have one nextpoint node id, skip this loop
                if (!allEffectsDictInLinearEvent[allKeys[i]].bm_NodeBaseData.HasAtLeastOneNextPointNode)
                    continue;

                TryToRestichConnections(allEffectsDictInLinearEvent[allKeys[i]]);
            }

            //Dont stitch up start node if it isnt connected to at least one point
            if (CurrentLE.m_StartNodeData.HasAtLeastOneNextPointNode)
            {
                //Do the same for start and end nodes
                //if node has a m_NextPointNodeID and that the next node this node is assigned to doesnt have a connection on the outpoint,
                if (!String.IsNullOrEmpty(CurrentLE.m_StartNodeData.m_NextPointsIDs[0])
                    && !StartNode.m_OutPoint.IsConnected)
                {
                    //Else just find the next node from the dictionary of all effects node
                    CreateConnection(
                                     AllEffectsNodeInEditor[CurrentLE.m_StartNodeData.m_NextPointsIDs[0]].effectNode.m_InPoint,
                                     StartNode.m_OutPoint
                                    );
                }
            }
            #endregion

            Repaint();
            m_EditorState = EDITORSTATE.LOADED;
        }

        #endregion

        #region Node Related Static Functions
        public static LEM_BaseEffect GetNodeEffectFromID(string nodeID)
        {
            return AllEffectsNodeInEditor[nodeID].effectNode.CompileToBaseEffect(EditorEffectsContainer);
        }

        public static GroupRectNodeBase GetGroupRectDataFromID(string nodeID)
        {
            return AllGroupRectsInEditorDictionary[nodeID].SaveGroupRectNodedata();
        }


        public static void DeselectAllNodes()
        {
            while (AllSelectedNodes.Count > 0)
            {
                AllSelectedNodes[0].DeselectNode();
            }
        }


        #region Create Functions

        //Use this if you know exactly what nodes to connect
        //For now only used for saving and loading
        public static void CreateConnection(ConnectionPoint inPoint, ConnectionPoint outPoint)
        {
            //Add connection to dual key dictionary
            AllConnectionsDictionary.Add(
                new Tuple<string, string>(inPoint.m_ParentNode.NodeID, outPoint.m_ParentNode.NodeID),
                new Connection(inPoint, outPoint, Instance.OnClickRemoveConnection)
                );

            Instance.TrySetConnectionPoint(inPoint);
            Instance.TrySetConnectionPoint(outPoint);
        }

        //Use this if you have the ID of the nodes you wish to connect but dont know their identities
        public static void CreateConnection(string inPointNodeID, string outPointNodeID, int outPointIndex)
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
                new Connection(inPoint, outPoint, Instance.OnClickRemoveConnection)
                );

            Instance.TrySetConnectionPoint(inPoint);
            Instance.TrySetConnectionPoint(outPoint);
        }

        #endregion


        #region Delete Functions
        public static void DeleteGroupRects(GroupRectNodeBase[] groupRectNodes)
        {
            for (int i = 0; i < groupRectNodes.Length; i++)
            {
                DeleteGroupRect(groupRectNodes[i]);
            }
        }

        public static void DeleteGroupRect(GroupRectNodeBase groupRectNodes)
        {
            Node[] nestedNodes = new Node[groupRectNodes.m_NestedNodeIDs.Length];

            BaseEffectNodePair dummy1;
            GroupRectNode dummy2;

            for (int i = 0; i < nestedNodes.Length; i++)
            {
                //If this node is the start node,
                if (StartNode.NodeID == groupRectNodes.m_NestedNodeIDs[i])
                {
                    nestedNodes[i] = StartNode;
                    nestedNodes[i].m_GroupedParent = null;
                }
                else if (AllEffectsNodeInEditor.TryGetValue(groupRectNodes.m_NestedNodeIDs[i], out dummy1))
                {
                    nestedNodes[i] = dummy1.effectNode;
                    nestedNodes[i].m_GroupedParent = null;
                }
                else if (AllGroupRectsInEditorDictionary.TryGetValue(groupRectNodes.m_NestedNodeIDs[i], out dummy2))
                {
                    nestedNodes[i] = dummy2;
                    nestedNodes[i].m_GroupedParent = null;
                }

            }
            Instance.TryToRemoveNodeFromSelectedCollection(groupRectNodes.m_NodeID);

            //O(n) operation only, inother words same as list.Remove( )
            //Need nodeid to be checked cause Node references are lost during command invoker
            int indexOfNodeToRemove = AllGroupRectNodesInEditor.FindIndex(x => x.NodeID == groupRectNodes.m_NodeID);
            AllGroupRectNodesInEditor.RemoveEfficiently(indexOfNodeToRemove);

            //Parent of the group rect to be deleted must be updated before the grouprect node is completely removed
            if (AllGroupRectsInEditorDictionary[groupRectNodes.m_NodeID].IsGrouped)
            {
                dummy2 = AllGroupRectsInEditorDictionary[groupRectNodes.m_NodeID].m_GroupedParent;
                AllGroupRectsInEditorDictionary.Remove(groupRectNodes.m_NodeID);
                //Forcefully update
                dummy2.UpdateNestedNodes();
                return;
            }

            AllGroupRectsInEditorDictionary.Remove(groupRectNodes.m_NodeID);
        }

        public static void DeleteConnectableNodes(NodeBaseData[] nodesToBeDeleted)
        {
            for (int i = 0; i < nodesToBeDeleted.Length; i++)
                Instance.DeleteConnectableNode(nodesToBeDeleted[i]);
        }

        public static void TryToRemoveConnection(string inPointNodeID, string outPointNodeID)
        {
            if (AllConnectionsDictionary.TryGetValue(
               new Tuple<string, string>(inPointNodeID,
               outPointNodeID),
               out Connection connectionToRemove))
            {
                //Remove any connections that is connected to the node's outpoint
                Instance.OnClickRemoveConnection(connectionToRemove);
            }
        }

        #endregion

        #endregion

    }

}