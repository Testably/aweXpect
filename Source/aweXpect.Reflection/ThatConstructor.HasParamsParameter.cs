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
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="params" /> parameter.
	/// </summary>
	public static AndOrResult<ConstructorInfo?, IThat<ConstructorInfo?>> HasParamsParameter(
		this IThat<ConstructorInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasParamsParameterConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="params" /> parameter of type
	///     <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<ConstructorInfo?, TParameter> HasParamsParameter<TParameter>(
		this IThat<ConstructorInfo?> subject)
		=> subject.HasParameter<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="params" /> parameter of type
	///     <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<ConstructorInfo?, TParameter> HasParamsParameter<TParameter>(
		this IThat<ConstructorInfo?> subject, string expected)
		=> subject.HasParameter<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="params" /> parameter with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<ConstructorInfo?, object?> HasParamsParameter(
		this IThat<ConstructorInfo?> subject, string expected)
		=> subject.HasParameter(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="params" /> parameter of exact type
	///     <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<ConstructorInfo?, TParameter> HasParamsParameterExactly<TParameter>(
		this IThat<ConstructorInfo?> subject)
		=> subject.HasParameterExactly<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="params" /> parameter of exact type
	///     <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<ConstructorInfo?, TParameter> HasParamsParameterExactly<TParameter>(
		this IThat<ConstructorInfo?> subject, string expected)
		=> subject.HasParameterExactly<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	private sealed class HasParamsParameterConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<ConstructorInfo?>(it, grammars),
			IValueConstraint<ConstructorInfo?>
	{
		public ConstraintResult IsMetBy(ConstructorInfo? actual)
		{
			Actual = actual;
			Outcome = actual?.GetParameters().Any(p => p.IsParamsParameter()) == true
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has a params parameter");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have a params parameter");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
