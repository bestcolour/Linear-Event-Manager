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
    //public NodeBaseData m_EndNodeData = default;


    //Removes any unconnected LEM effects from the m_AllEffectsDictionary
    public void RemoveUnusedEvents()
    {
        //Check if start node is even connected to anything
        if (m_StartNodeData.HasAtLeastOneNextPointNode)
        {
            LEM_BaseEffect currentEffect = m_AllEffectsDictionary[m_StartNodeData.m_NextPointsIDs[0]];

            List<LEM_BaseEffect> effectsInUse = new List<LEM_BaseEffect>();

            while (currentEffect != null)
            {
                effectsInUse.Add(currentEffect);

                //If this effect has at least one next node connected to, assign this to next point's node
                if (currentEffect.m_NodeBaseData.HasAtLeastOneNextPointNode)
                {
                    currentEffect = m_AllEffectsDictionary[currentEffect.m_NodeBaseData.m_NextPointsIDs[0]];
                    continue;
                }

                break;
            }

            m_AllEffectsDictionary = new Dictionary<string, LEM_BaseEffect>();
            m_AllEffects = new LEM_BaseEffect[effectsInUse.Count];

            for (int i = 0; i < effectsInUse.Count; i++)
            {
                //Repopulate the collections with the effects that are only in use
                m_AllEffectsDictionary.Add(effectsInUse[i].m_NodeBaseData.m_NodeID, effectsInUse[i]);
                m_AllEffects[i] = effectsInUse[i];
            }
        }
        else
        {
            Debug.LogWarning("There is no effects connected to the start node thus there is no Events being used.");
        }

    }

}



