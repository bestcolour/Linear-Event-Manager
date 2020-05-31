#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LEM_Effects.Extensions;

public static class CurveExtensions
{
    public static AnimationCurve Clone(this AnimationCurve curve)
    {
        return ReflectionExtensions.DeepCopy<AnimationCurve>(curve);
    }


}

#endif