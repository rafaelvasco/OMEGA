namespace OMEGA
{
    public struct GamePadTriggers
    {
        public float Left
        {
            get
            {
                return _left;
            }
        }
        public float Right
        {
            get
            {
                return _right;
            }
        }

        private float _left;
        private float _right;

        public GamePadTriggers(float leftTrigger, float rightTrigger)
        {
            _left = Calc.Clamp(leftTrigger, 0.0f, 1.0f);
            _right = Calc.Clamp(rightTrigger, 0.0f, 1.0f);
        }

        internal GamePadTriggers(
            float leftTrigger,
            float rightTrigger,
            GamePadDeadZone deadZoneMode
        )
        {
            /* XNA applies dead zones before rounding/clamping values.
			 * The public constructor does not allow this because the
			 * dead zone must be known first.
			 */
            if (deadZoneMode == GamePadDeadZone.None)
            {
                _left = Calc.Clamp(leftTrigger, 0.0f, 1.0f);
                _right = Calc.Clamp(rightTrigger, 0.0f, 1.0f);
            }
            else
            {
                _left = Calc.Clamp(
                    GamePad.ExcludeAxisDeadZone(
                        leftTrigger,
                        GamePad.TRIGGER_THRESHOLD
                    ),
                    0.0f,
                    1.0f
                );
                _right = Calc.Clamp(
                    GamePad.ExcludeAxisDeadZone(
                        rightTrigger,
                        GamePad.TRIGGER_THRESHOLD
                    ),
                    0.0f,
                    1.0f
                );
            }
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GamePadTriggers"/> are
        /// equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        /// True if <paramref name="left"/> and <paramref name="right"/> are equal;
        /// otherwise, false.
        /// </returns>
        public static bool operator ==(GamePadTriggers left, GamePadTriggers right)
        {
            return ((Calc.WithinEpsilon(left._left, right._left)) &&
                    (Calc.WithinEpsilon(left._right, right._right)));
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GamePadTriggers"/> are
        /// not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        /// True if <paramref name="left"/> and <paramref name="right"/> are not equal;
        /// otherwise, false.
        /// </returns>
        public static bool operator !=(GamePadTriggers left, GamePadTriggers right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare to this instance.</param>
        /// <returns>
        /// True if <paramref name="obj"/> is a <see cref="GamePadTriggers"/> and has the
        /// same value as this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return (obj is GamePadTriggers) && (this == (GamePadTriggers)obj);
        }

        public override int GetHashCode()
        {
            return this.Left.GetHashCode() + this.Right.GetHashCode();
        }

    }
}
