using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HaveAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static HaveAttributeResult<IEnumerable<Type?>, Type?> Have<TAttribute>(
		this IThat<IEnumerable<Type?>?> subject)
		where TAttribute : Attribute
		=> subject.Have(typeof(TAttribute));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have
	///     attribute of type <typeparamref name="TAttribute" /> that matches the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HaveAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static HaveAttributeResult<IEnumerable<Type?>, Type?> Have<TAttribute>(
		this IThat<IEnumerable<Type?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have
	///     attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HaveAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute.</exception>
	[GuaranteesNotNull]
	public static HaveAttributeResult<IEnumerable<Type?>, Type?> Have(
		this IThat<IEnumerable<Type?>?> subject,
		Type attributeType)
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <summary>
	///     …have attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{Type}})" path="/remarks" />
	public static HaveAttributeResult<IEnumerable<Type?>, Type?> Have<TAttribute>(
		this IEnumerableItems<Type?> items)
		where TAttribute : Attribute
		=> items.Have(typeof(TAttribute));

	/// <summary>
	///     …have attribute of type <typeparamref name="TAttribute" /> that matches the <paramref name="predicate" />.
	/// </summary>
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{Type}})" path="/remarks" />
	public static HaveAttributeResult<IEnumerable<Type?>, Type?> Have<TAttribute>(
		this IEnumerableItems<Type?> items,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> items.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <summary>
	///     …have attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{Type}})" path="/remarks" />
	/// <inheritdoc cref="Have(IThat{IEnumerable{Type}}, Type)" path="/exception" />
	public static HaveAttributeResult<IEnumerable<Type?>, Type?> Have(
		this IEnumerableItems<Type?> items,
		Type attributeType)
		=> items.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <inheritdoc cref="Have(IEnumerableItems{Type}, Type)" />
	public static HaveAttributeResult<TCollection, Type?> Have<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items,
		Type attributeType)
		where TCollection : struct, IEnumerable<Type?>
	{
		HaveAttributeExpectation<Type> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{Type}})" />
	[GuaranteesNotNull]
	public static HaveAttributeResult<IAsyncEnumerable<Type?>, Type?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		where TAttribute : Attribute
		=> subject.Have(typeof(TAttribute));

	/// <inheritdoc cref="Have{TAttribute}(IThat{IEnumerable{Type}}, Func{TAttribute, bool}, string)" />
	[GuaranteesNotNull]
	public static HaveAttributeResult<IAsyncEnumerable<Type?>, Type?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <inheritdoc cref="Have(IThat{IEnumerable{Type}}, Type)" />
	[GuaranteesNotNull]
	public static HaveAttributeResult<IAsyncEnumerable<Type?>, Type?> Have(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type attributeType)
		=> subject.HaveAttribute(AttributeOptions().WithAttribute(attributeType));

	/// <inheritdoc cref="Have{TAttribute}(IEnumerableItems{Type})" />
	public static HaveAttributeResult<IAsyncEnumerable<Type?>, Type?> Have<TAttribute>(
		this IAsyncEnumerableItems<Type?> items)
		where TAttribute : Attribute
		=> items.Have(typeof(TAttribute));

	/// <inheritdoc cref="Have{TAttribute}(IEnumerableItems{Type}, Func{TAttribute, bool}, string)" />
	public static HaveAttributeResult<IAsyncEnumerable<Type?>, Type?> Have<TAttribute>(
		this IAsyncEnumerableItems<Type?> items,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> items.HaveAttribute(AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));

	/// <inheritdoc cref="Have(IEnumerableItems{Type}, Type)" />
	public static HaveAttributeResult<IAsyncEnumerable<Type?>, Type?> Have(
		this IAsyncEnumerableItems<Type?> items,
		Type attributeType)
		=> items.HaveAttribute(AttributeOptions().WithAttribute(attributeType));
#endif

	private static AttributeFilterOptions<Type?> AttributeOptions()
		=> new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));

	private static HaveAttributeResult<IEnumerable<Type?>, Type?> HaveAttribute(
		this IThat<IEnumerable<Type?>?> subject,
		AttributeFilterOptions<Type?> options)
	{
		HaveAttributeExpectation<Type> expectation = new(options);
		return new(subject.ExpectAll(expectation), subject, expectation.Filter);
	}

	private static HaveAttributeResult<IEnumerable<Type?>, Type?> HaveAttribute(
		this IEnumerableItems<Type?> items,
		AttributeFilterOptions<Type?> options)
	{
		HaveAttributeExpectation<Type> expectation = new(options);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}

#if NET8_0_OR_GREATER
	private static HaveAttributeResult<IAsyncEnumerable<Type?>, Type?> HaveAttribute(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		AttributeFilterOptions<Type?> options)
	{
		HaveAttributeExpectation<Type> expectation = new(options);
		return new(subject.ExpectAll(expectation), subject, expectation.Filter);
	}

	private static HaveAttributeResult<IAsyncEnumerable<Type?>, Type?> HaveAttribute(
		this IAsyncEnumerableItems<Type?> items,
		AttributeFilterOptions<Type?> options)
	{
		HaveAttributeExpectation<Type> expectation = new(options);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Filter);
	}
#endif
}
