#nullable disable
namespace OneID.Domain.ValueObjects
{
    public sealed class DepartmentStatus : ValueObject
    {
        public static readonly DepartmentStatus Active = new("ACTIVE");
        public static readonly DepartmentStatus Inactive = new("INACTIVE");
        public string Value { get; }

        private DepartmentStatus(string value) => Value = value;

        public static DepartmentStatus From(string value)
        {
            return value?.ToUpperInvariant() switch
            {
                "ACTIVE" => Active,
                "INACTIVE" => Inactive,
                _ => throw new InvalidOperationException($"Invalid department status: '{value}'")
            };
        }

        public override string ToString() => Value;

        public override bool Equals(object obj)
            => obj is DepartmentStatus other && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator string(DepartmentStatus status) => status.Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;

        }
    }
}
