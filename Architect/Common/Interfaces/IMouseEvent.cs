
using Architect.Common.Models;

namespace Architect.Common.Interfaces;

interface IMouseEvent : IEvent
{
    Vector2 Position { get; }
}