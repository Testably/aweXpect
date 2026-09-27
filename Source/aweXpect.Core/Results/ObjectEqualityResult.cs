using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />.
///     <para />
///     In addition to the combinations from <see cref="AndOrResult{TType,TThat}" />, allows specifying
///     options on the <see cref="ObjectEqualityOptions" />.
/// </summary>
public class ObjectEqualityResult<TType, TThat, TElement>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	ObjectEqualityOptions<TElement> options)
	: ObjectEqualityResult<TType, TThat, TElement,
		ObjectEqualityResult<TType, TThat, TElement>>(
		expectationBuilder,
		returnValue,
		options);

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />.
///     <para />
///     In addition to the combinations from <see cref="AndOrResult{TType,TThat}" />, allows specifying
///     options on the <see cref="ObjectEqualityOptions" />.
/// </summary>
public class ObjectEqualityResult<TType, TThat, TElement, TSelf>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	ObjectEqualityOptions<TElement> options)
	: AndOrResult<TType, TThat, TSelf>(expectationBuilder, returnValue),
		IOptionsProvider<ObjectEqualityOptions<TElement>>
	where TSelf : ObjectEqualityResult<TType, TThat, TElement, TSelf>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TElement> IOptionsProvider<ObjectEqualityOptions<TElement>>.Options => options;

	/// <summary>
	///     Uses the provided <paramref name="comparer" /> for comparing <see langword="object" />s.
	/// </summary>
	public TSelf Using(IEqualityComparer<object> comparer)
	{
		options.Using(comparer);
		return (TSelf)this;
	}

	/// <summary>
	///     Uses the provided <paramref name="comparer" /> for comparing <typeparamref name="TElement" /> values.
	/// </summary>
	public TSelf Using(IEqualityComparer<TElement> comparer)
	{
		options.Using(comparer);
		return (TSelf)this;
	}
}
