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

    [Header("Settings")]
    [Tooltip("Should the LEM Manager initialise itself or let other scripts initialise for it?")]
    [SerializeField] bool m_SelfInitialise = default;

    [Tooltip("Should the LEM Manager play the events on awake?")]
    [SerializeField] bool m_PlayOnAwake = default;
   
    //Base effects collection for node editing 
    public LinearEvent[] m_AllLinearEvents = default;

  

    [Header("Initialization Checks")]
    public bool m_IsFinishedQueuing;
    public bool m_isFinishedEffects;

    [Header("Effect Stats")]
    public int m_NumOfEventsPlayin;
    public int m_CurrEventPlaying;
    public int m_NumOfEventsLeft;

    [Header("After Effect Type")]
    public bool m_CallingImmediateEffect;
    public bool m_CallingNextClick;

    [Header("End of All Effects Disposal")]
    public GameObject[] m_EndEffectDestroy;

    private void Awake()
    {
        Instance = this;
        QueueEffects();
    }

    public void QueueEffects()
    {

    }


    //Update loop will be where all the effects will be called and then removed if 
    //their effects are done
    void Update()
    {
        //for (int i = 0; i < m_PlayingEffects.Count; i++)
        //{
        //    //If the current effect returns true,
        //    if (m_PlayingEffects[i].TEM_Update())
        //    {
        //        //Get a copy of the last element
        //        LEM_BaseEffect copy = m_PlayingEffects[m_PlayingEffects.Count - 1];
        //        //Set the effect that you want to remove as the last effect to remove it
        //        m_PlayingEffects[i] = copy;
        //        //Remove the first element
        //        m_PlayingEffects.RemoveAt(m_PlayingEffects.Count - 1);
        //    }

        //}
    }


}
