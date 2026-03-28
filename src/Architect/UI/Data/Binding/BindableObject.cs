using System.Collections.Frozen;
using System.Reflection;

namespace Architect.UI.Data.Binding;

internal record TypeMetadata(string TypeName, FrozenDictionary<string, int> PropertyTable);

public delegate void PropertyChangedCallback(string name, object? previousValue, object value);

public abstract class BindableObject
{
    // ReSharper disable once UnusedTypeParameter
    private static class Metadata<T>
    {
        public static TypeMetadata? Instance;
    }

    internal static TypeMetadata GetTypeMetadata<T>()
    {
        if (Metadata<T>.Instance != null)
            return Metadata<T>.Instance;

        Type type = typeof(T);
        FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance |
                                            BindingFlags.DeclaredOnly);
        Dictionary<string, int> propertyTable = new(fields.Length);
        foreach (FieldInfo field in fields)
        {
            string propertyName = char.ToUpperInvariant(field.Name[1]) + field.Name[2..];
            propertyTable[propertyName] = (field.FieldHandle.Value + (4 + IntPtr.Size)).ToInt32() & 0xFFFFFF;
        }

        return Metadata<T>.Instance = new TypeMetadata(type.Name, propertyTable.ToFrozenDictionary());
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public abstract event PropertyChangedCallback PropertyChanged;

    public abstract TValue? GetProperty<TValue>(ref TValue? field, TValue? initialValue = default);

    /// <summary>
    /// Sets the property value.
    /// </summary>
    /// <typeparam name="TValue">The type of the property value.</typeparam>
    /// <param name="field">The field reference.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="name">The name of the field</param>
    public abstract void SetProperty<TValue>(ref TValue? field, TValue value, string name = "");
}