using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YChartsDL
{
    static class Utilities
    {
        /// <summary>
        /// Try to parse a UNIX timestamp string
        /// </summary>
        /// <param name="unixTimestamp">The UNIX timestamp string to be parsed</param>
        /// <param name="time">The result of the parse attempt</param>
        /// <param name="timestampInMillis">If true the UNIX timestamp will be parsed in milliseconds rather than seconds</param>
        /// <returns>Returns true if the parse attempt succeeded</returns>
        public static bool TryParseUnixTime(
            string unixTimestamp, 
            out DateTime time, 
            bool timestampInMillis = false)
        {
            // Set the output to a default value to ensure an output if the parse attempt fails
            time = default(DateTime);

            // Attempt to parse the unix timestamp to a long integer
            long unixInt;
            bool stringParseSuccess =
                long.TryParse(unixTimestamp, out unixInt);

            // Return false if the parse attempt failed 
            if (!stringParseSuccess)
                return false;

            // Determine the multiplier needed to convert the unix timestamp to system ticks
            // depending on whether the timestamp is in seconds or milliseconds
            long tickMultiplier = timestampInMillis ?
                TimeSpan.TicksPerMillisecond :
                TimeSpan.TicksPerSecond;

            // Convert the timestamp to system ticks
            long ticks = (long)(unixInt * tickMultiplier);

            // Add the ticks to the unix origin (1970) and return the result as a DateTime
            time = new DateTime(Constants.UnixOrigin.Ticks + ticks);

            // Return true indicating a successful parse attempt
            return true;
        }
        
        // <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<DateTime, double>> InterpolateMissingValues(
            IEnumerable<KeyValuePair<DateTime, double>> input)
        {
            //
            if (input == null)
                throw new ArgumentException("Input must not be null");

            //
            IEnumerator<KeyValuePair<DateTime, double>> inputEn =
                input.GetEnumerator();

            //
            inputEn.MoveNext();

            //
            yield return inputEn.Current;

            //
            var last = inputEn.Current;

            //
            while (inputEn.MoveNext())
            {
                //
                int deltaDays = (inputEn.Current.Key - last.Key).Days;

                //
                if (deltaDays < 0)
                    throw new ArgumentException("Input data is not in chronological order");

                //
                for (int i = 1; i < deltaDays; i++)
                {
                    //
                    double a = last.Value;
                    double b = inputEn.Current.Value;
                    double r = (double)(i) / deltaDays;

                    //
                    double iValue = r * (b - a) + a;

                    //
                    DateTime iDate = last.Key.AddDays(i);

                    //
                    yield return new KeyValuePair<DateTime, double>(iDate, iValue);
                }

                //
                yield return inputEn.Current;

                //
                last = inputEn.Current;
            }
        }
    }
}
