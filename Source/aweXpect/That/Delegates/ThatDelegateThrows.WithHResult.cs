using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Delegates;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDelegateThrows
{
	/// <summary>
	///     Verifies that the actual <see cref="Exception" /> has an <paramref name="expected" /> HResult.
	/// </summary>
	public static AndOrResult<TException, ThatDelegateThrows<TException>> WithHResult<TException>(
		this ThatDelegateThrows<TException> subject,
		int expected)
		where TException : Exception?
		=> new(subject.ExpectationBuilder.AddConstraint((it, grammars)
				=> new WithHResultConstraint(
					it,
					grammars | ExpectationGrammars.Active | ExpectationGrammars.Nested,
					expected)),
			subject);

	private sealed class WithHResultConstraint(
		string it,
		ExpectationGrammars grammars,
		int expected)
		: ConstraintResult.WithNotNullValue<Exception>(it, grammars),
			IValueConstraint<Exception?>
	{
		public ConstraintResult IsMetBy(Exception? actual)
		{
			Actual = actual;
			Outcome = actual?.HResult == expected ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Active))
			{
				stringBuilder.Append("with HResult ");
			}
			else if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("HResult is ");
			}
			else
			{
				stringBuilder.Append("has HResult ");
			}

			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had HResult ");
			Formatter.Format(stringBuilder, Actual?.HResult);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("without");
			}
			else
			{
				stringBuilder.Append("does not have");
			}

			stringBuilder.Append(" HResult ");
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");
	}
}
