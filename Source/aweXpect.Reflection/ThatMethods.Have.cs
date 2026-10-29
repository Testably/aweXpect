using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HaveAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static HaveAttributeResult<IEnumerable<MethodInfo?>, MethodInfo?> Have<TAttribute>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.Have(typeof(TAttribute));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" /> that matches the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HaveAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static HaveAttributeResult<IEnumerable<MethodInfo?>, MethodInfo?> Have<TAttribute>(
		this IThat<IEnumerable<MethodInfo?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HaveAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute.</exception>
	[GuaranteesNotNull]
	public static HaveAttributeResult<IEnumerable<MethodInfo?>, MethodInfo?> Have(
		this IThat<IEnumerable<MethodInfo?>?> subject,
		Type attributeType)
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <summary>
	///     …have attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{MethodInfo}})" path="/remarks" />
	public static HaveAttributeResult<IEnumerable<MethodInfo?>, MethodInfo?> Have<TAttribute>(
		this IEnumerableItems<MethodInfo?> items)
		where TAttribute : Attribute
		=> items.Have(typeof(TAttribute));

	/// <summary>
	///     …have attribute of type <typeparamref name="TAttribute" /> that matches the <paramref name="predicate" />.
	/// </summary>
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{MethodInfo}})" path="/remarks" />
	public static HaveAttributeResult<IEnumerable<MethodInfo?>, MethodInfo?> Have<TAttribute>(
		this IEnumerableItems<MethodInfo?> items,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> items.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <summary>
	///     …have attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{MethodInfo}})" path="/remarks" />
	/// <inheritdoc cref="Have(IThat{IEnumerable{MethodInfo}}, Type)" path="/exception" />
	public static HaveAttributeResult<IEnumerable<MethodInfo?>, MethodInfo?> Have(
		this IEnumerableItems<MethodInfo?> items,
		Type attributeType)
		=> items.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <inheritdoc cref="Have(IEnumerableItems{MethodInfo}, Type)" />
	public static HaveAttributeResult<TCollection, MethodInfo?> Have<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items,
		Type attributeType)
		where TCollection : struct, IEnumerable<MethodInfo?>
	{
		HaveAttributeExpectation<MethodInfo> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{MethodInfo}})" />
	[GuaranteesNotNull]
	public static HaveAttributeResult<IAsyncEnumerable<MethodInfo?>, MethodInfo?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.Have(typeof(TAttribute));

	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{MethodInfo}}, Func{TAttribute, bool}, string)" />
	[GuaranteesNotNull]
	public static HaveAttributeResult<IAsyncEnumerable<MethodInfo?>, MethodInfo?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <inheritdoc cref="Have(IThat{IEnumerable{MethodInfo}}, Type)" />
	[GuaranteesNotNull]
	public static HaveAttributeResult<IAsyncEnumerable<MethodInfo?>, MethodInfo?> Have(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject,
		Type attributeType)
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <inheritdoc cref="Have{TAttribute}(IEnumerableItems{MethodInfo})" />
	public static HaveAttributeResult<IAsyncEnumerable<MethodInfo?>, MethodInfo?> Have<TAttribute>(
		this IAsyncEnumerableItems<MethodInfo?> items)
		where TAttribute : Attribute
		=> items.Have(typeof(TAttribute));

	/// <inheritdoc cref="Have{TAttribute}(IEnumerableItems{MethodInfo}, Func{TAttribute, bool}, string)" />
	public static HaveAttributeResult<IAsyncEnumerable<MethodInfo?>, MethodInfo?> Have<TAttribute>(
		this IAsyncEnumerableItems<MethodInfo?> items,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> items.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <inheritdoc cref="Have(IEnumerableItems{MethodInfo}, Type)" />
	public static HaveAttributeResult<IAsyncEnumerable<MethodInfo?>, MethodInfo?> Have(
		this IAsyncEnumerableItems<MethodInfo?> items,
		Type attributeType)
		=> items.HaveAttribute(AttributeOptions().WithAttribute(attributeType));
#endif

	private static AttributeFilterOptions<MethodInfo?> AttributeOptions()
		=> new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));

	private static HaveAttributeResult<IEnumerable<MethodInfo?>, MethodInfo?> HaveAttribute(
		this IThat<IEnumerable<MethodInfo?>?> subject,
		AttributeFilterOptions<MethodInfo?> options)
	{
		HaveAttributeExpectation<MethodInfo> expectation = new(options);
		return new(subject.ExpectAll(expectation), subject, expectation.Filter);
	}

	private static HaveAttributeResult<IEnumerable<MethodInfo?>, MethodInfo?> HaveAttribute(
		this IEnumerableItems<MethodInfo?> items,
		AttributeFilterOptions<MethodInfo?> options)
	{
		HaveAttributeExpectation<MethodInfo> expectation = new(options);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}

#if NET8_0_OR_GREATER
	private static HaveAttributeResult<IAsyncEnumerable<MethodInfo?>, MethodInfo?> HaveAttribute(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject,
		AttributeFilterOptions<MethodInfo?> options)
	{
		HaveAttributeExpectation<MethodInfo> expectation = new(options);
		return new(subject.ExpectAll(expectation), subject, expectation.Filter);
	}

	private static HaveAttributeResult<IAsyncEnumerable<MethodInfo?>, MethodInfo?> HaveAttribute(
		this IAsyncEnumerableItems<MethodInfo?> items,
		AttributeFilterOptions<MethodInfo?> options)
	{
		HaveAttributeExpectation<MethodInfo> expectation = new(options);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}
#endif
}
