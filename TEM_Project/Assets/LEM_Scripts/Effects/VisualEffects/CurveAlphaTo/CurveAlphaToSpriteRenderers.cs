using UnityEngine;
namespace LEM_Effects
{
    public class CurveAlphaToSpriteRenderers : SingleCurveBasedUpdateEffect<CurveAlphaToSpriteRenderers>
#if UNITY_EDITOR
        , IEffectSavable<SpriteRenderer[], AnimationCurve>
#endif
    {
        //target
        [Tooltip("The renderer you want to fade")]
        [SerializeField] SpriteRenderer[] m_TargetSpriteRenderers = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            Color c;
            delta = m_Graph.Evaluate(m_Timer);

            for (int i = 0; i < m_TargetSpriteRenderers.Length; i++)
            {
                c = m_TargetSpriteRenderers[i].color;
                c.a = delta;
                m_TargetSpriteRenderers[i].color = c;
            }

            return d_UpdateCheck.Invoke();
        }


#if UNITY_EDITOR

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            CurveAlphaToSpriteRenderers t = go.AddComponent<CurveAlphaToSpriteRenderers>();
            t.CloneBaseValuesFrom(this);
            t.SetUp(m_TargetSpriteRenderers, m_Graph.Clone());

            return t;
        }

        public void SetUp(SpriteRenderer[] t1, AnimationCurve t2)
        {
            m_TargetSpriteRenderers = t1;
            m_Graph = t2;
        }

        public void UnPack(out SpriteRenderer[] t1, out AnimationCurve t2)
        {
            t1 = m_TargetSpriteRenderers;
            t2 = m_Graph;
        }

#endif


    }

}