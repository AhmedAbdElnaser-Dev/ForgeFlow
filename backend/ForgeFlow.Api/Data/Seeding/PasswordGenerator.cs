using System.Security.Cryptography;

namespace ForgeFlow.Api.Data.Seeding;

/// <summary>
/// Generates passwords that satisfy the default Identity policy: upper, lower,
/// digit and non-alphanumeric, with the remainder drawn from the full set.
/// </summary>
public static class PasswordGenerator
{
    private const string Upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const string Lower = "abcdefghijkmnopqrstuvwxyz";
    private const string Digits = "23456789";
    private const string Symbols = "!@#$%^&*?-_";

    public static string Generate(int length = 16)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(length, 8);

        var all = Upper + Lower + Digits + Symbols;
        var characters = new List<char>(length)
        {
            Pick(Upper),
            Pick(Lower),
            Pick(Digits),
            Pick(Symbols),
        };

        while (characters.Count < length)
        {
            characters.Add(Pick(all));
        }

        // Shuffle so the guaranteed characters are not always in the same positions.
        for (var i = characters.Count - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (characters[i], characters[j]) = (characters[j], characters[i]);
        }

        return new string([.. characters]);
    }

    private static char Pick(string source) => source[RandomNumberGenerator.GetInt32(source.Length)];
}
