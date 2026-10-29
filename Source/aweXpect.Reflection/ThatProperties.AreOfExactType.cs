using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatProperties
{
	/// <summary>
	///     Verifies that all properties in the filtered collection are of exactly type <typeparamref name="TProperty" />.
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IEnumerable<PropertyInfo?>> AreOfExactType<TProperty>(
		this IThat<IEnumerable<PropertyInfo?>?> subject)
		=> AreOfExactType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that all properties in the filtered collection are of exactly type <paramref name="propertyType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IEnumerable<PropertyInfo?>> AreOfExactType(
		this IThat<IEnumerable<PropertyInfo?>?> subject, Type propertyType)
	{
		OfTypeExpectation<PropertyInfo> expectation = new(static property => property.PropertyType, propertyType, true);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <summary>
	///     …are of exactly type <typeparamref name="TProperty" />.
	/// </summary>
	public static OfTypeResult<IEnumerable<PropertyInfo?>> AreOfExactType<TProperty>(
		this IEnumerableItems<PropertyInfo?> items)
		=> AreOfExactType(items, typeof(TProperty));

	/// <summary>
	///     …are of exactly type <paramref name="propertyType" />.
	/// </summary>
	public static OfTypeResult<IEnumerable<PropertyInfo?>> AreOfExactType(
		this IEnumerableItems<PropertyInfo?> items, Type propertyType)
	{
		OfTypeExpectation<PropertyInfo> expectation = new(static property => property.PropertyType, propertyType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

	/// <inheritdoc cref="AreOfExactType(IEnumerableItems{PropertyInfo}, Type)" />
	public static OfTypeResult<TCollection> AreOfExactType<TCollection>(
		this IStructEnumerableItems<TCollection, PropertyInfo?> items, Type propertyType)
		where TCollection : struct, IEnumerable<PropertyInfo?>
	{
		OfTypeExpectation<PropertyInfo> expectation = new(static property => property.PropertyType, propertyType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="AreOfExactType{TProperty}(IThat{IEnumerable{PropertyInfo}})" />
	[GuaranteesNotNull]
	public static OfTypeResult<IAsyncEnumerable<PropertyInfo?>> AreOfExactType<TProperty>(
		this IThat<IAsyncEnumerable<PropertyInfo?>?> subject)
		=> AreOfExactType(subject, typeof(TProperty));

	/// <inheritdoc cref="AreOfExactType(IThat{IEnumerable{PropertyInfo}}, Type)" />
	[GuaranteesNotNull]
	public static OfTypeResult<IAsyncEnumerable<PropertyInfo?>> AreOfExactType(
		this IThat<IAsyncEnumerable<PropertyInfo?>?> subject, Type propertyType)
	{
		OfTypeExpectation<PropertyInfo> expectation = new(static property => property.PropertyType, propertyType, true);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <inheritdoc cref="AreOfExactType{TProperty}(IEnumerableItems{PropertyInfo})" />
	public static OfTypeResult<IAsyncEnumerable<PropertyInfo?>> AreOfExactType<TProperty>(
		this IAsyncEnumerableItems<PropertyInfo?> items)
		=> AreOfExactType(items, typeof(TProperty));

	/// <inheritdoc cref="AreOfExactType(IEnumerableItems{PropertyInfo}, Type)" />
	public static OfTypeResult<IAsyncEnumerable<PropertyInfo?>> AreOfExactType(
		this IAsyncEnumerableItems<PropertyInfo?> items, Type propertyType)
	{
		OfTypeExpectation<PropertyInfo> expectation = new(static property => property.PropertyType, propertyType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}
#endif

	/// <summary>
	///     Verifies that none of the properties in the filtered collection are of exactly type <typeparamref name="TProperty" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>?>> AreNotOfExactType<TProperty>(
		this IThat<IEnumerable<PropertyInfo?>?> subject)
		=> AreNotOfExactType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that none of the properties in the filtered collection are of exactly type <paramref name="propertyType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>?>> AreNotOfExactType(
		this IThat<IEnumerable<PropertyInfo?>?> subject, Type propertyType)
		=> new(subject.ExpectNone(new OfTypeExpectation<PropertyInfo>(static property => property.PropertyType, propertyType, true)), subject);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="AreNotOfExactType{TProperty}(IThat{IEnumerable{PropertyInfo}})" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>?>> AreNotOfExactType<TProperty>(
		this IThat<IAsyncEnumerable<PropertyInfo?>?> subject)
		=> AreNotOfExactType(subject, typeof(TProperty));

	/// <inheritdoc cref="AreNotOfExactType(IThat{IEnumerable{PropertyInfo}}, Type)" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>?>> AreNotOfExactType(
		this IThat<IAsyncEnumerable<PropertyInfo?>?> subject, Type propertyType)
		=> new(subject.ExpectNone(new OfTypeExpectation<PropertyInfo>(static property => property.PropertyType, propertyType, true)), subject);
#endif
}
