using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatException
{
	/// <summary>
	///     Verifies that the actual <see cref="Exception" /> has an <paramref name="expected" /> HResult.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TException, IThat<TException>> HasHResult<TException>(
		this IThat<TException> source,
		int expected)
		where TException : Exception?
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasHResultValueConstraint(it, grammars, expected)),
			source);

	internal class HasHResultValueConstraint(
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
