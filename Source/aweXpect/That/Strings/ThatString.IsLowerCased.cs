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
	///     Verifies that all cased characters in the subject are lower-case.
	/// </summary>
	/// <remarks>
	///     That is, that the string could be the result of a call to <see cref="string.ToLowerInvariant()" />.
	///     Letters without a lower-case form count as lower-cased, see
	///     <see cref="CasingResult{TType, TThat}.IncludingUncasedLetters" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static CasingResult<string?, IThat<string?>> IsLowerCased(
		this IThat<string?> subject)
	{
		CasingOptions options = new();
		return new CasingResult<string?, IThat<string?>>(subject.Get().ExpectationBuilder.AddConstraint(
				(it, grammars) => new IsLowerCasedConstraint(it, grammars, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that of all cased characters in the subject at least one is upper-case.
	/// </summary>
	/// <remarks>
	///     That is, that the string could not be the result of a call to <see cref="string.ToLowerInvariant()" />.
	///     Letters without a lower-case form count as lower-cased, see
	///     <see cref="CasingResult{TType, TThat}.IncludingUncasedLetters" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static CasingResult<string, IThat<string?>> IsNotLowerCased(
		this IThat<string?> subject)
	{
		CasingOptions options = new();
		return new CasingResult<string, IThat<string?>>(subject.Get().ExpectationBuilder.AddConstraint(
				(it, grammars) => new IsLowerCasedConstraint(it, grammars, options).Invert()),
			subject,
			options);
	}

	private sealed class IsLowerCasedConstraint(string it, ExpectationGrammars grammars, CasingOptions options)
		: ConstraintResult.WithNotNullValue<string?>(it, grammars),
			IValueConstraint<string?>
	{
		public ConstraintResult IsMetBy(string? actual)
		{
			Actual = actual;
			Outcome = actual != null && actual == actual.ToLowerInvariant() &&
			          !(options.IncludesUncasedLetters &&
			            actual.ContainsCharacterOfCategory(UnicodeCategory.UppercaseLetter,
				            UnicodeCategory.TitlecaseLetter))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is lower-cased", "are lower-cased")).Append(options);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual, FormattingOptions.SingleLine);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not lower-cased", "are not lower-cased")).Append(options);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
