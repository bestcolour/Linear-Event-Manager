using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;

public static class OrderedDictionaryExtensions
{
    /// <summary>
    /// Returns an array of the dictionary's keys as the key's type.
    /// </summary>
    /// <typeparam name="KeyType"></typeparam>
    /// <param name="orderedDictionary"></param>
    /// <returns></returns>
    public static KeyType[] GetIndexCollectionOfKeys<KeyType>(this OrderedDictionary orderedDictionary)
    {
        KeyType[] keyTypeArr = new KeyType[orderedDictionary.Keys.Count];
        orderedDictionary.Keys.CopyTo(keyTypeArr, 0);

        return keyTypeArr;

    }

    /// <summary>
    /// Returns an array of the dictionary's values as the value's type.
    /// </summary>
    /// <typeparam name="ValueType"></typeparam>
    /// <param name="orderedDictionary"></param>
    /// <returns></returns>
    public static ValueType[] GetIndexCollectionOfValues<ValueType>(this OrderedDictionary orderedDictionary)
    {
        ValueType[] valueTypeArr = new ValueType[orderedDictionary.Values.Count];
        orderedDictionary.Values.CopyTo(valueTypeArr, 0);

        return valueTypeArr;

    }

    /// <summary>
    /// Rearranges OrderedDictionary elements with indexes. O(n)
    /// </summary>
    /// <typeparam name="KeyType"></typeparam>
    /// <typeparam name="ValueType"></typeparam>
    /// <param name="orderedDictionary"></param>
    /// <param name="smallerInt"></param>
    /// <param name="biggerInt"></param>
    public static void RearrangeElement<KeyType, ValueType>(this OrderedDictionary orderedDictionary, int smallerInt, int biggerInt)
    {
#if UNITY_EDITOR

        if (smallerInt > orderedDictionary.Count || smallerInt < 0)
            Debug.LogError(smallerInt + " Index is out of bounds!");

        if (biggerInt > orderedDictionary.Count || biggerInt < 0)
            Debug.LogError(biggerInt + " Index is out of bounds!");

        if (smallerInt >= biggerInt)
            Debug.LogError("SmallerInt is either bigger or the same integer as BiggerInt");
#endif

        //O(n)
        //Get a copy of all the keys
        KeyType[] allKeys = orderedDictionary.GetIndexCollectionOfKeys<KeyType>();

        //Will be reusing the clonedKey and clonedValue variables
        KeyType clonedKey = allKeys[smallerInt];
        ValueType clonedValue = (ValueType)orderedDictionary[smallerInt];

        orderedDictionary.Remove(clonedKey);

        if (biggerInt == orderedDictionary.Count)
            orderedDictionary.Add(clonedKey, clonedValue);
        else
            orderedDictionary.Insert(biggerInt, clonedKey, clonedValue);

        //Since this copy of array is not changed at all from the removal of element
        clonedKey = allKeys[biggerInt];

        //Minus the bigger int since the element we are getting IS affected by removal of element
        biggerInt--;
        clonedValue = (ValueType)orderedDictionary[biggerInt];

        orderedDictionary.Remove(clonedKey);
        orderedDictionary.Insert(smallerInt, clonedKey, clonedValue);

    }



}
