using System.Security.Cryptography;
using System.Text;

namespace Ceataec.ExampleService.Infrastructure.Providers;

public sealed class HashProvider : IHashProvider
{
    public string Hash(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
