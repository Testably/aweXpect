using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatField
{
	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> is read-only.
	/// </summary>
	public static AndOrResult<FieldInfo?, IThat<FieldInfo?>> IsReadOnly(
		this IThat<FieldInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsReadOnlyConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> is not read-only.
	/// </summary>
	public static AndOrResult<FieldInfo?, IThat<FieldInfo?>> IsNotReadOnly(
		this IThat<FieldInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsReadOnlyConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsReadOnlyConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<FieldInfo?>(it, grammars),
			IValueConstraint<FieldInfo?>
	{
		public ConstraintResult IsMetBy(FieldInfo? actual)
		{
			Actual = actual;
			Outcome = actual?.IsInitOnly == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is read-only");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was non-read-only ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not read-only");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was read-only ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
