using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatEvent
{
	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> is static.
	/// </summary>
	public static AndOrResult<EventInfo?, IThat<EventInfo?>> IsStatic(
		this IThat<EventInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsStaticConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> is not static.
	/// </summary>
	public static AndOrResult<EventInfo?, IThat<EventInfo?>> IsNotStatic(
		this IThat<EventInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsStaticConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsStaticConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<EventInfo?>(it, grammars),
			IValueConstraint<EventInfo?>
	{
		public ConstraintResult IsMetBy(EventInfo? actual)
		{
			Actual = actual;
			Outcome = actual?.IsReallyStatic() == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was non-static ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not static");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was static ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
