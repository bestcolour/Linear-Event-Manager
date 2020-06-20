using UnityEngine;
namespace LEM_Effects
{
    [AddComponentMenu("")] public class  CurveAlphaToRenderer : SingleCurveBasedUpdateEffect<CurveAlphaToRenderer>
#if UNITY_EDITOR
        , IEffectSavable<Renderer, AnimationCurve>
#endif
    {
        //target
        [Tooltip("The renderer you want to fade")]
        [SerializeField] Renderer m_TargetRenderer = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            Color c;
            delta = m_Graph.Evaluate(m_Timer);

#if UNITY_EDITOR
            int fadeModeInt = (int)UnityEngine.Rendering.RenderQueue.Geometry;
#endif

            for (int i = 0; i < m_TargetRenderer.materials.Length; i++)
            {
#if UNITY_EDITOR
                Debug.Assert(
                    m_TargetRenderer.materials[i].renderQueue != fadeModeInt,
                    "Material " + m_TargetRenderer.materials[i] + " is not set to Fade/Transparent Rendering Mode",
                     m_TargetRenderer.materials[i]);
#endif


                c =  m_TargetRenderer.materials[i].color;
                c.a = delta;
                m_TargetRenderer.materials[i].color = c;
            }

            return d_UpdateCheck.Invoke();
        }


#if UNITY_EDITOR

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            CurveAlphaToRenderer t = go.AddComponent<CurveAlphaToRenderer>();
            t.CloneBaseValuesFrom(this);
            t.SetUp(m_TargetRenderer, m_Graph.Clone());

            return t;
        }

        public void SetUp(Renderer t1, AnimationCurve t2)
        {
            m_TargetRenderer=  t1 ;
            m_Graph = t2;
        }

        public void UnPack(out Renderer t1, out AnimationCurve t2)
        {
            t1 = m_TargetRenderer;
            t2 = m_Graph;
        }

#endif


    }

}