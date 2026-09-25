using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aweXpect.Core;

/// <summary>
///     The type defining how two strings are compared.
/// </summary>
public interface IStringMatchType
{
	/// <summary>
	///     Returns <see langword="true" /> if the two strings <paramref name="actual" /> and <paramref name="expected" /> are
	///     considered equal; otherwise <see langword="false" />.
	/// </summary>
	ValueTask<bool>
	AreConsideredEqual(string? actual, string? expected,
		bool ignoreCase,
		IEqualityComparer<string>? comparer);

	/// <summary>
	///     Get the expectations text.
	/// </summary>
	string GetExpectation(string? expected, ExpectationGrammars grammars);

	/// <summary>
	///     Get an extended failure text.
	/// </summary>
	string GetExtendedFailure(string it, string? actual, string? expected,
		bool ignoreCase,
		IEqualityComparer<string> comparer,
		StringDifferenceSettings? settings);

	/// <summary>
	///     A string representation of the match type.
	/// </summary>
	string GetTypeString();

	/// <summary>
	///     A string representation of the options.
	/// </summary>
	string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer);
}
