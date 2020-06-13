using UnityEngine;
using TMPro;
namespace LEM_Effects
{

    public class FadeToAlphaTextMeshProGUI : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<TextMeshProUGUI, float, float> 
#endif
    {
        //target
        [Tooltip("The text you want to fade")]
        [SerializeField] TextMeshProUGUI m_TargetTextMeshProUGUI = default;

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
            m_InitialAlpha = m_TargetTextMeshProUGUI.color.a;

            //Recalculate the target alpha (convert to normalized value)
            m_TargetAlpha /= 255f;

            m_NextColour = new Color(m_TargetTextMeshProUGUI.color.r, m_TargetTextMeshProUGUI.color.g, m_TargetTextMeshProUGUI.color.b, m_TargetTextMeshProUGUI.color.a);
        }

#if UNITY_EDITOR
        public void SetUp(TextMeshProUGUI t1, float t2, float t3)
        {
            m_TargetTextMeshProUGUI = t1;
            m_TargetAlpha = t2;
            m_Duration = t3;
        }

        public void UnPack(out TextMeshProUGUI t1, out float t2, out float t3)
        {
            t1 = m_TargetTextMeshProUGUI;
            t2 = m_TargetAlpha;
            t3 = m_Duration;
        } 
        
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            FadeToAlphaTextMeshProGUI t = go.AddComponent<FadeToAlphaTextMeshProGUI>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetTextMeshProUGUI, out t.m_TargetAlpha, out t.m_Duration);
            return t;
        }
#endif

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;

            //Set target image colour as new colour value
            m_NextColour.a = Mathf.Lerp(m_InitialAlpha, m_TargetAlpha, m_Timer/m_Duration);
            m_TargetTextMeshProUGUI.color = m_NextColour;

            //Once the time passed exceeds the assigned duration, end this effect
            if (m_Timer > m_Duration)
            {
                //Set the targetimage as the actual targetted colour
                m_NextColour.a = m_TargetAlpha;
                m_TargetTextMeshProUGUI.color = m_NextColour;
                m_IsFinished = true;
            }

            return m_IsFinished;
        }

      
    } 
}