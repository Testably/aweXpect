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
public class ObjectCountWithToleranceResult<TType, TThat, TItem, TTolerance>
	: AndOrResult<TType, TThat, ObjectCountWithToleranceResult<TType, TThat, TItem, TTolerance>>,
		IOptionsProvider<Quantifier>,
		IObjectEqualityWithToleranceResult<ObjectCountWithToleranceResult<TType, TThat, TItem, TTolerance>,
			TItem, TTolerance>
{
	private readonly ObjectEqualityWithToleranceOptions<TItem, TTolerance> _options;
	private readonly Quantifier _quantifier;
	private readonly TThat _returnValue;

	internal ObjectCountWithToleranceResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		Quantifier quantifier,
		ObjectEqualityWithToleranceOptions<TItem, TTolerance> options)
		: base(expectationBuilder, returnValue)
	{
		_returnValue = returnValue;
		_quantifier = quantifier;
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TItem> IOptionsProvider<ObjectEqualityOptions<TItem>>.Options => _options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityWithToleranceOptions<TItem, TTolerance>
		IOptionsProvider<ObjectEqualityWithToleranceOptions<TItem, TTolerance>>.Options => _options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	Quantifier IOptionsProvider<Quantifier>.Options => _quantifier;

	/// <inheritdoc cref="ObjectEqualityOptionsExtensions.Within{TSelf, TItem, TTolerance}(IObjectEqualityWithToleranceResult{TSelf, TItem, TTolerance}, TTolerance)" />
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
		: ObjectCountWithToleranceResult<TType, TThat, TItem, TTolerance>,
			IWithinToleranceResult<ObjectCountWithToleranceResult<TType, TThat, TItem, TTolerance>, TTolerance>
	{
		internal WithinResult(
			ExpectationBuilder expectationBuilder,
			TThat returnValue,
			Quantifier quantifier,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options)
			: base(expectationBuilder, returnValue, quantifier, options)
		{
		}

		/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
		IRelativeToleranceOptions<TTolerance> IOptionsProvider<IRelativeToleranceOptions<TTolerance>>.Options
			=> ((IOptionsProvider<ObjectEqualityWithToleranceOptions<TItem, TTolerance>>)this).Options;

		/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.Result" />
		ObjectCountWithToleranceResult<TType, TThat, TItem, TTolerance>
			IWithinToleranceResult<ObjectCountWithToleranceResult<TType, TThat, TItem, TTolerance>, TTolerance>.Result
			=> this;
	}
}
