using Architect.UI.Models;

namespace Architect.UI.Utils;

public static class AlignmentHelper
{
    public static Vector2 Center(Size parent, Size child) => new(
        (parent.Width - child.Width) / 2,
        (parent.Height - child.Height) / 2
    );

    public static Vector2 TopLeft(Size parent, Size child) => new(0, 0);

    public static Vector2 TopRight(Size parent, Size child) => new(
        parent.Width - child.Width,
        0
    );

    public static Vector2 BottomLeft(Size parent, Size child) => new(
        0,
        parent.Height - child.Height
    );

    public static Vector2 BottomRight(Size parent, Size child) => new(
        parent.Width - child.Width,
        parent.Height - child.Height
    );

    public static Vector2 TopCenter(Size parent, Size child) => new(
        (parent.Width - child.Width) / 2,
        0
    );

    public static Vector2 BottomCenter(Size parent, Size child) => new(
        (parent.Width - child.Width) / 2,
        parent.Height - child.Height
    );

    public static Vector2 LeftCenter(Size parent, Size child) => new(
        0,
        (parent.Height - child.Height) / 2
    );

    public static Vector2 RightCenter(Size parent, Size child) => new(
        parent.Width - child.Width,
        (parent.Height - child.Height) / 2
    );

    public static Vector2 Right(Size parent, Size child) => new(parent.Width - child.Width, 0);

    public static Vector2 Bottom(Size parent, Size child) => new(parent.Height - child.Height, 0);

}