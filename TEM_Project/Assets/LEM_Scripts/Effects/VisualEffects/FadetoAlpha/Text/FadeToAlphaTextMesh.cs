using UnityEngine;
namespace LEM_Effects
{

    public class FadeToAlphaTextMesh : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<TextMesh, float, float>
#endif
    {
        //target
        [Tooltip("The text you want to fade")]
        [SerializeField] TextMesh m_TargetTextMesh = default;

        //Cached alpha values
        [SerializeField, ReadOnly]
        float m_InitialAlpha = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want the text to have at the end of the fade")]
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
            m_InitialAlpha = m_TargetTextMesh.color.a;

            //Recalculate the target alpha (convert to normalized value)
            m_TargetAlpha /= 255f;

            m_NextColour = new Color(m_TargetTextMesh.color.r, m_TargetTextMesh.color.g, m_TargetTextMesh.color.b, m_TargetTextMesh.color.a);
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            //Set target image colour as new colour value
            m_NextColour.a = Mathf.Lerp(m_InitialAlpha, m_TargetAlpha, m_Timer / m_Duration);
            m_TargetTextMesh.color = m_NextColour;

            //Once the time passed exceeds the assigned duration, end this effect
            if (m_Timer > m_Duration)
            {
                //Set the targetimage as the actual targetted colour
                m_NextColour.a = m_TargetAlpha;
                m_TargetTextMesh.color = m_NextColour;
                m_IsFinished = true;
            }

            return m_IsFinished;
        }

#if UNITY_EDITOR  
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            FadeToAlphaTextMesh t = go.AddComponent<FadeToAlphaTextMesh>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetTextMesh, out t.m_TargetAlpha, out t.m_Duration);
            return t;
        }
        public void SetUp(TextMesh t1, float t2, float t3)
        {
            m_TargetTextMesh = t1;
            m_TargetAlpha = t2;
            m_Duration = t3;

        }

        public void UnPack(out TextMesh t1, out float t2, out float t3)
        {
            t1 = m_TargetTextMesh;
            t2 = m_TargetAlpha;
            t3 = m_Duration;

        }
#endif


    }
}