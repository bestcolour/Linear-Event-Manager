﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    /// <summary>
    /// Removes an element in a list efficiently but sort of scramble up the order of the elements in a list in the process. This is O(1) operation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="indexToRemove"></param>
    public static void RemoveEfficiently<T>(this List<T> list, int indexToRemove)
    {
        list[indexToRemove] = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
    }

    /// <summary>
    /// Rearranges list elements with indexes. Less than O(n)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="e1"></param>
    /// <param name="e2"></param>
    public static void RearrangeElement<T>(this List<T> list, int e1, int e2)
    {
#if UNITY_EDITOR
        Debug.Assert(list[e1] != null, list + " doesn't have an index " + e1 + ". Element is out of range!");
        Debug.Assert(list[e2] != null, list + " doesn't have an index " + e2 + ". Element is out of range!");
#endif

        //Clone this e1 and put it at the end of the list
        list.Add(list[e1]);

        list[e1] = list[e2];

        list[e2] = list[list.Count - 1];

        list.RemoveAt(list.Count - 1);
    }

    /// <summary>
    /// Rearranges list elements with a known Element inside the list and an index. O(n) operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="e1"></param>
    /// <param name="e2"></param>
    public static void RearrangeElement<T>(this List<T> list, T knownElement, int e2)
    {
        //Returns -1  if nothing is found
        int knownElementInt = list.IndexOf(knownElement);

#if UNITY_EDITOR
        //O(n) but wont be performed in real game build
        Debug.Assert(knownElementInt != -1, list + " doesn't have an element " + knownElement);
        Debug.Assert(list[e2] != null, list + " doesn't have an index " + e2);
#endif
        //Clone this e1 and put it at the end of the list
        list[knownElementInt] = list[e2];

        list[e2] = knownElement;
    }

    /// <summary>
    /// Rearranges list elements with known Elements inside the list. O(n) operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="e1"></param>
    /// <param name="e2"></param>
    public static void RearrangeElement<T>(this List<T> list, T knownElement1, T knownElement2)
    {
        //O(n) but wont be performed in real game build
        int knownElementInt1 = list.IndexOf(knownElement1);
        int knownElementInt2 = list.IndexOf(knownElement2);

#if UNITY_EDITOR
        //Returns -1  if nothing is found
        Debug.Assert(knownElementInt1 != -1, list + " doesn't have an element " + knownElement1);
        Debug.Assert(knownElementInt2 != -1, list + " doesn't have an element " + knownElement2);
#endif

        //Clone this e1 and put it at the end of the list
        list[knownElementInt1] = knownElement2;

        list[knownElementInt2] = knownElement1;

    }

}