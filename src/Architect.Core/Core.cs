
using Architect.Common.Models;
using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.ScanMaps;

namespace Architect.Core;

class Core
{

    public InputManager InputManager { get; set; }

    private const int TargetFps = 60;
    private const int FrameTime = 1000 / TargetFps;

    public RenderManager RenderManager { get; set; }

    public void Initialize() => Intialize(FullScreenCanvas.GetFullScreenCanvas());
    public void Intialize(Canvas canvas)
    {
        KeyboardManager.SetKeyLayout(new USStandardLayout());
        InputManager = new InputManager();
        RenderManager = new RenderManager(canvas);
    }

    public void Tick()
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