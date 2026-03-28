using Architect.Input.Events.Mouse;
using Cosmos.Kernel.System.Mouse;

namespace Architect.Utilities.Extensions;

public static class MouseEx
{
    public static MouseButton GetButton() => MouseManager.LeftButton ? MouseButton.Left :
        MouseManager.RightButton ? MouseButton.Right :
        MouseManager.MiddleButton ? MouseButton.Middle : MouseButton.None;
}