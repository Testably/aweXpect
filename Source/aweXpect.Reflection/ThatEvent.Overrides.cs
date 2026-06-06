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
	///     Verifies that the <see cref="EventInfo" /> overrides a base class event.
	/// </summary>
	public static AndOrResult<EventInfo?, IThat<EventInfo?>> Overrides(
		this IThat<EventInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new OverridesConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> does not override a base class event.
	/// </summary>
	public static AndOrResult<EventInfo?, IThat<EventInfo?>> DoesNotOverride(
		this IThat<EventInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new OverridesConstraint(it, grammars).Invert()),
			subject);

	private sealed class OverridesConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<EventInfo?>(it, grammars),
			IValueConstraint<EventInfo?>
	{
		public ConstraintResult IsMetBy(EventInfo? actual)
		{
			Actual = actual;
			Outcome = actual.IsOverride() ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("overrides a base event");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not override a base event ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not override a base event");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did override a base event ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
