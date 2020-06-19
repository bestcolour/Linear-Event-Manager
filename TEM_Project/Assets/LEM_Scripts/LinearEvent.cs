using System.Collections.Generic;
using UnityEngine;
using LEM_Effects.Extensions;
using LEM_Effects;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

#region General Rules
//General rules 
//Rule #1 : Do not place an PlayLinearEvent effect as the first effect to play in any LinearEvent
//Reason/Problem that had occured but is now fixed if you follow the rule: 
//Linear Event 1: 
// Node 1 -> Any kind of repeating update effect (eg. CurveAlphaToGraphic set to loop curve mode.)
// Node 2 -> Play LinearEvent effect where the target Linear Event is Linear Event 2

//Linear Event 2:
// Node 1 -> AwaitKeyCodeInput
// Node 2 -> StopLinearEvent
// Node 3 -> AwaitKeyCodeInput
// Node 4 -> PlayLinearEvent (Target is Linear Event 1)

//Order of execution of events
//Playing LE 1 will play LE 2
//Press key (make sure its GetKeyDown)
//Stops and reset LE 1
//Press key agn
//Plays LE 1 which plays LE 2 which plays LE 1 by adding the AwaitKeyCodeInput effect into the running effect list 
//This will make the HasNoEffectsPlaying == false and hence disallow a continuous loop.




#endregion


[
#if UNITY_EDITOR
        ExecuteInEditMode,
#endif
        System.Serializable]
public class LinearEvent : MonoBehaviour
{

    #region Cached Values
    //NOTE ENSURE THAT THIS FIELD IS ALWAYS THE FIRST IN THIS SCRIPT. IF YOU WISH TO CHANGE IT, PLEASE TAKE A LOOK AT LinearEvent_Inspector.cs DrawInspector() METHOD FOR POTENTIAL ISSUES
    public LEM_BaseEffect[] m_AllEffects = default;

#if UNITY_EDITOR
    [Header("DESCRIPTION"), Tooltip("This is purely for labelling purposes. It will not be added as a variable in the final build."), TextArea(2, 10)] public string m_LinearDescription = default;
#endif

    [Header("Settings"), SerializeField, Tooltip("Should Linear Event initalize its effect dictionary on awake?")]
    bool m_InitializeOnAwake = true;

    [SerializeField, Tooltip("What is the expected amount of effects to run at one time? Increase this if you feel like there will be many effects running over a short period of time")]
    int m_UpdateEffectCapacity = 5;

    [SerializeField, Tooltip("How many effects should be executed in a single frame?")]
    int m_InstantEffectCapacity = 10;

    [HideInInspector]
    public NodeBaseData m_StartNodeData = default;


    #region Editor Variables and Methods

#if UNITY_EDITOR


    [HideInInspector]
    public GroupRectNodeBase[] m_AllGroupRectNodes = default;

    [SerializeField, Header("Debug Settings")] bool m_ShowEffects = false;
    HideFlags m_CurrentFlags => m_ShowEffects ? HideFlags.NotEditable : HideFlags.HideInInspector;

    /// <summary>
    /// Editor Method: Clears all effects currently on the Linear Event
    /// </summary>
    public void EditorMethod_ClearAllEffects()
    {
        if (m_AllEffects == null || m_AllEffects.Length == 0)
            return;

        for (int i = 0; i < m_AllEffects.Length; i++)
        {
            UnityEngine.Object.DestroyImmediate(m_AllEffects[i], true);
        }
        m_AllEffects = new LEM_BaseEffect[0];
        m_AllGroupRectNodes = new GroupRectNodeBase[0];

        EditorUtility.SetDirty(this);
    }

    private void OnEnable()
    {
        Selection.selectionChanged += EditorMethod_TryToHideEffectComponents;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= EditorMethod_TryToHideEffectComponents;
    }

    void EditorMethod_TryToHideEffectComponents()
    {
        if (Selection.activeGameObject != this.gameObject || m_AllEffects == null || m_AllEffects.Length == 0)
            return;

        //Change HideFlags to NotEditable to debug
        if (m_AllEffects[0].hideFlags == m_CurrentFlags)
            return;

        EditorMethod_HideEffectComponents();
    }

    void EditorMethod_HideEffectComponents()
    {
        for (int i = 0; i < m_AllEffects.Length; i++)
        {
            //Change HideFlags to NotEditable to debug
            m_AllEffects[i].hideFlags = m_CurrentFlags;
        }
    }

