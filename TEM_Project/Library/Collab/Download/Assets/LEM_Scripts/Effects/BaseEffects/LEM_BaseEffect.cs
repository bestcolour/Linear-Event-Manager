using System;
using UnityEngine;

/// <summary>
/// © 2020 Lee Kee Shen All Rights Reserved
/// </summary>
namespace LEM_Effects.AbstractClasses
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
        [ReadOnly] public string bm_NodeEffectType = default;

        [Tooltip("Which Update Cycle Effect is In")]
        public UpdateCycle bm_UpdateCycle = default;

        [Tooltip("Stores basic node data. This is applicable for effect nodes as well"), Header("Node Data")]
        public NodeBaseData bm_NodeBaseData = default;


        public virtual void OnInitialiseEffect() { }

        //Update event for the event inheriting this
        /// <summary>
        /// Returns true when the effect has finished executing else return false to keep updating the effect
        /// </summary>
        /// <returns></returns>
        public virtual bool OnUpdateEffect(float delta) { return true; }

        /// <summary>
        /// To be called before loading the next effect
        /// </summary>
        /// <returns></returns>
        public virtual string GetNextNodeID() { return bm_NodeBaseData.m_NextPointsIDs[0]; }

        /// <summary>
        /// To be called when to forcibly stop the effect if they are the UpdateEffect
        /// </summary>
        public virtual void OnForceStop() { }

        /// <summary>
        /// To be called to reset a updateEffect to reset the isFinished boolean and maybe some other values if necessary
        /// </summary>
        public virtual void OnReset() { }

        /// <summary>
        /// To be called when the effect is removed
        /// </summary>
        public virtual void OnEndEffect() { }


#if UNITY_EDITOR
        public virtual LEM_BaseEffect CreateClone() { return (LEM_BaseEffect)this.MemberwiseClone(); }
#endif

    }

#if UNITY_EDITOR
    public static class LEM_BaseEffect_Extensions
    {
        public static void CloneBaseValuesFrom(this LEM_BaseEffect copy, LEM_BaseEffect original)
        {
            copy.bm_NodeBaseData = original.bm_NodeBaseData;
            copy.bm_NodeEffectType = original.bm_NodeEffectType;
            copy.bm_UpdateCycle = original.bm_UpdateCycle;
        }
    } 
#endif



}