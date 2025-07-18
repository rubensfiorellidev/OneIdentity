namespace OneID.Domain.ValueObjects
{
    public sealed class AdmissionStatus : ValueObject
    {
        public static readonly AdmissionStatus Pending = new("PENDING");

        public string Value { get; }
        private AdmissionStatus(string value) => Value = value;

        public static AdmissionStatus From(string value)
        {
            return value?.ToUpperInvariant() switch
            {
                "PENDING" => Pending,
                _ => throw new InvalidOperationException($"Ivalid status '{value}")
            };
        }
        public override string ToString() => Value;

        public override bool Equals(object obj)
            => obj is AdmissionStatus other && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator string(AdmissionStatus status) => status.Value;
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
