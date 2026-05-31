using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatProperty
{
	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> is readable (can be read).
	/// </summary>
	public static AndOrResult<PropertyInfo?, IThat<PropertyInfo?>> IsReadable(
		this IThat<PropertyInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsReadableConstraint(it, grammars)),
			subject);

	private sealed class IsReadableConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<PropertyInfo?>(it, grammars),
			IValueConstraint<PropertyInfo?>
	{
		public ConstraintResult IsMetBy(PropertyInfo? actual)
		{
			Actual = actual;
			Outcome = actual.IsReadable() ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is readable");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was not readable ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not readable");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was readable ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
