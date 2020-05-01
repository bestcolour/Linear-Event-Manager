using System;
using UnityEngine;
#if UNITY_EDITOR

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class ReadOnlyAttribute : PropertyAttribute { }


#endif