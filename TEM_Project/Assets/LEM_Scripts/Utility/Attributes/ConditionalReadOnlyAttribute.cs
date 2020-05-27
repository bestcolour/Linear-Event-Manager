using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class ConditionalReadOnlyAttribute : PropertyAttribute
{
    public string m_ConditionalFieldPropPath = default;
    public bool m_ConditionToMeet = default;
    public bool m_IsFullPropertyPath = default;

    //Set isFullPropertyPath to be false if the conditionalfield is not on the same propertypath level as the field u use this attribute on
    public ConditionalReadOnlyAttribute(string propertyPath, bool isFullPropertyPath = true)
    {
        m_ConditionalFieldPropPath = propertyPath;
        m_ConditionToMeet = true;
        m_IsFullPropertyPath = isFullPropertyPath;
    }

    public ConditionalReadOnlyAttribute(string propertyPath, bool conditionToMeet, bool isFullPropertyPath = true)
    {
        m_ConditionalFieldPropPath = propertyPath;
        m_ConditionToMeet = conditionToMeet;
        m_IsFullPropertyPath = isFullPropertyPath;
    }
}

