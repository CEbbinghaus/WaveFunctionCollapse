using System;
using System.Collections.Generic;
using System.Linq;

namespace wfc {
    using Direction = Tuple<int, int>;

    using NeighbourFunction = Func < (int, int), (int, int), (int, int) [] >;

    public static class DefaultParsers {
        public static (int, int) [] Neighbours ((int x, int y) pos, (int width, int height) size) {
            List < (int, int) > directions = new List < (int, int) > ();
            if (pos.x > 0) directions.Add ((-1, 0));
            if (pos.y > 0) directions.Add ((0, -1));
            if (pos.x < size.width - 1) directions.Add ((1, 0));
            if (pos.y < size.height - 1) directions.Add ((0, 1));
            return directions.ToArray ();
        }
    }

    public struct GenerationParams<T> where T : IConvertible {
        public Dictionary < T,
            Dictionary < (int x, int y),
            T[] >> rules;
        public T[] possibilities;
        public Dictionary<T, float> weights;
        public int? seed;
        public NeighbourFunction GetNeighbours;

        public static GenerationParams<T> Create (Dictionary < T, Dictionary < (int x, int y), T[] >> rules, T[] possibilities, Dictionary<T, float> weights, int? seed = null, NeighbourFunction neighbourFunc = null) {
            GenerationParams<T> g = new GenerationParams<T> ();
            g.rules = rules;
            g.possibilities = possibilities;
            g.weights = weights;
            g.seed = seed;
            g.GetNeighbours = neighbourFunc ?? DefaultParsers.Neighbours;
            return g;
        }
    }

    public class Generator<T> where T : struct, IConvertible {
        IndeterminationField<T> field;
        GenerationParams<T> settings;
        Random random;
        (int width, int height) outputSize;

        public Generator (int width, int height, GenerationParams<T> gen) {
            field = new IndeterminationField<T> (width, height);
            field.Initialize (gen.possibilities);
            outputSize = (width, height);
            random = new Random (gen.seed ?? DateTime.UtcNow.Millisecond);
            settings = gen;
            Console.WriteLine ("Generator Initialized");
        }

        bool isDetermined () {
            foreach (var element in field) {
                if (!element.Determined)
                    return false;
            }
            return true;
        }

        Indeterminant<T> getLowestEntropy () {
            int possibilityCount = settings.possibilities.Length;
            Indeterminant<T> lowest = null;
            foreach (var e in field) {
                if (e.Determined) continue;
                if (lowest == null || e.ScaledEntropy (possibilityCount) - random.NextDouble () / 1000 <
                    lowest.ScaledEntropy (possibilityCount))
                    lowest = e;
            }
            return lowest;
        }

        void Collapse (Indeterminant<T> element) {
            if (element.Determined)
                throw new Exception ("Trying to Collapse a Already Determined Tile");

            Dictionary<T, float> tileWeights = new Dictionary<T, float> ();
            foreach (var possibility in element.Possibilities) {
                tileWeights.Add (possibility, settings.weights[possibility]);
            }

            float totalWeight = tileWeights.Values.ToArray ().Sum ();
            T[] keys = tileWeights.Keys.ToArray ();
            float rnd = (float) random.NextDouble () * totalWeight;
            // keys.Shuffle();
            T? tile = null;

            foreach (var item in keys) {
                rnd -= tileWeights[item];
                if (rnd < 0) {
                    tile = item;
                    break;
                }
            }

            element.Determine (tile ?? default (T));
        }

        void Propergate (Indeterminant<T> element) {
            List<Indeterminant<T>> stack = new List<Indeterminant<T>> () { element };

            while (stack.Count > 0) {
                Indeterminant<T> current = stack.Pop ();

                var pos = current.Position;

                foreach ((int x, int y) direction in settings.GetNeighbours (pos, outputSize)) {
                    var other = field[(pos.x + direction.x, pos.y + direction.y)];
                    if (other.Determined) continue;

                    IEnumerable<T> possibilities = new T[] { };
                    foreach (var possibility in current.Possibilities) {
                        var tilePossibilities = settings.rules[possibility][direction].Intersect (other.Possibilities);
                        possibilities = possibilities.Union (tilePossibilities);
                    }

                    if (other.ConstrainPossibilities (possibilities.ToArray ()))
                        stack.Add (other);

                }
            }
        }

        public T[, ] evaulate () {
            while (!isDetermined ()) {
                Indeterminant<T> toBeCollapsed = getLowestEntropy ();
                Collapse (toBeCollapsed);
                Propergate (toBeCollapsed);
            }

            //Converting from Field to output
            T[, ] output = new T[outputSize.width, outputSize.height];
            foreach (var element in field) {
                var pos = element.Position;
                output[pos.x, pos.y] = element.GetDeterminant;
            }
            return output;
        }
    }
}