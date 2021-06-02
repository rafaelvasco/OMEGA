
namespace OMEGA
{
    public struct GamePadThumbSticks
	{
		public Vec2 Left
		{
			get
			{
				return _left;
			}
		}
		public Vec2 Right
		{
			get
			{
				return _right;
			}
		}

		private Vec2 _left;
		private Vec2 _right;

		public GamePadThumbSticks(Vec2 leftPosition, Vec2 rightPosition)
		{
			_left = leftPosition;
			_right = rightPosition;
			ApplySquareClamp();
		}

		internal GamePadThumbSticks(
			Vec2 leftPosition,
			Vec2 rightPosition,
			GamePadDeadZone deadZoneMode
		) {
			_left = leftPosition;
			_right = rightPosition;
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
					_left.X = GamePad.ExcludeAxisDeadZone(_left.X, GamePad.LEFT_DEAD_ZONE);
					_left.Y = GamePad.ExcludeAxisDeadZone(_left.Y, GamePad.LEFT_DEAD_ZONE);
					_right.X = GamePad.ExcludeAxisDeadZone(_right.X, GamePad.RIGHT_DEAD_ZONE);
					_right.Y = GamePad.ExcludeAxisDeadZone(_right.Y, GamePad.RIGHT_DEAD_ZONE);
					break;
				case GamePadDeadZone.Circular:
					_left = ExcludeCircularDeadZone(_left, GamePad.LEFT_DEAD_ZONE);
					_right = ExcludeCircularDeadZone(_right, GamePad.RIGHT_DEAD_ZONE);
					break;
			}
		}

		private void ApplySquareClamp()
		{
			_left.X = Calc.Clamp(_left.X, -1.0f, 1.0f);
			_left.Y = Calc.Clamp(_left.Y, -1.0f, 1.0f);
			_right.X = Calc.Clamp(_right.X, -1.0f, 1.0f);
			_right.Y = Calc.Clamp(_right.Y, -1.0f, 1.0f);
		}

		private void ApplyCircularClamp()
		{
			if (_left.LengthSquared() > 1.0f)
			{
				_left.Normalize();
			}
			if (_right.LengthSquared() > 1.0f)
			{
				_right.Normalize();
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
			return (left._left == right._left) && (left._right == right._right);
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
