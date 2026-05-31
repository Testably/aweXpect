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

public static partial class ThatMethod
{
	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has an <see langword="in" /> parameter.
	/// </summary>
	public static AndOrResult<MethodInfo?, IThat<MethodInfo?>> HasInParameter(
		this IThat<MethodInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasInParameterConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has an <see langword="in" /> parameter of type
	///     <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<MethodInfo?, TParameter> HasInParameter<TParameter>(
		this IThat<MethodInfo?> subject)
		=> subject.HasParameter<TParameter>().WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has an <see langword="in" /> parameter of type
	///     <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<MethodInfo?, object?> HasInParameter(
		this IThat<MethodInfo?> subject, Type parameterType)
		=> subject.HasParameter(parameterType).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has an <see langword="in" /> parameter of type
	///     <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<MethodInfo?, TParameter> HasInParameter<TParameter>(
		this IThat<MethodInfo?> subject, string expected)
		=> subject.HasParameter<TParameter>(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has an <see langword="in" /> parameter of type
	///     <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<MethodInfo?, object?> HasInParameter(
		this IThat<MethodInfo?> subject, Type parameterType, string expected)
		=> subject.HasParameter(parameterType, expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has an <see langword="in" /> parameter with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<MethodInfo?, object?> HasInParameter(
		this IThat<MethodInfo?> subject, string expected)
		=> subject.HasParameter(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has an <see langword="in" /> parameter of exact type
	///     <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<MethodInfo?, TParameter> HasInParameterExactly<TParameter>(
		this IThat<MethodInfo?> subject)
		=> subject.HasParameterExactly<TParameter>().WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has an <see langword="in" /> parameter of exact type
	///     <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<MethodInfo?, object?> HasInParameterExactly(
		this IThat<MethodInfo?> subject, Type parameterType)
		=> subject.HasParameterExactly(parameterType).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has an <see langword="in" /> parameter of exact type
	///     <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<MethodInfo?, TParameter> HasInParameterExactly<TParameter>(
		this IThat<MethodInfo?> subject, string expected)
		=> subject.HasParameterExactly<TParameter>(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has an <see langword="in" /> parameter of exact type
	///     <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<MethodInfo?, object?> HasInParameterExactly(
		this IThat<MethodInfo?> subject, Type parameterType, string expected)
		=> subject.HasParameterExactly(parameterType, expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	private sealed class HasInParameterConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<MethodInfo?>(it, grammars),
			IValueConstraint<MethodInfo?>
	{
		public ConstraintResult IsMetBy(MethodInfo? actual)
		{
			Actual = actual;
			Outcome = actual?.GetParameters().Any(p => p.IsInParameter()) == true
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has an in parameter");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have an in parameter");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
