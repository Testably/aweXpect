using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Reflection.Extensions;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatField
{
	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> has the <paramref name="expected" /> name.
	/// </summary>
	public static StringEqualityTypeResult<FieldInfo?, IThat<FieldInfo?>> HasName(
		this IThat<FieldInfo?> subject, string expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<FieldInfo?, IThat<FieldInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasNameConstraint(it, grammars, expected, options)),
			subject,
			options);
	}

	private sealed class HasNameConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		StringEqualityOptions options)
		: ConstraintResult.WithNotNullValue<FieldInfo?>(it, grammars),
			IValueConstraint<FieldInfo?>
	{
		public ConstraintResult IsMetBy(FieldInfo? actual)
		{
			Actual = actual;
			Outcome = options.AreConsideredEqual(actual?.Name, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has name ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExtendedFailure(It, Grammars, Actual?.Name, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have name ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
