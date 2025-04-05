using System.Drawing;

namespace Architect.Common.Utilities;

public static class ColorHelper
{
    public static Color GetMonoChromaticColor(Color color) => GetMonoChromaticColor(color, 0.5);

    public static Color GetMonoChromaticColor(Color color, double factor) =>
        Color.FromArgb(
            (int)((color.R * (1 - factor)) + (255 * factor)),
            (int)((color.G * (1 - factor)) + (255 * factor)),
            (int)((color.B * (1 - factor)) + (255 * factor))
        );
}
