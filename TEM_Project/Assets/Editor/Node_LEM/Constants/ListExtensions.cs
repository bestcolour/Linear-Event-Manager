using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    //Removes an element in a list efficiently but sort of scramble up the order of the elements in a list in the process
    public static void UnOrderlyRemoveAt<T>(this List<T> list, int indexToRemove)
    {
        list[indexToRemove] = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
    }

}
