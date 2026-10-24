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
public class ObjectCountWithToleranceResult<TType, TThat, TElement, TTolerance>
	: AndOrResult<TType, TThat, ObjectCountWithToleranceResult<TType, TThat, TElement, TTolerance>>,
		IOptionsProvider<Quantifier>,
		IObjectEqualityWithToleranceResult<ObjectCountWithToleranceResult<TType, TThat, TElement, TTolerance>,
			TElement, TTolerance>
{
	private readonly ObjectEqualityWithToleranceOptions<TElement, TTolerance> _options;
	private readonly Quantifier _quantifier;
	private readonly TThat _returnValue;

	internal ObjectCountWithToleranceResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		Quantifier quantifier,
		ObjectEqualityWithToleranceOptions<TElement, TTolerance> options)
		: base(expectationBuilder, returnValue)
	{
		_returnValue = returnValue;
		_quantifier = quantifier;
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TElement> IOptionsProvider<ObjectEqualityOptions<TElement>>.Options => _options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityWithToleranceOptions<TElement, TTolerance>
		IOptionsProvider<ObjectEqualityWithToleranceOptions<TElement, TTolerance>>.Options => _options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	Quantifier IOptionsProvider<Quantifier>.Options => _quantifier;

	/// <inheritdoc cref="ObjectEqualityOptionsExtensions.Within{TSelf, TElement, TTolerance}(IObjectEqualityWithToleranceResult{TSelf, TElement, TTolerance}, TTolerance)" />
	public WithinResult Within(TTolerance tolerance)
	{
		_options.Within(tolerance);
		return new WithinResult(ExpectationBuilder, _returnValue, _quantifier, _options);
	}

	/// <summary>
	///     The result directly after <see cref="Within(TTolerance)" />, whose tolerance can still be made relative to
	///     the expected value, e.g. via <c>Percent()</c>.
	/// </summary>
	public sealed class WithinResult
		: ObjectCountWithToleranceResult<TType, TThat, TElement, TTolerance>,
			IWithinToleranceResult<ObjectCountWithToleranceResult<TType, TThat, TElement, TTolerance>, TTolerance>
	{
		internal WithinResult(
			ExpectationBuilder expectationBuilder,
			TThat returnValue,
			Quantifier quantifier,
			ObjectEqualityWithToleranceOptions<TElement, TTolerance> options)
			: base(expectationBuilder, returnValue, quantifier, options)
		{
		}

		/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
		IRelativeToleranceOptions<TTolerance> IOptionsProvider<IRelativeToleranceOptions<TTolerance>>.Options
			=> ((IOptionsProvider<ObjectEqualityWithToleranceOptions<TElement, TTolerance>>)this).Options;

		/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.Result" />
		ObjectCountWithToleranceResult<TType, TThat, TElement, TTolerance>
			IWithinToleranceResult<ObjectCountWithToleranceResult<TType, TThat, TElement, TTolerance>, TTolerance>.Result
			=> this;
	}
}
