using Architect.UI.Widgets.Binding.Core;

namespace Architect.UI.Widgets.Binding.Interfaces;

public interface IBinding : IDisposable
{
    void Deconstruct(
        out string sourcePropertyName,
        out string targetPropertyName,
        out BindingDirection direction
    );
}
