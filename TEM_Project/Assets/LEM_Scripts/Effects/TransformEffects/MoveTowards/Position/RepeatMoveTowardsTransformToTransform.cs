using UnityEngine;
namespace LEM_Effects
{
    //This move has no stop. It will keep moving until you use Stop Repeat event
    public class RepeatMoveTowardsTransformToTransform : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<Transform, Transform, float, float> 
#endif
    {
        [Tooltip("The transform you want to lerp repeatedly")]
        [SerializeField] Transform m_TargetTransform = default;

        [Tooltip("The position you want to lerp to")]
        [SerializeField] Transform m_TargetDestination = default;

        [Tooltip("The time needed for target to reach target position with lerp. Not recommended for constant speed movement.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_Speed = 1f;

        [Tooltip("This is the distance between the target transform and the target position for the target transform to be considered at the targetposition.")]
        [SerializeField, Range(0.0001f, 1000f)] float m_SnapDistance = 1f;


        Vector3 m_InitialPosition = default;

        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateEffect;


        public override void OnInitialiseEffect()
        {
            //Calculate speed in initialise
            m_InitialPosition = m_TargetTransform.position;
        }

        public override bool OnUpdateEffect(float delta)
        {

            //meanwhile, lerp the transform to the target
            m_TargetTransform.position = Vector3.MoveTowards(m_TargetTransform.position, m_TargetDestination.position, m_Speed * delta);

            //if sqr distance between target transform and targetpos is less than snapping dist^2, 
            if (Vector3.SqrMagnitude(m_TargetDestination.position - m_TargetTransform.position) < m_SnapDistance * m_SnapDistance)
            {
                //Snap the position to the targetposition
                m_TargetTransform.position = m_InitialPosition;
            }

            return m_IsFinished;
        }
#if UNITY_EDITOR
        public void SetUp(Transform t1, Transform t2, float t3, float t4)
        {
            m_TargetTransform = t1;
            m_TargetDestination = t2;
            m_Speed = t3;
            m_SnapDistance = t4;
        }

        public void UnPack(out Transform t1, out Transform t2, out float t3, out float t4)
        {
            t1 = m_TargetTransform;
            t2 = m_TargetDestination;
            t3 = m_Speed;
            t4 = m_SnapDistance;

        } 
#endif


    }
}