using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// © 2020 Lee Kee Shen All Rights Reserved
/// </summary>
namespace LEM_Effects
{

    public enum UpdateCycle
    {
        Update, FixedUpdate, LateUpdate
    }

    [Serializable]
    public abstract class LEM_BaseEffect : ScriptableObject
    {
        //Records the node type this effect belongs to
        [ReadOnly] public string m_NodeEffectType = default;

//#if UNITY_EDITOR
        [Tooltip("Write a summary about what this event does if you want to"), TextArea(3, 5)]
        public string m_Description = default; 
//#endif

        [Tooltip("Which Update Cycle Effect is In")]
        public UpdateCycle m_UpdateCycle = default;

        [Tooltip("Stores basic node data. This is applicable for effect nodes as well"), Header("Node Data")]
        public NodeBaseData m_NodeBaseData = default;

        public LEM_BaseEffect ShallowClone()
        {
            return (LEM_BaseEffect)this.MemberwiseClone();
        }

        public virtual void Initialise() { }

        //Update event for the event inheriting this
        /// <summary>
        /// Returns true when the effect has finished executing else return false to keep updating the effect
        /// </summary>
        /// <returns></returns>
        public virtual bool ExecuteEffect()
        { return true; }

        /// <summary>
        /// Potential for multiple output node
        /// </summary>
        /// <returns></returns>
        public virtual string NextEffectID()
        {
            return m_NodeBaseData.m_NextPointsIDs[0];
        }
    }

}