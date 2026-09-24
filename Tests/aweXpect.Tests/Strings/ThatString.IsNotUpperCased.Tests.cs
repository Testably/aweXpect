namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed class IsNotUpperCased
	{
		public sealed class IncludingUncasedLettersTests
		{
			[Theory]
			[InlineData("STRAßE")]
			[InlineData("ı")]
			[InlineData("ﬁ")]
			[InlineData("𝐚")]
			public async Task WhenActualContainsLowerCaseLetterWithoutUpperCaseForm_ShouldSucceed(string subject)
			{
				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow()
					.Because("the negation is the exact complement of IsUpperCased().IncludingUncasedLetters()");
			}

			[Theory]
			[InlineData("ǅ")]
			[InlineData("ᾈ")]
			public async Task WhenActualContainsTitlecaseLetter_ShouldSucceed(string subject)
			{
				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow()
					.Because("a titlecase letter is not upper-cased, even where the runtime has no upper-case mapping for it");
			}

			[Fact]
			public async Task WhenActualIsDigitsAndPunctuation_ShouldFail()
			{
				string subject = "1-2, 3!";

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased including uncased letters,
					             but it was "1-2, 3!"
					             """);
			}

			[Fact]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased including uncased letters,
					             but it was ""
					             """);
			}

			[Fact]
			public async Task WhenActualIsLowerCased_ShouldSucceed()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased including uncased letters,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenActualIsSurrogatePairLowerCased_ShouldSucceed()
			{
				string subject = "𐐨";

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsSurrogatePairUpperCased_ShouldFail()
			{
				string subject = "𐐀";

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased including uncased letters,
					             but it was "𐐀"
					             """);
			}

			[Fact]
			public async Task WhenActualIsUpperCased_ShouldFail()
			{
				string subject = "ABC";

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased including uncased letters,
					             but it was "ABC"
					             """);
			}

			[Fact]
			public async Task WhenActualIsUpperCasedOrCaseless_ShouldFail()
			{
				string subject = "A漢字B";

				async Task Act()
					=> await That(subject).IsNotUpperCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased including uncased letters,
					             but it was "A漢字B"
					             """);
			}
		}

		public sealed class Tests
		{
			[Theory]
			[InlineData("STRAßE")]
			[InlineData("ı")]
			[InlineData("ﬁ")]
			[InlineData("𝐚")]
			public async Task WhenActualContainsLowerCaseLetterWithoutUpperCaseForm_ShouldFail(string subject)
			{
				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not upper-cased,
					              but it was "{subject}"
					              """)
					.Because("without IncludingUncasedLetters a letter without an upper-case form counts as upper-cased");
			}

			[Fact]
			public async Task WhenActualContainsTitlecaseLetter_ShouldFollowTheRuntimeCaseMapping()
			{
				string subject = "ǅ";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

#if NETFRAMEWORK
				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased,
					             but it was ""
					             """);
			}

			[Fact]
			public async Task WhenActualIsLowerCased_ShouldSucceed()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsMixedCased_ShouldSucceed()
			{
				string subject = "AbC";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsNotUpperCased_ShouldLimitDisplayedStringTo100Characters()
			{
				string subject = StringWithMoreThan100Characters.ToUpperInvariant();

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not upper-cased,
					              but it was "{StringWith100Characters.ToUpperInvariant()}…"
					              """);
			}

			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenActualIsSurrogatePairLowerCased_ShouldSucceed()
			{
				string subject = "𐐨";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsSurrogatePairUpperCased_ShouldFail()
			{
				string subject = "𐐀";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased,
					             but it was "𐐀"
					             """);
			}

			[Fact]
			public async Task WhenActualIsUpperCased_ShouldFail()
			{
				string subject = "ABC";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased,
					             but it was "ABC"
					             """);
			}

			[Fact]
			public async Task WhenActualIsUpperCasedOrCaseless_ShouldFail()
			{
				string subject = "A漢字B";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased,
					             but it was "A漢字B"
					             """);
			}

			[Fact]
			public async Task WhenActualIsUpperCasedOrSpecialCharacters_ShouldFail()
			{
				string subject = "A-B-C!";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased,
					             but it was "A-B-C!"
					             """);
			}

			[Fact]
			public async Task WhenActualIsWhitespace_ShouldSucceed()
			{
				string subject = " \t ";

				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not upper-cased,
					             but it was " \t "
					             """);
			}
		}
	}
}
