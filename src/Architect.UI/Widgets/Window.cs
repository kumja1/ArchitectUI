using System.Drawing;
using Architect.UI.Enums;
using Size = Architect.Common.Models.Size;

namespace Architect.UI.Widgets;


class Window : Widget
{

    public Size MaxSize { get; init; }

    public Size MinSize { get; init; }

    public Window()
    {
        MinSize = new Size(100, 100);
        MaxSize = new Size(800, 600);
        Content = new DockPanel
        {
            Content = [
                new DockPanel.Item {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Size = new Size(0, 30),
                    Content = new Stack {
                        Content = [
                            new Button {
                                Text = "Close",
                                Size = new Size(100, 30),
                                BackgroundColor = Color.Red
                            },

                            new Button {
                                Text = "Minimize",
                                Size = new Size(100, 30),
                                BackgroundColor = Color.Yellow
                            },

                            new Button {
                                Text = "Maximize",
                                Size = new Size(100, 30),
                                BackgroundColor = Color.Green
                            }
                        ]
                    }
                },
                new DockPanel.Item  {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Content = Content
                }
            ]
        };
    }




    public Size CurrentSize
    {
        get => field;
        set
        {
            if (value.Width < MinSize.Width || value.Height < MinSize.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Size cannot be smaller than MinSize");
            }
            if (value.Width > MaxSize.Width || value.Height > MaxSize.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Size cannot be bigger than MaxSize");
            }
            field = value;
        }
    }


    public override void Draw()
    {
        throw new NotImplementedException();
    }
}
