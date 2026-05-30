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
	///     Verifies that the <see cref="MethodInfo" /> has an <see langword="out" /> parameter.
	/// </summary>
	public static AndOrResult<MethodInfo?, IThat<MethodInfo?>> HasOutParameter(
		this IThat<MethodInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasOutParameterConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has an <see langword="out" /> parameter of type
	///     <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<MethodInfo?, TParameter> HasOutParameter<TParameter>(
		this IThat<MethodInfo?> subject)
		=> subject.HasParameter<TParameter>().WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has an <see langword="out" /> parameter of type
	///     <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<MethodInfo?, TParameter> HasOutParameter<TParameter>(
		this IThat<MethodInfo?> subject, string expected)
		=> subject.HasParameter<TParameter>(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has an <see langword="out" /> parameter with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<MethodInfo?, object?> HasOutParameter(
		this IThat<MethodInfo?> subject, string expected)
		=> subject.HasParameter(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has an <see langword="out" /> parameter of exact type
	///     <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<MethodInfo?, TParameter> HasOutParameterExactly<TParameter>(
		this IThat<MethodInfo?> subject)
		=> subject.HasParameterExactly<TParameter>().WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has an <see langword="out" /> parameter of exact type
	///     <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<MethodInfo?, TParameter> HasOutParameterExactly<TParameter>(
		this IThat<MethodInfo?> subject, string expected)
		=> subject.HasParameterExactly<TParameter>(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	private sealed class HasOutParameterConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<MethodInfo?>(it, grammars),
			IValueConstraint<MethodInfo?>
	{
		public ConstraintResult IsMetBy(MethodInfo? actual)
		{
			Actual = actual;
			Outcome = actual?.GetParameters().Any(p => p.IsOutParameter()) == true
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has an out parameter");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have an out parameter");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
