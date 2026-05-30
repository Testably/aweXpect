using System;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatProperty
{
	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> is of type <typeparamref name="TProperty" /> (or a subtype).
	/// </summary>
	public static PropertyOfTypeResult<PropertyInfo?, IThat<PropertyInfo?>> IsOfType<TProperty>(
		this IThat<PropertyInfo?> subject)
		=> IsOfType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> is of type <paramref name="propertyType" /> (or a subtype).
	/// </summary>
	public static PropertyOfTypeResult<PropertyInfo?, IThat<PropertyInfo?>> IsOfType(
		this IThat<PropertyInfo?> subject, Type propertyType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(propertyType, false);
		return new PropertyOfTypeResult<PropertyInfo?, IThat<PropertyInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

	/// <summary>
	///     Result that allows chaining additional types for a single property.
	/// </summary>
	public sealed partial class PropertyOfTypeResult<TValue, TResult>(
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
		///     Allow an alternative type <typeparamref name="TProperty" /> (or a subtype).
		/// </summary>
		public PropertyOfTypeResult<TValue, TResult> OrOfType<TProperty>()
			=> OrOfType(typeof(TProperty));

		/// <summary>
		///     Allow an alternative type <paramref name="propertyType" /> (or a subtype).
		/// </summary>
		public PropertyOfTypeResult<TValue, TResult> OrOfType(Type propertyType)
		{
			typeFilterOptions.RegisterType(propertyType, false);
			return this;
		}
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
			=> stringBuilder.Append("it did");
	}
}
