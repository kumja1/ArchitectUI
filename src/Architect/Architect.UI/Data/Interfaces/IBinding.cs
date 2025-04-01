using Architect.UI.Data.Core;

namespace Architect.UI.Data.Interfaces;

public interface IBinding : IDisposable
{
    BindingDirection Direction { get; }
    bool IsTwoWay { get; }
    string SourcePropertyName { get; }
    string TargetPropertyName { get; }
}
