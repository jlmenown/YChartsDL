using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YChartsDL
{
    static class Constants
    {
        public static readonly DateTime UnixOrigin = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        public const string SessionIdHelpText = "This is an optional ycharts session ID used to view premium content. The session ID can be retrieved from your ycharts cookies once you've logged in.";

        public const string InterpolateHelpText = "Interpolate price data on missing days to ease data comparisons.";

        public const string FastHelpText = "This forces the program to skip its intentional execution delay, which may cause your data usage to show up on ycharts metrics if you abuse this program.";

        public const string TickerHelpText = "The ycharts-formatted ticker to pull data from. Examples are M:VFISX, VTIP, GOOG, ^SPY. ycharts doesn't follow consistent ticker formatting in its data endpoints. If your request doesn't work, you can check the correct formatting in the chart URL queries on the website.";

        public const string PathHelpText = "The location to write the results file. If none is provided the file will be saved to the current directory with the following format: 'ycharts_TICKER_TIMESTAMP.csv'.";

        public const string WebException400Text = "400 error. Your ticker may be invalid (MUTFs need an M: prefix) or Ycharts may have changed its request format. The URL was:\r\n{0}";

        public const string WebException401Text = "401 (Unauthorized) error. Ycharts may have patched some holes, changed its format, or changed how it restricts premium content. The URL was:\r\n{0}";

        public const string WebException403Text = "403 (Forbidden) error. Ycharts may have patched some holes, changed its format, or changed how it restricts premium content. The URL was:\r\n{0}";

        public const string WebException404Text = "404 error. Ycharts may have changed its request format. The URL was:\r\n{0}";

        public const string WriteErrorText = "Failed to write to the output file. The output path was: \r\n{0}\r\n\r\nThe output contents were:\r\n{1}\r\n";
    }
}
