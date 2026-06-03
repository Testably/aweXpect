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
	///     Verifies that the <see cref="PropertyInfo" /> overrides a base class property.
	/// </summary>
	public static AndOrResult<PropertyInfo?, IThat<PropertyInfo?>> Overrides(
		this IThat<PropertyInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new OverridesConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> does not override a base class property.
	/// </summary>
	public static AndOrResult<PropertyInfo?, IThat<PropertyInfo?>> DoesNotOverride(
		this IThat<PropertyInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new OverridesConstraint(it, grammars).Invert()),
			subject);

	private sealed class OverridesConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<PropertyInfo?>(it, grammars),
			IValueConstraint<PropertyInfo?>
	{
		public ConstraintResult IsMetBy(PropertyInfo? actual)
		{
			Actual = actual;
			Outcome = actual.IsOverride() ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("overrides a base property");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not override a base property ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not override a base property");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did override a base property ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
