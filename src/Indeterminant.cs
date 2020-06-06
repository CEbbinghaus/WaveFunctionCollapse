using System;
using System.Collections.Generic;
using System.Linq;

namespace wfc {
	using Dir = Vector2;

	public class Indeterminant<T> where T : struct, IConvertible {
		List<T> possibilities;
		T? determinant = null;

		public T[] Possibilities {
			get {
				if (determinant != null)
					return new T[] {
						(T) determinant };
				return possibilities.ToArray ();
			}
		}

		Vector2 position;

		public T GetDeterminant {
			get {
				if (determinant == null)
					throw new Exception ("Indeterminant wasn't collapsed before Field was Finalized");
				return (T) determinant;
			}
		}

		public Vector2 Position {
			get {
				return position;
			}
		}

		public int Entropy {
			get {
				if (determinant != null)
					return -1;
				return possibilities.Count;
			}
		}

		public bool Determined {
			get {
				return determinant != null;
			}
		}

		void CheckDetermined () {
			if (possibilities.Count == 1) {
				determinant = possibilities.Pop ();
			}
		}

		public float ScaledEntropy (int count) {
			return Entropy / count;
		}

		public Indeterminant (T[] possibilities, Vector2 pos) {
			position = pos;
			this.possibilities = new List<T> (possibilities);
		}

		public void SetPossibilities (T[] newPossibilities) {
			possibilities = new List<T> (newPossibilities);
			CheckDetermined ();
		}

		public bool ConstrainPossibilities (T[] contrained) {
			if (contrained.Length == 0)
				throw new Exception ("Trying to Constrain Nothing");
			var diff = possibilities.Except (contrained).ToArray ();
			if (diff.Length > 0) {
				possibilities = possibilities.Intersect (contrained).ToList ();
				CheckDetermined ();
				return true;
			}
			return false;
		}

		public void Determine (T result) {
			possibilities.Clear ();
			determinant = result;
		}

		public void RemovePossibility (T possibility) {
			possibilities.Remove (possibility);
			CheckDetermined ();
		}

		public override string ToString () {
			return $"Ø({position.x}, {position.y})[{(Determined ? ((T)determinant).ToString() : string.Join(", ", Possibilities))}]";
		}
	}
}