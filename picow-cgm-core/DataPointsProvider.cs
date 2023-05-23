using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static picow_cgm_core.CgmReadingsSet;

namespace picow_cgm_core
{
    internal class DataPointsProvider
    {
        static HttpClient httpClient = new HttpClient();
        const string url = "https://toadhallcgm.azurewebsites.net/api/GetReadings";
        static List<DataPoint> cachedDatapoints = new List<DataPoint>();
        static DateTime cacheExpiresAt = DateTime.MinValue;

        static public List<DataPoint> GetDataPoints(int maxMinutes)
        {
            DateTime utcNow = DateTime.UtcNow;
            if (cacheExpiresAt < utcNow)
            {
                var response = httpClient.GetAsync(url).GetAwaiter().GetResult();
                var readingsSet = response.Content.ReadFromJsonAsync<CgmReadingsSet>().GetAwaiter().GetResult();

                List<DataPoint> dataPoints = new List<DataPoint>();
                if (readingsSet != null && readingsSet.items != null)
                {
                    dataPoints.AddRange(ReadingsToDataPoints(readingsSet.items, utcNow, maxMinutes));
                }

                cachedDatapoints = dataPoints;
                cacheExpiresAt = utcNow.AddMinutes(1);
                return dataPoints;
            }
            else
            {
                return cachedDatapoints;
            }

        }

        static private List<DataPoint> ReadingsToDataPoints(IEnumerable<ReadingItem> readings, DateTime asOfTime, int maxMinutes)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            foreach (var reading in readings)
            {
                TimeSpan ts = asOfTime.Subtract(reading.dateTime);
                DataPoint dp = new DataPoint() { TimeOffsetMinutes = ts.TotalMinutes, Reading = reading.convertedReading };
                if (dp.TimeOffsetMinutes <= maxMinutes && dp.TimeOffsetMinutes >= 0)
                {
                    dataPoints.Add(dp);
                }
            }
            return dataPoints;
        }
    }
}
