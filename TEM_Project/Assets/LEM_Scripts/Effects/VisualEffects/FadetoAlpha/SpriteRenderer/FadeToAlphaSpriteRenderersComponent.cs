using UnityEngine;
namespace LEM_Effects
{

    public class FadeToAlphaSpriteRenderersComponent : UpdateBaseEffect, IEffectSavable<SpriteRenderer[], float, float>
    {
        //target
        [Tooltip("The SpriteRenderers you want to fade")]
        [SerializeField] SpriteRenderer[] m_SpriteRenderers = default;

        //Cached alpha values
        float[] m_InitialAlphas = default;
        Color[] m_NextColour = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want all the SpriteRenderers to have at the end of the fade")]
        [SerializeField] float m_TargetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for all the SpriteRenderers' fade to be complete")]
        [SerializeField] float m_Duration = default;

        //How much time passed since this effect was started
        float m_Timer = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void OnInitialiseEffect()
        {
            m_InitialAlphas = new float[m_SpriteRenderers.Length];
            m_NextColour = new Color[m_SpriteRenderers.Length];

            for (int i = 0; i < m_SpriteRenderers.Length; i++)
            {
                //Record initial alpha first for each of the targetimages
                m_InitialAlphas[i] = m_SpriteRenderers[i].color.a;
                m_NextColour[i] = new Color(m_SpriteRenderers[i].color.r, m_SpriteRenderers[i].color.g, m_SpriteRenderers[i].color.b, m_SpriteRenderers[i].color.a);
            }

            //Recalculate the target alpha (convert to normalized value)
            m_TargetAlpha /= 255f;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += Time.deltaTime;

            //Lerp all the alphas of the images 
            for (int i = 0; i < m_SpriteRenderers.Length; i++)
            {
                m_NextColour[i].a = Mathf.Lerp(m_InitialAlphas[i], m_TargetAlpha, m_Timer);
                //Set target image colour as new colour value
                m_SpriteRenderers[i].color = m_NextColour[i];
            }

            //Once the time passed exceeds the assigned duration, end this effect
            if (m_Timer > m_Duration)
            {
                for (int i = 0; i < m_SpriteRenderers.Length; i++)
                {
                    m_NextColour[i].a = m_TargetAlpha;
                    //Set the targetimages as the actual targetted colour
                    m_SpriteRenderers[i].color = m_NextColour[i];
                }
                m_IsFinished = true;
            }

            return m_IsFinished;
        }

        public void SetUp(SpriteRenderer[] t1, float t2, float t3)
        {
            m_SpriteRenderers = t1;

            m_TargetAlpha = t2;
            m_Duration = t3;
        }

        public void UnPack(out SpriteRenderer[] t1, out float t2, out float t3)
        {
            t1 = m_SpriteRenderers;

            t2 = m_TargetAlpha;
            t3 = m_Duration;
        }


    }
}