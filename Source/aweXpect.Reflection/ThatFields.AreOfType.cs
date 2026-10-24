using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatFields
{
	/// <summary>
	///     Verifies that all fields in the filtered collection are of type <typeparamref name="TField" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static FieldsOfTypeResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>?>> AreOfType<TField>(
		this IThat<IEnumerable<FieldInfo?>?> subject)
		=> AreOfType(subject, typeof(TField));

	/// <summary>
	///     Verifies that all fields in the filtered collection are of type <paramref name="fieldType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static FieldsOfTypeResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>?>> AreOfType(
		this IThat<IEnumerable<FieldInfo?>?> subject, Type fieldType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(fieldType, false);
		return new FieldsOfTypeResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IEnumerable<FieldInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all fields in the filtered collection are of type <typeparamref name="TField" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static FieldsOfTypeResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>?>>
		AreOfType<TField>(
			this IThat<IAsyncEnumerable<FieldInfo?>?> subject)
		=> AreOfType(subject, typeof(TField));

	/// <summary>
	///     Verifies that all fields in the filtered collection are of type <paramref name="fieldType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static FieldsOfTypeResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>?>> AreOfType(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject, Type fieldType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(fieldType, false);
		return new FieldsOfTypeResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IAsyncEnumerable<FieldInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}
#endif

	/// <summary>
	///     Verifies that none of the fields in the filtered collection are of type <typeparamref name="TField" /> (or a
	///     subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>?>> AreNotOfType<TField>(
		this IThat<IEnumerable<FieldInfo?>?> subject)
		=> AreNotOfType(subject, typeof(TField));

	/// <summary>
	///     Verifies that none of the fields in the filtered collection are of type <paramref name="fieldType" /> (or a
	///     subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>?>> AreNotOfType(
		this IThat<IEnumerable<FieldInfo?>?> subject, Type fieldType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(fieldType, false);
		return new AndOrResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IEnumerable<FieldInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreNotOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the fields in the filtered collection are of type <typeparamref name="TField" /> (or a
	///     subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>?>> AreNotOfType<TField>(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject)
		=> AreNotOfType(subject, typeof(TField));

	/// <summary>
	///     Verifies that none of the fields in the filtered collection are of type <paramref name="fieldType" /> (or a
	///     subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>?>> AreNotOfType(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject, Type fieldType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(fieldType, false);
		return new AndOrResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IAsyncEnumerable<FieldInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreNotOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject);
	}
#endif

	/// <summary>
	///     Result that allows chaining additional types for field collections.
	/// </summary>
	public sealed partial class FieldsOfTypeResult<TValue, TResult>
		: AndOrResult<TValue, TResult>,
			IOptionsProvider<TypeFilterOptions>
		where TResult : IThat<TValue?>
	{
		private readonly TypeFilterOptions _typeFilterOptions;

		internal FieldsOfTypeResult(
			ExpectationBuilder expectationBuilder,
			TResult subject,
			TypeFilterOptions typeFilterOptions)
			: base(expectationBuilder, subject)
		{
			_typeFilterOptions = typeFilterOptions;
		}

		/// <inheritdoc cref="IOptionsProvider{TypeFilterOptions}.Options" />
		TypeFilterOptions IOptionsProvider<TypeFilterOptions>.Options => _typeFilterOptions;

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
			_typeFilterOptions.RegisterType(fieldType, false);
			return this;
		}
	}

	private sealed class AreOfTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeFilterOptions typeFilterOptions)
		: CollectionConstraintResult<FieldInfo?>(it, grammars),
			IContextConstraint<IEnumerable<FieldInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<FieldInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<FieldInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				field => typeFilterOptions.Matches(field?.FieldType), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<FieldInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, field => typeFilterOptions.Matches(field?.FieldType));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("are all ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching fields");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("are not all ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching fields");
	}

	private sealed class AreNotOfTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeFilterOptions typeFilterOptions)
		: CollectionConstraintResult<FieldInfo?>(it, grammars),
			IContextConstraint<IEnumerable<FieldInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<FieldInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<FieldInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				field => field is not null && !typeFilterOptions.Matches(field.FieldType), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<FieldInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, field => field is not null && !typeFilterOptions.Matches(field.FieldType));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("are all not ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained fields ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("also contain a field ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" only contained fields not ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}
	}
}