    /// <summary>
    /// Editor Method: Duplicates the LinearEvent as well as all its effects to a targeted GameObject and then destroy the original copy and set of effects
    /// </summary>
    /// <param name="movingTo"></param>
    public void EditorMethod_MoveLinearEventComponent(GameObject movingTo)
    {
        EditorMethod_DuplicateLinearEvent(movingTo);

        EditorMethod_ClearAllEffects();
        UnityEngine.Object.DestroyImmediate(this, true);
    }

    /// <summary>
    /// Editor Method: Duplicates the LinearEvent
    /// </summary>
    public void EditorMethod_DuplicateLinearEvent(GameObject duplicatingTo)
    {
        LinearEvent duplicate = duplicatingTo.AddComponent<LinearEvent>();

        //Copy arrays
        duplicate.m_AllEffects = new LEM_BaseEffect[m_AllEffects.Length];
        for (int i = 0; i < duplicate.m_AllEffects.Length; i++)
        {
            duplicate.m_AllEffects[i] = m_AllEffects[i].CloneMonoBehaviour(duplicatingTo);
        }

        duplicate.m_AllGroupRectNodes = new GroupRectNodeBase[m_AllGroupRectNodes.Length];
        m_AllGroupRectNodes.CopyTo(duplicate.m_AllGroupRectNodes, 0);

        //Copy Cached values
        duplicate.m_InitializeOnAwake = m_InitializeOnAwake;
        duplicate.m_UpdateEffectCapacity = m_UpdateEffectCapacity;
        duplicate.m_InstantEffectCapacity = m_InstantEffectCapacity;
        duplicate.m_StartNodeData = m_StartNodeData;
        duplicate.m_LinearDescription = m_LinearDescription;

        duplicate.EditorMethod_HideEffectComponents();

        RefreshReferences(duplicate);

    }

    //Duplicating and Moving Linear Event will cause references to be lost due to 
    //copying and deleting(for eg. the TargetLinear Events which are either dragged and dropped or by default set to the current linear event in which the effect is in)
    //Call this method to refresh those connections
    void RefreshReferences(LinearEvent le)
    {
        if (le.m_AllEffects == null)
            return;

        for (int i = 0; i < le.m_AllEffects.Length; i++)
        {
            le.m_AllEffects[i].OnRefreshReferenceToParentLinearEvent(le);
        }
    }

#endif
    #endregion


    #endregion

    #region RunTime Values

    //Runtime values
    public Dictionary<string, LEM_BaseEffect> GetAllEffectsDictionary
    {
        get
        {
            if (m_AllEffects == null || m_AllEffects.Length <= 0)
            {
                return null;
            }

            Dictionary<string, LEM_BaseEffect> allEffectsDictionary = new Dictionary<string, LEM_BaseEffect>(m_AllEffects.Length);

            for (int i = 0; i < m_AllEffects.Length; i++)
            {
                //Debug.Log("ELement " + i);
                allEffectsDictionary.Add(m_AllEffects[i].bm_NodeBaseData.m_NodeID, m_AllEffects[i]);
            }

            return allEffectsDictionary;
        }
    }

    #region Halt Variables

    float m_DelayBeforeNextEffect = 0f;
    public float AddDelayBeforeNextEffect { set { m_DelayBeforeNextEffect += value; } }
    public float SetDelayBeforeNextEffect { set { m_DelayBeforeNextEffect = value; } }

    public bool PauseLinearEvent { set; private get; } = true;

    //Using ushort here cause i highly doubt u need a range of 4 billion int
    ushort m_NumOfAwaitingInput = 0;

    public void DeductNumberOfAwaitingInput() { m_NumOfAwaitingInput--; }
    public void AddNumberOfAwaitingInput() { m_NumOfAwaitingInput++; }

    #endregion

    LEM_BaseEffect m_PreviousEffectPlayed = default;

    public bool HasNoEffectsPlaying => m_UpdateCycle.Count == 0 && m_FixedUpdateCycle.Count == 0 && m_LateUpdateCycle.Count == 0;

    //Update cycles
    List<LEM_BaseEffect> m_UpdateCycle = default;

    //[SerializeField, ReadOnly]
    List<LEM_BaseEffect> m_FixedUpdateCycle = default, m_LateUpdateCycle = default;

    public Dictionary<string, LEM_BaseEffect> EffectsDictionary { get; private set; } = null;
    bool IsInitialized => EffectsDictionary != null && EffectsDictionary.Count > 0;
    #endregion

    #region Initialization Methods

    void Awake()
    {
        //This will prevent the initialization of LE all the time (if bool == true) during editor mode
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;
#endif

        if (m_InitializeOnAwake)
        {
            InitializeLinearEvent();
        }
    }

