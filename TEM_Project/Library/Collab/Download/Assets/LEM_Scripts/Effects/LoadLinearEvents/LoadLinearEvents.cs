//using UnityEngine;
//namespace LEM_Effects
//{

//    public class LoadLinearEvents : LEM_BaseEffect
//#if UNITY_EDITOR
//        , IEffectSavable<LinearEvent[]>
//#endif
//    {
//        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

//        [SerializeField]
//        LinearEvent[] m_TargetLinearEvents = default;

//        public override void OnInitialiseEffect()
//        {
//            for (int i = 0; i < m_TargetLinearEvents.Length; i++)
//            {
//                m_TargetLinearEvents[i].InitialiseLinearEvent();
//            }
//        }
//#if UNITY_EDITOR

//        public void SetUp(LinearEvent[] t1)
//        {
//            m_TargetLinearEvents = t1;
//        }

//        public void UnPack(out LinearEvent[] t1)
//        {
//            t1 = m_TargetLinearEvents;
//        }

//        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
//        {
//            LoadLinearEvents t = go.AddComponent<LoadLinearEvents>();
//            t.CloneBaseValuesFrom(this);
//            UnPack(out t.m_TargetLinearEvents);
//            return t;
//        }
//#endif
//    }

//}