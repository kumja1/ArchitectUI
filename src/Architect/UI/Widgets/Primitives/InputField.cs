using System.Drawing;
using Architect.Input;
using Architect.Input.Events.Keyboard;
using Architect.UI.Data.Binding;
using Cosmos.Kernel.System.Graphics.Fonts;
using Cosmos.Kernel.System.Keyboard;

namespace Architect.UI.Widgets.Primitives;

public delegate void TextChangedEvent();

public class InputField : SingleContentWidget
{
    public bool UnfocusOnEnter
    {
        get => GetProperty(ref field, initialValue: true);
        set => SetProperty(ref field, value);
    }

    public event EventHandler<TextChangedEvent> TextChanged = delegate { };


    public Color BorderColor
    {
        get => GetProperty(ref field);
        set => SetProperty(ref field, value);
    }

    public int BorderThickness
    {
        get => GetProperty(ref field);
        set => SetProperty(ref field, value);
    }

    public string Text
    {
        get => GetProperty(ref field, string.Empty)!;
        set => SetProperty(ref field, value, propertyChanged: OnTextChanged);
    }

    public Color TextColor
    {
        get => GetProperty(ref field, initialValue: Color.Black);
        set => SetProperty(ref field, value);
    }

    public bool EnableTextWrapping
    {
        get => GetProperty(ref field);
        set => SetProperty(ref field, value);
    }

    public Font TextFont
    {
        get => GetProperty(ref field, PCScreenFont.DefaultFont)!;
        set => SetProperty(ref field, value);
    }

    private static readonly List<ConsoleKeyEx> _lettersAndNumbers =
    [
        .. Enum.GetValues<ConsoleKeyEx>()
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

    public InputField()
    {
        Border border = new()
        {
            OutlineThickness = new Size(BorderThickness, BorderThickness),
            BackgroundColor = BackgroundColor,
            OutlineColor = BorderColor,
            Content = new TextBlock
                { TextColor = TextColor, Text = Text, EnableTextWrapping = EnableTextWrapping, TextFont = TextFont },
        };


        Content = border;

        Bind<InputField, int>(nameof(BorderThickness))
            .WithBindingDirection(BindingDirection.TwoWay)
            .WithConverter(converter: b => new Size(b, b), backwardConverter: s => (int)s.Width)
            .To(border, nameof(Border.OutlineThickness));

        Bind<InputField, Color>(nameof(BorderColor))
            .WithBindingDirection(BindingDirection.TwoWay)
            .To(border, nameof(Border.OutlineColor));

        Bind<InputField, Color>(nameof(BackgroundColor))
            .WithBindingDirection(BindingDirection.TwoWay)
            .To(border);
    }

    protected override void OnAttachToWidget()
    {
        InputManager.Instance.RegisterKeyboardInput<InputField, KeyboardPressedEvent>(
            this,
            InputType.KeyboardPressed,
            _lettersAndNumbers,
            OnTextInput
        );

        InputManager.Instance.RegisterKeyboardInput<InputField, KeyboardPressedEvent>(
            this,
            InputType.KeyboardPressed,
            OnControlKey,
            ConsoleKeyEx.Backspace,
            ConsoleKeyEx.Delete,
            ConsoleKeyEx.Spacebar,
            ConsoleKeyEx.Enter
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
        string oldText = Content.Text;
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