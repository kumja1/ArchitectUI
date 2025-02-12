using Architect.Common.Models;
using Cosmos.System.Graphics;

namespace Architect.Common.Interfaces;

    public interface IWidget : IDisposable
    {



        /// <summary>
        /// Gets or sets the drawing context associated with the widget.
        /// </summary>
        IDrawingContext Context { get; set; }

        /// <summary>
        /// Gets a value indicating whether the widget is visible.
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Gets or sets the size of the widget.
        /// </summary>
        Size Size { get; set; }

        /// <summary>
        /// Gets or sets the position of the widget.
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the z-index of the widget for layering purposes.
        /// </summary>
        int ZIndex { get; set; }

        /// <summary>
        /// Method called when the widget is attached to a drawing context.
        /// </summary>
        /// <param name="context">The drawing context to which the widget is attached.</param>
        void OnAttachToWidget(IDrawingContext context);

        /// <summary>
        /// Method called when the widget is detached from its drawing context.
        /// </summary>
        void OnDetachFromWidget();

        /// <summary>
        /// Prepares the widget for drawing operations.
        /// </summary>
        void BeginDraw(Canvas canvas);

        /// <summary>
        /// Draws the widget.
        /// </summary>
        void Draw(Canvas canvas);

        /// <summary>
        /// Marks the widget as dirty, indicating it needs to be redrawn.
        /// </summary>
        /// <param name="dirty">A boolean value indicating whether the widget is dirty.</param>
        void MarkDirty(bool dirty);

        /// <summary>
        /// Determines whether a specified position hits the widget.
        /// </summary>
        /// <param name="position">The position to test.</param>
        /// <returns><c>true</c> if the position hits the widget; otherwise, <c>false</c>.</returns>
        bool HitTest(Vector2 position);
    }
