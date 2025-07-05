#nullable disable
using OneID;

namespace OneID.Domain.Entities.RabbitSettings
{
    public sealed class RabbitMqUrlSettings
    {
        public EnvironmentUrl Development { get; set; }
        public EnvironmentUrl Staging { get; set; }
        public EnvironmentUrl Production { get; set; }

    }
    public class EnvironmentUrl
    {
        public string BaseUrl { get; set; }
    }

}
