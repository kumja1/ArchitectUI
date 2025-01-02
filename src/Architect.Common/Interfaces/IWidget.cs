using System.Drawing;
using Architect.Common.Models;
using Size = Architect.Common.Models.Size;

namespace Architect.Common.Interfaces
{
    public interface IWidget : IDisposable
    {
        IDrawingContext Context { get; set; }
        bool IsVisible { get; }
        Size Size { get; set; }
        Vector2 Position { get; set; }
        Color BackgroundColor { get; set; }
        IWidget Content { get; set; }
        int ZIndex { get; set; }
        void OnAttachToWidget(IDrawingContext context);
        void OnDetachFromWidget();
        void BeginDraw();
        void Draw();
        void MarkDirty(bool dirty);
    }
}