using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LEM_Effects
{
    public class EqualRandomOutCome : LEM_BaseEffect, IEffectSavable<int>
    {
        public int m_NumberOfOutComes = 2;


        public override bool UpdateEffect()
        {
            return true;
        }

        public override void Initialise() { }

        public override string GetNextNodeID()
        {
            if (m_NodeBaseData.HasAtLeastOneNextPointNode)
            {
                List<string> listOfPossibleIDs = new List<string>();

                for (int i = 0; i < m_NodeBaseData.m_NextPointsIDs.Length; i++)
                {
                    //only when id is valid,
                    if (!string.IsNullOrEmpty(m_NodeBaseData.m_NextPointsIDs[i]))
                    {
                        listOfPossibleIDs.Add(m_NodeBaseData.m_NextPointsIDs[i]);
                    }
                }

                return listOfPossibleIDs[Random.Range(0, listOfPossibleIDs.Count)];

            }

            return null;
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