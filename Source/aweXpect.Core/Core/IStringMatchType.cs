using System.Collections.Generic;
using System.Threading.Tasks;

namespace aweXpect.Core;

/// <summary>
///     The type defining how two strings are compared.
/// </summary>
public interface IStringMatchType
{
	/// <summary>
	///     Indicates whether the match type inspects the content of the subject instead of comparing it as a value.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> subject has no content to inspect, so an inspecting match type fails for it in both
	///     polarities, exactly like <c>StartsWith</c> and <c>DoesNotStartWith</c> do. A match type that compares the
	///     subject as a value, e.g. a custom equality, returns <see langword="false" />, so that a <see langword="null" />
	///     subject is equal to a <see langword="null" /> expected value and different from every other value.
	/// </remarks>
	bool InspectsSubject { get; }

	/// <summary>
	///     Returns <see langword="true" /> if the two strings <paramref name="actual" /> and <paramref name="expected" /> are
	///     considered equal; otherwise <see langword="false" />.
	/// </summary>
	/// <remarks>
	///     Return <see cref="StringMatchResult.NotComparable(string, System.Exception)" /> when the
	///     <paramref name="actual" /> value cannot be compared at all, e.g. a string that is no valid JSON, so that the
	///     expectation fails in both polarities, instead of the negated expectation succeeding. Start its reason with
	///     "it", which the failure message replaces with the name of the compared string, e.g. a member name.
	/// </remarks>
	ValueTask<StringMatchResult>
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

	/// <summary>
	///     Rejects options that the match type cannot honour, by throwing.
	/// </summary>
	/// <remarks>
	///     This is called when the match type is set, with the casing and the comparer that are specified so far, and
	///     before either of them changes afterwards, so that a conflict throws at the call that specifies it, in either
	///     order. It is never called with both <paramref name="ignoreCase" /> and a <paramref name="comparer" />.
	/// </remarks>
	void ValidateOptions(bool ignoreCase, IEqualityComparer<string>? comparer);

	/// <summary>
	///     Rejects an <paramref name="expected" /> value that the match type cannot use, by throwing.
	/// </summary>
	/// <remarks>
	///     This receives the value as
	///     <see cref="AreConsideredEqual(string?, string?, bool, IEqualityComparer{string})" /> does, and is called
	///     before a subject is compared with it and also when there is none to compare, e.g. for an empty collection,
	///     so that an unusable value is rejected whichever subject it is verified for.<br />
	///     It is called again for every item of a collection that is compared with the value, so it should be cheap
	///     for a value that it already accepted.
	/// </remarks>
	void ValidateExpected(string? expected);
}
