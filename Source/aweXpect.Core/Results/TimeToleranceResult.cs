using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />.
///     <para />
///     In addition to the combinations from <see cref="AndOrResult{TType,TThat}" />, allows specifying a
///     tolerance via <see cref="ToleranceExtensions" />.
/// </summary>
public class TimeToleranceResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	TimeTolerance options)
	: AndOrResult<TType, TThat, TimeToleranceResult<TType, TThat>>(expectationBuilder, returnValue),
		IOptionsProvider<TimeTolerance>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	TimeTolerance IOptionsProvider<TimeTolerance>.Options => options;
}
