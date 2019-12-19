using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GooglePlaces
{
    public class GooglePlacesClient
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        private readonly string _apiKey;

        public GooglePlacesClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SearchResult>> SearchAsync(SearchCriteria criteria)
        {
            var client = new HttpClient();

            var uri = new Uri($"https://maps.googleapis.com/maps/api/place/nearbysearch/json?key={_apiKey}&location={criteria.Latitude},{criteria.Longitude}&rankby=distance&name={criteria.Name}");

            var json = await client.GetStringAsync(uri);

            var resultsJson = JObject.Parse(json);

            return resultsJson["results"]
                .Select(r => new SearchResult
                {
                    Name = (string)r["name"],
                    PlaceId = (string)r["place_id"]
                })
                .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datetime"></param>

            /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public async Task<TimeZoneInfo> GetTimeZoneInfoAsync(DateTime datetime, decimal latitude, decimal longitude)
        {
            var timestamp = (datetime.ToUniversalTime() - Epoch).TotalSeconds;

            var client = new HttpClient();

            var uri = new Uri($"https://maps.googleapis.com/maps/api/timezone/json?key={_apiKey}&location={latitude},{longitude}&timestamp={timestamp}");

            var json = await client.GetStringAsync(uri);

            return JsonConvert.DeserializeObject<TimeZoneInfo>(json);
        }

        public async Task<PlaceDetail> GetGooglePlaceDetailsAsync(string id)
        {
            var client = new HttpClient();

            var uri = new Uri($"https://maps.googleapis.com/maps/api/place/details/json?placeid={id}&key={_apiKey}");

            var json = await client.GetStringAsync(uri);

            var o = JObject.Parse(json);

            var googlePlace = o["result"];

            var addressQueryable = googlePlace["address_components"];

            // Proceed to create the new venue object

            string GetAddressPart(string partName, string prop)
            {
                var part = addressQueryable.SingleOrDefault(a => a["types"][0].ToString() == partName);

                return part?[prop]?.ToString();
            }
            
            var types = GooglePlaceTypeParser.Parse(googlePlace["types"]);
            
            return new PlaceDetail
            {
                Name = (string)googlePlace["name"],
                FormattedAddress = (string)googlePlace["formatted_address"],
                Address = new Address
                {
                    Line1 = GetAddressPart("street_number", "long_name") + " " + GetAddressPart("route", "short_name"),
                    City = GetAddressPart("locality", "long_name"),
                    State = GetAddressPart("administrative_area_level_1", "short_name"),
                    Country = GetAddressPart("country", "short_name"),
                    PostCode = GetAddressPart("postal_code", "long_name")
                },
                Location = new Location(
                    (decimal)googlePlace["geometry"]["location"]["lat"],
                    (decimal)googlePlace["geometry"]["location"]["lng"]
                ),
                WebsiteUrl = (string)googlePlace["Website"],
                PhoneNumber = (string)googlePlace["formatted_phone_number"],
                Types = types
            };
        }
    }
}
