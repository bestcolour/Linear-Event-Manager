using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LEM_Effects;

/// <summary>
/// © 2020 Lee Kee Shen All Rights Reserved
/// </summary>

public class LinearEventsManager : MonoBehaviour
{
    public static LinearEventsManager Instance;

    //Dictionary<LinearEvent, Dictionary<string, LEM_BaseEffect>> m_AllLinearEventsEffectsDictionary = new Dictionary<LinearEvent, Dictionary<string, LEM_BaseEffect>>();
    //public static Dictionary<LinearEvent, Dictionary<string, LEM_BaseEffect>> AllLinearEventsEffectsDictionary => Instance.m_AllLinearEventsEffectsDictionary;

    #region Linear Events Settings
    [Header("Events to Play"), SerializeField, Tooltip("Should LinearEventManager find all LinearEvents in the scene automatically on Initialisation?")] 
    bool m_AutoFindLinearEvents = false;

    [SerializeField, Tooltip("This array is supposed to represent all the Linear Events in the scene."), ConditionalReadOnly("m_AutoFindLinearEvents", m_ConditionToMeet = false)]
    LinearEvent[] m_AllLinearEventsInScene = default;
    public static LinearEvent[] AllLinearEventsInScene => Instance.m_AllLinearEventsInScene;

    //The linear events currently running
    [Tooltip("Is used as the first events to play. Is also used to determine what LinearEvents are currently playing."), SerializeField]
    List<LinearEvent> m_RunningLinearEvents = new List<LinearEvent>();
    public List<LinearEvent> RunningLinearEvents => m_RunningLinearEvents;

    [Header("Initialisation Settings")]
    [Tooltip("Should the LEM Manager initialise itself on Awake or let other scripts initialise it?")]
    [SerializeField] bool m_InitialiseOnAwake = default;

    [Tooltip("Should the LEM Manager start playing an event on Awake after self-initialisation?")]
    [SerializeField] bool m_PlayOnAwake = default;

    //[Tooltip("Is used as the first event to play. Leave empty if not needed. Is also used to show the current event playing."), SerializeField]
    //LinearEvent m_PlayingEvent = default;
    //public LinearEvent CurrentEvent { set { m_PlayingEvent = value; } }

    [Tooltip("What is the expected amount of effects to run at one time? Increase this if you feel like there will be many effects running over a short period of time"), SerializeField]
    int m_StartingUpdateEffectCapacity = 5;
    public static int StartingUpdateEffectCapacity => Instance.m_StartingUpdateEffectCapacity;

    [SerializeField, Tooltip("How many effects should be executed in a single frame?")]
    int m_InstantEffectCapacity = 10;
    public static int InstantEffectCapacity => Instance.m_StartingUpdateEffectCapacity;
    #endregion


    //Stop conditions
    [ReadOnly, Header("RunTime Checks"), Space(15), SerializeField]
    bool m_IsInitialised = false;

    [ReadOnly] bool m_PauseAllRunningLinearEvents = true;
    public bool PauseAllRunningLinearEvents { set { m_PauseAllRunningLinearEvents = value; } }

    //[ReadOnly, SerializeField]
    //LEM_BaseEffect m_PreviousEffectPlayed = null;

    //[ReadOnly, SerializeField]
    //float m_DelayBeforeNextEffect = default;
    //public float TimeToAddToDelay { set { m_DelayBeforeNextEffect += value; } }

    //[ReadOnly, SerializeField]      //Listening for trigger is just a test for many other things to come can remove if you havent thought things thru for trigger

    //bool m_ListeningForClick = default, m_ListeningForTrigger = default;
    //public bool ListeningForClick { set { m_ListeningForClick = value; } }
    //public bool ListeningForTrigger { set { m_ListeningForTrigger = value; } }

    //[SerializeField, ReadOnly, Header("RunTime Cycles (Do not touch)"), Space(15)]
    //List<LEM_BaseEffect> m_UpdateCycle = new List<LEM_BaseEffect>();

    //[SerializeField, ReadOnly]
    //List<LEM_BaseEffect> m_FixedUpdateCycle = new List<LEM_BaseEffect>(), m_LateUpdateCycle = new List<LEM_BaseEffect>();

    


    //Call this if you dont want the manager to be initialised on awake
    public void Initialise()
    {
        Instance = this;
        InitialiseAllLinearEvents();
    }

