using UnityEngine;
using UnityEngine.UI;
namespace LEM_Effects
{

    public class FadeToAlphaGraphics : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<Graphic[], float, float>
#endif
    {
        //target
        [Tooltip("The images you want to fade")]
        [SerializeField] Graphic[] m_TargetGraphics = default;

        //Cached alpha values
        float[] m_InitialAlphas = default;
        Color[] m_NextColour = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want all the images to have at the end of the fade")]
        [SerializeField] float m_TargetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for all the images' fade to be complete")]
        [SerializeField] float m_Duration = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;



        public override void OnInitialiseEffect()
        {
            m_InitialAlphas = new float[m_TargetGraphics.Length];
            m_NextColour = new Color[m_TargetGraphics.Length];

            for (int i = 0; i < m_TargetGraphics.Length; i++)
            {
                //Record initial alpha first for each of the targetimages
                m_InitialAlphas[i] = m_TargetGraphics[i].color.a;
                m_NextColour[i] = new Color(m_TargetGraphics[i].color.r, m_TargetGraphics[i].color.g, m_TargetGraphics[i].color.b, m_TargetGraphics[i].color.a);
            }

            //Recalculate the target alpha (convert to normalized value)
            m_TargetAlpha /= 255f;
        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;
            delta = m_Timer / m_Duration;

            //Lerp all the alphas of the images 
            for (int i = 0; i < m_TargetGraphics.Length; i++)
            {
                m_NextColour[i].a = Mathf.Lerp(m_InitialAlphas[i], m_TargetAlpha, delta);
                //Set target image colour as new colour value
                m_TargetGraphics[i].color = m_NextColour[i];
            }

            //Once the time passed exceeds the assigned duration, end this effect
            if (m_Timer > m_Duration)
            {
                for (int i = 0; i < m_TargetGraphics.Length; i++)
                {
                    m_NextColour[i].a = m_TargetAlpha;
                    //Set the targetimages as the actual targetted colour
                    m_TargetGraphics[i].color = m_NextColour[i];
                }
                m_IsFinished = true;
            }

            return m_IsFinished;
        }

#if UNITY_EDITOR
        public void SetUp(Graphic[] t1, float t2, float t3)
        {
            m_TargetGraphics = t1;

            m_TargetAlpha = t2;
            m_Duration = t3;
        }

        public void UnPack(out Graphic[] t1, out float t2, out float t3)
        {
            t1 = m_TargetGraphics;

            t2 = m_TargetAlpha;
            t3 = m_Duration;
        }
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            FadeToAlphaGraphics t = go.AddComponent<FadeToAlphaGraphics>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetGraphics, out t.m_TargetAlpha, out t.m_Duration);
            return t;
        }
#endif

    }
}