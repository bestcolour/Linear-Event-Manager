using UnityEditor;
using UnityEngine;
namespace LEM_Effects
{

    public class AwaitAxisInput : UpdateBaseEffect
#if UNITY_EDITOR
        , IEffectSavable<LinearEvent, SerializedObject>
#endif
    {
        public override EffectFunctionType FunctionType => EffectFunctionType.UpdateHaltEffect;

        //[SerializeField]
        //AwaitAxisInputData m_AxisInputData = default;

        [SerializeField, Header("Check More Than")]
        public AwaitAxisInputData.AxisData[] m_MoreThanAxises = default;

        [SerializeField, Header("Check Less Than")]
        public AwaitAxisInputData.AxisData[] m_LessThanAxises = default;

        [SerializeField, Header("Check ApproximatelyEqual To")]
        public AwaitAxisInputData.AxisData[] m_ApproxEqualToAxises = default;

        [SerializeField]
        LinearEvent m_TargetLinearEvent = default;

        //Runtime values
        [ReadOnly,SerializeField,Header("Runtime")]
        bool m_AllInputConditionsMet = false;


#if UNITY_EDITOR
        public override LEM_BaseEffect CloneMonoBehaviour(GameObject go)
        {
            AwaitAxisInput t = go.AddComponent<AwaitAxisInput>();
            t.CloneBaseValuesFrom(this);
            t.m_TargetLinearEvent = m_TargetLinearEvent;

            int length = m_MoreThanAxises != null ? m_MoreThanAxises.Length : 0;
            t.m_MoreThanAxises = new AwaitAxisInputData.AxisData[length];

            length = m_LessThanAxises != null ? m_LessThanAxises.Length : 0;
            t.m_LessThanAxises = new AwaitAxisInputData.AxisData[length];

            length = m_ApproxEqualToAxises != null ? m_ApproxEqualToAxises.Length : 0;
            t.m_ApproxEqualToAxises = new AwaitAxisInputData.AxisData[length];


            m_MoreThanAxises?.CopyTo(t.m_MoreThanAxises, 0);
            m_LessThanAxises?.CopyTo(t.m_LessThanAxises, 0);
            m_ApproxEqualToAxises?.CopyTo(t.m_ApproxEqualToAxises, 0);

            return t;
        }

        // MAJOR MONOCHANGE
        //public override LEM_BaseEffect ScriptableClone()
        //{
        //    AwaitAxisInput newClone = ScriptableObject.CreateInstance<AwaitAxisInput>();

        //    //AwaitAxisInputData newDataInstance = ScriptableObject.CreateInstance<AwaitAxisInputData>();

        //    //int length = m_MoreThanAxises != null ? m_MoreThanAxises.Length : 0;
        //    //m_MoreThanAxises = new AwaitAxisInputData.AxisData[length];

        //    //length = m_LessThanAxises != null ? m_LessThanAxises.Length : 0;
        //    //m_LessThanAxises = new AwaitAxisInputData.AxisData[length];

        //    //length = m_ApproxEqualToAxises != null ? m_ApproxEqualToAxises.Length : 0;
        //    //m_ApproxEqualToAxises = new AwaitAxisInputData.AxisData[length];

        //    //for (int i = 0; i < m_MoreThanAxises.Length; i++)
        //    //m_MoreThanAxises[i] = m_MoreThanAxises[i];

        //    //for (int i = 0; i < m_LessThanAxises.Length; i++)
        //    //m_LessThanAxises[i] = m_LessThanAxises[i];

        //    //for (int i = 0; i < m_ApproxEqualToAxises.Length; i++)
        //    //m_ApproxEqualToAxises[i] = m_ApproxEqualToAxises[i];

        //    //newClone.m_AxisInputData = newDataInstance;

        //    int length = m_MoreThanAxises != null ? m_MoreThanAxises.Length : 0;
        //    newClone.m_MoreThanAxises = new AwaitAxisInputData.AxisData[length];

        //    length = m_LessThanAxises != null ? m_LessThanAxises.Length : 0;
        //    newClone.m_LessThanAxises = new AwaitAxisInputData.AxisData[length];

        //    length = m_ApproxEqualToAxises != null ? m_ApproxEqualToAxises.Length : 0;
        //    newClone.m_ApproxEqualToAxises = new AwaitAxisInputData.AxisData[length];


        //    m_MoreThanAxises?.CopyTo(newClone.m_MoreThanAxises,0);
        //    m_LessThanAxises?.CopyTo(newClone.m_LessThanAxises,0);
        //    m_ApproxEqualToAxises?.CopyTo(newClone.m_ApproxEqualToAxises,0);

        //    newClone.bm_NodeBaseData = bm_NodeBaseData;
        //    newClone.bm_NodeEffectType = bm_NodeEffectType;
        //    newClone.bm_UpdateCycle = bm_UpdateCycle;

        //    return newClone;
        //}

        public void SetUp(LinearEvent t1, SerializedObject t2)
        {
            m_TargetLinearEvent = t1;
            AwaitAxisInputData temp  = (AwaitAxisInputData)t2.targetObject;

            //m_MoreThanAxises = temp.m_MoreThanAxises;
            //m_LessThanAxises = temp.m_LessThanAxises;
            //m_ApproxEqualToAxises = temp.m_ApproxEqualToAxises;

            int length = temp.m_MoreThanAxises != null ? temp.m_MoreThanAxises.Length : 0;
            m_MoreThanAxises = new AwaitAxisInputData.AxisData[length];

            length = temp.m_LessThanAxises != null ? temp.m_LessThanAxises.Length : 0;
            m_LessThanAxises = new AwaitAxisInputData.AxisData[length];

            length = temp.m_ApproxEqualToAxises != null ? temp.m_ApproxEqualToAxises.Length : 0;
            m_ApproxEqualToAxises = new AwaitAxisInputData.AxisData[length];

            temp.m_MoreThanAxises?.CopyTo(m_MoreThanAxises, 0);
            temp.m_LessThanAxises?.CopyTo(m_LessThanAxises, 0);
            temp.m_ApproxEqualToAxises?.CopyTo(m_ApproxEqualToAxises, 0);

        }

        public void UnPack(out LinearEvent t1, out SerializedObject t2)
        {
            t1 = m_TargetLinearEvent;

            AwaitAxisInputData temp = ScriptableObject.CreateInstance<AwaitAxisInputData>();
            //temp.m_MoreThanAxises = m_MoreThanAxises;
            //temp.m_LessThanAxises = m_LessThanAxises;
            //temp.m_ApproxEqualToAxises = m_ApproxEqualToAxises;

            int length = m_MoreThanAxises != null ? m_MoreThanAxises.Length : 0;
            temp.m_MoreThanAxises = new AwaitAxisInputData.AxisData[length];

            length = m_LessThanAxises != null ? m_LessThanAxises.Length : 0;
            temp.m_LessThanAxises = new AwaitAxisInputData.AxisData[length];

            length = m_ApproxEqualToAxises != null ? m_ApproxEqualToAxises.Length : 0;
            temp.m_ApproxEqualToAxises = new AwaitAxisInputData.AxisData[length];

            m_MoreThanAxises?.CopyTo(temp.m_MoreThanAxises, 0);
            m_LessThanAxises?.CopyTo(temp.m_LessThanAxises, 0);
            m_ApproxEqualToAxises?.CopyTo(temp.m_ApproxEqualToAxises, 0);

            t2 = new SerializedObject(temp);
        }
#endif

        public override void OnInitialiseEffect()
        {
            m_TargetLinearEvent.AddNumberOfAwaitingInput = 1;
        }

        public override bool OnUpdateEffect(float delta)
        {
            if (m_IsFinished)
                return m_IsFinished;

            m_AllInputConditionsMet = true;

            for (int i = 0; i < m_MoreThanAxises.Length; i++)
                if (!(Input.GetAxisRaw(m_MoreThanAxises[i].axisName) > m_MoreThanAxises[i].value))
                    m_AllInputConditionsMet = false;

            for (int i = 0; i < m_LessThanAxises.Length; i++)
                if (!(Input.GetAxisRaw(m_LessThanAxises[i].axisName) < m_LessThanAxises[i].value))
                    m_AllInputConditionsMet = false;

            for (int i = 0; i < m_ApproxEqualToAxises.Length; i++)
                if (!(Mathf.Approximately(Input.GetAxisRaw(m_ApproxEqualToAxises[i].axisName), m_ApproxEqualToAxises[i].value)))
                    m_AllInputConditionsMet = false;

            //Return when
            return m_AllInputConditionsMet;
        }

       

        public override void OnEndEffect()
        {
            base.OnEndEffect();
            m_TargetLinearEvent.AddNumberOfAwaitingInput = -1;
        }

     
    }

}