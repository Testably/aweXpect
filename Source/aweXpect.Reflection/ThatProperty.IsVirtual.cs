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
	///     Verifies that the <see cref="PropertyInfo" /> is virtual.
	/// </summary>
	public static AndOrResult<PropertyInfo?, IThat<PropertyInfo?>> IsVirtual(
		this IThat<PropertyInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsVirtualConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> is not virtual.
	/// </summary>
	public static AndOrResult<PropertyInfo?, IThat<PropertyInfo?>> IsNotVirtual(
		this IThat<PropertyInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsVirtualConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsVirtualConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<PropertyInfo?>(it, grammars),
			IValueConstraint<PropertyInfo?>
	{
		public ConstraintResult IsMetBy(PropertyInfo? actual)
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
