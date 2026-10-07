namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed class IsNotUpperCased
	{
		public sealed class IncludingUncasedLettersTests
		{
			[Test]
			[Arguments("STRAßE")]
			[Arguments("ı")]
			[Arguments("ﬁ")]
			[Arguments("𝐚")]
			public async Task WhenActualContainsLowerCaseLetterWithoutUpperCaseForm_ShouldSucceed(string subject)
			{
				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow()
					.Because("the negation is the exact complement of IsUpperCased().IncludingUncasedLetters()");
			}

			[Test]
			[Arguments("ǅ")]
			[Arguments("ᾈ")]
			public async Task WhenActualContainsTitlecaseLetter_ShouldSucceed(string subject)
			{
				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow()
					.Because("a titlecase letter is not upper-cased, even where the runtime has no upper-case mapping for it");
			}

			[Test]
			public async Task WhenActualIsDigitsAndPunctuation_ShouldFail()
			{
				string subject = "1-2, 3!";

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased including uncased letters,
					             but it was "1-2, 3!"
					             """);
			}

			[Test]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased including uncased letters,
					             but it was ""
					             """);
			}

			[Test]
			public async Task WhenActualIsLowerCased_ShouldSucceed()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased including uncased letters,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenActualIsSurrogatePairLowerCased_ShouldSucceed()
			{
				string subject = "𐐨";

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsSurrogatePairUpperCased_ShouldFail()
			{
				string subject = "𐐀";

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased including uncased letters,
					             but it was "𐐀"
					             """);
			}

			[Test]
			public async Task WhenActualIsUpperCased_ShouldFail()
			{
				string subject = "ABC";

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased including uncased letters,
					             but it was "ABC"
					             """);
			}

			[Test]
			public async Task WhenActualIsUpperCasedOrCaseless_ShouldFail()
			{
				string subject = "A漢字B";

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased including uncased letters,
					             but it was "A漢字B"
					             """);
			}

			[Test]
			public async Task WhenFalseIsSpecified_ShouldFailForLowerCaseLetterWithoutUpperCaseForm()
			{
				string subject = "STRAßE";

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters(false);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased,
					             but it was "STRAßE"
					             """)
					.Because("a letter without an upper-case form counts as upper-cased, as without the option");
			}
		}

		public sealed class Tests
		{
			[Test]
			[Arguments("STRAßE")]
			[Arguments("ı")]
			[Arguments("ﬁ")]
			[Arguments("𝐚")]
			public async Task WhenActualContainsLowerCaseLetterWithoutUpperCaseForm_ShouldFail(string subject)
			{
				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not upper-cased,
					              but it was "{subject}"
					              """)
					.Because("without IncludingUncasedLetters a letter without an upper-case form counts as upper-cased");
			}

			[Test]
			public async Task WhenActualContainsTitlecaseLetter_ShouldFollowTheRuntimeCaseMapping()
			{
				string subject = "ǅ";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

#if NETFRAMEWORK
				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased,
					             but it was "ǅ"
					             """)
					.Because("the invariant culture of .NET Framework has no upper-case mapping for ǅ");
#else
				await That(Act).DoesNotThrow()
					.Because("the invariant culture of .NET maps ǅ to Ǆ");
#endif
			}

			[Test]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased,
					             but it was ""
					             """);
			}

			[Test]
			public async Task WhenActualIsLowerCased_ShouldSucceed()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsMixedCased_ShouldSucceed()
			{
				string subject = "AbC";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsNotUpperCased_ShouldLimitDisplayedStringTo100Characters()
			{
				string subject = StringWithMoreThan100Characters.ToUpperInvariant();

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not upper-cased,
					              but it was "{StringWith100Characters.ToUpperInvariant()}…"
					              """);
			}

			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenActualIsSurrogatePairLowerCased_ShouldSucceed()
			{
				string subject = "𐐨";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsSurrogatePairUpperCased_ShouldFail()
			{
				string subject = "𐐀";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased,
					             but it was "𐐀"
					             """);
			}

			[Test]
			public async Task WhenActualIsUpperCased_ShouldFail()
			{
				string subject = "ABC";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased,
					             but it was "ABC"
					             """);
			}

			[Test]
			public async Task WhenActualIsUpperCasedOrCaseless_ShouldFail()
			{
				string subject = "A漢字B";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased,
					             but it was "A漢字B"
					             """);
			}

			[Test]
			public async Task WhenActualIsUpperCasedOrSpecialCharacters_ShouldFail()
			{
				string subject = "A-B-C!";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased,
					             but it was "A-B-C!"
					             """);
			}

			[Test]
			public async Task WhenActualIsWhitespace_ShouldFail()
			{
				string subject = " \t ";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased,
					             but it was " \t "
					             """);
			}
		}
	}
}
