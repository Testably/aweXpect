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
public class DelegateWithOptionalValueResult<TValue>(ExpectationBuilder expectationBuilder)
	: ExpectationResultWithoutValue<DelegateWithOptionalValueResult<TValue>>(expectationBuilder)
{
	/// <inheritdoc cref="DelegateWithValueResult{TValue}.WhoseResult" />
	public IThatSubject<TValue> WhoseResult => DelegateWithValueResult<TValue>.ContinueWithResult(ExpectationBuilder);
}
