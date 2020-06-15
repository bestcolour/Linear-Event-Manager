using UnityEngine;
namespace LEM_Effects
{
    public class CurveAlphaToRenderers : SingleCurveBasedUpdateEffect<CurveAlphaToRenderers>
#if UNITY_EDITOR
        , IEffectSavable<Renderer[], AnimationCurve>
#endif
    {
        //target
        [Tooltip("The renderer you want to fade")]
        [SerializeField] Renderer[] m_TargetRenderers = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

#if UNITY_EDITOR
            int fadeModeInt = (int)UnityEngine.Rendering.RenderQueue.Geometry;
#endif

            Color c;
            delta = m_Graph.Evaluate(m_Timer);

            for (int r = 0; r < m_TargetRenderers.Length; r++)
            {
                for (int m = 0; m < m_TargetRenderers[r].materials.Length; m++)
                {
#if UNITY_EDITOR
                    Debug.Assert(
                        m_TargetRenderers[r].materials[m].renderQueue != fadeModeInt,
                        "Material " + m_TargetRenderers[r].materials[m] + " is not set to Fade/Transparent Rendering Mode",
                        m_TargetRenderers[r].materials[m]);
#endif

                    c = m_TargetRenderers[r].materials[m].color;
                    c.a = delta;
                    m_TargetRenderers[r].materials[m].color = c;
                }
            }

            return d_UpdateCheck.Invoke();
        }


#if UNITY_EDITOR

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            CurveAlphaToRenderers t = go.AddComponent<CurveAlphaToRenderers>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetRenderers, out t.m_Graph);
            return t;
        }

        public void SetUp(Renderer[] t1, AnimationCurve t2)
        {
            m_TargetRenderers = t1;
            m_Graph = t2;
        }

        public void UnPack(out Renderer[] t1, out AnimationCurve t2)
        {
            t1 = m_TargetRenderers;
            t2 = m_Graph;
        }

#endif


    }

}