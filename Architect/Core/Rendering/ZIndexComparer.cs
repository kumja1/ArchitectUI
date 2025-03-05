using Architect.Common.Interfaces;

public class ZIndexComparer : IComparer<IWidget>
{
    public int Compare(IWidget x, IWidget y)
    {
        return x.ZIndex.CompareTo(y.ZIndex);
    }
}