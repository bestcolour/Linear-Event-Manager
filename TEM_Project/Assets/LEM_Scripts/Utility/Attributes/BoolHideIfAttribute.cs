using System;
using UnityEngine;

#if UNITY_EDITOR

//Not usable for arrays or lists
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class BoolHideIfAttribute : PropertyAttribute
{
    //The propertypath of the field you wanna use as reference to determine whether the field u placed this attribute on is to be hidden or not
    public string m_ConditionalFieldPropPath = default;
    //The bool state of the conditionalfield that you wish to refer to. If that conditionalfield has this boolean value, the field u placed this attribute on will be hidden
    public bool m_StateToObserve = default;
    //Bool to determine whether the string propertypath is the full path or just the name of the field
    public bool m_IsFullPropertyPath = default;

    //Overloads constructor for boolean hide attributes
    public BoolHideIfAttribute(string propertyPath, bool isFullPropertyPath = false)
    {
        m_ConditionalFieldPropPath = propertyPath;
        m_IsFullPropertyPath = isFullPropertyPath;
    }

    public BoolHideIfAttribute(string propertyPath, bool hideInInspector, bool isFullPropertyPath = false)
    {
        m_ConditionalFieldPropPath = propertyPath;
        m_StateToObserve = hideInInspector;
        m_IsFullPropertyPath = isFullPropertyPath;
    }

}
#endif
