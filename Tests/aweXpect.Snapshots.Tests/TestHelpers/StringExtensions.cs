#if NETFRAMEWORK
namespace aweXpect.Snapshots.Tests.TestHelpers;

/// <summary>
///     Polyfill for the <see langword="char" /> overload of <see cref="string.StartsWith(string)" />, which .NET Framework
///     lacks.
/// </summary>
internal static class StringExtensions
{
	public static bool StartsWith(this string value, char prefix)
		=> value.Length > 0 && value[0] == prefix;
}
#endif
