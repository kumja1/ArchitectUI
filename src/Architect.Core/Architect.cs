using Architect.Core.Input;
using Architect.Core.Rendering;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.ScanMaps;

namespace Architect.Core;

public class Architect
{
    private const int FrameTime = 1000 / 60;

    public static void Initialize() => Initialize(FullScreenCanvas.GetFullScreenCanvas());

    public static void Initialize(Canvas canvas, ScanMapBase? keyboardLayout = null)
    {
        keyboardLayout ??= new USStandardLayout();
        RenderManager.Initialize(canvas);
        InputManager.Initialize(keyboardLayout);
    }

    public static void Tick()
    {
        var start = DateTime.Now;
        InputManager.Instance.Tick();
        RenderManager.Instance.Tick();

        var elasped = (DateTime.Now - start).TotalMilliseconds;
        if (elasped < FrameTime)
        {
            Thread.Sleep(FrameTime - (int)elasped);
        }
    }
}