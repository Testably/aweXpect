namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed class IsNotLowerCased
	{
		public sealed class IncludingUncasedLettersTests
		{
			[Theory]
			[InlineData("ǅ")]
			[InlineData("ᾈ")]
			public async Task WhenActualContainsTitlecaseLetter_ShouldSucceed(string subject)
			{
				async Task Act()
					=> await That(subject).IsNotLowerCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow()
					.Because("a titlecase letter is not lower-cased, even where the runtime has no lower-case mapping for it");
			}

			[Theory]
			[InlineData("İstanbul")]
			[InlineData("ℂ")]
			[InlineData("ϒ")]
			[InlineData("𝐀")]
			public async Task WhenActualContainsUpperCaseLetterWithoutLowerCaseForm_ShouldSucceed(string subject)
			{
				async Task Act()
					=> await That(subject).IsNotLowerCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow()
					.Because("the negation is the exact complement of IsLowerCased().IncludingUncasedLetters()");
			}

			[Fact]
			public async Task WhenActualIsDigitsAndPunctuation_ShouldFail()
			{
				string subject = "1-2, 3!";

				async Task Act()
					=> await That(subject).IsNotLowerCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not lower-cased including uncased letters,
					             but it was "1-2, 3!"
					             """);
			}

			[Fact]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsNotLowerCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not lower-cased including uncased letters,
					             but it was ""
					             """);
			}

			[Fact]
			public async Task WhenActualIsLowerCased_ShouldFail()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsNotLowerCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not lower-cased including uncased letters,
					             but it was "abc"
					             """);
			}

			[Fact]
			public async Task WhenActualIsLowerCasedOrCaseless_ShouldFail()
			{
				string subject = "a漢字b";

				async Task Act()
					=> await That(subject).IsNotLowerCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not lower-cased including uncased letters,
					             but it was "a漢字b"
					             """);
			}

			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotLowerCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not lower-cased including uncased letters,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenActualIsSurrogatePairLowerCased_ShouldFail()
			{
				string subject = "𐐨";

				async Task Act()
					=> await That(subject).IsNotLowerCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not lower-cased including uncased letters,
					             but it was "𐐨"
					             """);
			}

			[Fact]
			public async Task WhenActualIsSurrogatePairUpperCased_ShouldSucceed()
			{
				string subject = "𐐀";

				async Task Act()
					=> await That(subject).IsNotLowerCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsUpperCased_ShouldSucceed()
			{
				string subject = "ABC";

				async Task Act()
					=> await That(subject).IsNotLowerCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Fact]
			public async Task WhenActualContainsTitlecaseLetter_ShouldFollowTheRuntimeCaseMapping()
			{
				string subject = "ǅ";

				async Task Act()
					=> await That(subject).IsNotLowerCased();

#if NETFRAMEWORK
				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not lower-cased,
					             but it was "ǅ"
					             """)
					.Because("the invariant culture of .NET Framework has no lower-case mapping for ǅ");
#else
				await That(Act).DoesNotThrow()
					.Because("the invariant culture of .NET maps ǅ to ǆ");
#endif
			}

			[Theory]
			[InlineData("İstanbul")]
			[InlineData("ℂ")]
			[InlineData("ϒ")]
			[InlineData("𝐀")]
			public async Task WhenActualContainsUpperCaseLetterWithoutLowerCaseForm_ShouldFail(string subject)
			{
				async Task Act()
					=> await That(subject).IsNotLowerCased();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not lower-cased,
					              but it was "{subject}"
					              """)
					.Because("without IncludingUncasedLetters a letter without a lower-case form counts as lower-cased");
			}

			[Fact]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsNotLowerCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not lower-cased,
					             but it was ""
					             """);
			}

			[Fact]
			public async Task WhenActualIsLowerCased_ShouldFail()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsNotLowerCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not lower-cased,
					             but it was "abc"
					             """);
			}

			[Fact]
			public async Task WhenActualIsLowerCasedOrCaseless_ShouldFail()
			{
				string subject = "a漢字b";

				async Task Act()
					=> await That(subject).IsNotLowerCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not lower-cased,
					             but it was "a漢字b"
					             """);
			}

			[Fact]
			public async Task WhenActualIsLowerCasedOrSpecialCharacters_ShouldFail()
			{
				string subject = "a-b-c!";

				async Task Act()
					=> await That(subject).IsNotLowerCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not lower-cased,
					             but it was "a-b-c!"
					             """);
			}

			[Fact]
			public async Task WhenActualIsMixedCased_ShouldSucceed()
			{
				string subject = "aBc";

				async Task Act()
					=> await That(subject).IsNotLowerCased();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsNotLowerCased_ShouldLimitDisplayedStringTo100Characters()
			{
				string subject = StringWithMoreThan100Characters.ToLowerInvariant();

				async Task Act()
					=> await That(subject).IsNotLowerCased();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not lower-cased,
					              but it was "{StringWith100Characters.ToLowerInvariant()}…"
					              """);
			}

			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotLowerCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not lower-cased,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenActualIsSurrogatePairLowerCased_ShouldFail()
			{
				string subject = "𐐨";

				async Task Act()
					=> await That(subject).IsNotLowerCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not lower-cased,
					             but it was "𐐨"
					             """);
			}

			[Fact]
			public async Task WhenActualIsSurrogatePairUpperCased_ShouldSucceed()
			{
				string subject = "𐐀";

				async Task Act()
					=> await That(subject).IsNotLowerCased();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsUpperCased_ShouldSucceed()
			{
				string subject = "ABC";

				async Task Act()
					=> await That(subject).IsNotLowerCased();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsWhitespace_ShouldSucceed()
			{
				string subject = " \t ";

				async Task Act()
					=> await That(subject).IsNotLowerCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not lower-cased,
					             but it was " \t "
					             """);
			}
		}
	}
}
