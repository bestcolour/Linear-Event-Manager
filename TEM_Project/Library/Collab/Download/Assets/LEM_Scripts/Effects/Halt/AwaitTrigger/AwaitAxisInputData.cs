using UnityEngine;
namespace LEM_Effects
{

    [System.Serializable]
    public class AwaitAxisInputData : ScriptableObject
    {
        [System.Serializable]
        public struct AxisData
        {
            public string axisName;
            public float value;

            public AxisData(string axisName, float value)
            {
                this.axisName = axisName;
                this.value = value;
            }

        }

        [SerializeField, Header("Check More Than")]
        public AxisData[] m_MoreThanAxises = default;

        [SerializeField, Header("Check Less Than")]
        public AxisData[] m_LessThanAxises = default;

        [SerializeField, Header("Check ApproximatelyEqual To")]
        public AxisData[] m_ApproxEqualToAxises = default;


        public AwaitAxisInputData(AxisData[] moreThanAxises, AxisData[] lessThanAxises, AxisData[] approxEqualToAxises)
        {
            m_MoreThanAxises = moreThanAxises;
            m_LessThanAxises = lessThanAxises;
            m_ApproxEqualToAxises = approxEqualToAxises;
        }



    }

}