#nullable disable
namespace OneID.Domain.ValueObjects
{
    public sealed class UserAccountStatus : ValueObject
    {
        public static readonly UserAccountStatus Active = new("ACTIVE");
        public static readonly UserAccountStatus Inactive = new("INACTIVE");

        public string Value { get; }

        public UserAccountStatus() => Value = null!;
        private UserAccountStatus(string value) => Value = value;

        public static UserAccountStatus From(string value)
        {
            return value?.ToUpperInvariant() switch
            {
                "ACTIVE" => Active,
                "INACTIVE" => Inactive,
                _ => throw new InvalidOperationException($"Invalid user account status: '{value}'")
            };
        }

        public override string ToString() => Value;

        public override bool Equals(object obj)
            => obj is UserAccountStatus other && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator string(UserAccountStatus status) => status.Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }

}
