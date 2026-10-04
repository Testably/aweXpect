using System.Diagnostics;
using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Results;

namespace aweXpect.Core;

/// <summary>
///     Wraps the <see cref="ExpectationBuilder" /> for a bool.
/// </summary>
/// <remarks>
///     Awaiting it without any expectation verifies that the subject is <see langword="true" />.
/// </remarks>
[DebuggerDisplay("ThatBoolSubject: {ExpectationBuilder}")]
public class ThatBoolSubject : ExpectationResult<bool>, IExpectThat<bool>
{
	/// <inheritdoc cref="ThatBoolSubject" />
	/// <remarks>
	///     Marks the <paramref name="expectationBuilder" /> instead of wrapping it in a second builder, because every
	///     <c>Expect.That(bool)</c> would otherwise allocate that builder.
	/// </remarks>
	public ThatBoolSubject(ExpectationBuilder expectationBuilder) : base(expectationBuilder)
	{
		expectationBuilder.IsTrueWithoutExpectations = true;
	}

	/// <inheritdoc cref="IExpectThat{T}.ExpectationBuilder" />
	ExpectationBuilder IExpectThat<bool>.ExpectationBuilder => ExpectationBuilder;

	internal sealed class IsTrueConstraint(ExpectationGrammars grammars)
		: ConstraintResult.WithEqualToValue<bool>("it", grammars, false),
			IValueConstraint<bool>
	{
		public ConstraintResult IsMetBy(bool actual)
		{
			Actual = actual;
			Outcome = true.Equals(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is ", "are "));
			Formatter.Format(stringBuilder, true, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It);
			stringBuilder.Append(" was ");
			Formatter.Format(stringBuilder, Actual, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not ", "are not "));
			Formatter.Format(stringBuilder, true, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It);
			stringBuilder.Append(" was");
		}
	}
}
