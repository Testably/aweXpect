using System.Globalization;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatString
{
	/// <summary>
	///     Verifies that all cased characters in the subject are upper-case.
	/// </summary>
	/// <remarks>
	///     That is, that the string could be the result of a call to <see cref="string.ToUpperInvariant()" />.
	///     Letters without an upper-case form count as upper-cased, see
	///     <see cref="CasingResult{TType, TThat}.IncludingUncasedLetters" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static CasingResult<string?, IThat<string?>> IsUpperCased(
		this IThat<string?> subject)
	{
		CasingOptions options = new();
		return new CasingResult<string?, IThat<string?>>(subject.Get().ExpectationBuilder.AddConstraint(
				(it, grammars) => new IsUpperCasedConstraint(it, grammars, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that of all cased characters in the subject at least one is lower-case.
	/// </summary>
	/// <remarks>
	///     That is, that the string could not be the result of a call to <see cref="string.ToUpperInvariant()" />.
	///     Letters without an upper-case form count as upper-cased, see
	///     <see cref="CasingResult{TType, TThat}.IncludingUncasedLetters" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static CasingResult<string, IThat<string?>> IsNotUpperCased(
		this IThat<string?> subject)
	{
		CasingOptions options = new();
		return new CasingResult<string, IThat<string?>>(subject.Get().ExpectationBuilder.AddConstraint(
				(it, grammars) => new IsUpperCasedConstraint(it, grammars, options).Invert()),
			subject,
			options);
	}

	private sealed class IsUpperCasedConstraint(string it, ExpectationGrammars grammars, CasingOptions options)
		: ConstraintResult.WithNotNullValue<string?>(it, grammars),
			IValueConstraint<string?>
	{
		public ConstraintResult IsMetBy(string? actual)
		{
			Actual = actual;
			Outcome = actual != null && actual == actual.ToUpperInvariant() &&
			          !(options.IncludesUncasedLetters &&
			            actual.ContainsCharacterOfCategory(UnicodeCategory.LowercaseLetter,
				            UnicodeCategory.TitlecaseLetter))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is upper-cased", "are upper-cased")).Append(options);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual, FormattingOptions.SingleLine);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not upper-cased", "are not upper-cased")).Append(options);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
