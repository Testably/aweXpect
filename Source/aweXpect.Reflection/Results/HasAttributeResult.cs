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
	private readonly AttributeFilter<TMember> _attributeFilter;

	internal HasAttributeResult(
		ExpectationBuilder expectationBuilder,
		IThat<TMember> subject,
		AttributeFilter<TMember> attributeFilter)
		: base(expectationBuilder, subject)
	{
		_attributeFilter = attributeFilter;
	}

	/// <summary>
	///     Allows an alternative attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public HasAttributeResult<TType, TMember> OrHas<TAttribute>()
		where TAttribute : Attribute
	{
		_attributeFilter.Options.Value = _attributeFilter.Options.Value.WithAttribute<TAttribute>();
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
		_attributeFilter.Options.Value = _attributeFilter.Options.Value.WithAttribute(predicate, doNotPopulateThisValue.TrimExpression());
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
		_attributeFilter.Scope.Directly();
		return this;
	}
}
