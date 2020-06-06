using System;
using System.Linq;
using System.Collections.Generic;

public static class OverLoads {
    public static T Pop<T>(this List<T> list) {
        if (list.Count <= 0) return default(T);
        T value = list[0];
        list.RemoveAt(0);
        return value;
    }

    public static Dictionary<KT, TT> Map<KT, FT, TT>(this Dictionary<KT, FT> dict, Func<KT, FT, TT> converter) {
        Dictionary<KT, TT> result = new Dictionary<KT, TT>();
        foreach (var key in dict.Keys) {
            result.Add(key, converter(key, dict[key]));
        }
        return result;
    }

    public static T[] Shuffle<T>(this T[] array) {
        Random random = new Random();
        int n = array.Length;
        while (n > 1) {
            int k = random.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
        return array;
    }

    public static T[,] Transpose<T>(this T[,] array) {
        int width = array.GetLength(0);
		int height = array.GetLength(1);

		T[,] output = new T[height, width];

		for(int x = 0; x < width; ++x){
			for(int y = 0; y < height; ++y){
				output[y, x] = array[x, y];
			}
		}
		return output;
    }
}