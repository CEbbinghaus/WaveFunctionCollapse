using System;
using System.Linq;
using System.Collections.Generic;

namespace wfc{
    using Direction = Tuple<int, int>;

    using NeighbourFunction = Func<(int, int), (int, int), (int, int)[]>;

    public static class DefaultParsers{
        public static (int, int)[] Neighbours((int x, int y) pos, (int width, int height) size){
            List<(int, int)> directions = new List<(int, int)>();
            if(pos.x > 0)directions.Add((-1, 0));
            if(pos.y > 0)directions.Add((0, -1));
            if(pos.x < size.width - 1)directions.Add((1, 0));
            if(pos.y < size.height - 1)directions.Add((0, 1));
            return directions.ToArray();
        }
    }

    public struct GenerationParams<T> where T: IConvertible{
        public Dictionary<T, Dictionary<(int x, int y), T[]>> rules;
        public T[] possibilities;
        public Dictionary<T, float> weights;
        public int? seed;
        public NeighbourFunction GetNeighbours;

        public static GenerationParams<T> Create(Dictionary<T, Dictionary<(int x, int y), T[]>> rules, T[] possibilities, Dictionary<T, float> weights, int? seed = null, NeighbourFunction neighbourFunc = null){
            GenerationParams<T> g = new GenerationParams<T>();
            g.rules = rules;
            g.possibilities = possibilities;
            g.weights = weights;
            g.seed = seed;
            g.GetNeighbours = neighbourFunc ?? DefaultParsers.Neighbours;
            return g;
        }
    }

    public class Generator<T> where T: struct, IConvertible{
        IndeterminationField<T> field;
        GenerationParams<T> settings;
        Random random;
        int seed;
        (int width, int height) outputSize;

        public Generator(int width, int height, GenerationParams<T> gen){
            field = new IndeterminationField<T>(width, height);
            field.Initialize(gen.possibilities);
            outputSize = (width, height);
            seed = gen.seed ?? Environment.TickCount;
            random = new Random(seed);
            settings = gen;
            Console.WriteLine("Generator Initialized with Seed {0}", seed);
        }

        bool isDetermined(){
            foreach(var element in field){
                if(!element.Determined)
                    return false;
            }
            return true;
        }

        Indeterminant<T> getLowestEntropy(){
            int possibilityCount = settings.possibilities.Length;
            Indeterminant<T> lowest = field[random.Next(outputSize.width), random.Next(outputSize.height)];

            foreach(var e in field){
                if(e.Determined)continue;
                if(lowest == null || lowest.Determined || e.ScaledEntropy(possibilityCount) < // - random.NextDouble() / 10000
                    lowest.ScaledEntropy(possibilityCount))
                    lowest = e;
            }
            return lowest;
        }

        void Collapse(Indeterminant<T> element){
            if(element.Determined)
                throw new Exception("Trying to Collapse a Already Determined Tile");

            Dictionary<T, float> tileWeights = new Dictionary<T, float>();
            foreach(var possibility in element.Possibilities){
                tileWeights.Add(possibility, settings.weights[possibility]);
            }

            float totalWeight = tileWeights.Values.ToArray().Sum();
            T[] keys = tileWeights.Keys.ToArray();
            float rnd = (float)random.NextDouble() * totalWeight;
            // keys.Shuffle();
            T? tile = null;

            foreach(var item in keys){
                rnd -= tileWeights[item];
                if(rnd < 0){
                    tile = item;
                    break;
                }
            }

            element.Determine(tile ?? default(T));
        }

        IEnumerable<int> Propergate(Indeterminant<T> element){
            List<Indeterminant<T>> stack = new List<Indeterminant<T>>(){element};

            while(stack.Count > 0){
                Indeterminant<T> current = stack.Pop();

                var pos = current.Position;

                bool changed = false;

                foreach((int x, int y) direction in settings.GetNeighbours(pos, outputSize).Shuffle()){
                    var other = field[(pos.x + direction.x, pos.y + direction.y)];
                    if(other.Determined)continue;

                    IEnumerable<T> possibilities = new T[]{};
                    foreach(var possibility in current.Possibilities){
                        var tilePossibilities = settings.rules[possibility][direction].Intersect(other.Possibilities);
                        possibilities = possibilities.Union(tilePossibilities);
                    }

                    if(other.ConstrainPossibilities(possibilities.ToArray())){
                        stack.Add(other);
                        changed = true;
                    }

                }
                if(changed)
                    yield return 0;
            }
        }

        public T[,] evaulate(ref (int, int) start){
            start = (outputSize.width, outputSize.height);
            // {
            //     //var first = field[random.Next(outputSize.width), random.Next(outputSize.height)];
            //     //var first = field[outputSize.width / 2, outputSize.height / 2];
            //     //var first = field[outputSize.width - 1, outputSize.height - 1]; //outputSize.height / 2
            //     var first = field[outputSize.width - 1, 0]; //outputSize.height / 2
            //     //var first = field[0, 0]; //outputSize.height / 2
            //     Console.WriteLine("First Collapsed is: ({0}, {1})", first.Position.x, first.Position.y);
            //     start = first.Position;
            //     Collapse(first);
            //     Propergate(first);
            // }

            while(!isDetermined()){
                Indeterminant<T> toBeCollapsed = getLowestEntropy();
                if(start == (outputSize.width, outputSize.height))
                    start = toBeCollapsed.Position;
                Collapse(toBeCollapsed);
                Propergate(toBeCollapsed).ToArray();
            }

            //Converting from Field to output
            T[,] output = new T[outputSize.width, outputSize.height];
            foreach(var element in field){
                var pos = element.Position;
                output[pos.x, pos.y] = element.GetDeterminant;
            }
            return output;
        }

        public T[,] iterate(Func<IndeterminationField<T>, bool?> func){
            // {
            //     //var first = field[random.Next(outputSize.width), random.Next(outputSize.height)];
            //     //var first = field[outputSize.width / 2, outputSize.height / 2];
            //     //var first = field[outputSize.width - 1, outputSize.height - 1]; //outputSize.height / 2
            //     var first = field[outputSize.width - 1, 0]; //outputSize.height / 2
            //     //var first = field[0, 0]; //outputSize.height / 2
            //     Console.WriteLine("First Collapsed is: ({0}, {1})", first.Position.x, first.Position.y);
            //     Collapse(first);
            //     Propergate(first);
            // }
            //func(field);

            bool shouldSkip = false;
            while(!isDetermined()){
                Indeterminant<T> toBeCollapsed = getLowestEntropy();
                shouldSkip = false;
                Collapse(toBeCollapsed);
                shouldSkip = func(field) ?? false;
                
                foreach(var _ in Propergate(toBeCollapsed)){
                    if(shouldSkip)continue;
                    shouldSkip = func(field) ?? false;
                }
            }

            //Converting from Field to output
            T[,] output = new T[outputSize.width, outputSize.height];
            foreach(var element in field){
                var pos = element.Position;
                output[pos.x, pos.y] = element.GetDeterminant;
            }
            return output;
        }
    }
}