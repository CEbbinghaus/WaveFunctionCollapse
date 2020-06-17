using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using wfc;

//Iteration Seed: 219709328



class Program {
	// static ConsoleColor[] colors = {ConsoleColor.Magenta, ConsoleColor.DarkYellow, ConsoleColor.Yellow, ConsoleColor.White, ConsoleColor.DarkBlue, ConsoleColor.Blue, ConsoleColor.Green};
	//static ConsoleColor[] colors = {ConsoleColor.Magenta, ConsoleColor.DarkYellow, ConsoleColor.Yellow, ConsoleColor.White, ConsoleColor.DarkBlue, ConsoleColor.Blue, ConsoleColor.Green};
	static ConsoleColor[] colors = {
		ConsoleColor.Magenta,
		ConsoleColor.Green,
		ConsoleColor.Yellow,
		ConsoleColor.DarkYellow,
		ConsoleColor.Blue,
		ConsoleColor.Magenta,
		ConsoleColor.DarkBlue,
		ConsoleColor.White,
		};
    
	enum Tile {
        n, g, c, j, s, GS, k, l
    }

	static (Tile[] possibilities, Dictionary<Tile, float> weights) ParseExample(Tile[,] arr){
		Dictionary<Tile, float> intermediate = new Dictionary<Tile, float>();

		foreach(Tile t in arr){
			if(!intermediate.ContainsKey(t))
				intermediate.Add(t, 0.0f);
			intermediate[t] += 1.0f;
		}

		Tile[] possibilities = intermediate.Keys.ToArray();
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

	static Vector2[] GetDirections(Vector2 pos, (int width, int height) size){
		List<Vector2> directions = new List<Vector2>();
		if(pos.x > 0)directions.Add((-1, 0));
		if(pos.y > 0)directions.Add((0, -1));
		if(pos.x < size.width - 1)directions.Add((1, 0));
		if(pos.y < size.height - 1)directions.Add((0, 1));

		// if(directions.Contains((-1, 0)) && directions.Contains((0, 1)))directions.Add((-1, 1));
		// if(directions.Contains((1, 0)) && directions.Contains((0, -1)))directions.Add((1, -1));
		// if(directions.Contains((1, 0)) && directions.Contains((0, 1)))directions.Add((1, 1));
		// if(directions.Contains((-1, 0)) && directions.Contains((0, -1)))directions.Add((-1, -1));

		return directions.ToArray();
	}

	static Dictionary<Tile, Dictionary<Vector2, Tile[]>> ParseRules(Tile[,] arr){
		Dictionary<Tile, Dictionary<Vector2, List<Tile>>> result = new Dictionary<Tile, Dictionary<Vector2, List<Tile>>>();
		var width = arr.GetLength(0);
		var height = arr.GetLength(1);

		for(int x = 0; x < width; ++x){
			for(int y = 0; y < height; ++y){

				Tile id = arr[x, y];
				if(!result.ContainsKey(id))
					result.Add(id, new Dictionary<Vector2, List<Tile>>());

				var dict = result[id];

				var directions = GetDirections((x, y), (width, height));
				foreach(var direction in directions){
					if(!dict.ContainsKey(direction))
						dict.Add(direction, new List<Tile>());

					var neighbour = arr[x + direction.x, y + direction.y];

					if(!dict[direction].Contains(neighbour))
						dict[direction].Add(neighbour);
				}
				
			}
		}

		return result.Map((key, value) => {
			return value.Map((key, value) => {
				return value.ToArray();
			});
		});
	}

    static void Main(string[] raw_args) {
		List<string> args = raw_args.ToList();

		int iPos = args.IndexOf("-i");
		bool interractive = iPos != -1;
		if(interractive)
			args.RemoveAt(iPos);


		int width;
		int height;
		if(args.Count >= 2){
			width = int.Parse(args[0]);
			height = int.Parse(args[1]);
		}else if(args.Count >= 1){
			width = height = int.Parse(args[0]);
		}else{
			width = 50;
			height = 20;
		}

		int? seed = null;
		if(args.Count == 3)
			seed = int.Parse(args[2]);


		var initialPosition = (0, 0);


		// Tile[,] example = new Tile[,]{
		// 	{Tile.g,Tile.g,Tile.g,Tile.g,Tile.g,Tile.g},
		// 	{Tile.g,Tile.g,Tile.a,Tile.g,Tile.b,Tile.g},
		// 	{Tile.g,Tile.c,Tile.g,Tile.g,Tile.g,Tile.g},
		// 	{Tile.c,Tile.f,Tile.c,Tile.c,Tile.c,Tile.c},
		// 	{Tile.f,Tile.e,Tile.e,Tile.f,Tile.f,Tile.f},
		// 	{Tile.f,Tile.e,Tile.f,Tile.e,Tile.f,Tile.f},
		// }.Transpose();


		Tile[,] example = new Tile[,]{
			{Tile.g,Tile.g,Tile.g,Tile.g,Tile.g,Tile.g},
			{Tile.g,Tile.g,Tile.g,Tile.g,Tile.g,Tile.g},
			{Tile.g,Tile.g,Tile.g,Tile.g,Tile.g,Tile.g},
			{Tile.g,Tile.g,Tile.c,Tile.g,Tile.c,Tile.c},
			{Tile.g,Tile.c,Tile.s,Tile.c,Tile.s,Tile.s},
			{Tile.c,Tile.s,Tile.s,Tile.s,Tile.s,Tile.s},
		}.Transpose();
		// Tile[,] example = new Tile[,]{
		// 	{Tile.f,Tile.f,Tile.f,Tile.c,Tile.f,Tile.f},
		// 	{Tile.f,Tile.f,Tile.c,Tile.g,Tile.c,Tile.f},
		// 	{Tile.f,Tile.c,Tile.g,Tile.g,Tile.g,Tile.c},
		// 	{Tile.c,Tile.g,Tile.g,Tile.g,Tile.g,Tile.c},
		// 	{Tile.f,Tile.c,Tile.c,Tile.c,Tile.c,Tile.f},
		// 	{Tile.f,Tile.f,Tile.f,Tile.f,Tile.f,Tile.f},
		// }.Transpose();
		// Tile[,] example = new Tile[,]{
		// 	{Tile.a,Tile.a,Tile.b,Tile.a,Tile.b,Tile.b},
		// 	{Tile.a,Tile.a,Tile.b,Tile.a,Tile.a,Tile.a},
		// 	{Tile.b,Tile.a,Tile.b,Tile.a,Tile.a,Tile.a},
		// 	{Tile.a,Tile.a,Tile.b,Tile.a,Tile.b,Tile.b},
		// 	{Tile.a,Tile.a,Tile.b,Tile.a,Tile.a,Tile.a},
		// 	{Tile.a,Tile.a,Tile.b,Tile.a,Tile.a,Tile.a},
		// }.Transpose();
		// Tile[,] example = new Tile[,]{
		// 	{Tile.f,Tile.f,Tile.f,Tile.f,Tile.f,Tile.f},
		// 	{Tile.f,Tile.e,Tile.e,Tile.f,Tile.f,Tile.f},
		// 	{Tile.f,Tile.e,Tile.e,Tile.e,Tile.e,Tile.f},
		// 	{Tile.f,Tile.e,Tile.e,Tile.e,Tile.e,Tile.f},
		// 	{Tile.f,Tile.e,Tile.e,Tile.e,Tile.e,Tile.f},
		// 	{Tile.f,Tile.f,Tile.f,Tile.f,Tile.f,Tile.f},
		// }.Transpose();
        // test{] v = (test[})Enum.GetValues(typeof(test));
        // Console.WriteLine(v.Length);
        // foreach (test t in v) {
        //     Console.WriteLine(t);
        // }
        // test t = testing.Pop();

		var (possibilities, weights) = ParseExample(example);
		var rules = ParseRules(example);


		GenerationParams<Tile> gen = GenerationParams<Tile>.Create(
			possibilities: possibilities,
			neighbourFunc: GetDirections,
			weights: weights,
			rules: rules,
			seed: seed
		);



        Generator<Tile> g = new Generator<Tile>(width, height, gen);

		if(interractive){
			bool finish = false;
			Tile[,] result = g.iterate((field) => {
				if(finish)
					return null;
				for(int y = 0; y < height; ++y){
					for(int x = 0; x < width; ++x){
						var element = field[x, y];
						
						if(element.Determined){
							Console.ForegroundColor = colors[(int)element.GetDeterminant];
							Console.Write(element.GetDeterminant.ToString());
						}else{
							Tile[] possibilities = element.Possibilities;
							int value = possibilities.Sum(tile => (int)tile);
							Console.ForegroundColor = colors[value];
							Console.Write(((Tile)value).ToString());
						}
						
					}
					Console.WriteLine("");
				}
				Console.ForegroundColor = ConsoleColor.White;
				
				string read = Console.ReadLine();
				if(read == "finish" || read == "f")
					finish = true;
				else if(read == "continue" || read == "c")
					return true;
				return null;
				
			});

			for(int y = 0; y < height; ++y){
				for(int x = 0; x < width; ++x){
					Console.ForegroundColor = colors[(int)result[x,y]];
					Console.Write((result[x,y]).ToString());					
				}
				Console.WriteLine("");
			}
			Console.ForegroundColor = ConsoleColor.White;

		}else{
			Tile[,] result = g.evaulate(ref initialPosition);

			for(int y = 0; y < height; ++y){
				for(int x = 0; x < width; ++x){
					Console.ForegroundColor = colors[(int)result[x,y]];

					if((x, y) == initialPosition){
						Console.ForegroundColor = ConsoleColor.Magenta;
					}

					Console.Write((result[x,y]).ToString());

					// Console.ForegroundColor = ConsoleColor.White;
					// if(x != 0)
					// 	Console.Write(",");
					
				}
				Console.WriteLine("");
			}
			Console.ForegroundColor = ConsoleColor.White;
		}




		// new ConsoleColor();

        // Generator<test> t = new Generator<test>();
        // WaveFunctionCollapse.Print();
        // Console.WriteLine("Hello World!");
    }
}