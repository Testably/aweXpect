using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatAssemblies
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IEnumerable<Assembly?>, Assembly?> Have<TAttribute>(
		this IThat<IEnumerable<Assembly?>?> subject)
		where TAttribute : Attribute
		=> subject.Have(typeof(TAttribute));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     attribute of type <typeparamref name="TAttribute" /> that matches the <paramref name="predicate" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IEnumerable<Assembly?>, Assembly?> Have<TAttribute>(
		this IThat<IEnumerable<Assembly?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute or an open generic type.</exception>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IEnumerable<Assembly?>, Assembly?> Have(
		this IThat<IEnumerable<Assembly?>?> subject,
		Type attributeType)
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <summary>
	///     …have attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public static HaveAttributeWithoutInheritResult<IEnumerable<Assembly?>, Assembly?> Have<TAttribute>(
		this IEnumerableItems<Assembly?> items)
		where TAttribute : Attribute
		=> items.Have(typeof(TAttribute));

	/// <summary>
	///     …have attribute of type <typeparamref name="TAttribute" /> that matches the <paramref name="predicate" />.
	/// </summary>
	public static HaveAttributeWithoutInheritResult<IEnumerable<Assembly?>, Assembly?> Have<TAttribute>(
		this IEnumerableItems<Assembly?> items,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> items.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <summary>
	///     …have attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <inheritdoc cref="Have(IThat{IEnumerable{Assembly}}, Type)" path="/exception" />
	public static HaveAttributeWithoutInheritResult<IEnumerable<Assembly?>, Assembly?> Have(
		this IEnumerableItems<Assembly?> items,
		Type attributeType)
		=> items.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <inheritdoc cref="Have(IEnumerableItems{Assembly}, Type)" />
	public static HaveAttributeWithoutInheritResult<TCollection, Assembly?> Have<TCollection>(
		this IStructEnumerableItems<TCollection, Assembly?> items,
		Type attributeType)
		where TCollection : struct, IEnumerable<Assembly?>
	{
		HaveAttributeExpectation<Assembly> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{Assembly}})" />
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<Assembly?>, Assembly?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<Assembly?>?> subject)
		where TAttribute : Attribute
		=> subject.Have(typeof(TAttribute));

	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{Assembly}}, Func{TAttribute, bool}, string)" />
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<Assembly?>, Assembly?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<Assembly?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <inheritdoc cref="Have(IThat{IEnumerable{Assembly}}, Type)" />
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<Assembly?>, Assembly?> Have(
		this IThat<IAsyncEnumerable<Assembly?>?> subject,
		Type attributeType)
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <inheritdoc cref="Have{TAttribute}(IEnumerableItems{Assembly})" />
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<Assembly?>, Assembly?> Have<TAttribute>(
		this IAsyncEnumerableItems<Assembly?> items)
		where TAttribute : Attribute
		=> items.Have(typeof(TAttribute));

	/// <inheritdoc cref="Have{TAttribute}(IEnumerableItems{Assembly}, Func{TAttribute, bool}, string)" />
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<Assembly?>, Assembly?> Have<TAttribute>(
		this IAsyncEnumerableItems<Assembly?> items,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> items.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <inheritdoc cref="Have(IEnumerableItems{Assembly}, Type)" />
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<Assembly?>, Assembly?> Have(
		this IAsyncEnumerableItems<Assembly?> items,
		Type attributeType)
		=> items.HaveAttribute(AttributeOptions().WithAttribute(attributeType));
#endif

	private static AttributeFilterOptions<Assembly?> AttributeOptions()
		=> new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));

	private static HaveAttributeWithoutInheritResult<IEnumerable<Assembly?>, Assembly?> HaveAttribute(
		this IThat<IEnumerable<Assembly?>?> subject,
		AttributeFilterOptions<Assembly?> options)
	{
		HaveAttributeExpectation<Assembly> expectation = new(options);
		return new(subject.ExpectAll(expectation), subject, expectation.Filter);
	}

	private static HaveAttributeWithoutInheritResult<IEnumerable<Assembly?>, Assembly?> HaveAttribute(
		this IEnumerableItems<Assembly?> items,
		AttributeFilterOptions<Assembly?> options)
	{
		HaveAttributeExpectation<Assembly> expectation = new(options);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}

#if NET8_0_OR_GREATER
	private static HaveAttributeWithoutInheritResult<IAsyncEnumerable<Assembly?>, Assembly?> HaveAttribute(
		this IThat<IAsyncEnumerable<Assembly?>?> subject,
		AttributeFilterOptions<Assembly?> options)
	{
		HaveAttributeExpectation<Assembly> expectation = new(options);
		return new(subject.ExpectAll(expectation), subject, expectation.Filter);
	}

	private static HaveAttributeWithoutInheritResult<IAsyncEnumerable<Assembly?>, Assembly?> HaveAttribute(
		this IAsyncEnumerableItems<Assembly?> items,
		AttributeFilterOptions<Assembly?> options)
	{
		HaveAttributeExpectation<Assembly> expectation = new(options);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}
#endif
}
