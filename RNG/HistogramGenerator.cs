using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RNG;

public static class HistogramGenerator
{
    public static SortedDictionary<T, int> CreateHistogramFromArray<T>(this IEnumerable<T> enumerable) => new(
        enumerable
            .GroupBy(item => item)
            .ToDictionary(x => x.Key, x => x.Count())
        );

    public static void WriteHistogramToFile<T>(this SortedDictionary<T, int> histogram, string fileName)
    {
        if (histogram is null)
        {
            throw new ArgumentNullException(nameof(histogram));
        }

        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException($"'{nameof(fileName)}' cannot be null or empty.", nameof(fileName));
        }

        StringBuilder sb = new StringBuilder();
        
        foreach (var item in histogram)
        {
            sb.AppendLine($"{item.Key.ToString()} {item.Value.ToString()}");
        }
        sb.AppendLine($"Entropy: {CalculateEntropy(histogram)}");

        File.WriteAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar.ToString() + fileName, sb.ToString());

    }

    public static double CalculateEntropy<T>(this SortedDictionary<T, int> histogram)
    {
        if (histogram is null)
        {
            throw new ArgumentNullException(nameof(histogram));
        }

        int sum = histogram.Select(x => x.Value).Sum();

        double entropy = 0;
        foreach (var item in histogram)
        {
            var propability = (double)item.Value / sum;
            entropy += (propability * Math.Log2(propability));
        }

        return -entropy;
    }



}
