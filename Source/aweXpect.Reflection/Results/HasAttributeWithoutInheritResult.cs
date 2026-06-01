using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Allows chaining of multiple attributes for members whose attributes cannot be inherited
///     (fields and constructors).
/// </summary>
public sealed class HasAttributeWithoutInheritResult<TMember>(
	ExpectationBuilder expectationBuilder,
	IThat<TMember> subject,
	AttributeFilterOptions<TMember> attributeFilterOptions)
	: AndOrResult<TMember, IThat<TMember>>(expectationBuilder, subject),
		IOptionsProvider<AttributeFilterOptions<TMember>>
{
	/// <inheritdoc cref="IOptionsProvider{AttributeFilterOptions}.Options" />
	AttributeFilterOptions<TMember> IOptionsProvider<AttributeFilterOptions<TMember>>.Options
		=> attributeFilterOptions;

	/// <summary>
	///     Allows an alternative attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public HasAttributeWithoutInheritResult<TMember> OrHas<TAttribute>()
		where TAttribute : Attribute
	{
		attributeFilterOptions.RegisterAttribute<TAttribute>(true);
		return this;
	}

	/// <summary>
	///     Allows an alternative attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	public HasAttributeWithoutInheritResult<TMember> OrHas<TAttribute>(
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		attributeFilterOptions.RegisterAttribute(true, predicate, doNotPopulateThisValue.TrimCommonWhiteSpace());
		return this;
	}
}
