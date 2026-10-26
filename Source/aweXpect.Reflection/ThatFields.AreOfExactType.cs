using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Options;
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
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.AddType(fieldType, true);
		return new OfTypeResult<IEnumerable<FieldInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IEnumerable<FieldInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all fields in the filtered collection are of exactly type <typeparamref name="TField" />.
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IAsyncEnumerable<FieldInfo?>> AreOfExactType<TField>(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject)
		=> AreOfExactType(subject, typeof(TField));

	/// <summary>
	///     Verifies that all fields in the filtered collection are of exactly type <paramref name="fieldType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<IAsyncEnumerable<FieldInfo?>> AreOfExactType(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject, Type fieldType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.AddType(fieldType, true);
		return new OfTypeResult<IAsyncEnumerable<FieldInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IAsyncEnumerable<FieldInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
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
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.AddType(fieldType, true);
		return new AndOrResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IEnumerable<FieldInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreNotOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the fields in the filtered collection are of exactly type <typeparamref name="TField" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>?>>
		AreNotOfExactType<TField>(
			this IThat<IAsyncEnumerable<FieldInfo?>?> subject)
		=> AreNotOfExactType(subject, typeof(TField));

	/// <summary>
	///     Verifies that none of the fields in the filtered collection are of exactly type <paramref name="fieldType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>?>> AreNotOfExactType(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject, Type fieldType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.AddType(fieldType, true);
		return new AndOrResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IAsyncEnumerable<FieldInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreNotOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject);
	}
#endif
}
