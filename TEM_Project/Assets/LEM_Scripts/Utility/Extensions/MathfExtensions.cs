using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathfExtensions
{
    /// <summary>
    /// Returns i such that when i == maxInt, i returns as 0. When i goes below 0, it returns maxInt - 1
    /// </summary>
    /// <param name="i"></param>
    /// <param name="maxInt"></param>
    /// <returns></returns>
    public static int CycleInt(int i, int maxInt)
    {
        //If i is positive or 0,
        if (i >= 0)
            return i % maxInt;

        //else if it's negative 
        return maxInt-1;
    }
}
