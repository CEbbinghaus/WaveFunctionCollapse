using System;
using System.Collections;
using System.Collections.Generic;

namespace wfc{
	class IndeterminationField{
		public bool Ready{
			get{
				return fieldInitialized;
			}
		}

		Indeterminant[,] field;
		bool fieldInitialized = false;
		int width, height;

		public Indeterminant this[(int x, int y) value]{
			get{
				return field[value.x, value.y];
			}
		}
		public Indeterminant this[int x, int y]{
			get{
				return field[x, y];
			}
		}

		public IndeterminationField(int width, int height){
			this.width = width; this.height = height;
			field = new Indeterminant[width, height];           
		}

		public void Initialize(int[] possibilities){
			Map((index, i) => {
				return new Indeterminant(possibilities, index);
			});
			fieldInitialized = true;
		}

		public (int x, int y)? GetLowestEntropy(){
			if(!Ready)return null;
			
			var lowest = (x: 0, y: 0);
			ForEach((index, indeterminant) => {
				if(indeterminant.Determinant)return;

				if(indeterminant.Entropy < field[lowest.x, lowest.y].Entropy){
					lowest = index;
				}
			});

			return lowest;
		}

		public void Map(Func<(int x, int y), Indeterminant, Indeterminant> f){
			for(int x = 0; x < width; ++x){
				for(int y = 0; y < height; ++y){
					var res = f((x, y), field[x, y]);
					if(res != null)
						field[x, y] = res;
				}
			}
		}

		public void ForEach(Action<(int x, int y), Indeterminant> f){
			for(int x = 0; x < width; ++x){
				for(int y = 0; y < height; ++y){
					f((x, y), field[x, y]);
				}
			}
		}
	
		public IEnumerator<Indeterminant> GetEnumerator(){
			for(int x = 0; x < width; ++x){
				for(int y = 0; y < height; ++y){
					yield return field[x, y];
				}
			}
		}
	}
}