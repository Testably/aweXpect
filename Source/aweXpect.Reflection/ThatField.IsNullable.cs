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
	///     Verifies that the <see cref="FieldInfo" /> is nullable.
	/// </summary>
	/// <remarks>
	///     A field is considered nullable if its type is a <see cref="System.Nullable{T}" /> value type or a
	///     reference type annotated as nullable (according to the nullable reference type metadata).
	/// </remarks>
	public static AndOrResult<FieldInfo?, IThat<FieldInfo?>> IsNullable(
		this IThat<FieldInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsNullableConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> is not nullable.
	/// </summary>
	/// <remarks>
	///     A field is considered nullable if its type is a <see cref="System.Nullable{T}" /> value type or a
	///     reference type annotated as nullable (according to the nullable reference type metadata).
	///     Fields without nullability annotations (oblivious code) count as non-nullable.
	/// </remarks>
	public static AndOrResult<FieldInfo?, IThat<FieldInfo?>> IsNotNullable(
		this IThat<FieldInfo?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsNullableConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsNullableConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<FieldInfo?>(it, grammars),
			IValueConstraint<FieldInfo?>
	{
		public ConstraintResult IsMetBy(FieldInfo? actual)
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