    /// <summary>
    /// Initialses the Linear Event to get its Effects ready. Please call this during loading of assets or before the game starts.
    /// </summary>
    public void InitializeLinearEvent()
    {
        if (IsInitialized)
            return;

        m_UpdateCycle = new List<LEM_BaseEffect>(m_UpdateEffectCapacity);
        m_FixedUpdateCycle = new List<LEM_BaseEffect>(m_UpdateEffectCapacity);
        m_LateUpdateCycle = new List<LEM_BaseEffect>(m_UpdateEffectCapacity);

        EffectsDictionary = GetAllEffectsDictionary;
        PauseLinearEvent = false;
    }
    #endregion

    /// <summary>
    /// Plays the Linear Event provided that it has been initialized.
    /// </summary>
    public void PlayLinearEvent()
    {
#if UNITY_EDITOR
        Debug.Assert(IsInitialized, "You are trying to play the Linear Event " + m_LinearDescription + " that has not been initialized or have 0 Effects in it!", this);
        Debug.Assert(m_StartNodeData.HasAtLeastOneNextPointNode, "Linear Event " + m_LinearDescription + " does not have its Start Node connected to any effects!", this);
#endif

        //Checks if the Linear Event is currently being played
        if (!HasNoEffectsPlaying)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Cannot play Linear Event " + m_LinearDescription + " which is currently playing!", this);
#endif
            return;
        }

