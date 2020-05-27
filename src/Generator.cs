using System;
using System.Linq;
using System.Collections.Generic;

namespace wfc{
    using Direction = Tuple<int, int>;
    using RuleList = Dictionary<int, Dictionary<(int x, int y), int[]>>;
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

    public struct GenerationParams{
        public RuleList rules;
        public int[] possibilities;
        public Dictionary<int, float> weights;
        public int? seed;
        public NeighbourFunction GetNeighbours;

        public static GenerationParams Create(RuleList rules, int[] possibilities, Dictionary<int, float> weights, int? seed = null, NeighbourFunction neighbourFunc = null){
            GenerationParams g = new GenerationParams();
            g.rules = rules;
            g.possibilities = possibilities;
            g.weights = weights;
            g.seed = seed;
            g.GetNeighbours = neighbourFunc ?? DefaultParsers.Neighbours;
            return g;
        }
    }

    public class Generator{
        IndeterminationField field;
        GenerationParams settings;
        Random random;
        (int width, int height) outputSize;

        public Generator(int width, int height, GenerationParams gen){
            field = new IndeterminationField(width, height);
            field.Initialize(gen.possibilities);
            outputSize = (width, height);
            random = new Random(gen.seed ?? DateTime.UtcNow.Millisecond);
            settings = gen;
            Console.WriteLine("Generator Initialized");
        }

        bool isDetermined(){
            foreach(var element in field){
                if(!element.Determinant)
                    return false;
            }
            return true;
        }

        Indeterminant getLowestEntropy(){
            int possibilityCount = settings.possibilities.Length;
            Indeterminant lowest = null;
            foreach(var e in field){
                if(e.Determinant)continue;
                if(lowest == null || e.ScaledEntropy(possibilityCount) - random.NextDouble() / 1000 < 
                    lowest.ScaledEntropy(possibilityCount))
                    lowest = e;
            }
            return lowest;
        }

        void Collapse(Indeterminant element){
            if(element.Determinant)
                throw new Exception("Trying to Collapse a Already Determined Tile");

            Dictionary<int, float> tileWeights = new Dictionary<int, float>();
            foreach(var possibility in element.Possibilities){
                tileWeights.Add(possibility, settings.weights[possibility]);
            }

            float totalWeight = tileWeights.Values.ToArray().Sum();
            int[] keys = tileWeights.Keys.ToArray();
            float rnd = (float)random.NextDouble() * totalWeight;
            // keys.Shuffle();
            int? tile = null;

            foreach(var item in keys){
                rnd -= tileWeights[item];
                if(rnd < 0){
                    tile = item;
                    break;
                }
            }

            if(tile == null)
                throw new Exception("Could not Determine Value");

            element.Determine(tile ?? -1);
        }

        void Propergate(Indeterminant element){
            List<Indeterminant> stack = new List<Indeterminant>(){element};

            while(stack.Count > 0){
                Indeterminant current = stack.Pop();

                var pos = current.Position;


                foreach((int x, int y) direction in settings.GetNeighbours(pos, outputSize)){
                    var other = field[(pos.x + direction.x, pos.y + direction.y)];
                    if(other.Determinant)continue;

                    IEnumerable<int> possibilities = new int[]{};
                    foreach(var possibility in current.Possibilities){
                        var tilePossibilities = settings.rules[possibility][direction].Intersect(other.Possibilities);
                        possibilities = possibilities.Union(tilePossibilities);
                    }

                    if(other.ConstrainPossibilities(possibilities.ToArray()))
                        stack.Add(other);

                }
            }
        }

        public int[,] evaulate(){
            while(!isDetermined()){
                Indeterminant toBeCollapsed = getLowestEntropy();
                Collapse(toBeCollapsed);
                Propergate(toBeCollapsed);
            }

            //Converting from Field to output
            int[,] output = new int[outputSize.width, outputSize.height];
            foreach(var element in field){
                var pos = element.Position;
                output[pos.x, pos.y] = element.GetDeterminant;
            }
            return output;
        }
    }
}