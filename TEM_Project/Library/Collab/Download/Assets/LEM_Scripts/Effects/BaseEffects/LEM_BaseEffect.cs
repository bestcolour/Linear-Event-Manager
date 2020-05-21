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
        public enum EffectFunctionType { InstantEffect, UpdateEffect, InstantHaltEffect,UpdateHaltEffect}

        public abstract EffectFunctionType FunctionType { get; }

        //Records the node type this effect belongs to
        [ReadOnly] public string m_NodeEffectType = default;

//#if UNITY_EDITOR
//        [Tooltip("Write a summary about what this event does if you want to"), TextArea(3, 5)]
//        public string m_Description = default;
//#endif

        [Tooltip("Which Update Cycle Effect is In")]
        public UpdateCycle m_UpdateCycle = default;

        [Tooltip("Stores basic node data. This is applicable for effect nodes as well"), Header("Node Data")]
        public NodeBaseData m_NodeBaseData = default;

        public virtual LEM_BaseEffect ShallowClone() { return (LEM_BaseEffect)this.MemberwiseClone(); }

        public virtual void OnInitialiseEffect() { }

        //Update event for the event inheriting this
        /// <summary>
        /// Returns true when the effect has finished executing else return false to keep updating the effect
        /// </summary>
        /// <returns></returns>
        public virtual bool OnUpdateEffect() { return true; }

        /// <summary>
        /// To be called before loading the next effect
        /// </summary>
        /// <returns></returns>
        public virtual string GetNextNodeID() { return m_NodeBaseData.m_NextPointsIDs[0]; }

        /// <summary>
        /// To be called when to forcibly stop the effect if they are the UpdateEffect
        /// </summary>
        public virtual void ForceStop() { }

        /// <summary>
        /// To be called when the effect is removed
        /// </summary>
        public virtual void OnEndEffect() { }
    }

}