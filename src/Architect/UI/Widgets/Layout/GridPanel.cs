using Cosmos.Kernel.System.Graphics;

namespace Architect.UI.Widgets.Layout;

public class GridPanel : MultiContentWidget
{
   
    public record struct RowDefinition(double Height);

    public record struct ColumnDefinition(double Width);

    public List<RowDefinition> RowDefinitions { get; set; } = [];
    public List<ColumnDefinition> ColumnDefinitions { get; set; } = [];
    
    

    public void AddRow(RowDefinition row)
    {
        RowDefinitions.Add(row);
    }

    public void AddColumn(ColumnDefinition column)
    {
        ColumnDefinitions.Add(column);
    }
    

    public override void Draw(Canvas canvas)
    {
        DrawBackground(canvas);
        foreach (Widget? item in Content)
        {
            item?.Draw(canvas);
        }
    }

    public override Size Measure(Size availableSize)
    {
        double width = 0;
        double height = 0;

        foreach (Widget item in Content)
        {
            double row = RowDefinitions[item.GridRow].Height;
            double column = ColumnDefinitions[item.GridColumn].Width;

           Size desiredSize = item.Measure(
                new Size(
                    Math.Max(0, column - Padding.Width - item.Margin.Width),
                    Math.Max(0, row - Padding.Top - item.Margin.Height)
                )
            );

            Size itemSize = desiredSize + item.Margin.Size;
            width = Math.Max(width, itemSize.Width);
            height = Math.Max(height, itemSize.Height);
        }

        return new Size(width, height) + Padding.Size;
    }

    // protected override void ArrangeContent()
    // {
    //     foreach (Item item in Content)
    //     {
    //         Vector2 position = GetCellPosition(item.Row, item.Column);
    //         item.Content?.ArrangeInternal(
    //             new Rect(
    //                 X + position.X + Padding.Left + item.Content.Margin.Left,
    //                 Y + position.Y + Padding.Top + item.Content.Margin.Top,
    //                 item.Content._mesureSize
    //             )
    //         );
    //     }
    // }

    private Vector2 GetCellPosition(int row, int column)
    {
        double x = ColumnDefinitions.Take(column).Sum(c => c.Width);
        double y = RowDefinitions.Take(row).Sum(r => r.Height);
        return new Vector2(x, y);
    }
}