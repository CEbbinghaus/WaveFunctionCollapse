using System;
using System.Collections.Generic;

public static class OverLoads{
	public static T Pop<T>(this List<T> list){
		if(list.Count <= 0)return default(T);
		T value = list[0];
		list.RemoveAt(0);
		return value;
	}

	public static Dictionary<KT, TT> Map<KT, FT, TT>(this Dictionary<KT, FT> dict, Func<KT, FT, TT> converter){
		Dictionary<KT, TT> result = new Dictionary<KT, TT>();
		foreach(var key in dict.Keys){
			result.Add(key, converter(key, dict[key]));
		}
		return result;
	}
}