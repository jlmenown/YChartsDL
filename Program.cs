using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace YChartsDL
{
    class Program
    {
        static void Main(string[] args)
        {
            // Parse console arguments into an Options object using the CommandLine library.
            // If parse fails, return. The CommandLine library *may* have printed useful instructions to the console.
            Options options;
            if (!TryParseConsoleOptions(args, out options))
                return;

            // Download data from YCharts for a given ticker, authenticating with a given session ID string
            string response = DownloadResponseString(options.Ticker, options.SessionId);

            // Parse a raw YCharts response string into a Dictionary 
            Dictionary<DateTime, double> data = ParseResponseString(response);

            // If requested by console arguments, interpolate between missing historical dates
            if (options.Interpolate)
                data = Utilities.InterpolateMissingValues(data)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Write the parsed Dictionary to a CSV file
            WriteAsCSV(data, options.Path);

            // Delay for a variable period to reduce the risk of this data crawler appearing on ycharts metrics
            if (!options.Fast)
            {
                // Compute a variable delay in milliseconds
                int delayMillis = new Random().Next(
                    Properties.Settings.Default.minDelayMillis,
                    Properties.Settings.Default.maxDelayMillis);

                // Wait for the specified number of milliseconds
                System.Threading.Tasks.Task.Delay(delayMillis).Wait();
            }
        }

        /// <summary>
        /// Parse raw console arguments using the CommandLine library
        /// </summary>
        static bool TryParseConsoleOptions(string[] args, out Options options)
        {
            // Parse console arguments into an Options object using the CommandLine library
            ParserResult<Options> parserResults
                = Parser.Default.ParseArguments<Options>(args);

            // If the parse attempt succeeded,
            if (parserResults.Tag == ParserResultType.Parsed)
            {
                // Retrieve the Options object
                options = ((Parsed<Options>)parserResults).Value;

                // Construct a default output path if one was not provided
                if (options.Path == null)
                {
                    // Calculate the current unix time
                    long unixTime = (DateTime.UtcNow - Constants.UnixOrigin).Ticks / TimeSpan.TicksPerSecond;

                    // Sanitize the ticker to remove any illegal filename characters
                    string sanitizedTicker =
                        Regex.Replace(options.Ticker, @"[\s,:.;/\\]+", "");

                    // Generate a relative output path in the form "ycharts_{TICKER}_{UNIXTIME}.csv"
                    options.Path =
                        string.Format("ycharts_{0}_{1}.csv", sanitizedTicker, unixTime);
                }

                // Return true to indicate success
                return true;
            }
            // Else return false to indicate failure
            else
            {
                options = null;
                return false;
            }
        }

        /// <summary>
        /// Download data from YCharts for a given ticker, authenticating with a given session ID string
        /// </summary>
        /// <param name="ticker">A YCharts-formatted ticker, ex "GOOG"</param>
        /// <param name="sessionId">A YCharts session ID required to retrieve non-free data</param>
        /// <returns></returns>
        static string DownloadResponseString(string ticker, string sessionId)
        {
            // Construct a URL targeting a ycharts endpoint
            // Example: https://ycharts.com/charts/fund_data.json?calcs=id:total_return_forward_adjusted_price,include:true,,&format=indexed&securities=id:{0},include:true,,&splitType=single&maxPoints=34000000
            // Queries:
            //      calcs       Specifies the type of data to query
            //                  Multiple options can be specified, but this code won't be able to parse them.
            //                  Many data types exist. Check the options in the ycharts viewer to pull more if desired. Code may or may not be able to handle parsing them.
            //      format      "indexed" provides zero-indexed total returns to date.
            //                  "real" provides equivalent reinvested shares from creation, in the context of total return.
            //      securities  The security tickers to query
            //                  Multiple options can be specified, but this code won't be able to parse them.
            //      splitType   This should be related to the handling of share split events, not sure.
            //      maxPoints   This indirectly sets the start date for data retrieval. The last datapoint is always the current date.
            string url = string.Format(
                Properties.Settings.Default.ychartsUrlTemplate, 
                ticker);

            // Create a cookie containing a session ID used to identify a ycharts account.
            // If no ID is specified, data availability will be limited to what you'd see on the site while logged out.
            // This includes a 5 year limit on historic data.
            // A 1 week trial can be created. Check your cookies after creating the account and navigating to any chart,
            // locate the ID cookie, and provide the value of that cookie to the -i|--id option.
            Cookie sessionIdCookie = new Cookie()
            {
                Name = Properties.Settings.Default.sessionCookieName,
                Value = sessionId,
                Domain = Properties.Settings.Default.sessionCookieDomain
            };

            // Construct a GET web request.
            // Apply GET content type and useragent in an attempt to bypass any server webcrawler refusals.
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = "GET";
            request.Timeout = 20000;
            request.ContentType = Properties.Settings.Default.httpContentType;
            request.UserAgent = Properties.Settings.Default.httpUserAgent;

            // Append cookie to web request
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(sessionIdCookie);

            // Execute the web request, decode the response, and return the result as a string
            try
            {
                using (WebResponse response = request.GetResponse())
                    using (var reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.ASCII))
                        return reader.ReadToEnd();
            }

            // Print help text for common web exceptions before throwing.
            catch (WebException ex)
            {
                HttpWebResponse errorResponse = ex.Response as HttpWebResponse;

                switch (errorResponse.StatusCode)
                {
                    case HttpStatusCode.BadRequest: 
                        Console.WriteLine(Constants.WebException400Text, url);
                        throw;

                    case HttpStatusCode.NotFound:
                        Console.WriteLine(Constants.WebException404Text, url);
                        throw;

                    case HttpStatusCode.Forbidden:
                        Console.WriteLine(Constants.WebException401Text, url);
                        throw;

                    case HttpStatusCode.Unauthorized:
                        Console.WriteLine(Constants.WebException403Text, url);
                        throw;

                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// Parse a raw YCharts response string into a Dictionary 
        /// </summary>
        /// <param name="response">Raw YCharts http response string generated by DownloadResponseString</param>
        /// <returns>A Dictionary with Values containing historical financial data, and Keys containing the respective DateTime</returns>
        static Dictionary<DateTime, double> ParseResponseString(string response)
        {
            ////////////////////////////////////////////
            // Parse the downloaded ycharts JSON data //
            ////////////////////////////////////////////

            // Create a dictionary to contain parse results
            var data = new Dictionary<DateTime, double>();

            try
            {
                // Deserialize the downloaded JSON data using JSON.Net
                dynamic responseData = JsonConvert.DeserializeObject(response);

                // The JSON structure contains tokens echoing the queries we provided in the GET request.
                // It also contains a chart_data two dimensional array containing raw data for our security
                //   (the array would presumably have one axis to contain different tickers, 
                //    and a separate axis containing multiple data types for each ticker, 
                //    but we're not going to implement that).
                // Navigate through the above structure to get the raw security data.
                JArray chartData = responseData.chart_data[0][0].raw_data;

                // JSON.Net stores the raw security data as a collection of JToken objects.
                // The tokens are structured as [ Date, Value ] to provide the security's historic value at a given date.
                // Iterate through the tokens, parse the data they contain, and append it to the dictionary.
                foreach (JToken token in chartData)
                {
                    // Retrieve strings from the current token.
                    string unixDateString = (string)token[0];
                    string valueString = (string)token[1];

                    // Set a flag that we'll use to detect and log any parse errors
                    bool success = true;

                    // Attempt to parse the date for the current token, which is given in millisecond UNIX time.
                    DateTime date;
                    success &= Utilities.TryParseUnixTime(unixDateString, out date, timestampInMillis: true);

                    // Attempt to parse the value for the current token.
                    double value;
                    success &= double.TryParse(valueString, out value);

                    // If there weren't any parse errors, add the parse results to the dictionary.
                    // Else fail silently.
                    if (success)
                        data.Add(date, value);

                    Debug.WriteIf(!success, "JToken parse failed: " + token.ToString());
                }
            }
            // Print help text on parse failure before throwing.
            catch
            {
                Console.WriteLine("Failed to parse the JSON data downloaded from ycharts. They may have changed their formatting.");

                throw;
            }

            return data;
        }

        /// <summary>
        /// Write YCharts data to a CSV file
        /// </summary>
        /// <param name="data">YCharts data produced by ParseResponseString</param>
        /// <param name="path">Filepath to write CSV file</param>
        static void WriteAsCSV(Dictionary<DateTime, double> data, string path)
        {
            // Construct a StringBuilder to collect the parsed data in .csv format
            StringBuilder fileSb = new StringBuilder();

            // Define headers and append them to the .csv
            string delimiter = Properties.Settings.Default.outputDelimiter;
            string[] headers = { "Date", "Change" };
            fileSb.AppendFormat(headers.Aggregate((a, b) => a + delimiter + b));

            // Write data to the StringBuilder: one [ Date, Value ] pair per line, comma separated.
            foreach (var kvp in data)
                fileSb.AppendFormat("\r\n{0}{1}{2}", kvp.Key, delimiter, kvp.Value);

            // Finish building the file output string
            string fileString = fileSb.ToString();

            // Write the string to a file
            try
            {
                File.WriteAllText(path, fileString);
            }
            // Or print help text on write failure before throwing
            catch
            {
                Console.WriteLine(Constants.WriteErrorText, path, fileString);
                throw;
            }
        }
    }
}
