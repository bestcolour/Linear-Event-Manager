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


    //bool HasEffects => m_CurrentLinearEvent != null;

    //Dictionary<string, LEM_BaseEffect> m_CurrentEventDictionary = default;
    Dictionary<LinearEvent, Dictionary<string, LEM_BaseEffect>> m_AllLinearEventsEffectsDictionary = default;

    [Header("Initialisation Settings")]
    [SerializeField, Tooltip("This array is supposed to represent all the Linear Events in the scene.")]
    LinearEvent[] m_AllLinearEventsInScene = default;

    [Tooltip("Should the LEM Manager initialise itself on Awake or let other scripts initialise it?")]
    [SerializeField] bool m_InitialiseOnAwake = default;

    [Tooltip("Should the LEM Manager start playing an event on Awake after self-initialisation?")]
    [SerializeField] bool m_PlayOnAwake = default;

    [Tooltip("Is used as the first event to play. Leave empty if not needed. Is also used to show the current event playing."),SerializeField]
    LinearEvent m_PlayingEvent = default;
    public LinearEvent CurrentEvent { set { m_PlayingEvent = value; } }

    //Stop conditions
    [ReadOnly,Header("RunTime Checks"), Space(15)] 
    public bool m_IsInitialised = false;

    [ReadOnly] bool m_isEffectsPaused = true;
    public bool PauseEffects { set { m_isEffectsPaused = value; } }

    [ReadOnly, SerializeField]
    LEM_BaseEffect m_PreviousEffectPlayed = null;

    [ReadOnly,/* Header("RunTime Values"), Space(15), */SerializeField]
    float m_DelayBeforeNextEffect = default;
    public float TimeToAddToDelay { set { m_DelayBeforeNextEffect += value; } }

    [SerializeField, ReadOnly, Header("RunTime Cycles (Do not touch)"), Space(15)]
    List<LEM_BaseEffect> m_UpdateCycle = new List<LEM_BaseEffect>();

    [SerializeField, ReadOnly]
    List<LEM_BaseEffect> m_FixedUpdateCycle = new List<LEM_BaseEffect>(), m_LateUpdateCycle = new List<LEM_BaseEffect>();

    //[Header("After Effect Type")]
    //public bool m_CallingNextClick;

    //[Header("End of All Effects Disposal")]
    //public GameObject[] m_EndEffectDestroy;

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
                Debug.Assert(m_PlayingEvent.m_StartNodeData.HasAtLeastOneNextPointNode, "The Start Node of " + m_PlayingEvent.name + " has not been connected to any effects", m_PlayingEvent);
#endif
                m_PreviousEffectPlayed = m_AllLinearEventsEffectsDictionary[m_PlayingEvent][m_PlayingEvent.m_StartNodeData.m_NextPointsIDs[0]];
                AddEffectToCycle(m_PreviousEffectPlayed);
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

        m_AllLinearEventsEffectsDictionary = new Dictionary<LinearEvent, Dictionary<string, LEM_BaseEffect>>();

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

        m_IsInitialised = true;
    }

    //Update loop will be where all the effects will be called and then removed if 
    //their effects are done
    void Update()
    {
        if (!m_IsInitialised || m_isEffectsPaused)
            return;

        if (m_DelayBeforeNextEffect > 0)
        {
            m_DelayBeforeNextEffect -= Time.deltaTime;
            return;
        }

        if (m_PlayingEvent != null && m_PreviousEffectPlayed.m_NodeBaseData.HasAtLeastOneNextPointNode) 
        {
            //Load next effect
            m_PreviousEffectPlayed = m_AllLinearEventsEffectsDictionary[m_PlayingEvent][m_PreviousEffectPlayed.NextEffectID()];
            AddEffectToCycle(m_PreviousEffectPlayed);

        }

        for (int i = 0; i < m_UpdateCycle.Count; i++)
        {
            if (m_UpdateCycle[i].TEM_Update())
            {
                m_UpdateCycle.RemoveEfficiently(i);
            }
        }

     
    }

    void FixedUpdate()
    {
        if (!m_IsInitialised || m_isEffectsPaused)
            return;

        if (m_DelayBeforeNextEffect > 0)
            return;

        for (int i = 0; i < m_FixedUpdateCycle.Count; i++)
        {
            if (m_FixedUpdateCycle[i].TEM_Update())
            {
                m_FixedUpdateCycle.RemoveEfficiently(i);
            }
        }
    }

    void LateUpdate()
    {
        if (!m_IsInitialised || m_isEffectsPaused)
            return;

        if (m_DelayBeforeNextEffect > 0)
            return;

        for (int i = 0; i < m_LateUpdateCycle.Count; i++)
        {
            if (m_LateUpdateCycle[i].TEM_Update())
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

}
