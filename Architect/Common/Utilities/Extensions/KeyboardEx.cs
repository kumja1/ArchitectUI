using Cosmos.System;

public static class KeyboardEx
{

    public static List<ConsoleKeyEx> PressedKeys { get; private set; } = [];


    /// <summary>
    /// Attempts to read a key, returns true if a key is pressed.
    /// </summary>
    /// <param name="Key">Key read, if key is available.</param>
    /// <returns>True when key is read.</returns>
    public static bool TryReadKey(out KeyEvent Key)
    {
        if (KeyboardManager.TryReadKey(out Key))
        {
            if (Key.Type == KeyEvent.KeyEventType.Break)
                PressedKeys.Remove(Key.Key);
            else
                PressedKeys.Add(Key.Key);

            return true;
        }

        Key = default;
        return false;
    }

    /// <summary>
    /// A non-blocking key read method.
    /// </summary>
    /// <returns>The currently pressed key, or null if none is pressed.</returns>
    public static KeyEvent? ReadKey()
    {
        if (TryReadKey(out KeyEvent Key))
        {
            return Key;
        }

        return null;
    }
}