using UnityEngine;
namespace LEM_Effects
{

    [AddComponentMenu("")]
    public class CurveAlphaToTextMesh : SingleCurveBasedUpdateEffect<CurveAlphaToTextMesh>
#if UNITY_EDITOR
        , IEffectSavable<TextMesh, AnimationCurve> 
#endif
    {
        [SerializeField, Tooltip("The target TextMesh you wish to curve its alpha")]
        TextMesh m_TargetTextMesh = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            Color c = m_TargetTextMesh.color;
            c.a = m_Graph.Evaluate(m_Timer);
            m_TargetTextMesh.color = c;

            return d_UpdateCheck.Invoke();
        }

#if UNITY_EDITOR
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            CurveAlphaToTextMesh t = go.AddComponent<CurveAlphaToTextMesh>();

            t.CloneBaseValuesFrom(this);
            t.SetUp(m_TargetTextMesh, m_Graph.Clone());

            return t;
        }

        public void SetUp(TextMesh t1, AnimationCurve t2)
        {
            m_TargetTextMesh = t1;
            m_Graph = t2;
        }

        public void UnPack(out TextMesh t1, out AnimationCurve t2)
        {
            t1 = m_TargetTextMesh;
            t2 = m_Graph;
        }
#endif
    }

}