using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LEM_Effects.Extensions;
namespace LEM_Effects
{

    [CanEditMultipleObjects, System.Serializable]
    public class LinearEvent : MonoBehaviour
    {

        #region Cached Values
        [Header("DONT TOUCH! Cached Values!"), ReadOnly]
        public LEM_BaseEffect[] m_AllEffects = default;

#if UNITY_EDITOR
        [HideInInspector]
        public GroupRectNodeBase[] m_AllGroupRectNodes = default;
#endif

        [HideInInspector]
        public NodeBaseData m_StartNodeData = default;

        //Runtime values
        public Dictionary<string, LEM_BaseEffect> GetAllEffectsDictionary
        {
            get
            {
                if (m_AllEffects == null || m_AllEffects.Length <= 0)
                    return null;

                Dictionary<string, LEM_BaseEffect> allEffectsDictionary = new Dictionary<string, LEM_BaseEffect>();

                for (int i = 0; i < m_AllEffects.Length; i++)
                    allEffectsDictionary.Add(m_AllEffects[i].m_NodeBaseData.m_NodeID, m_AllEffects[i]);

                return allEffectsDictionary;
            }
        }

     
        #endregion

        #region RunTime Values

        public int m_LinearEventIndex = -1;

        float m_DelayBeforeNextEffect = 0f;
        public float AddDelayBeforeNextEffect { set { m_DelayBeforeNextEffect += value; } }
        public float SetDelayBeforeNextEffect { set { m_DelayBeforeNextEffect = value; } }

        bool m_PauseLinearEvent = true;
        public bool PauseLinearEvent { set { m_PauseLinearEvent = value; } }

        #region Bool Conditions
        int m_NumOfAwaitingInput = default;
        public int AddNumberOfAwaitingInput { set { m_NumOfAwaitingInput += value; } }
        //public bool m_ListeningForClick = false;
        //public bool m_ListeningForTrigger = false;


        #endregion
        [SerializeField, ReadOnly]
        LEM_BaseEffect m_PreviousEffectPlayed = default;
        //public bool HasAnymoreEffectsToLoad => m_PreviousEffectPlayed.m_NodeBaseData.HasAtLeastOneNextPointNode;
        public bool HasNoEffectsPlaying => m_UpdateCycle.Count == 0 && m_FixedUpdateCycle.Count == 0 && m_LateUpdateCycle.Count == 0;

        //Update cycles
        [SerializeField, ReadOnly, Header("RunTime Cycles (Do not touch)"), Space(15)]
        List<LEM_BaseEffect> m_UpdateCycle = default;

        [SerializeField, ReadOnly]
        List<LEM_BaseEffect> m_FixedUpdateCycle = default, m_LateUpdateCycle = default;

        public Dictionary<string, LEM_BaseEffect> m_EffectsDictionary = default;

        #endregion

        #region Initialisation Methods

        public void InitialiseLinearEvent()
        {
            m_UpdateCycle = new List<LEM_BaseEffect>(LinearEventsManager.StartingUpdateEffectCapacity);
            m_FixedUpdateCycle = new List<LEM_BaseEffect>(LinearEventsManager.StartingUpdateEffectCapacity);
            m_LateUpdateCycle = new List<LEM_BaseEffect>(LinearEventsManager.StartingUpdateEffectCapacity);

            m_EffectsDictionary = GetAllEffectsDictionary;
            m_PauseLinearEvent = false;
        }

        //Returns true if there is a starting next point node
        public void RunTimeStartLinearEvent()
        {
            if (m_StartNodeData.HasAtLeastOneNextPointNode)
            {
                LoadStartingEffect(LinearEventsManager.InstantEffectCapacity, m_StartNodeData.m_NextPointsIDs[0]);
                LinearEventsManager.Instance.RunningLinearEvents.Add(this);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning("LinearEvent " + name + " does not have any effect connected to its start ndoe!", this);
            }
#endif
        }

        public void OnStartPlayingLinearEvent()
        {
            LoadStartingEffect(LinearEventsManager.InstantEffectCapacity, m_StartNodeData.m_NextPointsIDs[0]);
        }

        #endregion

        #region Main Update Methods

        public void CycleUpdate()
        {
            if (m_PauseLinearEvent)
                return;

            for (int i = 0; i < m_UpdateCycle.Count; i++)
            {
                if (m_UpdateCycle[i].OnUpdateEffect())
                {
                    m_UpdateCycle[i].OnEndEffect();
                    m_UpdateCycle.RemoveEfficiently(i);
                    i--;
                }
            }
        }


        public void CycleFixedUpdate()
        {
            if (m_PauseLinearEvent)
                return;

            for (int i = 0; i < m_FixedUpdateCycle.Count; i++)
            {
                if (m_FixedUpdateCycle[i].OnUpdateEffect())
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
            if (m_PauseLinearEvent)
                return false;

            for (int i = 0; i < m_LateUpdateCycle.Count; i++)
            {
                if (m_LateUpdateCycle[i].OnUpdateEffect())
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

        #region Supporting Methods
        //Checks if this LinearEvent needs a click or some trigger b4 it can load next effect
        //Returns true if there is no more possible effects that can be loaded
        bool ListenToLoadNextEffect()
        {
            if (m_NumOfAwaitingInput > 0)
                return false;

            //If LE isnt finished loading all the effects
            else if (!TryLoadNextEffect(LinearEventsManager.InstantEffectCapacity))
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
            if (m_PreviousEffectPlayed == null || !m_PreviousEffectPlayed.m_NodeBaseData.HasAtLeastOneNextPointNode || !m_EffectsDictionary.TryGetValue(m_PreviousEffectPlayed.GetNextNodeID(), out m_PreviousEffectPlayed))
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
            if (!m_EffectsDictionary.TryGetValue(nextEffect, out m_PreviousEffectPlayed))
            {
#if UNITY_EDITOR
                Debug.LogWarning("Linear Event " + name + " does not contain the ID " + nextEffect, this);
#endif
                return;
            }

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
                    break;
            }

            if (maxEffectsPerFrame > 0/* && !HasAnymoreEffectsToLoad*/)
                TryLoadNextEffect(maxEffectsPerFrame);

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


        #endregion

    }



}