using System;

//Namespace can be removed if u wish to use the extensions for ur own purpose :D
//and feel free to add to it!
namespace LEM_Effects.Extensions
{

    public static class TextExtensions
    {
        public static bool CaseInsensitiveContains(this string text, string value,
             StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) >= 0;
        }

    }

}