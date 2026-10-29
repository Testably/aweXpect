using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?> DoNotHave<TAttribute>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.DoNotHave(typeof(TAttribute));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute or an open generic type.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?> DoNotHave(
		this IThat<IEnumerable<MethodInfo?>?> subject,
		Type attributeType)
	{
		HaveAttributeExpectation<MethodInfo> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(subject.ExpectNone(expectation), subject, expectation.Filter.Scope);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DoNotHave{TAttribute}(IThat{IEnumerable{MethodInfo}})" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?> DoNotHave<TAttribute>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.DoNotHave(typeof(TAttribute));

	/// <inheritdoc cref="DoNotHave(IThat{IEnumerable{MethodInfo}}, Type)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?> DoNotHave(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject,
		Type attributeType)
	{
		HaveAttributeExpectation<MethodInfo> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(subject.ExpectNone(expectation), subject, expectation.Filter.Scope);
	}
#endif
}