    void InitialiseAllLinearEvents()
    {
        //Change this to getcomponentsfromchildren later
        if (m_AutoFindLinearEvents)
            m_AllLinearEventsInScene = FindObjectsOfType<LinearEvent>();

        //Check if there is any linear event in the scene
#if UNITY_EDITOR
        if (m_AllLinearEventsInScene == null || m_AllLinearEventsInScene.Length <= 0)
        {
            Debug.LogWarning("There is no Linear Events in this scene!", this);
            return;
        }
#endif

        for (int i = 0; i < m_AllLinearEventsInScene.Length; i++)
        {
            //Check if linearevent is null and that if its LEM_Effects array is null or has a length <= 0
#if UNITY_EDITOR
            Debug.Assert(m_AllLinearEventsInScene[i] != null, "There is a null Linear Event in AllLinearEventsInScene's array at element " + i, m_AllLinearEventsInScene[i]);

            if (m_AllLinearEventsInScene[i].m_AllEffects == null && m_AllLinearEventsInScene[i].m_AllEffects.Length <= 0)
            {
                Debug.LogWarning("Linear Event " + m_AllLinearEventsInScene[i].name + " does not have any effects on it. AllLinearEventsInScene element: " + i,
                    m_AllLinearEventsInScene[i]);
            }
#endif
            m_AllLinearEventsInScene[i].m_LinearEventIndex = i;
            m_AllLinearEventsInScene[i].InitialiseLinearEvent();
            //m_AllLinearEventsEffectsDictionary.Add(m_AllLinearEventsInScene[i], m_AllLinearEventsInScene[i].GetAllEffectsDictionary);
        }

        //m_UpdateCycle = new List<LEM_BaseEffect>(m_StartingUpdateEffectCapacity);
        //m_FixedUpdateCycle = new List<LEM_BaseEffect>(m_StartingUpdateEffectCapacity);
        //m_LateUpdateCycle = new List<LEM_BaseEffect>(m_StartingUpdateEffectCapacity);

        m_IsInitialised = true;
        m_PauseAllRunningLinearEvents = false;
    }

    void Awake()
    {
#if UNITY_EDITOR
        if (m_PlayOnAwake && !m_InitialiseOnAwake)
        {
            Debug.LogWarning("LinearEventManager will not play on Awake because InitialiseOnAwake is false", this);
        }
#endif

        if (m_InitialiseOnAwake)
        {
            Initialise();

            if (m_PlayOnAwake)
            {

                for (int i = 0; i < m_RunningLinearEvents.Count; i++)
                {
#if UNITY_EDITOR
                    Debug.Assert(m_RunningLinearEvents[i] != null, "There is no Playing Event to play OnAwake!", this);
                    Debug.Assert(m_RunningLinearEvents[i].m_StartNodeData.HasAtLeastOneNextPointNode, "The Start Node of " + m_RunningLinearEvents[i].name + " has not been connected to any effects", m_RunningLinearEvents[i]);
#endif
                    m_RunningLinearEvents[i].OnStartPlayingLinearEvent();
                }
                //LoadNextEffect(m_InstantEffectCapacity, m_PlayingEvent.m_StartNodeData.m_NextPointsIDs[0]);
            }

        }


    }

  

    //Update loop will be where all the effects will be called and then removed if 
    //their effects are done
    void Update()
    {
        if (!m_IsInitialised || m_PauseAllRunningLinearEvents)
            return;

        //for (int i = 0; i < m_UpdateCycle.Count; i++)
        //{
        //    if (m_UpdateCycle[i].UpdateEffect())
        //    {
        //        m_UpdateCycle.RemoveEfficiently(i);
        //        i--;
        //    }
        //}

        //No removal of linear event from the list will be done in either update or fixed update, rather it will be done in LateUpdate
        for (int i = 0; i < m_RunningLinearEvents.Count; i++)
        {
            m_RunningLinearEvents[i].CycleUpdate();
        }

        ////Dont load new effect if there is a delay
        //if (m_DelayBeforeNextEffect > 0)
        //{
        //    m_DelayBeforeNextEffect -= Time.deltaTime;
        //    return;
        //}

        //if (m_PlayingEvent)
        //{
        //    ListenToLoadNextEffect();
        //}

    }

    void FixedUpdate()
    {
        if (!m_IsInitialised || m_PauseAllRunningLinearEvents)
            return;

        //for (int i = 0; i < m_FixedUpdateCycle.Count; i++)
        //{
        //    if (m_FixedUpdateCycle[i].UpdateEffect())
        //    {
        //        m_FixedUpdateCycle.RemoveEfficiently(i);
        //        i--;
        //    }
        //}

        //No removal of linear event from the list will be done in either update or fixed update, rather it will be done in LateUpdate
        for (int i = 0; i < m_RunningLinearEvents.Count; i++)
        {
            m_RunningLinearEvents[i].CycleFixedUpdate();
        }

    }

