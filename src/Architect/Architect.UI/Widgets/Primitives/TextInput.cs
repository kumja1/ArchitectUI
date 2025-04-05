using System.Drawing;
using Architect.Common.Interfaces;
using Architect.Core.Input;
using Architect.Core.Input.Events;
using Architect.UI.Data.Core;
using Architect.UI.Widgets.Base;
using Cosmos.System;
using Size = Architect.Common.Models.Size;
using Vector2 = Architect.Common.Models.Vector2;

namespace Architect.UI.Widgets.Primitives;

class TextInput : FocusableWidget
{
    public bool UnfocusOnEnter
    {
        get => GetProperty<bool>(nameof(UnfocusOnEnter));
        set => SetProperty(nameof(UnfocusOnEnter), value);
    }

    public event EventHandler<TextChangedEvent> TextChanged = delegate { };

    public new required TextBlock Content
    {
        get => GetProperty<TextBlock>(nameof(Content));
        set => SetProperty(nameof(Content), value);
    }

    public Color BorderColor
    {
        get => GetProperty<Color>(nameof(BorderColor));
        set => SetProperty(nameof(BorderColor), value);
    }

    public int BorderThickness
    {
        get => GetProperty<int>(nameof(BorderThickness));
        set => SetProperty(nameof(BorderThickness), value);
    }

    public int BorderFocusMultiplier
    {
        get => GetProperty(nameof(BorderFocusMultiplier), defaultValue: 2);
        set => SetProperty(nameof(BorderFocusMultiplier), value);
    }

    private static readonly List<ConsoleKeyEx> _lettersAndNumbers =
    [
        .. Enum.GetValues(typeof(ConsoleKeyEx))
            .Cast<ConsoleKeyEx>()
            .Where(e =>
                e
                    is not (
                        // System and function keys (should not produce text)
                        ConsoleKeyEx.Escape
                        or ConsoleKeyEx.F1
                        or ConsoleKeyEx.F2
                        or ConsoleKeyEx.F3
                        or ConsoleKeyEx.F4
                        or ConsoleKeyEx.F5
                        or ConsoleKeyEx.F6
                        or ConsoleKeyEx.F7
                        or ConsoleKeyEx.F8
                        or ConsoleKeyEx.F9
                        or ConsoleKeyEx.F10
                        or ConsoleKeyEx.F11
                        or ConsoleKeyEx.F12
                        or ConsoleKeyEx.PrintScreen
                        or ConsoleKeyEx.ScrollLock
                        or ConsoleKeyEx.PauseBreak
                        or ConsoleKeyEx.Tab
                        or ConsoleKeyEx.CapsLock
                        or ConsoleKeyEx.LShift
                        or ConsoleKeyEx.RShift
                        or ConsoleKeyEx.LCtrl
                        or ConsoleKeyEx.RCtrl
                        or ConsoleKeyEx.LAlt
                        or ConsoleKeyEx.RAlt
                        or ConsoleKeyEx.LWin
                        or ConsoleKeyEx.RWin
                        or ConsoleKeyEx.Menu
                        or // Navigation keys
                        ConsoleKeyEx.Insert
                        or ConsoleKeyEx.Home
                        or ConsoleKeyEx.PageUp
                        or ConsoleKeyEx.End
                        or ConsoleKeyEx.PageDown
                        or ConsoleKeyEx.UpArrow
                        or ConsoleKeyEx.DownArrow
                        or ConsoleKeyEx.LeftArrow
                        or ConsoleKeyEx.RightArrow
                        or
                        // Numeric keypad control keys
                        ConsoleKeyEx.NumLock
                        or ConsoleKeyEx.NumEnter
                        or
                        // Others
                        ConsoleKeyEx.Power
                        or ConsoleKeyEx.Sleep
                        or ConsoleKeyEx.Wake
                    )
            ),
    ];

    public TextInput()
    {
        Position = new Vector2(0, 0);
        Size = new Size(100, 30);

        InternalContent = new Border
        {
            OutlineThickness = new Size(BorderThickness, BorderThickness),
            BackgroundColor = BackgroundColor,
            OutlineColor = BorderColor,
            Content = Content,
        }.GetReference(out Border border);

        Bind<TextInput, int>(nameof(BorderThickness))
            .WithBindingDirection(BindingDirection.TwoWay)
            .WithConverter(converter: b => new Size(b, b), backwardConverter: s => (int)s.Width)
            .To(border, nameof(Border.OutlineThickness));

        Bind<TextInput, Color>(nameof(BorderColor))
            .WithBindingDirection(BindingDirection.TwoWay)
            .To(border, nameof(Border.OutlineColor));

        Bind<TextInput, Color>(nameof(BackgroundColor))
            .WithBindingDirection(BindingDirection.TwoWay)
            .To(border);
    }

    public override void OnAttachToWidget(IWidget parent)
    {
        base.OnAttachToWidget(parent);

        InputManager.Instance.RegisterKeyboardInput<TextInput, KeyboardPressedEvent>(
            this,
            InputType.KeyboardPressed,
            _lettersAndNumbers,
            OnTextInput
        );

        InputManager.Instance.RegisterKeyboardInput<TextInput, KeyboardPressedEvent>(
            this,
            InputType.KeyboardPressed,
            [
                ConsoleKeyEx.Backspace,
                ConsoleKeyEx.Delete,
                ConsoleKeyEx.Spacebar,
                ConsoleKeyEx.Enter,
            ],
            OnControlKey
        );
    }

    public override void OnUnfocus()
    {
        base.OnUnfocus();
        BorderThickness /= BorderFocusMultiplier;
    }

    public override void OnFocus()
    {
        base.OnFocus();
        BorderThickness *= BorderFocusMultiplier;
    }

    private void OnTextInput(object? sender, KeyboardPressedEvent e)
    {
        var oldText = Content.Text;
        Content.Text += e.KeyChar.ToString();
        TextChanged?.Invoke(this, new TextChangedEvent(Content.Text, oldText));
    }

    private void OnControlKey(object? sender, KeyboardPressedEvent e)
    {
        switch (e.Key)
        {
            case ConsoleKeyEx.Enter:
                if (UnfocusOnEnter)
                {
                    FocusManager.Instance.RemoveFocus(this);
                    return;
                }
                Content.Text += "\n";
                break;

            case ConsoleKeyEx.Backspace:
                Content.Text = Content.Text[..^1];
                break;

            case ConsoleKeyEx.Delete:
                Content.Text = Content.Text[1..];
                break;

            case ConsoleKeyEx.Spacebar:
                Content.Text += " ";
                break;
        }
    }

    public override void OnDetachFromWidget()
    {
        base.OnDetachFromWidget();
        InputManager.Instance.RemoveInput(this);
    }

    public override void Dispose()
    {
        base.Dispose();
        Content.Dispose();
    }
}
