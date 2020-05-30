using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using System.Reflection;

public static class UnityEventExtensions
{
    /// <summary>
    /// Clones the specified unity event list.
    /// </summary>
    /// <param name="ev">The unity event.</param>
    /// <returns>Cloned UnityEvent</returns>
    public static T Clone<T>(this T ev) where T : UnityEventBase
    {
        return DeepCopy(ev);
    }

    /// <summary>
    /// Perform a deep copy of the class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <returns>A deep copy of obj.</returns>
    /// <exception cref="System.ArgumentNullException">Object cannot be null</exception>
    public static T DeepCopy<T>(T obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException("Object cannot be null");
        }
        return (T)DoCopy(obj);
    }

    /// <summary>
    /// Does the copy.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentException">Unknown type</exception>
    static object DoCopy(object obj)
    {

        if (obj == null)
        {
            return null;
        }

        Type objType = obj.GetType();


        // Value type
        if (objType.IsValueType || objType == typeof(string))
        {
            return obj;
        }

        // Array
        else if (objType.IsArray)
        {
            Type elementType = objType.GetElementType();

            Array array = obj as Array;

            Array newInstance = Array.CreateInstance(elementType, array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                //Deep copy
                newInstance.SetValue(DoCopy(array.GetValue(i)), i);
            }

            return Convert.ChangeType(newInstance, obj.GetType());
        }

        // Unity Object
        else if (typeof(UnityEngine.Object).IsAssignableFrom(objType))
        {
            return obj;
        }

        // Class -> Recursion
        else if (objType.IsClass)
        {
            var copy = Activator.CreateInstance(obj.GetType());

            var fields = objType.GetAllFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                var fieldValue = field.GetValue(obj);
                if (fieldValue != null)
                {
                    field.SetValue(copy, DoCopy(fieldValue));
                }
            }

            return copy;
        }

        // Fallback
        else
        {
            throw new ArgumentException("Unknown type");
        }
    }

    /// <summary>
    /// Gets all fields from an object and its hierarchy inheritance.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="flags">The flags.</param>
    /// <returns>All fields of the type.</returns>
    static List<FieldInfo> GetAllFields(this Type type, BindingFlags flags)
    {
        // Early exit if Object type
        if (type == typeof(System.Object))
        {
            return new List<FieldInfo>();
        }

        // Recursive call
        List<FieldInfo> fields = type.BaseType.GetAllFields(flags);
        fields.AddRange(type.GetFields(flags | BindingFlags.DeclaredOnly));
        return fields;
    }




}
