using System.Drawing;
using System.Runtime.CompilerServices;

namespace Architect.Utilities;

public static class ColorHelper
{
    /// <summary>
    /// Generates a monochromatic color based on the input color and a specified factor.
    /// </summary>
    /// <param name="color">The base color to apply the monochromatic calculation to.</param>
    /// <param name="factor">A value between 0 and 1 representing the intensity of the monochromatic effect.
    /// A value closer to 0 retains the original color, and a value closer to 1 blends it towards white.
    /// Defaults to 0.5 if not specified.</param>
    /// <returns>A new <see cref="Color"/> instance that represents the monochromatic color derived from the input color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color GetMonoChromaticColor(Color color, double factor = 0.5) =>
        Color.FromArgb(
            (int)(color.R * (1 - factor) + 255 * factor),
            (int)(color.G * (1 - factor) + 255 * factor),
            (int)(color.B * (1 - factor) + 255 * factor)
        );
}