using Architect.UI;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.ScanMaps;

namespace Architect.Core;

class Architect
{
    private const int FrameTime = 1000 / 60;

    public static void Initialize() => Intialize(FullScreenCanvas.GetFullScreenCanvas());

    public static void Intialize(Canvas canvas)
    {
        RenderManager.Initialize(canvas);
        KeyboardManager.SetKeyLayout(new USStandardLayout());
    }

    public static void Tick()
    {
        var start = DateTime.Now;
        InputManager.Tick();
        RenderManager.Tick();

        var elasped = (DateTime.Now - start).TotalMilliseconds;
        if (elasped < FrameTime)
        {
            Thread.Sleep(FrameTime - (int)elasped);
        }
    }
}