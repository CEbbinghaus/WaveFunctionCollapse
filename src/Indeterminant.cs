using System;
using System.Linq;
using System.Collections.Generic;

namespace wfc {
    class Indeterminant{
		List<int> possibilities;
		int? determinant = null;

		public int[] Possibilities{
			get{
				if(determinant != null)
					return new int[]{(int)determinant};
				return possibilities.ToArray();
			}
		}

		(int x, int y) position;

		public int GetDeterminant{
			get{
				if(determinant == null)
					throw new Exception("Indeterminant wasn't collapsed before Field was Finalized");
				return determinant ?? -1;
			}
		}

		public (int x, int y) Position{
			get{
				return position;
			}
		}

		public int Entropy{
			get{
				if(Determinant)
					return -1;
				return possibilities.Count;
			}
		}

		public bool Determinant{
			get{
				return determinant != null;
			}
		}

		void CheckDetermined(){
			if(possibilities.Count == 1){
				determinant = possibilities.Pop();
			}
		}

		public float ScaledEntropy(int possibilities){
			return Entropy / possibilities;
		}

		public Indeterminant(int[] possibilities, (int x, int y) pos){
			position = pos;
			this.possibilities = new List<int>(possibilities);
		} 

		public void SetPossibilities(int[] newPossibilities){
			possibilities = new List<int>(newPossibilities);
			CheckDetermined();
		}

		public bool ConstrainPossibilities(int[] contrained){
			var diff = possibilities.Except(contrained).ToArray();
			if(diff.Length > 0){
				possibilities = possibilities.Intersect(contrained).ToList();
				CheckDetermined();
				return true;
			}
			return false;
		}

		public void Determine(int result){
			possibilities.Clear();
			determinant = result;
		}

		public void RemovePossibility(int possibility){
			possibilities.Remove(possibility);
			CheckDetermined();
		}

		public override string ToString(){
			return $"Ø({position.x}, {position.y})[{string.Join(", ", Possibilities)}]";
		}
	}
}