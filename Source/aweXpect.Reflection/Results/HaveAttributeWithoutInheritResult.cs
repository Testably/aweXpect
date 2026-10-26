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
public sealed class HaveAttributeWithoutInheritResult<TType, TMember>
	: AndOrResult<TType, IThat<TType?>, HaveAttributeWithoutInheritResult<TType, TMember>>
{
	private readonly AttributeFilter<TMember> _attributeFilter;

	internal HaveAttributeWithoutInheritResult(
		ExpectationBuilder expectationBuilder,
		IThat<TType?> subject,
		AttributeFilter<TMember> attributeFilter)
		: base(expectationBuilder, subject)
	{
		_attributeFilter = attributeFilter;
	}

	/// <summary>
	///     Allows an alternative attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public HaveAttributeWithoutInheritResult<TType, TMember> OrHave<TAttribute>()
		where TAttribute : Attribute
	{
		_attributeFilter.Options.Value = _attributeFilter.Options.Value.WithAttribute<TAttribute>();
		return this;
	}

	/// <summary>
	///     Allows an alternative attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	public HaveAttributeWithoutInheritResult<TType, TMember> OrHave<TAttribute>(
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		_attributeFilter.Options.Value = _attributeFilter.Options.Value.WithAttribute(predicate, doNotPopulateThisValue.TrimExpression());
		return this;
	}
}
