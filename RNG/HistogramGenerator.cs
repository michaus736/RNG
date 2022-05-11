using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RNG;

public static class HistogramGenerator
{
    public static SortedDictionary<T, int> CreateHistogramFromArray<T>(this IEnumerable<T> enumerable)
    {
        return new SortedDictionary<T, int>(enumerable
            .GroupBy(item => item)
            .ToDictionary(x => x.Key, x => x.Count()));
    }

    public static void WriteHistogramToFile<T>(this SortedDictionary<T, int> histogram, string fileName)
    {
        StringBuilder sb = new StringBuilder();
        
        foreach (var item in histogram)
        {
            sb.AppendLine($"{item.Key.ToString()} {item.Value.ToString()}");
        }

        File.WriteAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar.ToString() + fileName, sb.ToString());

    }


}
