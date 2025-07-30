using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace OneID.Shared.Tools
{
    public class ActivityEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var activity = Activity.Current;

            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(
                "TraceId", activity?.TraceId.ToString() ?? "-"));

            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(
                "SpanId", activity?.SpanId.ToString() ?? "-"));
        }
    }
}
