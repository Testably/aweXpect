using System.Threading.Tasks;

namespace aweXpect.Core;

/// <summary>
///     The matcher for collections.
/// </summary>
public interface ICollectionMatcher<in TItem, out TMatch> where TItem : TMatch
{
	/// <summary>
	///     Indicates that the values verified so far determine the result, so that the remaining values of the subject
	///     need not be verified before calling <see cref="VerifyComplete" />.
	/// </summary>
	/// <remarks>
	///     For the containment relation, this is the case once all expected items are found, as additional items cannot
	///     change the result.
	/// </remarks>
	bool IsDetermined { get; }

	/// <summary>
	///     Verifies for each <paramref name="value" /> in the subject if it results in a failure.
	/// </summary>
	/// <remarks>
	///     When returning <see langword="true" /> and the error is <see langword="null" /> this indicates that there
	///     are too many deviations.
	/// </remarks>
	/// <returns><see langword="true" /> when it results in a failure, otherwise <see langword="false" />.</returns>
	ValueTask<(bool IsFailure, string? Failure)>
		Verify(string it, TItem value, IEqualityEvaluator<TMatch> options, int maximumNumber);

	/// <summary>
	///     Verifies if it results in a failure when the enumeration is complete.
	/// </summary>
	/// <remarks>
	///     When returning <see langword="true" /> and the error is <see langword="null" /> this indicates that there
	///     are too many deviations.
	/// </remarks>
	/// <returns><see langword="true" /> when it results in a failure, otherwise <see langword="false" />.</returns>
	ValueTask<(bool IsFailure, string? Failure)>
		VerifyComplete(string it, IEqualityEvaluator<TMatch> options, int maximumNumber);
}
