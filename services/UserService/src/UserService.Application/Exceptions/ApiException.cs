namespace UserService.Application.Exceptions;

public class ApiException : Exception
{
    public int StatusCode { get; }

    public string? Details { get; }

    public ApiException(string message, int statusCode, string? details = null)
        : base(message)
    {
        StatusCode = statusCode;
        Details = details;
    }
}