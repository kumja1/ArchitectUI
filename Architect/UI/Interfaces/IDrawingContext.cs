using Architect.Common.Models;

namespace Architect.UI.Interfaces;

/// <summary>
/// This is the drawing context for a widget
/// </summary>
public interface IDrawingContext : IDisposable
{
    /// <summary>
    /// This is the direct ancestor of the child widget
    ///</summary>
    public IWidget Parent { get; set; }

    /// <summary>
    /// This is the widget that is being drawn
    ///</summary>
    public IWidget Child { get; set; }

    /// <summary>
    /// This is the root ancestor of both the child and parent widget
    ///</summary>
    public Window RootWindow { get; set; }

    /// <summary>
    /// This is the size of the context between the parent and child widget
    ///</summary>
    public Size Size { get; set; }
}