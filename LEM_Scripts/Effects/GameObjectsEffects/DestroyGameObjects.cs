using UnityEngine;


namespace LEM_Effects
{
    [AddComponentMenu("")] public class  DestroyGameObjects : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<GameObject[]> 
#endif
    {
        [Tooltip("Object to destroy")]
        [SerializeField] GameObject[] m_TargetObjects = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

      
        public override void OnInitialiseEffect()
        {
            //Destroy the targetted objects
            for (int i = 0; i < m_TargetObjects.Length; i++)
            {
                GameObject.Destroy(m_TargetObjects[i]);
            }
        }

#if UNITY_EDITOR
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            DestroyGameObjects t = go.AddComponent<DestroyGameObjects>();

            t.CloneBaseValuesFrom(this);
            UnPack(out m_TargetObjects);
            return t;
        }


        public void SetUp(GameObject[] t1)
        {
            m_TargetObjects = t1;
        }

        public void UnPack(out GameObject[] t1)
        {
            t1 = m_TargetObjects;
        } 
#endif
    }

   
}

