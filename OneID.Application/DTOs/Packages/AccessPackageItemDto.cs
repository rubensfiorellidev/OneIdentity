namespace OneID.Application.DTOs.Packages
{
    public readonly record struct AccessPackageItemDto
    {
        public string Type { get; init; }
        public string Value { get; init; }

        public AccessPackageItemDto(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }

}
