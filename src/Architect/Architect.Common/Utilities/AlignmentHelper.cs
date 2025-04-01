using Architect.Common.Models;

namespace Architect.Common.Utilities;

public static class AlignmentHelper
{
    public static Vector2 CenterX(Size parent, Size child) =>
        new((parent.Width - child.Width) / 2, 0);

    public static Vector2 CenterY(Size parent, Size child) =>
        new(0, (parent.Height - child.Height) / 2);

    public static Vector2 Right(Size parent, Size child) => new(parent.Width - child.Width, 0);

    public static Vector2 Bottom(Size parent, Size child) => new(0, parent.Height - child.Height);
}
