using Architect.Common.Interfaces;
using Architect.Common.Models;
using Architect.UI.Widgets.Base;
using Cosmos.System.Graphics;

namespace Architect.UI.Widgets.Layout;

public class GridPanel : MultiContentWidget
{
    public class Item
    {
        public int Row;
        public int Column;
        public Widget? Content;
    }

    public record struct RowDefinition(double Height);

    public record struct ColumnDefinition(double Width);

    public List<RowDefinition> RowDefinitions { get; set; } = [];
    public List<ColumnDefinition> ColumnDefinitions { get; set; } = [];

    public new List<Item> Content
    {
        get => GetProperty<List<Item>>(nameof(Content), defaultValue: []);
        set => SetProperty(nameof(Content), value);
    }

    public void AddRow(RowDefinition row)
    {
        RowDefinitions.Add(row);
    }

    public void AddColumn(ColumnDefinition column)
    {
        ColumnDefinitions.Add(column);
    }

    public override void AddContent(Widget widget) => AddContent(0, 0, widget);

    public void AddContent(int row, int column, Widget widget)
    {
        if (row >= RowDefinitions.Count || column >= ColumnDefinitions.Count)
            throw new ArgumentOutOfRangeException("Row or column index is out of range.");

        var item = new Item
        {
            Row = row,
            Column = column,
            Content = widget,
        };
        Content.Add(item);
        item?.Content?.OnAttachToWidget(this);
        ArrangeContent();
    }

    public override void Draw(Canvas canvas)
    {
        DrawBackground(canvas);
        foreach (var item in Content)
        {
            item?.Content?.Draw(canvas);
        }
    }

    public override Size Measure(Size availableSize = default)
    {
        double width = 0;
        double height = 0;

        foreach (var item in Content)
        {
            var row = RowDefinitions[item.Row].Height;
            var column = ColumnDefinitions[item.Column].Width;

            item.Content?.Measure(
                new Size(
                    Math.Max(0, column - Padding.Width - item.Content.Margin.Width),
                    Math.Max(0, row - Padding.Top - item.Content.Margin.Height)
                )
            );

            var itemSize = item.Content.MeasuredSize + item.Content.Margin.Size;
            width = Math.Max(width, itemSize.Width);
            height = Math.Max(height, itemSize.Height);
        }

        return new Size(width, height) + Padding.Size;
    }

    protected override void ArrangeContent()
    {
        foreach (var item in Content)
        {
            var position = GetCellPosition(item.Row, item.Column);
            item.Content?.Arrange(
                new Rect(
                    X + position.X + Padding.Left + item.Content.Margin.Left,
                    Y + position.Y + Padding.Top + item.Content.Margin.Top,
                    item.Content.MeasuredSize
                )
            );
        }
    }

    private Vector2 GetCellPosition(int row, int column)
    {
        var x = ColumnDefinitions.Take(column).Sum(c => c.Width);
        var y = RowDefinitions.Take(row).Sum(r => r.Height);
        return new Vector2(x, y);
    }
}
