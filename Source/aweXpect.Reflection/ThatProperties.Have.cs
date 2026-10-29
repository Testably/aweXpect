using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatProperties
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HaveAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static HaveAttributeResult<IEnumerable<PropertyInfo?>, PropertyInfo?> Have<TAttribute>(
		this IThat<IEnumerable<PropertyInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.Have(typeof(TAttribute));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" /> that matches the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HaveAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static HaveAttributeResult<IEnumerable<PropertyInfo?>, PropertyInfo?> Have<TAttribute>(
		this IThat<IEnumerable<PropertyInfo?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> have
	///     attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HaveAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute.</exception>
	[GuaranteesNotNull]
	public static HaveAttributeResult<IEnumerable<PropertyInfo?>, PropertyInfo?> Have(
		this IThat<IEnumerable<PropertyInfo?>?> subject,
		Type attributeType)
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <summary>
	///     …have attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{PropertyInfo}})" path="/remarks" />
	public static HaveAttributeResult<IEnumerable<PropertyInfo?>, PropertyInfo?> Have<TAttribute>(
		this IEnumerableItems<PropertyInfo?> items)
		where TAttribute : Attribute
		=> items.Have(typeof(TAttribute));

	/// <summary>
	///     …have attribute of type <typeparamref name="TAttribute" /> that matches the <paramref name="predicate" />.
	/// </summary>
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{PropertyInfo}})" path="/remarks" />
	public static HaveAttributeResult<IEnumerable<PropertyInfo?>, PropertyInfo?> Have<TAttribute>(
		this IEnumerableItems<PropertyInfo?> items,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> items.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <summary>
	///     …have attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{PropertyInfo}})" path="/remarks" />
	/// <inheritdoc cref="Have(IThat{IEnumerable{PropertyInfo}}, Type)" path="/exception" />
	public static HaveAttributeResult<IEnumerable<PropertyInfo?>, PropertyInfo?> Have(
		this IEnumerableItems<PropertyInfo?> items,
		Type attributeType)
		=> items.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <inheritdoc cref="Have(IEnumerableItems{PropertyInfo}, Type)" />
	public static HaveAttributeResult<TCollection, PropertyInfo?> Have<TCollection>(
		this IStructEnumerableItems<TCollection, PropertyInfo?> items,
		Type attributeType)
		where TCollection : struct, IEnumerable<PropertyInfo?>
	{
		HaveAttributeExpectation<PropertyInfo> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{PropertyInfo}})" />
	[GuaranteesNotNull]
	public static HaveAttributeResult<IAsyncEnumerable<PropertyInfo?>, PropertyInfo?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<PropertyInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.Have(typeof(TAttribute));

	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{PropertyInfo}}, Func{TAttribute, bool}, string)" />
	[GuaranteesNotNull]
	public static HaveAttributeResult<IAsyncEnumerable<PropertyInfo?>, PropertyInfo?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<PropertyInfo?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <inheritdoc cref="Have(IThat{IEnumerable{PropertyInfo}}, Type)" />
	[GuaranteesNotNull]
	public static HaveAttributeResult<IAsyncEnumerable<PropertyInfo?>, PropertyInfo?> Have(
		this IThat<IAsyncEnumerable<PropertyInfo?>?> subject,
		Type attributeType)
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <inheritdoc cref="Have{TAttribute}(IEnumerableItems{PropertyInfo})" />
	public static HaveAttributeResult<IAsyncEnumerable<PropertyInfo?>, PropertyInfo?> Have<TAttribute>(
		this IAsyncEnumerableItems<PropertyInfo?> items)
		where TAttribute : Attribute
		=> items.Have(typeof(TAttribute));

	/// <inheritdoc cref="Have{TAttribute}(IEnumerableItems{PropertyInfo}, Func{TAttribute, bool}, string)" />
	public static HaveAttributeResult<IAsyncEnumerable<PropertyInfo?>, PropertyInfo?> Have<TAttribute>(
		this IAsyncEnumerableItems<PropertyInfo?> items,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> items.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <inheritdoc cref="Have(IEnumerableItems{PropertyInfo}, Type)" />
	public static HaveAttributeResult<IAsyncEnumerable<PropertyInfo?>, PropertyInfo?> Have(
		this IAsyncEnumerableItems<PropertyInfo?> items,
		Type attributeType)
		=> items.HaveAttribute(AttributeOptions().WithAttribute(attributeType));
#endif

	private static AttributeFilterOptions<PropertyInfo?> AttributeOptions()
		=> new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));

	private static HaveAttributeResult<IEnumerable<PropertyInfo?>, PropertyInfo?> HaveAttribute(
		this IThat<IEnumerable<PropertyInfo?>?> subject,
		AttributeFilterOptions<PropertyInfo?> options)
	{
		HaveAttributeExpectation<PropertyInfo> expectation = new(options);
		return new(subject.ExpectAll(expectation), subject, expectation.Filter);
	}

	private static HaveAttributeResult<IEnumerable<PropertyInfo?>, PropertyInfo?> HaveAttribute(
		this IEnumerableItems<PropertyInfo?> items,
		AttributeFilterOptions<PropertyInfo?> options)
	{
		HaveAttributeExpectation<PropertyInfo> expectation = new(options);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}

#if NET8_0_OR_GREATER
	private static HaveAttributeResult<IAsyncEnumerable<PropertyInfo?>, PropertyInfo?> HaveAttribute(
		this IThat<IAsyncEnumerable<PropertyInfo?>?> subject,
		AttributeFilterOptions<PropertyInfo?> options)
	{
		HaveAttributeExpectation<PropertyInfo> expectation = new(options);
		return new(subject.ExpectAll(expectation), subject, expectation.Filter);
	}

	private static HaveAttributeResult<IAsyncEnumerable<PropertyInfo?>, PropertyInfo?> HaveAttribute(
		this IAsyncEnumerableItems<PropertyInfo?> items,
		AttributeFilterOptions<PropertyInfo?> options)
	{
		HaveAttributeExpectation<PropertyInfo> expectation = new(options);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}
#endif
}
