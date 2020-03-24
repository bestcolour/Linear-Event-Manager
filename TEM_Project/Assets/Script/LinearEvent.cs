using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LEM_Effects;

public class LinearEvent : MonoBehaviour
{
    [Header("Name of This String of Events ")]
    public string m_LinearEventName = default;

    //For testing purposes only, remove once if else node and switch case node has been introduced
    [Header("Flow of Events That Will Run")]
    public LEM_BaseEffect[] m_EffectsConnected = default;

    [Header("Unused Events Saved From Node Editor")]
    public LEM_BaseEffect[] m_EffectsUnConnected= default;

    //public NodeBaseData m_StartNodeData = default;
    //public NodeBaseData m_EndNodeData = default;

    //private void Start()
    //{
    //    Debug.Log(m_EffectsConnected[0].m_Description);

    //    Debug.Log(m_EffectsConnected[0].TEM_Update());

    //}


}



