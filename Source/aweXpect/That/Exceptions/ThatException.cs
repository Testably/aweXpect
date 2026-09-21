using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="Exception" /> values.
/// </summary>
public static partial class ThatException
{
	private sealed class HasInnerExceptionValueConstraint(
		Type innerExceptionType,
		string it,
		ExpectationGrammars grammars,
		bool hasMemberExpectations = false)
		: ConstraintResult.WithNotNullValue<Exception>(it, grammars),
			IValueConstraint<Exception?>
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(Exception? actual)
		{
			Actual = actual;
			if (hasMemberExpectations && actual?.InnerException is null)
			{
				// Expectations on a missing inner exception could only repeat that there is nothing to inspect.
				FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
			}

			Outcome = innerExceptionType.IsAssignableFrom(actual?.InnerException?.GetType())
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("has an inner ");
			if (innerExceptionType == typeof(Exception))
			{
				stringBuilder.Append("exception");
			}
			else
			{
				Formatter.Format(stringBuilder, innerExceptionType);
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual!.InnerException is null)
			{
				stringBuilder.Append(It).Append(" had no inner exception");
			}
			else
			{
				stringBuilder.Append(It).Append(" had ");
				stringBuilder.Append(Actual.InnerException.FormatForMessage(indentation, "inner "));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("does not have an inner ");
			if (innerExceptionType == typeof(Exception))
			{
				stringBuilder.Append("exception");
			}
			else
			{
				Formatter.Format(stringBuilder, innerExceptionType);
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had ")
				.Append(Actual!.InnerException!.FormatForMessage(indentation, "inner "));
	}
}
