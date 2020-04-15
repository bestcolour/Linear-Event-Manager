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

    public ProgressBar m_ProgressBar = new ProgressBar();
    public bool m_isLoading = default;

    void OnGUI()
    {
        if (m_isLoading)
        {
            Debug.Log("dd");
            m_isLoading = m_ProgressBar.Draw();
        }
    }

    //Ok since whenever u change ur script, unity will recompile and all values will be set to its initialisers (that means m_AllEffectsDictionary will be reseted to null)
    //hence removing all saved progress EXCEPT FOR SERIALIZED VALUES WHICH MEANS THAT M_ALLEFFECTS is unaffected
    public bool CheckAllEffectsDictionary()
    {
        if (m_AllEffectsDictionary != null && m_AllEffectsDictionary.Count > 0)
            return true;

        m_AllEffectsDictionary = new Dictionary<string, LEM_BaseEffect>();

        //Update the m_AllEffectsDictionary
        for (int i = 0; i < m_AllEffects.Length; i++)
        {
            m_AllEffectsDictionary.Add(m_AllEffects[i].m_NodeBaseData.m_NodeID, m_AllEffects[i]);
        }

        if (m_AllEffectsDictionary.Count > 0)
            return true;

        return false;
    }

    //Removes any unconnected LEM effects from the m_AllEffectsDictionary
    public void RemoveUnusedEvents()
    {
        //Check if start node is even connected to anything
        if (m_StartNodeData.HasAtLeastOneNextPointNode)
        {
            if (!CheckAllEffectsDictionary())
                return;

            int numberEffectsRemoved = 0;


            LEM_BaseEffect currentEffect = m_AllEffectsDictionary[m_StartNodeData.m_NextPointsIDs[0]];

            List<LEM_BaseEffect> effectsInUse = new List<LEM_BaseEffect>();

            while (currentEffect != null)
            {
                effectsInUse.Add(currentEffect);

                //For now ill use this to keep track of what is useful
                numberEffectsRemoved++;

                //If this effect has at least one next node connected to, assign this to next point's node
                if (currentEffect.m_NodeBaseData.HasAtLeastOneNextPointNode)
                {
                    currentEffect = m_AllEffectsDictionary[currentEffect.m_NodeBaseData.m_NextPointsIDs[0]];
                    continue;
                }

                break;
            }

            //Now do simple math
            numberEffectsRemoved = m_AllEffectsDictionary.Count - numberEffectsRemoved;

            m_AllEffectsDictionary = new Dictionary<string, LEM_BaseEffect>();
            m_AllEffects = new LEM_BaseEffect[effectsInUse.Count];

            for (int i = 0; i < effectsInUse.Count; i++)
            {
                //Repopulate the collections with the effects that are only in use
                m_AllEffectsDictionary.Add(effectsInUse[i].m_NodeBaseData.m_NodeID, effectsInUse[i]);
                m_AllEffects[i] = effectsInUse[i];
            }

            Debug.Log("Successfully removed " + numberEffectsRemoved + " unused effects");

        }
        else
        {
            Debug.LogWarning("There is no effects connected to the start node thus there is no Events being used.");
        }

    }

    //public void UpdateProgressBar(float progress,string titleString = "",string informationString = "")
    //{
    //    m_ProgressBar.Progress = progress;
    //    m_ProgressBar.TitleString = titleString;
    //    m_ProgressBar.InformationString = informationString;
    //}

}


