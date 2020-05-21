using UnityEngine;

/// <summary>
/// © 2020 Lee Kee Shen All Rights Reserved
/// Basically this is where i am going to store all the gameobject related events for TEM
/// </summary>
namespace LEM_Effects
{
    public class DestroyGameObjects : LEM_BaseEffect,IEffectSavable<GameObject[]>
    {
        [Tooltip("Object to destroy")]
        [SerializeField] GameObject[] m_TargetObjects = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void OnInitialiseEffect()
        {
            //Destroy the targetted objects
            for (int i = 0; i < m_TargetObjects.Length; i++)
            {
                GameObject.Destroy(m_TargetObjects[i]);
            }
        }

        public void SetUp(GameObject[] t1)
        {
            m_TargetObjects = t1;
        }

        public void UnPack(out GameObject[] t1)
        {
            t1 = m_TargetObjects;
        }
    }

   
}

