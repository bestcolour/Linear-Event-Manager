using UnityEngine;
using TMPro;
namespace LEM_Effects
{

    public class FadeToAlphaTextMeshProComponent : LEM_BaseEffect,IEffectSavable<TextMeshPro,float,float>
    {
        //target
        [Tooltip("The TextMeshPro you want to fade")]
        [SerializeField] TextMeshPro m_TargetText = default;

        //Cached alpha values
        [SerializeField, ReadOnly]
        float m_InitialAlpha = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want the TextMeshPro to have at the end of the fade")]
        [SerializeField] float m_TargetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for the fade to be complete")]
        [SerializeField] float m_Duration = default;

        //How much time passed since this effect was started
        [SerializeField, ReadOnly]
        float m_Time = default;

        [SerializeField, ReadOnly]
        Color m_NextColour = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;

        bool m_IsFinished = false;

        public override void Initialise()
        {
            //Record initial alpha first
            m_InitialAlpha = m_TargetText.color.a;

            //Recalculate the target alpha (convert to normalized value)
            m_TargetAlpha /= 255f;

            m_NextColour = new Color(m_TargetText.color.r, m_TargetText.color.g, m_TargetText.color.b, m_TargetText.color.a);
        }

        public override bool UpdateEffect()
        {
            m_Time += Time.deltaTime;

            //Set target image colour as new colour value
            m_NextColour.a = Mathf.Lerp(m_InitialAlpha, m_TargetAlpha, m_Time);
            m_TargetText.color = m_NextColour;

            //Once the time passed exceeds the assigned duration, end this effect
            if (m_Time > m_Duration)
            {
                //Set the targetimage as the actual targetted colour
                m_NextColour.a = m_TargetAlpha;
                m_TargetText.color = m_NextColour;
                m_IsFinished = true;
            }

            return m_IsFinished;
        }
        
        public void SetUp(TextMeshPro t1, float t2, float t3)
        {
            m_TargetText = t1;
            m_TargetAlpha = t2;
            m_Duration = t3;
         
        }

        public void UnPack(out TextMeshPro t1, out float t2, out float t3)
        {
            t1 = m_TargetText;
            t2 = m_TargetAlpha;
            t3 = m_Duration;
           
        }

        public override void ForceStop()
        {
            m_IsFinished = true;
        }

    } 
}