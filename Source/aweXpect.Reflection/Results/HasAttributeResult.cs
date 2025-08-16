using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Allows chaining of multiple attributes.
/// </summary>
public sealed class HasAttributeResult<TMember>(
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
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" />) specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public HasAttributeResult<TMember> OrHas<TAttribute>(bool inherit = true)
		where TAttribute : Attribute
	{
		attributeFilterOptions.RegisterAttribute<TAttribute>(inherit);
		return this;
	}

	/// <summary>
	///     Allows an alternative attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" />) specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public HasAttributeResult<TMember> OrHas<TAttribute>(
		Func<TAttribute, bool> predicate,
		bool inherit = true,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		attributeFilterOptions.RegisterAttribute(inherit, predicate, doNotPopulateThisValue);
		return this;
	}
}
