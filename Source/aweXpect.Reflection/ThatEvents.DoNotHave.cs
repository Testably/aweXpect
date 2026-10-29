using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatEvents
{
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="EventInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<EventInfo?>, IEnumerable<EventInfo?>?> DoNotHave<TAttribute>(
		this IThat<IEnumerable<EventInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.DoNotHave(typeof(TAttribute));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="EventInfo" /> have
	///     attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<EventInfo?>, IEnumerable<EventInfo?>?> DoNotHave(
		this IThat<IEnumerable<EventInfo?>?> subject,
		Type attributeType)
	{
		HaveAttributeExpectation<EventInfo> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(subject.ExpectNone(expectation), subject, expectation.Filter.Scope);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DoNotHave{TAttribute}(IThat{IEnumerable{EventInfo}})" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<EventInfo?>, IAsyncEnumerable<EventInfo?>?> DoNotHave<TAttribute>(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.DoNotHave(typeof(TAttribute));

	/// <inheritdoc cref="DoNotHave(IThat{IEnumerable{EventInfo}}, Type)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<EventInfo?>, IAsyncEnumerable<EventInfo?>?> DoNotHave(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject,
		Type attributeType)
	{
		HaveAttributeExpectation<EventInfo> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(subject.ExpectNone(expectation), subject, expectation.Filter.Scope);
	}
#endif
}
