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
	///     Verifies that the <see cref="PropertyInfo" /> is sealed.
	/// </summary>
	public static AndOrResult<PropertyInfo?, IThat<PropertyInfo?>> IsSealed(
		this IThat<PropertyInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsSealedConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> is not sealed.
	/// </summary>
	public static AndOrResult<PropertyInfo?, IThat<PropertyInfo?>> IsNotSealed(
		this IThat<PropertyInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsSealedConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsSealedConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<PropertyInfo?>(it, grammars),
			IValueConstraint<PropertyInfo?>
	{
		public ConstraintResult IsMetBy(PropertyInfo? actual)
		{
			Actual = actual;
			Outcome = actual?.IsReallySealed() == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is sealed");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was non-sealed ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not sealed");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was sealed ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
