using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatFields
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="FieldInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IEnumerable<FieldInfo?>, FieldInfo?> Have<TAttribute>(
		this IThat<IEnumerable<FieldInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.Have(typeof(TAttribute));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="FieldInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" /> that matches the <paramref name="predicate" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IEnumerable<FieldInfo?>, FieldInfo?> Have<TAttribute>(
		this IThat<IEnumerable<FieldInfo?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="FieldInfo" /> have
	///     attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute.</exception>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IEnumerable<FieldInfo?>, FieldInfo?> Have(
		this IThat<IEnumerable<FieldInfo?>?> subject,
		Type attributeType)
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <summary>
	///     …have attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public static HaveAttributeWithoutInheritResult<IEnumerable<FieldInfo?>, FieldInfo?> Have<TAttribute>(
		this IEnumerableItems<FieldInfo?> items)
		where TAttribute : Attribute
		=> items.Have(typeof(TAttribute));

	/// <summary>
	///     …have attribute of type <typeparamref name="TAttribute" /> that matches the <paramref name="predicate" />.
	/// </summary>
	public static HaveAttributeWithoutInheritResult<IEnumerable<FieldInfo?>, FieldInfo?> Have<TAttribute>(
		this IEnumerableItems<FieldInfo?> items,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> items.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <summary>
	///     …have attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <inheritdoc cref="Have(IThat{IEnumerable{FieldInfo}}, Type)" path="/exception" />
	public static HaveAttributeWithoutInheritResult<IEnumerable<FieldInfo?>, FieldInfo?> Have(
		this IEnumerableItems<FieldInfo?> items,
		Type attributeType)
		=> items.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <inheritdoc cref="Have(IEnumerableItems{FieldInfo}, Type)" />
	public static HaveAttributeWithoutInheritResult<TCollection, FieldInfo?> Have<TCollection>(
		this IStructEnumerableItems<TCollection, FieldInfo?> items,
		Type attributeType)
		where TCollection : struct, IEnumerable<FieldInfo?>
	{
		HaveAttributeExpectation<FieldInfo> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{FieldInfo}})" />
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<FieldInfo?>, FieldInfo?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.Have(typeof(TAttribute));

	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{FieldInfo}}, Func{TAttribute, bool}, string)" />
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<FieldInfo?>, FieldInfo?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <inheritdoc cref="Have(IThat{IEnumerable{FieldInfo}}, Type)" />
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<FieldInfo?>, FieldInfo?> Have(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject,
		Type attributeType)
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <inheritdoc cref="Have{TAttribute}(IEnumerableItems{FieldInfo})" />
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<FieldInfo?>, FieldInfo?> Have<TAttribute>(
		this IAsyncEnumerableItems<FieldInfo?> items)
		where TAttribute : Attribute
		=> items.Have(typeof(TAttribute));

	/// <inheritdoc cref="Have{TAttribute}(IEnumerableItems{FieldInfo}, Func{TAttribute, bool}, string)" />
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<FieldInfo?>, FieldInfo?> Have<TAttribute>(
		this IAsyncEnumerableItems<FieldInfo?> items,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> items.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <inheritdoc cref="Have(IEnumerableItems{FieldInfo}, Type)" />
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<FieldInfo?>, FieldInfo?> Have(
		this IAsyncEnumerableItems<FieldInfo?> items,
		Type attributeType)
		=> items.HaveAttribute(AttributeOptions().WithAttribute(attributeType));
#endif

	private static AttributeFilterOptions<FieldInfo?> AttributeOptions()
		=> new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));

	private static HaveAttributeWithoutInheritResult<IEnumerable<FieldInfo?>, FieldInfo?> HaveAttribute(
		this IThat<IEnumerable<FieldInfo?>?> subject,
		AttributeFilterOptions<FieldInfo?> options)
	{
		HaveAttributeExpectation<FieldInfo> expectation = new(options);
		return new(subject.ExpectAll(expectation), subject, expectation.Filter);
	}

	private static HaveAttributeWithoutInheritResult<IEnumerable<FieldInfo?>, FieldInfo?> HaveAttribute(
		this IEnumerableItems<FieldInfo?> items,
		AttributeFilterOptions<FieldInfo?> options)
	{
		HaveAttributeExpectation<FieldInfo> expectation = new(options);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}

#if NET8_0_OR_GREATER
	private static HaveAttributeWithoutInheritResult<IAsyncEnumerable<FieldInfo?>, FieldInfo?> HaveAttribute(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject,
		AttributeFilterOptions<FieldInfo?> options)
	{
		HaveAttributeExpectation<FieldInfo> expectation = new(options);
		return new(subject.ExpectAll(expectation), subject, expectation.Filter);
	}

	private static HaveAttributeWithoutInheritResult<IAsyncEnumerable<FieldInfo?>, FieldInfo?> HaveAttribute(
		this IAsyncEnumerableItems<FieldInfo?> items,
		AttributeFilterOptions<FieldInfo?> options)
	{
		HaveAttributeExpectation<FieldInfo> expectation = new(options);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}
#endif
}
