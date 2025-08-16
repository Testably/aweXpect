using System;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatMethod
{
	/// <summary>
	///     Verifies that the method returns type <typeparamref name="TReturn" />.
	/// </summary>
	public static MethodReturnResult<MethodInfo?, IThat<MethodInfo?>> Returns<TReturn>(
		this IThat<MethodInfo?> subject)
		=> Returns(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that the method returns type <paramref name="returnType" />.
	/// </summary>
	public static MethodReturnResult<MethodInfo?, IThat<MethodInfo?>> Returns(
		this IThat<MethodInfo?> subject, Type returnType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(returnType, false);
		return new MethodReturnResult<MethodInfo?, IThat<MethodInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new ReturnsConstraint(it, grammars, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

	/// <summary>
	///     Result that allows chaining additional return types for a single method.
	/// </summary>
	public sealed partial class MethodReturnResult<TValue, TResult>(
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
		///     Allow an alternative return type <typeparamref name="TReturn" />.
		/// </summary>
		public MethodReturnResult<TValue, TResult> OrReturns<TReturn>()
			=> OrReturns(typeof(TReturn));

		/// <summary>
		///     Allow an alternative return type <paramref name="returnType" />.
		/// </summary>
		public MethodReturnResult<TValue, TResult> OrReturns(Type returnType)
		{
			typeFilterOptions.RegisterType(returnType, false);
			return this;
		}
	}

	private sealed class ReturnsConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeFilterOptions typeFilterOptions)
		: ConstraintResult.WithNotNullValue<MethodInfo?>(it, grammars),
			IValueConstraint<MethodInfo?>
	{
		public ConstraintResult IsMetBy(MethodInfo? actual)
		{
			Actual = actual;
			Outcome = typeFilterOptions.Matches(actual?.ReturnType)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> typeFilterOptions.AppendDescription(stringBuilder, Grammars);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it returned ").Append(Formatter.Format(Actual?.ReturnType));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it did");
	}
}
