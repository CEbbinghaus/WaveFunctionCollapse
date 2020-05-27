using System;
using System.Linq;
using System.Collections.Generic;
using wfc;

class Program {
	
	static ConsoleColor[] colors = {ConsoleColor.Magenta, ConsoleColor.DarkYellow, ConsoleColor.Yellow, ConsoleColor.White, ConsoleColor.DarkBlue, ConsoleColor.Blue, ConsoleColor.Green};
    enum Tile {
        a, b, c, d, e, f, g
    }

	static (int[] possibilities, Dictionary<int, float> weights) ParseExample(Tile[,] arr){
		Dictionary<int, float> intermediate = new Dictionary<int, float>();

		foreach(Tile t in arr){
			if(!intermediate.ContainsKey((int)t))
				intermediate.Add((int)t, 0.0f);
			intermediate[(int)t] += 1.0f;
		}

		int[] possibilities = intermediate.Keys.ToArray();
		float[] weights = intermediate.Values.ToArray();

		// float total = 0;
		// foreach(float weight in weights){
		// 	total += weight;
		// }

		// for(int i = 0; i < weights.Length; ++i){
		// 	intermediate[possibilities[i]] /= total;
		// }

		return (possibilities, intermediate);
	}

	static (int x, int y)[] GetDirections((int x, int y) pos, (int width, int height) size){
		List<(int, int)> directions = new List<(int, int)>();
		if(pos.x > 0)directions.Add((-1, 0));
		if(pos.y > 0)directions.Add((0, -1));
		if(pos.x < size.width - 1)directions.Add((1, 0));
		if(pos.y < size.height - 1)directions.Add((0, 1));
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

				var directions = GetDirections((x, y), (width, height));
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
		int width;
		int height;
		if(args.Length == 2){
			width = int.Parse(args[0]);
			height = int.Parse(args[1]);
		}else if(args.Length == 1){
			width = height = int.Parse(args[0]);
		}else{
			width = 50;
			height = 20;
		}



		Tile[,] example = new Tile[,]{
			{Tile.g,Tile.g,Tile.g,Tile.g,Tile.g,Tile.g},
			{Tile.g,Tile.g,Tile.a,Tile.g,Tile.b,Tile.g},
			{Tile.g,Tile.c,Tile.g,Tile.g,Tile.g,Tile.g},
			{Tile.c,Tile.f,Tile.c,Tile.c,Tile.c,Tile.c},
			{Tile.f,Tile.e,Tile.e,Tile.f,Tile.f,Tile.f},
			{Tile.f,Tile.e,Tile.f,Tile.e,Tile.f,Tile.f},
		}.Transpose();
        // test{] v = (test[})Enum.GetValues(typeof(test));
        // Console.WriteLine(v.Length);
        // foreach (test t in v) {
        //     Console.WriteLine(t);
        // }
        // test t = testing.Pop();

		var (possibilities, weights) = ParseExample(example);
		var rules = ParseRules(example);


		GenerationParams<int> gen = GenerationParams<int>.Create(
			possibilities: possibilities,
			weights: weights,
			rules: rules
		);



        Generator<int> g = new Generator<int>(width, height, gen);
		int[,] result = g.evaulate();

		for(int y = 0; y < result.GetLength(1); ++y){
			for(int x = 0; x < result.GetLength(0); ++x){
				Console.ForegroundColor = ConsoleColor.White;
				if(x != 0)
					Console.Write(",");
				
				Console.ForegroundColor = colors[(int)result[x,y]];
				Console.Write(((Tile)result[x,y]).ToString()) ;
			}
			Console.WriteLine("");
		}

        // Generator<test> t = new Generator<test>();
        // WaveFunctionCollapse.Print();
        // Console.WriteLine("Hello World!");
    }
}
