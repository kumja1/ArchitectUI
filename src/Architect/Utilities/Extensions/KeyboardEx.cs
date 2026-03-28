using Cosmos.Kernel.System.Keyboard;

namespace Architect.Utilities.Extensions;

public static class KeyboardEx
{
    private static KeyEvent? _lastKeyEvent;

    /// <summary>
    /// Attempts to read a key, returns true if a key is pressed.
    /// </summary>
    /// <param name="key">Key read, if key is available.</param>
    /// <returns>True when key is read.</returns>
    private static bool TryReadKey(out KeyEvent? key)
    {
        if (KeyboardManager.TryReadKey(out key))
        {
            _lastKeyEvent = key;
            return true;
        }

        key = null;
        return false;
    }

    /// <summary>
    /// A non-blocking key read method.
    /// </summary>
    /// <returns>The currently pressed key, or null if none is pressed.</returns>
    public static KeyEvent? ReadKey() => TryReadKey(out KeyEvent? key) ? key : null;

    /// <summary>
    /// Determines if a specific key is currently being pressed.
    /// </summary>
    /// <param name="key">The key to check for its pressed state.</param>
    /// <returns>True if the specified key is being pressed; otherwise, false.</returns>
    public static bool IsKeyBeingPressed(ConsoleKeyEx key) =>
        TryReadKey(out KeyEvent? keyEvent) && keyEvent?.Key == key;

    /// <summary>
    /// Checks whether the previously registered key event corresponds to the release of the same key.
    /// </summary>
    /// <returns>True if the previous key event was a key release, otherwise false.</returns>
    public static bool CheckKeyPress()
    {
        if (_lastKeyEvent?.Type != KeyEvent.KeyEventType.Make)
            return false;

        if (TryReadKey(out KeyEvent? current) && current?.Key == _lastKeyEvent.Key)
            return current.Type == KeyEvent.KeyEventType.Break;

        return false;
    }
}