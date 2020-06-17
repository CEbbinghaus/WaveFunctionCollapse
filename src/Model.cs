using System;
using System.Collections.Generic;
using System.Linq;

namespace wfc{
	abstract class Model<T> where T: struct, IConvertible{
		public abstract Vector2[] GetNeighbours(Vector2 position);
		public abstract T[] GetPossibilities(Vector2 center, Vector2 direction);

		protected IndeterminationField<T> field;
		protected Model(IndeterminationField<T> afield){
			this.field = afield;
		}	
	}


	class SimpleModel<T> : Model<T> where T: struct, IConvertible{

		Dictionary<T, Dictionary<Vector2, T[]>> rules;
		

		public SimpleModel(IndeterminationField<T> field): base(field){}


		public override Vector2[] GetNeighbours(Vector2 position){
			return DefaultFunctions.Neighbours(position, (field.Width, field.Height));
		}

		public override T[] GetPossibilities(Vector2 center, Vector2 direction){
			var element = field[center];
			if(element.Determined)
				return rules[field[center].GetDeterminant][direction];
			else{
				HashSet<T> possibilities = new HashSet<T>();
				foreach(var possibility in element.Possibilities){
					possibilities.UnionWith(rules[possibility][direction]);
				}
				return possibilities.ToArray();
			}
		}
	} 

	
	class TiledModel<T>: Model<T> where T: struct, IConvertible{

		int N;

		TiledModel(IndeterminationField<T> field, int N): base(field){
			this.N = N;
		}

		public override Vector2[] GetNeighbours(Vector2 position){
			return DefaultFunctions.Neighbours(position, (field.Width, field.Height));
		}

		public override T[] GetPossibilities(Vector2 center, Vector2 direction){
			return new T[]{};
		}
	}

}