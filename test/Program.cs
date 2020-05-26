using System;
using System.Linq;
using System.Collections.Generic;
using wfc;

class Program {
	

    enum Tile {
        a, b, c, d, e, f, g
    }

	static (int[] possibilities, float[] weights) ParseExample(Tile[,] arr){
		Dictionary<int, float> intermediate = new Dictionary<int, float>();

		foreach(Tile t in arr){
			if(!intermediate.ContainsKey((int)t))
				intermediate.Add((int)t, 0.0f);
			intermediate[(int)t] += 1.0f;
		}

		int[] possibilities = intermediate.Keys.ToArray();
		float[] weights = intermediate.Values.ToArray();

		float total = 0;
		foreach(float weight in weights){
			total += weight;
		}

		for(int i = 0; i < weights.Length; ++i){
			weights[i] /= total;
		}

		return (possibilities, weights);
	}

	static (int x, int y)[] GetDirections(int x, int y, int width, int height){
		List<(int, int)> directions = new List<(int, int)>();
		if(x > 0)directions.Add((-1, 0));
		if(y > 0)directions.Add((0, -1));
		if(x < width - 1)directions.Add((1, 0));
		if(y < height - 1)directions.Add((0, 1));
		return directions.ToArray();
	}

	static Dictionary<int, Dictionary<(int, int), int[]>> ParseRules(Tile[,] arr){
		Dictionary<int, Dictionary<(int, int), List<int>>> result = new Dictionary<int, Dictionary<(int, int), List<int>>>();
		var width = arr.GetLength(0);
		var height = arr.GetLength(1);

		for(int x = 0; x < width; ++x){
			for(int y = 0; y < height; ++y){
				int id = (int)arr[x, y];
				if(!result.ContainsKey(id))
					result.Add(id, new Dictionary<(int, int), List<int>>());

				var element = result[id];

				var directions = GetDirections(x, y, width, height);
				foreach(var direction in directions){
					if(!element.ContainsKey(direction))
						element.Add(direction, new List<int>());
					var neighbour = (int)arr[x + direction.x, y + direction.y];

					if(!element[direction].Contains(neighbour))
						element[direction].Add(neighbour);
				}
				
			}
		}

		return result.Map((key, value) => {
			return value.Map((key, value) => {
				return value.ToArray();
			});
		});
	}

    static void Main(string[] args) {
		Tile[,] example = new Tile[,]{
			{Tile.g,Tile.g,Tile.g,Tile.g,Tile.g,Tile.g},
			{Tile.g,Tile.b,Tile.g,Tile.a,Tile.g,Tile.g},
			{Tile.g,Tile.g,Tile.g,Tile.g,Tile.g,Tile.g},
			{Tile.g,Tile.c,Tile.c,Tile.g,Tile.g,Tile.c},
			{Tile.c,Tile.f,Tile.f,Tile.c,Tile.c,Tile.f},
			{Tile.f,Tile.f,Tile.f,Tile.f,Tile.f,Tile.f},
			{Tile.f,Tile.f,Tile.f,Tile.e,Tile.f,Tile.f},
			{Tile.f,Tile.f,Tile.f,Tile.f,Tile.f,Tile.f},
			{Tile.f,Tile.f,Tile.f,Tile.f,Tile.f,Tile.f},
		};
        // test{] v = (test[})Enum.GetValues(typeof(test));
        // Console.WriteLine(v.Length);
        // foreach (test t in v) {
        //     Console.WriteLine(t);
        // }
        // test t = testing.Pop();

		var (possibilities, weights) = ParseExample(example);
		var rules = ParseRules(example);


		Generation gen = Generation.Create(
			possibilities: possibilities,
			weights: weights,
			rules: rules
			);


        Generator g = new Generator(100, 100, gen);

        // Generator<test> t = new Generator<test>();
        // WaveFunctionCollapse.Print();
        // Console.WriteLine("Hello World!");
    }
}
