using UnityEngine;
namespace LEM_Effects
{

    [AddComponentMenu("")] public class  FadeToAlphaCanvasGroups : TimerBasedUpdateEffect
#if UNITY_EDITOR
        , IEffectSavable<CanvasGroup[], float, float>
#endif
    {
        //target
        [Tooltip("The Graphic you want to fade")]
        [SerializeField] CanvasGroup[] m_TargetCanvasGroups = default;

        float[] m_InitialAlphas = default;

        //User inputted alpha value
        [Tooltip("The target alpha value you want the canvas group to have at the end of the fade")]
        [SerializeField] float m_TargetAlpha = default;

        //User inputted alpha value
        [Tooltip("How long it takes for the fade to be complete")]
        [SerializeField] float m_Duration = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;



        public override void OnInitialiseEffect()
        {
            m_InitialAlphas = new float[m_TargetCanvasGroups.Length];

            for (int i = 0; i < m_InitialAlphas.Length; i++)
            {
                //Record initial alpha first
                m_InitialAlphas[i] = m_TargetCanvasGroups[i].alpha;
            }
           

            //Recalculate the target alpha (convert to normalized value)
            m_TargetAlpha /= 255f;

        }

        public override bool OnUpdateEffect(float delta)
        {
            m_Timer += delta;
            delta = m_Timer / m_Duration;
            float a;

            for (int i = 0; i < m_InitialAlphas.Length; i++)
            {
                //Set target canvas grp alpha as new alpha value
                a = Mathf.Lerp(m_InitialAlphas[i], m_TargetAlpha, delta);
                m_TargetCanvasGroups[i].alpha = a;
            }

            //Once the time passed exceeds the assigned duration, end this effect
            if (m_Timer > m_Duration)
            {
                for (int i = 0; i < m_InitialAlphas.Length; i++)
                {
                    m_TargetCanvasGroups[i].alpha = m_TargetAlpha;
                }

                return true;
            }

            return m_IsFinished;
        }


#if UNITY_EDITOR
        public void SetUp(CanvasGroup[] t1, float t2, float t3)
        {
            m_TargetCanvasGroups = t1;
            m_TargetAlpha = t2;
            m_Duration = t3;

        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            FadeToAlphaCanvasGroups t = go.AddComponent<FadeToAlphaCanvasGroups>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetCanvasGroups, out t.m_TargetAlpha, out t.m_Duration);
            return t;
        }

        public void UnPack(out CanvasGroup[] t1, out float t2, out float t3)
        {
            t1 = m_TargetCanvasGroups;
            t2 = m_TargetAlpha;
            t3 = m_Duration;

        }


#endif
    }
}