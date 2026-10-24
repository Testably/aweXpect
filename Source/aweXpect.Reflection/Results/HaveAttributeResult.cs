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
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" />) specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public HaveAttributeResult<TMember, TResult> OrHave<TAttribute>(bool inherit = true)
		where TAttribute : Attribute
	{
		_attributeFilterOptions.RegisterAttribute<TAttribute>(inherit);
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
	public HaveAttributeResult<TMember, TResult> OrHave<TAttribute>(
		Func<TAttribute, bool> predicate,
		bool inherit = true,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		_attributeFilterOptions.RegisterAttribute(inherit, predicate, doNotPopulateThisValue.TrimExpression());
		return this;
	}
}
