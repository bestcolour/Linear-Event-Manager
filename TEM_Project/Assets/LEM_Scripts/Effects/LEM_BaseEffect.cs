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

        [Tooltip("Write a summary about what this event does if you want to"), TextArea(3, 5)]
        public string m_Description = default;

        [Tooltip("Which Update Cycle Effect is In")]
        public UpdateCycle m_UpdateCycle = default;

        [Tooltip("Stores basic node data. This is applicable for effect nodes as well"), Header("Node Data")]
        public NodeBaseData m_NodeBaseData = default;

        public LEM_BaseEffect ShallowClone()
        {
            return (LEM_BaseEffect)this.MemberwiseClone();
        }

        public virtual void Initialise()
        { }

        //Update event for the event inheriting this
        /// <summary>
        /// Returns true when the effect has finished executing else return false to keep updating the effect
        /// </summary>
        /// <returns></returns>
        public virtual bool TEM_Update()
        { return true; }



    }

    public interface IEffectSavable<T1>
    {
        void SetUp(T1 t1);

        void UnPack(out T1 t1);

    }

    public interface IEffectSavable<T1, T2>
    {
        void SetUp(T1 t1, T2 t2);

        void UnPack(out T1 t1, out T2 t2);

    }

    public interface IEffectSavable<T1, T2, T3>
    {
        void SetUp(T1 t1, T2 t2, T3 t3);

        void UnPack(out T1 t1, out T2 t2, out T3 t3);

    }

    public interface IEffectSavable<T1, T2, T3, T4>
    {
        void SetUp(T1 t1, T2 t2, T3 t3, T4 t4);

        void UnPack(out T1 t1, out T2 t2, out T3 t3, out T4 t4);

    }

    public interface IEffectSavable<T1, T2, T3, T4, T5>
    {
        void SetUp(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

        void UnPack(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5);

    }

    public interface IEffectSavable<T1, T2, T3, T4, T5, T6>
    {
        void SetUp(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

        void UnPack(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6);

    }

    public interface IEffectSavable<T1, T2, T3, T4, T5, T6, T7>
    {
        void SetUp(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);

        void UnPack(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6, out T7 t7);

    }

    public interface IEffectSavable<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        void SetUp(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);

        void UnPack(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5, out T6 t6, out T7 t7, out T8 t8);

    }

}