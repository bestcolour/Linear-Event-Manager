using UnityEngine;
using UnityEngine.UI;
namespace LEM_Effects
{

    public class FadeToAlphaImagesComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The images you want to fade")]
        [SerializeField] Image[] m_TargetImages = default;

        //Cached alpha values
        float[] m_InitialAlphas = default;
        Color[] m_NextColour = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want all the images to have at the end of the fade")]
        [SerializeField] float m_TargetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for all the images' fade to be complete")]
        [SerializeField] float m_Duration = default;

        //How much time passed since this effect was started
        float m_Timer = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void Initialise()
        {
            m_InitialAlphas = new float[m_TargetImages.Length];
            m_NextColour = new Color[m_TargetImages.Length];

            for (int i = 0; i < m_TargetImages.Length; i++)
            {
                //Record initial alpha first for each of the targetimages
                m_InitialAlphas[i] = m_TargetImages[i].color.a;
                m_NextColour[i] = new Color(m_TargetImages[i].color.r, m_TargetImages[i].color.g, m_TargetImages[i].color.b, m_TargetImages[i].color.a);
            }

            //Recalculate the target alpha (convert to normalized value)
            m_TargetAlpha /=  255f;
        }

        public override bool UpdateEffect()
        {
            m_Timer += Time.deltaTime;

            //Lerp all the alphas of the images 
            for (int i = 0; i < m_TargetImages.Length; i++)
            {
                m_NextColour[i].a = Mathf.Lerp(m_InitialAlphas[i], m_TargetAlpha, m_Timer);
                //Set target image colour as new colour value
                m_TargetImages[i].color = m_NextColour[i];
            }

            //Once the time passed exceeds the assigned duration, end this effect
            if (m_Timer > m_Duration)
            {
                for (int i = 0; i < m_TargetImages.Length; i++)
                {
                    m_NextColour[i].a = m_TargetAlpha;
                    //Set the targetimages as the actual targetted colour
                    m_TargetImages[i].color = m_NextColour[i];
                }
                return true;
            }

            return false;
        }

    } 
}