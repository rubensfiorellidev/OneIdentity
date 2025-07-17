namespace OneID.Domain.Interfaces
{
    public interface INotification
    {
        string Id { get; }
        int Version { get; }
        string Message { get; }
        string PropertyName { get; }
        object Data { get; }
        bool Success { get; }
        int StatusCode { get; }

        IReadOnlyCollection<string> Messages { get; }

        void AddMessage(string message);
    }
}
