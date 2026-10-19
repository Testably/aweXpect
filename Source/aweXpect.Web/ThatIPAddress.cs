using System;
using System.Net;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="IPAddress" /> values.
/// </summary>
public static partial class ThatIPAddress
{
	private sealed class IsKindConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<IPAddress, bool> predicate,
		string singular,
		string plural)
		: ConstraintResult.WithNotNullValue<IPAddress>(it, grammars),
			IValueConstraint<IPAddress?>
	{
		public ConstraintResult IsMetBy(IPAddress? actual)
		{
			Actual = actual;
			Outcome = actual is not null && predicate(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is ", "are ")).Append(Grammars.Verb(singular, plural));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not ", "are not ")).Append(Grammars.Verb(singular, plural));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
