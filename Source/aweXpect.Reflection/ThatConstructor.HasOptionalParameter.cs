using System;
using System.Linq;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatConstructor
{
	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has an optional parameter.
	/// </summary>
	public static AndOrResult<ConstructorInfo?, IThat<ConstructorInfo?>> HasOptionalParameter(
		this IThat<ConstructorInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasOptionalParameterConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has an optional parameter of type
	///     <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<ConstructorInfo?, TParameter> HasOptionalParameter<TParameter>(
		this IThat<ConstructorInfo?> subject)
		=> subject.HasParameter<TParameter>().WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has an optional parameter of type
	///     <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<ConstructorInfo?, object?> HasOptionalParameter(
		this IThat<ConstructorInfo?> subject, Type parameterType)
		=> subject.HasParameter(parameterType).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has an optional parameter of type
	///     <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<ConstructorInfo?, TParameter> HasOptionalParameter<TParameter>(
		this IThat<ConstructorInfo?> subject, string expected)
		=> subject.HasParameter<TParameter>(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has an optional parameter of type
	///     <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<ConstructorInfo?, object?> HasOptionalParameter(
		this IThat<ConstructorInfo?> subject, Type parameterType, string expected)
		=> subject.HasParameter(parameterType, expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has an optional parameter with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<ConstructorInfo?, object?> HasOptionalParameter(
		this IThat<ConstructorInfo?> subject, string expected)
		=> subject.HasParameter(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has an optional parameter of exact type
	///     <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<ConstructorInfo?, TParameter> HasOptionalParameterExactly<TParameter>(
		this IThat<ConstructorInfo?> subject)
		=> subject.HasParameterExactly<TParameter>().WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has an optional parameter of exact type
	///     <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<ConstructorInfo?, object?> HasOptionalParameterExactly(
		this IThat<ConstructorInfo?> subject, Type parameterType)
		=> subject.HasParameterExactly(parameterType).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has an optional parameter of exact type
	///     <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<ConstructorInfo?, TParameter> HasOptionalParameterExactly<TParameter>(
		this IThat<ConstructorInfo?> subject, string expected)
		=> subject.HasParameterExactly<TParameter>(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has an optional parameter of exact type
	///     <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<ConstructorInfo?, object?> HasOptionalParameterExactly(
		this IThat<ConstructorInfo?> subject, Type parameterType, string expected)
		=> subject.HasParameterExactly(parameterType, expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	private sealed class HasOptionalParameterConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<ConstructorInfo?>(it, grammars),
			IValueConstraint<ConstructorInfo?>
	{
		public ConstraintResult IsMetBy(ConstructorInfo? actual)
		{
			Actual = actual;
			Outcome = actual?.GetParameters().Any(p => p.IsOptionalParameter()) == true
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has an optional parameter");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have an optional parameter");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
