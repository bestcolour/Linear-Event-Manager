using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class DictionaryExtensions
{

    /// <summary>
    /// Returns the Value which matches the predicate. O(n^2) operation.
    /// </summary>
    /// <typeparam name="KeyValue"></typeparam>
    /// <typeparam name="OutValue"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    public static OutValue Find<KeyValue, OutValue>(this Dictionary<KeyValue, OutValue> dictionary, Predicate<OutValue> match)
    {
        KeyValue[] allKeysInDictionary = dictionary.Keys.ToArray();

        for (int i = 0; i < allKeysInDictionary.Length; i++)
        {
            if (match(dictionary[allKeysInDictionary[i]]))
            {
                return dictionary[allKeysInDictionary[i]];
            }
        }

        return default;
    }

    /// <summary>
    /// Returns the Value which matches the predicate. O(n) operation.
    /// </summary>
    /// <typeparam name="KeyValue"></typeparam>
    /// <typeparam name="OutValue"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="allKeysInDictionary"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    public static OutValue Find<KeyValue, OutValue>(this Dictionary<KeyValue, OutValue> dictionary, KeyValue[] allKeysInDictionary, Predicate<OutValue> match)
    {
        for (int i = 0; i < allKeysInDictionary.Length; i++)
        {
            if (match(dictionary[allKeysInDictionary[i]]))
            {
                return dictionary[allKeysInDictionary[i]];
            }
        }

        return default;
    }

    /// <summary>
    /// Returns the Key which corresponding value matches the predicate. O(n^2) operation.
    /// </summary>
    /// <typeparam name="KeyValue"></typeparam>
    /// <typeparam name="OutValue"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    public static KeyValue FindKey<KeyValue, OutValue>(this Dictionary<KeyValue, OutValue> dictionary, Predicate<OutValue> match)
    {
        KeyValue[] allKeysInDictionary = dictionary.Keys.ToArray();

        for (int i = 0; i < allKeysInDictionary.Length; i++)
        {
            if (match(dictionary[allKeysInDictionary[i]]))
            {
                //if true
                return allKeysInDictionary[i];
            }
        }

        return default;
    }

    /// <summary>
    /// Returns the Key which corresponding value matches the predicate. O(n) operation.
    /// </summary>
    /// <typeparam name="KeyValue"></typeparam>
    /// <typeparam name="OutValue"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="allKeysInDictionary"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    public static KeyValue FindKey<KeyValue, OutValue>(this Dictionary<KeyValue, OutValue> dictionary, KeyValue[] allKeysInDictionary, Predicate<OutValue> match)
    {
        for (int i = 0; i < allKeysInDictionary.Length; i++)
        {
            if (match(dictionary[allKeysInDictionary[i]]))
            {
                return allKeysInDictionary[i];
            }
        }

        return default;
    }

    /// <summary>
    /// Converts all of dictionary's Key Type into another Type as an array. O(n^2) operation.
    /// </summary>
    /// <typeparam name="KeyValue"></typeparam>
    /// <typeparam name="OutValue"></typeparam>
    /// <typeparam name="TypeToConvertTo"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="convert"></param>
    /// <returns></returns>
    public static TypeToConvertTo[] ConvertAllKeys<KeyValue, OutValue,TypeToConvertTo >(this Dictionary<KeyValue, OutValue> dictionary, Converter<KeyValue, TypeToConvertTo> convert)
    {
        //O(n)
        KeyValue[] allKeysInDictionary = dictionary.Keys.ToArray();
        TypeToConvertTo[] convertedValueArray = new TypeToConvertTo[allKeysInDictionary.Length];

        //O(n)
        for (int i = 0; i < allKeysInDictionary.Length; i++)
        {
            convertedValueArray[i] = convert(allKeysInDictionary[i]);
        }

        return convertedValueArray;
    }

    /// <summary>
    /// Converts all of dictionary's Key Type into another Type as an array. O(n) operation.
    /// </summary>
    /// <typeparam name="KeyValue"></typeparam>
    /// <typeparam name="OutValue"></typeparam>
    /// <typeparam name="TypeToConvertTo"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="convert"></param>
    /// <returns></returns>
    public static TypeToConvertTo[] ConvertAllKeys<KeyValue, OutValue, TypeToConvertTo>(this Dictionary<KeyValue, OutValue> dictionary, KeyValue[] allKeysInDictionary, Converter<KeyValue, TypeToConvertTo> convert)
    {
        TypeToConvertTo[] convertedValueArray = new TypeToConvertTo[allKeysInDictionary.Length];

        //O(n)
        for (int i = 0; i < allKeysInDictionary.Length; i++)
        {
            convertedValueArray[i] = convert(allKeysInDictionary[i]);
        }

        return convertedValueArray;
    }


    /// <summary>
    /// Converts all of dictionary's Value Type into another Type as an array. O(n^2) operation.
    /// </summary>
    /// <typeparam name="KeyValue"></typeparam>
    /// <typeparam name="OutValue"></typeparam>
    /// <typeparam name="TypeToConvertTo"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="convert"></param>
    /// <returns></returns>
    public static TypeToConvertTo[] ConvertAllValues<KeyValue, OutValue, TypeToConvertTo>(this Dictionary<KeyValue, OutValue> dictionary, Converter<OutValue, TypeToConvertTo> convert)
    {
        //O(n)
        KeyValue[] allKeysInDictionary = dictionary.Keys.ToArray();
        TypeToConvertTo[] convertedValueArray = new TypeToConvertTo[allKeysInDictionary.Length];

        //O(n)
        for (int i = 0; i < allKeysInDictionary.Length; i++)
        {
            convertedValueArray[i] = convert(dictionary[allKeysInDictionary[i]]);
        }

        return convertedValueArray;
    }

    /// <summary>
    /// Converts all of dictionary's Value Type into another Type as an array. O(n) operation.
    /// </summary>
    /// <typeparam name="KeyValue"></typeparam>
    /// <typeparam name="OutValue"></typeparam>
    /// <typeparam name="TypeToConvertTo"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="convert"></param>
    /// <returns></returns>
    public static TypeToConvertTo[] ConvertAllValues<KeyValue, OutValue, TypeToConvertTo>(this Dictionary<KeyValue, OutValue> dictionary, KeyValue[] allKeysInDictionary, Converter<OutValue, TypeToConvertTo> convert)
    {
        TypeToConvertTo[] convertedValueArray = new TypeToConvertTo[allKeysInDictionary.Length];

        //O(n)
        for (int i = 0; i < allKeysInDictionary.Length; i++)
        {
            convertedValueArray[i] = convert(dictionary[allKeysInDictionary[i]]);
        }

        return convertedValueArray;
    }
}
