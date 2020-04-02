using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using LEM_Effects;

[CanEditMultipleObjects]
public class LinearEvent : MonoBehaviour
{
    [Header("Name of This String of Events ")]
    public string m_LinearEventName = default;

    //For testing purposes only, remove once if else node and switch case node has been introduced
    //Comment this out after you are done
    public LEM_BaseEffect[] m_AllEffects = default;

    public Dictionary<string, LEM_BaseEffect> m_AllEffectsDictionary = default;


    public NodeBaseData m_StartNodeData = default;
    public NodeBaseData m_EndNodeData = default;


    //Removes any unconnected LEM effects from the m_AllEffectsDictionary
    public void RemoveUnusedEvents()
    {
        string[] allKeys = m_AllEffectsDictionary.Keys.ToArray();

        LEM_BaseEffect currentEffect = m_AllEffectsDictionary[m_StartNodeData.m_NextPointNodeID];
        List<LEM_BaseEffect> effectsInUse = new List<LEM_BaseEffect>();

        while (currentEffect.m_NodeBaseData.m_NodeID != m_EndNodeData.m_NodeID)
        {
            effectsInUse.Add(currentEffect);
            currentEffect = m_AllEffectsDictionary[];
        }
      



    }

}



