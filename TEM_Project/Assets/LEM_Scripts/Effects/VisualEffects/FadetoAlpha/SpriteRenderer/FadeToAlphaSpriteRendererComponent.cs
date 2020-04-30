using UnityEngine;
namespace LEM_Effects
{

    public class FadeToAlphaSpriteRendererComponent : LEM_BaseEffect
    {
        //target
        [Tooltip("The SpriteRenderer you want to fade")]
        [SerializeField] SpriteRenderer m_TargetSpriteRenderer = default;

        //Cached alpha values
        [SerializeField, ReadOnly]
        float m_InitialAlpha = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want the SpriteRenderer to have at the end of the fade")]
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

        public override void Initialise()
        {
            //Record initial alpha first
            m_InitialAlpha = m_TargetSpriteRenderer.color.a;

            //Recalculate the target alpha (convert to normalized value)
            m_TargetAlpha /= 255f;

            m_NextColour = new Color(m_TargetSpriteRenderer.color.r, m_TargetSpriteRenderer.color.g, m_TargetSpriteRenderer.color.b, m_TargetSpriteRenderer.color.a);
        }

        public override bool UpdateEffect()
        {
            m_Time += Time.deltaTime;

            //Set target image colour as new colour value
            m_NextColour.a = Mathf.Lerp(m_InitialAlpha, m_TargetAlpha, m_Time);
            m_TargetSpriteRenderer.color = m_NextColour;

            //Once the time passed exceeds the assigned duration, end this effect
            if (m_Time > m_Duration)
            {
                //Set the targetimage as the actual targetted colour
                m_NextColour.a = m_TargetAlpha;
                m_TargetSpriteRenderer.color = m_NextColour;
                return true;
            }

            return false;
        }

    }
}