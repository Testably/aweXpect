#if NETSTANDARD2_0
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace aweXpect.Polyfills;

/// <summary>
///     Provides extension methods to simplify writing platform independent tests.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class StringExtensionMethods
{
	/// <summary>
	///     Returns a value indicating whether a specified character occurs within this string, using the specified comparison
	///     rules.
	/// </summary>
	/// <returns>
	///     <see langword="true" /> if the <paramref name="value" /> parameter occurs withing this string; otherwise,
	///     <see langword="false" />.
	/// </returns>
	internal static bool Contains(
		this string @this,
		char value,
		StringComparison comparisonType)
		=> @this.IndexOf(value, comparisonType) >= 0;

	/// <summary>
	///     Returns a value indicating whether a specified character occurs within this string, using the specified comparison
	///     rules.
	/// </summary>
	/// <returns>
	///     <see langword="true" /> if the <paramref name="value" /> parameter occurs withing this string; otherwise,
	///     <see langword="false" />.
	/// </returns>
	internal static bool Contains(
		this string @this,
		string value,
		StringComparison comparisonType)
		=> @this.IndexOf(value, comparisonType) >= 0;

	/// <summary>
	///     Determines whether the end of this string instance matches the specified character.
	/// </summary>
	internal static bool EndsWith(
		this string @this,
		char value)
		=> @this.EndsWith($"{value}", StringComparison.Ordinal);

	/// <summary>
	///     Reports the zero-based index of the first occurrence of the specified Unicode character in this string. A parameter
	///     specifies the type of search to use for the specified character.
	/// </summary>
	/// <returns>
	///     The zero-based index of <paramref name="value" /> if that character is found, or <c>-1</c> if it is not.
	/// </returns>
	internal static int IndexOf(
		this string @this,
		char value,
		StringComparison comparisonType)
		=> @this.IndexOf($"{value}", comparisonType);

	/// <summary>
	///     Splits a string into substrings that are based on the provided string separator.
	/// </summary>
	/// <returns>
	///     An array whose elements contain the substrings from this instance that are delimited by separator.
	/// </returns>
	internal static string[] Split(
		this string @this,
		string separator,
		StringSplitOptions options = StringSplitOptions.None)
		=> @this.Split([separator,], options);

	/// <summary>
	///     Determines whether this string instance starts with the specified character.
	/// </summary>
	internal static bool StartsWith(
		this string @this,
		char value)
		=> @this.StartsWith($"{value}", StringComparison.Ordinal);

	/// <summary>
	///     Returns a new string in which all occurrences of a specified string in the current instance are replaced with
	///     another specified string, using the provided comparison type.
	/// </summary>
	internal static string Replace(this string @this,
		string oldValue,
		string? newValue,
		StringComparison comparisonType)
	{
		if (string.IsNullOrEmpty(oldValue))
		{
			// Let the BCL throw the same exceptions as the overload with a comparison type on newer frameworks.
			return @this.Replace(oldValue, newValue);
		}

		StringBuilder sb = new();
		int start = 0;
		int index;
		while ((index = @this.IndexOf(oldValue, start, comparisonType)) >= 0)
		{
			sb.Append(@this, start, index - start).Append(newValue);
			start = index + oldValue.Length;
		}

		return sb.Append(@this, start, @this.Length - start).ToString();
	}
}
#endif
