using System;
using System.Collections.Generic;

namespace wfc {
    class Indeterminant {
		List<int> Possibilities;

		int? determinant;

		(int x, int y) position;

		public (int x, int y) Position{
			get{
				return position;
			}
		}

		public int Entropy{
			get{
				if(Determinant)
					return -1;
				return Possibilities.Count;
			}
		}

		public bool Determinant{
			get{
				return determinant != null;
			}
		}

		public Indeterminant(int[] possibilities, (int x, int y) pos){
			position = pos;
			this.Possibilities = new List<int>(possibilities);
		} 

		public void RemovePossibility(int possibility){
			Possibilities.Remove(possibility);
			if(Possibilities.Count == 1){
				determinant = Possibilities[0];
			}
		}

		public override string ToString(){
			return "Works";
		}
	}
}