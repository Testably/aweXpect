using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatException
{
	/// <summary>
	///     Verifies that the actual <see cref="ArgumentException" /> has an <paramref name="expected" /> param name.
	/// </summary>
	/// <remarks>
	///     If <paramref name="expected" /> is <see langword="null" />, does not verify anything.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TException, IThat<TException>> HasParamName<TException>(
		this IThat<TException> source,
		string? expected)
		where TException : ArgumentException?
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasParamNameValueConstraint<TException>(it, grammars, expected)),
			source);

	internal class HasParamNameValueConstraint<TArgumentException>(
		string it,
		ExpectationGrammars grammars,
		string? expected)
		: ConstraintResult.WithNotNullValue<Exception?>(it, grammars),
			IValueConstraint<Exception?>
		where TArgumentException : ArgumentException?
	{
		public ConstraintResult IsMetBy(Exception? actual)
		{
			Actual = actual;
			Outcome = actual is TArgumentException argumentException &&
			          (expected is null || argumentException.ParamName == expected)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				if (Grammars.HasFlag(ExpectationGrammars.Active))
				{
					stringBuilder.Append("with a ParamName");
				}
				else if (Grammars.HasFlag(ExpectationGrammars.Nested))
				{
					stringBuilder.Append("ParamName exists");
				}
				else
				{
					stringBuilder.Append("has a ParamName");
				}
			}
			else
			{
				if (Grammars.HasFlag(ExpectationGrammars.Active))
				{
					stringBuilder.Append("with ParamName ");
				}
				else if (Grammars.HasFlag(ExpectationGrammars.Nested))
				{
					stringBuilder.Append("ParamName is ");
				}
				else
				{
					stringBuilder.Append("has ParamName ");
				}

				Formatter.Format(stringBuilder, expected);
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is TArgumentException argumentException)
			{
				stringBuilder.Append(It).Append(" had ParamName ");
				Formatter.Format(stringBuilder, argumentException.ParamName);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				if (Grammars.HasFlag(ExpectationGrammars.Active))
				{
					stringBuilder.Append("without a ParamName");
				}
				else if (Grammars.HasFlag(ExpectationGrammars.Nested))
				{
					stringBuilder.Append("ParamName does not exist");
				}
				else
				{
					stringBuilder.Append("does not have a ParamName");
				}
			}
			else
			{
				if (Grammars.HasFlag(ExpectationGrammars.Active))
				{
					stringBuilder.Append("without ParamName ");
				}
				else if (Grammars.HasFlag(ExpectationGrammars.Nested))
				{
					stringBuilder.Append("ParamName is not ");
				}
				else
				{
					stringBuilder.Append("does not have ParamName ");
				}

				Formatter.Format(stringBuilder, expected);
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");
	}
}
