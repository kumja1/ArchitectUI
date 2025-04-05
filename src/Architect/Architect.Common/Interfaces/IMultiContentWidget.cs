namespace Architect.Common.Interfaces;

public interface IMultiContentWidget : IWidget
{
    /// <summary>
    /// Gets the collection of child widgets.
    /// </summary>
    List<IWidget> Content { get; }
}
