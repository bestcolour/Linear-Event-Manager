using UnityEngine;
namespace LEM_Effects
{

    public class CurveAlphaToTextMeshes : SingleCurveBasedUpdateEffect<CurveAlphaToTextMeshes>, IEffectSavable<TextMesh[], AnimationCurve>
    {
        [SerializeField, Tooltip("The target TextMesh you wish to curve its alpha")]
        TextMesh[] m_TargetTextMeshes = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;
            Color c;
            delta = m_Graph.Evaluate(m_Timer);


            for (int i = 0; i < m_TargetTextMeshes.Length; i++)
            {
                c = m_TargetTextMeshes[i].color;
                c.a = delta;
                m_TargetTextMeshes[i].color = c;
            }


            return d_UpdateCheck.Invoke();
        }

#if UNITY_EDITOR
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            CurveAlphaToTextMeshes t = go.AddComponent<CurveAlphaToTextMeshes>();

            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetTextMeshes, out t.m_Graph);

            return t;
        }

        public void SetUp(TextMesh[] t1, AnimationCurve t2)
        {
            m_TargetTextMeshes = t1;
            m_Graph = t2;
        }

        public void UnPack(out TextMesh[] t1, out AnimationCurve t2)
        {
            t1 = m_TargetTextMeshes;
            t2 = m_Graph;
        }
#endif
    }

}