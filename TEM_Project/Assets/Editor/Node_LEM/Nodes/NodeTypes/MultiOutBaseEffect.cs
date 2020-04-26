using System.Collections.Generic;

namespace LEM_Editor
{
    public abstract class MultiOutBaseEffectNode : BaseEffectNode
    {
        public List<OutConnectionPoint> m_NumberOfExtraOutPoints = new List<OutConnectionPoint>();

        public override OutConnectionPoint[] GetOutConnectionPoints
        {
            get
            {
                OutConnectionPoint[] outConnectionPoints = new OutConnectionPoint[1+ m_NumberOfExtraOutPoints.Count];
                outConnectionPoints[0] = m_OutPoint;

                for (int i = 0; i < m_NumberOfExtraOutPoints.Count; i++)
                    outConnectionPoints[i + 1] = m_NumberOfExtraOutPoints[i];

                return outConnectionPoints;
            }
        }

        protected override string[] TryToSaveNextPointNodeID()
        {
            //Returns true value of saved state
            string[] connectedNodeIDs = new string[m_NumberOfExtraOutPoints.Count + 1];

            connectedNodeIDs[0] = m_OutPoint.GetConnectedNodeID(0);

            for (int i = 0; i < m_NumberOfExtraOutPoints.Count; i++)
                connectedNodeIDs[i + 1] = m_NumberOfExtraOutPoints[i].GetConnectedNodeID(0);

            return connectedNodeIDs;
        }

    }

}