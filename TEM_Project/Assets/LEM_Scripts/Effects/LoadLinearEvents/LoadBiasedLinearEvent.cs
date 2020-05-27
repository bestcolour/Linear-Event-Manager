using UnityEngine;
namespace LEM_Effects
{
    public class LoadBiasedLinearEvent : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<LinearEvent[], float[]> 
#endif
    {
        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        [SerializeField]
        float[] m_Probabilities = default;

        [SerializeField]
        LinearEvent[] m_TargetLinearEvent = default;

        public override void OnInitialiseEffect()
        {
            float randomFloat = Random.Range(0f, 99.999f);
            float currentMaxRange = 0f;
            for (int i = 0; i < m_Probabilities.Length; i++)
            {
                currentMaxRange += m_Probabilities[i];

                //If randomfloat is more or equal to minrange, and lesser than maxrange,
                if (randomFloat >= 0f && randomFloat < currentMaxRange)
                {
                    LinearEventsManager.LoadLinearEvent(m_TargetLinearEvent[i]);
                    return;
                }
            }


#if UNITY_EDITOR
            Debug.LogError("Something went wrong, there should be no reason for the code to flow here");
#endif
        }

        public void SetUp(LinearEvent[] t1, float[] t2)
        {
            m_TargetLinearEvent = t1;
            m_Probabilities = t2;

        }

        public void UnPack(out LinearEvent[] t1, out float[] t2)
        {
            t1 = m_TargetLinearEvent;
            t2 = m_Probabilities;

        }
    }

}