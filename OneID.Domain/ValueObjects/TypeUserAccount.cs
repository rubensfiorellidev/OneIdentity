namespace OneID.Domain.ValueObjects
{
    public sealed class TypeUserAccount : ValueObject
    {
        public static readonly TypeUserAccount Clt = new("CLT");
        public static readonly TypeUserAccount Pj = new("PJ");

        public string Value { get; }

        private TypeUserAccount(string value) => Value = value;

        public static TypeUserAccount From(string value)
        {
            return value?.ToUpperInvariant() switch
            {
                "CLT" => Clt,
                "PJ" => Pj,
                _ => throw new InvalidOperationException($"Invalid user account type: '{value}'")
            };
        }

        public override string ToString() => Value;

        public override bool Equals(object obj)
            => obj is TypeUserAccount other && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator string(TypeUserAccount type) => type.Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }

}
