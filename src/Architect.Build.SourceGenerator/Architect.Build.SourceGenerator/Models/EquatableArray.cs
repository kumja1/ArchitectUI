using System.Collections;

namespace Architect.Build.SourceGenerator.Models;

internal readonly struct EquatableArray<T>(T[] array)
    : IEquatable<EquatableArray<T>>,
        IEnumerable<T>,
        IEnumerable
{
    private readonly T[] _array = array;

    public readonly int Length => _array.Length;

    public bool Equals(EquatableArray<T> other) => _array.SequenceEqual(other._array);

    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode() =>
        _array.Aggregate(0, (hash, item) => hash ^ item.GetHashCode());

    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right) =>
        left.Equals(right);

    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right) =>
        !left.Equals(right);

    public static implicit operator EquatableArray<T>(List<T> list) => new([.. list]);

    public static implicit operator EquatableArray<T>(T[] array) => new(array);

    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_array).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _array.GetEnumerator();
}
