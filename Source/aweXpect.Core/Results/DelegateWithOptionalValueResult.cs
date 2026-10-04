using aweXpect.Core;

namespace aweXpect.Results;

/// <summary>
///     Result for a delegate with a value that does not throw an exception of a specific type.
/// </summary>
/// <remarks>
///     The delegate may have thrown an exception of another type instead of returning a value, so awaiting the result
///     returns no value. <see cref="WhoseResult" /> verifies the returned value, which requires that no exception was
///     thrown at all.
/// </remarks>
public class DelegateWithOptionalValueResult<T>(ExpectationBuilder expectationBuilder)
	: ExpectationResult(expectationBuilder)
{
	/// <inheritdoc cref="DelegateWithValueResult{T}.WhoseResult" />
	public IThat<T> WhoseResult => DelegateWithValueResult<T>.ContinueWithResult(ExpectationBuilder);
}
