namespace Algo.Runtime.Build.Runtime
{
    /// <summary>
    /// Defines identifiers for the state of the interpreter
    /// </summary>
    public enum SimulatorState
    {
        /// <summary>
        /// Actually in pause, ready to continue
        /// </summary>
        Ready = 0,
        /// <summary>
        /// In pause, waiting for an action from the user to continue
        /// </summary>
        Pause = 1,
        /// <summary>
        /// Preparing to an action
        /// </summary>
        Preparing = 2,
        /// <summary>
        /// Working and or interpreting
        /// </summary>
        Running = 3,
        /// <summary>
        /// The interpreter is stopped
        /// </summary>
        Stopped = 4,
        /// <summary>
        /// The interpreter is stopped and an error has been thrown
        /// </summary>
        StoppedWithError = 5,
        /// <summary>
        /// In pause on a breakpoint, waiting for an action from the user to continue
        /// </summary>
        PauseBreakpoint = 6,
        /// <summary>
        /// In pause because of an error in the algorithm, waiton for an action from the user to stop
        /// </summary>
        PauseWithError = 7,
        /// <summary>
        /// A log message is send
        /// </summary>
        Log = 8
    }
}
