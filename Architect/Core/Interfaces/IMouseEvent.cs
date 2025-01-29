
using Architect.Common.Models;

namespace Architect.Core.Interfaces;

interface IMouseEvent : IEvent
{
    Vector2 Position { get; }
}