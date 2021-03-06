using UnityEngine;
namespace LEM_Effects
{

    [AddComponentMenu("")]
    public class CurveAlphaToCanvasGroup : SingleCurveBasedUpdateEffect<CurveAlphaToGraphic>
#if UNITY_EDITOR
        , IEffectSavable<CanvasGroup, AnimationCurve> 
#endif
    {
        [SerializeField, Tooltip("The target CanvasGroup you wish to curve its alpha")]
        CanvasGroup m_TargetCanvasGroup = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;



        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            float a = m_TargetCanvasGroup.alpha;
            a = m_Graph.Evaluate(m_Timer);
            m_TargetCanvasGroup.alpha = a;


            return d_UpdateCheck.Invoke();
        }

#if UNITY_EDITOR
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            CurveAlphaToCanvasGroup t = go.AddComponent<CurveAlphaToCanvasGroup>();

            t.CloneBaseValuesFrom(this);
            t.SetUp(m_TargetCanvasGroup, m_Graph.Clone());

            return t;
        }

        public void SetUp(CanvasGroup t1, AnimationCurve t2)
        {
            m_TargetCanvasGroup = t1;
            m_Graph = t2;
        }

        public void UnPack(out CanvasGroup t1, out AnimationCurve t2)
        {
            t1 = m_TargetCanvasGroup;
            t2 = m_Graph;
        }
#endif
    }

}