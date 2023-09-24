namespace LockerService.Application.Common.Utils;

public static class EnumerableUtils
{
    public static bool ContainsUniqueElements<TSource, TKey>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
    {
        var enumerable = source.ToList();
        return enumerable.DistinctBy(keySelector, comparer).Count() == enumerable.Count();
    }
}