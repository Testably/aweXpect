using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.EvaluationContext;

/// <summary>
///     The evaluation context.
/// </summary>
/// <remarks>
///     Use it by implementing <see cref="IContextConstraint{TValue}" />.
/// </remarks>
public interface IEvaluationContext
{
	/// <summary>
	///     Stores a <paramref name="value" /> under the <paramref name="key" /> in the evaluation context.
	/// </summary>
	void Store<T>(string key, T value);

	/// <summary>
	///     Tries to retrieve a previously stored <paramref name="value" /> under the <paramref name="key" /> from the
	///     evaluation context.
	/// </summary>
	bool TryReceive<T>(string key, [NotNullWhen(true)] out T? value);
}
