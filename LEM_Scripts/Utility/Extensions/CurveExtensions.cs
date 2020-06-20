#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LEM_Effects.Extensions;

public static class CurveExtensions
{
    public static AnimationCurve Clone(this AnimationCurve curve)
    {
        AnimationCurve c = new AnimationCurve();
        c.keys = curve.keys;
        c.preWrapMode = curve.preWrapMode;
        c.postWrapMode = curve.postWrapMode;
        return c;
    }


}

#endif