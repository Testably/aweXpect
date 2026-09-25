using System.Threading.Tasks;

namespace aweXpect.Core;

/// <summary>
///     The type defining how two objects are compared.
/// </summary>
public interface IObjectMatchType
{
	/// <summary>
	///     Returns <see langword="true" /> if the two objects <paramref name="actual" /> and <paramref name="expected" /> are
	///     considered equal; otherwise <see langword="false" />.
	/// </summary>
	ValueTask<bool> AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected);

	/// <summary>
	///     Get the expectations text.
	/// </summary>
	string GetExpectation(string expected, ExpectationGrammars grammars);

	/// <summary>
	///     Get an extended failure text.
	/// </summary>
	string GetExtendedFailure(string it, ExpectationGrammars grammars, object? actual, object? expected);

	/// <summary>
	///     Get the expectation text for a single expected item, e.g. <c>equivalent to {expected}</c>.
	/// </summary>
	/// <remarks>
	///     A match type that describes the item instead of only formatting it prepends the
	///     <paramref name="itemNoun" />, so that a verb such as <c>contains</c> keeps a direct object.<br />
	///     A match type that only formats the value prepends the <paramref name="comparison" /> instead, so that a verb
	///     which already names the item reads <c>has item equal to 3</c>.
	/// </remarks>
	string PrependItemAndComparison(string expected, string? itemNoun = null, string? comparison = null);
}
