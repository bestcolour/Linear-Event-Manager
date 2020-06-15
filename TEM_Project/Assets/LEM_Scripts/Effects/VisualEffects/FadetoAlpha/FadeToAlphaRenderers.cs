using UnityEngine;
namespace LEM_Effects
{
    //Will fade all materials on all the renderers
    public class FadeToAlphaRenderers : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<Renderer[], float, float> 
#endif
    {
        //target
        [Tooltip("The renderers you want to fade")]
        [SerializeField] Renderer[] m_TargetRenderers = default;

        //Cached alpha values
        float[][] m_InitialAlphas = default;
        Color[][] m_NextColours = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want all the renderers' materials to have at the end of the fade")]
        [SerializeField] float m_TargetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for all the sprites' fade to be complete")]
        [SerializeField] float m_Duration = default;


        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

    

        public override void OnInitialiseEffect()
        {

            m_InitialAlphas = new float[m_TargetRenderers.Length][];
            m_NextColours = new Color[m_TargetRenderers.Length][];

#if UNITY_EDITOR
            int fadeModeInt = (int)UnityEngine.Rendering.RenderQueue.Geometry; 
#endif

            for (int i = 0; i < m_TargetRenderers.Length; i++)
            {
                m_InitialAlphas[i] = new float[m_TargetRenderers[i].materials.Length];
                m_NextColours[i] = new Color[m_TargetRenderers[i].materials.Length];

                for (int r = 0; r < m_TargetRenderers[i].materials.Length; r++)
                {
                    //Record initial alpha first for each of the targetimages
                    m_InitialAlphas[i][r] = m_TargetRenderers[i].materials[r].color.a;
#if UNITY_EDITOR
                    Debug.Assert(
                        m_TargetRenderers[i].materials[r].renderQueue != fadeModeInt,
                        "Material " + m_TargetRenderers[i].materials[r] + " is not set to Fade/Transparent Rendering Mode",
                        m_TargetRenderers[i].materials[r]);
#endif
                    m_NextColours[i][r] = new Color(m_TargetRenderers[i].materials[r].color.r, m_TargetRenderers[i].materials[r].color.g, m_TargetRenderers[i].materials[r].color.b, m_TargetRenderers[i].materials[r].color.a);
                }

            }

            //Recalculate the target alpha (convert to normalized value)
            m_TargetAlpha /= 255f;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;
            delta = m_Timer / m_Duration;

            //Lerp all the alphas of the images 
            for (int i = 0; i < m_TargetRenderers.Length; i++)
            {
                for (int r = 0; r < m_TargetRenderers[i].materials.Length; r++)
                {
                    //Set target image colour as new colour value
                    m_NextColours[i][r].a = Mathf.Lerp(m_InitialAlphas[i][r], m_TargetAlpha, delta);
                    m_TargetRenderers[i].materials[r].color = m_NextColours[i][r];
                }
            }

            //Once the time passed exceeds the assigned duration, end this effect
            if (m_Timer > m_Duration)
            {
                //Lerp all the alphas of the images 
                for (int i = 0; i < m_TargetRenderers.Length; i++)
                {
                    for (int r = 0; r < m_TargetRenderers[i].materials.Length; r++)
                    {
                        //Set target image colour as new colour value
                        m_NextColours[i][r].a = m_TargetAlpha;
                        m_TargetRenderers[i].materials[r].color = m_NextColours[i][r];
                    }
                }

                m_IsFinished =  true;
            }

            return m_IsFinished;
        }

#if UNITY_EDITOR

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            FadeToAlphaRenderers t = go.AddComponent<FadeToAlphaRenderers>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetRenderers, out t.m_TargetAlpha, out t.m_Duration);
            return t;
        }

        public void SetUp(Renderer[] t1, float t2, float t3)
        {
            m_TargetRenderers = t1;

            m_TargetAlpha = t2;
            m_Duration = t3;
        }

        public void UnPack(out Renderer[] t1, out float t2, out float t3)
        {
            t1 = m_TargetRenderers;

            t2 = m_TargetAlpha;
            t3 = m_Duration;
        }

#endif

    }

}