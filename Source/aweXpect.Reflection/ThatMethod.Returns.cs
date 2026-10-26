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

public static partial class ThatMethod
{
	/// <summary>
	///     Verifies that the method returns type <typeparamref name="TReturn" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ReturnsResult<MethodInfo> Returns<TReturn>(
		this IThat<MethodInfo?> subject)
		=> Returns(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that the method returns type <paramref name="returnType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ReturnsResult<MethodInfo> Returns(
		this IThat<MethodInfo?> subject, Type returnType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.AddType(returnType, false);
		return new ReturnsResult<MethodInfo>(
			subject.Get().ExpectationBuilder.AddConstraint(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new ReturnsConstraint(it, grammars, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

	/// <summary>
	///     Verifies that the method does not return type <typeparamref name="TReturn" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<MethodInfo, IThat<MethodInfo?>> DoesNotReturn<TReturn>(
		this IThat<MethodInfo?> subject)
		=> DoesNotReturn(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that the method does not return type <paramref name="returnType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<MethodInfo, IThat<MethodInfo?>> DoesNotReturn(
		this IThat<MethodInfo?> subject, Type returnType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.AddType(returnType, false);
		return new AndOrResult<MethodInfo, IThat<MethodInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new ReturnsConstraint(it, grammars, typeFilterOptions).Invert()),
			subject);
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
			=> typeFilterOptions.AppendExpectation(stringBuilder, Grammars);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it returned ").Append(Formatter.Format(Actual?.ReturnType));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
