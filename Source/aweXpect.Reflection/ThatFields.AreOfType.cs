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
	///     Verifies that all fields in the filtered collection are of type <typeparamref name="TField" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IEnumerable<FieldInfo?>> AreOfType<TField>(
		this IThat<IEnumerable<FieldInfo?>?> subject)
		=> AreOfType(subject, typeof(TField));

	/// <summary>
	///     Verifies that all fields in the filtered collection are of type <paramref name="fieldType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IEnumerable<FieldInfo?>> AreOfType(
		this IThat<IEnumerable<FieldInfo?>?> subject, Type fieldType)
	{
		OfTypeExpectation<FieldInfo> expectation = new(static field => field.FieldType, fieldType, false);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <summary>
	///     …are of type <typeparamref name="TField" /> (or a subtype).
	/// </summary>
	public static OfTypeResult<IEnumerable<FieldInfo?>> AreOfType<TField>(
		this IEnumerableItems<FieldInfo?> items)
		=> AreOfType(items, typeof(TField));

	/// <summary>
	///     …are of type <paramref name="fieldType" /> (or a subtype).
	/// </summary>
	public static OfTypeResult<IEnumerable<FieldInfo?>> AreOfType(
		this IEnumerableItems<FieldInfo?> items, Type fieldType)
	{
		OfTypeExpectation<FieldInfo> expectation = new(static field => field.FieldType, fieldType, false);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

	/// <inheritdoc cref="AreOfType(IEnumerableItems{FieldInfo}, Type)" />
	public static OfTypeResult<TCollection> AreOfType<TCollection>(
		this IStructEnumerableItems<TCollection, FieldInfo?> items, Type fieldType)
		where TCollection : struct, IEnumerable<FieldInfo?>
	{
		OfTypeExpectation<FieldInfo> expectation = new(static field => field.FieldType, fieldType, false);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="AreOfType{TField}(IThat{IEnumerable{FieldInfo}})" />
	[GuaranteesNotNull]
	public static OfTypeResult<IAsyncEnumerable<FieldInfo?>> AreOfType<TField>(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject)
		=> AreOfType(subject, typeof(TField));

	/// <inheritdoc cref="AreOfType(IThat{IEnumerable{FieldInfo}}, Type)" />
	[GuaranteesNotNull]
	public static OfTypeResult<IAsyncEnumerable<FieldInfo?>> AreOfType(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject, Type fieldType)
	{
		OfTypeExpectation<FieldInfo> expectation = new(static field => field.FieldType, fieldType, false);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <inheritdoc cref="AreOfType{TField}(IEnumerableItems{FieldInfo})" />
	public static OfTypeResult<IAsyncEnumerable<FieldInfo?>> AreOfType<TField>(
		this IAsyncEnumerableItems<FieldInfo?> items)
		=> AreOfType(items, typeof(TField));

	/// <inheritdoc cref="AreOfType(IEnumerableItems{FieldInfo}, Type)" />
	public static OfTypeResult<IAsyncEnumerable<FieldInfo?>> AreOfType(
		this IAsyncEnumerableItems<FieldInfo?> items, Type fieldType)
	{
		OfTypeExpectation<FieldInfo> expectation = new(static field => field.FieldType, fieldType, false);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}
#endif

	/// <summary>
	///     Verifies that none of the fields in the filtered collection are of type <typeparamref name="TField" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>?>> AreNotOfType<TField>(
		this IThat<IEnumerable<FieldInfo?>?> subject)
		=> AreNotOfType(subject, typeof(TField));

	/// <summary>
	///     Verifies that none of the fields in the filtered collection are of type <paramref name="fieldType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>?>> AreNotOfType(
		this IThat<IEnumerable<FieldInfo?>?> subject, Type fieldType)
		=> new(subject.ExpectNone(new OfTypeExpectation<FieldInfo>(static field => field.FieldType, fieldType, false)), subject);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="AreNotOfType{TField}(IThat{IEnumerable{FieldInfo}})" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>?>> AreNotOfType<TField>(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject)
		=> AreNotOfType(subject, typeof(TField));

	/// <inheritdoc cref="AreNotOfType(IThat{IEnumerable{FieldInfo}}, Type)" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>?>> AreNotOfType(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject, Type fieldType)
		=> new(subject.ExpectNone(new OfTypeExpectation<FieldInfo>(static field => field.FieldType, fieldType, false)), subject);
#endif
}
