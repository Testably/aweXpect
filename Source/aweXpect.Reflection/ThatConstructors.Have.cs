using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatConstructors
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IEnumerable<ConstructorInfo?>, ConstructorInfo?> Have<TAttribute>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.Have(typeof(TAttribute));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" /> that matches the <paramref name="predicate" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IEnumerable<ConstructorInfo?>, ConstructorInfo?> Have<TAttribute>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute.</exception>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IEnumerable<ConstructorInfo?>, ConstructorInfo?> Have(
		this IThat<IEnumerable<ConstructorInfo?>?> subject,
		Type attributeType)
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <summary>
	///     …have attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public static HaveAttributeWithoutInheritResult<IEnumerable<ConstructorInfo?>, ConstructorInfo?> Have<TAttribute>(
		this IEnumerableItems<ConstructorInfo?> items)
		where TAttribute : Attribute
		=> items.Have(typeof(TAttribute));

	/// <summary>
	///     …have attribute of type <typeparamref name="TAttribute" /> that matches the <paramref name="predicate" />.
	/// </summary>
	public static HaveAttributeWithoutInheritResult<IEnumerable<ConstructorInfo?>, ConstructorInfo?> Have<TAttribute>(
		this IEnumerableItems<ConstructorInfo?> items,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> items.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <summary>
	///     …have attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <inheritdoc cref="Have(IThat{IEnumerable{ConstructorInfo}}, Type)" path="/exception" />
	public static HaveAttributeWithoutInheritResult<IEnumerable<ConstructorInfo?>, ConstructorInfo?> Have(
		this IEnumerableItems<ConstructorInfo?> items,
		Type attributeType)
		=> items.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <inheritdoc cref="Have(IEnumerableItems{ConstructorInfo}, Type)" />
	public static HaveAttributeWithoutInheritResult<TCollection, ConstructorInfo?> Have<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items,
		Type attributeType)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
	{
		HaveAttributeExpectation<ConstructorInfo> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{ConstructorInfo}})" />
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<ConstructorInfo?>, ConstructorInfo?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.Have(typeof(TAttribute));

	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{ConstructorInfo}}, Func{TAttribute, bool}, string)" />
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<ConstructorInfo?>, ConstructorInfo?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <inheritdoc cref="Have(IThat{IEnumerable{ConstructorInfo}}, Type)" />
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<ConstructorInfo?>, ConstructorInfo?> Have(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject,
		Type attributeType)
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <inheritdoc cref="Have{TAttribute}(IEnumerableItems{ConstructorInfo})" />
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<ConstructorInfo?>, ConstructorInfo?> Have<TAttribute>(
		this IAsyncEnumerableItems<ConstructorInfo?> items)
		where TAttribute : Attribute
		=> items.Have(typeof(TAttribute));

	/// <inheritdoc cref="Have{TAttribute}(IEnumerableItems{ConstructorInfo}, Func{TAttribute, bool}, string)" />
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<ConstructorInfo?>, ConstructorInfo?> Have<TAttribute>(
		this IAsyncEnumerableItems<ConstructorInfo?> items,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> items.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <inheritdoc cref="Have(IEnumerableItems{ConstructorInfo}, Type)" />
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<ConstructorInfo?>, ConstructorInfo?> Have(
		this IAsyncEnumerableItems<ConstructorInfo?> items,
		Type attributeType)
		=> items.HaveAttribute(AttributeOptions().WithAttribute(attributeType));
#endif

	private static AttributeFilterOptions<ConstructorInfo?> AttributeOptions()
		=> new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));

	private static HaveAttributeWithoutInheritResult<IEnumerable<ConstructorInfo?>, ConstructorInfo?> HaveAttribute(
		this IThat<IEnumerable<ConstructorInfo?>?> subject,
		AttributeFilterOptions<ConstructorInfo?> options)
	{
		HaveAttributeExpectation<ConstructorInfo> expectation = new(options);
		return new(subject.ExpectAll(expectation), subject, expectation.Filter);
	}

	private static HaveAttributeWithoutInheritResult<IEnumerable<ConstructorInfo?>, ConstructorInfo?> HaveAttribute(
		this IEnumerableItems<ConstructorInfo?> items,
		AttributeFilterOptions<ConstructorInfo?> options)
	{
		HaveAttributeExpectation<ConstructorInfo> expectation = new(options);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}

#if NET8_0_OR_GREATER
	private static HaveAttributeWithoutInheritResult<IAsyncEnumerable<ConstructorInfo?>, ConstructorInfo?> HaveAttribute(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject,
		AttributeFilterOptions<ConstructorInfo?> options)
	{
		HaveAttributeExpectation<ConstructorInfo> expectation = new(options);
		return new(subject.ExpectAll(expectation), subject, expectation.Filter);
	}

	private static HaveAttributeWithoutInheritResult<IAsyncEnumerable<ConstructorInfo?>, ConstructorInfo?> HaveAttribute(
		this IAsyncEnumerableItems<ConstructorInfo?> items,
		AttributeFilterOptions<ConstructorInfo?> options)
	{
		HaveAttributeExpectation<ConstructorInfo> expectation = new(options);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}
#endif
}
