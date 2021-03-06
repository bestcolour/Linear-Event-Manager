using UnityEngine;

namespace LEM_Effects
{
    [AddComponentMenu("")] public class  SetGameObjectActive : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<GameObject, bool> 
#endif
    {
        [Tooltip("Object to set its active state to true or false")]
        [SerializeField] GameObject m_TargetObject = default;

        [Tooltip("Set object's active state")]
        [SerializeField] bool m_State = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

     
        public override void OnInitialiseEffect()
        {
            //Set the target object to true or false
            m_TargetObject.SetActive(m_State);
        }

#if UNITY_EDITOR
        public void SetUp(GameObject t1, bool t2)
        {
            m_TargetObject = t1;
            m_State = t2;
        }

        public void UnPack(out GameObject t1, out bool t2)
        {
            t1 = m_TargetObject;
            t2 = m_State;
        }   
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            SetGameObjectActive t = go.AddComponent<SetGameObjectActive>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetObject, out t.m_State);
            return t;
        }

#endif
    } 
}