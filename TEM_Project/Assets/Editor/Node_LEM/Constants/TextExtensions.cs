using System;

public static class TextExtensions 
{
    public static bool CaseInsensitiveContains(this string text, string value,
         StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
    {
        return text.IndexOf(value, stringComparison) >= 0;
    }

}
