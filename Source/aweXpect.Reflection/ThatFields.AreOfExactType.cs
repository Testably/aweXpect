using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatFields
{
	/// <summary>
	///     Verifies that all fields in the filtered collection are of exactly type <typeparamref name="TField" />.
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IEnumerable<FieldInfo?>> AreOfExactType<TField>(
		this IThat<IEnumerable<FieldInfo?>?> subject)
		=> AreOfExactType(subject, typeof(TField));

	/// <summary>
	///     Verifies that all fields in the filtered collection are of exactly type <paramref name="fieldType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IEnumerable<FieldInfo?>> AreOfExactType(
		this IThat<IEnumerable<FieldInfo?>?> subject, Type fieldType)
	{
		OfTypeExpectation<FieldInfo> expectation = new(static field => field.FieldType, fieldType, true);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <summary>
	///     …are of exactly type <typeparamref name="TField" />.
	/// </summary>
	public static OfTypeResult<IEnumerable<FieldInfo?>> AreOfExactType<TField>(
		this IEnumerableItems<FieldInfo?> items)
		=> AreOfExactType(items, typeof(TField));

	/// <summary>
	///     …are of exactly type <paramref name="fieldType" />.
	/// </summary>
	public static OfTypeResult<IEnumerable<FieldInfo?>> AreOfExactType(
		this IEnumerableItems<FieldInfo?> items, Type fieldType)
	{
		OfTypeExpectation<FieldInfo> expectation = new(static field => field.FieldType, fieldType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

	/// <inheritdoc cref="AreOfExactType(IEnumerableItems{FieldInfo}, Type)" />
	public static OfTypeResult<TCollection> AreOfExactType<TCollection>(
		this IStructEnumerableItems<TCollection, FieldInfo?> items, Type fieldType)
		where TCollection : struct, IEnumerable<FieldInfo?>
	{
		OfTypeExpectation<FieldInfo> expectation = new(static field => field.FieldType, fieldType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="AreOfExactType{TField}(IThat{IEnumerable{FieldInfo}})" />
	[GuaranteesNotNull]
	public static OfTypeResult<IAsyncEnumerable<FieldInfo?>> AreOfExactType<TField>(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject)
		=> AreOfExactType(subject, typeof(TField));

	/// <inheritdoc cref="AreOfExactType(IThat{IEnumerable{FieldInfo}}, Type)" />
	[GuaranteesNotNull]
	public static OfTypeResult<IAsyncEnumerable<FieldInfo?>> AreOfExactType(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject, Type fieldType)
	{
		OfTypeExpectation<FieldInfo> expectation = new(static field => field.FieldType, fieldType, true);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <inheritdoc cref="AreOfExactType{TField}(IEnumerableItems{FieldInfo})" />
	public static OfTypeResult<IAsyncEnumerable<FieldInfo?>> AreOfExactType<TField>(
		this IAsyncEnumerableItems<FieldInfo?> items)
		=> AreOfExactType(items, typeof(TField));

	/// <inheritdoc cref="AreOfExactType(IEnumerableItems{FieldInfo}, Type)" />
	public static OfTypeResult<IAsyncEnumerable<FieldInfo?>> AreOfExactType(
		this IAsyncEnumerableItems<FieldInfo?> items, Type fieldType)
	{
		OfTypeExpectation<FieldInfo> expectation = new(static field => field.FieldType, fieldType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}
#endif

	/// <summary>
	///     Verifies that none of the fields in the filtered collection are of exactly type <typeparamref name="TField" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>?>> AreNotOfExactType<TField>(
		this IThat<IEnumerable<FieldInfo?>?> subject)
		=> AreNotOfExactType(subject, typeof(TField));

	/// <summary>
	///     Verifies that none of the fields in the filtered collection are of exactly type <paramref name="fieldType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>?>> AreNotOfExactType(
		this IThat<IEnumerable<FieldInfo?>?> subject, Type fieldType)
		=> new(subject.ExpectNone(new OfTypeExpectation<FieldInfo>(static field => field.FieldType, fieldType, true)), subject);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="AreNotOfExactType{TField}(IThat{IEnumerable{FieldInfo}})" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>?>> AreNotOfExactType<TField>(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject)
		=> AreNotOfExactType(subject, typeof(TField));

	/// <inheritdoc cref="AreNotOfExactType(IThat{IEnumerable{FieldInfo}}, Type)" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>?>> AreNotOfExactType(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject, Type fieldType)
		=> new(subject.ExpectNone(new OfTypeExpectation<FieldInfo>(static field => field.FieldType, fieldType, true)), subject);
#endif
}
