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
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter.
	/// </summary>
	public static AndOrResult<ConstructorInfo?, IThat<ConstructorInfo?>> HasRefParameter(
		this IThat<ConstructorInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasRefParameterConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter of type
	///     <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<ConstructorInfo?, TParameter> HasRefParameter<TParameter>(
		this IThat<ConstructorInfo?> subject)
		=> subject.HasParameter<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter of type
	///     <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<ConstructorInfo?, object?> HasRefParameter(
		this IThat<ConstructorInfo?> subject, Type parameterType)
		=> subject.HasParameter(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter of type
	///     <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<ConstructorInfo?, TParameter> HasRefParameter<TParameter>(
		this IThat<ConstructorInfo?> subject, string expected)
		=> subject.HasParameter<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter of type
	///     <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<ConstructorInfo?, object?> HasRefParameter(
		this IThat<ConstructorInfo?> subject, Type parameterType, string expected)
		=> subject.HasParameter(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<ConstructorInfo?, object?> HasRefParameter(
		this IThat<ConstructorInfo?> subject, string expected)
		=> subject.HasParameter(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter of exact type
	///     <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<ConstructorInfo?, TParameter> HasRefParameterExactly<TParameter>(
		this IThat<ConstructorInfo?> subject)
		=> subject.HasParameterExactly<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter of exact type
	///     <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<ConstructorInfo?, object?> HasRefParameterExactly(
		this IThat<ConstructorInfo?> subject, Type parameterType)
		=> subject.HasParameterExactly(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter of exact type
	///     <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<ConstructorInfo?, TParameter> HasRefParameterExactly<TParameter>(
		this IThat<ConstructorInfo?> subject, string expected)
		=> subject.HasParameterExactly<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter of exact type
	///     <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<ConstructorInfo?, object?> HasRefParameterExactly(
		this IThat<ConstructorInfo?> subject, Type parameterType, string expected)
		=> subject.HasParameterExactly(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	private sealed class HasRefParameterConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<ConstructorInfo?>(it, grammars),
			IValueConstraint<ConstructorInfo?>
	{
		public ConstraintResult IsMetBy(ConstructorInfo? actual)
		{
			Actual = actual;
			Outcome = actual?.GetParameters().Any(p => p.IsRefParameter()) == true
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has a ref parameter");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have a ref parameter");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
