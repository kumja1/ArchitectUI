
using Architect.Common.Models;
using Architect.Core.Interfaces;
using Cosmos.System;

namespace Architect.Core.Events;

record MouseClickEvent(MouseState Button, Vector2 Position) :  IMouseEvent;
