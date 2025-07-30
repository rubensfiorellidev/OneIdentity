using System.Diagnostics;

namespace OneID.Domain.OpenTelemetryEntity
{
    public static class Telemetry
    {
        public static readonly ActivitySource Source = new("oneid.auth");
    }

}
