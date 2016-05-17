using System;
using System.ComponentModel;
using System.Linq;

public static class Util
{
    public static T Parse<T>(object value)
    {
        try { return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value == null ? null : value.ToString()); }
        catch { return default(T); }
    }
}
