﻿using System.Collections.Generic;
using UnityEngine;

namespace LEM_Effects
{
    public class EqualProbabilityOutCome : LEM_BaseEffect, IEffectSavable<int>
    {
        [SerializeField]
        int m_NumberOfOutComes = 2;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override string GetNextNodeID()
        {
            if (!m_NodeBaseData.HasAtLeastOneNextPointNode)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Equal Random OutCome effect doesnt have any proceeding effects to randomise to!", this);
#endif
                return null;

            }

            //This means that null next pointids can also be triggered as long as there is at least one non-empty outcome
            return m_NodeBaseData.m_NextPointsIDs[Random.Range(0, m_NumberOfOutComes)];

        }

        public void SetUp(int t1)
        {
            m_NumberOfOutComes = t1;
        }

        public void UnPack(out int t1)
        {
            t1 = m_NumberOfOutComes;
        }
    }

}