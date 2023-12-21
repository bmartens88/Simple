using System.Runtime.CompilerServices;

namespace Ardalis.GuardClauses;

public static class LengthGuard
{
    public static string Length(this IGuardClause guardClause, string input, int maxLength,
        [CallerArgumentExpression(nameof(input))]
        string? paramName = null)
    {
        if (input.Length >= maxLength)
            throw new ArgumentException($"Should not exceed length of {maxLength}!", paramName);
        return input;
    }
}