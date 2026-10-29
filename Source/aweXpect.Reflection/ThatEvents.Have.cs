using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatEvents
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HaveAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static HaveAttributeResult<IEnumerable<EventInfo?>, EventInfo?> Have<TAttribute>(
		this IThat<IEnumerable<EventInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.Have(typeof(TAttribute));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" /> that matches the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HaveAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static HaveAttributeResult<IEnumerable<EventInfo?>, EventInfo?> Have<TAttribute>(
		this IThat<IEnumerable<EventInfo?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="EventInfo" /> have
	///     attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HaveAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute or an open generic type.</exception>
	[GuaranteesNotNull]
	public static HaveAttributeResult<IEnumerable<EventInfo?>, EventInfo?> Have(
		this IThat<IEnumerable<EventInfo?>?> subject,
		Type attributeType)
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <summary>
	///     …have attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{EventInfo}})" path="/remarks" />
	public static HaveAttributeResult<IEnumerable<EventInfo?>, EventInfo?> Have<TAttribute>(
		this IEnumerableItems<EventInfo?> items)
		where TAttribute : Attribute
		=> items.Have(typeof(TAttribute));

	/// <summary>
	///     …have attribute of type <typeparamref name="TAttribute" /> that matches the <paramref name="predicate" />.
	/// </summary>
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{EventInfo}})" path="/remarks" />
	public static HaveAttributeResult<IEnumerable<EventInfo?>, EventInfo?> Have<TAttribute>(
		this IEnumerableItems<EventInfo?> items,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> items.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <summary>
	///     …have attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{EventInfo}})" path="/remarks" />
	/// <inheritdoc cref="Have(IThat{IEnumerable{EventInfo}}, Type)" path="/exception" />
	public static HaveAttributeResult<IEnumerable<EventInfo?>, EventInfo?> Have(
		this IEnumerableItems<EventInfo?> items,
		Type attributeType)
		=> items.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <inheritdoc cref="Have(IEnumerableItems{EventInfo}, Type)" />
	public static HaveAttributeResult<TCollection, EventInfo?> Have<TCollection>(
		this IStructEnumerableItems<TCollection, EventInfo?> items,
		Type attributeType)
		where TCollection : struct, IEnumerable<EventInfo?>
	{
		HaveAttributeExpectation<EventInfo> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{EventInfo}})" />
	[GuaranteesNotNull]
	public static HaveAttributeResult<IAsyncEnumerable<EventInfo?>, EventInfo?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.Have(typeof(TAttribute));

	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{EventInfo}}, Func{TAttribute, bool}, string)" />
	[GuaranteesNotNull]
	public static HaveAttributeResult<IAsyncEnumerable<EventInfo?>, EventInfo?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <inheritdoc cref="Have(IThat{IEnumerable{EventInfo}}, Type)" />
	[GuaranteesNotNull]
	public static HaveAttributeResult<IAsyncEnumerable<EventInfo?>, EventInfo?> Have(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject,
		Type attributeType)
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <inheritdoc cref="Have{TAttribute}(IEnumerableItems{EventInfo})" />
	public static HaveAttributeResult<IAsyncEnumerable<EventInfo?>, EventInfo?> Have<TAttribute>(
		this IAsyncEnumerableItems<EventInfo?> items)
		where TAttribute : Attribute
		=> items.Have(typeof(TAttribute));

	/// <inheritdoc cref="Have{TAttribute}(IEnumerableItems{EventInfo}, Func{TAttribute, bool}, string)" />
	public static HaveAttributeResult<IAsyncEnumerable<EventInfo?>, EventInfo?> Have<TAttribute>(
		this IAsyncEnumerableItems<EventInfo?> items,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> items.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <inheritdoc cref="Have(IEnumerableItems{EventInfo}, Type)" />
	public static HaveAttributeResult<IAsyncEnumerable<EventInfo?>, EventInfo?> Have(
		this IAsyncEnumerableItems<EventInfo?> items,
		Type attributeType)
		=> items.HaveAttribute(AttributeOptions().WithAttribute(attributeType));
#endif

	private static AttributeFilterOptions<EventInfo?> AttributeOptions()
		=> new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));

	private static HaveAttributeResult<IEnumerable<EventInfo?>, EventInfo?> HaveAttribute(
		this IThat<IEnumerable<EventInfo?>?> subject,
		AttributeFilterOptions<EventInfo?> options)
	{
		HaveAttributeExpectation<EventInfo> expectation = new(options);
		return new(subject.ExpectAll(expectation), subject, expectation.Filter);
	}

	private static HaveAttributeResult<IEnumerable<EventInfo?>, EventInfo?> HaveAttribute(
		this IEnumerableItems<EventInfo?> items,
		AttributeFilterOptions<EventInfo?> options)
	{
		HaveAttributeExpectation<EventInfo> expectation = new(options);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}

#if NET8_0_OR_GREATER
	private static HaveAttributeResult<IAsyncEnumerable<EventInfo?>, EventInfo?> HaveAttribute(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject,
		AttributeFilterOptions<EventInfo?> options)
	{
		HaveAttributeExpectation<EventInfo> expectation = new(options);
		return new(subject.ExpectAll(expectation), subject, expectation.Filter);
	}

	private static HaveAttributeResult<IAsyncEnumerable<EventInfo?>, EventInfo?> HaveAttribute(
		this IAsyncEnumerableItems<EventInfo?> items,
		AttributeFilterOptions<EventInfo?> options)
	{
		HaveAttributeExpectation<EventInfo> expectation = new(options);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}
#endif
}