    void LateUpdate()
    {
        if (!m_IsInitialised || m_PauseAllRunningLinearEvents)
            return;

        //for (int i = 0; i < m_LateUpdateCycle.Count; i++)
        //{
        //    if (m_LateUpdateCycle[i].UpdateEffect())
        //    {
        //        m_LateUpdateCycle.RemoveEfficiently(i);
        //        i--;
        //    }
        //}

        //No removal of linear event from the list will be done in either update or fixed update, rather it will be done in LateUpdate
        for (int i = 0; i < m_RunningLinearEvents.Count; i++)
        {
            //if linear events hv ran out of effects to load
            if (m_RunningLinearEvents[i].CycleLateUpdate())
            {
                m_RunningLinearEvents.RemoveEfficiently(i);
                i--;
            }
        }

    }

    //void AddEffectToCycle(LEM_BaseEffect effectToAdd)
    //{
    //    switch (effectToAdd.m_UpdateCycle)
    //    {
    //        case UpdateCycle.Update:
    //            m_UpdateCycle.Add(effectToAdd);
    //            break;

    //        case UpdateCycle.FixedUpdate:
    //            m_FixedUpdateCycle.Add(effectToAdd);
    //            break;

    //        case UpdateCycle.LateUpdate:
    //            m_LateUpdateCycle.Add(effectToAdd);
    //            break;

    //        default:
    //            m_UpdateCycle.Add(effectToAdd);
    //            break;

    //    }
    //}

    //void ListenToLoadNextEffect()
    //{
    //    //If manager is listening for a click
    //    if (m_ListeningForClick)
    //        return;

    //    if (m_ListeningForTrigger)
    //        return;

    //    LoadNextEffect(m_InstantEffectCapacity);

    //}

    ////Linear Events Manager will not handle any loading of next effects
    ////the LE will handle that
    //void LoadNextEffect(int maxEffectsPerFrame)
    //{
    //    //Stop loading effect if there is no next effect
    //    if (!m_PreviousEffectPlayed.m_NodeBaseData.HasAtLeastOneNextPointNode)
    //        return;

    //    //Record next effect
    //    m_PreviousEffectPlayed = m_PlayingEvent.m_EffectsDictionary[m_PreviousEffectPlayed.GetNextNodeID()];

    //    maxEffectsPerFrame--;

    //    m_PreviousEffectPlayed.Initialise();

    //    switch (m_PreviousEffectPlayed.FunctionType)
    //    {
    //        //Effect type that are instantaneously settled 
    //        case LEM_BaseEffect.EffectFunctionType.InstantEffect:
    //            break;

    //        //Effect type that are supposed to be updated everyframe
    //        case LEM_BaseEffect.EffectFunctionType.UpdateEffect:
    //            AddEffectToCycle(m_PreviousEffectPlayed);
    //            break;

    //        //If effect type is a halt effect type (delay time, listen for trigger, listen for mouse etc.
    //        case LEM_BaseEffect.EffectFunctionType.HaltEffect:
    //            return;
    //    }

    //    if (maxEffectsPerFrame > 0)
    //        LoadNextEffect(maxEffectsPerFrame);
    //}

    //void LoadNextEffect(int maxEffectsPerFrame, string nextEffect)
    //{
    //    //Load next effect
    //    if (!m_PlayingEvent.m_EffectsDictionary.TryGetValue(nextEffect, out m_PreviousEffectPlayed))
    //        return;

    //    m_PreviousEffectPlayed.Initialise();
    //    maxEffectsPerFrame--;

    //    switch (m_PreviousEffectPlayed.FunctionType)
    //    {
    //        //Effect type that are instantaneously settled 
    //        case LEM_BaseEffect.EffectFunctionType.InstantEffect:
    //            break;

    //        //Effect type that are supposed to be updated everyframe
    //        case LEM_BaseEffect.EffectFunctionType.UpdateEffect:
    //            AddEffectToCycle(m_PreviousEffectPlayed);
    //            break;

    //        //If effect type is a halt effect type (delay time, listen for trigger, listen for mouse etc.
    //        case LEM_BaseEffect.EffectFunctionType.HaltEffect:
    //            return;
    //    }

    //    if (maxEffectsPerFrame > 0)
    //        LoadNextEffect(maxEffectsPerFrame);


    //}

}
