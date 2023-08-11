using System.Security.Cryptography;
using System.Text;

namespace RecordAppBlazor.Function;

public static class Utiles
{
    public static string ToMD5(this string? raw)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(raw);
        var hash = md5.ComputeHash(bytes);
        var sb = new StringBuilder();
        foreach (var b in hash)
        {
            sb.Append(b.ToString("X2"));
        }

        return sb.ToString();
    }
}