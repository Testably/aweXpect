namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class IsLowerCased
	{
		public sealed class IncludingUncasedLettersTests
		{
			[Theory]
			[InlineData("ǅ")]
			[InlineData("ᾈ")]
			public async Task WhenActualContainsTitlecaseLetter_ShouldFail(string subject)
			{
				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is lower-cased including uncased letters,
					              but it was "{subject}"
					              """)
					.Because("a titlecase letter is not lower-cased, even where the runtime has no lower-case mapping for it");
			}

			[Theory]
			[InlineData("İstanbul")]
			[InlineData("ℂ")]
			[InlineData("ϒ")]
			[InlineData("𝐀")]
			public async Task WhenActualContainsUpperCaseLetterWithoutLowerCaseForm_ShouldFail(string subject)
			{
				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is lower-cased including uncased letters,
					              but it was "{subject}"
					              """);
			}

			[Fact]
			public async Task WhenActualIsDigitsAndPunctuation_ShouldSucceed()
			{
				string subject = "1-2, 3!";

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsEmpty_ShouldSucceed()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsLowerCased_ShouldSucceed()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsLowerCasedOrCaseless_ShouldSucceed()
			{
				string subject = "a漢字b";

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased including uncased letters,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenActualIsSurrogatePairLowerCased_ShouldSucceed()
			{
				string subject = "𐐨";

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsSurrogatePairUpperCased_ShouldFail()
			{
				string subject = "𐐀";

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased including uncased letters,
					             but it was "𐐀"
					             """);
			}

			[Fact]
			public async Task WhenActualIsUpperCased_ShouldFail()
			{
				string subject = "ABC";

				async Task Act()
					=> await That(subject).IsLowerCased().IncludingUncasedLetters();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased including uncased letters,
					             but it was "ABC"
					             """);
			}
		}

		public sealed class Tests
		{
			[Fact]
			public async Task WhenActualContainsTitlecaseLetter_ShouldFollowTheRuntimeCaseMapping()
			{
				string subject = "ǅ";

				async Task Act()
					=> await That(subject).IsLowerCased();

#if NETFRAMEWORK
				await That(Act).DoesNotThrow()
					.Because("the invariant culture of .NET Framework has no lower-case mapping for ǅ");
#else
				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased,
					             but it was "ǅ"
					             """)
					.Because("the invariant culture of .NET maps ǅ to ǆ");
#endif
			}

			[Theory]
			[InlineData("İstanbul")]
			[InlineData("ℂ")]
			[InlineData("ϒ")]
			[InlineData("𝐀")]
			public async Task WhenActualContainsUpperCaseLetterWithoutLowerCaseForm_ShouldSucceed(string subject)
			{
				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).DoesNotThrow()
					.Because("without IncludingUncasedLetters a letter without a lower-case form counts as lower-cased");
			}

			[Fact]
			public async Task WhenActualIsEmpty_ShouldSucceed()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsLowerCased_ShouldSucceed()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsLowerCasedOrCaseless_ShouldSucceed()
			{
				string subject = "a漢字b";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsLowerCasedOrSpecialCharacters_ShouldSucceed()
			{
				string subject = "a-b-c!";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsMixedCased_ShouldFail()
			{
				string subject = "aBc";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased,
					             but it was "aBc"
					             """);
			}

			[Fact]
			public async Task WhenActualIsNotLowerCased_ShouldLimitDisplayedStringTo100Characters()
			{
				string subject = StringWithMoreThan100Characters;

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is lower-cased,
					              but it was "{StringWith100Characters}…"
					              """);
			}

			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenActualIsSurrogatePairLowerCased_ShouldSucceed()
			{
				string subject = "𐐨";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsSurrogatePairUpperCased_ShouldFail()
			{
				string subject = "𐐀";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased,
					             but it was "𐐀"
					             """);
			}

			[Fact]
			public async Task WhenActualIsUpperCased_ShouldFail()
			{
				string subject = "ABC";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is lower-cased,
					             but it was "ABC"
					             """);
			}

			[Fact]
			public async Task WhenActualIsWhitespace_ShouldSucceed()
			{
				string subject = " \t\r\n";

				async Task Act()
					=> await That(subject).IsLowerCased();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
