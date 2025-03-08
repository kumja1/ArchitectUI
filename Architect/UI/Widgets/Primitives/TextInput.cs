using Size = Architect.Common.Models.Size;
using Vector2 = Architect.Common.Models.Vector2;
using Architect.Common.Interfaces;
using Cosmos.System;
using Architect.Core.Input.Events;
using Architect.Core.Input;
using Architect.UI.Widgets.Base;
using System.Drawing;

namespace Architect.UI.Widgets.Primitives;

class TextInput : Widget, IFocusableWidget
{

    public bool IsFocused { get; private set; }

    public EventHandler<InputEvent> TextChanged;

    public TextBlock InnerTextBlock
    {
        get => GetProperty<TextBlock>(nameof(InnerTextBlock));
        set => SetProperty(nameof(InnerTextBlock), value);
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

    public int BorderFocusMultiplier { get; set; } = 2;



    public TextInput()
    {
        Position = new Vector2(0, 0);
        Size = new Size(100, 30);

        Content = new Border
        {
            OutlineThickness = new Size(BorderThickness, BorderThickness),
            BackgroundColor = BackgroundColor,
            OutlineColor = BorderColor,
            Content = InnerTextBlock
        };

        Bind<TextInput, int>(nameof(BorderThickness))
            .WithConverter(b => new Size(b, b))
            .To((Border)Content, nameof(Border.OutlineThickness));

        Bind<TextInput, Color>(nameof(BorderColor))
            .To((Border)Content, nameof(Border.OutlineColor));

        Bind<TextInput, Color>(nameof(BackgroundColor))
            .To((Border)Content, nameof(Border.BackgroundColor));
    }


    public override void OnAttachToWidget(IWidget parent)
    {
        base.OnAttachToWidget(parent);

        InputManager.Instance.RegisterMouseInput<MouseClickEvent>(this, InputType.MouseClick, OnMouseClick);
        InputManager.Instance.RegisterMouseInput<MouseClickOutEvent>(this, InputType.MouseClickOut, OnMouseClickOut);

        InputManager.Instance.RegisterKeyboardInput(this, [.. Enum.GetValues(typeof(ConsoleKeyEx)).Cast<ConsoleKeyEx>()], OnTextChanged);

    }

    private void OnMouseClickOut(object? sender, MouseClickOutEvent e)
    {
        BorderThickness /= BorderFocusMultiplier;
        IsFocused = false;
    }

    private void OnMouseClick(object? sender, MouseClickEvent e)
    {
        BorderThickness *= BorderFocusMultiplier;
        IsFocused = true;
    }

    private void OnTextChanged(object sender, InputEvent e)
    {
        if (e is KeyboardEvent keyboardEvent)
        {
            if (keyboardEvent.Type == KeyEvent.KeyEventType.Make)
            {
                if (keyboardEvent.Key == ConsoleKeyEx.Backspace && InnerTextBlock.Text.Length > 0)
                    InnerTextBlock.Text = InnerTextBlock.Text[0..^1];
                else if (keyboardEvent.Key == ConsoleKeyEx.Delete && InnerTextBlock.Text.Length > 0)
                    InnerTextBlock.Text = string.Empty;
                else if (keyboardEvent.Key == ConsoleKeyEx.Enter)
                    IsFocused = false;
                else if (keyboardEvent.Key == ConsoleKeyEx.Spacebar)
                    InnerTextBlock.Text += " ";
                else
                    InnerTextBlock.Text += keyboardEvent.KeyChar.ToString();

            }
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
        InnerTextBlock.Dispose();
    }

    bool IFocusableWidget.IsFocused { get => IsFocused; set => IsFocused = value; }
}
