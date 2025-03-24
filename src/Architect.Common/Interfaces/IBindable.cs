namespace Architect.Common.Interfaces;

public interface IBindable
{
    public event Action<string, object> PropertyChanged;

    public void OnPropertyChanged(string propertyName, object currentValue, object value);

    public T GetProperty<T>(string propertyName, T defaultValue = default);

    public void SetProperty<T>(string propertyName, T value);
}
