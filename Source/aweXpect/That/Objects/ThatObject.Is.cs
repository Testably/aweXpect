using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatObject
{
	/// <summary>
	///     Verifies that the subject is of type <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<T, IThat<T?>> Is<T>(
		this IThat<T?> subject,
		Type type)
		where T : class
	{
		type.ThrowIfNull();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<T, IThat<T?>>(expectationBuilder.AddConstraint((it, grammars)
				=> new IsOfTypeConstraint(expectationBuilder, it, grammars, type)),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is not of type <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<T, IThat<T?>> IsNot<T>(
		this IThat<T?> subject,
		Type type)
		where T : class
	{
		type.ThrowIfNull();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<T, IThat<T?>>(expectationBuilder.AddConstraint((it, grammars)
				=> new IsOfTypeConstraint(expectationBuilder, it, grammars, type).Invert()),
			subject);
	}

	private sealed class IsOfTypeConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Type type)
		: ConstraintResult.WithNotNullValue<object>(it, grammars),
			IValueConstraint<object?>
	{
		public ConstraintResult IsMetBy(object? actual)
		{
			Actual = actual;
			Outcome = type.IsOrImplements(actual) ? Outcome.Success : Outcome.Failure;
			if (Outcome == Outcome.Failure && actual is not null)
			{
				expectationBuilder.AddContext(new ResultContext.Fixed("Actual",
					Formatter.Format(actual, FormattingOptions.MultipleLines)));
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is of type ", "are of type "));
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual!.GetType());
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not of type ", "are not of type "));
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
