using System;
using Newtonsoft.Json.Linq;

namespace GooglePlaces
{
    public class PlaceDetail
    {
        public string Name { get; set; }

        public string FormattedAddress { get; set; }

        public Address Address { get; set; }

        public Location Location { get; set; }

        public string PhoneNumber { get; set; }

        public string WebsiteUrl { get; set; }

        public GooglePlaceType Types { get; set; }
    }

    [Flags]
    public enum GooglePlaceType
    {
        None = 0,
        Establishment = 1,
        PointOfInterest = 2
    }

    internal static class GooglePlaceTypeParser
    {
        public static GooglePlaceType Parse(JToken token)
        {
            var types = GooglePlaceType.None;

            foreach (var item in token)
            {
                var tmp = ParseItem((string)item);

                if (tmp != null)
                {
                    if (types == GooglePlaceType.None)
                        types = (GooglePlaceType)tmp;
                    else
                        types |= (GooglePlaceType)tmp;
                }
            }

            return types;
        }

        private static GooglePlaceType? ParseItem(string item)
        {
            switch (item)
            {
                case "point_of_interest": return GooglePlaceType.PointOfInterest;
                case "establishment": return GooglePlaceType.Establishment;
            }

            return null;
        }
    }
}