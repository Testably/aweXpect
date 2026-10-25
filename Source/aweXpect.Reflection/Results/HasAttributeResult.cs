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
public sealed class HasAttributeResult<TType, TMember>
	: AndOrResult<TType, IThat<TMember>, HasAttributeResult<TType, TMember>>
{
	private readonly AttributeFilterOptions<TMember> _attributeFilterOptions;

	internal HasAttributeResult(
		ExpectationBuilder expectationBuilder,
		IThat<TMember> subject,
		AttributeFilterOptions<TMember> attributeFilterOptions)
		: base(expectationBuilder, subject)
	{
		_attributeFilterOptions = attributeFilterOptions;
	}

	/// <summary>
	///     Allows an alternative attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public HasAttributeResult<TType, TMember> OrHas<TAttribute>()
		where TAttribute : Attribute
	{
		_attributeFilterOptions.RegisterAttribute<TAttribute>();
		return this;
	}

	/// <summary>
	///     Allows an alternative attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	public HasAttributeResult<TType, TMember> OrHas<TAttribute>(
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
	///     Applies to all attributes of the expectation, also to those added via <see cref="OrHas{TAttribute}()" />.
	/// </remarks>
	public HasAttributeResult<TType, TMember> Directly()
	{
		_attributeFilterOptions.Scope.Directly();
		return this;
	}
}
