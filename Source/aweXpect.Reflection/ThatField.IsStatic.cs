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
	///     Verifies that the <see cref="FieldInfo" /> is static.
	/// </summary>
	public static AndOrResult<FieldInfo?, IThat<FieldInfo?>> IsStatic(
		this IThat<FieldInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsStaticConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> is not static.
	/// </summary>
	public static AndOrResult<FieldInfo?, IThat<FieldInfo?>> IsNotStatic(
		this IThat<FieldInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsStaticConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsStaticConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<FieldInfo?>(it, grammars),
			IValueConstraint<FieldInfo?>
	{
		public ConstraintResult IsMetBy(FieldInfo? actual)
		{
			Actual = actual;
			Outcome = actual?.IsStatic == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was non-static ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not static");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was static ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
