
namespace OMEGA
{
    public struct GamePadThumbSticks
	{
		public Vec2 Left
		{
			get
			{
				return left;
			}
		}
		public Vec2 Right
		{
			get
			{
				return right;
			}
		}

		private Vec2 left;
		private Vec2 right;

		public GamePadThumbSticks(Vec2 leftPosition, Vec2 rightPosition)
		{
			left = leftPosition;
			right = rightPosition;
			ApplySquareClamp();
		}

		internal GamePadThumbSticks(
			Vec2 leftPosition,
			Vec2 rightPosition,
			GamePadDeadZone deadZoneMode
		) {
			left = leftPosition;
			right = rightPosition;
			ApplyDeadZone(deadZoneMode);
			if (deadZoneMode == GamePadDeadZone.Circular)
			{
				ApplyCircularClamp();
			}
			else
			{
				ApplySquareClamp();
			}
		}

		private void ApplyDeadZone(GamePadDeadZone dz)
		{
			switch (dz)
			{
				case GamePadDeadZone.None:
					break;
				case GamePadDeadZone.IndependentAxes:
					left.X = GamePad.ExcludeAxisDeadZone(left.X, GamePad.LeftDeadZone);
					left.Y = GamePad.ExcludeAxisDeadZone(left.Y, GamePad.LeftDeadZone);
					right.X = GamePad.ExcludeAxisDeadZone(right.X, GamePad.RightDeadZone);
					right.Y = GamePad.ExcludeAxisDeadZone(right.Y, GamePad.RightDeadZone);
					break;
				case GamePadDeadZone.Circular:
					left = ExcludeCircularDeadZone(left, GamePad.LeftDeadZone);
					right = ExcludeCircularDeadZone(right, GamePad.RightDeadZone);
					break;
			}
		}

		private void ApplySquareClamp()
		{
			left.X = Calc.Clamp(left.X, -1.0f, 1.0f);
			left.Y = Calc.Clamp(left.Y, -1.0f, 1.0f);
			right.X = Calc.Clamp(right.X, -1.0f, 1.0f);
			right.Y = Calc.Clamp(right.Y, -1.0f, 1.0f);
		}

		private void ApplyCircularClamp()
		{
			if (left.LengthSquared() > 1.0f)
			{
				left.Normalize();
			}
			if (right.LengthSquared() > 1.0f)
			{
				right.Normalize();
			}
		}

		private static Vec2 ExcludeCircularDeadZone(Vec2 value, float deadZone)
		{
			float originalLength = value.Length();
			if (originalLength <= deadZone)
			{
				return Vec2.Zero;
			}
			float newLength = (originalLength - deadZone) / (1.0f - deadZone);
			return value * (newLength / originalLength);
		}

		/// <summary>
		/// Determines whether two specified instances of <see cref="GamePadThumbSticks"/>
		/// are equal.
		/// </summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		/// True if <paramref name="left"/> and <paramref name="right"/> are equal;
		/// otherwise, false.
		/// </returns>
		public static bool operator ==(GamePadThumbSticks left, GamePadThumbSticks right)
		{
			return (left.left == right.left) && (left.right == right.right);
		}

		/// <summary>
		/// Determines whether two specified instances of <see cref="GamePadThumbSticks"/>
		/// are not equal.
		/// </summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		/// True if <paramref name="left"/> and <paramref name="right"/> are not equal;
		/// otherwise, false.
		/// </returns>
		public static bool operator !=(GamePadThumbSticks left, GamePadThumbSticks right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Returns a value indicating whether this instance is equal to a specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns>
		/// True if <paramref name="obj"/> is a <see cref="GamePadThumbSticks"/> and has the
		/// same value as this instance; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			return (obj is GamePadThumbSticks) && (this == (GamePadThumbSticks) obj);
		}

		public override int GetHashCode()
		{
			return this.Left.GetHashCode() + 37 * this.Right.GetHashCode();
		}

	}
}
