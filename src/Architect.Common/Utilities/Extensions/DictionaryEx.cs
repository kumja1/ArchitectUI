namespace Architect.Common.Utilities.Extensions;

public static class DictionaryEx
{

    public static T GetOrAddValue<T>(this Dictionary<string, object> dict, string key, T defaultValue)
    {
        if (dict.TryGetValue(key, out var value))
            return (T)value;

        dict.Add(key, defaultValue);
        return defaultValue;
    }
}