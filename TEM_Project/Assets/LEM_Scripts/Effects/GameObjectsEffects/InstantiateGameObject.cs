using UnityEngine;
namespace LEM_Effects
{
    public class InstantiateGameObject : LEM_BaseEffect
#if UNITY_EDITOR
        , IEffectSavable<GameObject, int, Vector3, Vector3, Vector3> 
#endif
    {
        [Tooltip("Object to instantiate. Usually the prefab of an object")]
        [SerializeField] GameObject m_TargetObject = default;

        [Tooltip("Number of times to instantiate this object")]
        [SerializeField] int m_NumberOfTimes = 1;

        [Tooltip("Position to instantiate the object at")]
        [SerializeField] Vector3 m_TargetPosition = Vector3.zero;

        [Tooltip("Rotation to instantiate the object at")]
        [SerializeField] Vector3 m_TargetRotation = Vector3.zero;

        [Tooltip("Scale to instantiate the object at")]
        [SerializeField] Vector3 m_TargetScale = Vector3.one;

        public override EffectFunctionType FunctionType => EffectFunctionType.InstantEffect;

#if UNITY_EDITOR
        public void SetUp(GameObject targetObject, int numberOfTimes, Vector3 targetPosition, Vector3 targetRotation, Vector3 targetScale)
        {
            m_TargetObject = targetObject;
            m_NumberOfTimes = numberOfTimes;
            m_TargetPosition = targetPosition;
            m_TargetRotation = targetRotation;
            m_TargetScale = targetScale;
        }

        public void UnPack(out GameObject targetObject, out int numberOfTimes, out Vector3 targetPosition, out Vector3 targetRotation, out Vector3 targetScale)
        {
            targetObject = m_TargetObject;
            numberOfTimes = m_NumberOfTimes;
            targetPosition = m_TargetPosition;
            targetRotation = m_TargetRotation;
            targetScale = m_TargetScale;
        }

        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            InstantiateGameObject t = go.AddComponent<InstantiateGameObject>();
            t.CloneBaseValuesFrom(this);
            UnPack(out t.m_TargetObject, out t.m_NumberOfTimes, out t.m_TargetPosition, out t.m_TargetRotation, out t.m_TargetScale);
            return t;
        }
#endif

        public override void OnInitialiseEffect()
        {
            //Create a dummy variable outside of the loop so that we dont create 
            //a new var every loop (optimise)
            GameObject instantiatedObject;

            for (int i = 0; i < m_NumberOfTimes; i++)
            {
                //Instantiate the object
                instantiatedObject = GameObject.Instantiate(m_TargetObject);

                //Set its transform components
                instantiatedObject.transform.localRotation = Quaternion.Euler(m_TargetRotation);
                instantiatedObject.transform.localScale = m_TargetScale;
                //Set position as last 
                instantiatedObject.transform.position = m_TargetPosition;

            }

        }

      
    }


}