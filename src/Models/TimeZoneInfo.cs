using System;

namespace GooglePlaces
{
    public class TimeZoneInfo
    {
        public int DSTOffset { get; set; }

        public int RawOffset { get; set; }

        public string TimeZoneId { get; set; }

        public string TimeZoneName { get; set; }

        public TimeSpan Offset => TimeSpan.FromSeconds(RawOffset + DSTOffset);
    }
}