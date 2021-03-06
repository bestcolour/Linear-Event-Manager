using UnityEngine;
using UnityEngine.UI;
namespace LEM_Effects
{

    [AddComponentMenu("")]
    public class CurveAlphaToGraphic : SingleCurveBasedUpdateEffect<CurveAlphaToGraphic>
#if UNITY_EDITOR
        , IEffectSavable<Graphic, AnimationCurve> 
#endif
    {
        [SerializeField, Tooltip("The target Graphic you wish to curve its alpha")]
        Graphic m_TargetGraphic = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

      

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            Color c = m_TargetGraphic.color;
            c.a = m_Graph.Evaluate(m_Timer);
            m_TargetGraphic.color = c;


            return d_UpdateCheck.Invoke();
        }

#if UNITY_EDITOR
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            CurveAlphaToGraphic t = go.AddComponent<CurveAlphaToGraphic>();

            t.CloneBaseValuesFrom(this);
            t.SetUp(m_TargetGraphic, m_Graph.Clone());

            return t;
        }

        public void SetUp(Graphic t1, AnimationCurve t2)
        {
            m_TargetGraphic = t1;
            m_Graph = t2;
        }

        public void UnPack(out Graphic t1, out AnimationCurve t2)
        {
            t1 = m_TargetGraphic;
            t2 = m_Graph;
        } 
#endif
    }

}