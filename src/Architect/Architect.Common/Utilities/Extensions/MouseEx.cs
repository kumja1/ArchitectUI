using Cosmos.System;

namespace Architect.Common.Utilities.Extensions;

public static class MouseEx
{
    public static bool MouseClicked =>
        MouseManager.LastMouseState != MouseState.None
        && MouseManager.MouseState == MouseState.None;

    public static bool MouseDrag =>
        MouseManager.LastMouseState != MouseState.None
        && MouseManager.MouseState != MouseState.None
        && (MouseManager.DeltaX != 0 || MouseManager.DeltaY != 0);
}
