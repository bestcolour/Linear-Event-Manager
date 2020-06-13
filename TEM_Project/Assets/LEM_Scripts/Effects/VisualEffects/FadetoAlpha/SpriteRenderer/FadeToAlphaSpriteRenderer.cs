using UnityEngine;
namespace LEM_Effects
{

    public class FadeToAlphaSpriteRenderer : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<SpriteRenderer, float, float> 
#endif
    {
        //target
        [Tooltip("The SpriteRenderer you want to fade")]
        [SerializeField] SpriteRenderer m_TargetSpriteRenderer = default;

        //Cached alpha values
        [SerializeField, ReadOnly]
        float m_InitialAlpha = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want the SpriteRenderer to have at the end of the fade")]
        [SerializeField] float m_TargetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for the fade to be complete")]
        [SerializeField] float m_Duration = default;

        [SerializeField, ReadOnly]
        Color m_NextColour = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

     

        public override void OnInitialiseEffect()
        {
            //Record initial alpha first
            m_InitialAlpha = m_TargetSpriteRenderer.color.a;

            //Recalculate the target alpha (convert to normalized value)
            m_TargetAlpha /= 255f;

            m_NextColour = new Color(m_TargetSpriteRenderer.color.r, m_TargetSpriteRenderer.color.g, m_TargetSpriteRenderer.color.b, m_TargetSpriteRenderer.color.a);
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            //Set target image colour as new colour value
            m_NextColour.a = Mathf.Lerp(m_InitialAlpha, m_TargetAlpha, m_Timer/m_Duration);
            m_TargetSpriteRenderer.color = m_NextColour;

            //Once the time passed exceeds the assigned duration, end this effect
            if (m_Timer > m_Duration)
            {
                //Set the targetimage as the actual targetted colour
                m_NextColour.a = m_TargetAlpha;
                m_TargetSpriteRenderer.color = m_NextColour;
                m_IsFinished = true;
            }

            return m_IsFinished;
        }
#if UNITY_EDITOR

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            FadeToAlphaSpriteRenderer t = go.AddComponent<FadeToAlphaSpriteRenderer>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetSpriteRenderer, out t.m_TargetAlpha, out t.m_Duration);
            return t;
        }

        public void SetUp(SpriteRenderer t1, float t2, float t3)
        {
            m_TargetSpriteRenderer = t1;

            m_TargetAlpha = t2;
            m_Duration = t3;
        }

        public void UnPack(out SpriteRenderer t1, out float t2, out float t3)
        {
            t1 = m_TargetSpriteRenderer;

            t2 = m_TargetAlpha;
            t3 = m_Duration;
        } 
#endif


    }
}