using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Equivalency;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection has a specified number of items.
/// </summary>
/// <remarks>
///     <seealso cref="CountResult{TType,TThat,TSelf}" />
/// </remarks>
public class ObjectCountResult<TType, TThat, TElement>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	Quantifier quantifier,
	ObjectEqualityOptions<TElement> options)
	: ObjectCountResult<TType, TThat, TElement,
		ObjectCountResult<TType, TThat, TElement>>(
		expectationBuilder,
		returnValue,
		quantifier,
		options);

/// <summary>
///     The result for verifying that a collection has a specified number of items.
/// </summary>
/// <remarks>
///     <seealso cref="CountResult{TType,TThat,TSelf}" />
/// </remarks>
public class ObjectCountResult<TType, TThat, TElement, TSelf>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	Quantifier quantifier,
	ObjectEqualityOptions<TElement> options)
	: CountResult<TType, TThat, TSelf>(expectationBuilder, returnValue, quantifier),
		IOptionsProvider<ObjectEqualityOptions<TElement>>
	where TSelf : ObjectCountResult<TType, TThat, TElement, TSelf>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TElement> IOptionsProvider<ObjectEqualityOptions<TElement>>.Options => options;

	/// <summary>
	///     Uses the provided <paramref name="comparer" /> for comparing <see langword="object" />s.
	/// </summary>
	public TSelf Using(
		IEqualityComparer<object> comparer)
	{
		options.Using(comparer);
		return (TSelf)this;
	}
}
