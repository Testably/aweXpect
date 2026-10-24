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

public static partial class ThatProperty
{
	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> is of type <typeparamref name="TProperty" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<PropertyInfo> IsOfType<TProperty>(
		this IThat<PropertyInfo?> subject)
		=> IsOfType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> is of type <paramref name="propertyType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<PropertyInfo> IsOfType(
		this IThat<PropertyInfo?> subject, Type propertyType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(propertyType, false);
		return new OfTypeResult<PropertyInfo>(
			subject.Get().ExpectationBuilder.AddConstraint(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> is not of type <typeparamref name="TProperty" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<PropertyInfo, IThat<PropertyInfo?>> IsNotOfType<TProperty>(
		this IThat<PropertyInfo?> subject)
		=> IsNotOfType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> is not of type <paramref name="propertyType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<PropertyInfo, IThat<PropertyInfo?>> IsNotOfType(
		this IThat<PropertyInfo?> subject, Type propertyType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(propertyType, false);
		return new AndOrResult<PropertyInfo, IThat<PropertyInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions).Invert()),
			subject);
	}

	private sealed class IsOfTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeFilterOptions typeFilterOptions)
		: ConstraintResult.WithNotNullValue<PropertyInfo?>(it, grammars),
			IValueConstraint<PropertyInfo?>
	{
		public ConstraintResult IsMetBy(PropertyInfo? actual)
		{
			Actual = actual;
			Outcome = typeFilterOptions.Matches(actual?.PropertyType)
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
			=> stringBuilder.Append("it was of type ").Append(Formatter.Format(Actual?.PropertyType));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
