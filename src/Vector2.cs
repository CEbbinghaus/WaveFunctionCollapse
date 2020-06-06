namespace wfc {
	public struct Vector2 {
		public int x;
		public int y;

		public static Vector2 one {
			get {
				return new Vector2 (1, 1);
			}
		}

		public static Vector2 zero {
			get {
				return new Vector2 (0, 0);
			}
		}

		public static Vector2 right {
			get {
				return new Vector2 (1, 0);
			}
		}

		public static Vector2 up {
			get {
				return new Vector2 (0, 1);
			}
		}

		public Vector2 (int x, int y) {
			this.x = x;
			this.y = y;
		}
		public Vector2 (int a) {
			this.x = a;
			this.y = a;
		}
		public Vector2 (Vector2 v) {
			this.x = v.x;
			this.y = v.y;
		}
		public Vector2 ((int x, int y) v) {
			this.x = v.x;
			this.y = v.y;
		}

		public static Vector2 operator + (Vector2 a, Vector2 b) {
			return new Vector2 (a.x + b.x, a.y + b.y);
		}

		public static Vector2 operator - (Vector2 a, Vector2 b) {
			return new Vector2 (a.x - b.x, a.y - b.y);
		}

		public static Vector2 operator * (Vector2 a, Vector2 b) {
			return new Vector2 (a.x * b.x, a.y * b.y);
		}

		public static Vector2 operator * (Vector2 a, int b) {
			return new Vector2 (a.x * b, a.y * b);
		}

		public static Vector2 operator / (Vector2 a, Vector2 b) {
			return new Vector2 (a.x / b.x, a.y / b.y);
		}

		public static Vector2 operator / (Vector2 a, int b) {
			return new Vector2 (a.x / b, a.y / b);
		}

		public static Vector2 operator - (Vector2 v) {
			return new Vector2 (-v.x, -v.y);
		}

		public static implicit operator (int, int) (Vector2 v) {
			return (v.x, v.y);
		}

		public static implicit operator Vector2((int x, int y) v) {
			return new Vector2(v.x, v.y);
		}
	}
}