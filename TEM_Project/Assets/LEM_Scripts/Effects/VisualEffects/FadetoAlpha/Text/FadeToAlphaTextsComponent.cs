using UnityEngine;
using UnityEngine.UI;
namespace LEM_Effects
{
    public class FadeToAlphaTextsComponent : UpdateBaseEffect, IEffectSavable<Text[], float, float>
    {
        //target
        [Tooltip("The texts you want to fade")]
        [SerializeField] Text[] m_TargetTexts = default;

        //Cached alpha values
        float[] m_InitialAlphas = default;
        Color[] m_NextColour = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want all the texts to have at the end of the fade")]
        [SerializeField] float m_TargetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for all the texts' fade to be complete")]
        [SerializeField] float m_Duration = default;

        //How much time passed since this effect was started
        float m_Timer = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;


        public override void OnInitialiseEffect()
        {
            m_InitialAlphas = new float[m_TargetTexts.Length];
            m_NextColour = new Color[m_TargetTexts.Length];

            for (int i = 0; i < m_TargetTexts.Length; i++)
            {
                //Record initial alpha first for each of the targetimages
                m_InitialAlphas[i] = m_TargetTexts[i].color.a;
                m_NextColour[i] = new Color(m_TargetTexts[i].color.r, m_TargetTexts[i].color.g, m_TargetTexts[i].color.b, m_TargetTexts[i].color.a);
            }

            //Recalculate the target alpha (convert to normalized value)
            m_TargetAlpha /= 255f;
        }

        public override bool OnUpdateEffect()
        {
            m_Timer += Time.deltaTime;

            //Lerp all the alphas of the images 
            for (int i = 0; i < m_TargetTexts.Length; i++)
            {
                m_NextColour[i].a = Mathf.Lerp(m_InitialAlphas[i], m_TargetAlpha, m_Timer);
                //Set target image colour as new colour value
                m_TargetTexts[i].color = m_NextColour[i];
            }

            //Once the time passed exceeds the assigned duration, end this effect
            if (m_Timer > m_Duration)
            {
                for (int i = 0; i < m_TargetTexts.Length; i++)
                {
                    m_NextColour[i].a = m_TargetAlpha;
                    //Set the targetimages as the actual targetted colour
                    m_TargetTexts[i].color = m_NextColour[i];
                }
                m_IsFinished = true;
            }

            return m_IsFinished;
        }

        public void SetUp(Text[] t1, float t2, float t3)
        {
            m_TargetTexts = t1;
            m_TargetAlpha = t2;
            m_Duration = t3;

        }

        public void UnPack(out Text[] t1, out float t2, out float t3)
        {
            t1 = m_TargetTexts;
            t2 = m_TargetAlpha;
            t3 = m_Duration;

        }



    }
}