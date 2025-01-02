using Architect.Common.Models;
using Cosmos.System.Graphics;

namespace Architect.Common.Interfaces;

public interface IDrawingContext : IDisposable
{
    public IWidget Parent { get; set; }
    public IWidget Child { get; set; }
    public Canvas Canvas { get; set; }
    public IRenderManager RenderManager { get; }
    public Size Size { get; set; }
}