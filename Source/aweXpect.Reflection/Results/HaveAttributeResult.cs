using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Allows chaining of multiple attributes.
/// </summary>
public sealed class HaveAttributeResult<TMember, TResult>
	: AndOrResult<TResult, IThat<TResult?>, HaveAttributeResult<TMember, TResult>>,
		IOptionsProvider<AttributeFilterOptions<TMember>>
{
	private readonly AttributeFilterOptions<TMember> _attributeFilterOptions;

	internal HaveAttributeResult(
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
	public HaveAttributeResult<TMember, TResult> OrHave<TAttribute>()
		where TAttribute : Attribute
	{
		_attributeFilterOptions.RegisterAttribute<TAttribute>();
		return this;
	}

	/// <summary>
	///     Allows an alternative attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	public HaveAttributeResult<TMember, TResult> OrHave<TAttribute>(
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		_attributeFilterOptions.RegisterAttribute(predicate, doNotPopulateThisValue.TrimExpression());
		return this;
	}

	/// <summary>
	///     Only considers the attributes declared directly on the member, not those inherited from a base type or an
	///     overridden member.
	/// </summary>
	/// <remarks>
	///     Applies to all attributes of the expectation, also to those added via <see cref="OrHave{TAttribute}()" />.
	/// </remarks>
	public HaveAttributeResult<TMember, TResult> Directly()
	{
		_attributeFilterOptions.Scope.Directly();
		return this;
	}
}
