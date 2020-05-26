using System;
using System.Collections.Generic;


namespace wfc{
    using Direction = Tuple<int, int>;
    using RuleList = Dictionary<int, Dictionary<(int x, int y), int[]>>;

    public struct Generation{
        public RuleList rules;
        public int[] possibilities;
        public float[] weights;
        public int? seed;

        public static Generation Create(RuleList rules, int[] possibilities, float[] weights, int? seed = null){
            Generation g = new Generation();
            g.rules = rules;
            g.possibilities = possibilities;
            g.weights = weights;
            g.seed = seed;
            return g;
        }
    }

    public class Generator{
        IndeterminationField field;
        Generation settings;
        Random random;

        public Generator(int width, int height, Generation gen){
            field = new IndeterminationField(width, height);
            random = new Random(gen.seed ?? DateTime.UtcNow.Millisecond);
            settings = gen;
            Console.WriteLine("Successfully Ran");
        }
    }
}