using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection;

public static partial class ThatFields
{
	/// <summary>
	///     Verifies that all fields in the filtered collection are of exactly type <typeparamref name="TField" />.
	/// </summary>
	public static FieldsOfTypeResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>>> AreOfExactType<TField>(
		this IThat<IEnumerable<FieldInfo?>> subject)
		=> AreOfExactType(subject, typeof(TField));

	/// <summary>
	///     Verifies that all fields in the filtered collection are of exactly type <paramref name="fieldType" />.
	/// </summary>
	public static FieldsOfTypeResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>>> AreOfExactType(
		this IThat<IEnumerable<FieldInfo?>> subject, Type fieldType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(fieldType, true);
		return new FieldsOfTypeResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>>>(
			subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<FieldInfo?>>((it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all fields in the filtered collection are of exactly type <typeparamref name="TField" />.
	/// </summary>
	public static FieldsOfTypeResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>>>
		AreOfExactType<TField>(
			this IThat<IAsyncEnumerable<FieldInfo?>> subject)
		=> AreOfExactType(subject, typeof(TField));

	/// <summary>
	///     Verifies that all fields in the filtered collection are of exactly type <paramref name="fieldType" />.
	/// </summary>
	public static FieldsOfTypeResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>>>
		AreOfExactType(
			this IThat<IAsyncEnumerable<FieldInfo?>> subject, Type fieldType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(fieldType, true);
		return new FieldsOfTypeResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>>>(
			subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<FieldInfo?>>((it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}
#endif

	public sealed partial class FieldsOfTypeResult<TValue, TResult>
	{
		/// <summary>
		///     Allow an alternative exact type <typeparamref name="TField" />.
		/// </summary>
		public FieldsOfTypeResult<TValue, TResult> OrOfExactType<TField>()
			=> OrOfExactType(typeof(TField));

		/// <summary>
		///     Allow an alternative exact type <paramref name="fieldType" />.
		/// </summary>
		public FieldsOfTypeResult<TValue, TResult> OrOfExactType(Type fieldType)
		{
			typeFilterOptions.RegisterType(fieldType, true);
			return this;
		}
	}
}
