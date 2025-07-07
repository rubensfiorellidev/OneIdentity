using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OneID.Domain.Helpers
{
    public static class EventSerializer
    {
        public static string SerializeToJson<T>(T eventObj)
        {
            if (eventObj == null)
            {
                throw new ArgumentNullException(nameof(eventObj), "Event object cannot be null.");
            }

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = [new StringEnumConverter()]
            };

            return JsonConvert.SerializeObject(eventObj, settings);
        }
    }
}
