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
	///     Verifies that the <see cref="EventInfo" /> is virtual.
	/// </summary>
	public static AndOrResult<EventInfo?, IThat<EventInfo?>> IsVirtual(
		this IThat<EventInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsVirtualConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> is not virtual.
	/// </summary>
	public static AndOrResult<EventInfo?, IThat<EventInfo?>> IsNotVirtual(
		this IThat<EventInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsVirtualConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsVirtualConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<EventInfo?>(it, grammars),
			IValueConstraint<EventInfo?>
	{
		public ConstraintResult IsMetBy(EventInfo? actual)
		{
			Actual = actual;
			Outcome = actual.IsReallyVirtual() ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is virtual");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was non-virtual ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not virtual");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was virtual ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
