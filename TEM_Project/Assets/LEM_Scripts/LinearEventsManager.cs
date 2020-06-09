using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LEM_Effects;
using LEM_Effects.Extensions;

/// <summary>
/// © 2020 Lee Kee Shen All Rights Reserved
/// </summary>

public class LinearEventsManager : MonoBehaviour
{
    public static LinearEventsManager Instance;

    #region Linear Events Settings
    [Header("Events to Play (Hover over Fields for ToolTips!)"), SerializeField, Tooltip("Should LinearEventManager find all LinearEvents in a LinearEvent Container automatically on Initialisation?")] 
    bool m_AutoFindLinearEvents = false;

    [SerializeField, Tooltip("The Container GameObject that stores all the LinearEvents in the scene"), ConditionalReadOnly("m_AutoFindLinearEvents", m_ConditionToMeet = true)]
    GameObject m_LinearEventsHolder = default;

    [SerializeField, Tooltip("This array is supposed to represent all the Linear Events in the scene. If you enable AutoFindLinearEvents, make sure that the only LinearEvents dragged and dropped in this array are NOT inisde the Container ")/*, ConditionalReadOnly("m_AutoFindLinearEvents", m_ConditionToMeet = false)*/]
    List<LinearEvent> m_AllLinearEventsInScene = default;
    public static List<LinearEvent> AllLinearEventsInScene => Instance.m_AllLinearEventsInScene;

    //The linear events currently running
    [Space(10f), Tooltip("Is used as the first events to play. Is also used to determine what LinearEvents are currently playing."), SerializeField]
    List<LinearEvent> m_RunningLinearEvents = new List<LinearEvent>();
    public List<LinearEvent> RunningLinearEvents => m_RunningLinearEvents;

    [Header("Initialisation Settings"),Space(15f)]
    [Tooltip("Should the LEM Manager initialise itself on Awake or let other scripts initialise it?")]
    [SerializeField] bool m_InitialiseOnAwake = default;

    [Tooltip("Should the LEM Manager start playing an event on Awake after self-initialisation?")]
    [SerializeField] bool m_PlayOnAwake = default;

    [Tooltip("What is the expected amount of effects to run at one time? Increase this if you feel like there will be many effects running over a short period of time"), SerializeField]
    int m_StartingUpdateEffectCapacity = 5;
    public static int StartingUpdateEffectCapacity => Instance.m_StartingUpdateEffectCapacity;

    [SerializeField, Tooltip("How many effects should be executed in a single frame?")]
    int m_InstantEffectCapacity = 10;
    public static int InstantEffectCapacity => Instance.m_InstantEffectCapacity;
    #endregion


    //Stop conditions
    [ReadOnly, Header("RunTime Checks"), Space(15), SerializeField]
    bool m_IsInitialised = false;

    [ReadOnly] bool m_PauseAllRunningLinearEvents = true;
    public bool PauseAllRunningLinearEvents { set { m_PauseAllRunningLinearEvents = value; } }

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
        {
            m_AllLinearEventsInScene.AddRange(m_LinearEventsHolder.GetComponentsInChildren<LinearEvent>());
        }

        //Check if there is any linear event in the scene
#if UNITY_EDITOR
        if (m_AllLinearEventsInScene == null || m_AllLinearEventsInScene.Count <= 0)
        {
            Debug.LogWarning("There is no Linear Events in this scene!", this);
            return;
        }
#endif

        for (int i = 0; i < m_AllLinearEventsInScene.Count; i++)
        {
            //Check if linearevent is null and that if its LEM_Effects array is null or has a length <= 0
#if UNITY_EDITOR
            Debug.Assert(m_AllLinearEventsInScene[i] != null, "There is a null Linear Event in AllLinearEventsInScene's array at element " + i, m_AllLinearEventsInScene[i]);

            if (m_AllLinearEventsInScene[i].m_AllEffects == null && m_AllLinearEventsInScene[i].m_AllEffects.Length <= 0)
            {
                Debug.LogWarning("Linear Event " + m_AllLinearEventsInScene[i].name + " does not have any effects on it. AllLinearEventsInScene element: " + i, m_AllLinearEventsInScene[i]);
            }
#endif
            m_AllLinearEventsInScene[i].m_LinearEventIndex = i;
            m_AllLinearEventsInScene[i].InitialiseLinearEvent();
        }

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
                //Precached events will be ran here on awake if Play on Awake is true
                for (int i = 0; i < m_RunningLinearEvents.Count; i++)
                {
#if UNITY_EDITOR
                    Debug.Assert(m_RunningLinearEvents[i] != null, "Element of Index " + i + " is null!", this);
#endif
                    m_RunningLinearEvents[i].OnLEM_Awake_PlayLinearEvent();
                }
            }

        }


    }

    public static void LoadLinearEvent(LinearEvent linearEventToLoad)
    {
        Instance.m_RunningLinearEvents.Add(linearEventToLoad);
        linearEventToLoad.OnLEM_Runtime_PlayLinearEvent();
    }


    #region Update Loop
    //Update loop will be where all the effects will be called and then removed if 
    //their effects are done
    void Update()
    {
        if (!m_IsInitialised || m_PauseAllRunningLinearEvents)
            return;

        //No removal of linear event from the list will be done in either update or fixed update, rather it will be done in LateUpdate
        for (int i = 0; i < m_RunningLinearEvents.Count; i++)
        {
            m_RunningLinearEvents[i].CycleUpdate();
        }
    }

    void FixedUpdate()
    {
        if (!m_IsInitialised || m_PauseAllRunningLinearEvents)
            return;

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


    #endregion

}
