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
