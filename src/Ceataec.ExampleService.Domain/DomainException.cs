namespace Ceataec.ExampleService.Domain;

public sealed class DomainException : Exception
{
    public string? Code { get; }

    public DomainException(string message, string? code = null)
        : base(message)
    {
        Code = code;
    }
}
