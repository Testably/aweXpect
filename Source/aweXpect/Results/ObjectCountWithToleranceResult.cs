using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying how often an item occurs in a collection, allowing a
///     <typeparamref name="TTolerance" /> on the comparison.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="QuantifierExtensions" /> and
///     <see cref="ObjectEqualityOptionsExtensions" />.
/// </remarks>
public class ObjectCountWithToleranceResult<TType, TThat, TElement, TTolerance>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	Quantifier quantifier,
	ObjectEqualityWithToleranceOptions<TElement, TTolerance> options)
	: AndOrResult<TType, TThat, ObjectCountWithToleranceResult<TType, TThat, TElement, TTolerance>>(
			expectationBuilder, returnValue),
		IOptionsProvider<Quantifier>,
		IObjectEqualityWithToleranceResult<ObjectCountWithToleranceResult<TType, TThat, TElement, TTolerance>,
			TElement, TTolerance>
{
	private readonly TThat _returnValue = returnValue;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TElement> IOptionsProvider<ObjectEqualityOptions<TElement>>.Options => options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityWithToleranceOptions<TElement, TTolerance>
		IOptionsProvider<ObjectEqualityWithToleranceOptions<TElement, TTolerance>>.Options => options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	Quantifier IOptionsProvider<Quantifier>.Options => quantifier;

	/// <inheritdoc cref="ObjectEqualityOptionsExtensions.Within{TSelf, TElement, TTolerance}(IObjectEqualityWithToleranceResult{TSelf, TElement, TTolerance}, TTolerance)" />
	public WithinResult Within(TTolerance tolerance)
	{
		options.Within(tolerance);
		return new WithinResult(ExpectationBuilder, _returnValue, quantifier, options);
	}

	/// <summary>
	///     The result directly after <see cref="Within(TTolerance)" />, whose tolerance can still be made relative to
	///     the expected value, e.g. via <c>Percent()</c>.
	/// </summary>
	public sealed class WithinResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		Quantifier quantifier,
		ObjectEqualityWithToleranceOptions<TElement, TTolerance> options)
		: ObjectCountWithToleranceResult<TType, TThat, TElement, TTolerance>(expectationBuilder, returnValue,
				quantifier, options),
			IWithinToleranceResult<ObjectCountWithToleranceResult<TType, TThat, TElement, TTolerance>, TTolerance>
	{
		/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
		IRelativeToleranceOptions<TTolerance> IOptionsProvider<IRelativeToleranceOptions<TTolerance>>.Options
			=> ((IOptionsProvider<ObjectEqualityWithToleranceOptions<TElement, TTolerance>>)this).Options;

		/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.Result" />
		ObjectCountWithToleranceResult<TType, TThat, TElement, TTolerance>
			IWithinToleranceResult<ObjectCountWithToleranceResult<TType, TThat, TElement, TTolerance>, TTolerance>.Result
			=> this;
	}
}
