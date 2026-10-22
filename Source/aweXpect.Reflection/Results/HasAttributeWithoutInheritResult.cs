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
public sealed class HasAttributeWithoutInheritResult<TType, TMember>
	: AndOrResult<TType, IThat<TMember>, HasAttributeWithoutInheritResult<TType, TMember>>,
		IOptionsProvider<AttributeFilterOptions<TMember>>
{
	private readonly AttributeFilterOptions<TMember> _attributeFilterOptions;

	internal HasAttributeWithoutInheritResult(
		ExpectationBuilder expectationBuilder,
		IThat<TMember> subject,
		AttributeFilterOptions<TMember> attributeFilterOptions)
		: base(expectationBuilder, subject)
	{
		_attributeFilterOptions = attributeFilterOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{AttributeFilterOptions}.Options" />
	AttributeFilterOptions<TMember> IOptionsProvider<AttributeFilterOptions<TMember>>.Options
		=> _attributeFilterOptions;

	/// <summary>
	///     Allows an alternative attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public HasAttributeWithoutInheritResult<TType, TMember> OrHas<TAttribute>()
		where TAttribute : Attribute
	{
		_attributeFilterOptions.RegisterAttribute<TAttribute>(true);
		return this;
	}

	/// <summary>
	///     Allows an alternative attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	public HasAttributeWithoutInheritResult<TType, TMember> OrHas<TAttribute>(
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		_attributeFilterOptions.RegisterAttribute(true, predicate, doNotPopulateThisValue.TrimExpression());
		return this;
	}
}
