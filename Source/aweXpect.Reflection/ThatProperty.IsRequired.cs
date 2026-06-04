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
	///     Verifies that the <see cref="PropertyInfo" /> is required.
	/// </summary>
	public static AndOrResult<PropertyInfo?, IThat<PropertyInfo?>> IsRequired(
		this IThat<PropertyInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsRequiredConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> is not required.
	/// </summary>
	public static AndOrResult<PropertyInfo?, IThat<PropertyInfo?>> IsNotRequired(
		this IThat<PropertyInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsRequiredConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsRequiredConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<PropertyInfo?>(it, grammars),
			IValueConstraint<PropertyInfo?>
	{
		public ConstraintResult IsMetBy(PropertyInfo? actual)
		{
			Actual = actual;
			Outcome = actual.IsRequired() ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is required");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was non-required ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not required");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was required ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
