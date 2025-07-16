namespace OneID.Domain.ValueObjects
{
    public sealed class JobTitleStatus : ValueObject
    {
        public static readonly JobTitleStatus Active = new("ACTIVE");
        public static readonly JobTitleStatus Inactive = new("INACTIVE");

        public string Value { get; }

        private JobTitleStatus(string value)
        {
            Value = value;
        }

        public static JobTitleStatus From(string value)
        {
            return value?.ToUpperInvariant() switch
            {
                "ACTIVE" => Active,
                "INACTIVE" => Inactive,
                _ => throw new InvalidOperationException($"Invalid job title status: '{value}'")
            };
        }

        public override string ToString() => Value;

        public override bool Equals(object obj)
            => obj is JobTitleStatus other && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator string(JobTitleStatus status) => status.Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }

}
