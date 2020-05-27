using System;
using System.Linq;
using System.Collections.Generic;

namespace wfc {
    class Indeterminant<T> where T: IComparable<T>{
		List<T> possibilities;
		T determinant;
		bool determined = false;

		public T[] Possibilities{
			get{
				if(determined)
					return new T[]{determinant};
				return possibilities.ToArray();
			}
		}

		(int x, int y) position;

		public T GetDeterminant{
			get{
				if(!determined)
					throw new Exception("Indeterminant wasn't collapsed before Field was Finalized");
				return determinant;
			}
		}

		public (int x, int y) Position{
			get{
				return position;
			}
		}

		public int Entropy{
			get{
				if(Determined)
					return -1;
				return possibilities.Count;
			}
		}

		public bool Determined{
			get{
				return determined;
			}
		}

		void CheckDetermined(){
			if(possibilities.Count == 1){
				determinant = possibilities.Pop();
				determined = true;
			}
		}

		public float ScaledEntropy(int count){
			return Entropy / count;
		}

		public Indeterminant(T[] possibilities, (int x, int y) pos){
			position = pos;
			this.possibilities = new List<T>(possibilities);
		} 

		public void SetPossibilities(T[] newPossibilities){
			possibilities = new List<T>(newPossibilities);
			CheckDetermined();
		}

		public bool ConstrainPossibilities(T[] contrained){
			if(contrained.Length == 0)
				throw new Exception("Trying to Constrain Nothing");
			var diff = possibilities.Except(contrained).ToArray();
			if(diff.Length > 0){
				possibilities = possibilities.Intersect(contrained).ToList();
				CheckDetermined();
				return true;
			}
			return false;
		}

		public void Determine(T result){
			possibilities.Clear();
			determinant = result;
			determined = true;
		}

		public void RemovePossibility(T possibility){
			possibilities.Remove(possibility);
			CheckDetermined();
		}

		public override string ToString(){
			return $"Ø({position.x}, {position.y})[{string.Join(", ", Possibilities)}]";
		}
	}
}