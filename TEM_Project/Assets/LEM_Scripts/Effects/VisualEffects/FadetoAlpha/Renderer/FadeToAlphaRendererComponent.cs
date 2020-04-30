using UnityEngine;
namespace LEM_Effects
{
    public class FadeToAlphaRendererComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The renderers you want to fade")]
        [SerializeField] Renderer m_TargetRenderer = default;

        //Cached alpha values
        float[] m_InitialAlphas = default;
        Color[] m_NextColours = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want all the renderers' materials to have at the end of the fade")]
        [SerializeField] float m_TargetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for all the sprites' fade to be complete")]
        [SerializeField] float m_Duration = default;

        //How much time passed since this effect was started
        float m_Timer = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        public override void Initialise()
        {

            m_InitialAlphas = new float[m_TargetRenderer.materials.Length];
            m_NextColours = new Color[m_TargetRenderer.materials.Length];

            for (int i = 0; i < m_TargetRenderer.materials.Length; i++)
            {
                //Record initial alpha first for each of the targetimages
                m_InitialAlphas[i] = m_TargetRenderer.materials[i].color.a;
                m_NextColours[i] = new Color(m_TargetRenderer.materials[i].color.r, m_TargetRenderer.materials[i].color.g, m_TargetRenderer.materials[i].color.b, m_TargetRenderer.materials[i].color.a);
            }

            //Recalculate the target alpha (convert to normalized value)
            m_TargetAlpha /= 255f;
        }

        public override bool UpdateEffect()
        {
            m_Timer += Time.deltaTime;

            //Lerp all the alphas of the images 
            for (int i = 0; i < m_TargetRenderer.materials.Length; i++)
            {
                //Set target image colour as new colour value
                m_NextColours[i].a = Mathf.Lerp(m_InitialAlphas[i], m_TargetAlpha, m_Timer);
                m_TargetRenderer.materials[i].color = m_NextColours[i];
            }

            //Once the time passed exceeds the assigned duration, end this effect
            if (m_Timer > m_Duration)
            {
                //Lerp all the alphas of the images 
                for (int i = 0; i < m_TargetRenderer.materials.Length; i++)
                {
                    //Set target image colour as new colour value
                    m_NextColours[i].a = m_TargetAlpha;
                    m_TargetRenderer.materials[i].color = m_NextColours[i];
                }

                return true;
            }

            return false;
        }

    }

}