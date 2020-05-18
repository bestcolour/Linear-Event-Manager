using UnityEngine;
namespace LEM_Effects
{
    public class FadeToAlphaRendererComponent : LEM_BaseEffect, IEffectSavable<Renderer, float, float>
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

        bool m_IsFinished = false;

        public override void Initialise()
        {

            m_InitialAlphas = new float[m_TargetRenderer.materials.Length];
            m_NextColours = new Color[m_TargetRenderer.materials.Length];
            int fadeModeInt = (int)UnityEngine.Rendering.RenderQueue.Geometry;

            for (int i = 0; i < m_TargetRenderer.materials.Length; i++)
            {
                //Record initial alpha first for each of the targetimages
                m_InitialAlphas[i] = m_TargetRenderer.materials[i].color.a;

#if UNITY_EDITOR
                Debug.Assert(
                    m_TargetRenderer.materials[i].renderQueue != fadeModeInt,
                    "Material " + m_TargetRenderer.materials[i] + " is not set to Fade/Transparent Rendering Mode",
                     m_TargetRenderer.materials[i]);
#endif

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

                m_IsFinished = true;
            }

            return m_IsFinished;
        }

        public void SetUp(Renderer t1, float t2, float t3)
        {
            m_TargetRenderer = t1;
            m_TargetAlpha = t2;
            m_Duration = t3;
        }

        public void UnPack(out Renderer t1, out float t2, out float t3)
        {
            t1 = m_TargetRenderer;
            t2 = m_TargetAlpha;
            t3 = m_Duration;
        }

        public override void ForceStop()
        {
            m_IsFinished = true;
        }

    }

}