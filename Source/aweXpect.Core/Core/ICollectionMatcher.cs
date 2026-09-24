using System.Threading.Tasks;

namespace aweXpect.Core;

/// <summary>
///     The matcher for collections.
/// </summary>
public interface ICollectionMatcher<in T, out T2> where T : T2
{
	/// <summary>
	///     Verifies for each <paramref name="value" /> in the subject if it results in a failure.
	/// </summary>
	/// <remarks>
	///     When returning <see langword="true" /> and the error is <see langword="null" /> this indicates that there
	///     are too many deviations.
	/// </remarks>
	/// <returns><see langword="true" /> when it results in a failure, otherwise <see langword="false" />.</returns>
#if NET8_0_OR_GREATER
	public ValueTask<(bool, string?)>
#else
	public Task<(bool, string?)>
#endif
		Verify(string it, T value, IOptionsEquality<T2> options, int maximumNumber);

	/// <summary>
	///     Verifies if it results in a failure when the enumeration is complete.
	/// </summary>
	/// <remarks>
	///     When returning <see langword="true" /> and the error is <see langword="null" /> this indicates that there
	///     are too many deviations.
	/// </remarks>
	/// <returns><see langword="true" /> when it results in a failure, otherwise <see langword="false" />.</returns>
#if NET8_0_OR_GREATER
	public ValueTask<(bool, string?)>
#else
	public Task<(bool, string?)>
#endif
		VerifyComplete(string it, IOptionsEquality<T2> options, int maximumNumber);
}
