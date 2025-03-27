namespace Architect.UI.Widgets.Binding.Interfaces;

public interface IBindable
{
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event Action<string, object> PropertyChanged;

    public T? GetProperty<T>(string propertyName, T? defaultValue = default);

    /// <summary>
    /// Sets the property value.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="associatedBinding">The binding associated with the property.</param>
    public void SetProperty<T>(string propertyName, T value, IBinding? associatedBinding = null)
        where T : notnull;
}
