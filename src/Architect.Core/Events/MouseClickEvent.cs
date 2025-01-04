
using Architect.Common.Models;
using Cosmos.System;

namespace Architect.Core.Events;

record  MouseClickEvent(MouseState Button, Vector2 Position) : BaseEvent, IMouseEvent;


