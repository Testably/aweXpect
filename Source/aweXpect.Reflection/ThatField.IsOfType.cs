using System;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatField
{
	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> is of type <typeparamref name="TField" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<FieldInfo> IsOfType<TField>(
		this IThat<FieldInfo?> subject)
		=> IsOfType(subject, typeof(TField));

	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> is of type <paramref name="fieldType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<FieldInfo> IsOfType(
		this IThat<FieldInfo?> subject, Type fieldType)
	{
		OptionsHolder<TypeFilterOptions> typeFilterOptions = new(TypeFilterOptions.Default.WithType(fieldType, false));
		return new OfTypeResult<FieldInfo>(
			subject.Get().ExpectationBuilder.AddConstraint(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> is not of type <typeparamref name="TField" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<FieldInfo, IThat<FieldInfo?>> IsNotOfType<TField>(
		this IThat<FieldInfo?> subject)
		=> IsNotOfType(subject, typeof(TField));

	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> is not of type <paramref name="fieldType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<FieldInfo, IThat<FieldInfo?>> IsNotOfType(
		this IThat<FieldInfo?> subject, Type fieldType)
	{
		OptionsHolder<TypeFilterOptions> typeFilterOptions = new(TypeFilterOptions.Default.WithType(fieldType, false));
		return new AndOrResult<FieldInfo, IThat<FieldInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions).Invert()),
			subject);
	}

	private sealed class IsOfTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		IReadOnlyOptionsHolder<TypeFilterOptions> typeFilterOptions)
		: ConstraintResult.WithNotNullValue<FieldInfo?>(it, grammars),
			IValueConstraint<FieldInfo?>
	{
		public ConstraintResult IsMetBy(FieldInfo? actual)
		{
			Actual = actual;
			Outcome = typeFilterOptions.Value.Matches(actual?.FieldType)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.HasFlag(ExpectationGrammars.Negated) ? "is not " : "is ");
			typeFilterOptions.Value.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it was of type ").Append(Formatter.Format(Actual?.FieldType));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
