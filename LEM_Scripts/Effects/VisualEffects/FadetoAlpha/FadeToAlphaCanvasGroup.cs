using UnityEngine;
namespace LEM_Effects
{

    [AddComponentMenu("")] public class  FadeToAlphaCanvasGroup : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<CanvasGroup, float, float>
#endif
    {
        //target
        [Tooltip("The Graphic you want to fade")]
        [SerializeField] CanvasGroup m_TargetCanvasGroup = default;

        //Cached alpha values
        [SerializeField, ReadOnly]
        float m_InitialAlpha = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want the canvas group to have at the end of the fade")]
        [SerializeField] float m_TargetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for the fade to be complete")]
        [SerializeField] float m_Duration = default;

        //Runtime value
        float m_NextAlpha = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;



        public override void OnInitialiseEffect()
        {
            //Record initial alpha first
            m_InitialAlpha = m_TargetCanvasGroup.alpha;

            //Recalculate the target alpha (convert to normalized value)
            m_TargetAlpha /= 255f;

            m_NextAlpha = m_TargetCanvasGroup.alpha;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            //Set target image colour as new colour value
            m_NextAlpha = Mathf.Lerp(m_InitialAlpha, m_TargetAlpha, m_Timer / m_Duration);
            m_TargetCanvasGroup.alpha = m_NextAlpha;

            //Once the time passed exceeds the assigned duration, end this effect
            if (m_Timer > m_Duration)
            {
                //Set the targetimage as the actual targetted colour
                m_TargetCanvasGroup.alpha = m_TargetAlpha;
                m_IsFinished = true;
            }

            return m_IsFinished;
        }


#if UNITY_EDITOR
        public void SetUp(CanvasGroup t1, float t2, float t3)
        {
            m_TargetCanvasGroup = t1;
            m_TargetAlpha = t2;
            m_Duration = t3;

        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            FadeToAlphaCanvasGroup t = go.AddComponent<FadeToAlphaCanvasGroup>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetCanvasGroup, out t.m_TargetAlpha, out t.m_Duration);
            return t;
        }

        public void UnPack(out CanvasGroup t1, out float t2, out float t3)
        {
            t1 = m_TargetCanvasGroup;
            t2 = m_TargetAlpha;
            t3 = m_Duration;

        }


#endif
    }
}