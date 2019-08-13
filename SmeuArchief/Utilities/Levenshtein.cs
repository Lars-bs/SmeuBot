using System;
using System.Collections.Generic;
using System.Text;

namespace SmeuArchief.Utilities
{
    public static class Levenshtein
    {
        public static int GetLevenshteinDistance(string first, string second)
        {
            int[,] d = new int[first.Length + 1, second.Length + 1];

            for(int i = 1; i < first.Length + 1; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 1; j < second.Length + 1; j++)
            {
                d[0, j] = j;
            }

            for(int j = 1; j < second.Length + 1; j++)
            {
                for(int i = 1; i < first.Length + 1; i++)
                {
                    int cost = (first[i - 1] == second[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j], d[i, j - 1]) + 1, d[i - 1, j - 1] + cost);
                }
            }

            return d[first.Length, second.Length];
        }

        public static float GetSimilarity(string first, string second)
        {
            return GetSimilarity(first, second, GetLevenshteinDistance(first, second));
        }

        public static float GetSimilarity(string first, string second, int distance)
        {
            int l = Math.Max(first.Length, second.Length);
            return (l - distance) / (float)l;
        }
    }
}
