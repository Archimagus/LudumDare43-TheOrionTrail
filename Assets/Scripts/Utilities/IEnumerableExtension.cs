using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtension
{
    private static System.Random r;
    static IEnumerableExtension()
    {
        r = new System.Random();
    }

    public static T Random<T>(this IEnumerable<T> input)
    {
		if (input.Count() == 0)
			return default(T);
        return input.ElementAt(r.Next(input.Count()));
    }


    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.Shuffle(new System.Random());
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, System.Random rng)
    {
        if (source == null) throw new System.ArgumentNullException("source");
        if (rng == null) throw new System.ArgumentNullException("rng");

        return source.ShuffleIterator(rng);
    }

    private static IEnumerable<T> ShuffleIterator<T>(this IEnumerable<T> source, System.Random rng)
    {
        var buffer = source.ToList();
        for(int i = 0, n = buffer.Count; i < n; ++i)
        {
            int j = rng.Next(i, buffer.Count);
            yield return buffer[j];
            buffer[j] = buffer[i];
        }
    }

    public static IEnumerable<T> Interleve<T>(this IEnumerable<T> first, IEnumerable<T> second)
    {
        using (var enumerator1 = first.GetEnumerator())
        using (var enumerator2 = second.GetEnumerator())
        {
            bool firstHasMore, secondHasMore;
            while((firstHasMore = enumerator1.MoveNext()) | (secondHasMore = enumerator2.MoveNext()))
            {
                if (firstHasMore) yield return enumerator1.Current;
                if (secondHasMore) yield return enumerator2.Current;
            }
        }
    }
}
