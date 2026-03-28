namespace Architect.Utilities.Extensions;

public static class EnumerableEx
{
    public static bool Any<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate, out T item)
    {
        foreach (T i in enumerable)
        {
            if (predicate(i))
            {
                item = i;
                return true;
            }
        }
        item = default;
        return false;
    }
}
