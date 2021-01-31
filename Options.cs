using CommandLine;

namespace YChartsDL
{
    /// <summary>
    /// Represents user options defined in the command line, as parsed by the CommandLine library
    /// </summary>
    class Options
    {
        /// <summary>
        /// Ycharts session ID string used to access data that requires an account
        /// </summary>
        [Option('s', "session", Default = "", HelpText = Constants.SessionIdHelpText)]
        public string SessionId { get; set; }

        /// <summary>
        /// Interpolates values for missing dates 
        /// </summary>
        [Option('i', "interpolate", Default = false, HelpText = Constants.InterpolateHelpText)]
        public bool Interpolate { get; set; }

        /// <summary>
        /// Disables the execution delay added to the program to limit ycharts data consumption
        /// </summary>
        [Option('f', "fast", Default = false, HelpText = Constants.FastHelpText)]
        public bool Fast { get; set; }

        /// <summary>
        /// The ticker to pull data for. Examples are "M:VFISX", "VTIP", "GOOG", "^SPY"
        /// </summary>
        [Value(0, Required = true, HelpText = Constants.TickerHelpText)]
        public string Ticker { get; set; }

        /// <summary>
        /// The location to write the results file
        /// </summary>
        [Option('p', "path", Default = null, HelpText = Constants.PathHelpText)]
        public string Path { get; set; }
    }
}
