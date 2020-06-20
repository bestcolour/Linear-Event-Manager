using UnityEngine;
using UnityEngine.UI;
namespace LEM_Effects
{

    [AddComponentMenu("")]
    public class CurveAlphaToCanvasGroups : SingleCurveBasedUpdateEffect<CurveAlphaToCanvasGroups>
#if UNITY_EDITOR
        , IEffectSavable<CanvasGroup[], AnimationCurve> 
#endif
    {
        [SerializeField, Tooltip("The target Canvas Groups you wish to curve their alpha")]
        CanvasGroup[] m_TargetCanvasGroup = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;



        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            float a;
            a = m_Graph.Evaluate(m_Timer);


            for (int i = 0; i < m_TargetCanvasGroup.Length; i++)
            {
                m_TargetCanvasGroup[i].alpha = a;
            }


            return d_UpdateCheck.Invoke();
        }

#if UNITY_EDITOR
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            CurveAlphaToCanvasGroups t = go.AddComponent<CurveAlphaToCanvasGroups>();

            t.CloneBaseValuesFrom(this);
            t.SetUp(m_TargetCanvasGroup, m_Graph.Clone());

            return t;
        }

        public void SetUp(CanvasGroup[] t1, AnimationCurve t2)
        {
            m_TargetCanvasGroup = t1;
            m_Graph = t2;
        }

        public void UnPack(out CanvasGroup[] t1, out AnimationCurve t2)
        {
            t1 = m_TargetCanvasGroup;
            t2 = m_Graph;
        }
#endif
    }

}