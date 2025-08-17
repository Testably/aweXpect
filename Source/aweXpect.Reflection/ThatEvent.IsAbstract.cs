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
	///     Verifies that the <see cref="EventInfo" /> is abstract.
	/// </summary>
	public static AndOrResult<EventInfo?, IThat<EventInfo?>> IsAbstract(
		this IThat<EventInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsAbstractConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> is not abstract.
	/// </summary>
	public static AndOrResult<EventInfo?, IThat<EventInfo?>> IsNotAbstract(
		this IThat<EventInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsAbstractConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsAbstractConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<EventInfo?>(it, grammars),
			IValueConstraint<EventInfo?>
	{
		public ConstraintResult IsMetBy(EventInfo? actual)
		{
			Actual = actual;
			Outcome = actual?.IsReallyAbstract() == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is abstract");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was non-abstract ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not abstract");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was abstract ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
