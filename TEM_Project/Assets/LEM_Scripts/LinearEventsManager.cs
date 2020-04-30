using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LEM_Effects;

/// <summary>
/// © 2020 Lee Kee Shen All Rights Reserved
/// </summary>

public class LinearEventsManager : MonoBehaviour
{
    public static LinearEventsManager Instance;

    Dictionary<LinearEvent, Dictionary<string, LEM_BaseEffect>> m_AllLinearEventsEffectsDictionary = new Dictionary<LinearEvent, Dictionary<string, LEM_BaseEffect>>();

    #region Linear Events
    [Header("Initialisation Settings")]
    [SerializeField, Tooltip("This array is supposed to represent all the Linear Events in the scene.")]
    LinearEvent[] m_AllLinearEventsInScene = default;

    [Tooltip("Should the LEM Manager initialise itself on Awake or let other scripts initialise it?")]
    [SerializeField] bool m_InitialiseOnAwake = default;

    [Tooltip("Should the LEM Manager start playing an event on Awake after self-initialisation?")]
    [SerializeField] bool m_PlayOnAwake = default;

    [Tooltip("Is used as the first event to play. Leave empty if not needed. Is also used to show the current event playing."), SerializeField]
    LinearEvent m_PlayingEvent = default;
    public LinearEvent CurrentEvent { set { m_PlayingEvent = value; } }

    [Tooltip("What is the expected amount of effects to run at one time? Increase this if you feel like there will be many effects running over a short period of time"), SerializeField]
    int m_StartingUpdateEffectCapacity = 5;

    [SerializeField, Tooltip("How many effects should be executed in a single frame?")]
    int m_InstantEffectCapacity = 10;
    #endregion


    //Stop conditions
    [ReadOnly, Header("RunTime Checks"), Space(15), SerializeField]
    bool m_IsInitialised = false;

    [ReadOnly] bool m_isEffectsPaused = true;
    public bool PauseEffects { set { m_isEffectsPaused = value; } }

    [ReadOnly, SerializeField]
    LEM_BaseEffect m_PreviousEffectPlayed = null;

    [ReadOnly, SerializeField]
    float m_DelayBeforeNextEffect = default;
    public float TimeToAddToDelay { set { m_DelayBeforeNextEffect += value; } }

    [ReadOnly, SerializeField]
    bool m_ListeningForClick = default, m_ListeningForTrigger = default;
    public bool ListeningForClick { set { m_ListeningForClick = value; } }
    public bool ListeningForTrigger { set { m_ListeningForTrigger = value; } }

    [SerializeField, ReadOnly, Header("RunTime Cycles (Do not touch)"), Space(15)]
    List<LEM_BaseEffect> m_UpdateCycle = new List<LEM_BaseEffect>();

    [SerializeField, ReadOnly]
    List<LEM_BaseEffect> m_FixedUpdateCycle = new List<LEM_BaseEffect>(), m_LateUpdateCycle = new List<LEM_BaseEffect>();


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
            Instance = this;
            InitialiseAllLinearEvents();

