using UnityEngine;
namespace LEM_Effects
{
    public class CurveAlphaToSpriteRenderer : SingleCurveBasedUpdateEffect<CurveAlphaToSpriteRenderer>
#if UNITY_EDITOR
        , IEffectSavable<SpriteRenderer, AnimationCurve>
#endif
    {
        //target
        [Tooltip("The renderer you want to fade")]
        [SerializeField] SpriteRenderer m_TargetSpriteRenderer = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            Color c = m_TargetSpriteRenderer.color;
            c.a = m_Graph.Evaluate(m_Timer);
            m_TargetSpriteRenderer.color = c;

            return d_UpdateCheck.Invoke();
        }


#if UNITY_EDITOR

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            CurveAlphaToSpriteRenderer t = go.AddComponent<CurveAlphaToSpriteRenderer>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetSpriteRenderer, out t.m_Graph);
            return t;
        }

        public void SetUp(SpriteRenderer t1, AnimationCurve t2)
        {
            m_TargetSpriteRenderer = t1;
            m_Graph = t2;
        }

        public void UnPack(out SpriteRenderer t1, out AnimationCurve t2)
        {
            t1 = m_TargetSpriteRenderer;
            t2 = m_Graph;
        }

#endif


    }

}