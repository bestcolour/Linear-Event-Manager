using UnityEngine;
namespace LEM_Effects
{

    public class SetGameObjectsActive : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<GameObject[], bool> 
#endif
    {
        [Tooltip("Objects to set their active states to true or false")]
        [SerializeField] GameObject[] m_TargetObjects = default;

        [Tooltip("Set all the objects to this one active state")]
        [SerializeField] bool m_State = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        public override void OnInitialiseEffect()
        {
            //Set all objects to the same state
            for (int i = 0; i < m_TargetObjects.Length; i++)
            {
                m_TargetObjects[i].SetActive(m_State);
            }
        }

        public void SetUp(GameObject[] t1, bool t2)
        {
            m_TargetObjects = t1;
            m_State= t2;
        }

        public void UnPack(out GameObject[] t1, out bool t2)
        {
            t1 = m_TargetObjects;
            t2 = m_State;
        }
    }
}