            if (m_PlayOnAwake)
            {
#if UNITY_EDITOR
                Debug.Assert(m_PlayingEvent != null, "There is no Playing Event to play OnAwake!", this);
                Debug.Assert(m_PlayingEvent.m_StartNodeData.HasAtLeastOneNextPointNode, "The Start Node of " + m_PlayingEvent.name + " has not been connected to any effects", m_PlayingEvent);
#endif

                LoadNextEffect(m_InstantEffectCapacity, m_PlayingEvent.m_StartNodeData.m_NextPointsIDs[0]);
                m_isEffectsPaused = false;
            }

        }


    }

    //Call this if you dont want the manager to be initialised on awake
    public void Initialise()
    {
        Instance = this;
        InitialiseAllLinearEvents();
    }

    void InitialiseAllLinearEvents()
    {

#if UNITY_EDITOR
        if (m_AllLinearEventsInScene == null || m_AllLinearEventsInScene.Length <= 0)
        {
            Debug.LogWarning("There is no Linear Events in this scene!", this);
            return;
        }
#endif

        //O(n^2)
        for (int i = 0; i < m_AllLinearEventsInScene.Length; i++)
        {
#if UNITY_EDITOR
            Debug.Assert(m_AllLinearEventsInScene[i] != null, "There is a null Linear Event in AllLinearEventsInScene's array at element " + i, m_AllLinearEventsInScene[i]);
            Debug.Assert(m_AllLinearEventsInScene[i].m_AllEffects != null && m_AllLinearEventsInScene[i].m_AllEffects.Length > 0,
                "Linear Event " + m_AllLinearEventsInScene[i].name + " does not have any effects on it. AllLinearEventsInScene element: " + i,
                m_AllLinearEventsInScene[i]);
#endif

            m_AllLinearEventsEffectsDictionary.Add(m_AllLinearEventsInScene[i], m_AllLinearEventsInScene[i].AllEffectsDictionary);
        }

        m_UpdateCycle = new List<LEM_BaseEffect>(m_StartingUpdateEffectCapacity);
        m_FixedUpdateCycle = new List<LEM_BaseEffect>(m_StartingUpdateEffectCapacity);
        m_LateUpdateCycle = new List<LEM_BaseEffect>(m_StartingUpdateEffectCapacity);

        m_IsInitialised = true;
    }

    //Update loop will be where all the effects will be called and then removed if 
    //their effects are done
    void Update()
    {
        if (!m_IsInitialised || m_isEffectsPaused)
            return;

        for (int i = 0; i < m_UpdateCycle.Count; i++)
        {
            if (m_UpdateCycle[i].UpdateEffect())
            {
                m_UpdateCycle.RemoveEfficiently(i);
            }
        }

        //Dont load new effect if there is a delay
        if (m_DelayBeforeNextEffect > 0)
        {
            m_DelayBeforeNextEffect -= Time.deltaTime;
            return;
        }

        if (m_PlayingEvent != null)
        {
            ListenToLoadNextEffect();
        }

    }

    void FixedUpdate()
    {
        if (!m_IsInitialised || m_isEffectsPaused)
            return;

        for (int i = 0; i < m_FixedUpdateCycle.Count; i++)
        {
            if (m_FixedUpdateCycle[i].UpdateEffect())
            {
                m_FixedUpdateCycle.RemoveEfficiently(i);
            }
        }
    }

    void LateUpdate()
    {
        if (!m_IsInitialised || m_isEffectsPaused)
            return;

        for (int i = 0; i < m_LateUpdateCycle.Count; i++)
        {
            if (m_LateUpdateCycle[i].UpdateEffect())
            {
                m_LateUpdateCycle.RemoveEfficiently(i);
            }
        }
    }

    void AddEffectToCycle(LEM_BaseEffect effectToAdd)
    {
        switch (effectToAdd.m_UpdateCycle)
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

    void ListenToLoadNextEffect()
    {
        //If manager is listening for a click
        if (m_ListeningForClick)
            return;

        if (m_ListeningForTrigger)
            return;

        LoadNextEffect(m_InstantEffectCapacity);

    }

    void LoadNextEffect(int maxEffectsPerFrame)
    {
        //Stop loading effect if there is no next effect
        if (!m_PreviousEffectPlayed.m_NodeBaseData.HasAtLeastOneNextPointNode)
            return;

        //Record next effect
        m_PreviousEffectPlayed = m_AllLinearEventsEffectsDictionary[m_PlayingEvent][m_PreviousEffectPlayed.GetNextNodeID()];

        maxEffectsPerFrame--;

        m_PreviousEffectPlayed.Initialise();

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
            case LEM_BaseEffect.EffectFunctionType.HaltEffect:
                return;
        }

        if (maxEffectsPerFrame > 0)
            LoadNextEffect(maxEffectsPerFrame);
    }

    void LoadNextEffect(int maxEffectsPerFrame, string effectID)
    {
        //Load next effect
        m_PreviousEffectPlayed = m_AllLinearEventsEffectsDictionary[m_PlayingEvent][effectID];

        //Stop loading effect if there is no next effect
        if (!m_PreviousEffectPlayed.m_NodeBaseData.HasAtLeastOneNextPointNode)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Effect " + m_PreviousEffectPlayed.name + " does not have any effect to play!", m_PreviousEffectPlayed);
#endif
            return;
        }

        maxEffectsPerFrame--;

        m_PreviousEffectPlayed.Initialise();

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
            case LEM_BaseEffect.EffectFunctionType.HaltEffect:
                return;
        }

        if (maxEffectsPerFrame > 0)
            LoadNextEffect(maxEffectsPerFrame);

    }

}
