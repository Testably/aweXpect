using System;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatField
{
	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> is of type <typeparamref name="TField" /> (or a subtype).
	/// </summary>
	public static FieldOfTypeResult<FieldInfo?, IThat<FieldInfo?>> IsOfType<TField>(
		this IThat<FieldInfo?> subject)
		=> IsOfType(subject, typeof(TField));

	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> is of type <paramref name="fieldType" /> (or a subtype).
	/// </summary>
	public static FieldOfTypeResult<FieldInfo?, IThat<FieldInfo?>> IsOfType(
		this IThat<FieldInfo?> subject, Type fieldType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(fieldType, false);
		return new FieldOfTypeResult<FieldInfo?, IThat<FieldInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

	/// <summary>
	///     Result that allows chaining additional types for a single field.
	/// </summary>
	public sealed partial class FieldOfTypeResult<TValue, TResult>(
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
		public FieldOfTypeResult<TValue, TResult> OrOfType<TField>()
			=> OrOfType(typeof(TField));

		/// <summary>
		///     Allow an alternative type <paramref name="fieldType" /> (or a subtype).
		/// </summary>
		public FieldOfTypeResult<TValue, TResult> OrOfType(Type fieldType)
		{
			typeFilterOptions.RegisterType(fieldType, false);
			return this;
		}
	}

	private sealed class IsOfTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeFilterOptions typeFilterOptions)
		: ConstraintResult.WithNotNullValue<FieldInfo?>(it, grammars),
			IValueConstraint<FieldInfo?>
	{
		public ConstraintResult IsMetBy(FieldInfo? actual)
		{
			Actual = actual;
			Outcome = typeFilterOptions.Matches(actual?.FieldType)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.HasFlag(ExpectationGrammars.Negated) ? "is not " : "is ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it was of type ").Append(Formatter.Format(Actual?.FieldType));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it did");
	}
}
