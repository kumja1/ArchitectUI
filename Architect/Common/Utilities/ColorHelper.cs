using System.Drawing;

namespace Architect.Common.Utilities;

static class ColorHelper
{
    public static Color GetMonoChromaticColor(Color color) => Color.FromArgb((int)((color.R * (1 - 0.5)) + (255 * 0.5)), (int)((color.G * (1 - 0.5)) + (255 * 0.5)), (int)((color.B * (1 - 0.5)) + (255 * 0.5)));

    public static Color GetMonoChromaticColor(Color color, double factor) => Color.FromArgb((int)((color.R * (1 - factor)) + (255 * factor)), (int)((color.G * (1 - factor)) + (255 * factor)), (int)((color.B * (1 - factor)) + (255 * factor)));

}