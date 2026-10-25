#if NETSTANDARD2_0
using System;
using System.Diagnostics.CodeAnalysis;

namespace aweXpect.Web.Polyfills;

/// <summary>
///     Provides the overloads of <see cref="string" /> that are missing in .NET Standard 2.0.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class StringExtensionMethods
{
	/// <summary>
	///     Returns a value indicating whether a specified string occurs within this string, using the specified comparison
	///     rules.
	/// </summary>
	internal static bool Contains(
		this string @this,
		string value,
		StringComparison comparisonType)
		=> @this.IndexOf(value, comparisonType) >= 0;
}
#endif
