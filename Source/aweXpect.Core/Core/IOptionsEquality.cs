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

	/// <summary>
	///     Adds the contexts that explain a failure of a comparison with these options, e.g. the equivalency options.
	/// </summary>
	/// <remarks>
	///     Only called while a failure message is created.
	/// </remarks>
	void AppendContexts(ResultContextCollector contexts);
}
