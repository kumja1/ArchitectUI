namespace Architect.Common.Interfaces;

public interface IBindable
{
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event Action<string, object> PropertyChanged;

    public T? GetProperty<T>(string propertyName, T? defaultValue = default);

    public void SetProperty<T>(string propertyName, T value)
        where T : notnull;
}
