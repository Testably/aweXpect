using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Allows chaining of multiple attributes for subjects whose attributes cannot be inherited
///     (assemblies, constructors and fields).
/// </summary>
public sealed class HaveAttributeWithoutInheritResult<TMember, TResult>
	: AndOrResult<TResult, IThat<TResult?>, HaveAttributeWithoutInheritResult<TMember, TResult>>,
		IOptionsProvider<AttributeFilterOptions<TMember>>
{
	private readonly AttributeFilterOptions<TMember> _attributeFilterOptions;

	internal HaveAttributeWithoutInheritResult(
		ExpectationBuilder expectationBuilder,
		IThat<TResult?> subject,
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
	public HaveAttributeWithoutInheritResult<TMember, TResult> OrHave<TAttribute>()
		where TAttribute : Attribute
	{
		_attributeFilterOptions.RegisterAttribute<TAttribute>();
		return this;
	}

	/// <summary>
	///     Allows an alternative attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	public HaveAttributeWithoutInheritResult<TMember, TResult> OrHave<TAttribute>(
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		_attributeFilterOptions.RegisterAttribute(predicate, doNotPopulateThisValue.TrimExpression());
		return this;
	}
}
