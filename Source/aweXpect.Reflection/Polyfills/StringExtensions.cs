#if NETSTANDARD2_0
using System;
using System.Diagnostics.CodeAnalysis;

namespace aweXpect.Reflection.Polyfills;

/// <summary>
///     Provides the <see langword="char" /> overloads of <see cref="string" /> that are missing in .NET Standard 2.0.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class StringExtensionMethods
{
	/// <summary>
	///     Determines whether the end of this string instance matches the specified character.
	/// </summary>
	internal static bool EndsWith(
		this string @this,
		char value)
		=> @this.EndsWith($"{value}", StringComparison.Ordinal);

	/// <summary>
	///     Determines whether this string instance starts with the specified character.
	/// </summary>
	internal static bool StartsWith(
		this string @this,
		char value)
		=> @this.StartsWith($"{value}", StringComparison.Ordinal);
}
#endif
