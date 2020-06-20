using UnityEngine;
namespace LEM_Effects
{
    [AddComponentMenu("")] public class  BiasedProbabilityOutCome : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<float[]> 
#endif
    {
        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

        [SerializeField]
        float[] m_Probabilities = default;

        public override string GetNextNodeID()
        {
            if (!bm_NodeBaseData.HasAtLeastOneNextPointNode)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Equal Random OutCome effect doesnt have any proceeding effects to randomise to!", this);
#endif
                return null;
            }


            //So long as there is at least one non-empty outcome, this will run and it is entirely possible to return an empty ID

            float randomFloat = Random.Range(0f, 99.999f);
            float currentMaxRange = 0f;
            for (int i = 0; i < m_Probabilities.Length; i++)
            {
                currentMaxRange += m_Probabilities[i];

                //If randomfloat is more or equal to minrange, and lesser than maxrange,
                if (randomFloat >= 0f && randomFloat < currentMaxRange)
                {
                    return bm_NodeBaseData.m_NextPointsIDs[i];
                }
            }


#if UNITY_EDITOR
            Debug.LogError("Something went wrong, there should be no reason for the code to flow here");
#endif
            return null;

        }

#if UNITY_EDITOR
        public void SetUp(float[] t1)
        {
            m_Probabilities = t1;
        }

        public void UnPack(out float[] t1)
        {
            t1 = m_Probabilities;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            BiasedProbabilityOutCome t = go.AddComponent<BiasedProbabilityOutCome>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_Probabilities);
            return t;
        }
#endif
    }

}