        //Note: This needs to be 1 to prevent interference with looping
        //View #1 rule in General Rules at the top for more information/reasoning why 1 is placed here
        LoadStartingEffect(1, m_StartNodeData.m_NextPointsIDs[0]);
        LinearEventsManager.Instance.RunningLinearEvents.Add(this);
    }

    #region Public Supporting Methods
    public void ClearAllUpdateLists()
    {
        while (m_UpdateCycle.Count > 0)
        {
            m_UpdateCycle[0].OnEndEffect();
            m_UpdateCycle.RemoveEfficiently(0);
        }

        while (m_FixedUpdateCycle.Count > 0)
        {
            m_FixedUpdateCycle[0].OnEndEffect();
            m_FixedUpdateCycle.RemoveEfficiently(0);
        }

        while (m_LateUpdateCycle.Count > 0)
        {
            m_LateUpdateCycle[0].OnEndEffect();
            m_LateUpdateCycle.RemoveEfficiently(0);
        }
    }
    #endregion


    #region Main Update Methods

    public void CycleUpdate()
    {
        if (PauseLinearEvent)
            return;

        for (int i = 0; i < m_UpdateCycle.Count; i++)
        {
            if (m_UpdateCycle[i].OnUpdateEffect(Time.deltaTime))
            {
                m_UpdateCycle[i].OnEndEffect();
                m_UpdateCycle.RemoveEfficiently(i);
                i--;
            }
        }
    }


    public void CycleFixedUpdate()
    {
        if (PauseLinearEvent)
            return;

        for (int i = 0; i < m_FixedUpdateCycle.Count; i++)
        {
            if (m_FixedUpdateCycle[i].OnUpdateEffect(Time.fixedDeltaTime))
            {
                m_FixedUpdateCycle[i].OnEndEffect();
                m_FixedUpdateCycle.RemoveEfficiently(i);
                i--;
            }
        }
    }

    //Returns true when LinearEvent is out of effects to load
    public bool CycleLateUpdate()
    {
        if (PauseLinearEvent)
            return false;

        for (int i = 0; i < m_LateUpdateCycle.Count; i++)
        {
            if (m_LateUpdateCycle[i].OnUpdateEffect(Time.deltaTime))
            {
                m_LateUpdateCycle[i].OnEndEffect();
                m_LateUpdateCycle.RemoveEfficiently(i);
                i--;
            }
        }


        //Dont load new effect if there is a delay
        if (m_DelayBeforeNextEffect > 0)
        {
            m_DelayBeforeNextEffect -= Time.deltaTime;
            return false;
        }

        //Check if there is any more effects that this LE can possibly load if yes return false if no return true;
        return ListenToLoadNextEffect() && HasNoEffectsPlaying;
    }

    #endregion

    #region Private Supporting Methods


    //Checks if this LinearEvent needs a click or some trigger b4 it can load next effect
    //Returns true if there is no more possible effects that can be loaded
    bool ListenToLoadNextEffect()
    {
        if (m_NumOfAwaitingInput > 0)
            return false;

        //If LE isnt finished loading all the effects
        else if (!TryLoadNextEffect(m_InstantEffectCapacity))
        {
            return false;
        }
        else
            return true;
    }

    //Returns true when there is still effects to load
    //Linear Events Manager will not handle any loading of next effects
    //the LE will handle that
    bool TryLoadNextEffect(int maxEffectsPerFrame)
    {
        //if the next nodeid or the previous effect played is null, that means thats the end of the linear event. No more events to load.
        //we need to check if prev node atleast hv one nextpoint node due to nodes that can give multiple outcomes
        if (m_PreviousEffectPlayed == null || !m_PreviousEffectPlayed.bm_NodeBaseData.HasAtLeastOneNextPointNode || !EffectsDictionary.TryGetValue(m_PreviousEffectPlayed.GetNextNodeID(), out m_PreviousEffectPlayed))
            return true;


        maxEffectsPerFrame--;

        m_PreviousEffectPlayed.OnInitialiseEffect();

        switch (m_PreviousEffectPlayed.FunctionType)
        {
            //Effect type that are instantaneously settled 
            case LEM_BaseEffect.EffectFunctionType.InstantEffect:
                break;

            //Effect type that are supposed to be updated everyframe
            case LEM_BaseEffect.EffectFunctionType.UpdateEffect:
                AddEffectToCycle(m_PreviousEffectPlayed);
                break;

            //If effect type is a halt effect type (delay time, listen for trigger, listen for mouse etc.
            case LEM_BaseEffect.EffectFunctionType.InstantHaltEffect:
                return false;

            case LEM_BaseEffect.EffectFunctionType.UpdateHaltEffect:
                AddEffectToCycle(m_PreviousEffectPlayed);
                return false;


        }

        //If there is still effects to spare to tryload, load it and if that is the straw that breaks the camel's back, then this entire shit will return true
        if (maxEffectsPerFrame > 0)
            return TryLoadNextEffect(maxEffectsPerFrame);

        //Else lets be conservative n return a false
        return false;
    }

    void LoadStartingEffect(int maxEffectsPerFrame, string nextEffect)
    {
        //Load next effect
        if (!EffectsDictionary.TryGetValue(nextEffect, out m_PreviousEffectPlayed))
        {
#if UNITY_EDITOR
            Debug.LogWarning("Linear Event " + m_LinearDescription + " does not contain the ID " + nextEffect, this);
#endif
            return;
        }



#if UNITY_EDITOR
        Type firstEffectPlayed = m_PreviousEffectPlayed.GetType();
        if (firstEffectPlayed == typeof(PlayLinearEvent) || firstEffectPlayed == typeof(PlayLinearEvents) || firstEffectPlayed == typeof(PlayRandomLinearEvent) || firstEffectPlayed == typeof(PlayBiasedLinearEvent))
        {
            Debug.LogWarning("Warning! The starting effect should not be a play linear event effect type to prevent interference in looping of Linear Event! Check Rule #1 in General Rules in LinearEvent.cs (at the top)");
        }

#endif


        m_PreviousEffectPlayed.OnInitialiseEffect();
        maxEffectsPerFrame--;

        switch (m_PreviousEffectPlayed.FunctionType)
        {
            //Effect type that are instantaneously settled 
            case LEM_BaseEffect.EffectFunctionType.InstantEffect:
                break;

            //Effect type that are supposed to be updated everyframe
            case LEM_BaseEffect.EffectFunctionType.UpdateEffect:
                AddEffectToCycle(m_PreviousEffectPlayed);
                break;

            //If effect type is a halt effect type (delay time, listen for trigger, listen for mouse etc.
            case LEM_BaseEffect.EffectFunctionType.InstantHaltEffect:
                return;

            case LEM_BaseEffect.EffectFunctionType.UpdateHaltEffect:
                AddEffectToCycle(m_PreviousEffectPlayed);
                return;
        }

        if (maxEffectsPerFrame > 0/* && !HasAnymoreEffectsToLoad*/)
            TryLoadNextEffect(maxEffectsPerFrame);

    }


    void AddEffectToCycle(LEM_BaseEffect effectToAdd)
    {
        switch (effectToAdd.bm_UpdateCycle)
        {
            case UpdateCycle.Update:
                m_UpdateCycle.Add(effectToAdd);
                break;

            case UpdateCycle.FixedUpdate:
                m_FixedUpdateCycle.Add(effectToAdd);
                break;

            case UpdateCycle.LateUpdate:
                m_LateUpdateCycle.Add(effectToAdd);
                break;

            default:
                m_UpdateCycle.Add(effectToAdd);
                break;

        }
    }


    #endregion



}



