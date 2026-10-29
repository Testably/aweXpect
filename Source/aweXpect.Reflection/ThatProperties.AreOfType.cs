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
	///     Verifies that all properties in the filtered collection are of type <typeparamref name="TProperty" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IEnumerable<PropertyInfo?>> AreOfType<TProperty>(
		this IThat<IEnumerable<PropertyInfo?>?> subject)
		=> AreOfType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that all properties in the filtered collection are of type <paramref name="propertyType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IEnumerable<PropertyInfo?>> AreOfType(
		this IThat<IEnumerable<PropertyInfo?>?> subject, Type propertyType)
	{
		OfTypeExpectation<PropertyInfo> expectation = new(static property => property.PropertyType, propertyType, false);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <summary>
	///     …are of type <typeparamref name="TProperty" /> (or a subtype).
	/// </summary>
	public static OfTypeResult<IEnumerable<PropertyInfo?>> AreOfType<TProperty>(
		this IEnumerableItems<PropertyInfo?> items)
		=> AreOfType(items, typeof(TProperty));

	/// <summary>
	///     …are of type <paramref name="propertyType" /> (or a subtype).
	/// </summary>
	public static OfTypeResult<IEnumerable<PropertyInfo?>> AreOfType(
		this IEnumerableItems<PropertyInfo?> items, Type propertyType)
	{
		OfTypeExpectation<PropertyInfo> expectation = new(static property => property.PropertyType, propertyType, false);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

	/// <inheritdoc cref="AreOfType(IEnumerableItems{PropertyInfo}, Type)" />
	public static OfTypeResult<TCollection> AreOfType<TCollection>(
		this IStructEnumerableItems<TCollection, PropertyInfo?> items, Type propertyType)
		where TCollection : struct, IEnumerable<PropertyInfo?>
	{
		OfTypeExpectation<PropertyInfo> expectation = new(static property => property.PropertyType, propertyType, false);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="AreOfType{TProperty}(IThat{IEnumerable{PropertyInfo}})" />
	[GuaranteesNotNull]
	public static OfTypeResult<IAsyncEnumerable<PropertyInfo?>> AreOfType<TProperty>(
		this IThat<IAsyncEnumerable<PropertyInfo?>?> subject)
		=> AreOfType(subject, typeof(TProperty));

	/// <inheritdoc cref="AreOfType(IThat{IEnumerable{PropertyInfo}}, Type)" />
	[GuaranteesNotNull]
	public static OfTypeResult<IAsyncEnumerable<PropertyInfo?>> AreOfType(
		this IThat<IAsyncEnumerable<PropertyInfo?>?> subject, Type propertyType)
	{
		OfTypeExpectation<PropertyInfo> expectation = new(static property => property.PropertyType, propertyType, false);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <inheritdoc cref="AreOfType{TProperty}(IEnumerableItems{PropertyInfo})" />
	public static OfTypeResult<IAsyncEnumerable<PropertyInfo?>> AreOfType<TProperty>(
		this IAsyncEnumerableItems<PropertyInfo?> items)
		=> AreOfType(items, typeof(TProperty));

	/// <inheritdoc cref="AreOfType(IEnumerableItems{PropertyInfo}, Type)" />
	public static OfTypeResult<IAsyncEnumerable<PropertyInfo?>> AreOfType(
		this IAsyncEnumerableItems<PropertyInfo?> items, Type propertyType)
	{
		OfTypeExpectation<PropertyInfo> expectation = new(static property => property.PropertyType, propertyType, false);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}
#endif

	/// <summary>
	///     Verifies that none of the properties in the filtered collection are of type <typeparamref name="TProperty" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>?>> AreNotOfType<TProperty>(
		this IThat<IEnumerable<PropertyInfo?>?> subject)
		=> AreNotOfType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that none of the properties in the filtered collection are of type <paramref name="propertyType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>?>> AreNotOfType(
		this IThat<IEnumerable<PropertyInfo?>?> subject, Type propertyType)
		=> new(subject.ExpectNone(new OfTypeExpectation<PropertyInfo>(static property => property.PropertyType, propertyType, false)), subject);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="AreNotOfType{TProperty}(IThat{IEnumerable{PropertyInfo}})" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>?>> AreNotOfType<TProperty>(
		this IThat<IAsyncEnumerable<PropertyInfo?>?> subject)
		=> AreNotOfType(subject, typeof(TProperty));

	/// <inheritdoc cref="AreNotOfType(IThat{IEnumerable{PropertyInfo}}, Type)" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>?>> AreNotOfType(
		this IThat<IAsyncEnumerable<PropertyInfo?>?> subject, Type propertyType)
		=> new(subject.ExpectNone(new OfTypeExpectation<PropertyInfo>(static property => property.PropertyType, propertyType, false)), subject);
#endif
}
