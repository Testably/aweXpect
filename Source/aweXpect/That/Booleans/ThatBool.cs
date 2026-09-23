using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="bool" /> values.
/// </summary>
public static partial class ThatBool
{
	private sealed class IsEqualToConstraint(string it, ExpectationGrammars grammars, bool expected)
		: ConstraintResult.WithEqualToValue<bool>(it, grammars, false),
			IValueConstraint<bool>
	{
		public ConstraintResult IsMetBy(bool actual)
		{
			Actual = actual;
			Outcome = expected.Equals(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is ", "are "));
			Formatter.Format(stringBuilder, expected, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It);
			stringBuilder.Append(" was ");
			Formatter.Format(stringBuilder, Actual, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not ", "are not "));
			Formatter.Format(stringBuilder, expected, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
