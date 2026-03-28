namespace Architect.Utilities.Extensions;

public static class DictionaryEx
{
    public static T GetOrAddValue<T>(
        this Dictionary<string, object> dict,
        string key,
        T defaultValue
    )
    {
        if (dict.TryGetValue(key, out object? value))
            return (T)value;

        dict.Add(key, defaultValue);
        return defaultValue;
    }
}
