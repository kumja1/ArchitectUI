using Architect.Common.Models;

namespace Architect.Common.Utilities;

public static class AlignmentHelper
{
    // Overloads for Size parameters.
    public static double AlignCenterX(Size parent, Size child) =>
        AlignCenterX(parent.Width, child.Width);

    public static double AlignCenterY(Size parent, Size child) =>
        AlignCenterY(parent.Height, child.Height);

    public static double AlignRight(Size parent, Size child) =>
        AlignRight(parent.Width, child.Width);

    public static double AlignBottom(Size parent, Size child) =>
        AlignBottom(parent.Height, child.Height);

    // Overloads for double parameters.
    public static double AlignCenterX(double parentWidth, double childWidth) =>
        (parentWidth - childWidth) / 2d;

    public static double AlignCenterY(double parentHeight, double childHeight) =>
        (parentHeight - childHeight) / 2d;

    public static double AlignRight(double parentWidth, double childWidth) =>
        parentWidth - childWidth;

    public static double AlignBottom(double parentHeight, double childHeight) =>
        parentHeight - childHeight;
}
