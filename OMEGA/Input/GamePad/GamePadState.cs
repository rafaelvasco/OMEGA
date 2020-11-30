
namespace OMEGA
{
    /// <summary>
	/// Represents specific information about the state of a controller,
	/// including the current state of buttons and sticks.
	/// </summary>
	public struct GamePadState
    {
        /// <summary>
        /// Indicates whether the controller is connected.
        /// </summary>
        public bool IsConnected
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the packet number associated with this state.
        /// </summary>
        public int PacketNumber
        {
            get;
            internal set;
        }

        /// <summary>
        /// Returns a structure that identifies which buttons on the controller
        /// are pressed.
        /// </summary>
        public GamePadButtons Buttons
        {
            get;
            internal set;
        }

        /// <summary>
        /// Returns a structure that indicates the position of the controller thumbsticks.
        /// </summary>
        public GamePadThumbSticks ThumbSticks
        {
            get;
            internal set;
        }

        /// <summary>
        /// Returns a structure that identifies the position of triggers on the controller.
        /// </summary>
        public GamePadTriggers Triggers
        {
            get;
            internal set;
        }

        public bool this[GamePadButtons button] => (Buttons & button) == button;


        /// <summary>
        /// Initializes a new instance of the GamePadState class using the specified
        /// GamePadThumbSticks, GamePadTriggers, GamePadButtons, and GamePadDPad.
        /// </summary>
        /// <param name="thumbSticks">Initial thumbstick state.</param>
        /// <param name="triggers">Initial trigger state.</param>
        /// <param name="buttons">Initial button state.</param>
        /// <param name="dPad">Initial directional pad state.</param>
        public GamePadState(
            GamePadThumbSticks thumbSticks,
            GamePadTriggers triggers,
            GamePadButtons buttons
        ) : this()
        {
            ThumbSticks = thumbSticks;
            Triggers = triggers;
            Buttons = buttons;
            IsConnected = true;
            PacketNumber = 0;
        }

        /// <summary>
        /// Determines whether two GamePadState instances are equal.
        /// </summary>
        /// <param name="left">Object on the left of the equal sign.</param>
        /// <param name="right">Object on the right of the equal sign.</param>
        public static bool operator ==(GamePadState left, GamePadState right)
        {
            return ((left.IsConnected == right.IsConnected) &&
                    (left.PacketNumber == right.PacketNumber) &&
                    (left.Buttons == right.Buttons) &&
                    (left.ThumbSticks == right.ThumbSticks) &&
                    (left.Triggers == right.Triggers));
        }

        /// <summary>
        /// Determines whether two GamePadState instances are not equal.
        /// </summary>
        /// <param name="left">Object on the left of the equal sign.</param>
        /// <param name="right">Object on the right of the equal sign.</param>
        public static bool operator !=(GamePadState left, GamePadState right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance is equal to a
        /// specified object.
        /// </summary>
        /// <param name="obj">Object with which to make the comparison.</param>
        public override bool Equals(object obj)
        {
            return (obj is GamePadState state) && (this == state);
        }

        /// <summary>
        /// Gets the hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Retrieves a string representation of this object.
        /// </summary>
        public override string ToString()
        {
            return base.ToString();
        }

    }
}
