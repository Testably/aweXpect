using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotHave<TAttribute>(
		this IThat<IEnumerable<Type?>?> subject)
		where TAttribute : Attribute
		=> subject.DoNotHave(typeof(TAttribute));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have
	///     attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotHave(
		this IThat<IEnumerable<Type?>?> subject,
		Type attributeType)
	{
		HaveAttributeExpectation<Type> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(subject.ExpectNone(expectation), subject, expectation.Filter.Scope);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DoNotHave{TAttribute}(IThat{IEnumerable{Type}})" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotHave<TAttribute>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		where TAttribute : Attribute
		=> subject.DoNotHave(typeof(TAttribute));

	/// <inheritdoc cref="DoNotHave(IThat{IEnumerable{Type}}, Type)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotHave(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type attributeType)
	{
		HaveAttributeExpectation<Type> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(subject.ExpectNone(expectation), subject, expectation.Filter.Scope);
	}
#endif
}
