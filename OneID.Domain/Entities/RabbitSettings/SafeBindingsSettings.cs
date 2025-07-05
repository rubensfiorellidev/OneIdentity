namespace OneID.Domain.Entities.RabbitSettings
{
    public sealed class SafeBindingsSettings
    {
        public string? Queue { get; set; }
        public string? Exchange { get; set; }
        public string? ForbiddenRoutingKey { get; set; }

    }
}
