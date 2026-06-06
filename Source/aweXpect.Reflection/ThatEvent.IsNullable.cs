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
	///     Verifies that the <see cref="EventInfo" /> is nullable.
	/// </summary>
	/// <remarks>
	///     An event is considered nullable if its handler type is annotated as nullable
	///     (according to the nullable reference type metadata).
	/// </remarks>
	public static AndOrResult<EventInfo?, IThat<EventInfo?>> IsNullable(
		this IThat<EventInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsNullableConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> is not nullable.
	/// </summary>
	/// <remarks>
	///     An event is considered nullable if its handler type is annotated as nullable
	///     (according to the nullable reference type metadata).
	///     Events without nullability annotations (oblivious code) count as non-nullable.
	/// </remarks>
	public static AndOrResult<EventInfo?, IThat<EventInfo?>> IsNotNullable(
		this IThat<EventInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsNullableConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsNullableConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<EventInfo?>(it, grammars),
			IValueConstraint<EventInfo?>
	{
		public ConstraintResult IsMetBy(EventInfo? actual)
		{
			Actual = actual;
			Outcome = actual.IsNullable() ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is nullable");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was non-nullable ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not nullable");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was nullable ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
