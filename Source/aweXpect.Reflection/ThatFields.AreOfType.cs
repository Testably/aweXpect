using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatFields
{
	/// <summary>
	///     Verifies that all fields in the filtered collection are of type <typeparamref name="TField" /> (or a subtype).
	/// </summary>
	public static FieldsOfTypeResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>>> AreOfType<TField>(
		this IThat<IEnumerable<FieldInfo?>> subject)
		=> AreOfType(subject, typeof(TField));

	/// <summary>
	///     Verifies that all fields in the filtered collection are of type <paramref name="fieldType" /> (or a subtype).
	/// </summary>
	public static FieldsOfTypeResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>>> AreOfType(
		this IThat<IEnumerable<FieldInfo?>> subject, Type fieldType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(fieldType, false);
		return new FieldsOfTypeResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>>>(
			subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<FieldInfo?>>((it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all fields in the filtered collection are of type <typeparamref name="TField" /> (or a subtype).
	/// </summary>
	public static FieldsOfTypeResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>>>
		AreOfType<TField>(
			this IThat<IAsyncEnumerable<FieldInfo?>> subject)
		=> AreOfType(subject, typeof(TField));

	/// <summary>
	///     Verifies that all fields in the filtered collection are of type <paramref name="fieldType" /> (or a subtype).
	/// </summary>
	public static FieldsOfTypeResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>>> AreOfType(
		this IThat<IAsyncEnumerable<FieldInfo?>> subject, Type fieldType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(fieldType, false);
		return new FieldsOfTypeResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>>>(
			subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<FieldInfo?>>((it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}
#endif

	/// <summary>
	///     Result that allows chaining additional types for field collections.
	/// </summary>
	public sealed partial class FieldsOfTypeResult<TValue, TResult>(
		ExpectationBuilder expectationBuilder,
		TResult subject,
		TypeFilterOptions typeFilterOptions)
		: AndOrResult<TValue, TResult>(expectationBuilder, subject),
			IOptionsProvider<TypeFilterOptions>
		where TResult : IThat<TValue>
	{
		/// <inheritdoc cref="IOptionsProvider{TypeFilterOptions}.Options" />
		TypeFilterOptions IOptionsProvider<TypeFilterOptions>.Options => typeFilterOptions;

		/// <summary>
		///     Allow an alternative type <typeparamref name="TField" /> (or a subtype).
		/// </summary>
		public FieldsOfTypeResult<TValue, TResult> OrOfType<TField>()
			=> OrOfType(typeof(TField));

		/// <summary>
		///     Allow an alternative type <paramref name="fieldType" /> (or a subtype).
		/// </summary>
		public FieldsOfTypeResult<TValue, TResult> OrOfType(Type fieldType)
		{
			typeFilterOptions.RegisterType(fieldType, false);
			return this;
		}
	}

	private sealed class AreOfTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeFilterOptions typeFilterOptions)
		: CollectionConstraintResult<FieldInfo?>(grammars),
			IValueConstraint<IEnumerable<FieldInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<FieldInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<FieldInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, field => typeFilterOptions.Matches(field?.FieldType));
#endif

		public ConstraintResult IsMetBy(IEnumerable<FieldInfo?> actual)
			=> SetValue(actual, field => typeFilterOptions.Matches(field?.FieldType));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all are ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching fields ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all are ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching fields ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
