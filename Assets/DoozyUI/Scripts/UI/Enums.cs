namespace DoozyUI
{
    /// <summary>
    /// Types of Input Modes.
    /// </summary>
    public enum ControllerInputMode
    {
        /// <summary>
        /// The Button will only react to mouse clicks and touches.
        /// </summary>
        None,
        /// <summary>
        /// The Button will react to set KeyCodes.
        /// </summary>
        KeyCode,
        /// <summary>
        /// The Button will react to set virtual Button names (set up in the InputManager).
        /// </summary>
        VirtualButton
    }
}