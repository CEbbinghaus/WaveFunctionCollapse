using System;

namespace wfc{
	abstract class Model<T> where T: struct, IConvertible{
		public abstract Vector2[] GetNeighbours(Vector2 position);
		public abstract T[] GetPossibilities(Vector2 center, Vector2 direction);

		IndeterminationField<T> field;

		
	}
}