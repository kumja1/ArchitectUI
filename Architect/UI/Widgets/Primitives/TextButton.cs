
using System.Drawing;
using Architect.UI.Base;

public class TextButton : Widget
{
    public string Text { get => field; set => SetProperty(ref field, value); }

    public Color TextColor { get => field; set => SetProperty(ref field, value); }


    public EventHandler<InputEvent> Clicked;

    public TextButton()
    {
        TextColor = Color.Black;
    }

    public override void Draw(Canvas canvas)
    {
        canvas.DrawString(Text, TextColor, Position.X, Position.Y);
    }

    public override void OnInput(InputEvent input)
    {
        if (input is MouseInputEvent mouseInput)
        {
            if (mouseInput.Type == MouseInputType.LeftButtonDown)
            {
                Clicked?.Invoke(this, input);
            }
        }
    }
}