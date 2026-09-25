using System.Threading.Tasks;

namespace aweXpect.Core;

/// <summary>
///     Determines if two objects are considered equal.
/// </summary>
public interface IOptionsEquality<in TSubject>
{
	/// <summary>
	///     Returns <see langword="true" /> if the two objects <paramref name="actual" /> and <paramref name="expected" /> are
	///     considered equal; otherwise <see langword="false" />.
	/// </summary>
	ValueTask<bool>
		AreConsideredEqual<TExpected>(TSubject actual, TExpected expected);
}
