namespace OneID.WebApp.Wrappers
{
    public record ApiResponse<T>(
      int HttpCode,
      string Message,
      bool Success,
      T Data
    );

}
